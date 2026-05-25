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

        public AdministraController(ILeadAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, ICRMAppService crmApps, IAssinanteAppService assApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            aceApp = aceApps;
            crmApp = crmApps;
            assApp = assApps;
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


















    }
}