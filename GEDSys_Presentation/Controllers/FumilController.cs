using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using Canducci.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EntitiesServices.Attributes;
using OfficeOpenXml.Table;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using System.Reflection;
using ERP_Condominios_Solution.Classes;
using System.Diagnostics;
using ApplicationServices.Services;
using GEDSys_Presentation.App_Start;

namespace GEDSys_Presentation.Controllers
{
    public class FunilController : Controller
    {
        private readonly IFunilAppService baseApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ICRMAppService crmApp;
        private readonly ILogAppService logApp;
        private readonly IAcessoMetodoAppService aceApp;

        private String msg;
        private Exception exception;
        FUNIL objeto = new FUNIL();
        FUNIL objetoAntes = new FUNIL();
        List<FUNIL> listaMaster = new List<FUNIL>();
        String extensao;

        public FunilController(IFunilAppService baseApps, IConfiguracaoAppService confApps, ICRMAppService crmApps, IUsuarioAppService usuApps, LogAppService logApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            usuApp = usuApps;
            confApp = confApps;
            crmApp = crmApps;
            logApp = logApps;
            aceApp = aceApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            LEAD item = new LEAD();
            LeadViewModel vm = Mapper.Map<LEAD, LeadViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaFunil()
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if ((List<FUNIL>)Session["ListaFunilX"] == null)
                {
                    listaMaster = CarregaFunil();
                    Session["ListaFunilX"] = listaMaster;
                }
                ViewBag.Listas = (List<FUNIL>)Session["ListaFunilX"];
                ViewBag.Title = "Funil";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4_1.pdf";

                // Mensagens
                if (Session["MensFunil"] != null)
                {
                    if ((Int32)Session["MensFunil"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0193", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0194", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "FUNIL", "Funil", "MontarTelaFunil");

                // Abre view
                Session["MensFunil"] = null;
                Session["VoltaFunil"] = 1;
                Session["TabFunil"] = 1;
                objeto = new FUNIL();
                objeto.FUNI_IN_ATIVO = 1;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroFunil()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaFunilX"] = null;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoFunil()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss);
                Session["ListaFunilX"] = listaMaster;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFunilX"] = null;
            return RedirectToAction("MontarTelaFunil");
        }

       [HttpGet]
        public ActionResult IncluirFunil()
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

                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                List<SelectListItem> aviso = new List<SelectListItem>();
                aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                aviso.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4_2.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "FUNIL_INCLUIR", "Funil", "IncluirFunil");

                // Prepara view
                FUNIL item = new FUNIL();
                FunilViewModel vm = Mapper.Map<FUNIL, FunilViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.FUNI_DT_CADASTRO = DateTime.Today;
                vm.FUNI_IN_ATIVO = 1;
                vm.FUNI_IN_FIXO = 0;
                vm.FUNI_IN_TIPO = 1;
                vm.FUNI_IN_CLIENTE = 0;
                vm.FUNI_IN_LEAD = 0;
                vm.FUNI_IN_SISTEMA = 6;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funis";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funis", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirFunil(FunilViewModel vm)
        {
            List<SelectListItem> proposta = new List<SelectListItem>();
            proposta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            proposta.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Proposta = new SelectList(proposta, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUNI_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_DS_DESCRICAO);
                    vm.FUNI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_NM_NOME);
                    vm.FUNI_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_SG_SIGLA);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    FUNIL item = Mapper.Map<FunilViewModel, FUNIL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0193", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O funil " + item.FUNI_NM_NOME.ToUpper() + " foi criado com sucesso";
                    Session["MensFunil"] = 61;

                    // Sucesso
                    listaMaster = new List<FUNIL>();
                    Session["ListaFunilX"] = null;
                    Session["IdFunil"] = item.FUNI_CD_ID;
                    Session["FunilAlterada"] = 1;
                    Session["TabFunil"] = 2;
                    Session["FlagAlteraEstado"] = 1;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarFunil(Int32 id)
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

                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Listas
                List<SelectListItem> aviso = new List<SelectListItem>();
                aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                aviso.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4_3.pdf";

