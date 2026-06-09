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
        private readonly ILeadAppService leaApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;

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
        private List<DIARIO_PROCESSO> listaMasterDiario = new List<DIARIO_PROCESSO>();
        private DIARIO_PROCESSO objetoDiario = new DIARIO_PROCESSO();

        public CRMController(ICRMAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IPacienteAppService pacApps, INoticiaAppService notApps, IFunilAppService funApps, ICRMDiarioAppService diaApps, ITemplateEMailAppService teApps, ILeadAppService leaApps, IRecursividadeAppService recuApps, IMensagemEnviadaSistemaAppService meApps)
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
            leaApp = leaApps;
            recuApp = recuApps;
            meApp = meApps;
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        [HttpPost]
        public ActionResult FiltrarCRM(CRM item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Sanitização
                item.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.CRM1_NM_NOME);
                item.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.CRM1_DS_DESCRICAO);
                item.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.CRM1_NM_CAMPANHA);

                // Executa a operação
                List<CRM> listaObj = new List<CRM>();
                Session["FiltroCRM"] = item;
                Tuple<Int32, List<CRM>, Boolean> volta = baseApp.ExecuteFilter(item.CRM1_IN_STATUS, item.CRM1_DT_CRIACAO, item.CRM1_DT_CANCELAMENTO, item.ORIG_CD_ID, item.CRM1_IN_ATIVO, item.CRM1_NM_NOME, item.CRM1_DS_DESCRICAO, item.CRM1_IN_ESTRELA, item.CRM1_NR_TEMPERATURA, item.FUNI_CD_ID, item.CRM1_NM_CAMPANHA, item.EMFI_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensCRM"] = 1;
                    return RedirectToAction("MontarTelaCRM");
                }

                // Sucesso
                List<CRM> crms = volta.Item2;
                Session["MensCRM"] = 0;
                listaMaster = crms;
                Session["ListaCRM"] = crms;
                Session["CRMs"] = null;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("MontarTelaCRM");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        public ActionResult RetirarFiltroCRM()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaCRM"] = null;
                Session["FiltroCRM"] = null;
                Session["VoltaTela"] = 0;
                Session["CRMs"] = null;
                Session["CRMAlterada"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("MontarTelaCRM");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        [HttpGet]
        public ActionResult EstrelaSim(Int32 id)
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

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ESTRELA = 1;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Gera diario
                LEAD cli = leaApp.GetItemById(item.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Promoção a Favorito";
                dia.DIPR_DS_DESCRICAO = "Promoção do Processo " + item.CRM1_NM_NOME.ToUpper() + " para favorito";
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                Int32 volta1 = diaApp.ValidateCreate(dia);

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("MontarTelaCRM");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        [HttpGet]
        public ActionResult EstrelaNao(Int32 id)
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

                // Processa
                CRM item = baseApp.GetItemById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRM1_IN_ESTRELA = 0;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Gera diario
                LEAD cli = leaApp.GetItemById(item.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Retirada de Favorito";
                dia.DIPR_DS_DESCRICAO = "Retirada de favorito no Processo " + item.CRM1_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                Int32 volta1 = diaApp.ValidateCreate(dia);

                Session["ListaCRM"] = null;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("MontarTelaCRM");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        public ActionResult EditarLeadForm(Int32 id)
        {
            Session["VoltaClienteCRM"] = 2;
            Session["VoltaMsg"] = 0;
            Session["VoltaCliente"] = 0;
            Session["VoltaCRM"] = 0;
            Session["NivelLead"] = 1;
            return RedirectToAction("EditarLead", new { id = id });
        }

        [HttpGet]
        public ActionResult EditarLead(Int32 id)
        {
            Session["VoltaCRM"] = 11;
            Session["IdCliente"] = id;
            Session["VoltaTela"] = 0;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("VoltarAnexoLead", "Lead");
        }

        [HttpGet]
        public ActionResult IncluirProcessoCRM()
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
                List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
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
                List<SelectListItem> envio = new List<SelectListItem>();
                envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Envio = new SelectList(envio, "Value", "Text");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Mensagem
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 22)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM_INCLUIR", "CRM", "IncluirProcessoCRM");

                // Prepara view
                Session["VoltaTela"] = 0;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                Session["CRMNovo"] = 0;
                Session["VoltaCliente"] = 8;
                Session["VoltaCatCliente"] = 2;
                CRM item = new CRM();
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.CLIE_CD_ID = 2;
                vm.CRM1_DT_CRIACAO = DateTime.Today.Date;
                vm.CRM1_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.CRM1_IN_STATUS = 1;
                vm.CRM1_IN_ENCERRADO = 0;
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                vm.EMFI_CD_ID = usuario.EMFI_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        [HttpPost]
        public async Task<ActionResult> IncluirProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            ViewBag.Funis = new SelectList(CarregaFunil().Where(m => m.FUNIL_ETAPA.Count > 0).OrderBy(p => p.FUNI_NM_NOME), "FUNI_CD_ID", "FUNI_NM_NOME");
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
            List<SelectListItem> envio = new List<SelectListItem>();
            envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Envio = new SelectList(fav, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_NM_NOME);
                    vm.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_DS_DESCRICAO);
                    vm.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_NM_CAMPANHA);
                    vm.CRM1_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_TX_INFORMACOES_GERAIS);

                    // Verifica Funil
                    if (vm.FUNI_CD_ID == null || vm.FUNI_CD_ID == 0)
                    {
                        FUNIL funil = funApp.GetAllItens(idAss).Where(p => p.FUNI_IN_FIXO == 1).FirstOrDefault();
                        vm.FUNI_CD_ID = funil.FUNI_CD_ID;
                    }                  
                    
                    // Verifica cliente
                    if (vm.LEAD_CD_ID == null || vm.LEAD_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0141", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Recupera etapa
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    LEAD lead = leaApp.GetItemById(vm.LEAD_CD_ID.Value);
                    USUARIO usu = usuApp.GetItemById(vm.USUA_CD_ID.Value);
                    FUNIL fun = funApp.GetItemById(vm.FUNI_CD_ID.Value);
                    List<FUNIL_ETAPA> etapas = fun.FUNIL_ETAPA.OrderBy(p => p.FUET_IN_ORDEM).ToList();
                    FUNIL_ETAPA etapa = etapas.First();
                    vm.CRM1_IN_STATUS = etapa.FUET_CD_ID;

                    // Monta descrição
                    if (vm.CRM1_NM_NOME == null)
                    {
                        vm.CRM1_NM_NOME = "Processo referente ao lead de " + lead.LEAD_NM_NOME.ToUpper();
                    }
                    if (vm.CRM1_DS_DESCRICAO == null)
                    {
                        vm.CRM1_DS_DESCRICAO = "Processo criado em " + DateTime.Today.Date.ToShortDateString() + " para o lead de " + lead.LEAD_NM_NOME + " com identificador " + lead.LEAD_GU_IDENTIFICADOR + " e atribuido a " + usu.USUA_NM_NOME;
                    }

                    // Verifica se já tem processo neste lead

                    // Executa a operação
                    vm.CRM1_GU_GUID = lead.LEAD_GU_IDENTIFICADOR;
                    vm.CRM1_ID_IDENTIFICADOR = lead.LEAD_GU_IDENTIFICADOR;
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    item.CRM1_IN_SISTEMA = 6;
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0747", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Listas
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = item.CRM1_CD_ID;
                    Session["IdCRM"] = item.CRM1_CD_ID;
                    Session["LinhaAlterada"] = item.CRM1_CD_ID;

                    // Copia contatos
                    CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                    LEAD cli = leaApp.GetItemById(item.LEAD_CD_ID.Value);

                    // Emite mensagem
                    if (conf.CONF_IN_MENSAGEM_CRM == 1)
                    {
                        USUARIO usuResp = usuApp.GetItemById(item.USUA_CD_ID.Value);
                        Int32 voltaEM = await ProcessaEnvioEMailProcesso(proc, cli, usuResp, 1);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O Processo de " + cli.LEAD_NM_NOME.ToUpper() + " foi incluído com sucesso. Identificação: " + proc.CRM1_GU_GUID;
                    Session["MensCRM"] = 161;


                    // Retorno
                    if ((Int32)Session["VoltaCRM"] == 3)
                    {
                        Session["VoltaCRM"] = 0;
                        Session["CRMAtendimento"] = 0;
                        return RedirectToAction("IncluirProcessoCRM", "CRM");
                    }

                    Session["CRMAtendimento"] = 0;
                    Session["PontoProposta"] = 0;
                    Session["CRMAlterada"] = 1;
                    Session["VoltaTela"] = 0;
                    Session["FlagCRM"] = 1;
                    Session["FlagAlteraEstado"] = 1;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRM", "CRM");
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
                    return RedirectToAction("TrataExcecao", "Administra");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult MontarTelaHistoricoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaDiario"] = null;
            Session["TipoHistorico"] = 2;
            return RedirectToAction("MontarTelaHistorico");
        }

        public ActionResult VerHistoricoCRM(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaDiario"] = null;
            Session["TipoHistorico"] = 1;
            Session["IdCRM"] = id;
            return RedirectToAction("MontarTelaHistorico");
        }

        [HttpGet]
        public ActionResult MontarTelaHistorico()
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
                Int32 flag = (Int32)Session["TipoHistorico"];
                if ((List<DIARIO_PROCESSO>)Session["ListaDiario"] == null)
                {
                    if ((Int32)Session["TipoHistorico"] == 1)
                    {
                        CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                        Session["IdCRM"] = crm.CRM1_CD_ID;
                        listaMasterDiario = crm.DIARIO_PROCESSO.ToList();
                        Session["ListaDiario"] = listaMasterDiario.ToList();
                        Session["VoltaHistorico"] = 1;
                    }
                    else
                    {
                        listaMasterDiario = diaApp.GetAllItens(idAss).Where(p => p.CRM.CRM1_IN_ATIVO != 2).OrderByDescending(p => p.DIPR_DT_DATA).ToList();
                        Session["ListaDiario"] = listaMasterDiario;
                        Session["VoltaHistorico"] = 2;
                    }
                }
                else
                {
                    listaMasterDiario = ((List<DIARIO_PROCESSO>)Session["ListaDiario"]).ToList();
                }

                // Prepara lista
                Session["CRM"] = null;
                List<DIARIO_PROCESSO> list = (List<DIARIO_PROCESSO>)Session["ListaDiario"];
                list = list.OrderByDescending(p => p.DIPR_DT_DATA).ToList();
                ViewBag.Listas = list;
                List<USUARIO> listaTotal = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                List<CRM> listaTotal1 = CarregaCRM();
                ViewBag.CRM = new SelectList(listaTotal1.OrderBy(p => p.CRM1_NM_NOME), "CRM1_CD_ID", "CRM1_NM_NOME");

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["VoltaCRM"] = 22;
                Session["VoltaPedido"] = 22;
                Session["PontoAcao"] = 22;
                Session["VoltaAgenda"] = 44;
                Session["MensCRM"] = null;
                Session["AbaAgenda"] = 1;
                Session["NaoFezNada"] = 0;

                // Abre view
                objetoDiario = new DIARIO_PROCESSO();
                if (Session["FiltroDiario"] != null)
                {
                    objetoDiario = (DIARIO_PROCESSO)Session["FiltroDiario"];
                }
                return View(objetoDiario);
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        [HttpPost]
        public JsonResult BuscaNomeLead(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<LEAD> clientes = leaApp.GetAllItens(idAss);
            Session["Clientes"] = clientes;

            if (nome != null)
            {
                List<LEAD> lstCliente = clientes.Where(x => x.LEAD_NM_NOME != null && x.LEAD_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<LEAD>();

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.LEAD_CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.LEAD_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.LEAD_NM_NOME);
                        }
                        listResult.Add(result);
                    }
                }
            }

            return Json(listResult);
        }

        [HttpPost]
        public ActionResult FiltrarDiario(DIARIO_PROCESSO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Sanitização
                item.DIPR_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.DIPR_NM_OPERACAO);
                item.DIPR_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.DIPR_DS_DESCRICAO);

                // Prepara processo
                if ((Int32)Session["TipoHistorico"] == 1)
                {
                    item.CRM1_CD_ID = (Int32)Session["IdCRM"];
                }

                // Executa a operação
                List<DIARIO_PROCESSO> listaObj = new List<DIARIO_PROCESSO>();
                Session["FiltroDiario"] = item;
                Int32 volta = diaApp.ExecuteFilter(item.CRM1_CD_ID, item.DIPR_DT_DUMMY_1, item.DIPR_DT_DUMMY, item.USUA_CD_ID, item.DIPR_NM_OPERACAO, item.DIPR_DS_DESCRICAO, idAss, out listaObj);

                // Sucesso
                listaMasterDiario = listaObj;
                Session["ListaDiario"] = listaObj;
                return RedirectToAction("MontarTelaHistorico");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        public ActionResult RetirarFiltroDiario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaDiario"] = null;
                Session["FiltroDiario"] = null;
                Session["VerDia"] = 0;
                return RedirectToAction("MontarTelaHistorico");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }
        }

        public ActionResult GerarListagemHistorico()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "HistoricoLista" + "_" + data + ".pdf";
                List<DIARIO_PROCESSO> lista = new List<DIARIO_PROCESSO>();
                lista = (List<DIARIO_PROCESSO>)Session["ListaDiario"];
                lista = lista.OrderBy(p => p.DIPR_DT_DATA).ToList();

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

                    // Verificamos se o caminho do logo existe
                    if (!string.IsNullOrEmpty(empresa.EMPR_AQ_LOGO))
                    {
                        // 1. Removemos o "~" para obter o caminho interno (ex: Imagens/1/Logos/logo.png)
                        string blobPath = empresa.EMPR_AQ_LOGO.Replace("~", "");

                        // 2. Montamos a URL usando as configurações de Storage que você já tem
                        // Recomendo usar as variáveis do seu objeto 'conf' para ficar dinâmico
                        string storageUrl = "https://rtistoragemain.blob.core.windows.net/rti-datacontainer/";

                        // Garante que a URL termine com barra antes de concatenar
                        if (!storageUrl.EndsWith("/")) storageUrl += "/";

                        string fullUrl = storageUrl + blobPath;

                        // 3. iTextSharp busca a imagem diretamente da URL do Azure
                        image = Image.GetInstance(fullUrl);
                    }
                    else
                    {
                        // Caso não tenha logo, você pode carregar um placeholder local ou ignorar
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/logo_padrao.png"));
                    }

                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Histórico de Processos", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Histórico de Processos", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 90f, 120f, 120f, 300f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Processo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Lead", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (DIARIO_PROCESSO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.DIPR_DT_DATA.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRM.CRM1_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CRM.LEAD.LEAD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.DIPR_DS_DESCRICAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
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

                return RedirectToAction("MontarTelaHistoricoGeral");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "CR<";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "Administra");
            }   
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailProcesso(CRM pro, LEAD lea, USUARIO usuario, Int32? tipo)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();
            ASSINANTE assinante = (ASSINANTE)Session["AssinanteLogado"];

            LEAD lead = leaApp.GetItemById(lea.LEAD_CD_ID);
            CRM crm = baseApp.GetItemById(pro.CRM1_CD_ID);

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Recupera Template
            TEMPLATE_EMAIL template = teApp.GetByCode("CRIAPROC", idAss);

            // Prepara cabeçalho
            String cab = template.TEEM_TX_CABECALHO;

            // Prepara assinatura
            String assinatura = String.Empty;
            assinatura += "Enviado por <b>WebDoctorPro - Administração" + "</b><br />";

            // Prepara corpo da mensagem
            String texto = template.TEEM_TX_CORPO;
            if (texto.Contains("{resp}"))
            {
                texto = texto.Replace("{resp}", usuario.USUA_NM_NOME.ToUpper());
            }
            if (texto.Contains("{lead}"))
            {
                texto = texto.Replace("{lead}", lead.LEAD_NM_NOME.ToUpper());
            }
            if (texto.Contains("{proc}"))
            {
                texto = texto.Replace("{proc}", crm.CRM1_NM_NOME.ToUpper());
            }
            if (texto.Contains("{guid}"))
            {
                texto = texto.Replace("{guid}", lead.LEAD_GU_IDENTIFICADOR.ToUpper());
            }
            if (texto.Contains("{data}"))
            {
                texto = texto.Replace("{data}", crm.CRM1_DT_CRIACAO.Value.ToLongDateString());
            }
            String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            List<AttachmentModel> models = new List<AttachmentModel>();
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Criação de Processo - " + crm.CRM1_NM_NOME.ToUpper();
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "WebDoctorPro";
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
                Session["VoltaExcecao"] = "CRM";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = usuario.USUA_NM_NOME;
                mens.ID = null;
                mens.MODELO = usuario.USUA_NM_NOME;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = usuario.USUA_NM_NOME;
                mens.MENS_NM_NOME = "Criação de Processo CRM - Envio de aviso ao responsável: " + usuario.USUA_NM_NOME;
                mens.MENS_GU_GUID = lead.LEAD_GU_IDENTIFICADOR;
                mens.MENS_DT_AGENDAMENTO = crm.CRM1_DT_CRIACAO;
                mens.MENS_DT_ENVIO = DateTime.Today.Date;
                mens.MENS_NM_CABECALHO = usuario.USUA_NR_CPF;
                mens.MENS_NR_REPETICOES = 0;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                mens.MENS_NM_RODAPE = String.Empty;
                mens.CELULAR = usuario.USUA_NR_CELULAR;
                mens.TELEFONE = usuario.USUA_NR_TELEFONE;
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                mens.MENS_TX_TEXTO = texto;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Processo CRM - Criação de Processo");
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensCRM"] = 991;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        public ActionResult GerarRelatorioListaCRM()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ProcessosLista" + "_" + data + ".pdf";
                List<CRM> lista = new List<CRM>();
                lista = (List<CRM>)Session["ListaCRM"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontP = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

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

                    // Verificamos se o caminho do logo existe
                    if (!string.IsNullOrEmpty(empresa.EMPR_AQ_LOGO))
                    {
                        // 1. Removemos o "~" para obter o caminho interno (ex: Imagens/1/Logos/logo.png)
                        string blobPath = empresa.EMPR_AQ_LOGO.Replace("~", "");

                        // 2. Montamos a URL usando as configurações de Storage que você já tem
                        // Recomendo usar as variáveis do seu objeto 'conf' para ficar dinâmico
                        string storageUrl = "https://rtistoragemain.blob.core.windows.net/rti-datacontainer/";

                        // Garante que a URL termine com barra antes de concatenar
                        if (!storageUrl.EndsWith("/")) storageUrl += "/";

                        string fullUrl = storageUrl + blobPath;

                        // 3. iTextSharp busca a imagem diretamente da URL do Azure
                        image = Image.GetInstance(fullUrl);
                    }
                    else
                    {
                        // Caso não tenha logo, você pode carregar um placeholder local ou ignorar
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/logo_padrao.png"));
                    }

                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Relação de Processos", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Relação de Processos", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 150f, 150f, 80f, 80f, 120f, 80f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Lead", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Processo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Funil", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Etapa", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Próxima Ação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data Prevista", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Origem", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Situação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CRM item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.LEAD.LEAD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.CRM1_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
                    cell = new PdfPCell(new Paragraph(funil.FUNI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == item.CRM1_IN_STATUS).FirstOrDefault();
                    cell = new PdfPCell(new Paragraph(etapa.FUET_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.CRM_ACAO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_NM_TITULO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFontP))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);
                    if (item.CRM_ACAO.Count > 0)
                    {
                        if (item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.Date >= DateTime.Today.Date)
                        {
                            cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph(item.CRM_ACAO.Where(p => p.CRAC_IN_ATIVO == 1).OrderByDescending(m => m.CRAC_DT_PREVISTA).FirstOrDefault().CRAC_DT_PREVISTA.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                        }
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFontP))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.CRM_ORIGEM.CROR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.CRM1_IN_ATIVO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativo", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Arquivado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Cancelado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Falhado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Sucesso", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    table.AddCell(cell);
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

                return RedirectToAction("MontarTelaCRM");
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
                return RedirectToAction("TrataExcecao", "Administra");
            }   
        }

        [HttpGet]
        public ActionResult CancelarProcessoCRM(Int32 id)
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
                ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
                Session["IncluirCRM"] = 0;
                Session["CRM"] = null;

                // Recupera
                Session["CRMNovo"] = 0;
                CRM item = baseApp.GetItemById(id);
                Session["IdCRM"] = item.CRM1_CD_ID;

                // Checa ações
                Session["TemAcao"] = 0;
                if (item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList().Count > 0)
                {
                    Session["TemAcao"] = 1;
                }

                // Prepara view
                Session["FlagAlteraEstado"] = 1;
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                vm.CRM1_DT_CANCELAMENTO = DateTime.Today.Date;
                vm.CRM1_IN_ATIVO = 3;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelarProcessoCRM(CRMViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Motivos = new SelectList(CarregaMotivoCancelamento().Where(p => p.MOCA_IN_TIPO == 1).OrderBy(p => p.MOCA_NM_NOME), "MOCA_CD_ID", "MOCA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRM1_DS_MOTIVO_CANCELAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_DS_MOTIVO_CANCELAMENTO);

                    // Executa a operação
                    CRM crm = Mapper.Map<CRMViewModel, CRM>(vm);
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(crm, crm, usuario);

                    // Verifica retorno
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 30;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 31;
                        return RedirectToAction("MontarTelaCRM");
                    }
                    if (volta == 5)
                    {
                        Session["MensCRM"] = 32;
                        return RedirectToAction("MontarTelaCRM");
                    }

                    // Emite mensagem
                    if (conf.CONF_IN_MENSAGEM_CRM == 1)
                    {
                        USUARIO usuResp = usuApp.GetItemById(crm.USUA_CD_ID.Value);
                        LEAD lead = leaApp.GetItemById(crm.LEAD_CD_ID.Value);
                        MOTIVO_CANCELAMENTO can = mcApp.GetItemById(crm.MOCA_CD_ID.Value);
                        Int32 voltaEM = await ProcessaEnvioEMailProcesso(crm, lead, usuResp, 2);




                        // Monta Texto
                        String info = String.Empty;
                        info = info + "Prezado Sr(a) " + usuResp.USUA_NM_NOME + "<br /><br />";
                        info = info + "<br />A processo abaixo foi cancelado pelo responsável em " + DateTime.Today.Date.ToShortDateString() + "<br />";
                        info = info + "Motivo do Cancelamento: <b>" + can.MOCA_NM_NOME + "</b><br />";
                        info = info + "Justificativa do Cancelamento: <b>" + crm.CRM1_DS_MOTIVO_CANCELAMENTO + "</b><br />";
                        info = info + "<br />Informações do Processo:<br />";
                        info = info + "Processo: <b style='color: darkblue'>" + crm.CRM1_NM_NOME + "</b><br />";
                        info = info + "Cliente: <b style='color: grenn'>" + cli.CLIE_NM_NOME + "</b><br />";
                        info = info + "Data de Início: <b>" + crm.CRM1_DT_CRIACAO.Value.ToShortDateString() + "</b><br />";
                        info = info + "Identificador: <b>" + crm.CRM1_GU_GUID + "</b><br />";

                    }


                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = crm.CRM1_CD_ID;
                    Session["IdCRM"] = crm.CRM1_CD_ID;
                    Session["CRMAlterada"] = 1;
                    Session["FlagCRM"] = 1;
                    return RedirectToAction("MontarTelaCRM");
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
            else
            {
                return View(vm);
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

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = usuApp.GetAllItens();
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllItens();
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
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
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return null;
            }
        }

        public ActionResult VoltarAcompanhamentoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("MontarTelaCRM");
            //return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public ActionResult VoltarAcompanhamentoCRMBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        public List<MOTIVO_CANCELAMENTO> CarregaMotivoCancelamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MOTIVO_CANCELAMENTO> conf = new List<MOTIVO_CANCELAMENTO>();
                if (Session["MotCancelamentos"] == null)
                {
                    conf = baseApp.GetAllMotivoCancelamento(idAss);
                }
                else
                {
                    if ((Int32)Session["MotCancelamentoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllMotivoCancelamento(idAss);
                    }
                    else
                    {
                        conf = (List<MOTIVO_CANCELAMENTO>)Session["MotCancelamentos"];
                    }
                }
                Session["MotCancelamentos"] = conf;
                Session["MotCancelamentoAlterada"] = 0;
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