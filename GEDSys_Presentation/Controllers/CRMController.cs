using ApplicationServices.Interfaces;
using AutoMapper;
using Azure.Communication.Email;
using Canducci.Zip;
using CRMPresentation.App_Start;
using CrossCutting;
using EntitiesServices.Model;
using EntitiesServices.WorkClasses;
using ERP_Condominios_Solution.Classes;
using ERP_Condominios_Solution.Controllers;
using ERP_Condominios_Solution.ViewModels;
using GEDSys_Presentation.App_Start;
using iText.IO.Codec;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using XidNet;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Image = iTextSharp.text.Image;
using Humanizer;
using Newtonsoft.Json;
using EntitiesServices.Work_Classes;
using System.Windows.Input;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Net.Http;
using iText.IO.Font.Otf;

namespace GEDSys_Presentation.Controllers
{
    public class CRMController : Controller
    {
        private readonly ICRMAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IAssinanteAppService assApp;
        private readonly IPacienteAppService pacApp;
        private readonly INoticiaAppService notApp;
        private readonly IFunilAppService funApp;
        private readonly ICRMDiarioAppService diaApp;
        private readonly ITemplateEMailAppService teApp;

#pragma warning disable CS0169 // O campo "PacienteController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "PacienteController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "PacienteController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "PacienteController.exception" nunca é usado
        private PACIENTE objetoPac = new PACIENTE();
        private PACIENTE objetoPacAntes = new PACIENTE();
        private List<PACIENTE> listaMasterPac = new List<PACIENTE>();
        private List<USUARIO> listaMasterUsuario = new List<USUARIO>();
        private USUARIO objetoUsuario = new USUARIO();
        private String extensao;
        private CRM objeto = new CRM();
        private CRM objetoAntes = new CRM();
        private List<CRM> listaMaster = new List<CRM>();
        private LOG objetoLog = new LOG();
        private LOG objetoLogAntes = new LOG();
        private List<LOG> listaMasterLog = new List<LOG>();
        private NOTICIA objetoNot = new NOTICIA();
        private NOTICIA objetoNotAntes = new NOTICIA();
        private List<NOTICIA> listaMasterNot = new List<NOTICIA>();

        public CRMController(ICRMAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IPacienteAppService pacApps, INoticiaAppService notApps, IFunilAppService funApps, ICRMDiarioAppService diaApps, ITemplateEMailAppService teApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            aceApp = aceApps;
            assApp = assApps;
            pacApp = pacApps;
            notApp = notApps;
            funApp = funApps;
            diaApp = diaApps;
            teApp = teApps;
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
        public ActionResult MontarTelaCRM()
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
                if ((List<CRM>)Session["ListaCRM"] == null)
                {
                    listaMaster = CarregaCRM();
                    Session["ListaCRM"] = listaMaster;
                }
                Session["CRM"] = null;
                List<CRM> list = (List<CRM>)Session["ListaCRM"];
                list = list.OrderByDescending(p => p.CRM1_DT_CRIACAO).ToList();
                ViewBag.Listas = list;
                ViewBag.Title = "CRM";

                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
                List<SelectListItem> visao = new List<SelectListItem>();
                visao.Add(new SelectListItem() { Text = "Lista", Value = "1" });
                visao.Add(new SelectListItem() { Text = "Kanban", Value = "2" });
                ViewBag.Visao = new SelectList(visao, "Value", "Text");
                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Arquivado", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelado", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Falhado", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Sucesso", Value = "5" });
                ViewBag.Adic = new SelectList(adic, "Value", "Text");
                List<SelectListItem> fav = new List<SelectListItem>();
                fav.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                fav.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Favorito = new SelectList(fav, "Value", "Text");
                List<SelectListItem> temp = new List<SelectListItem>();
                temp.Add(new SelectListItem() { Text = "Fria", Value = "1" });
                temp.Add(new SelectListItem() { Text = "Morna", Value = "2" });
                temp.Add(new SelectListItem() { Text = "Quente", Value = "3" });
                temp.Add(new SelectListItem() { Text = "Muito Quente", Value = "4" });
                ViewBag.Temp = new SelectList(temp, "Value", "Text");
                Session["IncluirCRM"] = 0;
                Session["CRMVoltaAtendimento"] = 0;
                Session["VoltaAgenda"] = 11;
                Session["VoltaCRMBase"] = 0;
                Session["LinkAprova"] = null;
                Session["VoltaPedido"] = 2;
                Session["VoltaHistorico"] = 0;
                Session["VoltaTela"] = 0;
                Session["FlagMensagensEnviadas"] = 7;
                Session["VerDia"] = 1;
                Session["LinhaAlterada"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                //EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                //List<EMPRESA_FILIAL> fils = emp.EMPRESA_FILIAL.Where(p => p.EMFI_IN_ATIVO == 1).ToList();
                //ViewBag.Filiais = new SelectList(fils, "EMFI_CD_ID", "EMFI_NM_APELIDO");
                //ViewBag.Filial = usuario.USUA_IN_FILIAIS;

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0035", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0036", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0037", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 31)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0038", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0043", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0046", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0047", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 63)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0048", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0055", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0056", CultureInfo.CurrentCulture));
                    }

                    if ((Int32)Session["MensCRM"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 32)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0366", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 161)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM", "CRM", "MontarTelaCRM");

                // Monta View
                Session["IdCRM"] = null;
                Session["MensCRM"] = null;
                Session["VoltaCRM"] = 1;
                Session["IncluirCliente"] = 0;
                Session["VoltaPedido"] = 2;
                Session["FlagMensagensEnviadas"] = 6;
                Session["PontoAcao"] = 101;
                Session["VoltaPedidoView"] = 200;
                objeto = new CRM();
                if (Session["FiltroCRM"] != null)
                {
                    objeto = (CRM)Session["FiltroCRM"];
                }
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


































        public List<FUNIL> CarregaFunil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<FUNIL> conf = new List<FUNIL>();
                if (Session["Funis"] == null)
                {
                    conf = funApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["FunilAlterada"] == 1)
                    {
                        conf = funApp.GetAllItens(idAss);
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
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM> CarregaCRM()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM> conf = new List<CRM>();
                if (Session["CRMs"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CRM>)Session["CRMs"];
                    }
                }
                Session["CRMs"] = conf;
                Session["CRMAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_ORIGEM> CarregaOrigem()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_ORIGEM> conf = new List<CRM_ORIGEM>();
                if (Session["Origens"] == null)
                {
                    conf = baseApp.GetAllOrigens(idAss);
                }
                else
                {
                    if ((Int32)Session["OrigemAlterada"] == 1)
                    {
                        conf = baseApp.GetAllOrigens(idAss);
                    }
                    else
                    {
                        conf = (List<CRM_ORIGEM>)Session["Origens"];
                    }
                }
                Session["Origens"] = conf;
                Session["OrigemAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }



    }
}