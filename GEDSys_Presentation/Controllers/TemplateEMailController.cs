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
    public class TemplateEMailController : Controller
    {
        private readonly ITemplateEMailAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly ITemplateEMailHTMLAppService htmApp;
        private readonly IAcessoMetodoAppService aceApp;


        private String msg;
        private Exception exception;
        TEMPLATE_EMAIL objeto = new TEMPLATE_EMAIL();
        TEMPLATE_EMAIL objetoAntes = new TEMPLATE_EMAIL();
        List<TEMPLATE_EMAIL> listaMaster = new List<TEMPLATE_EMAIL>();
        TEMPLATE_EMAIL_HTML objetoHtm = new TEMPLATE_EMAIL_HTML();
        TEMPLATE_EMAIL_HTML objetoHtmAntes = new TEMPLATE_EMAIL_HTML();
        List<TEMPLATE_EMAIL_HTML> listaMasterHtm = new List<TEMPLATE_EMAIL_HTML>();
        String extensao;

        public TemplateEMailController(ITemplateEMailAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, ITemplateEMailHTMLAppService htmApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            htmApp = htmApps;
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
        public ActionResult MontarTelaTemplateEMail()
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
                Session["ModuloAtual"] = "Template E-Mail";

                // Carrega listas
                if (Session["ListaTemplateEMail"] == null)
                {
                    listaMaster = CarregarModeloEMail();
                    Session["ListaTemplateEMail"] = listaMaster;
                }
                ViewBag.Listas = (List<TEMPLATE_EMAIL>)Session["ListaTemplateEMail"];
                Session["TemplateEMail"] = null;
                Session["IncluirTemplateEMail"] = 0;
                Session["LinhaAlterada"] = 0;
                Session["ListaLog"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensTemplateEMail"] != null)
                {
                    if ((Int32)Session["MensTemplateEMail"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0218", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0219", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0061", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 11)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0254", CultureInfo.CurrentCulture) + " - " + (String)Session["NomeImagem"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0427", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0428", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 32)
                    {

                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0429", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 33)
                    {

                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0430", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 61)
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_EMAIL", "TemplateEMail", "MontarTelaTemplateEMail");

                // Abre view
                Session["MensTemplateEMail"] = null;
                Session["VoltaTemplateEMail"] = 1;
                objeto = new TEMPLATE_EMAIL();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTemplateEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaTemplateEMail"] = null;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoTemplateEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.TEEM_IN_FIXO == 0).ToList();
                Session["ListaTemplateEMail"] = listaMaster;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTemplateEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTemplateEMail");
        }

        [HttpPost]
        public ActionResult FiltrarTemplateEMail(TEMPLATE_EMAIL item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<TEMPLATE_EMAIL> listaObj = new List<TEMPLATE_EMAIL>();
                Tuple<Int32, List<TEMPLATE_EMAIL>, Boolean> volta = baseApp.ExecuteFilter(item.TEEM_SG_SIGLA, item.TEEM_NM_NOME, item.TEEM_TX_CORPO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensTemplateEMail"] = 1;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }

                // Sucesso
                Session["MensTemplateEMail"] = 0;
                listaMaster = volta.Item2;
                Session["ListaTemplateEMail"] = volta.Item2;
                return RedirectToAction("MontarTelaTemplateEMail");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirTemplateEMail()
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
                        return RedirectToAction("MontarTelaTemplateEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Template E-Mail - Inclusão";

                // Prepara listas
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> imagem = new List<SelectListItem>();
                imagem.Add(new SelectListItem() { Text = "Imagens Externas", Value = "1" });
                imagem.Add(new SelectListItem() { Text = "Imagens Embutidas", Value = "2" });
                ViewBag.Imagem = new SelectList(imagem, "Value", "Text");
                List<SelectListItem> edit = new List<SelectListItem>();
                edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Editavel = new SelectList(edit, "Value", "Text");
                List<SelectListItem> aniv = new List<SelectListItem>();
                aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
                List<SelectListItem> fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21_1.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_EMAIL_INCLUIR", "TemplateEMail", "IncluirTemplateEMail");

                // Prepara view
                Session["MensTemplateEMail"] = null;
                TEMPLATE_EMAIL item = new TEMPLATE_EMAIL();
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.TEEM_IN_ATIVO = 1;
                vm.TEEM_IN_FIXO = 0;
                vm.TEEM_IN_ROBOT = 0;
                vm.TEEM_IN_HTML = 1;
                vm.TEEM_IN_SISTEMA = 6;
                vm.TEEM_IN_EDITAVEL = 0;
                vm.TEEM_IN_ANIVERSARIO = 0;
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateEMail(TemplateEMailViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> imagem = new List<SelectListItem>();
            imagem.Add(new SelectListItem() { Text = "Imagens Externas", Value = "1" });
            imagem.Add(new SelectListItem() { Text = "Imagens Embutidas", Value = "2" });
            ViewBag.Imagem = new SelectList(imagem, "Value", "Text");
            List<SelectListItem> edit = new List<SelectListItem>();
            edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Editavel = new SelectList(edit, "Value", "Text");
            List<SelectListItem> aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
            List<SelectListItem> fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.TEEM_TX_DADOS = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_DADOS);
                    vm.TEEM_TX_CORPO = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_CORPO);
                    vm.TEEM_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_NM_NOME);
                    vm.TEEM_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_SG_SIGLA);
                    vm.TEEM_TX_CABECALHO = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_CABECALHO);

                    // Prepara HTML
                    vm.TEEM_TX_CABECALHO = vm.TEEM_TX_CABECALHO.Replace("<p>", " ");
                    vm.TEEM_TX_CABECALHO = vm.TEEM_TX_CABECALHO.Replace("</p>", "<br />");
                    vm.TEEM_TX_CORPO = vm.TEEM_TX_CORPO.Replace("<p>", " ");
                    vm.TEEM_TX_CORPO = vm.TEEM_TX_CORPO.Replace("</p>", "<br />");
                    vm.TEEM_TX_DADOS = vm.TEEM_TX_DADOS.Replace("<p>", " ");
                    vm.TEEM_TX_DADOS = vm.TEEM_TX_DADOS.Replace("</p>", "<br />");

                    // Preparação
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTemplateEMail"] = 3;
                        return View(vm);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O modelo de E-Mail " + item.TEEM_NM_NOME.ToUpper() + " foi incluído com sucesso";
                    Session["MensTemplateEMail"] = 61;

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    Session["IdTemplateEMail"] = item.TEEM_CD_ID;
                    Session["ModeloEMailAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEEM_CD_ID;

                    return RedirectToAction("MontarTelaTemplateEMail");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTemplateEMail(Int32 id)
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
                Session["ModuloAtual"] = "Template E-Mail - Edição";

                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                Session["TemplateEMail"] = item;

                // Indicadores
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> edit = new List<SelectListItem>();
                edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Editavel = new SelectList(edit, "Value", "Text");
                List<SelectListItem> fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
                List<SelectListItem> aniv = new List<SelectListItem>();
                aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21_2.pdf";

                // Mensagens
                if (Session["MensTemplateEMail"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensTemplateEMail"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMail"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_EMAIL_EDITAR", "TemplateEMail", "EditarTemplateEMail");

                Session["MensTemplateEMail"] = null;
                Session["VoltaTemplateEMail"] = 1;
                Session["TemplateEMailAntes"] = item;
                Session["IdTemplateEMail"] = id;
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateEMail(TemplateEMailViewModel vm)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Texto HTML Digitado", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> edit = new List<SelectListItem>();
            edit.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            edit.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Editavel = new SelectList(edit, "Value", "Text");
            List<SelectListItem> fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
            List<SelectListItem> aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aniv = new SelectList(aniv, "Value", "Text");
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.TEEM_TX_DADOS = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_DADOS);
                    vm.TEEM_TX_CORPO = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_CORPO);
                    vm.TEEM_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_NM_NOME);
                    vm.TEEM_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_SG_SIGLA);
                    vm.TEEM_TX_CABECALHO = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.TEEM_TX_CABECALHO);

                    // Prepara HTML
                    vm.TEEM_TX_CABECALHO = vm.TEEM_TX_CABECALHO.Replace("<p>", " ");
                    vm.TEEM_TX_CABECALHO = vm.TEEM_TX_CABECALHO.Replace("</p>", "<br />");
                    vm.TEEM_TX_CORPO = vm.TEEM_TX_CORPO.Replace("<p>", " ");
                    vm.TEEM_TX_CORPO = vm.TEEM_TX_CORPO.Replace("</p>", "<br />");
                    vm.TEEM_TX_DADOS = vm.TEEM_TX_DADOS.Replace("<p>", " ");
                    vm.TEEM_TX_DADOS = vm.TEEM_TX_DADOS.Replace("</p>", "<br />");

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE_EMAIL item = Mapper.Map<TemplateEMailViewModel, TEMPLATE_EMAIL>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, (TEMPLATE_EMAIL)Session["TemplateEMailAntes"], usuario);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O modelo de E-Mail " + item.TEEM_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensTemplateEMail"] = 61;

                    // Sucesso
                    listaMaster = new List<TEMPLATE_EMAIL>();
                    Session["ListaTemplateEMail"] = null;
                    Session["ModeloEMailAlterada"] = 1;
                    Session["LinhaAlterada"] = item.TEEM_CD_ID;
                    return RedirectToAction("VoltarAnexoTemplateEMail");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoTemplateEMail()
        {

            return RedirectToAction("EditarTemplateEMail", new { id = (Int32)Session["IdTemplateEMail"] });
        }

        [HttpGet]
        public ActionResult ExcluirTemplateEMail(Int32 id)
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
                Session["ModuloAtual"] = "Template E-Mail - Exclusão";

                // Processa
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                item.TEEM_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTemplateEMail"] = 4;
                    return RedirectToAction("MontarTelaTemplateEMail");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo de E-Mail " + item.TEEM_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensTemplateEMail"] = 61;

                Session["ListaTemplateEMail"] = null;
                Session["ModeloEMailAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateEMail");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTemplateEMail(Int32 id)
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
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                objetoAntes = (TEMPLATE_EMAIL)Session["TemplateEMail"];
                item.TEEM_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo de E-Mail " + item.TEEM_NM_NOME.ToUpper() + " foi reativado com sucesso";
                Session["MensTemplateSMS"] = 61;

                Session["ListaTemplateEMail"] = null;
                Session["ModeloEMailAlterada"] = 1;
                return RedirectToAction("MontarTelaTemplateEMail");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerTemplateEMail(Int32 id)
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

                Session["IdTemplateEMail"] = id;
                TEMPLATE_EMAIL item = baseApp.GetItemById(id);
                TemplateEMailViewModel vm = Mapper.Map<TEMPLATE_EMAIL, TemplateEMailViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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

            Session["FileQueueEMail"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueEMail(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdTemplateEMail"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensTemplateEMail"] = 5;
                    return RedirectToAction("VoltarBaseTemplateEMail");
                }

                TEMPLATE_EMAIL item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 50)
                {
                    Session["MensTemplateEMail"] = 6;
                    return RedirectToAction("VoltarBaseTemplateEMail");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                extensao = extensao.ToUpper();

                // Monta pasta, se for HTML
                if (extensao == ".HTM" || extensao == ".HTML")
                {
                    String caminho = "Modelos/" + idAss.ToString() + "/";
                    String path = Path.Combine(Server.MapPath(caminho), fileName);
                    System.IO.File.WriteAllBytes(path, file.Contents);

                    // Gravar registro
                    item.TEEM_AQ_ARQUIVO = fileName;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usu);
                }
                else
                {
                    String caminho1 = "Images/" + idAss.ToString() + "/";
                    String path = Path.Combine(Server.MapPath(caminho1), fileName);
                    Tuple<Int32, String, Boolean> existe = CrossCutting.FileSystemLibrary.FileCheckExist(path);
                    if (existe.Item3)
                    {
                        Session["MensTemplateEMail"] = 11;
                        Session["NomeImagem"] = fileName;
                    }
                    else
                    {
                        System.IO.File.WriteAllBytes(path, file.Contents);
                    }
                }
                return RedirectToAction("VoltarBaseTemplateEMail");
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

        public List<TEMPLATE_EMAIL> CarregarModeloEMail()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
                if (Session["ModeloEMails"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ModeloEMailAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_EMAIL>)Session["ModeloEMails"];
                    }
                }
                conf = conf.Where(p => p.TEEM_IN_SISTEMA == 6).ToList();
                Session["ModeloEMailAlterada"] = 0;
                Session["ModeloEMails"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos E-Mails";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TEMPLATE_EMAIL_HTML> CarregarModeloEMailHTML()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_EMAIL_HTML> conf = new List<TEMPLATE_EMAIL_HTML>();
                conf = htmApp.GetAllItens(idAss);
                conf = conf.Where(p => p.TEHT_IN_SISTEMA == 6 || p.TEHT_IN_SISTEMA == 0).ToList();
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos E-Mails";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult IncluirTemplateEMailHTML()
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    // Verfifica permissão
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Template E-Mail - HTML - Inclusão";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_EMAIL_HTML_INCLUIR", "TemplateEMail", "IncluirTemplateEMailHTML");
                return View();
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirTemplateEMailHTML(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Inicializa
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usu = (USUARIO)Session["UserCredentials"];

            if (file == null)
            {
                Session["MensTemplateEMail"] = 30;
                return RedirectToAction("MontarTelaTemplateEMail");
            }
                      
            // Checa nome
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensTemplateEMail"] = 31;
                return RedirectToAction("MontarTelaTemplateEMail");
            }

            // Checa existencia
            TEMPLATE_EMAIL_HTML volta = htmApp.GetItemByNome(fileName);
            if (volta != null)
            {
                Session["MensTemplateEMail"] = 33;
                return RedirectToAction("MontarTelaTemplateEMail");
            }

            // Copia arquivo
            String caminho = "/TemplateEMail/Modelos/" + idAss.ToString() +"/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            // Grava registro
            TEMPLATE_EMAIL_HTML htm = new TEMPLATE_EMAIL_HTML();
            htm.ASSI_CD_ID = idAss;
            htm.TEHT_IN_SISTEMA = 6;
            htm.TEHT_NM_NOME = fileName;
            htm.TEHT_AQ_ARQUIVO =  fileName;
            htm.TEHT_DT_CADASTRO = DateTime.Today.Date;
            htm.USUA_CD_ID = usu.USUA_CD_ID;
            Int32 voltad = htmApp.ValidateCreate(htm, usu);

            // Mensagem do CRUD
            Session["MsgCRUD"] = "O modelo de HTML de E-Mail " + fileName.ToUpper() + " foi incluído com sucesso";
            Session["MensTemplateEMailHTML"] = 61;

            // Finaliza
            Session["MensTemplateEMail"] = null;
            Session["NomeHTML"] = fileName;
            return RedirectToAction("MontarTelaTemplateEMailHTML");
        }

        [HttpGet]
        public ActionResult MontarTelaTemplateEMailHTML()
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
                Session["ModuloAtual"] = "Template E-Mail - HTML";

                // Carrega listas
                listaMasterHtm = CarregarModeloEMailHTML();
                Session["ListaTemplateEMailHTML"] = listaMasterHtm;
                ViewBag.Listas = listaMasterHtm;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21_3.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensTemplateEMailHTML"] != null)
                {
                    if ((Int32)Session["MensTemplateEMailHTML"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplateEMailHTML"] == 61)
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TEMPLATE_EMAIL_HTML", "TemplateEMail", "MontarTelaTemplateEMailHTML");

                // Abre view
                Session["MensTemplateEMailHTML"] = null;
                Session["VoltaTemplateEMailHTML"] = 1;
                objetoHtm = new TEMPLATE_EMAIL_HTML();
                return View(objetoHtm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelo E-Mail";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirTemplateEMailHTML(Int32 id)
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
                Session["ModuloAtual"] = "Template E-Mail HTML - Exclusão";

                // Processa
                TEMPLATE_EMAIL_HTML item = htmApp.GetItemById(id);
                item.TEHT_IN_ATIVO = 0;
                Int32 volta = htmApp.ValidateDelete(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo de HTML de E-Mail " + item.TEHT_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensTemplateEMailHTML"] = 61;

                Session["ListaTemplateEMailHTML"] = null;
                return RedirectToAction("MontarTelaTemplateEMailHTML");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTemplateEMailHTML(Int32 id)
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
                TEMPLATE_EMAIL_HTML item = htmApp.GetItemById(id);
                item.TEHT_IN_ATIVO = 1;
                Int32 volta = htmApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O modelo de HTML de E-Mail " + item.TEHT_NM_NOME.ToUpper() + " foi reativado com sucesso";
                Session["MensTemplateEMailHTML"] = 61;

                Session["ListaTemplateEMailHTML"] = null;
                return RedirectToAction("MontarTelaTemplateEMailHTML");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}