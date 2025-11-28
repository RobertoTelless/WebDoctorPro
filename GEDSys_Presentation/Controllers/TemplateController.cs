using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;

namespace CRMPresentation.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;

#pragma warning disable CS0169 // O campo "TemplateController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "TemplateController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "TemplateController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "TemplateController.exception" nunca é usado
        TEMPLATE_EMAIL objeto = new TEMPLATE_EMAIL();
        TEMPLATE_EMAIL objetoAntes = new TEMPLATE_EMAIL();
        List<TEMPLATE_EMAIL> listaMaster = new List<TEMPLATE_EMAIL>();
#pragma warning disable CS0169 // O campo "TemplateController.extensao" nunca é usado
        String extensao;
#pragma warning restore CS0169 // O campo "TemplateController.extensao" nunca é usado

        public TemplateController(ITemplateAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
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

        [HttpGet]
        public ActionResult EditarTemplateCodigo(String codigo)
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
                        return RedirectToAction("MontarTelaAgenda" , "Agenda");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                TEMPLATE item = baseApp.GetByCode(codigo, idAss);
                Session["Template"] = item;

                // Mensagens
                if (Session["MensTemplate"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensTemplate"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensTemplate"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                }

                Session["MensTemplate"] = null;
                Session["VoltaTemplate"] = 1;
                Session["IdTemplate"] = item.TEMP_CD_ID;
                TemplateViewModel vm = Mapper.Map<TEMPLATE, TemplateViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Templates";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Templates", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarTemplateCodigo(TemplateViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    TEMPLATE item = Mapper.Map<TemplateViewModel, TEMPLATE>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item);

                    Session["TemplateAlterada"] = 1;
                    Session["MensAgenda"] = 70;
                    return RedirectToAction("MontarTelaAgenda", "Agenda");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Templates";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Templates", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }
    }
}