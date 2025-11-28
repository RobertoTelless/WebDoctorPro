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

namespace ERP_Condominios_Solution.Controllers
{
    public class TemplateSMSController : Controller
    {
        private readonly ITemplateSMSAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;

        private String msg;
        private Exception exception;
        TEMPLATE_SMS objeto = new TEMPLATE_SMS();
        TEMPLATE_SMS objetoAntes = new TEMPLATE_SMS();
        List<TEMPLATE_SMS> listaMaster = new List<TEMPLATE_SMS>();
        String extensao;

        public TemplateSMSController(ITemplateSMSAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
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
            return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaTemplateSMS()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_TEMPLATE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Modelos de Mensagens";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Template SMS";

                // Carrega listas
                if (Session["ListaTemplateSMS"] == null)
                {
                    listaMaster = CarregarModeloSMS();
                    Session["ListaTemplateSMS"] = listaMaster;
                }
                ViewBag.Listas = (List<TEMPLATE_SMS>)Session["ListaTemplateSMS"];
                Session["TemplateSMS"] = null;
                Session["IncluirTemplateSMS"] = 0;
                Session["ListaLog"] = null;
                Session["LinhaAlterada"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/22/Ajuda22.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensTemplateSMS"] != null)
                {
                    if ((Int32)Session["MensTemplateSMS"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0218", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0219", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 61)
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_SMS", "TemplateSMS", "MontarTelaTemplateSMS");


                // Abre view
                Session["MensTemplateSMS"] = null;
                Session["VoltaTemplateSMS"] = 1;
                objeto = new TEMPLATE_SMS();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTemplateSMS()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaTemplateSMS"] = null;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoTemplateSMS()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.TSMS_IN_FIXO == 0).ToList();
                Session["ListaTemplateSMS"] = listaMaster;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTemplateSMS()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateSMS");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateSMS(TEMPLATE_SMS item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<TEMPLATE_SMS> listaObj = new List<TEMPLATE_SMS>();
                Tuple<Int32, List<TEMPLATE_SMS>, Boolean> volta = baseApp.ExecuteFilter(item.TSMS_SG_SIGLA, item.TSMS_NM_NOME, item.TSMS_TX_CORPO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTemplateSMS"] = 1;
                    return RedirectToAction("MontarTelaTemplateSMS");
                }

                // Sucesso
                Session["MensTemplateSMS"] = 0;
                listaMaster = volta.Item2;
                Session["ListaTemplateSMS"] = volta.Item2;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirTemplateSMS()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_TEMPLATE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Inclusão";
                        return RedirectToAction("MontarTelaTemplateSMS");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Template SMS - Inclusão";

                // Prepara listas
                List<SelectListItem> edit = new List<SelectListItem>();
                edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Editavel = new SelectList(edit, "Value", "Text");
                List<SelectListItem> fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/22/Ajuda22_1.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_SMS_INCLUIR", "TemplateSMS", "IncluirTemplateSMS");

                // Prepara view
                Session["MensTemplateSMS"] = null;
                TEMPLATE_SMS item = new TEMPLATE_SMS();
                TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.TSMS_IN_ATIVO = 1;
                vm.TSMS_IN_FIXO = 0;
                vm.TSMS_IN_ROBOT = 0;
                vm.TSMS_NR_SISTEMA = 6;
                vm.TSMS_IN_EDITAVEL = 0;
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateSMS(TemplateSMSViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            List<SelectListItem> edit = new List<SelectListItem>();
            edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Editavel = new SelectList(edit, "Value", "Text");
            List<SelectListItem> fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.TSMS_TX_CORPO = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_TX_CORPO);
                    vm.TSMS_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_NM_NOME);
                    vm.TSMS_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_SG_SIGLA);

                    // Preparação
                    TEMPLATE_SMS item = Mapper.Map<TemplateSMSViewModel, TEMPLATE_SMS>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateSMS"] = 3;
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<TEMPLATE_SMS>();
                    Session["ListaTemplateSMS"] = null;
                    Session["IdTemplateSMS"] = item.TSMS_CD_ID;
                    Session["ModeloSMSAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TSMS_CD_ID;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O modelo de SMS " + item.TSMS_NM_NOME.ToUpper() + " foi incluído com sucesso";
                    Session["MensTemplateSMS"] = 61;

                    return RedirectToAction("MontarTelaTemplateSMS");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates SMS";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTemplateSMS(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDITAR_TEMPLATE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Edição";
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Template SMS - Edição";

                TEMPLATE_SMS item = baseApp.GetItemById(id);
                Session["TemplateSMS"] = item;

                // Indicadores
                List<SelectListItem> edit = new List<SelectListItem>();
                edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Editavel = new SelectList(edit, "Value", "Text");
                List<SelectListItem> fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/22/Ajuda22_2.pdf";

                // Mensagens
                if (Session["MensTemplateSMS"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensTemplateSMS"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateSMS"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_SMS_EDITAR", "TemplateSMS", "EditarTemplateSMS");

                Session["MensTemplateSMS"] = null;
                Session["VoltaTemplateSMS"] = 1;
                Session["TemplateSMSAntes"] = item;
                Session["IdTemplateSMS"] = id;
                TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateSMS(TemplateSMSViewModel vm)
        {
            List<SelectListItem> edit = new List<SelectListItem>();
            edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Editavel = new SelectList(edit, "Value", "Text");
            List<SelectListItem> fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.TSMS_TX_CORPO = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_TX_CORPO);
                    vm.TSMS_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_NM_NOME);
                    vm.TSMS_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeralNoHTML(vm.TSMS_SG_SIGLA);

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_SMS item = Mapper.Map<TemplateSMSViewModel, TEMPLATE_SMS>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O modelo de SMS " + item.TSMS_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensTemplateSMS"] = 61;

                    // Sucesso
                    listaMaster = new List<TEMPLATE_SMS>();
                    Session["ListaTemplateSMS"] = null;
                    Session["ModeloSMSAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TSMS_CD_ID;
                    return RedirectToAction("VoltarAnexoTemplateSMS");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates SMS";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoTemplateSMS()
        {

            return RedirectToAction("EditarTemplateSMS", new { id = (Int32)Session["IdTemplateSMS"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateSMS(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_TEMPLATE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Exclusão";
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Template SMS - Exclusão";

                // Processa
                TEMPLATE_SMS item = baseApp.GetItemById(id);
                item.TSMS_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTemplateSMS"] = 4;
                    return RedirectToAction("MontarTelaTemplateSMS");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo deSMS " + item.TSMS_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensTemplateSMS"] = 61;

                Session["ListaTemplateSMS"] = null;
                Session["ModeloSMSAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTemplateSMS(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVA_TEMPLATE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Modelos de Mensagens - Exclusão";
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                TEMPLATE_SMS item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_SMS)Session["TemplateSMS"];
                item.TSMS_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo deSMS " + item.TSMS_NM_NOME.ToUpper() + " foi reativado com sucesso";
                Session["MensTemplateSMS"] = 61;

                Session["ListaTemplateSMS"] = null;
                Session["ModeloSMSAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateSMS");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerTemplateSMS(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_SMS_VER", "TemplateSMS", "VerTemplateSMS");

                Session["IdTemplateSMS"] = id;
                TEMPLATE_SMS item = baseApp.GetItemById(id);
                TemplateSMSViewModel vm = Mapper.Map<TEMPLATE_SMS, TemplateSMSViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates SMS", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<EMPRESA> CarregaEmpresa()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<EMPRESA> conf = new List<EMPRESA>();
                if (Session["Empresas"] == null)
                {
                    conf = empApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["EmpresaAlterada"] == 1)
                    {
                        conf = empApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<EMPRESA>)Session["Empresas"];
                    }
                }
                Session["Empresas"] = conf;
                Session["EmpresaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates E-Mail", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_SMS> CarregarModeloSMS()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_SMS> conf = new List<TEMPLATE_SMS>();
                if (Session["ModeloSMSs"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ModeloSMSAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_SMS>)Session["ModeloSMSs"];
                    }
                }
                conf = conf.Where(p => p.TSMS_NR_SISTEMA == 6).ToList();
                Session["ModeloSMSAlterada"] = 0;
                Session["ModeloSMSs"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos SMS";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}