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
        private readonly IAgendaAppService ageApp;

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

        public CRMController(ICRMAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IPacienteAppService pacApps, INoticiaAppService notApps, IFunilAppService funApps, ICRMDiarioAppService diaApps, ITemplateEMailAppService teApps, ILeadAppService leaApps, IRecursividadeAppService recuApps, IMensagemEnviadaSistemaAppService meApps, IAgendaAppService ageApps)
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
            ageApp = ageApps;
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

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Relação de Processos*", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Processos/Data", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Processos/Mês", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Processos/Status", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Ativos", Value = "5" });
                relat.Add(new SelectListItem() { Text = "Cancelados", Value = "6" });
                relat.Add(new SelectListItem() { Text = "Encerrados", Value = "7" });
                relat.Add(new SelectListItem() { Text = "Perdido", Value = "8" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

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

                // Atualiza resumo
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                String velho = proc.CRM1_TX_RESUMO;
                String novo = "Promoção para Favorito - " + proc.CRM1_NM_NOME.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (proc.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = proc.CRM1_TX_RESUMO;
                    proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(proc, proc);

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

                // Atualiza resumo
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                String velho = proc.CRM1_TX_RESUMO;
                String novo = "Retirada de Favorito - " + proc.CRM1_NM_NOME.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (proc.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = proc.CRM1_TX_RESUMO;
                    proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(proc, proc);

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

                    // Emite mensagem
                    CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                    LEAD cli = leaApp.GetItemById(item.LEAD_CD_ID.Value);
                    USUARIO usuResp = usuApp.GetItemById(item.USUA_CD_ID.Value);
                    Int32 voltaEM = await ProcessaEnvioEMailProcesso(proc, cli, usuResp, 1);

                    // Atualiza resumo
                    proc = baseApp.GetItemById(item.CRM1_CD_ID);
                    String velho = proc.CRM1_TX_RESUMO;
                    String novo = "Criação de Processo - " + proc.CRM1_NM_NOME.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (proc.CRM1_TX_RESUMO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = proc.CRM1_TX_RESUMO;
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    Int32 voltaR = baseApp.ValidateEdit(proc, proc);

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
            CRM_ACAO acao = new CRM_ACAO();    
            LEAD lead = leaApp.GetItemById(lea.LEAD_CD_ID);
            CRM crm = baseApp.GetItemById(pro.CRM1_CD_ID);
            FUNIL_ETAPA etapa = new FUNIL_ETAPA();

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Recupera Template
            TEMPLATE_EMAIL template = new TEMPLATE_EMAIL();
            if (tipo == 1)
            {
                template = teApp.GetByCode("CRIAPROC", idAss);
            }
            if (tipo == 2)
            {
                template = teApp.GetByCode("CANCPROC", idAss);
            }
            if (tipo == 3)
            {
                template = teApp.GetByCode("CRIAACAO", idAss);
                acao = (CRM_ACAO)Session["AcaoMail"];
            }
            if (tipo == 4)
            {
                template = teApp.GetByCode("ETAPPROC", idAss);
                etapa = (FUNIL_ETAPA)Session["EtapaMail"];
            }

            // Prepara cabeçalho
            String cab = template.TEEM_TX_CABECALHO;

            // Prepara assinatura
            String assinatura = String.Empty;
            assinatura += "Enviado por <b>WebDoctorPro - Administração" + "</b><br />";

            // Prepara corpo da mensagem
            String texto = template.TEEM_TX_CORPO;
            if (tipo == 1)
            {
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
                    texto = texto.Replace("{guid}", lead.LEAD_GU_IDENTIFICADOR);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", crm.CRM1_DT_CRIACAO.Value.ToLongDateString());
                }
            }
            if (tipo == 2)
            {
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
                    texto = texto.Replace("{guid}", lead.LEAD_GU_IDENTIFICADOR);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", crm.CRM1_DT_CRIACAO.Value.ToLongDateString());
                }
                if (texto.Contains("{canc}"))
                {
                    texto = texto.Replace("{canc}", crm.CRM1_DT_CANCELAMENTO.Value.ToLongDateString());
                }
                if (texto.Contains("{motivo}"))
                {
                    texto = texto.Replace("{motivo}", crm.MOTIVO_CANCELAMENTO.MOCA_NM_NOME);
                }
                if (texto.Contains("{just}"))
                {
                    texto = texto.Replace("{just}", crm.CRM1_DS_MOTIVO_CANCELAMENTO);
                }
            }
            if (tipo == 3)
            {
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
                    texto = texto.Replace("{guid}", lead.LEAD_GU_IDENTIFICADOR);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", crm.CRM1_DT_CRIACAO.Value.ToLongDateString());
                }
                if (texto.Contains("{titulo}"))
                {
                    texto = texto.Replace("{titulo}", acao.CRAC_NM_TITULO.ToUpper());
                }
                if (texto.Contains("{prev}"))
                {
                    texto = texto.Replace("{prev}", acao.CRAC_DT_PREVISTA.Value.ToLongDateString());
                }
                if (texto.Contains("{dias}"))
                {
                    texto = texto.Replace("{dias}", ((acao.CRAC_DT_PREVISTA.Value.Date - DateTime.Today.Date).Days).ToString());
                }
            }
            if (tipo == 4)
            {
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
                    texto = texto.Replace("{guid}", lead.LEAD_GU_IDENTIFICADOR);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", crm.CRM1_DT_CRIACAO.Value.ToLongDateString());
                }
                if (texto.Contains("{etapa}"))
                {
                    texto = texto.Replace("{etapa}", etapa.FUET_NM_NOME.ToUpper());
                }
            }

            String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            List<AttachmentModel> models = new List<AttachmentModel>();
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            if (tipo == 1)
            {
                mensagem.ASSUNTO = "Criação de Processo - " + crm.CRM1_NM_NOME.ToUpper();
            }
            if (tipo == 2)
            {
                mensagem.ASSUNTO = "Cancelamento de Processo - " + crm.CRM1_NM_NOME.ToUpper();
            }
            if (tipo == 3)
            {
                mensagem.ASSUNTO = "Criação de Ação - " + acao.CRAC_NM_TITULO.ToUpper();
            }
            if (tipo == 4)
            {
                mensagem.ASSUNTO = "Mudança de Etapa - " + etapa.FUET_NM_NOME.ToUpper();
            }
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
                if (tipo == 1)
                {
                    mens.MENS_NM_NOME = "Criação de Processo CRM - Envio de aviso ao responsável: " + usuario.USUA_NM_NOME;
                }
                if (tipo == 2)
                {
                    mens.MENS_NM_NOME = "Cancelamento de Processo CRM - Envio de aviso ao responsável: " + usuario.USUA_NM_NOME;
                }
                if (tipo == 3)
                {
                    mens.MENS_NM_NOME = "Criação de Ação - Envio de aviso ao responsável: " + usuario.USUA_NM_NOME;
                }
                if (tipo == 4)
                {
                    mens.MENS_NM_NOME = "Mudança de Etapa de Processo - Envio de aviso ao responsável: " + usuario.USUA_NM_NOME;
                }
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
                if (tipo == 1)
                {
                    Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Processo CRM - Criação de Processo");
                }
                if (tipo == 2)
                {
                    Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Processo CRM - Cancelamento de Processo");
                }
                if (tipo == 3)
                {
                    Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Processo CRM - Criação de Ação");
                }
                if (tipo == 4)
                {
                    Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Processo CRM - Mudança de Etapa");
                }
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
                        cell = new PdfPCell(new Paragraph("Perdido", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                    }
                    else if (item.CRM1_IN_ATIVO == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrado", meuFont))
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
                    USUARIO usuResp = usuApp.GetItemById(crm.USUA_CD_ID.Value);
                    LEAD cli = leaApp.GetItemById(crm.LEAD_CD_ID.Value);
                    MOTIVO_CANCELAMENTO can = baseApp.GetMotivoCancelamentoById(crm.MOCA_CD_ID.Value);
                    if (conf.CONF_IN_MENSAGEM_CRM == 1)
                    {
                        Int32 voltaEM = await ProcessaEnvioEMailProcesso(crm, cli, usuResp, 2);
                    }

                    // Atualiza lead
                    LEAD lead = leaApp.GetItemById(crm.LEAD_CD_ID.Value);
                    lead.LEAD_IN_STATUS = 4;
                    lead.LEAD_DT_EXCLUSAO = DateTime.Today.Date;
                    Int32 voltaLead = leaApp.ValidateEdit(lead, lead, usuario);
                    Session["ListaLead"] = null;
                    Session["Leads"] = null;
                    Session["LeadAlterada"] = 1;

                    // Atualiza resumo
                    CRM proc = baseApp.GetItemById(crm.CRM1_CD_ID);
                    String velho = proc.CRM1_TX_RESUMO;
                    String novo = "Cancelamento de Processo - " + proc.CRM1_NM_NOME.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (proc.CRM1_TX_RESUMO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = proc.CRM1_TX_RESUMO;
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O Processo de " + cli.LEAD_NM_NOME.ToUpper() + " foi cancelado com sucesso. Identificação: " + crm.CRM1_GU_GUID;
                    Session["MensCRM"] = 161;

                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 1;
                    Session["CRMNovo"] = crm.CRM1_CD_ID;
                    Session["IdCRM"] = crm.CRM1_CD_ID;
                    Session["CRMAlterada"] = 1;
                    Session["CRMs"] = null;
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "Administra");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult ProcessaRelatorioCRM(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarRelatorioListaCRM");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarRelatorioCRMData");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarRelatorioCRMMes");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarRelatorioCRMStatus");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarRelatorioCRMAtivos");
            }
            if (tipoRel == 6)
            {
                return RedirectToAction("GerarRelatorioCRMCancelados");
            }
            if (tipoRel == 7)
            {
                return RedirectToAction("GerarRelatorioCRMEncerrados");
            }
            if (tipoRel == 8)
            {
                return RedirectToAction("GerarRelatorioCRMPerdidos");
            }
            return RedirectToAction("MontarTelaCRM");
        }

        [HttpGet]
        public ActionResult AcompanhamentoProcessoCRM(Int32 id)
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

                // Mensagens
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 42)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0203", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 43)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 44)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0042", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 52)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0122", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 53)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0123", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0040", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 82)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0140", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 91)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0146", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 92)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0147", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 93)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0424", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensCRM"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0187", CultureInfo.CurrentCulture));
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
                    if ((Int32)Session["MensCRM"] == 777)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0416", CultureInfo.CurrentCulture) + ". ID do envio: " + (String)Session["GuidEnvio"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensCRM"] == 161)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Processa...
                ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Etapa Inicial", Value = "1" });
                status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
                status.Add(new SelectListItem() { Text = "Testando", Value = "3" });
                status.Add(new SelectListItem() { Text = "Aguradando Resposta", Value = "4" });
                status.Add(new SelectListItem() { Text = "Em Negociação", Value = "5" });
                status.Add(new SelectListItem() { Text = "Encerrado", Value = "6" });
                ViewBag.Status = new SelectList(status, "Value", "Text");

                List<SelectListItem> adic = new List<SelectListItem>();
                adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
                adic.Add(new SelectListItem() { Text = "Excluidos", Value = "2" });
                adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
                adic.Add(new SelectListItem() { Text = "Perdidos", Value = "4" });
                adic.Add(new SelectListItem() { Text = "Encerrados", Value = "5" });
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

                List<SelectListItem> envio = new List<SelectListItem>();
                envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Envio = new SelectList(fav, "Value", "Text");
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                // Mensagem
                if (Session["MensCRM"] != null)
                {
                    if ((Int32)Session["MensCRM"] == 161)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                Session["IdCRM"] = id;
                CRM item = baseApp.GetItemById(id);
                CRMViewModel vm = Mapper.Map<CRM, CRMViewModel>(item);
                List<CRM_ACAO> acoes = item.CRM_ACAO.ToList().OrderByDescending(p => p.CRAC_DT_CRIACAO).ToList();
                CRM_ACAO acao = acoes.Where(p => p.CRAC_IN_STATUS == 1).FirstOrDefault();
                Session["ClienteCRM"] = item.LEAD.LEAD_NM_NOME;
                LEAD clie = leaApp.GetItemById(item.LEAD_CD_ID.Value);
                Session["ClienteBase"] = clie;

                Session["SegueInclusao"] = 0;
                Session["Tipo"] = 0;
                Session["TipoHistorico"] = 1;
                Session["VerDia"] = 1;
                Session["NivelLead"] = 1;
                Session["VoltaCRM"] = 11;
                Session["VoltaAgenda"] = 11;

                // Recupera dados do funil
                FUNIL funil = funApp.GetItemById(item.FUNI_CD_ID.Value);
                Session["Funil"] = funil.FUNI_NM_NOME;
                Session["IdFunil"] = funil.FUNI_CD_ID;
                List<FUNIL_ETAPA> etapas = funil.FUNIL_ETAPA.OrderBy(p => p.FUET_IN_ORDEM).ToList();
                Int32? ordemInicial = etapas.First().FUET_IN_ORDEM;

                ViewBag.Etapas = etapas.Count;
                Session["NumEtapas"] = etapas.Count;
                Session["Inicial"] = etapas.First().FUET_CD_ID;

                Int32 atual = item.CRM1_IN_STATUS;
                FUNIL_ETAPA etapaAtual = etapas.Where(p => p.FUET_CD_ID == atual).FirstOrDefault();
                String nomeEtapa = etapaAtual.FUET_NM_NOME;
                ViewBag.NomeEtapa = nomeEtapa;
                Session["EtapaAtual"] = atual;

                Int32? ordemAtual = etapaAtual.FUET_IN_ORDEM;
                if (ordemAtual == ordemInicial)
                {
                    ViewBag.Ant = 1;
                }
                else
                {
                    ViewBag.Ant = 2;     
                }
                if (ordemAtual == etapas.Count)
                {
                    ViewBag.Prox = 1;
                }
                else
                {
                    ViewBag.Prox = 2;
                }

                Int32 encerra = etapaAtual.FUET_IN_ENCERRA;
                ViewBag.Encerra = encerra;
                Int32? etapaEncerra = etapas.Where(p => p.FUET_IN_ENCERRA == 1).FirstOrDefault().FUET_IN_ORDEM;
                Session["EtapaEncerra"] = etapaEncerra;
                ViewBag.EtapaEncerra = etapaEncerra;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM_ACOMPANHAMENTO", "CRM", "AcompanhamentoProcessoCRM");

                // Sessões
                Session["Acoes"] = acoes;
                Session["CRM"] = item;
                Session["VoltaCRM"] = 11;
                Session["VoltaAgendaCRMCalend"] = 10;
                Session["ClienteCRM"] = item.LEAD;
                Session["VoltaAgenda"] = 22;
                ViewBag.Acoes = acoes;
                ViewBag.Acao = acao;
                Session["PontoAcao"] = 2;
                Session["SegueInclusao"] = 0;
                Session["FlagMensagensEnviadas"] = 8;
                Session["FlagMensagensEnviadas"] = 6;
                Session["MensCRM"] = null;
                Session["TipoHistorico"] = 1;
                Session["ListaDiario"] = null;
                Session["CatAgendas"] = null;
                Session["Usuarios"] = null;
                Session["AbaAgenda"] = 1;
                Session["NaoFezNada"] = 5;
                vm.CRM1_TX_RESUMO_OLD = item.CRM1_TX_RESUMO;
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
        [ValidateAntiForgeryToken]
        public ActionResult AcompanhamentoProcessoCRM(CRMViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Origem = new SelectList(CarregaOrigem().OrderBy(p => p.CROR_NM_NOME), "CROR_CD_ID", "CROR_NM_NOME");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Etapa Inicial", Value = "1" });
            status.Add(new SelectListItem() { Text = "Contato Realizado", Value = "2" });
            status.Add(new SelectListItem() { Text = "Testando", Value = "3" });
            status.Add(new SelectListItem() { Text = "Aguradando Resposta", Value = "4" });
            status.Add(new SelectListItem() { Text = "Em Negociação", Value = "5" });
            status.Add(new SelectListItem() { Text = "Encerrado", Value = "6" });
            ViewBag.Status = new SelectList(status, "Value", "Text");

            List<SelectListItem> adic = new List<SelectListItem>();
            adic.Add(new SelectListItem() { Text = "Ativos", Value = "1" });
            adic.Add(new SelectListItem() { Text = "Excluidos", Value = "2" });
            adic.Add(new SelectListItem() { Text = "Cancelados", Value = "3" });
            adic.Add(new SelectListItem() { Text = "Perdidos", Value = "4" });
            adic.Add(new SelectListItem() { Text = "Encerrados", Value = "5" });
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

            List<SelectListItem> envio = new List<SelectListItem>();
            envio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            envio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Envio = new SelectList(fav, "Value", "Text");
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Indicadores
                    ViewBag.Incluir = (Int32)Session["IncluirCRM"];

                    // Sanitização
                    vm.CRM1_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_DS_DESCRICAO);
                    vm.CRM1_NM_CAMPANHA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_NM_CAMPANHA);
                    vm.CRM1_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_NM_NOME);
                    vm.CRM1_TX_INFORMACOES_GERAIS = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRM1_TX_INFORMACOES_GERAIS);

                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Alteração em [" + dataHoje + "] ***";

                    if (vm.CRM1_TX_RESUMO != null)
                    {
                        String velho = vm.CRM1_TX_RESUMO_OLD;
                        String  novo = vm.CRM1_TX_RESUMO;
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            vm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            vm.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        String velho = vm.CRM1_TX_RESUMO_OLD;
                        vm.CRM1_TX_RESUMO = velho;
                    }

                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    CRM item = Mapper.Map<CRMViewModel, CRM>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (CRM)Session["CRM"], usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCRM"] = 60;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 2)
                    {
                        Session["MensCRM"] = 61;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 3)
                    {
                        Session["MensCRM"] = 62;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }
                    if (volta == 4)
                    {
                        Session["MensCRM"] = 63;
                        return RedirectToAction("AcompanhamentoProcessoCRM");
                    }

                    // Retorno
                    listaMaster = new List<CRM>();
                    Session["ListaCRM"] = null;
                    Session["IncluirCRM"] = 0;
                    Session["CRMAlterada"] = 1;
                    Session["FlagCRM"] = 1;
                    Session["LinhaAlterada"] = item.CRM1_CD_ID;
                    Session["FlagAlteraEstado"] = 1;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "CRM";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }
            }
            else
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
        }

        [HttpGet]
        public ActionResult EnviarSMSCliente(Int32 id)
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

                // Prepara
                Int32 crm = (Int32)Session["IdCRM"];
                CRM item = baseApp.GetItemById(crm);
                LEAD cont = leaApp.GetItemById(id);
                Session["Cliente"] = cont;
                ViewBag.Cliente = cont;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM_ENVIO_SMS", "CRM", "EnviarSMSCliente");

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.LEAD_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.LEAD_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.MENS_NM_NOME = "Mensagem para " + cont.LEAD_NM_NOME;
                return View(mens);
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
        public ActionResult EnviarSMSCliente(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idNot = (Int32)Session["IdCRM"];

                    // Sanitização
                    LEAD cont = (LEAD)Session["Cliente"];
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Mensagem para " + cont.LEAD_NM_NOME;

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSCliente(vm, usuarioLogado);

                    // Retorno
                    Session["VoltaTela"] = 0;
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = idNot });
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSCliente(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                Int32 idAss = (Int32)Session["IdAssinante"];
                LEAD cont = (LEAD)Session["Cliente"];

                // Processa SMS
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Recupera CRM
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = vm.MENS_TX_SMS;

                // Prepara corpo do SMS e trata link
                StringBuilder str = new StringBuilder();
                str.AppendLine(vm.MENS_TX_SMS);
                if (!String.IsNullOrEmpty(vm.LINK))
                {
                    if (!vm.LINK.Contains("www."))
                    {
                        vm.LINK = "www." + vm.LINK;
                    }
                    if (!vm.LINK.Contains("http://"))
                    {
                        vm.LINK = "http://" + vm.LINK;
                    }
                    str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                    texto += "  " + vm.LINK;
                }
                String body = str.ToString();
                String smsBody = body;
                String erro = null;
                String resposta = String.Empty;

                // processa envio
                String listaDest = "55" + Regex.Replace(cont.LEAD_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                // Monta o JSON corretamente
                var payload = new
                {
                    destinations = new[]
                    {
                        new {
                            to = listaDest,
                            text = smsBody,
                            customId = customId,
                            from = "WebDoctor"
                        }
    }
                };
                json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                // Prepara requisição
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers["Authorization"] = auth;

                // Converte JSON em bytes e seta ContentLength
                var dataBytes = Encoding.UTF8.GetBytes(json);
                httpWebRequest.ContentLength = dataBytes.Length;

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }

                // Lê resposta
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resposta = streamReader.ReadToEnd();
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.PACI_CD_ID = null;
                env.MEEN_IN_TIPO = 2;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_NR_CELULAR_DESTINO = cont.LEAD_NR_CELULAR;
                env.MEEN_NM_TITULO = "Mensagem SMS para Lead";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_TX_CORPO_COMPLETO = texto;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 2;
                env.MEEN_NM_ORIGEM = "Lead : " + cont.LEAD_NM_NOME;
                env.MEEN_SG_STATUS = "Succeeded";
                env.MEEN_GU_ID_MENSAGEM = Guid.NewGuid().ToString();
                env.MEEN_ID_IDENTIFICADOR = Xid.NewXid().ToString();
                env.MEEN_IN_SISTEMA = 6;
                env.MEEN_IN_ENTREGUE = 1;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID;
                Int32 volta5 = meApp.ValidateCreate(env);
                return 0;
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
                throw;
            }
        }

        [HttpGet]
        public ActionResult EnviarEMailCliente(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Recupera paciente
                LEAD cont = leaApp.GetItemById(id);
                Session["Paciente"] = cont;
                ViewBag.Paciente = cont;
                ViewBag.NomePaciente = cont.LEAD_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_EMAIL", "CRM", "EnviarEMailCliente");

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.LEAD_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.LEAD_EM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = cont.LEAD_EM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem para Lead: " + cont.LEAD_NM_NOME;
                mens.PACI_CD_ID = null;
                mens.MENS_NM_RODAPE = null;
                mens.MENS_NM_ASSINATURA = cont.LEAD_NR_CELULAR;
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                return View(mens);
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
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarEMailCliente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            LEAD cont = (LEAD)Session["Paciente"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Lead: " + cont.LEAD_NM_NOME;

                    // Prepara cabeçalho
                    String cab = "Prezado Sr(a). <b>" + cont.LEAD_NM_NOME + "</b><br />";

                    // Prepara assinatura
                    String rod = "Enviado por <b>WebDoctorPro</b>";

                    // Prepara corpo do e-mail e trata link
                    String corpo = vm.MENS_TX_TEXTO;
                    StringBuilder str = new StringBuilder();
                    str.AppendLine(corpo);
                    if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
                    {
                        if (!vm.MENS_NM_LINK.Contains("www."))
                        {
                            vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                        }
                        if (!vm.MENS_NM_LINK.Contains("http://"))
                        {
                            vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                        }
                        str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui acessar o link " + vm.MENS_NM_LINK + "</a>");
                    }
                    String body = str.ToString();
                    body = body.Replace("\r\n", "<br />");
                    String emailBody = cab + "<br />" + body + "<br />" + rod;

                    // Decriptografa chaves
                    String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                    String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);
                    List<AttachmentModel> models = new List<AttachmentModel>();

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                    EmailAzure mensagem = new EmailAzure();
                    mensagem.ASSUNTO = "Lead - " + cont.LEAD_NM_NOME.ToUpper() + " - Envio de Mensagem";
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_TO_DESTINO = cont.LEAD_EM_EMAIL;
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
                        Session["VoltaExcecao"] = "CRM";
                        Session["Excecao"] = ex;
                        Session["ExcecaoTipo"] = ex.GetType().ToString();
                        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                        Int32 voltaX = grava.GravarLogExcecao(ex, "CRM", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                        return RedirectToAction("TrataExcecao", "Administra");
                    }

                    // Grava mensagem enviada
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = cont.LEAD_NM_NOME;
                    mens.ID = cont.LEAD_CD_ID;
                    mens.MODELO = cont.LEAD_EM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_CAMPANHA = cont.LEAD_EM_EMAIL;
                    mens.MENS_NM_NOME = "Mensagem para Lead";
                    mens.PACI_CD_ID = null;
                    mens.MENS_TX_TEXTO = emailBody;

                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    String guid = Xid.NewXid().ToString();
                    Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Lead - " + cont.LEAD_NM_NOME.ToUpper());


                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Lead - Envio de E-Mail",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.LEAD_NM_NOME + " | Data:" + DateTime.Today.Date.ToShortDateString(),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Mensagem de e-mail para o lead " + cont.LEAD_NM_NOME.ToUpper() + " foi enviada com sucesso.";
                    Session["MensCRM"] = 161;

                    // Sucesso
                    return RedirectToAction("VoltarAcompanhamentoCRM");
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

        [HttpGet]
        public ActionResult IncluirAcao()
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

                // Prepara view
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
                List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> agenda = new List<SelectListItem>();
                agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
                ViewBag.Agenda = new SelectList(agenda, "Value", "Text");

                TimeSpan inicio = TimeSpan.Parse("12:00");
                TimeSpan final = TimeSpan.Parse("13:00");

                CRM_ACAO item = new CRM_ACAO();
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                vm.CRM = (CRM)Session["CRM"];
                vm.CRM1_CD_ID = (Int32)Session["IdCRM"];
                vm.CRAC_IN_ATIVO = 1;
                vm.ASSI_CD_ID = idAss;
                vm.CRAC_DT_CRIACAO = DateTime.Now;
                vm.CRAC_IN_STATUS = 1;
                vm.CRAC_IN_SISTEMA = 6;
                vm.USUA_CD_ID1 = usuario.USUA_CD_ID;
                vm.CRAC_DT_PREVISTA = DateTime.Now.AddDays(Convert.ToDouble(conf.CONF_NR_DIAS_ACAO));
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                vm.CRIA_AGENDA = 2;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IncluirAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> agenda = new List<SelectListItem>();
            agenda.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            agenda.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Agenda = new SelectList(agenda, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CRAC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRAC_DS_DESCRICAO);
                    vm.CRAC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRAC_NM_TITULO);

                    // Verifica tipo de ação
                    if (vm.TIAC_CD_ID == null || vm.TIAC_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0142", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.USUA_CD_ID2 == null || vm.USUA_CD_ID2 == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0143", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    vm.CRM = null;
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateAcao(item, usuarioLogado);

                    // Gera diario
                    CRM not = baseApp.GetItemById(item.CRM1_CD_ID);
                    LEAD cli = leaApp.GetItemById(not.CLIE_CD_ID);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                    dia.DIPR_NM_OPERACAO = "Criação de Ação";
                    dia.DIPR_DS_DESCRICAO = "Criação de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Lead: " + cli.LEAD_NM_NOME;
                    dia.DIPR_IN_SISTEMA = 6;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Processa agenda
                    if (vm.CRIA_AGENDA == 1)
                    {
                        AGENDA ag = new AGENDA();
                        ag.AGEN_DS_DESCRICAO = "Ação: " + vm.CRAC_DS_DESCRICAO;
                        ag.AGEN_DT_DATA = item.CRAC_DT_PREVISTA.Value.Date;
                        ag.AGEN_HR_HORA = item.CRAC_HR_INICIO.Value;
                        ag.AGEN_HR_FINAL = item.CRAC_HR_FINAL.Value;
                        ag.AGEN_IN_ATIVO = 1;
                        ag.AGEN_IN_STATUS = 1;
                        ag.AGEN_NM_TITULO = item.CRAC_NM_TITULO;
                        ag.ASSI_CD_ID = idAss;
                        ag.CAAG_CD_ID = 1;
                        ag.AGEN_CD_USUARIO = item.USUA_CD_ID2;
                        ag.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                        ag.CRM1_CD_ID = item.CRM1_CD_ID;
                        ag.CRAC_CD_ID = item.CRAC_CD_ID;
                        Int32 voltaAg = ageApp.ValidateCreate(ag, usuarioLogado);

                        if (voltaAg > 0)
                        {
                            Session["MensCRM"] = 93;
                        }
                        else
                        {
                            // Gera diario
                            dia = new DIARIO_PROCESSO();
                            dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                            dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                            dia.DIPR_DT_DATA = DateTime.Today.Date;
                            dia.CRM1_CD_ID = item.CRM1_CD_ID;
                            dia.CRAC_CD_ID = item.CRAC_CD_ID;
                            dia.AGEN_CD_ID = ag.AGEN_CD_ID;
                            dia.DIPR_NM_OPERACAO = "Agendamento de Ação";
                            dia.DIPR_DS_DESCRICAO = "Agendamento de Ação " + item.CRAC_NM_TITULO + ". Processo: " + not.CRM1_NM_NOME + ". Lead: " + cli.LEAD_NM_NOME + ". Data: " + ag.AGEN_DT_DATA.ToLongDateString();
                            dia.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID.Value;
                            dia.DIPR_IN_SISTEMA = 6;
                            Int32 volta4 = diaApp.ValidateCreate(dia);
                        }
                    }

                    // Mensagem para responsavel
                    CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                    LEAD lead = leaApp.GetItemById(item.CRM.LEAD_CD_ID.Value);
                    USUARIO usuResp = usuApp.GetItemById(item.USUA_CD_ID2.Value);
                    Session["AcaoMail"] = item;
                    Int32 voltaEM = await ProcessaEnvioEMailProcesso(proc, cli, usuResp, 3);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "CRM - Ação - Criação",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM_ACAO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Atualiza resumo
                    String velho = proc.CRM1_TX_RESUMO;
                    String novo = "Criação de Ação - " + item.CRAC_NM_TITULO.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (proc.CRM1_TX_RESUMO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = proc.CRM1_TX_RESUMO;
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    Int32 voltaR = baseApp.ValidateEdit(proc, proc);


                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A ação " + item.CRAC_NM_TITULO.ToUpper() + " foi criada com sucesso. Processo: " + proc.CRM1_GU_GUID;
                    Session["MensCRM"] = 161;

                    // Verifica retorno
                    Session["CRMAcaoAlterada"] = 1;
                    Session["CRMAlterada"] = 1;
                    Session["ListaCRM"] = null;
                    Session["FlagCRM"] = 1;
                    Session["VoltaTela"] = 1;
                    Session["LinhaAlterada1"] = item.CRAC_CD_ID;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcompanhamentoCRMBase");
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

        [HttpGet]
        public ActionResult EditarAcao(Int32 id)
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

                // Verifica se pode editar ação
                CRM_ACAO item = baseApp.GetAcaoById(id);
                if (item.CRAC_IN_STATUS > 2)
                {
                    Session["MensCRM"] = 43;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Prepara view
                ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
                List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
                ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Monta Status
                List<SelectListItem> status = new List<SelectListItem>();
                if (item.CRAC_IN_STATUS == 1)
                {
                    status.Add(new SelectListItem() { Text = "Pendente", Value = "2" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                    ViewBag.Status = new SelectList(status, "Value", "Text");
                }
                else if (item.CRAC_IN_STATUS == 2)
                {
                    status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                    status.Add(new SelectListItem() { Text = "Encerrada", Value = "3" });
                    ViewBag.Status = new SelectList(status, "Value", "Text");
                }


                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACAO_ALTERAR", "CRM", "EditarAcao");

                // Processa
                Session["Acao"] = item;
                objetoAntes = (CRM)Session["CRM"];
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                vm.DATA_PREVISTA = item.CRAC_DT_PREVISTA.Humanize(culture: new CultureInfo("pt-BR"));
                vm.CRAC_IN_SISTEMA = 6;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
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
        [ValidateAntiForgeryToken]
        public ActionResult EditarAcao(CRMAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Tipos = new SelectList(CarregaTipoAcao().Where(p => p.TIAC_IN_TIPO == 1).OrderBy(p => p.TIAC_NM_NOME), "TIAC_CD_ID", "TIAC_NM_NOME");
            List<USUARIO> listaTotal = CarregaUsuario().Where(p => p.USUA_IN_ESPECIAL == 1).ToList();
            ViewBag.Usuarios = new SelectList(listaTotal.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {

                    // Sanitização
                    vm.CRAC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRAC_DS_DESCRICAO);
                    vm.CRAC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CRAC_NM_TITULO);

                    // Executa a operação
                    CRM_ACAO item = Mapper.Map<CRMAcaoViewModel, CRM_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEditAcao(item);

                    // Gera diario
                    CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                    LEAD cli = leaApp.GetItemById(proc.LEAD_CD_ID.Value);
                    DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                    dia.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    dia.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    dia.DIPR_DT_DATA = DateTime.Today.Date;
                    dia.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia.CRAC_CD_ID = item.CRAC_CD_ID;
                    dia.DIPR_NM_OPERACAO = "Alteração de Ação";
                    dia.DIPR_DS_DESCRICAO = "Alteração de Ação " + item.CRAC_NM_TITULO.ToUpper() + ". Processo: " + proc.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                    dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                    dia.DIPR_IN_SISTEMA = 6;
                    Int32 volta3 = diaApp.ValidateCreate(dia);

                    // Monta Log
                    CRM_ACAO antes = (CRM_ACAO)Session["Acao"];
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "CRM - Ação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Processo: " + proc.CRM1_NM_NOME.ToUpper() + " - Ação: " + item.CRAC_NM_TITULO.ToUpper(),
                        LOG_TX_REGISTRO_ANTES = null,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Atualiza resumo
                    String velho = proc.CRM1_TX_RESUMO;
                    String novo = "Alteração de Ação - " + item.CRAC_NM_TITULO.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (proc.CRM1_TX_RESUMO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = proc.CRM1_TX_RESUMO;
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    Int32 voltaR = baseApp.ValidateEdit(proc, proc);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A ação " + item.CRAC_NM_TITULO.ToUpper() + " foi alterada com sucesso. Processo: " + proc.CRM1_GU_GUID;
                    Session["MensCRM"] = 161;

                    // Verifica retorno
                    Session["ListaCRM"] = null;
                    Session["CRMAlterada"] = 1;
                    Session["CRMAcaoAlterada"] = 1;
                    Session["VoltaTela"] = 1;
                    Session["FlagCRM"] = 1;
                    Session["LinhaAlterada1"] = item.CRAC_CD_ID;
                    ViewBag.Incluir = (Int32)Session["VoltaTela"];
                    return RedirectToAction("VoltarAcaoCRM");
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

        public ActionResult VerAcao(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                CRMAcaoViewModel vm = Mapper.Map<CRM_ACAO, CRMAcaoViewModel>(item);
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
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

        [HttpGet]
        public ActionResult ExcluirAcao(Int32 id)
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
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRAC_IN_ATIVO = 0;
                item.CRAC_IN_STATUS = 4;
                Int32 volta = baseApp.ValidateEditAcao(item);

                // Exclui agendamentos
                if (item.AGENDA.Count > 0)
                {
                    foreach (AGENDA age in item.AGENDA)
                    {
                        AGENDA nova = new AGENDA();
                        nova.AGEN_CD_ID = age.AGEN_CD_ID;
                        nova.AGEN_CD_USUARIO = age.AGEN_CD_USUARIO;
                        nova.AGEN_DS_DESCRICAO = age.AGEN_DS_DESCRICAO;
                        nova.AGEN_DT_DATA = age.AGEN_DT_DATA;
                        nova.AGEN_HR_FINAL = age.AGEN_HR_FINAL;
                        nova.AGEN_HR_HORA = age.AGEN_HR_HORA;
                        nova.AGEN_IN_ATIVO = age.AGEN_IN_ATIVO;
                        nova.AGEN_IN_CONFIRMADO = age.AGEN_IN_CONFIRMADO;
                        nova.AGEN_IN_CORPORATIVA = age.AGEN_IN_CORPORATIVA;
                        nova.AGEN_IN_STATUS = age.AGEN_IN_STATUS;
                        nova.AGEN_LK_REUNIAO = age.AGEN_LK_REUNIAO;
                        nova.AGEN_NM_TITULO = age.AGEN_NM_TITULO;
                        nova.AGEN_TX_OBSERVACOES = age.AGEN_TX_OBSERVACOES;
                        nova.ASSI_CD_ID = age.ASSI_CD_ID;
                        nova.CAAG_CD_ID = age.CAAG_CD_ID;
                        nova.TARE_CD_ID = age.TARE_CD_ID;
                        nova.USUA_CD_ID = age.USUA_CD_ID;
                        nova.CRM1_CD_ID = age.CRM1_CD_ID;
                        nova.CRAC_CD_ID = age.CRAC_CD_ID;
                        nova.EMPR_CD_ID = age.EMPR_CD_ID;
                        nova.AGEN_IN_ATIVO = 0;
                        Int32 volta1 = ageApp.ValidateEdit(nova, usuario);
                    }
                }

                // Gera diario
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                LEAD cli = leaApp.GetItemById(proc.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_NM_OPERACAO = "Exclusão de Ação";
                dia.DIPR_DS_DESCRICAO = "Exclusão de Ação " + item.CRAC_NM_TITULO.ToUpper() + " - Processo: " + proc.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                dia.DIPR_IN_SISTEMA = 6;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                CRM_ACAO antes = (CRM_ACAO)Session["Acao"];
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Ação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + proc.CRM1_NM_NOME + " - Ação: " + item.CRAC_NM_TITULO,
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = proc.CRM1_TX_RESUMO;
                String novo = "Exclusão de Ação - " + item.CRAC_NM_TITULO.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (proc.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = proc.CRM1_TX_RESUMO;
                    proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(proc, proc);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A ação " + item.CRAC_NM_TITULO.ToUpper() + " foi excluida com sucesso. Processo: " + proc.CRM1_GU_GUID;
                Session["MensCRM"] = 161;

                Session["CRMAcaoAlterada"] = 1;
                Session["CRMAlterada"] = 1;
                Session["ListaCRM"] = null;
                Session["VoltaTela"] = 1;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult EncerrarAcao(Int32 id)
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
                CRM_ACAO item = baseApp.GetAcaoById(id);
                objetoAntes = (CRM)Session["CRM"];
                item.CRAC_IN_ATIVO = 0;
                item.CRAC_IN_STATUS = 3;
                Int32 volta = baseApp.ValidateEditAcao(item);

                // Gera diario
                CRM proc = baseApp.GetItemById(item.CRM1_CD_ID);
                LEAD cli = leaApp.GetItemById(proc.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.CRAC_CD_ID = item.CRAC_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                dia.DIPR_NM_OPERACAO = "Encerramento de Ação";
                dia.DIPR_DS_DESCRICAO = "Encerramento de Ação " + item.CRAC_NM_TITULO.ToUpper() + " - Processo: " + proc.CRM1_NM_NOME.ToUpper() + " - Lead: " + cli.LEAD_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Ação - Encerrar",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Processo: " + proc.CRM1_NM_NOME.ToUpper() + " - Ação: " + item.CRAC_NM_TITULO.ToUpper(),
                    LOG_TX_REGISTRO_ANTES = null,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta2 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = proc.CRM1_TX_RESUMO;
                String novo = "Encerrramento de Ação - " + item.CRAC_NM_TITULO.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (proc.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        proc.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = proc.CRM1_TX_RESUMO;
                    proc.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(proc, proc);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A ação " + item.CRAC_NM_TITULO.ToUpper() + " foi encerrada com sucesso. Processo: " + proc.CRM1_GU_GUID;
                Session["MensCRM"] = 161;

                Session["CRMAcaoAlterada"] = 1;
                Session["VoltaTela"] = 1;
                Session["FlagCRM"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("VoltarAcompanhamentoCRMBase");
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
        public async Task<ActionResult> ConfirmarEtapaAnterior()
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

                // Volta etapa anterior
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 etapaAtual = crm.CRM1_IN_STATUS;
                FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
                FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == etapaAtual & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                Int32? ordem = etapa.FUET_IN_ORDEM;
                Int32 etapas = (Int32)Session["NumEtapas"];
                if (ordem == 1 || crm.CRM1_DT_ENCERRAMENTO != null)
                {
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }

                Int32 roda = 1;
                Int32? novaEtapa = ordem;
                FUNIL_ETAPA etapaNova = new FUNIL_ETAPA();
                while (roda == 1)
                {
                    novaEtapa = novaEtapa - 1;
                    etapaNova = funil.FUNIL_ETAPA.Where(p => p.FUNI_CD_ID == funil.FUNI_CD_ID & p.FUET_IN_ORDEM == novaEtapa & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                    if (etapaNova == null)
                    {
                        continue;
                    }
                    break;              
                }           
                crm.CRM1_IN_STATUS = etapaNova.FUET_CD_ID;
                Int32 volta = baseApp.ValidateEditSimples(crm, crm, usuario);

                // Mensagens 
                if (etapa.FUET_IN_EMAIL == 1)
                {
                    CRM proc = baseApp.GetItemById(crm.CRM1_CD_ID);
                    LEAD lead = leaApp.GetItemById(proc.LEAD_CD_ID.Value);
                    USUARIO usuResp = usuApp.GetItemById(proc.USUA_CD_ID.Value);
                    Session["EtapaMail"] = etapaNova;
                    Int32 voltaEM = await ProcessaEnvioEMailProcesso(proc, lead, usuResp, 4);
                }

                // Gera diario
                LEAD cli = leaApp.GetItemById(crm.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                dia.DIPR_NM_OPERACAO = "Mudança de Etapa";
                dia.DIPR_DS_DESCRICAO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Lead: " + cli.LEAD_NM_NOME.ToUpper() + ". Para " + etapaNova.FUET_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Etapa Anterior",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME.ToUpper() + " - Lead: " + cli.LEAD_NM_NOME.ToUpper() + " - Para " + etapaNova.FUET_NM_NOME.ToUpper(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = crm.CRM1_TX_RESUMO;
                String novo = "Mudança de Etapa - " + etapaNova.FUET_NM_NOME.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (crm.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        crm.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = crm.CRM1_TX_RESUMO;
                    crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(crm, crm);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O processo " + crm.CRM1_NM_NOME.ToUpper() + " mudou de etapa com sucesso. Etapa: " + etapaNova.FUET_NM_NOME.ToUpper();
                Session["MensCRM"] = 161;

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["FlagCRM"] = 1;
                Session["VoltaTela"] = 0;
                Session["FlagAlteraEstado"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
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

        [HttpGet]
        public async Task<ActionResult> ConfirmarEtapaProxima()
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

                // Processa etapa
                CRM crm = baseApp.GetItemById((Int32)Session["IdCRM"]);
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32? idFunil = crm.FUNI_CD_ID;
                Int32 etapaAtual = crm.CRM1_IN_STATUS;
                FUNIL funil = funApp.GetItemById(crm.FUNI_CD_ID.Value);
                FUNIL_ETAPA etapa = funil.FUNIL_ETAPA.Where(p => p.FUET_CD_ID == etapaAtual).FirstOrDefault();
                Int32? ordem = etapa.FUET_IN_ORDEM;
                Int32 etapas = (Int32)Session["NumEtapas"];
                if (ordem == etapas)
                {
                    return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
                }

                Int32 roda = 1;
                Int32? novaEtapa = ordem;
                FUNIL_ETAPA etapaNova = new FUNIL_ETAPA();
                while (roda == 1)
                {
                    novaEtapa = novaEtapa + 1;
                    etapaNova = funil.FUNIL_ETAPA.Where(p => p.FUNI_CD_ID == funil.FUNI_CD_ID & p.FUET_IN_ORDEM == novaEtapa & p.FUET_IN_ATIVO == 1).FirstOrDefault();
                    if (etapaNova == null)
                    {
                        continue;
                    }
                    break;
                }
                crm.CRM1_IN_STATUS = etapaNova.FUET_CD_ID;
                Int32 volta = baseApp.ValidateEditSimples(crm, crm, usuario);

                // Processa mensagem
                if (etapa.FUET_IN_EMAIL == 1)
                {
                    CRM proc = baseApp.GetItemById(crm.CRM1_CD_ID);
                    LEAD lead = leaApp.GetItemById(proc.LEAD_CD_ID.Value);
                    USUARIO usuResp = usuApp.GetItemById(proc.USUA_CD_ID.Value);
                    Session["EtapaMail"] = etapaNova;
                    Int32 voltaEM = await ProcessaEnvioEMailProcesso(proc, lead, usuResp, 4);
                }

                // Gera diario
                LEAD cli = leaApp.GetItemById(crm.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = crm.CRM1_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                dia.DIPR_NM_OPERACAO = "Mudança de Etapa";
                dia.DIPR_DS_DESCRICAO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME + ". Lead: " + cli.LEAD_NM_NOME.ToUpper() + ". Para " + etapaNova.FUET_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID.Value;
                Int32 volta3 = diaApp.ValidateCreate(dia);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Proxima Etapa",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Mudança de Etapa. Processo: " + crm.CRM1_NM_NOME.ToUpper() + " - Lead: " + cli.LEAD_NM_NOME.ToUpper() + " - Para " + etapaNova.FUET_NM_NOME.ToUpper(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = crm.CRM1_TX_RESUMO;
                String novo = "Mudança de Etapa - " + etapaNova.FUET_NM_NOME.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (crm.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        crm.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = crm.CRM1_TX_RESUMO;
                    crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(crm, crm);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O processo " + crm.CRM1_NM_NOME.ToUpper() + " mudou de etapa com sucesso. Etapa: " + etapaNova.FUET_NM_NOME.ToUpper();
                Session["MensCRM"] = 161;

                // Retorno
                Session["CRMAlterada"] = 1;
                Session["VoltaTela"] = 0;
                Session["FlagCRM"] = 1;
                Session["FlagAlteraEstado"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
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

        public async Task<ActionResult> UploadFileCRMBlob(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdCRM"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera lead
                CRM item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensCRM"] = 15;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensCRM"] = 16;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensCRM"] = 17;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensCRM"] = 18;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
                // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
                String caminhoRelativo = "Base/CRM/" + item.CRM1_CD_ID.ToString() + "/Anexos/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                // 3. CÓPIA PARA O AZURE BLOB STORAGE
                try
                {
                    // Reinicia o ponteiro do stream para o início após a cópia local
                    file.InputStream.Position = 0;

                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    string connString = conf.CONF_NM_STORAGE_CONN;
                    string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                    var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    // O nome do blob no Azure incluirá toda a estrutura de pastas
                    string blobName = caminhoRelativo + fileName;
                    var blobClient = containerClient.GetBlobClient(blobName);

                    // Upload para o Azure (Idempotente: Se já existe, sobrescreve com true)
                    await blobClient.UploadAsync(file.InputStream, overwrite: true);
                }
                catch (Exception exAzure)
                {
                    Session["MsgCRUD"] = "Erro na sincronização: " + exAzure.Message;
                    Session["MensCRM"] = 61;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }

                // Gravar registro
                CRM_ANEXO foto = new CRM_ANEXO();
                foto.CRAN_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.CRAN_DT_ANEXO = DateTime.Today.Date;
                foto.CRAN_IN_ATIVO = 1;
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
                foto.CRAN_IN_TIPO = tipo;
                foto.CRAN_NM_TITULO = fileName;
                foto.CRM1_CD_ID = item.CRM1_CD_ID;
                item.CRM_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEdit(item, item, usu);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "CRM: " + item.CRM1_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = item.CRM1_TX_RESUMO;
                String novo = "Inclusão de Anexo - " + fileName.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (item.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        item.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        item.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = item.CRM1_TX_RESUMO;
                    item.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(item, item);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O arquivo " + fileName.ToUpper() + " foi anexado com sucesso. Processo: " + item.CRM1_NM_NOME.ToUpper();
                Session["MensCRM"] = 161;

                Session["NivelCRM"] = 2;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult VerAnexoLead(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                Session["NivelCRM"] = 4;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM_ANEXO", "CRM", "VerAnexoCRM");
                return View(item);
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
        public ActionResult VerAnexoCRMAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                Session["NivelCRM"] = 4;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CRM_ANEXO", "CRM", "VerAnexoCRM");
                return View(item);
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
        public ActionResult ExcluirAnexoLead(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                CRM crm = baseApp.GetItemById(item.CRM1_CD_ID);

                item.CRAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "CRM - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "CRM: " + crm.CRM1_NM_NOME.ToUpper() + " | Anexo: " + item.CRAN_NM_TITULO.ToUpper() + " | Data: " + item.CRAN_DT_ANEXO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                String velho = crm.CRM1_TX_RESUMO;
                String novo = "Exclusão de Anexo - " + item.CRAN_NM_TITULO.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (crm.CRM1_TX_RESUMO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        crm.CRM1_TX_RESUMO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = crm.CRM1_TX_RESUMO;
                    crm.CRM1_TX_RESUMO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaR = baseApp.ValidateEdit(crm, crm);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O arquivo " + item.CRAN_NM_TITULO.ToUpper() + " foi desanexado com sucesso. Processo: " + crm.CRM1_NM_NOME.ToUpper();
                Session["MensCRM"] = 161;

                Session["NivelCRM"] = 2;
                Session["CRMAlterada"] = 1;
                return RedirectToAction("VoltarAcompanhamentoCRM");
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
        public ActionResult DownloadCRM(Int32 id)
        {
            // Força o uso de TLS 1.2 (Obrigatório para Azure Storage no .NET 4.8)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                // 1. Carrega as configurações de Storage da sua tabela CONFIGURACAO
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (conf == null) return Content("Erro: Configurações de Storage não encontradas.");

                string connString = conf.CONF_NM_STORAGE_CONN;
                string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                if (string.IsNullOrEmpty(connString)) return Content("Erro: String de conexão do Azure está vazia.");

                // 2. Busca o registro do anexo no banco
                CRM_ANEXO item = baseApp.GetAnexoById(id);
                if (item == null || string.IsNullOrEmpty(item.CRAN_AQ_ARQUIVO))
                {
                    return Content("Erro: Registro do anexo não encontrado no banco de dados.");
                }

                // 3. LIMPEZA DO CAMINHO (Tratamento para o Azure)
                // Remove o '~', remove barras do início e padroniza as barras invertidas
                string caminhoFormatado = item.CRAN_AQ_ARQUIVO.Replace("~", "");
                caminhoFormatado = caminhoFormatado.TrimStart('/');
                caminhoFormatado = caminhoFormatado.Replace("\\", "/");

                // 4. Conexão com o Azure Blob Storage
                var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(caminhoFormatado);

                // 5. Verifica se o arquivo realmente existe no container
                if (!blobClient.Exists())
                {
                    return Content("Erro: Arquivo não localizado no Azure. Caminho tentado: [" + caminhoFormatado + "]");
                }

                // 6. Download do conteúdo para a memória do servidor
                var download = blobClient.DownloadContent();
                byte[] dados = download.Value.Content.ToArray();

                // 7. Define nome e tipo do arquivo
                string nomeDownload = Path.GetFileName(caminhoFormatado);
                string contentType = MimeMapping.GetMimeMapping(nomeDownload);

                // 8. Entrega o arquivo forçando o download no navegador
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;

                Response.ContentType = contentType;
                // Aspas duplas no nome do arquivo tratam nomes com espaços
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nomeDownload + "\"");

                Response.BinaryWrite(dados);
                Response.Flush();
                Response.End();

                return null;
            }
            catch (Exception ex)
            {
                // Gravação de Log de Exceção padrão WebDoctor/RTI
                try
                {
                    var user = Session["UserCredentials"] as USUARIO;
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, user);
                }
                catch { /* Evita erro no catch se a sessão estiver expirada */ }

                return Content("Erro técnico ao realizar download: " + ex.Message);
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
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
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

        public List<TIPO_ACAO> CarregaTipoAcao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_ACAO> conf = new List<TIPO_ACAO>();
                if (Session["TipoAcoes"] == null)
                {
                    conf = baseApp.GetAllTipoAcao(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoAcaoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipoAcao(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_ACAO>)Session["TipoAcoes"];
                    }
                }
                Session["TipoAcoes"] = conf;
                Session["TipoAcaoAlterada"] = 0;
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

        public ActionResult VoltarAcaoCRM()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 x = (Int32)Session["PontoAcao"];
            if ((Int32)Session["VoltaAcao"] == 84)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["VoltaCRM"] == 84)
            {
                return RedirectToAction("VerAcoesUsuarioCRM", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 100)
            {
                return RedirectToAction("MontarTelaDashboardCRMNovo", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 101)
            {
                return RedirectToAction("MontarTelaCRM", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 91)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["PontoAcao"] == 1)
            {
                return RedirectToAction("VerAcoesUsuarioCRMPrevia");
            }
            if ((Int32)Session["PontoAcao"] == 2)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
            }
            if ((Int32)Session["PontoAcao"] == 22)
            {
                return RedirectToAction("MontarTelaHistorico", "CRM");
            }
            if ((Int32)Session["PontoAcao"] == 55)
            {
                return RedirectToAction("VerAcoesUsuarioCRM", "CRM");
            }
            return RedirectToAction("AcompanhamentoProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }

        [HttpGet]
        public ActionResult EncerrarProcessoChamada()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EncerrarProcessoCRM", new { id = (Int32)Session["IdCRM"] });
        }






    }
}