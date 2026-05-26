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

namespace GEDSys_Presentation.Controllers
{
    public class AdministraController : Controller
    {
        private readonly ILeadAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly ICRMAppService crmApp;
        private readonly IAssinanteAppService assApp;
        private readonly IPacienteAppService pacApp;

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
        private LEAD objeto = new LEAD();
        private LEAD objetoAntes = new LEAD();
        private List<LEAD> listaMaster = new List<LEAD>();
        private LOG objetoLog = new LOG();
        private LOG objetoLogAntes = new LOG();
        private List<LOG> listaMasterLog = new List<LOG>();

        public AdministraController(ILeadAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, ICRMAppService crmApps, IAssinanteAppService assApps, IPacienteAppService pacApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            aceApp = aceApps;
            crmApp = crmApps;
            assApp = assApps;
            pacApp = pacApps;
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
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaAdministra()
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
                Session["ModuloAtual"] = "Financeiro";

                // Mensagem
                if (Session["MensAdm"] != null)
                {
                    if ((Int32)Session["MensAdm"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                List<LEAD> leads = new List<LEAD>();
                leads = CarregarLead().Where(p => p.LEAD_IN_ATIVO == 1).ToList();
                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);          
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<CRM> crms = new List<CRM>();
                crms = CarregarCRM().ToList();
                List<CRM_ACAO> acoes = new List<CRM_ACAO>();
                acoes = CarregarAcoes().ToList();
                List<ASSINANTE> assis = CarregarAssinante();
                List<ACESSO_METODO> acessos = CarregarAcessos();
                List<LOG> logs = logApp.GetAllItensDataCorrente().ToList();

                // Carrega widgets
                ViewBag.Assinantes = assis.Count();
                ViewBag.Acessos = acessos.Count();
                ViewBag.Logs = logs.Count();
                ViewBag.Leads = leads.Count();
                ViewBag.CRMs = crms.Count();
                ViewBag.Acoes = acoes.Count();

                // Assinantes por data - ANo corrente
                List<DateTime> datas = assis.Where(p => p.ASSI_DT_INICIO.Value.Year == DateTime.Today.Year).Select(p => p.ASSI_DT_INICIO.Value.Date).Distinct().ToList();
                if (Session["ListaAssinanteData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = assis.Where(p => p.ASSI_DT_INICIO.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.ToShortDateString();
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaAssinanteData = lista;
                    Session["ListaAssinanteData"] = lista;
                }
                else
                {
                    ViewBag.ListaAssinanteData = (List<ModeloViewModel>)Session["ListaAssinanteData"];
                }

                // Acessos por data - Mes corrente
                datas = acessos.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
                if (Session["ListaAcessoData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.ToShortDateString();
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaAcessoData = lista;
                    Session["ListaAcessoData"] = lista;
                }
                else
                {
                    ViewBag.ListaAcessoData = (List<ModeloViewModel>)Session["ListaAcessoData"];
                }

                // Logs por data - Mes corrente
                datas = logs.Where(p => p.LOG_DT_DATA.Value.Month == DateTime.Today.Month & p.LOG_DT_DATA.Value.Year == DateTime.Today.Year).Select(p => p.LOG_DT_DATA.Value.Date).Distinct().ToList();
                if (Session["ListaLogData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = logs.Where(p => p.LOG_DT_DATA.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.ToShortDateString();
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaLogData = lista;
                    Session["ListaLogData"] = lista;
                }
                else
                {
                    ViewBag.ListaLogData = (List<ModeloViewModel>)Session["ListaLogData"];
                }

                // Leads por data - Mes corrente
                datas = leads.Where(p => p.LEAD_DT_ENTRADA.Value.Month == DateTime.Today.Month & p.LEAD_DT_ENTRADA.Value.Year == DateTime.Today.Year).Select(p => p.LEAD_DT_ENTRADA.Value.Date).Distinct().ToList();
                if (Session["ListaLeadData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = leads.Where(p => p.LEAD_DT_ENTRADA.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.ToShortDateString();
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaLeadData = lista;
                    Session["ListaLeadData"] = lista;
                }
                else
                {
                    ViewBag.ListaLeadData = (List<ModeloViewModel>)Session["ListaLeadData"];
                }

                // Recupera Leads por Status
                if (Session["ListaLeadStatus"] == null)
                {
                    List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
                    for (int i = 0; i < 5; i++)
                    {
                        Int32 num = leads.Where(p => p.LEAD_IN_STATUS == i).ToList().Count;
                        if (num > 0)
                        {
                            String nome = String.Empty;
                            if (i == 0)
                            {
                                nome = "Aguardando";
                            }
                            else if (i == 1)
                            {
                                nome = "Processamento";
                            }
                            else if (i == 2)
                            {
                                nome = "Encerrado";
                            }
                            else if (i == 3)
                            {
                                nome = "Pendente";
                            }
                            else if (i == 4)
                            {
                                nome = "Cancelado   ";
                            }

                            ModeloViewModel mod3 = new ModeloViewModel();
                            mod3.Nome = nome;
                            mod3.Valor = num;
                            lista9.Add(mod3);
                        }
                    }
                    ViewBag.ListaLeadStatus = lista9;
                    Session["ListaLeadStatus"] = lista9;
                }
                else
                {
                    ViewBag.ListaLeadStatus = (List<ModeloViewModel>)Session["ListaLeadStatus"];
                }

                // CRM por data - Mes corrente
                datas = crms.Where(p => p.CRM1_DT_CRIACAO.Value.Month == DateTime.Today.Month & p.CRM1_DT_CRIACAO.Value.Year == DateTime.Today.Year).Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
                if (Session["ListaCRMData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = crms.Where(p => p.CRM1_DT_CRIACAO.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.ToShortDateString();
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaCRMData = lista;
                    Session["ListaCRMData"] = lista;
                }
                else
                {
                    ViewBag.ListaCRMData = (List<ModeloViewModel>)Session["ListaCRMData"];
                }

                // Recupera CRM por Status
                if (Session["ListaCRMStatus"] == null)
                {
                    List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
                    for (int i = 0; i < 5; i++)
                    {
                        Int32 num = crms.Where(p => p.CRM1_IN_ATIVO == i).ToList().Count;
                        if (num > 0)
                        {
                            String nome = String.Empty;
                            if (i == 1)
                            {
                                nome = "Ativo";
                            }
                            else if (i == 2)
                            {
                                nome = "Arquivado";
                            }
                            else if (i == 3)
                            {
                                nome = "Cancelado";
                            }
                            else if (i == 4)
                            {
                                nome = "Falhado";
                            }
                            else if (i == 5)
                            {
                                nome = "Sucesso";
                            }

                            ModeloViewModel mod3 = new ModeloViewModel();
                            mod3.Nome = nome;
                            mod3.Valor = num;
                            lista9.Add(mod3);
                        }
                    }
                    ViewBag.ListaCRMStatus = lista9;
                    Session["ListaCRMStatus"] = lista9;
                }
                else
                {
                    ViewBag.ListaCRMStatus = (List<ModeloViewModel>)Session["ListaCRMStatus"];
                }

                // Leads em Processo
                if (Session["LeadsProcesso"] == null)
                {
                    List<LEAD> proc = leads.Where(p => p.LEAD_IN_STATUS == 1).ToList();
                    ViewBag.LeadsProcesso= proc;
                    Session["LeadsProcesso"] = proc;
                }
                else
                {
                    ViewBag.LeadsProcesso = (List<ModeloViewModel>)Session["LeadsProcesso"];
                }

                // CRM em Processo
                if (Session["CRMsProcesso"] == null)
                {
                    List<CRM> procs = crms.Where(p => p.CRM1_IN_ATIVO == 1).ToList();
                    ViewBag.CRMsProcesso = procs;
                    Session["CRMsProcesso"] = procs;
                }
                else
                {
                    ViewBag.CRMsProcesso = (List<ModeloViewModel>)Session["CRMsProcesso"];
                }

                // Acerta estado    
                Session["LeadAlterada"] = 1;

                // Carrega view
                objeto = new LEAD();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ADMIN_DASHBOARD", "Administra", "MontarTelaAdministra");
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Admin";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Admin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaLog()
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
                Session["ModuloAtual"] = "Auditoria";

                // Carrega listas
                ViewBag.Usuarios = new SelectList(usuApp.GetAllItens().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                if ((List<LOG>)Session["ListaLog"] == null)
                {
                    listaMasterLog = logApp.GetAllItensMesCorrente().OrderByDescending(p => p.LOG_DT_DATA).ToList();
                    Session["ListaLog"] = listaMasterLog;
                    Session["FiltroLog"] = null;
                    Session["MensagemLonga"] = 0;
                }
                ViewBag.Listas = (List<LOG>)Session["ListaLog"];
                ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
                ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente().Count;
                ViewBag.LogsMesCorrente = ((List<LOG>)Session["ListaLog"]).Count;
                List<LOG> listAnt = logApp.GetAllItensMesAnterior().OrderByDescending(p => p.LOG_DT_DATA).ToList();
                ViewBag.LogsMesAnterior = listAnt.Count;

                // Mensagens
                if ((Int32)Session["MensLog"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensLog"] == 10)
                {
                    String frase = (String)Session["NumLogBkp"] + CRMSys_Base.ResourceManager.GetString("M0320", CultureInfo.CurrentCulture);
                    ModelState.AddModelError("", frase);
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AUDITORIA", "Administra", "MontarTelaLog");

                // Abre view
                Session["MensLog"] = 0;
                Session["VoltaLog"] = 1;
                objetoLog = new LOG();
                objetoLog.LOG_DT_DATA = DateTime.Today.Date;
                objetoLog.LOG_DT_DUMMY = DateTime.Today.Date;
                return View(objetoLog);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Adminstra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLog()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            return RedirectToAction("MontarTelaLog");
        }

        public ActionResult VerTodosLog()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                DateTime data = DateTime.Today.Date.AddDays(-365);
                listaMasterLog = logApp.GetAllItens().Where(p => p.LOG_DT_DATA >= data).OrderByDescending(p => p.LOG_DT_DATA).ToList();
                Session["ListaLog"] = listaMasterLog;
                Session["MensagemLonga"] = 1;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerMesAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMasterLog = logApp.GetAllItensMesAnterior().OrderByDescending(p => p.LOG_DT_DATA).ToList();
                Session["ListaLog"] = listaMasterLog;
                Session["MensagemLonga"] = 0;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarLog(LOG item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.LOG_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.LOG_NM_OPERACAO);

                // Executa a operação
                List<LOG> listaObj = new List<LOG>();
                Session["FiltroLog"] = item;
                Tuple<Int32, List<LOG>, Boolean> volta = logApp.ExecuteFilterTuple(item.USUA_CD_ID, item.LOG_DT_DATA, item.LOG_DT_DUMMY, item.LOG_NM_OPERACAO);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLog");
                }

                // Sucesso
                listaMasterLog = volta.Item2;
                Session["ListaLog"] = listaMasterLog;
                return RedirectToAction("MontarTelaLog");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLog(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Recupera log
                LOG item = logApp.GetById(id);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AUDITORIA_VER", "Administra", "VerLog");

                // Prepara JSON
                if (item.LOG_TX_REGISTRO != null)
                {
                    if (item.LOG_TX_REGISTRO.Substring(0,1) == "{") 
                    {
                        // Configuracao
                        if (item.LOG_NM_OPERACAO.Substring(3, 4) == "CONF")
                        {
                            CONFIGURACAO antes = JsonConvert.DeserializeObject<CONFIGURACAO>(item.LOG_TX_REGISTRO);
                            String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            String json1 = String.Empty;
                            if (item.LOG_TX_REGISTRO_ANTES != null)
                            {
                                CONFIGURACAO antes1 = JsonConvert.DeserializeObject<CONFIGURACAO>(item.LOG_TX_REGISTRO_ANTES);
                                json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                            }

                            item.LOG_TX_REGISTRO = json;
                            item.LOG_TX_REGISTRO_ANTES = json1;
                        }

                        // Empresa
                        if (item.LOG_NM_OPERACAO.Substring(3, 4) == "EMPR")
                        {
                            EMPRESA antes = JsonConvert.DeserializeObject<EMPRESA>(item.LOG_TX_REGISTRO);
                            String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            String json1 = String.Empty;
                            if (item.LOG_TX_REGISTRO_ANTES != null)
                            {
                                EMPRESA antes1 = JsonConvert.DeserializeObject<EMPRESA>(item.LOG_TX_REGISTRO_ANTES);
                                json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                            }

                            item.LOG_TX_REGISTRO = json;
                            item.LOG_TX_REGISTRO_ANTES = json1;
                        }

                        // Paciente
                        if (item.LOG_NM_OPERACAO.Substring(3, 4) == "PACI")
                        {
                            PACIENTE antes = JsonConvert.DeserializeObject<PACIENTE>(item.LOG_TX_REGISTRO);
                            String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            String json1 = String.Empty;
                            if (item.LOG_TX_REGISTRO_ANTES != null)
                            {
                                PACIENTE antes1 = JsonConvert.DeserializeObject<PACIENTE>(item.LOG_TX_REGISTRO_ANTES);
                                json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                            }

                            item.LOG_TX_REGISTRO = json;
                            item.LOG_TX_REGISTRO_ANTES = json1;
                        }

                        // Template E-Mail
                        if (item.LOG_NM_OPERACAO.Substring(3, 4) == "TEEM")
                        {
                            TEMPLATE_EMAIL antes = JsonConvert.DeserializeObject<TEMPLATE_EMAIL>(item.LOG_TX_REGISTRO);
                            String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            String json1 = String.Empty;
                            if (item.LOG_TX_REGISTRO_ANTES != null)
                            {
                                TEMPLATE_EMAIL antes1 = JsonConvert.DeserializeObject<TEMPLATE_EMAIL>(item.LOG_TX_REGISTRO_ANTES);
                                json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                            }

                            item.LOG_TX_REGISTRO = json;
                            item.LOG_TX_REGISTRO_ANTES = json1;
                        }

                        // Usuarios
                        if (item.LOG_NM_OPERACAO.Substring(3, 4) == "USUA")
                        {
                            USUARIO antes = JsonConvert.DeserializeObject<USUARIO>(item.LOG_TX_REGISTRO);
                            String json = JsonConvert.SerializeObject(antes, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            String json1 = String.Empty;
                            if (item.LOG_TX_REGISTRO_ANTES != null)
                            {
                                USUARIO antes1 = JsonConvert.DeserializeObject<USUARIO>(item.LOG_TX_REGISTRO_ANTES);
                                json1 = JsonConvert.SerializeObject(antes1, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Ignore,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                            }

                            item.LOG_TX_REGISTRO = json;
                            item.LOG_TX_REGISTRO_ANTES = json1;
                        }
                    }
                }

                // Prepara view
                LogViewModel vm = Mapper.Map<LOG, LogViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaAcesso()
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
                Session["ModuloAtual"] = "Acessos";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Carrega listas e parametros
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                List<ACESSO_METODO> acessos = aceApp.GetAllItensMes();
                Session["Acessos"] = acessos;
                List<ACESSO_METODO> acessosYear = aceApp.GetAllItensAno();
                Session["AcessosAno"] = acessosYear;

                List<LOG_EXCECAO_NOVO> falhas = usuApp.GetAllLogExcecaoMes();
                Session["Falhas"] = falhas;
                List<LOG_EXCECAO_NOVO> falhasYear = usuApp.GetAllLogExcecaoAno();
                Session["FalhasAno"] = falhasYear;

                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<ModeloViewModel> listaAcessoDia = new List<ModeloViewModel>();
                List<ModeloViewModel> listaAcessoMes = new List<ModeloViewModel>();

                // Carrega listas de filtros
                List<USUARIO> usus = usuApp.GetAllItens(idAss);
                ViewBag.Usuarios = new SelectList(usus.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Carrega widgets e grid
                List<ACESSO_METODO> acessosMes = acessos;
                List<ACESSO_METODO> acessosAno = acessosYear;
                List<ACESSO_METODO> acessosDia = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                Session["AcessosMes"] = acessosMes;
                Session["AcessosDia"] = acessosDia;
                Session["AcessosAno"] = acessosAno;
                ViewBag.Acessos = acessos;
                ViewBag.AcessosMes = acessosMes;
                ViewBag.AcessosDia = acessosDia;
                ViewBag.AcessosAno = acessosAno;
                ViewBag.AcessosConta = acessos.Count();
                ViewBag.AcessosMesConta = acessosMes.Count();
                ViewBag.AcessosDiaConta = acessosDia.Count();
                ViewBag.AcessosAnoConta = acessosAno.Count();

                List<LOG_EXCECAO_NOVO> falhasMes = falhas;
                List<LOG_EXCECAO_NOVO> falhasAno = falhasYear;
                List<LOG_EXCECAO_NOVO> falhasDia = falhas.Where(p => p.LOEX_DT_DATA.Date == DateTime.Today.Date).ToList();
                Session["FalhasMes"] = falhasMes;
                Session["FalhasDia"] = falhasDia;
                Session["FalhasAno"] = falhasAno;
                ViewBag.Falhas = falhas;
                ViewBag.FalhasMes = falhasMes;
                ViewBag.FalhasAno = falhasAno;
                ViewBag.FalhasDia = falhasDia;
                ViewBag.FalhasConta = falhas.Count();
                ViewBag.FalhasMesConta = falhasMes.Count();
                ViewBag.FalhasAnoConta = falhasAno.Count();
                ViewBag.FalhasDiaConta = falhasDia.Count();

                // Acessos por dia - Mes corrente
                List<DateTime> datas = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 conta = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == item.Date).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta;
                    lista.Add(mod);
                }
                ViewBag.ListaAcessoDia = lista;
                Session["ListaAcessoDia"] = lista;

                // Resumo Mensal Acessos
                datas = acessosAno.Where(p => p.ACES_DT_ACESSO.Value > limite).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String mes2 = null;
                String mesFeito2 = null;
                foreach (DateTime item in datas)
                {
                    if (item.Date > limite)
                    {
                        mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                        if (mes2 != mesFeito2)
                        {
                            Int32 conta = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == item.Month & p.ACES_DT_ACESSO.Value.Date.Year == item.Year & p.ACES_DT_ACESSO > limite).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.Nome = mes2;
                            mod.Valor = conta;
                            listaMes.Add(mod);
                            mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                        }
                    }
                }
                mes2 = null;
                mesFeito2 = null;
                ViewBag.ListaAcessoMes = listaMes;
                Session["ListaAcessoMes"] = listaMes;

                // Acessos por usuario - Mais acessos
                List<Int32> usuarios = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year).Select(p => p.USUA_CD_ID).Distinct().ToList();
                usuarios.Sort((i, j) => i.CompareTo(j));
                List<ModeloViewModel> listaUsu = new List<ModeloViewModel>();
                foreach (Int32 item in usuarios)
                {
                    Int32 conta = acessosMes.Where(p => p.USUA_CD_ID == item).Count();
                    USUARIO usu = usuApp.GetItemById(item);
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = usu.USUA_NM_NOME;
                    mod.Valor = conta;
                    mod.Nome1 = usu.USUA_NM_EMAIL;
                    mod.Nome2 = usu.USUA_NM_LOGIN;
                    mod.Nome3 = usu.USUA_NR_CELULAR;
                    mod.Nome4 = usu.USUA_NR_CPF;
                    if (usu.ESPECIALIDADE != null)
                    {
                        mod.Nome5 = usu.ESPECIALIDADE.ESPE_NM_NOME;
                    }
                    else
                    {
                        mod.Nome5 = "-";
                    }
                    if (usu.TIPO_CARTEIRA_CLASSE != null)
                    {
                        mod.Nome6 = usu.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + " / " + usu.USUA_NR_CLASSE;
                    }
                    else
                    {
                        mod.Nome6 = "-";
                    }
                    mod.Nome7 = idAss.ToString();
                    listaUsu.Add(mod);
                }
                listaUsu = listaUsu.OrderByDescending(p => p.Valor).ToList();
                ViewBag.ListaUsuarioMais = listaUsu;
                Session["ListaUsuarioMais"] = listaUsu;

                // Acessos por páginas - Mais acessos
                List<String> paginas = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year).Select(p => p.ACES_SG_ACESSO).Distinct().ToList();
                paginas.Sort((i, j) => i.CompareTo(j));
                List<ModeloViewModel> listaPag = new List<ModeloViewModel>();
                foreach (String item in paginas)
                {
                    Int32 conta = acessosMes.Where(p => p.ACES_SG_ACESSO == item).Count();
                    ACESSO_METODO acc = acessosMes.Where(p => p.ACES_SG_ACESSO == item).ToList().FirstOrDefault();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item;
                    mod.Valor = conta;
                    mod.Nome1 = acc.ACES_NM_CONTROLLER;
                    mod.Nome2 = acc.ACES_NM_METHOD;
                    listaPag.Add(mod);
                }
                listaPag = listaPag.OrderByDescending(p => p.Valor).ToList();
                ViewBag.ListaPaginaMais = listaPag;
                Session["ListaPaginaMais"] = listaPag;

                // Acessos por faixa de hora - Mês corrente
                Int32 k = 1;
                List<FaixaHoraViewModel> faixas = CarregarFaixas();
                List<ModeloViewModel> listaHora = new List<ModeloViewModel>();
                List<ModeloViewModel> listaHora1 = new List<ModeloViewModel>();
                foreach (FaixaHoraViewModel item in faixas)
                {
                    Int32 conta = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.TimeOfDay >= item.INICIO & p.ACES_DT_ACESSO.Value.TimeOfDay < item.FINAL).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.FAIXA;
                    mod.Valor = conta;
                    mod.Valor1 = k;
                    k++;
                    listaHora.Add(mod);
                }
                listaHora1 = listaHora.OrderBy(p => p.Valor1).ToList();
                listaHora = listaHora.OrderByDescending(p => p.Valor).ToList();
                ViewBag.ListaHora = listaHora;
                Session["ListaHora"] = listaHora;
                Session["ListaHoraGraf"] = listaHora1;

                // Acessos por IP - Mês Corrente
                List<String> ips = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year & p.ACES_IP_IP_LOGIN != null & p.ACES_IP_IP_LOGIN != "::1" & p.ACES_IP_IP_LOGIN != "ip").Select(p => p.ACES_IP_IP_LOGIN).Distinct().ToList();
                ips.Sort((i, j) => i.CompareTo(j));
                List<ModeloViewModel> listaIP = new List<ModeloViewModel>();
                foreach (String item in ips)
                {
                    Int32 conta = acessosMes.Where(p => p.ACES_IP_IP_LOGIN == item).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item;
                    mod.Valor = conta;
                    listaIP.Add(mod);
                }
                listaIP = listaIP.OrderByDescending(p => p.Valor).ToList();
                ViewBag.ListaIP = listaIP;
                Session["ListaIP"] = listaIP;

                // Acerta estado    
                Session["VoltaFinanceiro"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltarPesquisa"] = 0;

                // Carrega view
                ACESSO_METODO objeto = new ACESSO_METODO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTROLE_ACESSO", "Administra", "MontarTelaAcesso");
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerTotalAcessosData()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 3;
            return RedirectToAction("MontarTelaTodosAcessos", "Administra");
        }

        public ActionResult VerTotalAcessosAno()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 1;
            return RedirectToAction("MontarTelaTodosAcessos", "Administra");
        }

        public ActionResult VerTotalAcessosMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 2;
            return RedirectToAction("MontarTelaTodosAcessos", "Administra");
        }

        [HttpGet]
        public ActionResult MontarTelaTodosAcessosUsuarios()
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
            Session["ModuloAtual"] = "Acessos - Usuários";

            try
            {
                // Carrega listas
                List<ModeloViewModel> listaUsuMais = (List<ModeloViewModel>)Session["ListaUsuarioMais"];
                ViewBag.Listas = listaUsuMais;

                // Monta demais listas
                List<USUARIO> listaUsu = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaUsu, "USUA_CD_ID", "USUA_NM_NOME");
                Session["VoltaTela"] = 0;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_USUARIOS", "Administra", "MontarTelaTodosAcessosUsuarios");

                // Abre view
                Session["MensPaciente"] = null;
                ModeloViewModel objeto = new ModeloViewModel();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTodosAcessosFuncoes()
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
            Session["ModuloAtual"] = "Acessos - Funções";

            try
            {
                // Carrega listas
                List<ModeloViewModel> listaUsuMais = (List<ModeloViewModel>)Session["ListaPaginaMais"];
                ViewBag.Listas = listaUsuMais;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_FUNCOES", "Administra", "MontarTelaTodosAcessosFuncoes");

                // Abre view
                Session["MensPaciente"] = null;
                ModeloViewModel objeto = new ModeloViewModel();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTodosAcessosFaixas()
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
            Session["ModuloAtual"] = "Acessos - Usuários";

            try
            {
                // Carrega listas
                List<ModeloViewModel> listaUsuMais = (List<ModeloViewModel>)Session["ListaHora"];
                ViewBag.Listas = listaUsuMais;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_FAIXAS", "Administra", "MontarTelaTodosAcessosFaixas");

                // Abre view
                Session["MensPaciente"] = null;
                ModeloViewModel objeto = new ModeloViewModel();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTodosAcessos()
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
            Session["ModuloAtual"] = "Acessos - Detalhes";

            try
            {
                // Carrega listas
                List<ACESSO_METODO> acessos = new List<ACESSO_METODO>();
                if (Session["ListaAcessoTotal"] == null)
                {
                    acessos = aceApp.GetAllItensAno();
                    if ((Int32)Session["EscopoAcesso"] == 2)
                    {
                        acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == DateTime.Today.Date.Month & p.ACES_DT_ACESSO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                    }
                    if ((Int32)Session["EscopoAcesso"] == 3)
                    {
                        acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                    }
                    Session["ListaAcessoTotal"] = acessos;
                }

                // Aplica filtros pre definidos
                acessos = (List<ACESSO_METODO>)Session["ListaAcessoTotal"];
                ViewBag.Faixa = (Int32)Session["EscopoAcesso"];
                ViewBag.Listas = acessos;

                // Monta demais listas
                List<USUARIO> listaUsu = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaUsu, "USUA_CD_ID", "USUA_NM_NOME");
                List<ASSINANTE> listaAss = CarregaAssinanteRestrito();
                ViewBag.Assinante = new SelectList(listaAss, "ASSI_CD_ID", "ASSI_NM_NOME");
                Session["VoltaTela"] = 0;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL", "Administra", "MontarTelaTodosAcessos");

                // Abre view
                Session["MensPaciente"] = null;
                ACESSO_METODO objeto = new ACESSO_METODO();
                if (Session["FiltroAcessoTotal"] != null)
                {
                    objeto = (ACESSO_METODO)Session["FiltroAcessoTotal"];
                }
                objeto.USUA_CD_ID = usuario.USUA_CD_ID;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarAcessoTotal(ACESSO_METODO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ACESSO_METODO> listaObj = new List<ACESSO_METODO>();
                Session["FiltroAcessoTotal"] = item;
                Tuple<Int32, List<ACESSO_METODO>, Boolean> volta = aceApp.ExecuteFilter(item.ASSI_CD_ID, item.USUA_CD_ID, item.ACES_DT_ACESSO, item.ACES_DT_DUMMY, item.ACES_SG_ACESSO, item.ACES_NM_CONTROLLER, item.ACES_NM_METHOD);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaTodosAcessos");
                }

                // Sucesso
                List<ACESSO_METODO> listaVolta = volta.Item2;
                if ((Int32)Session["EscopoAcesso"] == 2)
                {
                    listaVolta = listaVolta.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == DateTime.Today.Date.Month & p.ACES_DT_ACESSO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                    ViewBag.Faixa = 2;
                }
                if ((Int32)Session["EscopoAcesso"] == 3)
                {
                    listaVolta = listaVolta.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                    ViewBag.Faixa = 3;
                }
                Session["ListaAcessoTotal"] = listaVolta;
                return RedirectToAction("MontarTelaTodosAcessos");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroAcessoTotal()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["ListaAcessoTotal"] = null;
                Session["FiltroAcessoTotal"] = null;
                if ((Int32)Session["EscopoAcesso"] == 2)
                {
                    return RedirectToAction("VerTotalAcessosMes");
                }
                if ((Int32)Session["EscopoAcesso"] == 3)
                {
                    return RedirectToAction("VerTotalAcessosData");
                }
                return RedirectToAction("VerTotalAcessos");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaLead()
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
                Session["ModuloAtual"] = "Solicitações";

                // Carrega listas
                if ((List<LEAD>)Session["ListaLead"] == null)
                {
                    listaMaster = CarregarLead().OrderByDescending(p => p.LEAD_DT_ENTRADA).ToList();
                    Session["ListaLead"] = listaMaster;
                }
                ViewBag.Listas = (List<LEAD>)Session["ListaLead"];
                ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
                ViewBag.UF = new SelectList(CarregaSexo(), "UF_CD_ID", "UF_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Aguardando", Value = "0" });
                status.Add(new SelectListItem() { Text = "Processamento", Value = "1" });
                status.Add(new SelectListItem() { Text = "Encerrado", Value = "2" });
                status.Add(new SelectListItem() { Text = "Pendente", Value = "3" });
                status.Add(new SelectListItem() { Text = "Cancelado", Value = "4" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
                Session["Lead"] = null;

                // Mensagens
                if (Session["MensLead"] != null)
                {
                    if ((Int32)Session["MensLead"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLead"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLead"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0557", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLead"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD", "Administra", "MontarTelaLead");

                // Abre view
                Session["MensLead"] = null;
                Session["VoltaLead"] = 1;
                Session["ListaLog"] = null;
                objeto = new LEAD();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarLead(LEAD item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LEAD> listaObj = new List<LEAD>();
                Tuple<Int32, List<LEAD>, Boolean> volta = baseApp.ExecuteFilter(item.LEAD_DT_ENTRADA, item.LEAD_DT_DUMMY, item.LEAD_NM_NOME, item.LEAD_EM_EMAIL, item.LEAD_IN_STATUS, item.LEAD_NR_CPF, item.LEAD_NR_CNPJ, item.LEAD_NM_CIDADE, item.UF_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLead"] = 1;
                    return RedirectToAction("MontarTelaLead");
                }

                // Sucesso
                Session["MensLead"] = null;
                listaMaster = volta.Item2;
                Session["ListaLead"] = volta.Item2;
                return RedirectToAction("MontarTelaLead");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        public ActionResult RetirarFiltroLead()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLead"] = null;
                return RedirectToAction("MontarTelaLead");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirLead(Int32 id)
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

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LEAD item = baseApp.GetItemById(id);
                item.LEAD_IN_ATIVO = 0;
                item.LEAD_DT_EXCLUSAO = DateTime.Today.Date;
                item.LEAD_IN_STATUS = 4;
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                Session["LeadAlterada"] = 1;
                Session["ListaLead"] = null;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O lead de " + item.LEAD_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensLead"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaLead");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirLead()
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
                Session["ModuloAtual"] = "Leads - Inclusão";

                if (Session["MensLead"] != null)
                {
                    if ((Int32)Session["MensLead"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0684", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
                ViewBag.UF = new SelectList(CarregaSexo(), "UF_CD_ID", "UF_NM_NOME");

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_INCLUIR", "Administra", "IncluirLead");

                // Prepara view
                Session["MensLead"] = null;
                LEAD item = new LEAD();
                LeadViewModel vm = Mapper.Map<LEAD, LeadViewModel>(item);
                vm.LEAD_IN_ATIVO = 1;
                vm.LEAD_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                vm.LEAD_IN_STATUS = 0;
                vm.LEAD_DT_ENTRADA = DateTime.Now;
                item.LEAD_IN_SISTEMA = 6;
                item.LEAD_IN_ENVIOS = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMedico(LeadViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaSexo(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LEAD_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAD_NM_NOME);
                    vm.LEAD_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAD_NM_ENDERECO);
                    vm.LEAD_NM_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAD_NM_COMPLEMENTO);
                    vm.LEAD_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAD_NM_BAIRRO);
                    vm.LEAD_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAD_NM_CIDADE);
                    vm.LEAD_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.LEAD_NR_NUMERO);

                    // Preparação
                    LEAD item = Mapper.Map<LeadViewModel, LEAD>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensLead"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0684", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria Processo
                    LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
                    CRM crm = new CRM();
                    crm.ASSI_CD_ID = 1;
                    crm.CRM1_DS_DESCRICAO = "Processo referente ao lead de " + lead.LEAD_NM_NOME.ToUpper();
                    crm.CRM1_DT_CRIACAO = DateTime.Today.Date;
                    crm.CRM1_GU_GUID = lead.LEAD_GU_IDENTIFICADOR;
                    crm.CRM1_IN_ATIVO = 1;
                    crm.CRM1_IN_ENCERRADO = 0;
                    crm.CRM1_IN_ESTRELA = 1;
                    crm.CRM1_IN_SISTEMA = 6;
                    crm.CRM1_IN_STATUS = 1;
                    crm.CRM1_NM_NOME = "Processo referente ao lead de " + lead.LEAD_NM_NOME.ToUpper();
                    crm.CRM1_NR_TEMPERATURA = 1;
                    crm.EMPR_CD_ID = 3;
                    crm.FUNI_CD_ID = 1;
                    crm.LEAD_CD_ID = lead.LEAD_CD_ID;
                    crm.USUA_CD_ID = 49;
                    crm.CLIE_CD_ID = 2;
                    Int32 volta1 = crmApp.ValidateCreate(crm, usuario);

                    // Sucesso
                    listaMaster = new List<LEAD>();
                    Session["ListaLead"] = null;
                    Session["IdLead"] = item.LEAD_CD_ID;
                    Session["LeadAlterada"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O lead de " + item.LEAD_NM_NOME.ToUpper() + " foi incluído com sucesso. Foi criado um processo associado ao lead";
                    Session["MensMedico"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaLead");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Lead";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
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

        public List<LEAD> CarregarLead()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LEAD> conf = new List<LEAD>();
                if (Session["Leads"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["LeadAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<LEAD>)Session["Leads"];
                    }
                }
                Session["LeadAlterada"] = 0;
                Session["Leads"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administra";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM> CarregarCRM()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM> conf = new List<CRM>();
                if (Session["CRMs"] == null)
                {
                    conf = crmApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["CRMAlterada"] == 1)
                    {
                        conf = crmApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CRM>)Session["CRMs"];
                    }
                }
                Session["CRMAlterada"] = 0;
                Session["CRMs"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administra";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CRM_ACAO> CarregarAcoes()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CRM_ACAO> conf = new List<CRM_ACAO>();
                if (Session["Acoes"] == null)
                {
                    conf = crmApp.GetAllAcoes();
                }
                else
                {
                    if ((Int32)Session["AcaoAlterada"] == 1)
                    {
                        conf = crmApp.GetAllAcoes();
                    }
                    else
                    {
                        conf = (List<CRM_ACAO>)Session["Acoes"];
                    }
                }
                Session["AcaoAlterada"] = 0;
                Session["Acoes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administra";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<ASSINANTE> CarregarAssinante()
        {
            try
            {
                List<ASSINANTE> conf = new List<ASSINANTE>();
                conf = assApp.GetAllItens();
                Session["Assis"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administra";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<ACESSO_METODO> CarregarAcessos()
        {
            try
            {
                List<ACESSO_METODO> conf = new List<ACESSO_METODO>();
                conf = aceApp.GetAllItensDia();
                Session["Acessos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administra";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public JsonResult GetDadosAssinanteAno()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAssinanteData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAcessoMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAcessoData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosLogMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosLeadMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLeadData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosCRMMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCRMData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosLeadStatus()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLeadStatus"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                cor.Add(cores[i]);
                i++;
                if (i > 10)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosCRMStatus()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCRMStatus"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                cor.Add(cores[i]);
                i++;
                if (i > 10)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public List<FaixaHoraViewModel> CarregarFaixas()
        {
            List<FaixaHoraViewModel> conf = new List<FaixaHoraViewModel>();
            TimeSpan? inicio = null;
            TimeSpan? final = null;
            String faixa = String.Empty;

            FaixaHoraViewModel item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("00:00:00");
            item.FINAL = TimeSpan.Parse("01:00:00");
            item.FAIXA = "00:00:00 - 01:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("01:00:00");
            item.FINAL = TimeSpan.Parse("02:00:00");
            item.FAIXA = "01:00:00 - 02:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("02:00:00");
            item.FINAL = TimeSpan.Parse("03:00:00");
            item.FAIXA = "02:00:00 - 03:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("03:00:00");
            item.FINAL = TimeSpan.Parse("04:00:00");
            item.FAIXA = "03:00:00 - 04:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("04:00:00");
            item.FINAL = TimeSpan.Parse("05:00:00");
            item.FAIXA = "04:00:00 - 05:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("05:00:00");
            item.FINAL = TimeSpan.Parse("06:00:00");
            item.FAIXA = "05:00:00 - 06:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("06:00:00");
            item.FINAL = TimeSpan.Parse("07:00:00");
            item.FAIXA = "06:00:00 - 07:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("07:00:00");
            item.FINAL = TimeSpan.Parse("08:00:00");
            item.FAIXA = "07:00:00 - 08:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("08:00:00");
            item.FINAL = TimeSpan.Parse("09:00:00");
            item.FAIXA = "08:00:00 - 09:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("09:00:00");
            item.FINAL = TimeSpan.Parse("10:00:00");
            item.FAIXA = "09:00:00 - 10:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("10:00:00");
            item.FINAL = TimeSpan.Parse("11:00:00");
            item.FAIXA = "10:00:00 - 11:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("11:00:00");
            item.FINAL = TimeSpan.Parse("12:00:00");
            item.FAIXA = "11:00:00 - 12:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("12:00:00");
            item.FINAL = TimeSpan.Parse("13:00:00");
            item.FAIXA = "12:00:00 - 13:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("13:00:00");
            item.FINAL = TimeSpan.Parse("14:00:00");
            item.FAIXA = "13:00:00 - 14:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("14:00:00");
            item.FINAL = TimeSpan.Parse("15:00:00");
            item.FAIXA = "14:00:00 - 15:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("15:00:00");
            item.FINAL = TimeSpan.Parse("16:00:00");
            item.FAIXA = "15:00:00 - 16:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("16:00:00");
            item.FINAL = TimeSpan.Parse("17:00:00");
            item.FAIXA = "16:00:00 - 17:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("17:00:00");
            item.FINAL = TimeSpan.Parse("18:00:00");
            item.FAIXA = "17:00:00 - 18:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("18:00:00");
            item.FINAL = TimeSpan.Parse("19:00:00");
            item.FAIXA = "18:00:00 - 19:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("19:00:00");
            item.FINAL = TimeSpan.Parse("20:00:00");
            item.FAIXA = "19:00:00 - 20:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("20:00:00");
            item.FINAL = TimeSpan.Parse("21:00:00");
            item.FAIXA = "20:00:00 - 21:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("21:00:00");
            item.FINAL = TimeSpan.Parse("22:00:00");
            item.FAIXA = "21:00:00 - 22:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("22:00:00");
            item.FINAL = TimeSpan.Parse("23:00:00");
            item.FAIXA = "22:00:00 - 23:00:00";
            conf.Add(item);
            item = new FaixaHoraViewModel();
            item.INICIO = TimeSpan.Parse("23:00:00");
            item.FINAL = TimeSpan.Parse("00:00:00");
            item.FAIXA = "23:00:00 - 00:00:00";
            conf.Add(item);
            Session["FaixasHoras"] = conf;
            return conf;
        }

        public JsonResult GetAcessosData()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAcessoDia"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetAcessosMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAcessoMes"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
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
                conf = conf.Where(p => p.USUA_IN_SISTEMA == 6).ToList();
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<ASSINANTE> CarregaAssinanteRestrito()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE> conf = new List<ASSINANTE>();
                if (Session["Assinantes"] == null)
                {
                    conf = assApp.GetAllItens();
                }
                else
                {
                    if ((Int32)Session["AssinanteAlterada"] == 1)
                    {
                        conf = assApp.GetAllItens();
                    }
                    else
                    {
                        conf = (List<ASSINANTE>)Session["Assinantes"];
                    }
                }
                Session["Assinantes"] = conf;
                Session["AssinanteAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Base";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Base", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public JsonResult GetAcessosFaixa()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaHoraGraf"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public List<SEXO> CarregaSexo()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<SEXO> conf = new List<SEXO>();
                conf = pacApp.GetAllSexo();
                Session["Sexos"] = conf;
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

        public List<UF> CarregaUF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<UF> conf = new List<UF>();
                conf = pacApp.GetAllUF();
                Session["UF"] = conf;
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

        [HttpPost]
        public JsonResult GetLeadNome(String term)
        {
            List<LEAD> usu = CarregarLead();
            List<String> nomes = usu.Select(p => p.LEAD_NM_NOME).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLeadCidade(String term)
        {
            List<LEAD> usu = CarregarLead();
            List<String> nomes = usu.Select(p => p.LEAD_NM_CIDADE).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }









    }
}