using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;
using ERP_Condominios_Solution.Classes;
using Newtonsoft.Json;
using System.Net;
using EntitiesServices.Work_Classes;

namespace CRMPresentation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["Close"] = false;
            Session["MensagemLogin"] = null;
            Session["Versao1"] = "../BaseAdmin/Versao/Versao_1_2_0_0.pdf";
            Session["Versao2"] = "../BaseAdmin/Versao/Versao_1_2_1_0.pdf";
            Session["Versao3"] = "../BaseAdmin/Versao/Versao_1_2_2_0.pdf";
            Session["Versao4"] = "../BaseAdmin/Versao/Versao_2_0_0_0.pdf";
            Session["eDemo"] = 0;
            Session["DemoVencido"] = 0;
            Session["PagVencido"] = 0;
            Session["AssinantePendente"] = 0;

            // Recupera informações
            AcessoVisitanteDTO dto = CookieManager.CapturarDadosAcesso();

            // Trata cookie
            //Boolean cook = CookieManager.VerificarValidadeCookie();
            //if (cook)
            //{
            //    return RedirectToAction("Login", "ControleAcesso");
            //}
            return RedirectToAction("CarregarLandingPage", "BaseAdmin");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}