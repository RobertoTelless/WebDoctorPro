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
using ModelServices.Interfaces.Repositories;
using iText.IO.Codec;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace GEDSys_Presentation.Controllers
{
    public class AvisoController : Controller
    {
        private readonly IAvisoLembreteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IPacienteAppService pacApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;

        private String msg;
        private Exception exception;
        AVISO_LEMBRETE objeto = new AVISO_LEMBRETE();
        AVISO_LEMBRETE objetoAntes = new AVISO_LEMBRETE();
        List<AVISO_LEMBRETE> listaMaster = new List<AVISO_LEMBRETE>();
        String extensao;

        public AvisoController(IAvisoLembreteAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IPacienteAppService pacApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            pacApp = pacApps;
            empApp = empApps;
            aceApp = aceApps;
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

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        [HttpGet]
        public ActionResult MontarTelaAviso()
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
                Session["ModuloAtual"] = "Avisos";

                // Carrega listas
                if ((List<AVISO_LEMBRETE>)Session["ListaAviso"] == null)
                {
                    listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    listaMaster = listaMaster.Where(p => p.AVIS_DT_AVISO.Value.Date == DateTime.Today.Date).ToList();
                    Session["ListaAviso"] = listaMaster;
                }
                ViewBag.Listas = (List<AVISO_LEMBRETE>)Session["ListaAviso"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["Aviso"] = null;
                Session["ListaLog"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/14/Ajuda14.pdf";

                // Indicadores
                var ciente = new List<SelectListItem>();
                ciente.Add(new SelectListItem() { Text = "Lido", Value = "1" });
                ciente.Add(new SelectListItem() { Text = "Pendente", Value = "0" });
                ViewBag.Ciente = new SelectList(ciente, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAviso"] != null)
                {
                    if ((Int32)Session["MensAviso"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAviso"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO", "Aviso", "MontarTelaAviso");

                // Abre view
                Session["MensAviso"] = null;
                Session["VoltaAviso"] = 1;
                objeto = new AVISO_LEMBRETE();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroAviso()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaAviso"] = null;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAvisoHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AVISO_LEMBRETE> listaHoje = new List<AVISO_LEMBRETE>();
                listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMaster.Where(p => p.AVIS_DT_AVISO.Value.Date == DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensAviso"] = 1;
                }
                else
                {
                    listaMaster = listaHoje;
                }
                Session["ListaAviso"] = listaHoje;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAvisoFuturo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AVISO_LEMBRETE> listaHoje = new List<AVISO_LEMBRETE>();
                listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMaster.Where(p => p.AVIS_DT_AVISO.Value.Date > DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensAviso"] = 1;
                    listaMaster = new List<AVISO_LEMBRETE>();
                }
                else
                {
                    listaMaster = listaHoje;
                }
                Session["ListaAviso"] = listaMaster;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAvisoAberto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AVISO_LEMBRETE> listaHoje = new List<AVISO_LEMBRETE>();
                listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMaster.Where(p => p.AVIS_IN_CIENTE == 0).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensAviso"] = 1;
                    listaMaster = new List<AVISO_LEMBRETE>();
                }
                else
                {
                    listaMaster = listaHoje;
                }
                Session["ListaAviso"] = listaMaster;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAvisoAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AVISO_LEMBRETE> listaHoje = new List<AVISO_LEMBRETE>();
                listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMaster.Where(p => p.AVIS_DT_AVISO.Value.Date < DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensAviso"] = 1;
                    listaMaster = new List<AVISO_LEMBRETE>();
                }
                else
                {
                    listaMaster = listaHoje;
                }
                Session["ListaAviso"] = listaMaster;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAvisoMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<AVISO_LEMBRETE> listaHoje = new List<AVISO_LEMBRETE>();
                listaMaster = CarregarAviso().Where(p => p.AVIS_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMaster.Where(p => p.AVIS_DT_AVISO.Value.Date.Month == DateTime.Today.Date.Month & p.AVIS_DT_AVISO.Value.Date.Year == DateTime.Today.Date.Year ).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensAviso"] = 1;
                    listaMaster = new List<AVISO_LEMBRETE>();
                }
                else
                {
                    listaMaster = listaHoje;
                }
                Session["ListaAviso"] = listaMaster;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseAviso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaAviso");
        }

        [HttpPost]
        public ActionResult FiltrarAviso(AVISO_LEMBRETE item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<AVISO_LEMBRETE> listaObj = new List<AVISO_LEMBRETE>();
                Tuple<Int32, List<AVISO_LEMBRETE>, Boolean> volta = baseApp.ExecuteFilter(item.AVIS_NM_TITULO, item.AVIS_DT_CRIACAO, item.AVIS_DT_AVISO, item.AVIS_IN_CIENTE, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensAviso"] = 1;
                    return RedirectToAction("MontarTelaAviso");
                }

                // Sucesso
                Session["MensAviso"] = 0;
                listaMaster = volta.Item2;
                Session["ListaAviso"] = volta.Item2;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirAviso()
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
                Session["ModuloAtual"] = "Avisos - Criação";

                // Prepara listas
                ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/14/Ajuda14_1.pdf";

                // Mensagem
                if (Session["MensAviso"] != null)
                {
                    if ((Int32)Session["MensAviso"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0547", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_INCLUIR_IN", "Aviso", "IncluirAviso");

                // Prepara view
                Session["MensAviso"] = null;
                AVISO_LEMBRETE item = new AVISO_LEMBRETE();
                AvisoLembreteViewModel vm = Mapper.Map<AVISO_LEMBRETE, AvisoLembreteViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.AVIS_IN_SISTEMA = 6;
                vm.AVIS_IN_CIENTE = 0;
                vm.AVIS_DT_CRIACAO = DateTime.Now;
                vm.AVIS_IN_ATIVO = 1;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirAviso(AvisoLembreteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AVIS_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AVIS_NM_TITULO);
                    vm.AVIS_DS_AVISO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AVIS_DS_AVISO);

                    // Critica
                    if (vm.AVIS_DT_AVISO.Value.Date < DateTime.Today.Date)
                    {
                        Session["MensAviso"] = 2;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0547", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Preparação
                    AVISO_LEMBRETE item = Mapper.Map<AvisoLembreteViewModel, AVISO_LEMBRETE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Acerta pendencias
                    List<AVISO_LEMBRETE> avisos = CarregarAviso().Where(p => p.AVIS_IN_CIENTE == 0 & p.AVIS_DT_AVISO.Value.Date <= DateTime.Today.Date).ToList();
                    Session["AvisosAbertos"] = avisos.Count;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O aviso " + item.AVIS_NM_TITULO.ToUpper() + " foi incluído com sucesso";
                    Session["MensAviso"] = 61;

                    // Sucesso
                    listaMaster = new List<AVISO_LEMBRETE>();
                    Session["ListaAviso"] = null;
                    Session["IdAviso"] = item.AVIS_CD_ID;
                    Session["AvisoAlterada"] = 1;

                    return RedirectToAction("MontarTelaAviso");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Aviso";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAviso(Int32 id)
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
                Session["ModuloAtual"] = "Avisos - Edição";

                AVISO_LEMBRETE item = baseApp.GetItemById(id);
                Session["Aviso"] = item;

                // Indicadores
                var ciente = new List<SelectListItem>();
                ciente.Add(new SelectListItem() { Text = "Visto", Value = "1" });
                ciente.Add(new SelectListItem() { Text = "Pendente", Value = "0" });
                ViewBag.Ciente = new SelectList(ciente, "Value", "Text");
                ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/14/Ajuda14_2.pdf";

                // Mensagem
                if (Session["MensAviso"] != null)
                {
                    if ((Int32)Session["MensAviso"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0547", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAviso"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_EDITAR", "Aviso", "EditarAviso");

                Session["MensAviso"] = null;
                objetoAntes = item;
                Session["IdAviso"] = id;
                AvisoLembreteViewModel vm = Mapper.Map<AVISO_LEMBRETE, AvisoLembreteViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarAviso(AvisoLembreteViewModel vm)
        {
            var ciente = new List<SelectListItem>();
            ciente.Add(new SelectListItem() { Text = "Visto", Value = "1" });
            ciente.Add(new SelectListItem() { Text = "Pendente", Value = "0" });
            ViewBag.Ciente = new SelectList(ciente, "Value", "Text");
            ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AVIS_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AVIS_NM_TITULO);
                    vm.AVIS_DS_AVISO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AVIS_DS_AVISO);

                    // Critica

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    AVISO_LEMBRETE item = Mapper.Map<AvisoLembreteViewModel, AVISO_LEMBRETE>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Acerta pendencias
                    List<AVISO_LEMBRETE> avisos = CarregarAviso().Where(p => p.AVIS_IN_CIENTE == 0 & p.AVIS_DT_AVISO.Value.Date <= DateTime.Today.Date).ToList();
                    Session["AvisosAbertos"] = avisos.Count;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O aviso " + item.AVIS_NM_TITULO.ToUpper() + " foi alterado com sucesso";
                    Session["MensAviso"] = 61;

                    // Sucesso
                    listaMaster = new List<AVISO_LEMBRETE>();
                    Session["ListaAviso"] = null;
                    Session["AvisoAlterada"] = 1;
                    Session["ListaAvisoAtivo"] = null;

                    if ((Int32)Session["VoltaAviso"] == 4)
                    {
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                    return RedirectToAction("MontarTelaAviso");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Aviso";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoAviso()
        {
            return RedirectToAction("EditarAviso", new { id = (Int32)Session["IdAviso"] });
        }

        [HttpGet]
        public ActionResult ExcluirAviso(Int32 id)
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

                // Processa
                AVISO_LEMBRETE item = baseApp.GetItemById(id);
                objetoAntes = (AVISO_LEMBRETE)Session["Aviso"];
                item.AVIS_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);

                // Acerta pendencias
                List<AVISO_LEMBRETE> avisos = CarregarAviso().Where(p => p.AVIS_IN_CIENTE == 0 & p.AVIS_DT_AVISO.Value.Date <= DateTime.Today.Date).ToList();
                Session["AvisosAbertos"] = avisos.Count;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O aviso " + item.AVIS_NM_TITULO.ToUpper() + " foi excluído com sucesso";
                Session["MensAviso"] = 61;

                Session["ListaAviso"] = null;
                Session["AvisoAlterada"] = 1;
                return RedirectToAction("MontarTelaAviso");

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarAviso(Int32 id)
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

                // Processa
                AVISO_LEMBRETE item = baseApp.GetItemById(id);
                objetoAntes = (AVISO_LEMBRETE)Session["Aviso"];
                item.AVIS_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O aviso " + item.AVIS_NM_TITULO.ToUpper() + " foi reativado com sucesso";
                Session["MensAviso"] = 61;

                Session["ListaAviso"] = null;
                Session["AvisoAlterada"] = 1;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAviso(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/14/Ajuda14_2.pdf";
                Session["IdAviso"] = id;
                AVISO_LEMBRETE item = baseApp.GetItemById(id);
                AvisoLembreteViewModel vm = Mapper.Map<AVISO_LEMBRETE, AvisoLembreteViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_VER", "Aviso", "VerAviso");
                
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult VerAviso(AvisoLembreteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Preparação
                    //vm.AVIS_IN_CIENTE = 1;
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    AVISO_LEMBRETE item = Mapper.Map<AvisoLembreteViewModel, AVISO_LEMBRETE>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Acerta pendencias
                    List<AVISO_LEMBRETE> avisos = CarregarAviso().Where(p => p.AVIS_IN_CIENTE == 0 & p.AVIS_DT_AVISO.Value.Date <= DateTime.Today.Date).ToList();
                    Session["AvisosAbertos"] = avisos.Count;

                    // Sucesso
                    listaMaster = new List<AVISO_LEMBRETE>();
                    Session["ListaAviso"] = null;
                    Session["AvisoAlterada"] = 1;
                    return RedirectToAction("MontarTelaAviso", "Aviso");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Aviso";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<AVISO_LEMBRETE> CarregarAviso()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<AVISO_LEMBRETE> conf = new List<AVISO_LEMBRETE>();
                if (Session["Avisos"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["AvisoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<AVISO_LEMBRETE>)Session["Avisos"];
                    }
                }
                conf = conf.Where(p => p.AVIS_IN_SISTEMA == 6).ToList();
                Session["AvisoAlterada"] = 0;
                Session["Avisos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Avisos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Avisos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult GerarListagemAviso()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "AvisoLista" + "_" + data + ".pdf";
                List<AVISO_LEMBRETE> lista = (List<AVISO_LEMBRETE>)Session["ListaAviso"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cabeçalho
                PdfPTable headerTable = new PdfPTable(new float[] { 20f, 700f });
                headerTable.WidthPercentage = 100;
                headerTable.HorizontalAlignment = 1;
                headerTable.SpacingBefore = 1f;
                headerTable.SpacingAfter = 1f;

                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.Border = 0;
                    cell1.Colspan = 1;
                    Image image = null;
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Avisos e Lembretes", meuFont2))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    cell1.Border = 0;
                    cell1.Colspan = 1;
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);
                }
                else
                {
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Avisos e Lembretes", meuFont2))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    cell2.Border = 0;
                    cell2.Colspan = 2;
                    headerTable.AddCell(cell2);

                    cell2 = new PdfPCell(new Paragraph(" ", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell2.Colspan = 2;
                    cell2.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell2);

                }

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = PdfPCell.TOP_BORDER;
                cell = new PdfPCell(new Paragraph("Gerado por WebDoctor 1.0 em " + DateTime.Today.Date.ToLongDateString(), meuFont));
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 60, 40);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 100f, 70f, 70f, 60f, 200f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Título", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data de Criação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data de Aviso", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Situação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (AVISO_LEMBRETE item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.AVIS_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AVIS_DT_CRIACAO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.AVIS_DT_AVISO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.AVIS_IN_CIENTE == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Visto", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACIENTE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
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

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ConfirmarVisualizacao(Int32 id)
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
                AVISO_LEMBRETE item = baseApp.GetItemById(id);
                item.AVIS_IN_CIENTE = 1;
                Int32 volta = baseApp.ValidateEdit(item, item, usuario);

                Session["AvisoAlterada"] = 1;
                Session["ListaAviso"] = null;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Aviso dto = MontarAvisoDTO(item.AVIS_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Aviso - Confirma Visualização",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O aviso " + item.AVIS_NM_TITULO.ToUpper() + " foi visualizado com sucesso";
                Session["MensAviso"] = 61;

                if ((Int32)Session["VoltaAviso"] == 4)
                {
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
                return RedirectToAction("MontarTelaAviso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Aviso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Aviso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public DTO_Aviso MontarAvisoDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.AVISO_LEMBRETE
                    .Where(l => l.AVIS_CD_ID == mediId)
                    .Select(l => new DTO_Aviso
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        AVIS_CD_ID = l.AVIS_CD_ID,
                        AVIS_DT_AVISO = l.AVIS_DT_AVISO,
                        AVIS_DT_CRIACAO = l.AVIS_DT_CRIACAO,
                        AVIS_IN_ATIVO = l.AVIS_IN_ATIVO,
                        AVIS_IN_CIENTE = l.AVIS_IN_CIENTE,
                        AVIS_DS_AVISO = l.AVIS_DS_AVISO,
                        AVIS_IN_SISTEMA = l.AVIS_IN_SISTEMA,
                        AVIS_NM_TITULO = l.AVIS_NM_TITULO,
                        PACI_CD_ID = l.PACI_CD_ID,
                        PROD_CD_ID = l.PROD_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

    }
}