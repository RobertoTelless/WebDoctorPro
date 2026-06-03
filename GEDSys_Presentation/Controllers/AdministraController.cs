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
        private readonly INoticiaAppService notApp;
        private readonly IFunilAppService funApp;

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
        private NOTICIA objetoNot = new NOTICIA();
        private NOTICIA objetoNotAntes = new NOTICIA();
        private List<NOTICIA> listaMasterNot = new List<NOTICIA>();

        public AdministraController(ILeadAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, ICRMAppService crmApps, IAssinanteAppService assApps, IPacienteAppService pacApps, INoticiaAppService notApps, IFunilAppService funApps)
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
            notApp = notApps;
            funApp = funApps;
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
                List<LOG> logs = logApp.GetAllItensMesCorrente().ToList();

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
                        mod.Nome = item.Year.ToString();
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

                // Resumo Mensal Lead
                datas = leads.Where(p => p.LEAD_DT_ENTRADA != null).Select(p => p.LEAD_DT_ENTRADA.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                if (Session["ListaLeadMes"] == null)
                {
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
                                Int32 conta = leads.Where(p => p.LEAD_DT_ENTRADA.Value.Date.Month == item.Month & p.LEAD_DT_ENTRADA.Value.Date.Year == item.Year & p.LEAD_DT_ENTRADA > limite & p.LEAD_IN_ATIVO == 1).Count();
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
                    ViewBag.ListaLeadMes = listaMes;
                    Session["ListaLeadMes"] = listaMes;
                }
                else
                {
                    ViewBag.ListaLeadMes = (List<ModeloViewModel>)Session["ListaLeadMes"];
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

                // Resumo Mensal CRM
                datas = crms.Where(p => p.CRM1_DT_CRIACAO != null).Select(p => p.CRM1_DT_CRIACAO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                if (Session["ListaCRMMes"] == null)
                {
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
                                Int32 conta = crms.Where(p => p.CRM1_DT_CRIACAO.Value.Date.Month == item.Month & p.CRM1_DT_CRIACAO.Value.Date.Year == item.Year & p.CRM1_DT_CRIACAO > limite & p.CRM1_IN_ATIVO > 0).Count();
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
                    ViewBag.ListaCRMMes = listaMes;
                    Session["ListaCRMMes"] = listaMes;
                }
                else
                {
                    ViewBag.ListaCRMMes = (List<ModeloViewModel>)Session["ListaCRMMes"];
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
                    List<LEAD> proc = leads.Where(p => p.LEAD_IN_STATUS == 1).OrderByDescending(p => p.LEAD_DT_ENTRADA).ToList();
                    ViewBag.LeadsProcesso= proc;
                    Session["LeadsProcesso"] = proc;
                }
                else
                {
                    ViewBag.LeadsProcesso = (List<LEAD>)Session["LeadsProcesso"];
                }

                // CRM em Processo
                if (Session["CRMsProcesso"] == null)
                {
                    List<CRM> procs = crms.Where(p => p.CRM1_IN_ATIVO == 1).OrderByDescending(p => p.CRM1_DT_CRIACAO).ToList();
                    ViewBag.CRMsProcesso = procs;
                    Session["CRMsProcesso"] = procs;
                }
                else
                {
                    ViewBag.CRMsProcesso = (List<CRM>)Session["CRMsProcesso"];
                }

                // Acerta estado    
                Session["LeadAlterada"] = 1;
                Session["NivelLead"] = 1;

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
                    listaMasterLog = logApp.GetAllItensDataCorrente().OrderByDescending(p => p.LOG_DT_DATA).ToList();
                    Session["ListaLog"] = listaMasterLog;
                    Session["FiltroLog"] = null;
                    Session["MensagemLonga"] = 0;
                }
                ViewBag.Listas = (List<LOG>)Session["ListaLog"];
                ViewBag.Logs = ((List<LOG>)Session["ListaLog"]).Count;
                ViewBag.LogsDataCorrente = logApp.GetAllItensDataCorrente().Count;
                ViewBag.LogsMesCorrente = logApp.GetAllItensMesCorrente().Count;
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
            return RedirectToAction("MontarTelaLog", "Administra");
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
                return RedirectToAction("MontarTelaLog", "Administra");
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
                return RedirectToAction("MontarTelaLog", "Administra");
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

        public ActionResult VerMesCorrente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMasterLog = logApp.GetAllItensMesCorrente().OrderByDescending(p => p.LOG_DT_DATA).ToList();
                Session["ListaLog"] = listaMasterLog;
                Session["MensagemLonga"] = 0;
                return RedirectToAction("MontarTelaLog", "Administra");
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
                List<ACESSO_METODO> acessosYear = aceApp.GetAllItensMesAnterior();
                Session["AcessosAno"] = acessosYear;

                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<ModeloViewModel> listaAcessoDia = new List<ModeloViewModel>();
                List<ModeloViewModel> listaAcessoMes = new List<ModeloViewModel>();

                // Carrega listas de filtros
                List<USUARIO> usus = usuApp.GetAllItens();
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

                //List<LOG_EXCECAO_NOVO> falhasMes = falhas;
                //List<LOG_EXCECAO_NOVO> falhasAno = falhasYear;
                //List<LOG_EXCECAO_NOVO> falhasDia = falhas.Where(p => p.LOEX_DT_DATA.Date == DateTime.Today.Date).ToList();
                //Session["FalhasMes"] = falhasMes;
                //Session["FalhasDia"] = falhasDia;
                //Session["FalhasAno"] = falhasAno;
                //ViewBag.Falhas = falhas;
                //ViewBag.FalhasMes = falhasMes;
                //ViewBag.FalhasAno = falhasAno;
                //ViewBag.FalhasDia = falhasDia;
                //ViewBag.FalhasConta = falhas.Count();
                //ViewBag.FalhasMesConta = falhasMes.Count();
                //ViewBag.FalhasAnoConta = falhasAno.Count();
                //ViewBag.FalhasDiaConta = falhasDia.Count();

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

                // Acessos por dia - Mes Anterior
                var currentMonth = DateTime.Today.Month;
                var previousMonth = DateTime.Today.AddMonths(-1).Month;
                var year = DateTime.Today.Year;
                if (currentMonth == 1)
                {
                    previousMonth = 12;
                    year -= year;
                }
                datas = acessosAno.Where(p => p.ACES_DT_ACESSO.Value.Month == previousMonth & p.ACES_DT_ACESSO.Value.Year == year).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 conta = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == item.Date).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta;
                    lista1.Add(mod);
                }
                ViewBag.ListaAcessoAnterior = lista1;
                Session["ListaAcessoAnterior"] = lista1;

                //// Resumo Mensal Acessos
                //datas = acessosAno.Where(p => p.ACES_DT_ACESSO.Value > limite).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
                //datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                //List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                //String mes2 = null;
                //String mesFeito2 = null;
                //foreach (DateTime item in datas)
                //{
                //    if (item.Date > limite)
                //    {
                //        mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                //        if (mes2 != mesFeito2)
                //        {
                //            Int32 conta = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == item.Month & p.ACES_DT_ACESSO.Value.Date.Year == item.Year & p.ACES_DT_ACESSO > limite).Count();
                //            ModeloViewModel mod = new ModeloViewModel();
                //            mod.Nome = mes2;
                //            mod.Valor = conta;
                //            listaMes.Add(mod);
                //            mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                //        }
                //    }
                //}
                //mes2 = null;
                //mesFeito2 = null;
                //ViewBag.ListaAcessoMes = listaMes;
                //Session["ListaAcessoMes"] = listaMes;

                // Acessos por usuario - Mais acessos
                //List<Int32> usuarios = acessosMes.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year & p.USUARIO.USUA_IN_ATIVO == 1 & p.ACES_IN_SISTEMA == 6).Select(p => p.USUA_CD_ID).Distinct().ToList();
                List<USUARIO> usuarios = usus;
                //usuarios.Sort((i, j) => i.CompareTo(j));
                List<ModeloViewModel> listaUsu = new List<ModeloViewModel>();
                foreach (USUARIO item in usuarios)
                {
                    Int32 conta = acessosMes.Where(p => p.USUA_CD_ID == item.USUA_CD_ID).Count();
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.USUA_NM_NOME;
                    mod.Valor = conta;
                    mod.Nome1 = item.USUA_NM_EMAIL;
                    mod.Nome2 = item.USUA_NM_LOGIN;
                    mod.Nome3 = item.USUA_NR_CELULAR;
                    mod.Nome4 = item.USUA_NR_CPF;
                    if (item.ESPECIALIDADE != null)
                    {
                        mod.Nome5 = item.ESPECIALIDADE.ESPE_NM_NOME;
                    }
                    else
                    {
                        mod.Nome5 = "-";
                    }
                    if (item.TIPO_CARTEIRA_CLASSE != null)
                    {
                        mod.Nome6 = item.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + " / " + item.USUA_NR_CLASSE;
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
                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Em Análise", Value = "0" });
                status.Add(new SelectListItem() { Text = "Qualificado", Value = "1" });
                status.Add(new SelectListItem() { Text = "Convertido", Value = "2" });
                status.Add(new SelectListItem() { Text = "Pedido", Value = "3" });
                status.Add(new SelectListItem() { Text = "Excluido", Value = "4" });
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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0746", CultureInfo.CurrentCulture));
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
                Session["NivelLead"] = 1;
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

        public ActionResult VerExcluidoLead()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss);
                Session["ListaLead"] = listaMaster;
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

        //[HttpGet]
        //public ActionResult ExcluirLead(Int32 id)
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

        //        // Recupera lead
        //        LEAD item = baseApp.GetItemById(id);
        //        Int32? crmX = item.CRM1_CD_ID;

        //        // Exclui lead
        //        item.LEAD_IN_ATIVO = 0;
        //        item.LEAD_DT_EXCLUSAO = DateTime.Today.Date;
        //        item.LEAD_IN_STATUS = 4;
        //        Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);
        //        if (volta > 0)
        //        {
        //            Session["MensLead"] = 3;
        //            return RedirectToAction("MontarTelaLead");
        //        }

        //        // Atualiza resumo
        //        LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
        //        String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
        //        String novo = "Exclusão de Lead - " + lead.LEAD_DT_ENTRADA.Value.ToLongDateString();
        //        String dataHoje = DateTime.Today.Date.ToLongDateString();
        //        dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
        //        if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
        //        {
        //            String anot = dataHoje + "\r\n" + novo;
        //            if (velho == null & novo != String.Empty)
        //            {
        //                lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
        //            }
        //            if (velho != null & novo != String.Empty)
        //            {
        //                String tripa = velho.Substring(velho.Length - 4, 4);
        //                if (tripa == "\r\n")
        //                {
        //                    velho = velho.Substring(0, velho.Length - 4);
        //                }
        //                lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
        //            }
        //        }
        //        else
        //        {
        //            velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
        //            lead.LEAD_DS_RESUMO_MOVIMENTO = velho;
        //        }

        //        // Grava movimentação
        //        Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuario);

        //        // Mensagem do CRUD
        //        Session["MsgCRUD"] = "O lead de " + item.LEAD_NM_NOME.ToUpper() + " foi excluído com sucesso";
        //        Session["MensLead"] = 61;

        //        // Retorno
        //        Session["LeadAlterada"] = 1;
        //        Session["ListaLead"] = null;
        //        return RedirectToAction("MontarTelaLead");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Lead";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        [HttpPost] // Alterado para Post para suportar textos longos de forma segura
        public ActionResult ExcluirLead(Int32 id, string motivo)
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

                // Recupera lead
                LEAD item = baseApp.GetItemById(id);
                Int32? crmX = item.CRM1_CD_ID;

                // Exclui lead (Soft Delete)
                item.LEAD_IN_ATIVO = 0;
                item.LEAD_DT_EXCLUSAO = DateTime.Today.Date;
                item.LEAD_IN_STATUS = 4;

                // Se o motivo veio vazio por alguma falha física, define um texto padrão
                if (String.IsNullOrEmpty(motivo))
                {
                    item.LEAD_DS_MOTIVO_EXCLUSAO = "Motivo não informado.";
                }
                else
                {
                    item.LEAD_DS_MOTIVO_EXCLUSAO = motivo;
                }
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                // Atualiza resumo agregando o Motivo digitado na Modal
                LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
                String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;

                // Injeta o motivo na string que vai para a transação
                String novo = "Exclusão de Lead - " + lead.LEAD_NM_NOME.ToUpper() +
                              "\r\nMotivo da Exclusão: " + motivo.Trim();

                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";

                if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null && novo != String.Empty)
                    {
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null && novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    // Tratamento caso o resumo antigo esteja nulo no banco
                    lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                }

                // Grava a movimentação com o histórico atualizado
                Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuario);

                // Acerta processo CRM
                CRM crm = crmApp.GetItemById(lead.CRM1_CD_ID.Value);
                crm.CRM1_IN_ATIVO = 2;
                crm.CRM1_DT_EXCLUSAO = DateTime.Today.Date;
                Int32 voltaC = crmApp.ValidateDelete(crm, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O lead de " + item.LEAD_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensLead"] = 61;

                // Retorno
                Session["LeadAlterada"] = 1;
                Session["ListaLead"] = null;
                Session["Leads"] = null;
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
                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");

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
                item.USUA_CD_ID = usuario.USUA_CD_ID;
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
        public ActionResult IncluirLead(LeadViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Sexo = new SelectList(CarregaSexo(), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
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
                    vm.LEAD_IN_SISTEMA = 6;
                    vm.USUA_CD_ID = usuario.USUA_CD_ID;

                    // Monta descrição
                    String desc = "Lead de " + vm.LEAD_NM_NOME.ToUpper() + " criado em " + vm.LEAD_DT_ENTRADA.Value.ToLongDateString();
                    vm.LEAD_DS_DESCRICAO = desc;

                    // Preparação
                    LEAD item = Mapper.Map<LeadViewModel, LEAD>(vm);

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
                    FUNIL fun = CarregarFunil().Where(p => p.FUNI_IN_FIXO == 1).FirstOrDefault();
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
                    crm.FUNI_CD_ID = fun.FUNI_CD_ID;
                    crm.LEAD_CD_ID = lead.LEAD_CD_ID;
                    crm.USUA_CD_ID = 49;
                    crm.CLIE_CD_ID = 2;
                    Int32 volta1 = crmApp.ValidateCreate(crm, usuario);

                    // Atualiza lead
                    lead.CRM1_CD_ID = crm.CRM1_CD_ID;
                    Int32 voltaL = baseApp.ValidateEdit(lead, lead, usuario);

                    // Atualiza resumo
                    lead = baseApp.GetItemById(lead.LEAD_CD_ID);
                    String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    String novo = "Criação de Lead - " + lead.LEAD_NM_NOME.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }

                    // Grava movimentação
                    Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuario);

                    // Sucesso
                    listaMaster = new List<LEAD>();
                    Session["ListaLead"] = null;
                    Session["IdLead"] = item.LEAD_CD_ID;
                    Session["LeadAlterada"] = 1;
                    Session["Leads"] = null;

                    Session["ListaLeadData"] = null;
                    Session["ListaLeadMes"] = null;
                    Session["ListaLeadStatus"] = null;
                    Session["ListaCRMData"] = null;
                    Session["ListaCRMMes"] = null;
                    Session["ListaCRMStatus"] = null;
                    Session["LeadsProcesso"] = null;
                    Session["CRMsProcesso"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O lead de " + item.LEAD_NM_NOME.ToUpper() + " foi incluído com sucesso. Foi criado um processo associado ao lead";
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
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarLead(Int32 id)
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
                Session["ModuloAtual"] = "Lead - Edição";

                LEAD item = baseApp.GetItemById(id);
                Session["Lead"] = item;
                Session["IdLead"] = id;
                Session["StatusLead"] = item.LEAD_IN_STATUS;

                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_NM_NOME");
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Qualificado", Value = "1" });
                ViewBag.Status0 = new SelectList(status, "Value", "Text");
                List<SelectListItem> status1 = new List<SelectListItem>();
                status1.Add(new SelectListItem() { Text = "Convertido", Value = "2" });
                status1.Add(new SelectListItem() { Text = "Perdido", Value = "3" });
                ViewBag.Status1 = new SelectList(status1, "Value", "Text");

                // Mensagens
                if (Session["MensLead"] != null)
                {
                    if ((Int32)Session["MensLead"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLead"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0744", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLead"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0745", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLead"] == 15)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0601", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }

                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_EDITAR", "Administra", "EditarLead");

                // Procesa view
                Session["MensLead"] = null;
                Session["TipoMedicoEnvio"] = 1;
                objetoAntes = item;
                LeadViewModel vm = Mapper.Map<LEAD, LeadViewModel>(item);
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
        public ActionResult EditarLead(LeadViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() { Text = "Qualificado", Value = "1" });
            ViewBag.Status0 = new SelectList(status, "Value", "Text");
            List<SelectListItem> status1 = new List<SelectListItem>();
            status1.Add(new SelectListItem() { Text = "Convertido", Value = "2" });
            status1.Add(new SelectListItem() { Text = "Perdido", Value = "3" });
            ViewBag.Status1 = new SelectList(status1, "Value", "Text");
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
                    vm.LEAD_IN_STATUS = (Int32)Session["StatusLead"];

                    // Critica
                    if (vm.LEAD_DS_DESCRICAO == null)
                    {
                        String desc = "Lead de " + vm.LEAD_NM_NOME.ToUpper() + " criado em " + vm.LEAD_DT_ENTRADA.Value.ToLongDateString();
                        vm.LEAD_DS_DESCRICAO = desc;
                    }
                    if (vm.LEAD_NR_CPF != null)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(vm.LEAD_NR_CPF))
                        {
                            Session["MensLead"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0608", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.LEAD_NR_CNPJ != null)
                    {
                        if (!CrossCutting.ValidarNumerosDocumentos.IsCnpjValid(vm.LEAD_NR_CNPJ))
                        {
                            Session["MensLead"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0609", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    LEAD item = Mapper.Map<LeadViewModel, LEAD>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Atualiza resumo
                    LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
                    String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    String novo = "Alteração de Lead - " + lead.LEAD_NM_NOME.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }


                    // Grava movimentação
                    Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuario);

                    // Sucesso
                    listaMaster = new List<LEAD>();
                    Session["ListaLead"] = null;
                    Session["LeadAlterada"] = 1;
                    Session["Leads"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O lead " + item.LEAD_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensLead"] = 61;

                    return RedirectToAction("EditarLead", new { id = (Int32)Session["IdLead"] });
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

        public async Task<ActionResult> UploadFileLeadBlob(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdLead"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera lead
                LEAD item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensLead"] = 15;
                    return RedirectToAction("VoltarAnexoLead");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensLead"] = 16;
                    return RedirectToAction("VoltarAnexoLead");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensLead"] = 17;
                    return RedirectToAction("VoltarAnexoLead");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensLead"] = 18;
                    return RedirectToAction("VoltarAnexoLead");
                }

                // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
                // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
                String caminhoRelativo = "Base/Lead/" + item.LEAD_CD_ID.ToString() + "/Anexos/";
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
                    Session["MensPaciente"] = 61;
                    return RedirectToAction("VoltarAnexoLead");
                }

                // Gravar registro
                LEAD_ANEXO foto = new LEAD_ANEXO();
                foto.LEAX_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.LEAX_DT_ANEXO = DateTime.Today.Date;
                foto.LEAX_IN_ATIVO = 1;
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
                foto.LEAX_IN_TIPO = tipo;
                foto.LEAX_NM_TITULO = fileName;
                foto.LEAD_CD_ID = item.LEAD_CD_ID;
                item.LEAD_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEdit(item, item, usu);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Lead - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Lead: " + item.LEAD_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
                String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                String novo = "Inclusão de Anexo ao Lead - " + lead.LEAD_NM_NOME.ToUpper() + "\r\nArquivo: " + fileName.Trim();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                }

                // Grava movimentação
                Int32 voltaW = baseApp.ValidateEdit(lead, lead, usu);

                Session["NivelLead"] = 2;
                Session["LeadAlterada"] = 1;
                return RedirectToAction("VoltarAnexoLead");
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

        public ActionResult VoltarAnexoLead()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarLead", new { id = (Int32)Session["IdLead"] });
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
                LEAD_ANEXO item = baseApp.GetLeadAnexoById(id);
                Session["NivelLead"] = 2;
                Session["ModuloAtual"] = "Administra - Lead - Anexos";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_ANEXO", "Administra", "VerAnexoLead");
                return View(item);
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
        public ActionResult VerAnexoLeadAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                LEAD_ANEXO item = baseApp.GetLeadAnexoById(id);
                Session["NivelLead"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_ANEXO", "Administra", "VerAnexoLead");
                return View(item);
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
        public ActionResult ExcluirAnexoLead(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LEAD_ANEXO item = baseApp.GetLeadAnexoById(id);
                LEAD pac = baseApp.GetItemById(item.LEAD_CD_ID);

                item.LEAX_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditLeadAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Lead - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Lead: " + item.LEAX_NM_TITULO.ToUpper() + " | Anexo: " + item.LEAX_NM_TITULO.ToUpper() + " | Data: " + item.LEAX_DT_ANEXO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Atualiza resumo
                LEAD lead = baseApp.GetItemById(item.LEAD_CD_ID);
                String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                String novo = "Exclusão de Anexo ao Lead - " + lead.LEAD_NM_NOME.ToUpper() + "\r\nArquivo: " + item.LEAX_NM_TITULO.Trim();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                }

                // Grava movimentação
                Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuarioLogado);

                Session["NivelLead"] = 2;
                Session["LeadAlterada"] = 1;
                return RedirectToAction("VoltarAnexoLead");
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
        public ActionResult DownloadLead(Int32 id)
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
                LEAD_ANEXO item = baseApp.GetLeadAnexoById(id);
                if (item == null || string.IsNullOrEmpty(item.LEAX_AQ_ARQUIVO))
                {
                    return Content("Erro: Registro do anexo não encontrado no banco de dados.");
                }

                // 3. LIMPEZA DO CAMINHO (Tratamento para o Azure)
                // Remove o '~', remove barras do início e padroniza as barras invertidas
                string caminhoFormatado = item.LEAX_AQ_ARQUIVO.Replace("~", "");
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

        public ActionResult IncluirAnotacaoLead()
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
                Session["NivelLead"] = 3;

                LEAD item = baseApp.GetItemById((Int32)Session["IdLead"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LEAD_ANOTACAO coment = new LEAD_ANOTACAO();
                LeadAnotacaoViewModel vm = Mapper.Map<LEAD_ANOTACAO, LeadAnotacaoViewModel>(coment);
                vm.LEAN_DT_ANOTACAO = DateTime.Now;
                vm.LEAN_IN_ATIVO = 1;
                vm.LEAD_CD_ID = item.LEAD_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_ANOTACAO_INCLUIR", "Administra", "IncluirAnotacaoLead");
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
        public ActionResult IncluirAnotacaoLead(LeadAnotacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LEAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAN_TX_ANOTACAO);

                    // Executa a operação
                    LEAD_ANOTACAO item = Mapper.Map<LeadAnotacaoViewModel, LEAD_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LEAD not = baseApp.GetItemById((Int32)Session["IdLead"]);

                    item.USUARIO = null;
                    not.LEAD_ANOTACAO.Add(item);
                    Int32 volta = baseApp.ValidateEdit(not, not, usuarioLogado);

                    // Sucesso
                    Session["NivelLead"] = 3;
                    return RedirectToAction("VoltarAnexoLead");
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

        [HttpGet]
        public ActionResult ExcluirAnotacaoLead(Int32 id)
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
                LEAD_ANOTACAO item = baseApp.GetAnotacaoById(id);
                item.LEAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["LeadAlterada"] = 1;
                Session["NivelLead"] = 3;

                return RedirectToAction("VoltarAnexoLead");
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
        public ActionResult EditarAnotacaoLead(Int32 id)
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
                Session["NivelLead"] = 3;
                LEAD_ANOTACAO item = baseApp.GetAnotacaoById(id);
                LeadAnotacaoViewModel vm = Mapper.Map<LEAD_ANOTACAO, LeadAnotacaoViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LEAD_ANOTACAO_EDITAR", "Administra", "EditarAnotacaoLead");
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
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoLead(LeadAnotacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LEAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LEAN_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LEAD_ANOTACAO item = Mapper.Map<LeadAnotacaoViewModel, LEAD_ANOTACAO>(vm);
                    LEAD copa = baseApp.GetItemById(item.LEAD_CD_ID);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Verifica retorno
                    Session["LeadAlterada"] = 1;
                    Session["NivelLead"] = 3;
                    return RedirectToAction("VoltarAnexoLead");
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

        public ActionResult GerarRelatorioLead()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "LeadLista" + "_" + data + ".pdf";
                List<LEAD> lista = new List<LEAD>();
                if (Session["ListaLead"] != null)
                {
                    lista = (List<LEAD>)Session["ListaLead"];
                }
                else
                {
                    lista = CarregarLead().ToList();
                }
                lista = lista.OrderBy(p => p.LEAD_DT_ENTRADA).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Leads", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Leads", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 160f, 80f, 60f, 60f, 110f, 60f, 80f });
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
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("UF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (LEAD item in lista)
                {
                    if (item.LEAD_DT_ENTRADA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.LEAD_DT_ENTRADA.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.LEAD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.LEAD_EM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.LEAD_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.LEAD_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Qualificado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LEAD_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Convertido", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LEAD_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Perdido", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LEAD_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Excluido", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LEAD_IN_STATUS == 0)
                    {
                        cell = new PdfPCell(new Paragraph("Em Análise", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.LEAD_NM_CIDADE, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.UF != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.UF.UF_SG_SIGLA, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(" ", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.LEAD_GU_IDENTIFICADOR, meuFont))
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

                Session["NivelLead"] = 1;
                return RedirectToAction("VoltarBaseLead");
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Avisos";

                // Carrega listas
                if ((List<MENSAGEM_FABRICANTE>)Session["ListaMensFab"] == null)
                {
                    List<MENSAGEM_FABRICANTE> listaMF = usuApp.GetAllMensFab(idAss).Where(p => p.MEFA_IN_ATIVO == 1 & p.MEFA_DT_VALIDADE.Date > DateTime.Today.Date).OrderBy(p => p.MEFA_NM_TITULO).ToList();
                    Session["ListaMensFab"] = listaMF;
                }
                ViewBag.Listas = (List<MENSAGEM_FABRICANTE>)Session["ListaMensFab"];
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Informação", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                Session["Aviso"] = null;

                // Indicadores
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_FABRICANTE", "Administra", "MontarTelaAviso");

                // Abre view
                Session["MensAviso"] = null;
                Session["VoltaAviso"] = 1;
                MENSAGEM_FABRICANTE objeto = new MENSAGEM_FABRICANTE();
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Avisos - Inclusão";

                // Prepara listas
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Informação", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_INCLUIR", "Administra", "IncluirAviso");

                // Prepara view
                Session["MensAviso"] = null;
                MENSAGEM_FABRICANTE item = new MENSAGEM_FABRICANTE();
                MensagemFabricanteViewModel vm = Mapper.Map<MENSAGEM_FABRICANTE, MensagemFabricanteViewModel>(item);
                vm.MEFA_IN_ATIVO = 1;
                vm.MEFA_DT_CADASTRO = DateTime.Today.Date;
                vm.MEFA_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                vm.MEFA_IN_SISTEMA = 6;
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirAviso(MensagemFabricanteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Informação", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEFA_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEFA_NM_TITULO);
                    vm.MEFA_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEFA_TX_TEXTO);
                    vm.MEFA_LK_LINK = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEFA_LK_LINK);

                    // Critica
                    if (vm.MEFA_DT_VALIDADE <= vm.MEFA_DT_CADASTRO)
                    {
                        Session["MensAviso"] = 15;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0237", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Preparação
                    MENSAGEM_FABRICANTE item = Mapper.Map<MensagemFabricanteViewModel, MENSAGEM_FABRICANTE>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = usuApp.ValidateCreateMensFab(item);

                    // Sucesso
                    Session["ListaAviso"] = null;
                    Session["IdAviso"] = item.MEFA_CD_ID;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O aviso " + item.MEFA_NM_TITULO.ToUpper() + " foi incluído com sucesso";
                    Session["MensAviso"] = 61;
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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Aviso - Edição";

                MENSAGEM_FABRICANTE item = usuApp.GetMensFabById(id);
                Session["Aviso"] = item;
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Informação", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                // Mensagens
                if (Session["MensAviso"] != null)
                {
                    if ((Int32)Session["MensAviso"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AVISO_EDITAR", "Administra", "EditarAviso");

                Session["MensAviso"] = null;
                Session["IdAviso"] = id;
                MensagemFabricanteViewModel vm = Mapper.Map<MENSAGEM_FABRICANTE, MensagemFabricanteViewModel>(item);
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarAviso(MensagemFabricanteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Informação", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Aviso", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEFA_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEFA_NM_TITULO);
                    vm.MEFA_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEFA_TX_TEXTO);

                    // Critica
                    if (vm.MEFA_DT_VALIDADE <= vm.MEFA_DT_CADASTRO)
                    {
                        Session["MensAviso"] = 15;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0237", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    MENSAGEM_FABRICANTE item = Mapper.Map<MensagemFabricanteViewModel, MENSAGEM_FABRICANTE>(vm);


                    // Processa
                    Int32 volta = usuApp.ValidateEditMensFab(item);

                    // Sucesso
                    Session["ListaAviso"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O aviso " + item.MEFA_NM_TITULO.ToUpper() + " foi alterado com sucesso";
                    Session["MensAviso"] = 61;

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
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                MENSAGEM_FABRICANTE item = usuApp.GetMensFabById(id);
                item.MEFA_IN_ATIVO = 0;
                Int32 volta = usuApp.ValidateEditMensFab(item);
                Session["ListaAviso"] = null;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O aviso " + item.MEFA_NM_TITULO.ToUpper() + " foi excluído com sucesso";
                Session["MensAviso"] = 61;

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

        public List<FUNIL> CarregarFunil()
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
                Session["FunilAlterada"] = 0;
                Session["Funis"] = conf;
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
                conf = aceApp.GetAllItensMes();
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
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaAcessoAnterior"];
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

        [HttpPost]
        public async Task<JsonResult> PesquisaCEP_JavascriptNova(String cep, int tipoEnd)
        {
            // 1. Garante TLS 1.2
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            var url = $"https://viacep.com.br/ws/{cep}/json/";
            var hash = new Hashtable();

            try
            {
                using (var client = new HttpClient())
                {
                    // 2. Faz a requisição e obtém o JSON
                    var response = await client.GetStringAsync(url);

                    // 3. Deserializa o JSON para o objeto
                    var end = JsonConvert.DeserializeObject<CepData>(response);

                    if (end.erro || string.IsNullOrEmpty(end.logradouro))
                    {
                        hash.Add("Sucesso", 0); // CEP não encontrado
                    }
                    else
                    {
                        // 4. Mapeia o resultado (sua lógica original)
                        if (tipoEnd == 1)
                        {
                            hash.Add("Sucesso", 1);
                            hash.Add("LEAD_NM_ENDERECO", end.logradouro);
                            hash.Add("LEAD_NR_NUMERO", end.complemento);
                            hash.Add("LEAD_NM_BAIRRO", end.bairro);
                            hash.Add("LEAD_NM_CIDADE", end.localidade);
                            hash.Add("UF_CD_ID", pacApp.GetUFbySigla(end.uf).UF_CD_ID);

                            // Retorna o CEP formatado
                            // cep já está limpo, o ViaCEP retorna no formato XXXXX-XXX
                            hash.Add("LEAD_NR_CEP", end.cep.Replace("-", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Logar o erro (ex: falha de rede ou JSON inválido)
                hash.Clear();
                hash.Add("Sucesso", 0);
                // Opcionalmente: logar ex.Message
            }

            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public JsonResult GetDadosLeadTotMes()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLeadMes"];
                List<String> dias = new List<String>();
                List<Int32> valor1 = new List<Int32>();
                dias.Add(" ");
                valor1.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.Nome);
                    valor1.Add(item.Valor);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valores", valor1);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosCRMTotMes()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCRMMes"];
                List<String> dias = new List<String>();
                List<Int32> valor1 = new List<Int32>();
                dias.Add(" ");
                valor1.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.Nome);
                    valor1.Add(item.Valor);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valores", valor1);
                return Json(result);
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
                return null;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaNoticia()
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
                Session["ModuloAtual"] = "Noticia";

                // Carrega listas
                if ((List<NOTICIA>)Session["ListaNoticia"] == null)
                {
                    listaMasterNot = CarregaNoticiaGeral();
                    Session["ListaNoticia"] = listaMasterNot;
                }
                ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
                ViewBag.Title = "Notícias";

                // Indicadores
                ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensNoticia"] != null)
                {
                    if ((Int32)Session["MensNoticia"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensNoticia"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Abre view
                objetoNot = new NOTICIA();
                Session["VoltaNoticia"] = 1;
                Session["MensNoticia"] = 0;
                Session["UsuarioEspecial"] = usuario.USUA_IN_ESPECIAL;
                return View(objetoNot);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroNoticiaGeral()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaNoticia"] = null;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoNoticiaGeral()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterNot = notApp.GetAllItensAdm(idAss);
                Session["ListaNoticia"] = listaMasterNot;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarNoticiaGeral(NOTICIA item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Tuple<Int32, List<NOTICIA>, Boolean> volta = notApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensNoticia"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMasterNot = volta.Item2;
                Session["ListaNoticia"] = volta.Item2;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpGet]
        public ActionResult IncluirNoticia()
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
            Session["ModuloAtual"] = "Noticia - Inclusão";
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Prepara view
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            vm.ASSI_CD_ID = 1;
            vm.NOTC_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTC_IN_ATIVO = 1;
            vm.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTC_NR_ACESSO = 0;
            vm.NOTC_IN_SISTEMA = 6;           
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNoticia(NoticiaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.NOTC_NM_AUTOR = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_AUTOR);
                    vm.NOTC_NM_ORIGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_ORIGEM);
                    vm.NOTC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_TITULO);
                    vm.NOTC_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_TX_TEXTO);
                    vm.NOTC_AQ_ARQUIVO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_AQ_ARQUIVO);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = notApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    Session["IdNoticia"] = item.NOTC_CD_ID;

                    // Carrega foto e processa alteracao
                    item.NOTC_AQ_FOTO = "~/Images/p_big2.jpg";
                    volta = notApp.ValidateEdit(item, item, usuarioLogado);

                    if (Session["FileQueueNoticia"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueNoticia"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                            }
                            else
                            {
                                UploadFotoQueueNoticia(file);
                            }
                        }

                        Session["FileQueueNoticia"] = null;
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi incluída com sucesso";
                    Session["MensNoticia"] = 61;

                    // Sucesso
                    listaMasterNot = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["IdVolta"] = item.NOTC_CD_ID;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
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
            Session["FileQueueNoticia"] = queue;
        }

        [HttpPost]
        public void UploadFileToSessionAviso(IEnumerable<HttpPostedFileBase> files, String profile)
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
            Session["FileQueueAviso"] = queue;
        }

        [HttpPost]
        public async Task<Int32> UploadFotoQueueNoticia(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdNoticia"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return 1;
                }

                // Recupera noticia
                NOTICIA item = notApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensNoticia"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensNoticia"] = 7;
                    return 3;
                }


                // 1. DEFINIÇÃO DE CAMINHOS (Removendo a barra inicial para o Azure)
                String caminhoRelativo = "Imagens/Base/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item = notApp.GetById(idNot);
                item.NOTC_AQ_FOTO = "~" + caminhoRelativo + fileName;
                Int32 volta = notApp.ValidateEdit(item, item);
                listaMasterNot = new List<NOTICIA>();
                Session["ListaNoticia"] = null;

                //// Garante que a pasta local existe
                //if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                //// 2. CÓPIA LOCAL
                //System.IO.File.WriteAllBytes(fullPathLocal, file.Contents);

                // 3. CÓPIA PARA O AZURE BLOB STORAGE
                try
                {
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    string connString = conf.CONF_NM_STORAGE_CONN;
                    string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                    var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    // Nome do blob incluindo as "pastas" virtuais
                    string blobName = caminhoRelativo + fileName;
                    var blobClient = containerClient.GetBlobClient(blobName);

                    // Upload idempotente usando MemoryStream
                    using (var ms = new MemoryStream(file.Contents))
                    {
                        await blobClient.UploadAsync(ms, overwrite: true);
                    }
                    Int32 x = 0;
                }
                catch (Exception exAzure)
                {
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        [HttpGet]
        public ActionResult EditarNoticia(Int32 id)
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
            Session["ModuloAtual"] = "Locacao - Inclusão";
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Mensagens
            if (Session["MensNoticia"] !=  null)
            {
                if ((Int32)Session["MensNoticia"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            NOTICIA item = notApp.GetItemById(id);
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            Session["Noticia"] = item;
            Session["IdNoticia"] = id;
            Session["MensNoticia"] = null;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarNoticia(NoticiaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.NOTC_NM_AUTOR = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_AUTOR);
                    vm.NOTC_NM_ORIGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_ORIGEM);
                    vm.NOTC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_TITULO);
                    vm.NOTC_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_TX_TEXTO);
                    vm.NOTC_AQ_ARQUIVO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_AQ_ARQUIVO);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = notApp.ValidateEdit(item, (NOTICIA)Session["Noticia"], usuarioLogado);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi alterada com sucesso";
                    Session["MensNoticia"] = 61;

                    // Sucesso
                    listaMasterNot = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["IdVolta"] = item.NOTC_CD_ID;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
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

        [HttpGet]
        public ActionResult ExcluirNoticia(Int32 id)
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
                NOTICIA item = notApp.GetItemById(id);
                item.NOTC_IN_ATIVO = 0;
                Int32 volta = notApp.ValidateDelete(item, usuarioLogado);

                Session["NoticiaAlterada"] = 1;
                Session["ListaNoticia"] = null;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi excluída com sucesso";
                Session["MensNoticia"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult UploadFotoNoticia(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdNoticia"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensNoticia"] = 5;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // Recupera noticia
                NOTICIA item = notApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensNoticia"] = 6;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensNoticia"] = 7;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // 1. DEFINIÇÃO DE CAMINHOS
                String caminhoRelativo = "Imagens/Base/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                //if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                //// 2. CÓPIA LOCAL
                //file.SaveAs(fullPathLocal);

                // 3. CÓPIA PARA O AZURE BLOB STORAGE (Síncrono)
                try
                {
                    file.InputStream.Position = 0;

                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    string connString = conf.CONF_NM_STORAGE_CONN;
                    string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                    var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    string blobName = caminhoRelativo + fileName;
                    var blobClient = containerClient.GetBlobClient(blobName);

                    // Chamada Síncrona usando .GetRawResponse() ou apenas omitindo await e usando Upload
                    // No SDK novo, usamos Upload(stream, overwrite) para modo síncrono
                    blobClient.Upload(file.InputStream, overwrite: true);
                }
                catch (Exception exAzure)
                {
                    Session["MsgCRUD"] = "Erro na sincronização Azure: " + exAzure.Message;
                    Session["MensPaciente"] = 61;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item.NOTC_AQ_FOTO = "~" + caminhoRelativo + fileName;
                Int32 volta = notApp.ValidateEdit(item, item);
                listaMasterNot = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["NoticiaAlterada"] = 1;

                return RedirectToAction("VoltarAnexoNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNoticia", new { id = idNot });
        }

        public ActionResult VoltarAnexoVerNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdNoticia"];
            return RedirectToAction("VerNoticia", new { id = idNot });
        }

        public List<NOTICIA> CarregaNoticiaGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> conf = new List<NOTICIA>();
                if (Session["NoticiaGeral"] == null)
                {
                    conf = notApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["NoticiaAlterada"] == 1)
                    {
                        conf = notApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<NOTICIA>)Session["NoticiaGeral"];
                    }
                }
                Session["NoticiaGeral"] = conf;
                Session["NoticiaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VerNoticia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            Session["IdNoticia"] = id;
            NOTICIA item = notApp.GetItemById(id);
            item.NOTC_NR_ACESSO = ++item.NOTC_NR_ACESSO;
            Int32 volta = notApp.ValidateEdit(item, item);

            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        public ActionResult VerNoticiaEspecial(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            Session["IdNoticia"] = id;
            NOTICIA item = notApp.GetItemById(id);
            item.NOTC_NR_ACESSO = ++item.NOTC_NR_ACESSO;
            Int32 volta = notApp.ValidateEdit(item, item);

            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        //        [HttpPost]
        //public async Task<ActionResult> UploadFileDocumentoBlob(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        // Inicializa
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idNot = (Int32)Session["IdAviso"];
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Recupera dados
        //        MENSAGEM_FABRICANTE item = usuApp.GetMensFabById(idNot);
        //        USUARIO usuario = (USUARIO)Session["UserCredentials"];

        //        // Criticas
        //        if (file == null)
        //        {
        //            Session["MensLocacao"] = 5;
        //            return RedirectToAction("CarregarContrato");
        //        }

        //        // Critica tamanho nome
        //        var fileName = Path.GetFileName(file.FileName);
        //        if (fileName.Length > 250)
        //        {
        //            Session["MensLocacao"] = 6;
        //            return RedirectToAction("CarregarContrato");
        //        }

        //        // Critica tamanho arquivo
        //        var fileSize = file.ContentLength;
        //        if (fileSize > 50000000)
        //        {
        //            Session["MensLocacao"] = 7;
        //            return RedirectToAction("CarregarContrato");
        //        }

        //        //Recupera tipo de arquivo
        //        extensao = Path.GetExtension(fileName).ToUpper();
        //        if (extensao != ".PDF")
        //        {
        //            Session["MensLocacao"] = 8;
        //            return RedirectToAction("CarregarContrato");
        //        }

        //        // Verifica exatidão do nome
        //        String nome = "Contrato_Locacao_" + pac.PACI_NM_NOME + "_" + item.LOCA_GU_GUID + "_Assinado.pdf";
        //        if (fileName.ToUpper() != nome.ToUpper())
        //        {
        //            Session["MensLocacao"] = 9;
        //            return RedirectToAction("CarregarContrato");
        //        }

        //        // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
        //        // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
        //        String caminhoRelativo = "Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Assinado/";
        //        String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
        //        String fullPathLocal = Path.Combine(caminhoLocal, fileName);

        //        // 3. CÓPIA PARA O AZURE BLOB STORAGE
        //        try
        //        {
        //            // Reinicia o ponteiro do stream para o início após a cópia local
        //            file.InputStream.Position = 0;

        //            CONFIGURACAO conf = CarregaConfiguracaoGeral();
        //            string connString = conf.CONF_NM_STORAGE_CONN;
        //            string containerName = conf.CONF_NM_STORAGE_CONTAINER;

        //            var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
        //            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //            // O nome do blob no Azure incluirá toda a estrutura de pastas
        //            string blobName = caminhoRelativo + fileName;
        //            var blobClient = containerClient.GetBlobClient(blobName);

        //            // Upload para o Azure (Idempotente: Se já existe, sobrescreve com true)
        //            await blobClient.UploadAsync(file.InputStream, overwrite: true);
        //        }
        //        catch (Exception exAzure)
        //        {
        //            Session["MsgCRUD"] = "Erro na sincronização: " + exAzure.Message;
        //            Session["MensPaciente"] = 61;
        //            return RedirectToAction("VoltarAnexoPagamento");
        //        }

        //        // Atualiza locacao
        //        item.LOCA_IN_CONTRATO_ASSINA = 1;
        //        Int32 volta = baseApp.ValidateEdit(item, item, usu);

        //        // Mensagem do CRUD
        //        Session["MsgCRUD"] = "O contrato de locação assinado de " + pac.PACI_NM_NOME.ToUpper() + " foi anexado com sucesso";
        //        Session["MensLocacao"] = 91;
        //        Session["MensArea"] = 61;

        //        // Finaliza
        //        Session["NivelLocacao"] = 1;
        //        Session["LocacaoAlterada"] = 1;
        //        if ((Int32)Session["VoltaContrato"] == 2)
        //        {
        //            return RedirectToAction("VoltarVerLocacao", "AreaPaciente");
        //        }
        //        return RedirectToAction("VoltarEditarLocacao");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Locacao";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        public Int32 GerarId()
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<LEAD> leads = CarregarLead();
            foreach (LEAD item in leads)
            {
                item.LEAD_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                Int32 volta = baseApp.ValidateEdit(item, item, usuario);
            }
            return 0;
        }

        public ActionResult VoltarBaseLead()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MonterTelaLead");
        }

        public ActionResult VerProcessoCRM(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarCRM", new { id = (Int32)Session["IdLead"] });
        }

        public Int32 GerarResumo()
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<LEAD> leads = CarregarLead();
            foreach (LEAD lead in leads)
            {
                String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                String novo = "Criação de Lead - " + lead.LEAD_NM_NOME.ToUpper();
                String dataHoje = DateTime.Today.Date.ToLongDateString();
                dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                {
                    String anot = dataHoje + "\r\n" + novo;
                    if (velho == null & novo != String.Empty)
                    {
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }
                    if (velho != null & novo != String.Empty)
                    {
                        String tripa = velho.Substring(velho.Length - 4, 4);
                        if (tripa == "\r\n")
                        {
                            velho = velho.Substring(0, velho.Length - 4);
                        }
                        lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                    }
                }
                else
                {
                    velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                }
                Int32 voltaW = baseApp.ValidateEdit(lead, lead, usuario);

            }
            return 0;
        }

        [HttpGet]
        public ActionResult QualificarLead(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                // 1. Recupera o lead do banco através do ID passado pelo JS
                LEAD lead = baseApp.GetItemById(id);

                if (lead != null)
                {
                    // 2. Altera o status para qualificado e registra histórico
                    lead.LEAD_IN_STATUS = 1; // 1 = Qualificado
                    lead.LEAD_DT_MOVIMENTO = DateTime.Now;

                    // 3. Persiste no banco de dados
                    baseApp.ValidateEdit(lead, lead, usuarioLogado);

                    // Atualiza resumo
                    lead = baseApp.GetItemById(lead.LEAD_CD_ID);
                    String velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                    String novo = "Qualificação de Lead - " + lead.LEAD_NM_NOME.ToUpper();
                    String dataHoje = DateTime.Today.Date.ToLongDateString();
                    dataHoje = "*** Movimentação em [" + dataHoje + "] ***";
                    if (lead.LEAD_DS_RESUMO_MOVIMENTO != null)
                    {
                        String anot = dataHoje + "\r\n" + novo;
                        if (velho == null & novo != String.Empty)
                        {
                            lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                        }
                        if (velho != null & novo != String.Empty)
                        {
                            String tripa = velho.Substring(velho.Length - 4, 4);
                            if (tripa == "\r\n")
                            {
                                velho = velho.Substring(0, velho.Length - 4);
                            }
                            lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + dataHoje + "\r\n" + novo;
                        }
                    }
                    else
                    {
                        velho = lead.LEAD_DS_RESUMO_MOVIMENTO;
                        lead.LEAD_DS_RESUMO_MOVIMENTO = dataHoje + "\r\n" + novo;
                    }
                    baseApp.ValidateEdit(lead, lead, usuarioLogado);

                    Session["MsgCRUD"] = "O lead de " + lead.LEAD_NM_NOME.ToUpper() + " foi qualificado com sucesso!";
                    Session["MensLead"] = 61;
                }

                // Retorna para a grid geral de Leads atualizada
                Session["ListaLead"] = null;
                Session["Leads"] = null;
                Session["LeadAlterada"] = 1;
                return RedirectToAction("VoltarAnexoLead");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult MudarStatusLead(Int32 id, Int32 novoStatus, String justificativa)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                // Recupera registro do Lead
                LEAD lead = baseApp.GetItemById(id);

                if (lead != null)
                {
                    // Salva o status anterior para possíveis checagens das validações de negócio
                    LEAD leadAntes = baseApp.GetItemById(id);

                    // Atualiza propriedades
                    lead.LEAD_IN_STATUS = novoStatus;
                    lead.LEAD_DT_MOVIMENTO = DateTime.Now;
                    lead.LEAD_DS_MOTIVO_EXCLUSAO = justificativa;

                    string tipoMovimentacao = (novoStatus == 2) ? "CONVERSÃO DE LEAD" : "LEAD MARCADO COMO PERDIDO";

                    // Monta o novo bloco de histórico para a transação
                    string dataHoje = DateTime.Today.Date.ToLongDateString();
                    string blocoMovimento = $"*** Movimentação em [{dataHoje}] ***\r\n" +
                                            $"{tipoMovimentacao}\r\n" +
                                            $"Justificativa: {justificativa.Trim()}";

                    // Concatena de forma limpa com o histórico antigo
                    if (string.IsNullOrEmpty(lead.LEAD_DS_RESUMO_MOVIMENTO))
                    {
                        lead.LEAD_DS_RESUMO_MOVIMENTO = blocoMovimento;
                    }
                    else
                    {
                        string velho = lead.LEAD_DS_RESUMO_MOVIMENTO.TrimEnd();
                        lead.LEAD_DS_RESUMO_MOVIMENTO = velho + "\r\n\r\n" + blocoMovimento;
                    }

                    // Persiste a edição no banco através do seu AppService
                    Int32 voltaW = baseApp.ValidateEdit(lead, leadAntes, usuarioLogado);

                    // Define feedbacks na Session
                    Session["MsgCRUD"] = $"O status do lead de {lead.LEAD_NM_NOME.ToUpper()} foi atualizado com sucesso.";
                    Session["MensLead"] = 61;
                }

                Session["LeadAlterada"] = 1;
                Session["Leads"] = null;
                Session["ListaLead"] = null; // Reseta cache da lista para forçar re-load
                return RedirectToAction("VoltarAnexoLead");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Lead";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                grava.GravarLogExcecao(ex, "Administra", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public Int32 AcertarTelefone()
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<LEAD> leads = CarregarLead();
            String novo = String.Empty;
            foreach (LEAD item in leads)
            {
                if (item.LEAD_NR_CELULAR != null)
                {
                    String tel = item.LEAD_NR_CELULAR.Trim();
                    if (!tel.Contains("-"))
                    {
                        novo = tel.Substring(0, 8) + "-" + tel.Substring(9);
                    }
                    else
                    {
                        continue;
                    }
                    item.LEAD_NR_CELULAR = novo;
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);
                }
            }
            return 0;
        }

        public Int32 AcertarData()
        {
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<LEAD> leads = CarregarLead();
            foreach (LEAD item in leads)
            {
                if (item.LEAD_IN_STATUS == 1)
                {
                    item.LEAD_DT_MOVIMENTO = item.LEAD_DT_ENTRADA.Value.Date;
                    Int32 volta = baseApp.ValidateEdit(item, item, usuario);
                }
            }
            return 0;
        }


    }
}