                // Mensagens
                if (Session["MensFunil"] != null)
                {
                    if ((Int32)Session["MensFunil"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0195", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara view
                FUNIL item = baseApp.GetItemById(id);
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                Int32 etapa = 0;
                if (item.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList().Count < conf.CONF_IN_ETAPAS_CRM)
                {
                    etapa = 1;
                }
                ViewBag.Etapa = etapa;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "FUNIL_ALTERAR", "Funil", "EditarFunil");

                // Indicadores
                Session["VoltaFunil"] = 1;
                objetoAntes = item;
                Session["Funil"] = item;
                Session["IdFunil"] = id;
                FunilViewModel vm = Mapper.Map<FUNIL, FunilViewModel>(item);
                return View(vm);

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarFunil(FunilViewModel vm)
        {
            List<SelectListItem> proposta = new List<SelectListItem>();
            proposta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            proposta.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Proposta = new SelectList(proposta, "Value", "Text");
            List<SelectListItem> aviso = new List<SelectListItem>();
            aviso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aviso.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aviso = new SelectList(aviso, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUNI_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_DS_DESCRICAO);
                    vm.FUNI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_NM_NOME);
                    vm.FUNI_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUNI_SG_SIGLA);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FUNIL item = Mapper.Map<FunilViewModel, FUNIL>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O funil " + item.FUNI_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensFunil"] = 61;

                    // Sucesso
                    listaMaster = new List<FUNIL>();
                    Session["ListaFunilX"] = null;
                    Session["FunilAlterada"] = 1;
                    Session["FlagAlteraEstado"] = 1;
                    Session["FlagAlteraEstado"] = 1;
                    return RedirectToAction("MontarTelaFunil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirFunil(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                FUNIL item = baseApp.GetItemById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUNI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensFunil"] = 4;
                    return RedirectToAction("MontarTelaFunil", "Funil");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O funil " + item.FUNI_NM_NOME + " foi excluído com sucesso";
                Session["MensFunil"] = 61;

