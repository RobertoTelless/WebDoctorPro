using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using EntitiesServices.WorkClasses;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using iTextSharp.text;
using iTextSharp.text.pdf;
using XidNet;
using CrossCutting;
using System.Collections;
using System.Threading.Tasks;
using DataServices.Repositories;
using System.Text;
using System.Net;
using System.Net.Mime;
using ERP_Condominios_Solution.Controllers;
using Microsoft.Ajax.Utilities;
using System.Net.Mail;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace GEDSys_Presentation.Controllers
{
    public class MedicoController : Controller
    {
        private readonly IMedicoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IPacienteAppService pacApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly ITemplateEMailAppService temApp;

        private String msg;
        private Exception exception;
        MEDICOS objeto = new MEDICOS();
        MEDICOS objetoAntes = new MEDICOS();
        List<MEDICOS> listaMaster = new List<MEDICOS>();
        List<MEDICOS_MENSAGEM> listaMasterTexto = new List<MEDICOS_MENSAGEM>();
        MEDICOS_MENSAGEM objetoTexto = new MEDICOS_MENSAGEM();
        String extensao;

        public MedicoController(IMedicoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, IPacienteAppService pacApps, IMensagemEnviadaSistemaAppService meApps, ITemplateEMailAppService temApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            aceApp = aceApps;
            pacApp = pacApps;
            meApp = meApps;
            temApp = temApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return View();
        }

        [HttpGet]
        public ActionResult MontarTelaMedico()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medico";

                // Carrega listas
                if (Session["ListaMedico"] == null)
                {
                    listaMaster = CarregarMedico().OrderBy(p => p.MEDC_NM_MEDICO).ToList();
                    Session["ListaMedico"] = listaMaster;
                }
                ViewBag.Listas = (List<MEDICOS>)Session["ListaMedico"];
                ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
                Session["Medico"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensMedico"] != null)
                {
                    if ((Int32)Session["MensMedico"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensMedico"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO", "Medico", "MontarTelaMedico");

                // Abre view
                Session["MensMedico"] = null;
                Session["ListaLog"] = null;
                Session["TipoMedicoEnvio"] = 1;
                Session["VoltaImpAnamnese"] = 0;
                Session["NivelEnvio"] = 1;
                objeto = new MEDICOS();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroMedico()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaMedico"] = null;
                return RedirectToAction("MontarTelaMedico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoMedico()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaMedico"] = listaMaster;
                return RedirectToAction("MontarTelaMedico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseMedico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMedico");
        }

        [HttpPost]
        public ActionResult FiltrarMedico(MEDICOS item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<MEDICOS> listaObj = new List<MEDICOS>();
                Tuple<Int32, List<MEDICOS>, Boolean> volta = baseApp.ExecuteFilter(item.ESPE_CD_ID, item.MEDC_NM_MEDICO, item.MEDC_NR_CRM, item.MEDC_EM_EMAIL, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMedico"] = 1;
                    return RedirectToAction("MontarTelaMedico");
                }

                // Sucesso
                Session["MensMedico"] = null;
                listaMaster = volta.Item2;
                Session["ListaMedico"] = volta.Item2;
                return RedirectToAction("MontarTelaMedico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirMedico()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Inclusão";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medicamentos - Inclusão";

                if (Session["MensMedico"] != null)
                {
                    if ((Int32)Session["MensMedico"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_INCLUIR", "Medico", "IncluirMedico");

                // Prepara view
                Session["MensMedico"] = null;
                MEDICOS item = new MEDICOS();
                MedicoViewModel vm = Mapper.Map<MEDICOS, MedicoViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.MEDC_IN_ATIVO = 1;
                vm.MEDC_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMedico(MedicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEDC_NM_MEDICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_MEDICO);
                    vm.MEDC_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_BAIRRO);
                    vm.MEDC_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_CIDADE);
                    vm.MEDC_NM_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_COMPLEMENTO);
                    vm.MEDC_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_ENDERECO);
                    vm.MEDC_NR_CRM = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.MEDC_NR_CRM);
                    vm.MEDC_NR_CEP = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.MEDC_NR_CEP);
                    vm.MEDC_EM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.MEDC_EM_EMAIL);

                    // Preparação
                    MEDICOS item = Mapper.Map<MedicoViewModel, MEDICOS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMedico"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0684", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<MEDICOS>();
                    Session["ListaMedico"] = null;
                    Session["IdMedico"] = item.MEDC_CD_ID;
                    Session["MedicoAlterada"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O médico" + item.MEDC_NM_MEDICO.ToUpper() + " foi incluído com sucesso e está disponível para envio de informações";
                    Session["MensMedico"] = 61;

                    // Retorno
                    return RedirectToAction("VoltarAnexoMedico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMedico(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Edição";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medico - Edição";

                MEDICOS item = baseApp.GetItemById(id);
                Session["Medico"] = item;
                ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
                ViewBag.Envios = item.MEDICOS_ENVIO.Where(p => p.MEEV_IN_ATIVO == 1).ToList();

                // Mensagens
                if (Session["MensMedico"] != null)
                {
                    if ((Int32)Session["MensMedico"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_EDITAR", "Medico", "EditarMedico");

                Session["MensMedico"] = null;
                Session["TipoMedicoEnvio"] = 1;
                objetoAntes = item;
                Session["IdMedico"] = id;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                MedicoViewModel vm = Mapper.Map<MEDICOS, MedicoViewModel>(item);
                return View(vm);

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarMedico(MedicoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEDC_NM_MEDICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_MEDICO);
                    vm.MEDC_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_BAIRRO);
                    vm.MEDC_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_CIDADE);
                    vm.MEDC_NM_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_COMPLEMENTO);
                    vm.MEDC_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDC_NM_ENDERECO);
                    vm.MEDC_NR_CRM = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.MEDC_NR_CRM);
                    vm.MEDC_NR_CEP = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.MEDC_NR_CEP);
                    vm.MEDC_EM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.MEDC_EM_EMAIL);

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    MEDICOS item = Mapper.Map<MedicoViewModel, MEDICOS>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Sucesso
                    listaMaster = new List<MEDICOS>();
                    Session["ListaMedico"] = null;
                    Session["MedicoAlterada"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O médico " + item.MEDC_NM_MEDICO.ToUpper() + " foi alterado com sucesso";
                    Session["MensMedico"] = 61;

                    return RedirectToAction("EditarMedico", new { id = (Int32)Session["IdMedico"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoMedico()
        {

            return RedirectToAction("EditarMedico", new { id = (Int32)Session["IdMedico"] });
        }

        public ActionResult VoltarAnexoEnvio()
        {

            return RedirectToAction("EditarEnvio", new { id = (Int32)Session["IdEnvio"] });
        }

        [HttpGet]
        public ActionResult ExcluirMedico(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Exclusão";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera Medico
                MEDICOS item = baseApp.GetItemById(id);

                // Verifica se foi usado
                List<MEDICOS_ENVIO> itens = item.MEDICOS_ENVIO.Where(p => p.MEEV_IN_ATIVO == 1).ToList();
                if (itens.Count > 0)
                {
                    Session["MensMedico"] = 3;
                    return RedirectToAction("MontarTelaMedico");
                }

                // Processa
                item.MEDC_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                Session["ListaMedico"] = null;
                Session["MedicoAlterada"] = 1;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O médico " + item.MEDC_NM_MEDICO.ToUpper() + " foi excluído com sucesso";
                Session["MensMedico"] = 61;

                return RedirectToAction("MontarTelaMedico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarMedico(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Exclusão";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                MEDICOS item = baseApp.GetItemById(id);
                item.MEDC_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                Session["ListaMedico"] = null;
                Session["MedicoAlterada"] = 1;
                return RedirectToAction("MontarTelaMedico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMedico(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medico - Consulta";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_VER", "Medico", "VerMedico");

                Session["IdMedico"] = id;
                MEDICOS item = baseApp.GetItemById(id);
                MedicoViewModel vm = Mapper.Map<MEDICOS, MedicoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<MEDICOS> CarregarMedico()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MEDICOS> conf = new List<MEDICOS>();
                if (Session["Medicos"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["MedicoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<MEDICOS>)Session["Medicos"];
                    }
                }
                Session["MedicoAlterada"] = 0;
                Session["Medicos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public CONFIGURACAO CarregaConfiguracaoGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONFIGURACAO conf = new CONFIGURACAO();
                if (Session["Configuracao"] == null)
                {
                    conf = confApp.GetAllItems(idAss).FirstOrDefault();
                }
                else
                {
                    if ((Int32)Session["ConfAlterada"] == 1)
                    {
                        conf = confApp.GetAllItems(idAss).FirstOrDefault();
                    }
                    else
                    {
                        conf = (CONFIGURACAO)Session["Configuracao"];
                    }
                }
                Session["ConfAlterada"] = 0;
                Session["Configuracao"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<ESPECIALIDADE> CarregaEspecialidade()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ESPECIALIDADE> conf = new List<ESPECIALIDADE>();
                if (Session["Especialidades"] == null)
                {
                    conf = usuApp.GetAllEspecialidade(idAss);
                }
                else
                {
                    if ((Int32)Session["EspecialidadeAlterada"] == 1)
                    {
                        conf = usuApp.GetAllEspecialidade(idAss);
                    }
                    else
                    {
                        conf = (List<ESPECIALIDADE>)Session["Especialidades"];
                    }
                }
                Session["EspecialidadeAlterada"] = 0;
                Session["Especialidades"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<UF> CarregaUF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<UF> conf = new List<UF>();
                if (Session["UF"] == null)
                {
                    conf = pacApp.GetAllUF();
                }
                else
                {
                    conf = (List<UF>)Session["UF"];
                }
                Session["UF"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult IncluirMovimento()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Envio - Inclusão";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Médico - Envio - Inclusão";

                // Carrega listas
                List<PACIENTE> listaPac = CarregaPaciente();
                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaPac = listaPac.ToList();
                }
                else
                {
                    listaPac = listaPac.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                String nomeMedico = String.Empty;
                MEDICOS medx = null;
                ViewBag.Paciente = new SelectList(listaPac, "PACI__CD_ID", "PACI_NM_NOME");
                ViewBag.TipoEnvio= new SelectList(CarregaTipoEnvio(), "TIEN_CD_ID", "TIEN_NM_NOME");
                ViewBag.Textos = new SelectList(CarregarMedicoTexto(), "METX_CD_ID", "METX_NM_NOME");
                if ((Int32)Session["TipoMedicoEnvio"] == 1)
                {
                    medx = baseApp.GetItemById((Int32)Session["IdMedico"]);
                    ViewBag.NomeMedico = medx.MEDC_NM_MEDICO;
                    nomeMedico = medx.MEDC_NM_MEDICO;
                }
                List<MEDICOS> listaMed = CarregarMedico();
                ViewBag.Medico = new SelectList(listaPac, "MEDC_CD_ID", "MEDC_NM_MEDICO");
                var anam = new List<SelectListItem>();
                anam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                anam.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Anamnese = new SelectList(anam, "Value", "Text");
                var mod = new List<SelectListItem>();
                mod.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                mod.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Modelo = new SelectList(mod, "Value", "Text");

                // Prepara objeto
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                MEDICOS_ENVIO item = new MEDICOS_ENVIO();
                MedicoEnvioViewModel vm = Mapper.Map<MEDICOS_ENVIO, MedicoEnvioViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.MEEV_DT_ENVIO = DateTime.Now;
                vm.MEEV_IN_ATIVO = 1;
                vm.MEEV_IN_ENVIADO = 0;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.MEDC_CD_ID = (Int32)Session["IdMedico"];
                vm.MEEV_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                if ((Int32)Session["TipoMedicoEnvio"] == 1)
                {
                    vm.MEEV_NM_TITULO = "Envio de informações para " + nomeMedico;
                    vm.NOME_MEDICO = medx.MEDC_NM_MEDICO;
                    vm.CRM = medx.MEDC_NR_CRM;
                    vm.MAIL = medx.MEDC_EM_EMAIL;
                }
                else
                {
                    vm.MEEV_NM_TITULO = "Envio de informações";
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_ENVIO_INCLUIR", "Medico", "IncluirMovimento");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IncluirMovimento(MedicoEnvioViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<PACIENTE> listaPac = CarregaPaciente();
            if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
            {
                listaPac = listaPac.ToList();
            }
            else
            {
                listaPac = listaPac.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            ViewBag.Paciente = new SelectList(listaPac, "PACI__CD_ID", "PACI_NM_NOME");
            ViewBag.TipoEnvio = new SelectList(CarregaTipoEnvio(), "TIEN_CD_ID", "TIEN_NM_NOME");
            MEDICOS medx = baseApp.GetItemById((Int32)Session["IdMedico"]);
            ViewBag.NomeMedico = medx.MEDC_NM_MEDICO;
            List<MEDICOS> listaMed = CarregarMedico();
            ViewBag.Medico = new SelectList(listaPac, "MEDC_CD_ID", "MEDC_NM_MEDICO");
            var anam = new List<SelectListItem>();
            anam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            anam.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Anamnese = new SelectList(anam, "Value", "Text");
            ViewBag.Textos = new SelectList(CarregarMedicoTexto(), "METX_CD_ID", "METX_NM_NOME");
            var mod = new List<SelectListItem>();
            mod.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mod.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Modelo = new SelectList(mod, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEEV_TX_MENSAGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEEV_TX_MENSAGEM);
                    vm.MEEV_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEEV_NM_TITULO);

                    // Critica
                    if (vm.MEEV_IN_ANAMNESE == null || vm.MEEV_IN_ANAMNESE == 0)
                    {
                        vm.MEEV_IN_ANAMNESE = 1;
                    }

                    // Monta movimento
                    MEDICOS_ENVIO item = Mapper.Map<MedicoEnvioViewModel, MEDICOS_ENVIO>(vm);
                    PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID.Value);
                    MEDICOS med = baseApp.GetItemById(item.MEDC_CD_ID);

                    // Executa
                    Int32 volta = baseApp.ValidateCreateEnvio(item);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Medico/" + item.MEDC_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + item.MEEV_CD_ID.ToString() + "/Anexos/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + item.MEEV_CD_ID.ToString() + "/Anamneses/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Acerta estado
                    Session["IdEnvio"] = item.MEEV_CD_ID;
                    Session["MedicoAlterada"] = 1;
                    Session["EnvioAlterada"] = 1;
                    Session["ListaMedico"] = null;
                    Session["ListaEnvio"] = null;
                    Session["IdMedico"] = item.MEDC_CD_ID;
                    Session["VoltarPesquisa"] = 0;
                    Int32 volta3 = 0;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Medico_Envio dto = MontarMedicoEnvioDTO(item.MEEV_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Medico - Envio de Informações",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Trata anexos
                    if (Session["FileQueuePaciente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePaciente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                volta3 = UploadFileQueueEnvio(file);
                            }
                        }
                        Session["FileQueuePaciente"] = null;
                    }

                    // Processa geração da anamnese
                    MEDICOS_ENVIO env = baseApp.GetEnvioById(item.MEEV_CD_ID);
                    PACIENTE_ANAMNESE ana = pac.PACIENTE_ANAMNESE.FirstOrDefault();
                    String guid = pac.PACI_GU_GUID;
                    String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();
                    if ((Int32)Session["BlocoAnamnese"] == 1)
                    {
                        Int32 voltaz = GerarAnamnesePDFTeste(pac, med, env);
                    }
                    else
                    {
                        Int32 voltaz = GerarAnamnesePDFTesteSono(pac, med, env);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O registro do envio de informações do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " para o médico(a) " + med.MEDC_NM_MEDICO.ToUpper() + " foi criado com sucesso mas ainda não enviado. Clique em Enviar na linha do envio na lista abaixo para enviar as informações";
                    Session["MensMedico"] = 61;
                    Session["NivelEnvio"] = 1;

                    // Retorno
                    return RedirectToAction("EditarMedico", new { id = (Int32)Session["IdMedico"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Paciente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "epront", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<PACIENTE> CarregaPaciente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE> conf = new List<PACIENTE>();
                if (Session["Pacientes"] == null)
                {
                    conf = pacApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["PacienteAlterada"] == 1)
                    {
                        conf = pacApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<PACIENTE>)Session["Pacientes"];
                    }
                }
                Session["Pacientes"] = conf;
                Session["PacienteAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TIPO_ENVIO> CarregaTipoEnvio()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_ENVIO> conf = new List<TIPO_ENVIO>();
                if (Session["TipoEnvios"] == null)
                {
                    conf = baseApp.GetAllTipos(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoEnvioAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipos(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_ENVIO>)Session["TipoEnvios"];
                    }
                }
                Session["TipoEnvios"] = conf;
                Session["TipoEnvioAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();
            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }
                queue.Add(f);
            }
            Session["FileQueuePaciente"] = queue;
        }

        [HttpPost]
        public Int32 UploadFileQueueEnvio(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdEnvio"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensMedico"] = 5;
                    return 1;
                }

                // Recupera envio
                MEDICOS_ENVIO item = baseApp.GetEnvioById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensMedico"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensMedico"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensMedico"] = 12;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + item.MEEV_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                MEDICOS_ENVIO_ANEXO foto = new MEDICOS_ENVIO_ANEXO();
                foto.MVAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.MVAN_DT_ANEXO = DateTime.Today;
                foto.MVAN_IN_ATIVO = 1;
                Int32 tipo = 3;
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    tipo = 1;
                }
                else if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 2;
                }
                else if (extensao.ToUpper() == ".PDF")
                {
                    tipo = 3;
                }
                else if (extensao.ToUpper() == ".MP3" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 4;
                }
                else if (extensao.ToUpper() == ".DOCX" || extensao.ToUpper() == ".DOC" || extensao.ToUpper() == ".ODT")
                {
                    tipo = 5;
                }
                else if (extensao.ToUpper() == ".XLSX" || extensao.ToUpper() == ".XLS" || extensao.ToUpper() == ".ODS")
                {
                    tipo = 6;
                }
                else
                {
                    tipo = 7;
                }
                foto.MVAN_IN_TIPO = tipo;
                foto.MVAN_NM_TITULO = fileName;
                foto.MEEV_CD_ID = item.MEEV_CD_ID;
                item.MEDICOS_ENVIO_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditEnvio(item);
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        public JsonResult GetMedico(Int32 id)
        {
            var medico = baseApp.GetItemById(id);
            var hash = new Hashtable();
            hash.Add("crm", medico.MEDC_NR_CRM);
            hash.Add("mail", medico.MEDC_EM_EMAIL);
            return Json(hash);
        }

        public JsonResult GetTexto(Int32 id)
        {
            var texto = baseApp.GetTextoMensagemById(id);
            var hash = new Hashtable();
            hash.Add("texto", texto.METX_TX_TEXTO);
            return Json(hash);
        }

        [HttpGet]
        public ActionResult EditarEnvio(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_ENVIAR_MEDICO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Envio - Edição";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Envio - Edição";

                // Trata mensagens
                if (Session["MensMedico"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensMedico"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 69)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0539", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 70)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0540", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara view
                ViewBag.TipoEnvio = new SelectList(CarregaTipoEnvio(), "TIEN_CD_ID", "TIEN_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                Session["VoltarPesquisa"] = 0;

                MEDICOS_ENVIO item = baseApp.GetEnvioById(id);
                Session["IdMedico"] = item.MEDC_CD_ID;
                Session["IdEnvio"] = item.MEEV_CD_ID;
                MedicoEnvioViewModel vm = Mapper.Map<MEDICOS_ENVIO, MedicoEnvioViewModel>(item);
                Session["Exame"] = item;
                ViewBag.NomeMedico = item.MEDICOS.MEDC_NM_MEDICO;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ENVIO_EDITAR", "Medico", "EditarEnvio");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEnvio(MedicoEnvioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ViewBag.TipoEnvio = new SelectList(CarregaTipoEnvio(), "TIEN_CD_ID", "TIEN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEEV_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEEV_NM_TITULO);
                    vm.MEEV_TX_MENSAGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEEV_TX_MENSAGEM);

                    // Executa a operação
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID.Value);
                    MEDICOS med = baseApp.GetItemById(vm.MEDC_CD_ID);
                    MEDICOS_ENVIO item = Mapper.Map<MedicoEnvioViewModel, MEDICOS_ENVIO>(vm);
                    Int32 volta = baseApp.ValidateEditEnvio(item);

                    // Acerta estado
                    Session["IdEnvio"] = item.MEEV_CD_ID;
                    Session["MedicoAlterada"] = 1;
                    Session["NivelEnvio"] = 1;
                    Session["EnvioAlterada"] = 1;
                    Session["IdMedico"] = item.MEDC_CD_ID;
                    Session["ListaMedico"] = null;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Medico_Envio dto = MontarMedicoEnvioDTO(item.MEEV_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Medico - Envio - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O envio das informações do paciente " + pac.PACI_NM_NOME.ToUpper() + " para o médico " + med.MEDC_NM_MEDICO.ToUpper() + " foi alterado com sucesso.";
                    Session["MensMedico"] = 61;

                    // Retorno
                    return RedirectToAction("VoltarAnexoEnvio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileEnvio(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdEnvio"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                MEDICOS_ENVIO item = baseApp.GetEnvioById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensMedico"] = 5;
                    return RedirectToAction("VoltarAnexoEnvio");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensMedico"] = 6;
                    return RedirectToAction("VoltarAnexoEnvio");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensMedico"] = 7;
                    return RedirectToAction("VoltarAnexoEnvio");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensMedico"] = 12;
                    return RedirectToAction("VoltarAnexoEnvio");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + item.MEEV_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                // Gravar registro
                MEDICOS_ENVIO_ANEXO foto = new MEDICOS_ENVIO_ANEXO();
                foto.MVAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.MVAN_DT_ANEXO = DateTime.Today;
                foto.MVAN_IN_ATIVO = 1;
                Int32 tipo = 3;
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    tipo = 1;
                }
                else if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 2;
                }
                else if (extensao.ToUpper() == ".PDF")
                {
                    tipo = 3;
                }
                else if (extensao.ToUpper() == ".MP3" || extensao.ToUpper() == ".MPEG")
                {
                    tipo = 4;
                }
                else if (extensao.ToUpper() == ".DOCX" || extensao.ToUpper() == ".DOC" || extensao.ToUpper() == ".ODT")
                {
                    tipo = 5;
                }
                else if (extensao.ToUpper() == ".XLSX" || extensao.ToUpper() == ".XLS" || extensao.ToUpper() == ".ODS")
                {
                    tipo = 6;
                }
                else
                {
                    tipo = 7;
                }
                foto.MVAN_IN_TIPO = tipo;
                foto.MVAN_NM_TITULO = fileName;
                foto.MEEV_CD_ID = item.MEEV_CD_ID;

                item.MEDICOS_ENVIO_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditEnvio(item);
                Session["NivelEnvio"] = 2;
                Session["EnvioAlterada"] = 1;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O arquivo " + fileName.ToUpper() + " foi anexado com sucesso ao envio para o médico " + item.MEDICOS.MEDC_NM_MEDICO.ToUpper();
                Session["MensMedico"] = 61;

                return RedirectToAction("VoltarAnexoEnvio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEnvio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara view
                MEDICOS_ENVIO_ANEXO item = baseApp.GetMedicoAnexoById(id);
                Session["NivelEnvio"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ENVIO_ANEXO", "Medico", "VerAnexoEnvio");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEnvioAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara view
                MEDICOS_ENVIO_ANEXO item = baseApp.GetMedicoAnexoById(id);
                Session["NivelEnvio"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ENVIO_ANEXO", "Medico", "VerAnexoEnvio");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexoEnvio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                MEDICOS_ENVIO_ANEXO item = baseApp.GetMedicoAnexoById(id);
                item.MVAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditMedicoAnexo(item);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O arquivo " + item.MVAN_AQ_ARQUIVO.ToUpper() + " foi excluído com sucesso do envio para o médico";
                Session["MensMedico"] = 61;

                Session["NivelEnvio"] = 2;
                Session["EnvioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoEnvio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadEnvio(Int32 id)
        {
            try
            {
                MEDICOS_ENVIO_ANEXO item = baseApp.GetMedicoAnexoById(id);
                String arquivo = item.MVAN_AQ_ARQUIVO;
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = arquivo.Substring(pos);
                String contentType = string.Empty;
                if (arquivo.Contains(".pdf"))
                {
                    contentType = "application/pdf";
                }
                else if (arquivo.Contains(".jpg") || arquivo.Contains(".jpeg"))
                {
                    contentType = "image/jpg";
                }
                else if (arquivo.Contains(".png"))
                {
                    contentType = "image/png";
                }
                else if (arquivo.Contains(".docx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (arquivo.Contains(".xlsx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else if (arquivo.Contains(".pptx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                }
                else if (arquivo.Contains(".mp3"))
                {
                    contentType = "audio/mpeg";
                }
                else if (arquivo.Contains(".mpeg"))
                {
                    contentType = "audio/mpeg";
                }
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                Session["NivelEnvio"] = 2;
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult IncluirAnotacaoEnvio()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["NivelEnvio"] = 3;
                Int32 s = (Int32)Session["VoltarPesquisa"];

                MEDICOS_ENVIO item = baseApp.GetEnvioById((Int32)Session["IdEnvio"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                MEDICOS_ENVIO_ANOTACAO coment = new MEDICOS_ENVIO_ANOTACAO();
                MedicoAnotacaoViewModel vm = Mapper.Map<MEDICOS_ENVIO_ANOTACAO, MedicoAnotacaoViewModel>(coment);
                vm.MEAT_DT_ANOTACAO = DateTime.Now;
                vm.MEAT_IN_ATIVO = 1;
                vm.MEEV_CD_ID = item.MEEV_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;

                MEDICOS cli = baseApp.GetItemById(item.MEDC_CD_ID);
                ViewBag.NomePaciente = cli.MEDC_NM_MEDICO;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_ENVIO_ANOTACAO_INCLUIR", "Medico", "IncluirAnotacaoEnvio");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirAnotacaoEnvio(MedicoAnotacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEAT_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEAT_TX_ANOTACAO);

                    // Executa a operação
                    MEDICOS_ENVIO_ANOTACAO item = Mapper.Map<MedicoAnotacaoViewModel, MEDICOS_ENVIO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MEDICOS_ENVIO not = baseApp.GetEnvioById((Int32)Session["IdEnvio"]);

                    item.USUARIO = null;
                    not.MEDICOS_ENVIO_ANOTACAO.Add(item);
                    Int32 volta = baseApp.ValidateEditEnvio(not);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Anotação incluída com sucesso ao envio para o médico " + not.MEDICOS.MEDC_NM_MEDICO.ToUpper();
                    Session["MensMedico"] = 61;

                    // Sucesso
                    Session["NivelEnvio"] = 3;
                    Session["VoltarPesquisa"] = 0;
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    return RedirectToAction("EditarEnvio", new { id = (Int32)Session["IdEnvio"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoEnvio(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_EXAME_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Exames - Alteração";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelEnvio"] = 3;
                MEDICOS_ENVIO_ANOTACAO item = baseApp.GetAnotacaoById(id);
                MedicoAnotacaoViewModel vm = Mapper.Map<MEDICOS_ENVIO_ANOTACAO, MedicoAnotacaoViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ENVIO_ANOTACAO_EDITAR", "Medico", "EditarAnotacaoEnvio");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoEnvio(MedicoAnotacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEAT_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEAT_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MEDICOS_ENVIO_ANOTACAO item = Mapper.Map<MedicoAnotacaoViewModel, MEDICOS_ENVIO_ANOTACAO>(vm);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Anotação alterada com sucesso";
                    Session["MensMedico"] = 61;

                    // Verifica retorno
                    Session["NivelEnvio"] = 3;
                    return RedirectToAction("VoltarAnexoEnvio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoEnvio(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                MEDICOS_ENVIO_ANOTACAO item = baseApp.GetAnotacaoById(id);
                item.MEAT_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["NivelEnvio"] = 3;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "Anotação excluída com sucesso";
                Session["MensMedico"] = 61;

                return RedirectToAction("VoltarAnexoEnvio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> ReenviarEnvio(Int32 id)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();
            MEDICOS_ENVIO envio = baseApp.GetEnvioById(id);
            MEDICOS medico = baseApp.GetItemById(envio.MEDC_CD_ID);
            PACIENTE paciente = pacApp.GetItemById(envio.PACI_CD_ID.Value);
            Session["IdEnvio"] = envio.MEEV_CD_ID;

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Prepara cabeçalho
            String cab = "<p style='background-color: darkseagreen; width: 600px; font-size: 24px; font-weight: bold; color: darkgreen; text-align: center;'>Envio de Informações</p>";

            // Prepara rodape
            String classe = String.Empty;
            if (usuario.TIPO_CARTEIRA_CLASSE != null)
            {
                classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            }
            String rod = "<b>" + usuario.USUA_NM_NOME + "</b><br />";
            if (usuario.ESPECIALIDADE != null)
            {
                rod += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
            }
            else
            {
                rod += usuario.USUA_NM_ESPECIALIDADE + "<br />";
            }
            rod += classe + "  CPF: " + usuario.USUA_NR_CPF;

            // Prepara mensagem
            String texto = String.Empty;
            String msg = String.Empty;
            if((Int32)Session["BlocoAnamnese"] == 1)
            {
                msg = envio.MEEV_TX_MENSAGEM;
                msg = msg.Replace("\r\n", "<br />");
            }
            else
            {
                TEMPLATE_EMAIL template = temApp.GetByCode("MENSMEDI", idAss);
                msg = template.TEEM_TX_CORPO;
                msg = msg.Replace("{data}", DateTime.Today.Date.ToShortDateString());
                msg = msg.Replace("{paciente}", paciente.PACI_NM_NOME);
                msg = msg.Replace("{medico}", medico.MEDC_NM_MEDICO);
                if (envio.MEEV_NR_PRESSAO_POSITIVA != null)
                {
                    msg = msg.Replace("{pressaoPositiva}", CrossCutting.Formatters.DecimalFormatter(envio.MEEV_NR_PRESSAO_POSITIVA.Value));
                }
                else
                {
                    msg = msg.Replace("{pressaoPositiva}", "---");
                }
                if (envio.MEEV_IN_IAH != null)
                {
                    msg = msg.Replace("{IAH}", envio.MEEV_IN_IAH.ToString());
                }
                else
                {
                    msg = msg.Replace("{IAH}", "---");
                }
                if (envio.MEEV_NM_EQUIPAMENTO != null)
                {
                    msg = msg.Replace("{equip}", envio.MEEV_NM_EQUIPAMENTO);
                }
                else
                {
                    msg = msg.Replace("{equip}", "---");
                }
                if (envio.MEEV_NM_MASCARA != null)
                {
                    msg = msg.Replace("{mascara}", envio.MEEV_NM_MASCARA);
                }
                else
                {
                    msg = msg.Replace("{mascara}", "---");
                }
                if (envio.MEEV_DT_NOITE_INICIO != null)
                {
                    msg = msg.Replace("{inicio}", envio.MEEV_DT_NOITE_INICIO.Value.ToShortDateString());
                }
                else
                {
                    msg = msg.Replace("{inicio}", "---");
                }
                if (envio.MEEV_DT_NOITE_FINAL != null)
                {
                    msg = msg.Replace("{final}", envio.MEEV_DT_NOITE_FINAL.Value.ToShortDateString());
                }
                else
                {
                    msg = msg.Replace("{final}", "---");
                }
                if (envio.MEEV_IN_NUM_NOITES != null)
                {
                    msg = msg.Replace("{naoUtil}", envio.MEEV_IN_NUM_NOITES.ToString());
                }
                else
                {
                    msg = msg.Replace("{naoUtil}", "---");
                }
                if (envio.MEEV_NR_PARAM_PRESSAO != null)
                {
                    msg = msg.Replace("{pressao}", CrossCutting.Formatters.DecimalFormatter(envio.MEEV_NR_PARAM_PRESSAO.Value));
                }
                else
                {
                    msg = msg.Replace("{pressao}", "---");
                }
                if (envio.MEEV_NR_MEDIA_USO != null)
                {
                    msg = msg.Replace("{usoHoras}", CrossCutting.Formatters.DecimalFormatter(envio.MEEV_NR_MEDIA_USO.Value));
                }
                else
                {
                    msg = msg.Replace("{usoHoras}", "---");
                }
                if (envio.MEEV_IN_PERCENTUAL != null)
                {
                    msg = msg.Replace("{usoMaior}", CrossCutting.Formatters.DecimalFormatter(envio.MEEV_IN_PERCENTUAL.Value));
                }
                else
                {
                    msg = msg.Replace("{usoMaior}", "---");
                }
                if (envio.MEEV_DS_SINTOMAS != null)
                {
                    msg = msg.Replace("{sintomas}", envio.MEEV_DS_SINTOMAS);
                }
                else
                {
                    msg = msg.Replace("{sintomas}", "---");
                }
                if (envio.MEEV_NR_IAH_RESIDUAL != null)
                {
                    msg = msg.Replace("{IAHRes}", CrossCutting.Formatters.DecimalFormatter(envio.MEEV_NR_IAH_RESIDUAL.Value));
                }
                else
                {
                    msg = msg.Replace("{IAHRes}", "---");
                }
            }

            // Dados do medico
            String dadosMedico = "<br /><b>Destinatário das Informações</b><br />Nome:  <b>{nome}</b><br />CRM:  <b>{crm}</b><br />E - Mail:  <b>{mail}</b>";
            dadosMedico = dadosMedico.Replace("{nome}", medico.MEDC_NM_MEDICO);
            dadosMedico = dadosMedico.Replace("{crm}", medico.MEDC_NR_CRM);
            dadosMedico = dadosMedico.Replace("{mail}", medico.MEDC_EM_EMAIL);

            // Dados do Paciente
            String dadosPaciente = "<br /><b>Dados do Paciente</b><br />Nome:  <b>{nome}</b ><br />Data de Nascimento:  <b>{nasc}</b ><br />CPF:  <b>{cpf}</b >";
            dadosPaciente = dadosPaciente.Replace("{nome}", paciente.PACI_NM_NOME);
            dadosPaciente =  dadosPaciente.Replace("{nasc}", paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString());
            dadosPaciente = dadosPaciente.Replace("{cpf}", paciente.PACI_NR_CPF);

            // Monta mensagem de anamnese
            String anamnese = String.Empty;
            if (envio.MEEV_IN_ANAMNESE == 1)
            {
                anamnese = "<b>ATENÇÃO</b> - A anamnese completa do paciente está anexada a esta mensagem.<br />";
            }

            // Prepara corpo do e-mail
            String emailBody = cab + anamnese + "<br />" + dadosMedico + "<br />" + dadosPaciente + "<br /><br />" + msg +  "<br /><br />" + rod;

            // Incluir Anamnese como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            AttachmentModel model = new AttachmentModel();
            if (envio.MEEV_IN_ANAMNESE == 1)
            {
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anamneses/";
                String fileNamePDF = "Anamnese_" + paciente.PACI_NM_NOME + "_" + envio.MEEV_GU_IDENTIFICADOR + ".pdf";
                String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                String base64String = Convert.ToBase64String(fileBytes);

                model = new AttachmentModel();
                model.PATH = path;
                model.ATTACHMENT_NAME = fileNamePDF;
                model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                models.Add(model);
            }

            // Inclui demais anexos
            List<MEDICOS_ENVIO_ANEXO> anexos = envio.MEDICOS_ENVIO_ANEXO.ToList();
            foreach (MEDICOS_ENVIO_ANEXO item in anexos)
            {
                String caminhoAnexo = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anexos/";
                String fileNameAnexo = item.MVAN_NM_TITULO;
                String pathAnexo = Path.Combine(Server.MapPath(caminhoAnexo), fileNameAnexo);

                byte[] fileBytesa = System.IO.File.ReadAllBytes(pathAnexo);
                String base64String1 = Convert.ToBase64String(fileBytesa);


                model = new AttachmentModel();
                model.PATH = pathAnexo;
                model.ATTACHMENT_NAME = fileNameAnexo;
                model.ContentBytes = base64String1;
                if (item.MVAN_IN_TIPO == 1)
                {
                    model.CONTENT_TYPE = System.Net.Mime.MediaTypeNames.Image.Jpeg;

                }
                else if (item.MVAN_IN_TIPO == 2)
                {
                    model.CONTENT_TYPE = "video/mp4" ;

                }
                else if (item.MVAN_IN_TIPO == 3)
                {
                    model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;

                }
                else if (item.MVAN_IN_TIPO == 4)
                {
                    model.CONTENT_TYPE = "audio/mpeg";

                }
                else if (item.MVAN_IN_TIPO == 5)
                {
                    model.CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                }
                else if (item.MVAN_IN_TIPO == 6)
                {
                    model.CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                }
                else
                {
                    model.CONTENT_TYPE = "application/octet-stream";
                }
                models.Add(model);
            }

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Envio de Informações - Paciente - " + paciente.PACI_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = medico.MEDC_EM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conn;

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, models);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

            // Atualiza status
            MEDICOS_ENVIO env = baseApp.GetEnvioById(id);
            env.MEEV_IN_ENVIADO = 1;
            Int32 volta1 = baseApp.ValidateEditEnvio(env);

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel vm = new MensagemViewModel();
                vm.MENS_NM_NOME = "Envio de Informação - Paciente - " + paciente.PACI_NM_NOME;
                vm.MENS_NM_CAMPANHA = medico.MEDC_EM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null;
                vm.MENS_IN_TIPO = 1;
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.MENS_IN_USUARIO = usuario.USUA_CD_ID;
                EnvioEMailGeralBase enviox = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = enviox.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Paciente - Envio Solicitação");
                Session["MensPaciente"] = 992;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensPaciente"] = 993;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }

            // Mensagem do CRUD
            Session["MsgCRUD"] = "As informações do(a) paciente " + paciente.PACI_NM_NOME.ToUpper() + " para o médico(a) " + medico.MEDC_NM_MEDICO.ToUpper() + " foram enviadas com sucesso";
            Session["MensMedico"] = 61;

            return RedirectToAction("EditarMedico", new { id = (Int32)Session["IdMedico"] });
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvio(MEDICOS_ENVIO envio)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();
            MEDICOS medico = baseApp.GetItemById(envio.MEDC_CD_ID);
            PACIENTE paciente = pacApp.GetItemById(envio.PACI_CD_ID.Value);

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Prepara cabeçalho
            String cab = "<p style='background - color: darkseagreen; font - size: 24px; font - weight: bold; color: darkgreen'>ENVIO DE INFORMA^ÇÃO DE PACIENTE</p>";

            // Prepara rodape
            String classe = String.Empty;
            if (usuario.TIPO_CARTEIRA_CLASSE != null)
            {
                classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            }
            String rod = "<b>" + usuario.USUA_NM_NOME + "</b><br />";
            if (usuario.ESPECIALIDADE != null)
            {
                rod += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
            }
            else
            {
                rod += usuario.USUA_NM_ESPECIALIDADE + "<br />";
            }
            rod += classe + "  CPF: " + usuario.USUA_NR_CPF;

            // Prepara mensagem
            String texto = String.Empty;

            // Dados do medico
            String dadosMedico = "<br /><b>Destinatário das Informações</b><br />Nome:  <b>{nome} </b><br />CRM:  <b>{crm} </b><br />E - Mail:  <b>{mail} </b>< br />";
            dadosMedico.Replace("{nome}", medico.MEDC_NM_MEDICO);
            dadosMedico.Replace("{crm}", medico.MEDC_NR_CRM);
            dadosMedico.Replace("{mail}", medico.MEDC_EM_EMAIL);

            // Dados do Paciente
            String dadosPaciente = "<br /><b>Dados do Paciente</b><br />Nome:  <b>{nome} </b ><br />Data de Nascimento:  <b>{nasc} </b ><br />CPF:  <b>{cpf} </b ><br />";
            dadosPaciente.Replace("{paciente}", paciente.PACI_NM_NOME);
            dadosPaciente.Replace("{nasc}", paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString());
            dadosPaciente.Replace("{cpf}", paciente.PACI_NR_CPF);

            // Monta anamnese
            String anamnese = String.Empty;
            if (envio.MEEV_IN_ANAMNESE == 1)
            {
                anamnese = "<br />A anamnese completa do paciente está anexada a esta mensagem juntamente com o último exame fisico realizado.<br />";
            }

            // Prepara corpo do e-mail
            String emailBody = cab + "<br /><br />" + anamnese + "<br /><br />" + msg + "<br /><br />" + dadosMedico + "<br /><br />" + dadosPaciente + "<br /><br />" + rod;

            // Gera anemnese em PDF
            List<AttachmentModel> models = new List<AttachmentModel>();
            if (envio.MEEV_IN_ANAMNESE == 1)
            {
                PACIENTE_ANAMNESE ana = paciente.PACIENTE_ANAMNESE.FirstOrDefault();
                String guid = paciente.PACI_GU_GUID;
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();
                Int32 voltaz = GerarAnamnesePDFTeste(paciente, medico, envio);

                // Gera exame fisico em PDF
                PACIENTE_EXAME_FISICOS fis = paciente.PACIENTE_EXAME_FISICOS.FirstOrDefault();
                Int32 voltay = GerarExameFisicoPDFTeste(paciente, medico, envio);

                // Incluir Anamnese como anexo
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anamneses/";
                String fileNamePDF = "Anamnese_" + paciente.PACI_NM_NOME + "_" + paciente.PACI_GU_GUID + ".pdf";
                String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

                AttachmentModel model = new AttachmentModel();
                model.PATH = path;
                model.ATTACHMENT_NAME = fileNamePDF;
                model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                models.Add(model);

                // Incluir exame fisico como anexo
                caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/ExamesFisicos/";
                fileNamePDF = "ExameFisico_" + paciente.PACI_NM_NOME + "_" + paciente.PACI_GU_GUID + "_" + hoje + ".pdf";
                path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

                model = new AttachmentModel();
                model.PATH = path;
                model.ATTACHMENT_NAME = fileNamePDF;
                model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                models.Add(model);
            }

            // Inclui demais anexos
            List<MEDICOS_ENVIO_ANEXO> anexos = envio.MEDICOS_ENVIO_ANEXO.ToList();
            foreach (MEDICOS_ENVIO_ANEXO item in anexos)
            {
                String caminhoAnexo = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anexos/";
                String fileNameAnexo = item.MVAN_AQ_ARQUIVO;
                String pathAnexo = Path.Combine(Server.MapPath(caminhoAnexo), fileNameAnexo);

                AttachmentModel model = new AttachmentModel();
                model.PATH = pathAnexo;
                model.ATTACHMENT_NAME = fileNameAnexo;
                if (item.MVAN_IN_TIPO == 1)
                {
                    model.CONTENT_TYPE = System.Net.Mime.MediaTypeNames.Image.Jpeg;

                }
                else if (item.MVAN_IN_TIPO == 2)
                {
                    model.CONTENT_TYPE = "video/mp4" ;

                }
                else if (item.MVAN_IN_TIPO == 3)
                {
                    model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;

                }
                else if (item.MVAN_IN_TIPO == 4)
                {
                    model.CONTENT_TYPE = "audio/mpeg";

                }
                else if (item.MVAN_IN_TIPO == 5)
                {
                    model.CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                }
                else if (item.MVAN_IN_TIPO == 6)
                {
                    model.CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                }
                else
                {
                    model.CONTENT_TYPE = "application/octet-stream";
                }
                models.Add(model);
            }

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Envio de Informações para Médico - Paciente - " + paciente.PACI_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = medico.MEDC_EM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.USUA_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conn;

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, models);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel vm = new MensagemViewModel();
                vm.MENS_NM_NOME = "Envio de Informação - Paciente - " + paciente.PACI_NM_NOME;
                vm.MENS_NM_CAMPANHA = medico.MEDC_EM_EMAIL;
                vm.MENS_IN_TIPO = 1;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null;
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.MENS_IN_USUARIO = usuario.USUA_CD_ID;
                EnvioEMailGeralBase enviox = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = enviox.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Paciente - Envio Solicitação");
                Session["MensPaciente"] = 992;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensPaciente"] = 993;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        public Int32 GerarAnamnesePDF(PACIENTE paciente, MEDICOS medico, MEDICOS_ENVIO envio)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();

                // Recupera informações
                PACIENTE_ANAMNESE anamnese = paciente.PACIENTE_ANAMNESE.FirstOrDefault();
                String nomeRel = "Anamnese_" + paciente.PACI_NM_NOME + "_" + paciente.PACI_GU_GUID + "_" + hoje + ".pdf";
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anamneses/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Processo
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;


                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    // Dados do medico
                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(nomeMedico, meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table1.AddCell(cell);
                    }

                    String frase = classe + " CPF: " + usuario.USUA_NR_CPF;
                    cell = new PdfPCell(new Paragraph(frase, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    // Dados do medico
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(usuario.USUA_NM_NOME, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(classe, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + usuario.USUA_NR_CPF, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String endereco = String.Empty;
                    String enderecoCont = String.Empty;
                    if (empresa.EMPR_NM_ENDERECO != null)
                    {
                        endereco += empresa.EMPR_NM_ENDERECO;
                        if (empresa.EMPR_NM_NUMERO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_NUMERO;
                        }
                        if (empresa.EMPR_NM_COMPLEMENTO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                        }
                        if (empresa.EMPR_NM_BAIRRO != null)
                        {
                            enderecoCont += empresa.EMPR_NM_BAIRRO;
                        }
                        if (empresa.EMPR_NM_CIDADE != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                        }
                        if (empresa.UF != null)
                        {
                            enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                        }
                        if (empresa.EMPR_NR_CEP != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NR_CEP;
                        }
                    }

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Documento assinado digitalmente", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 120);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    Chunk chunk3 = new Chunk("A N A M N E S E", FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK));
                    Paragraph paragraph = new Paragraph(chunk3);
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(paragraph);

                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados Gerais
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Última Consulta: " + anamnese.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    table = new PdfPTable(new float[] { 100f, 500f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    // Dados da anamnese
                    cell = new PdfPCell(new Paragraph("Motivo da Consulta: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_MOTIVO_CONSULTA, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Queixa Principal: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_QUEIXA_PRINCIPAL, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("História Familiar: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_FAMILIAR, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    if (anamnese.PAAM_IN_FLAG_HISTORIA_SOCIAL == 1)
                    {
                        cell = new PdfPCell(new Paragraph("História Social: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_SOCIAL, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph("História da Doença Atual: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Paragraph("Medicamentos em Uso: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_MEDICAMENTO, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    if (anamnese.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA == 1)
                    {
                        cell = new PdfPCell(new Paragraph("História Patológica Progressiva: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Avaliação Cardiológica: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_AVALIACAO_CARDIOLOGICA_LONG, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_FLAG_RESPIRATORIO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Avaliação Respiratória: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_RESPIRATORIO_LONG, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_FLAG_ABDOMEM == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Avaliação do Abdômem: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_ABDOMEM, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_FLAG_MEMBROS_INFERIORES == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Avaliação dos Membros Inferiores: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_MEMBROS_INFERIORES, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_1 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_1 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_1, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_2 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_2 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_2, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_3 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_3 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_3, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_4 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_4 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_4, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_5 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_5 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_5, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_6 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_6 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_6, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_7 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_7 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_7, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_8 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_8 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_8, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_9 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_9 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_9, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    if (anamnese.PAAM_IN_CAMPO_10 == 1)
                    {
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_10 + ": ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_10, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph("Diagnóstico: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_DIAGNOSTICO_1_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Conduta Adotada: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CONDUTA, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarExameFisicoPDF(PACIENTE paciente, MEDICOS medico, MEDICOS_ENVIO envio)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();

                // Recupera informações
                PACIENTE_EXAME_FISICOS fisico = paciente.PACIENTE_EXAME_FISICOS.FirstOrDefault();
                List<PACIENTE_DADOS_EXAME_FISICO> dados = fisico.PACIENTE_DADOS_EXAME_FISICO.ToList();
                String nomeRel = "ExameFisico" + paciente.PACI_NM_NOME + "_" + paciente.PACI_GU_GUID + "_" + hoje + ".pdf";
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/ExamesFisicos/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Processo
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    // Dados do medico
                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(nomeMedico, meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table1.AddCell(cell);
                    }

                    String frase = classe + " CPF: " + usuario.USUA_NR_CPF;
                    cell = new PdfPCell(new Paragraph(frase, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    // Dados do medico
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(usuario.USUA_NM_NOME, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(classe, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + usuario.USUA_NR_CPF, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String endereco = String.Empty;
                    String enderecoCont = String.Empty;
                    if (empresa.EMPR_NM_ENDERECO != null)
                    {
                        endereco += empresa.EMPR_NM_ENDERECO;
                        if (empresa.EMPR_NM_NUMERO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_NUMERO;
                        }
                        if (empresa.EMPR_NM_COMPLEMENTO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                        }
                        if (empresa.EMPR_NM_BAIRRO != null)
                        {
                            enderecoCont += empresa.EMPR_NM_BAIRRO;
                        }
                        if (empresa.EMPR_NM_CIDADE != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                        }
                        if (empresa.UF != null)
                        {
                            enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                        }
                        if (empresa.EMPR_NR_CEP != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NR_CEP;
                        }
                    }

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Documento assinado digitalmente", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 120);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);


                    Chunk chunk3 = new Chunk("E X A M E   F Í S I C O", FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK));
                    Paragraph paragraph = new Paragraph(chunk3);
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(paragraph);

                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados Gerais
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Última Consulta: " + fisico.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    table = new PdfPTable(new float[] { 200f, 700f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    // Exame Fisico
                    cell = new PdfPCell(new Paragraph("Pressão Sanguinea (mmHg): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_PA_ALTA.ToString() + " x " + fisico.PAEF_NR_PA_BAIXA, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Frequencia Cardiaca (bpm): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_FREQUENCIA_CARDIACA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Temperatura (oC): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_TEMPERATURA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Peso (Kg): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_PESO.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Altura (cm): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_ALTURA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("IMC: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_VL_IMC.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Histórico
                    if (dados.Count > 0)
                    {
                        // Linha Horizontal
                        line1 = new Paragraph("  ");
                        pdfDoc.Add(line1);  

                        table = new PdfPTable(new float[] { 60f, 60f, 60f, 60f, 60f, 60f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Data", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Peso (kg)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Altura (cm)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Pressão Sanguinea (mmHg)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Frequencia Cardíaca (bpm)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Temperatura (oC)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (PACIENTE_DADOS_EXAME_FISICO item in dados)
                        {
                            if (item.PDEF_DT_DATA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_DT_DATA.Value.ToShortDateString() + " " + item.PDEF_DT_DATA.Value.ToShortTimeString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_PESO != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_PESO.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_ALTURA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_ALTURA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_PRESSAO_ALTA != null & item.PDEF_IN_PRESSAO_BAIXA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_PRESSAO_ALTA.ToString() + " x " + item.PDEF_IN_PRESSAO_BAIXA.ToString(), meuFont1))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_FREQUENCIA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_FREQUENCIA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_TEMPERATURA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_TEMPERATURA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                        }
                        pdfDoc.Add(table);
                    }

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    table = new PdfPTable(new float[] { 200f, 700f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Hipertensão: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_HIPERTENSAO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Hipotensão: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_HIPOTENSAO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Diabetes: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_DIABETE == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Varizes: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_VARIZES == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Epilepsia: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_EPILEPSIA == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Tabagismo: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_TABAGISMO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Tabagismo - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_TABAGISMO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Gestante: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_GESTANTE == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Semanas de Gestação: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_MES_GESTANTE.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Alcoolismo: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ALCOOLISMO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Alcoolismo - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ALCOOLISMO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Exercício Físico: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_EXERCICIO_FISICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Exercício - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_EXERCICIO_FISICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anticoncepcional: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTICONCEPCIONAL == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anticoncepcionais usados: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ANTICONCEPCIONAL, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Marcapasso: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_MARCAPASSO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Marcapasso - Observações: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_MARCAPASSO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Cirurgias: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_CIRURGIAS == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Cirurgias - Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_TX_CIRURGIAS, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Antecedentes Alérgicos: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTE_ALERGICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ALERGICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Antecedentes Oncológicos: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTE_ONCOLOGICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ONCOLOGICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Resultado de Exames: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_TX_RESULTADOS, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anotações Diversas: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_EXAME_FISICO, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    pdfDoc.Add(table);

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarAnamnesePDFTeste(PACIENTE paciente, MEDICOS medico,  MEDICOS_ENVIO envio)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();

                // Recupera informações
                MEDICOS_ENVIO solic = envio;
                PACIENTE_ANAMNESE anamnese = paciente.PACIENTE_ANAMNESE.Where(p => p.PAAM_IN_ATIVO == 1).FirstOrDefault();
                String nomeRel = "Anamnese_" + paciente.PACI_NM_NOME + "_" + solic.MEEV_GU_IDENTIFICADOR + ".pdf";
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

                // Atualiza envio
                MEDICOS_ENVIO envio1 = baseApp.GetEnvioById(solic.MEEV_CD_ID);
                envio1.MEEV_DT_REMESSA = DateTime.Now;
                envio1.MEEV_IN_ENVIOS += 1;
                Int32 voltaA = baseApp.ValidateEditEnvio(envio1);

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anamneses/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    // Dados do medico
                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(nomeMedico, meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table1.AddCell(cell);
                    }

                    String frase = classe + " CPF: " + usuario.USUA_NR_CPF;
                    cell = new PdfPCell(new Paragraph(frase, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    // Dados do medico
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(usuario.USUA_NM_NOME, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(classe, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + usuario.USUA_NR_CPF, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String endereco = String.Empty;
                    String enderecoCont = String.Empty;
                    if (empresa.EMPR_NM_ENDERECO != null)
                    {
                        endereco += empresa.EMPR_NM_ENDERECO;
                        if (empresa.EMPR_NM_NUMERO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_NUMERO;
                        }
                        if (empresa.EMPR_NM_COMPLEMENTO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                        }
                        if (empresa.EMPR_NM_BAIRRO != null)
                        {
                            enderecoCont += empresa.EMPR_NM_BAIRRO;
                        }
                        if (empresa.EMPR_NM_CIDADE != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                        }
                        if (empresa.UF != null)
                        {
                            enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                        }
                        if (empresa.EMPR_NR_CEP != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NR_CEP;
                        }
                    }

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da anamnese
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    Chunk chunk3 = new Chunk("A N A M N E S E", FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK));
                    Paragraph paragraph = new Paragraph(chunk3);
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(paragraph);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Verifica tipo de anamnese
                    if (paciente.PACI_IN_PADRAO_ANAMNESE == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Última Consulta: " + anamnese.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Dados do paciente
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph("  ");
                        pdfDoc.Add(line1);

                        // Grid
                        table = new PdfPTable(new float[] { 100f, 500f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        // Dados da anamnese
                        cell = new PdfPCell(new Paragraph("Motivo da Consulta: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_MOTIVO_CONSULTA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Queixa Principal: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_QUEIXA_PRINCIPAL, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("História Familiar: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_FAMILIAR, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        if (anamnese.PAAM_IN_FLAG_HISTORIA_SOCIAL == 1)
                        {
                            cell = new PdfPCell(new Paragraph("História Social: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_SOCIAL, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        cell = new PdfPCell(new Paragraph("História da Doença Atual: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Paragraph("Medicamentos em Uso: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_MEDICAMENTO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        if (anamnese.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA == 1)
                        {
                            cell = new PdfPCell(new Paragraph("História Patológica Progressiva: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Avaliação Cardiológica: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_AVALIACAO_CARDIOLOGICA_LONG, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_FLAG_RESPIRATORIO == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Avaliação Respiratória: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_RESPIRATORIO_LONG, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_FLAG_ABDOMEM == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Avaliação do Abdômem: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_ABDOMEM, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_FLAG_MEMBROS_INFERIORES == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Avaliação dos Membros Inferiores: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAN_NM_MEMBROS_INFERIORES, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_1 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_1 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_1, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_2 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_2 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_2, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_3 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_3 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_3, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_4 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_4 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_4, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_5 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_5 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_5, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_6 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_6 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_6, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_7 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_7 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_7, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_8 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_8 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_8, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_9 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_9 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_9, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_10 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_10 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_10, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        cell = new PdfPCell(new Paragraph("Diagnóstico: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_DIAGNOSTICO_1_LONG, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Conduta Adotada: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CONDUTA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        pdfDoc.Add(table);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Última Consulta: " + anamnese.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Dados do paciente
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph("  ");
                        pdfDoc.Add(line1);

                        // Grid
                        table = new PdfPTable(new float[] { 1f, 800f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        // Dados da anamnese
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_TX_COMPLETA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        pdfDoc.Add(table);
                    }

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarExameFisicoPDFTeste(PACIENTE paciente, MEDICOS medico,  MEDICOS_ENVIO envio)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();

                // Recupera informações
                MEDICOS_ENVIO solic = envio;
                PACIENTE_EXAME_FISICOS fisico = paciente.PACIENTE_EXAME_FISICOS.Where(p => p.PAEF_IN_ATIVO == 1).FirstOrDefault();
                List<PACIENTE_DADOS_EXAME_FISICO> dados = fisico.PACIENTE_DADOS_EXAME_FISICO.ToList();
                String nomeRel = "ExameFisico_" + paciente.PACI_NM_NOME + "_" + solic.MEEV_GU_IDENTIFICADOR + ".pdf";
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

                // Atualiza envio
                MEDICOS_ENVIO envio1 = baseApp.GetEnvioById(solic.MEEV_CD_ID);
                envio1.MEEV_DT_REMESSA = DateTime.Now;
                envio1.MEEV_IN_ENVIOS += 1;
                Int32 voltaA = baseApp.ValidateEditEnvio(envio1);

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/ExamesFisicos/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    // Dados do medico
                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(nomeMedico, meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table1.AddCell(cell);
                    }

                    String frase = classe + " CPF: " + usuario.USUA_NR_CPF;
                    cell = new PdfPCell(new Paragraph(frase, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    // Dados do medico
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(usuario.USUA_NM_NOME, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(classe, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + usuario.USUA_NR_CPF, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String endereco = String.Empty;
                    String enderecoCont = String.Empty;
                    if (empresa.EMPR_NM_ENDERECO != null)
                    {
                        endereco += empresa.EMPR_NM_ENDERECO;
                        if (empresa.EMPR_NM_NUMERO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_NUMERO;
                        }
                        if (empresa.EMPR_NM_COMPLEMENTO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                        }
                        if (empresa.EMPR_NM_BAIRRO != null)
                        {
                            enderecoCont += empresa.EMPR_NM_BAIRRO;
                        }
                        if (empresa.EMPR_NM_CIDADE != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                        }
                        if (empresa.UF != null)
                        {
                            enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                        }
                        if (empresa.EMPR_NR_CEP != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NR_CEP;
                        }
                    }

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 120);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    Chunk chunk3 = new Chunk("E X A M E   F Í S I C O", FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK));
                    Paragraph paragraph = new Paragraph(chunk3);
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(paragraph);

                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados do exame
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data da Consulta: " + fisico.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Grid
                    table = new PdfPTable(new float[] { 200f, 700f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    // Dados da anamnese
                    // Exame Fisico
                    cell = new PdfPCell(new Paragraph("Pressão Sanguinea (mmHg): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_PA_ALTA.ToString() + " x " + fisico.PAEF_NR_PA_BAIXA, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Frequencia Cardiaca (bpm): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_FREQUENCIA_CARDIACA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Temperatura (oC): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_TEMPERATURA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Peso (Kg): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_PESO.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Altura (cm): ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_ALTURA.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("IMC: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_VL_IMC.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Histórico
                    if (dados.Count > 0)
                    {
                        // Linha Horizontal
                        line1 = new Paragraph("  ");
                        pdfDoc.Add(line1);  

                        table = new PdfPTable(new float[] { 60f, 60f, 60f, 60f, 60f, 60f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Data", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Peso (kg)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Altura (cm)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Pressão Sanguinea (mmHg)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Frequencia Cardíaca (bpm)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Temperatura (oC)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (PACIENTE_DADOS_EXAME_FISICO item in dados)
                        {
                            if (item.PDEF_DT_DATA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_DT_DATA.Value.ToShortDateString() + " " + item.PDEF_DT_DATA.Value.ToShortTimeString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_PESO != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_PESO.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_ALTURA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_ALTURA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_PRESSAO_ALTA != null & item.PDEF_IN_PRESSAO_BAIXA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_PRESSAO_ALTA.ToString() + " x " + item.PDEF_IN_PRESSAO_BAIXA.ToString(), meuFont1))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_FREQUENCIA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_FREQUENCIA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }

                            if (item.PDEF_IN_TEMPERATURA != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.PDEF_IN_TEMPERATURA.ToString(), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                table.AddCell(cell);
                            }
                        }
                        pdfDoc.Add(table);
                    }

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    table = new PdfPTable(new float[] { 200f, 700f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Hipertensão: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_HIPERTENSAO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Hipotensão: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_HIPOTENSAO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Diabetes: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_DIABETE == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Varizes: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_VARIZES == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Epilepsia: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_EPILEPSIA == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Tabagismo: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_TABAGISMO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Tabagismo - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_TABAGISMO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Gestante: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_GESTANTE == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Semanas de Gestação: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_NR_MES_GESTANTE.ToString(), meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Alcoolismo: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ALCOOLISMO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Alcoolismo - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ALCOOLISMO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Exercício Físico: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_EXERCICIO_FISICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Exercício - Frequência: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_EXERCICIO_FISICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anticoncepcional: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTICONCEPCIONAL == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anticoncepcionais usados: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ANTICONCEPCIONAL, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Marcapasso: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_MARCAPASSO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Marcapasso - Observações: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_MARCAPASSO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Cirurgias: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_CIRURGIAS == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Cirurgias - Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_TX_CIRURGIAS, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Antecedentes Alérgicos: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTE_ALERGICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ALERGICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Antecedentes Oncológicos: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_IN_ANTE_ONCOLOGICO == 1 ? "Sim" : "Não", meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Descrição: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_ONCOLOGICO_LONG, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Resultado de Exames: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_TX_RESULTADOS, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Anotações Diversas: ", meuFont1Bold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(fisico.PAEF_DS_EXAME_FISICO, meuFont1))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    pdfDoc.Add(table);

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarAnamnesePDFTesteSono(PACIENTE paciente, MEDICOS medico,  MEDICOS_ENVIO envio)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String hoje = DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString();

                // Recupera informações
                MEDICOS_ENVIO solic = envio;
                PACIENTE_ANAMNESE anamnese = paciente.PACIENTE_ANAMNESE.Where(p => p.PAAM_IN_ATIVO == 1).FirstOrDefault();
                String nomeRel = "Anamnese_" + paciente.PACI_NM_NOME + "_" + solic.MEEV_GU_IDENTIFICADOR + ".pdf";
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

                // Atualiza envio
                MEDICOS_ENVIO envio1 = baseApp.GetEnvioById(solic.MEEV_CD_ID);
                envio1.MEEV_DT_REMESSA = DateTime.Now;
                envio1.MEEV_IN_ENVIOS += 1;
                Int32 voltaA = baseApp.ValidateEditEnvio(envio1);

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Envio/" + envio.MEEV_CD_ID.ToString() + "/Anamneses/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    // Dados do medico
                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(nomeMedico, meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table1.AddCell(cell);
                    }

                    String frase = classe + " CPF: " + usuario.USUA_NR_CPF;
                    cell = new PdfPCell(new Paragraph(frase, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    // Dados do medico
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(usuario.USUA_NM_NOME, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    if (usuario.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(usuario.ESPECIALIDADE.ESPE_NM_NOME, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(classe, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + usuario.USUA_NR_CPF, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String endereco = String.Empty;
                    String enderecoCont = String.Empty;
                    if (empresa.EMPR_NM_ENDERECO != null)
                    {
                        endereco += empresa.EMPR_NM_ENDERECO;
                        if (empresa.EMPR_NM_NUMERO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_NUMERO;
                        }
                        if (empresa.EMPR_NM_COMPLEMENTO != null)
                        {
                            endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                        }
                        if (empresa.EMPR_NM_BAIRRO != null)
                        {
                            enderecoCont += empresa.EMPR_NM_BAIRRO;
                        }
                        if (empresa.EMPR_NM_CIDADE != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                        }
                        if (empresa.UF != null)
                        {
                            enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                        }
                        if (empresa.EMPR_NR_CEP != null)
                        {
                            enderecoCont += " - " + empresa.EMPR_NR_CEP;
                        }
                    }

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da anamnese
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    Chunk chunk3 = new Chunk("A N A M N E S E", FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK));
                    Paragraph paragraph = new Paragraph(chunk3);
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(paragraph);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Verifica tipo de anamnese
                        cell = new PdfPCell(new Paragraph("Última Consulta: " + anamnese.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToLongDateString(), meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Dados do paciente
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Nome do Paciente: " + paciente.PACI_NM_NOME, meuFont1Bold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Data Nasc.: " + paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString(), meuFont1));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph("  ");
                        pdfDoc.Add(line1);

                        // Grid
                        table = new PdfPTable(new float[] { 200f, 500f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        // Rotina do Sono
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Rotina do Sono *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Principal queixa do sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_PRINCIPAL_QUEIXA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sintomas :", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_SINTOMAS, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Horário regulares para dormir e acordar: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_HORARIO_REGULAR_NOVO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Latência para o início do sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_LATENCIA_NOVO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Duração do sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_DURACAO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Paragraph("Manutenção da rotina nos fins de semana: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ROTINA_FDS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        if (anamnese.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Cochilos diurnos: ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_COCHILOS == 1 ? "Sim" : "Não"), meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        // Higinene do Sono
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Higiene do Sono *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Assiste TV na cama: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TVCAMA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fica deitado na cama quando está sem sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DEITADO_PERDE_SONO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Lê na cama: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_LECAMA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Usa celular na cama : ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_CELULAR_CAMA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fuma: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_FUMA_NOITE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Último horário de consumo de álcool ou bebidas com cafeína : ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_ULTIMO_ALCOOL, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Refeições pesadas à noite: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_REFEICAO_PESADA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Faz todas as refeições: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TODAS_REFEICOES == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Café da manhã: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_CAFE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Almoço: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ALMOCO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Lanche: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_LANCHE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Jantar: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_JANTAR == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Exercício físico à noite: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_EXERCICIO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Frequencia semanal do exercício físico: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_EXERCICIO_FREQ, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);


                        // FRagmentacao do Sono
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Fragmentação do Sono *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantas vezes desperta durante o sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_QUANTAS_DESPERTA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quais motivos: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_MOTIVOS_DESPERTA, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quanto tempo demora para retornar ao sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_TEMPO_PEGAR_SONO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fica deitado na cama quando perde o sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DEITADO_PERDE_SONO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantas vezes vai ao banheiro para urinar durante a noite de sono : ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_URINA_NOITE, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Ainda durante do Sono
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Ainda Durante o Sono *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Presença de engasgos ou sensação de sufocamento: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ENGASGOS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Tosse: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TOSSE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Refluxo: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_REFLUXO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sudorese: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_SUDORESE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Posição preferencial para dormir: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_POSICAO_DORMIR, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Range os dentes durante o sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_RANGE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sensação de tensão ou rigidez nos músculos da face: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_RIGIDEZ_FACE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Apneia testemunhada: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_APNEIA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Ronco alto que se ouve do quarto ao lado: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_RONCO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Movimentos agressivos durante o sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_AGRESSIVO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fala durante o sono: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_FALA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Pesadelos: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_PESADELO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sonambulismo: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_SONANBULISMO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Encenação durante os sonhos: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ENCENACAO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Movimentos periódicos de membros: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_MOVE_MEMBRO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Cãibras: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_CAIBRAS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Ambiente
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Sobre o Ambiente Durante o Sono *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Seu quarto é aconchegante, confortável: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ACONCHEGANTE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Tem barulho: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_BARULHO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("A temperatura do ambiente é confortável: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TEMPERATURA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Presença de outras pessoas: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_PESSOAS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Presença de animais: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ANIMAIS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quais atividades realiza no quarto além de dormir: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_ATIVIDADES, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Socio
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Sobre Condições Sócio-Econômicas *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Problemas sociais e financeiros: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_FINANCAS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Acesso a serviços de saúde: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_ACESSO_SAUDE == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Matinal
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Sintomas Matinais *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sono reparador: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_REPARADOR == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sonolência excessiva: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_SONOLENCIA == 1 ? "Sim" : (anamnese.PAAM_DS_SONO_TIPO_RESPIRACAO == 0 ? "Não" : "Esporadicamente")), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Boca seca ao despertar: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_BOCA_SECA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Dor de cabeça: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DOR_CABECA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Congestão nasal: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_CONGESTAO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Refluxo ou Azia: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_AZIA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Diurnas
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Funções Diurnas *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sonolência e/ou acidentes causados por sonolência: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_SONOLENCIA_DIURNA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Cansaço: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_CANSACO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fadiga: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_FADIGA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Irritabilidade: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_IRRITA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Deficit de Concentração: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DEFICIT_CONCENTRA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Deficit de Memória: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DEFICIT_MEMO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Dor: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DOR == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        // Outros
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("***** Outros *****", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(" ", meuFont3Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 2;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Tipo de respiração: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TIPO_RESPIRACAO == 1 ? "Nasal" : (anamnese.PAAM_DS_SONO_TIPO_RESPIRACAO == 2 ? "Oronasal" : "Mista")), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sensação de tensão ou rigidez nos músculos da face: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_RIGIDEZ_FACE_OUTROS == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Disfunção sexual: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_DISFUNCAO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Alterações ponderais: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_PONDERAL == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Medicamentos e outras substâncias em uso: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_MEDICAMENTOS, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Comorbidades: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_COMORBIDADES, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Cirurgias prévias: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_CIRURGIAS, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Sensações desagradáveis nas pernas, principalmente à noite, final do dia, ou quando sentado em repouso: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_SENSACAO_PERNA == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Trabalha ou trabalhou em turno : ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((anamnese.PAAM_DS_SONO_TURNO == 1 ? "Sim" : "Não"), meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Dados da polissonografia: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_POLISONO, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Mallampati: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_MALLAMPATI, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Overlap: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_OVERLAP, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Patologias associadas: ", meuFont1Bold))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_SONO_PATOLOGIAS, meuFont1))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        if (anamnese.PAAM_IN_CAMPO_1 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_1 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_1, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_2 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_2 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_2, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_3 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_3 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_3, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_4 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_4 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_4, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_5 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_5 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_5, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_6 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_6 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_6, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_7 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_7 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_7, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_8 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_8 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_8, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_9 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_9 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_9, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }

                        if (anamnese.PAAM_IN_CAMPO_10 == 1)
                        {
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_NM_CAMPO_10 + ": ", meuFont1Bold))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(anamnese.PAAM_DS_CAMPO_10, meuFont1))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            cell.Colspan = 1;
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);
                        }
                        pdfDoc.Add(table);

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public DTO_Medico_Envio MontarMedicoEnvioDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.MEDICOS_ENVIO
                    .Where(l => l.MEEV_CD_ID == mediId)
                    .Select(l => new DTO_Medico_Envio
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        MEDC_CD_ID = l.MEDC_CD_ID,
                        MEEV_CD_ID = l.MEEV_CD_ID,
                        MEEV_DS_SINTOMAS = l.MEEV_DS_SINTOMAS,
                        MEEV_DT_ENVIO = l.MEEV_DT_ENVIO,
                        MEEV_DT_NOITE_FINAL = l.MEEV_DT_NOITE_FINAL,
                        MEEV_DT_NOITE_INICIO = l.MEEV_DT_NOITE_INICIO,
                        MEEV_DT_REMESSA = l.MEEV_DT_REMESSA,
                        MEEV_GU_IDENTIFICADOR = l.MEEV_GU_IDENTIFICADOR,
                        MEEV_IN_ANAMNESE = l.MEEV_IN_ANAMNESE,
                        MEEV_IN_ATIVO = l.MEEV_IN_ATIVO,
                        MEEV_IN_ENVIADO = l.MEEV_IN_ENVIADO,
                        MEEV_IN_ENVIOS = l.MEEV_IN_ENVIOS,
                        MEEV_IN_IAH = l.MEEV_IN_IAH,
                        MEEV_IN_IAH_RESIDUAL = l.MEEV_IN_IAH_RESIDUAL,
                        MEEV_IN_NUM_NOITES = l.MEEV_IN_NUM_NOITES,
                        MEEV_IN_PERCENTUAL = l.MEEV_IN_PERCENTUAL,
                        MEEV_NM_EQUIPAMENTO = l.MEEV_NM_EQUIPAMENTO,
                        MEEV_NM_MASCARA = l.MEEV_NM_MASCARA,
                        MEEV_NM_TITULO = l.MEEV_NM_TITULO,
                        MEEV_NR_IAH_RESIDUAL = l.MEEV_NR_IAH_RESIDUAL,
                        MEEV_NR_MEDIA_USO = l.MEEV_NR_MEDIA_USO,
                        MEEV_NR_PARAM_PRESSAO = l.MEEV_NR_PARAM_PRESSAO,
                        MEEV_NR_PRESSAO_POSITIVA = l.MEEV_NR_PRESSAO_POSITIVA,
                        MEEV_TX_MENSAGEM = l.MEEV_TX_MENSAGEM,
                        PACI_CD_ID = l.PACI_CD_ID,
                        TIEN_CD_ID = l.TIEN_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public ActionResult MontarTelaTextoEnvio()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Carrega listas
                if (Session["ListaTextoMedico"] == null)
                {
                    listaMasterTexto = CarregarMedicoTexto().OrderBy(p => p.METX_NM_NOME).ToList();
                    Session["ListaTextoMedico"] = listaMasterTexto;
                }
                ViewBag.Listas = (List<MEDICOS_MENSAGEM>)Session["ListaTextoMedico"];

                if (Session["MensMedico"] != null)
                {
                    if ((Int32)Session["MensMedico"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 111)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0712", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 112)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0713", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICOS_MENSAGEM", "Medico", "MontarTelaTextoEnvio");

                // Abre view
                Session["MensMedico"] = null;
                objetoTexto = new MEDICOS_MENSAGEM();
                return View(objetoTexto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirTextoEnvio()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_LOCACAO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medico - Inclusão";
                        return RedirectToAction("MontarTelaMedico");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Texto de Envio - Inclusão";
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                if (Session["MensMedico"] != null)
                {
                    if ((Int32)Session["MensMedico"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                var fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICO_MENSAGEM_INCLUIR", "Medico", "IncluirTextoEnvio");

                // Prepara view
                Session["MensMedico"] = null;
                MEDICOS_MENSAGEM item = new MEDICOS_MENSAGEM();
                MedicoMensagemViewModel vm = Mapper.Map<MEDICOS_MENSAGEM, MedicoMensagemViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.METX_IN_ATIVO = 1;
                vm.METX_DT_CRIACAO = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.METX_IN_FIXO = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTextoEnvio(MedicoMensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.METX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.METX_NM_NOME);
                    vm.METX_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.METX_TX_TEXTO);

                    // Critica

                    // Recupera
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();

                    // Preparação
                    MEDICOS_MENSAGEM item = Mapper.Map<MedicoMensagemViewModel, MEDICOS_MENSAGEM>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Serializa
                    String json = JsonConvert.SerializeObject(item);
                    Session["JSONTextoEnvio"] = json;

                    // Processa
                    Int32 volta = baseApp.ValidateCreateTextoMensagem(item);
                    Session["IdTexto"] = item.METX_CD_ID;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0711", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterTexto = new List<MEDICOS_MENSAGEM>();
                    Session["ListaTextoMedico"] = null;
                    Session["MedicoTextoAlterada"] = 1;
                    Session["MedicoTextos"] = null;
                    Session["IdTexto"] = item.METX_CD_ID;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O texto de envio " + item.METX_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensMedico"] = 61;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Texto de Envio - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("MontarTelaTextoEnvio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTextoEnvio(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Texto de Envio - Edição";
                        return RedirectToAction("VoltarAnexoTexto");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Texto de Envio - Edição";

                // Trata mensagens
                if (Session["MensMedico"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensMedico"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedico"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara view
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";
                Session["MensMedico"] = null;

                MEDICOS_MENSAGEM item = baseApp.GetTextoMensagemById(id);
                MedicoMensagemViewModel vm = Mapper.Map<MEDICOS_MENSAGEM, MedicoMensagemViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEXTO_ENVIO_EDITAR", "Medico", "EditarTextoEnvio");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTextoEnvio(MedicoMensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.METX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.METX_NM_NOME);
                    vm.METX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.METX_NM_NOME);

                    // Executa a operação
                    MEDICOS_MENSAGEM item = Mapper.Map<MedicoMensagemViewModel, MEDICOS_MENSAGEM>(vm);
                    Int32 volta = baseApp.ValidateEditTextoMensagem(item);

                    // Verifica retorno
                    Session["IdTexto"] = item.METX_CD_ID;
                    Session["MedicoTextoAlterada"] = 1;
                    Session["ListaTextoMedico"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O texto de envio " + item.METX_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensMedico"] = 61;

                    // Monta Log
                    DTO_Medico_Mensagem dto = MontarTextoEnvioDTO(item.METX_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Texto de Envio - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("MontarTelaTextoEnvio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medico";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTextoEnvio(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_LOCACAO__EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Texto de Envio - Exclusão";
                        return RedirectToAction("VoltarAnexoTexto");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                MEDICOS_MENSAGEM item = baseApp.GetTextoMensagemById(id);
                if (item.METX_IN_FIXO == 1)
                {
                    return RedirectToAction("MontarTelaTextoEnvio");
                }

                // Checa integridade
                if (item.MEDICOS_ENVIO.Count() > 0)
                {
                    Session["MensMedico"] = 111;
                    return RedirectToAction("MontarTelaTextoEnvio");
                }

                // Verifica a possibilidade de exclusao
                List<MEDICOS_MENSAGEM> conts = CarregarMedicoTexto().ToList();
                if (conts.Count() == 1)
                {
                    Session["MensMedico"] = 112;
                    return RedirectToAction("MontarTelaTextoEnvio");
                }

                // Processa
                item.METX_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditTextoMensagem(item);

                // Estado
                Session["MedicoTextoAlterada"] = 1;
                Session["ListaTextoMedico"] = null;
                Session["MedicoTextos"] = null;

                // Monta Log
                DTO_Medico_Mensagem dto = MontarTextoEnvioDTO(item.METX_CD_ID);
                String json = JsonConvert.SerializeObject(dto);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Texto de Envio - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta3 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O texto de envio " + item.METX_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensMedico"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaTextoEnvio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public DTO_Medico_Mensagem MontarTextoEnvioDTO(Int32 id)
        {
            using (var context = new CRMSysDBEntities())
            {
                var contrato = context.MEDICOS_MENSAGEM
                    .Where(l => l.METX_CD_ID == id)
                    .Select(l => new DTO_Medico_Mensagem
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        METX_DT_CRIACAO = l.METX_DT_CRIACAO,
                        METX_CD_ID = l.METX_CD_ID,
                        METX_IN_ATIVO = l.METX_IN_ATIVO,
                        METX_NM_NOME = l.METX_NM_NOME,
                        METX_TX_TEXTO = l.METX_TX_TEXTO,
                    })
                    .FirstOrDefault();
                return contrato;
            }
        }

        public List<MEDICOS_MENSAGEM> CarregarMedicoTexto()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MEDICOS_MENSAGEM> conf = new List<MEDICOS_MENSAGEM>();
                if (Session["MedicoTextos"] == null)
                {
                    conf = baseApp.GetAllTextoMensagem(idAss);
                }
                else
                {
                    if ((Int32)Session["MedicoTextoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTextoMensagem(idAss);
                    }
                    else
                    {
                        conf = (List<MEDICOS_MENSAGEM>)Session["MedicoTextos"];
                    }
                }
                Session["MedicoTextoAlterada"] = 0;
                Session["MedicoTextos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medico";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medico", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VoltarAnexoTexto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarTextoEnvio", new { id = (Int32)Session["IdTexto"] });
        }

    }
}