                listaMaster = new List<FUNIL>();
                Session["ListaFunilX"] = null;
                Session["FunilAlterada"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarFunil(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                FUNIL item = baseApp.GetItemById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUNI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O funil " + item.FUNI_NM_NOME + " foi reativado com sucesso";
                Session["MensFunil"] = 61;

                listaMaster = new List<FUNIL>();
                Session["ListaFunilX"] = null;
                Session["FunilAlterada"] = 1;
                Session["FlagAlteraEstado"] = 1;
                return RedirectToAction("MontarTelaFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoFunil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarFunil", new { id = (Int32)Session["IdFunil"] });
        }

        [HttpGet]
        public ActionResult EditarEtapa(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Lists
                List<SelectListItem> encerrra = new List<SelectListItem>();
                encerrra.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                encerrra.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Encerra = new SelectList(encerrra, "Value", "Text");
                List<SelectListItem> mail = new List<SelectListItem>();
                mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Mail = new SelectList(mail, "Value", "Text");
                List<SelectListItem> sms = new List<SelectListItem>();
                sms.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                sms.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.SMS = new SelectList(sms, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4_5.pdf";

                // Mensagens
                if (Session["MensFunil"] != null)
                {
                    if ((Int32)Session["MensFunil"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }
                Session["MensFunil"] = null;

                // Recupera ultima etapa
                FUNIL funil = (FUNIL)Session["Funil"];
                Int32? ordem = 1;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList();
                if (etapas.Count > 0)
                {
                    ordem = etapas.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
                }
                Session["Ordem"] = ordem;
                Session["Etapas"] = etapas;

                // Verifica
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                if (etapas.Count > conf.CONF_IN_ETAPAS_CRM)
                {
                    Session["MensFunil"] = 5;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                Session["OrdemMax"] = conf.CONF_IN_ETAPAS_CRM;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ETAPA_ALTERAR", "Funil", "EditarEtapa");

                // Prepara view
                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                objetoAntes = (FUNIL)Session["Funil"];
                FunilEtapaViewModel vm = Mapper.Map<FUNIL_ETAPA, FunilEtapaViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEtapa(FunilEtapaViewModel vm)
        {
            List<SelectListItem> encerrra = new List<SelectListItem>();
            encerrra.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            encerrra.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Encerra = new SelectList(encerrra, "Value", "Text");
            List<SelectListItem> mail = new List<SelectListItem>();
            mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Mail = new SelectList(mail, "Value", "Text");
            List<SelectListItem> sms = new List<SelectListItem>();
            sms.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sms.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.SMS = new SelectList(sms, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Sanitização
                    vm.FUET_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_DS_DESCRICAO);
                    vm.FUET_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_NM_NOME);
                    vm.FUET_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_SG_SIGLA);

                    // Recupera ordem
                    Int32 ordem = (Int32)Session["Ordem"];
                    Int32 ordemMax = (Int32)Session["OrdemMax"];
                    List<FUNIL_ETAPA> etapas = (List<FUNIL_ETAPA>)Session["Etapas"];
                    Int32? atual = vm.FUET_IN_ORDEM;

                    // Verifica existencia da ordem
                    Int32 flagOrdem = 0;
                    FUNIL_ETAPA etapa = etapas.Find(p => p.FUET_IN_ORDEM == atual);
                    if (etapa != null)
                    {
                        flagOrdem = 1;
                    }

                    // Verifica ultima
                    if (atual > ordem)
                    {
                        Session["MensFunil"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Valida flags
                    Int32 id = vm.FUET_CD_ID;
                    List<FUNIL_ETAPA> lista = baseApp.GetItemById(vm.FUNI_CD_ID).FUNIL_ETAPA.ToList();
                    if (vm.FUET_IN_ENCERRA == 1)
                    {
                        lista = lista.Where(p => p.FUET_IN_ENCERRA == 1 & p.FUET_CD_ID != id).ToList();
                        if (lista.Count > 0)
                        {
                            Session["MensFunil"] = 8;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }


                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FUNIL_ETAPA item = Mapper.Map<FunilEtapaViewModel, FUNIL_ETAPA>(vm);
                    Int32 volta = baseApp.ValidateEditEtapa(item);
                    FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                    etapas = funil.FUNIL_ETAPA.ToList();
                    Int32 indice = item.FUET_CD_ID;

                    // Rearruma etapas
                    Int32? nova = 0;
                    if (flagOrdem == 1)
                    {
                        etapas = etapas.Where(p => p.FUET_IN_ORDEM >= atual & p.FUET_CD_ID != indice).OrderBy(x => x.FUET_IN_ORDEM).ToList();
                        foreach (FUNIL_ETAPA eta in etapas)
                        {
                            nova = eta.FUET_IN_ORDEM + 1;
                            eta.FUET_IN_ORDEM = nova;
                            Int32 volta1 = GravaOrdem(eta);
                        }
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A etapa " + item.FUET_NM_NOME.ToUpper() + " do funil " + funil.FUNI_NM_NOME + " foi alterada com sucesso";
                    Session["MensFunil"] = 61;

                    // Verifica retorno
                    Session["TabFunil"] = 2;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirEtapa(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditEtapa(item);
                Session["TabFunil"] = 2;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A etapa " + item.FUET_NM_NOME.ToUpper() + " do funil " + funil.FUNI_NM_NOME + " foi excluida com sucesso";
                Session["MensFunil"] = 61;

                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEtapa(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                FUNIL_ETAPA item = baseApp.GetEtapaById(id);
                FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                objetoAntes = (FUNIL)Session["Funil"];
                item.FUET_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditEtapa(item);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A etapa " + item.FUET_NM_NOME.ToUpper() + " do funil " + funil.FUNI_NM_NOME + " foi reativada com sucesso";
                Session["MensFunil"] = 61;

                return RedirectToAction("VoltarAnexoFunil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirEtapa()
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
                    if (usuario.PERFIL.PERF_IN_ALTERACAO_FUNIL == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Funis";
                        return RedirectToAction("MontarTelaFunil", "Funil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Lists
                List<SelectListItem> encerrra = new List<SelectListItem>();
                encerrra.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                encerrra.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Encerra = new SelectList(encerrra, "Value", "Text");
                List<SelectListItem> mail = new List<SelectListItem>();
                mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Mail = new SelectList(mail, "Value", "Text");
                List<SelectListItem> sms = new List<SelectListItem>();
                sms.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                sms.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.SMS = new SelectList(sms, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4_4.pdf";

                // Mensagens
                if (Session["MensFunil"] != null)
                {
                    if ((Int32)Session["MensFunil"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensFunil"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                    }
                }
                Session["MensFunil"] = null;

                // Recupera ultima etapa
                FUNIL funil = baseApp.GetItemById((Int32)Session["IdFunil"]);
                Int32? ordem = 1;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.Where(p => p.FUET_IN_ATIVO == 1).ToList();
                if (etapas.Count > 0)
                {
                    ordem = etapas.OrderByDescending(p => p.FUET_IN_ORDEM).FirstOrDefault().FUET_IN_ORDEM;
                    ordem++;
                }
                Session["Ordem"] = ordem;
                Session["Etapas"] = etapas;

                // Verifica
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                if (etapas.Count > conf.CONF_IN_ETAPAS_CRM)
                {
                    Session["MensFunil"] = 5;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                Session["OrdemMax"] = conf.CONF_IN_ETAPAS_CRM;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ETAPA_INCLUIR", "Funil", "IncluirEtapa");

                // Prepara view
                FUNIL_ETAPA item = new FUNIL_ETAPA();
                FunilEtapaViewModel vm = Mapper.Map<FUNIL_ETAPA, FunilEtapaViewModel>(item);
                vm.FUNI_CD_ID = (Int32)Session["IdFunil"];
                vm.FUET_IN_ATIVO = 1;
                vm.FUET_IN_ORDEM = ordem;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEtapa(FunilEtapaViewModel vm)
        {
            List<SelectListItem> encerrra = new List<SelectListItem>();
            encerrra.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            encerrra.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Encerra = new SelectList(encerrra, "Value", "Text");
            List<SelectListItem> mail = new List<SelectListItem>();
            mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Mail = new SelectList(mail, "Value", "Text");
            List<SelectListItem> sms = new List<SelectListItem>();
            sms.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sms.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.SMS = new SelectList(sms, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.FUET_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_DS_DESCRICAO);
                    vm.FUET_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_NM_NOME);
                    vm.FUET_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.FUET_SG_SIGLA);

                    // Recupera ordem
                    Int32 ordem = (Int32)Session["Ordem"];
                    Int32 ordemMax = (Int32)Session["OrdemMax"];
                    List<FUNIL_ETAPA> etapas = (List<FUNIL_ETAPA>)Session["Etapas"];
                    Int32? atual = vm.FUET_IN_ORDEM;

                    // Verifica existencia da ordem
                    Int32 flagOrdem = 0;
                    FUNIL_ETAPA etapa = etapas.Find(p => p.FUET_IN_ORDEM == atual);
                    if (etapa != null)
                    {
                        flagOrdem = 1;
                    }

                    // Verifica ultima
                    if (atual > ordem)
                    {
                        Session["MensFunil"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Valida flags
                    List<FUNIL_ETAPA> lista = baseApp.GetItemById(vm.FUNI_CD_ID).FUNIL_ETAPA.ToList();
                    if (vm.FUET_IN_ENCERRA == 1)
                    {
                        lista = lista.Where(p => p.FUET_IN_ENCERRA == 1).ToList();
                        if (lista.Count > 0)
                        {
                            Session["MensFunil"] = 8;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    FUNIL_ETAPA item = Mapper.Map<FunilEtapaViewModel, FUNIL_ETAPA>(vm);
                    Int32 volta = baseApp.ValidateCreateEtapa(item);
                    FUNIL funil = baseApp.GetItemById(item.FUNI_CD_ID);
                    etapas = funil.FUNIL_ETAPA.ToList();
                    Int32 indice = item.FUET_CD_ID;

                    // Rearruma etapas
                    Int32? nova = 0;
                    if (flagOrdem == 1)
                    {
                        etapas = etapas.Where(p => p.FUET_IN_ORDEM >= atual & p.FUET_CD_ID != indice).OrderBy(x => x.FUET_IN_ORDEM).ToList();
                        foreach (FUNIL_ETAPA eta in etapas)
                        {
                            nova = eta.FUET_IN_ORDEM + 1;
                            eta.FUET_IN_ORDEM = nova;
                            Int32 volta1 = GravaOrdem(eta);
                        }
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A etapa " + item.FUET_NM_NOME.ToUpper() + " do funil " + funil.FUNI_NM_NOME + " foi incluida com sucesso";
                    Session["MensFunil"] = 61;

                    // Verifica retorno
                    Session["TabFunil"] = 2;
                    return RedirectToAction("VoltarAnexoFunil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Funil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public Int32 GravaOrdem(FUNIL_ETAPA etapa)
        {   
            FUNIL_ETAPA nova = new FUNIL_ETAPA();
            nova.FUET_CD_ID = etapa.FUET_CD_ID;
            nova.FUET_DS_DESCRICAO = etapa.FUET_DS_DESCRICAO;
            nova.FUET_IN_ATIVO = etapa.FUET_IN_ATIVO;
            nova.FUET_IN_EMAIL = etapa.FUET_IN_EMAIL;
            nova.FUET_IN_ENCERRA = etapa.FUET_IN_ENCERRA;
            nova.FUET_IN_ORDEM = etapa.FUET_IN_ORDEM;
            nova.FUET_IN_PROPOSTA = etapa.FUET_IN_PROPOSTA;
            nova.FUET_IN_SMS = etapa.FUET_IN_SMS;
            nova.FUET_NM_NOME = etapa.FUET_NM_NOME;
            nova.FUET_SG_SIGLA = etapa.FUET_SG_SIGLA;
            nova.FUNI_CD_ID = etapa.FUNI_CD_ID;
            nova.FUET_IN_FATURAMENTO = etapa.FUET_IN_FATURAMENTO;
            nova.FUET_IN_EXPEDICAO = etapa.FUET_IN_EXPEDICAO;

            Int32 volta = baseApp.ValidateEditEtapa(nova);
            return volta;

        }

        public List<FUNIL> CarregaFunil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<FUNIL> conf = new List<FUNIL>();
                if (Session["Funis"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["FunilAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<FUNIL>)Session["Funis"];
                    }
                }
                Session["Funis"] = conf;
                Session["FunilAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Funil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Funil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }
    }
}