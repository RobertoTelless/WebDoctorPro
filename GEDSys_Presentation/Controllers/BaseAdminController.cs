using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using GEDSys_Presentation.Controllers;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using CrossCutting;
using System.Text.RegularExpressions;
using Azure.Communication.Email;
using System.Threading.Tasks;
using System.Reflection;
using ERP_Condominios_Solution.Classes;
using System.Data.Entity;
using XidNet;
using CRMPresentation.App_Start;
using GEDSys_Presentation.App_Start;
using Twilio.TwiML.Voice;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;
using HtmlAgilityPack;
using Common.Logging.Simple;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using EntitiesServices.WorkClasses;
using System.Configuration;
using System.Diagnostics;
using EntitiesServices.Work_Classes;
using System.Net.Mail;
using System.Net.Mime;

namespace ERP_Condominios_Solution.Controllers
{
    public class BaseAdminController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IAssinanteAppService assiApp;
        private readonly IPlanoAppService planApp;
        private readonly ITemplateAppService temApp;
        private readonly IMensagemEnviadaSistemaAppService envApp;
        private readonly ITemplateEMailAppService mailApp;
        private readonly IEmpresaAppService empApp;
        private readonly IPerfilAppService perfApp;
#pragma warning disable CS0649 // Campo "BaseAdminController.mensApp" nunca é atribuído e sempre terá seu valor padrão null
        private readonly IMensagemAppService mensApp;
#pragma warning restore CS0649 // Campo "BaseAdminController.mensApp" nunca é atribuído e sempre terá seu valor padrão null
        private readonly IRecursividadeAppService recApp;
        private readonly IPacienteAppService pacApp;
        private readonly IGrupoAppService gruApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IConvenioAppService convApp;
        private readonly IEspecialidadeAppService espApp;
        private readonly IAvisoLembreteAppService avApp;
        private readonly ITemplateEMailAppService teApp;
        private readonly ITipoPacienteAppService tpApp;
        private readonly ITipoExameAppService tiApp;
        private readonly ITipoPagamentoAppService tgApp;
        private readonly ITipoValorConsultaAppService tcApp;
        private readonly ITipoAtestadoAppService taApp;
        private readonly IValorConsultaAppService vcApp;
        private readonly IPeriodicidadeAppService peApp;
        private readonly IConfiguracaoAnamneseAppService caApp;
        private readonly IConfiguracaoCalendarioAppService ccApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly ILocacaoAppService locApp;
        private readonly ITipoPagamentoAppService tpgApp;
        private readonly ITipoValorConsultaAppService ticoApp;
        private readonly IUnidadeAppService uniApp;

#pragma warning disable CS0169 // O campo "BaseAdminController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "BaseAdminController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "BaseAdminController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "BaseAdminController.exception" nunca é usado
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        MENSAGENS_ENVIADAS_SISTEMA objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
        MENSAGENS_ENVIADAS_SISTEMA objetEnviadaoAntes = new MENSAGENS_ENVIADAS_SISTEMA();
        List<MENSAGENS_ENVIADAS_SISTEMA> listaMasterEnviada = new List<MENSAGENS_ENVIADAS_SISTEMA>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IAssinanteAppService assiApps, IPlanoAppService planApps, ITemplateAppService temApps, IMensagemEnviadaSistemaAppService envApps, ITemplateEMailAppService mailApps, IEmpresaAppService empApps, IPerfilAppService perfApps, IMensagemAppService mensApps, IRecursividadeAppService recApps, IPacienteAppService pacApps, IGrupoAppService gruApps, IAcessoMetodoAppService aceApps, IConvenioAppService convApps, IEspecialidadeAppService espApps, IAvisoLembreteAppService avApps, ITemplateEMailAppService teApps, ITipoExameAppService tiApps, ITipoPacienteAppService tpApps, ITipoPagamentoAppService tgApps, ITipoValorConsultaAppService tcApps, ITipoAtestadoAppService taApps, IValorConsultaAppService vcApps, IPeriodicidadeAppService peApps, IConfiguracaoAnamneseAppService caApps, IConfiguracaoCalendarioAppService ccApps, ITemplateSMSAppService smsApps, ILocacaoAppService locApps, ITipoPagamentoAppService tpgApps, ITipoValorConsultaAppService ticoApps, IUnidadeAppService uniApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            assiApp = assiApps;
            planApp = planApps;
            temApp = temApps;
            envApp = envApps;
            mailApp = mailApps;
            empApp = empApps;
            recApp = recApps;
            pacApp = pacApps;
            gruApp = gruApps;
            aceApp = aceApps;
            convApp = convApps;
            espApp = espApps;
            avApp = avApps;
            teApp = teApps;
            tpApp = tpApps;
            tiApp = tiApps;
            tgApp = tgApps;
            tcApp = tcApps;
            taApp = taApps;
            vcApp = vcApps;
            peApp = peApps;
            caApp = caApps;
            ccApp = ccApps;
            perfApp = perfApps;
            smsApp = smsApps;
            locApp = locApps;
            tpgApp = tpgApps;
            ticoApp = ticoApps;
            uniApp = uniApps;
        }

        public ActionResult CarregarAdmin()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = baseApp.GetAllUsuarios(idAss.Value).Count;
            ViewBag.Logs = logApp.GetAllItens(idAss.Value).Count;
            ViewBag.UsuariosLista = baseApp.GetAllUsuarios(idAss.Value);
            ViewBag.LogsLista = logApp.GetAllItens(idAss.Value);
            return View();

        }

        public ActionResult CarregarLandingPage()
        {
            // Grava Acesso
            String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }
            Session["IPBase"] = ip;
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso("BaseAdmin", "Site", ip);

            Session["CompraState"] = null;
            return View();
        }

        public JsonResult GetRefreshTime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            return Json(confApp.GetAllItems(idAss).FirstOrDefault().CONF_NR_REFRESH_DASH);
        }

        public ActionResult CarregarDesenvolvimento()
        {
            return View();
        }

        public ActionResult CarregarCompraFake()
        {
            return View();
        }

        public ActionResult VoltarDashboard()
        {
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        public ActionResult VoltarIniciarPagamento()
        {
            PagamentoPagBankViewModel pag = (PagamentoPagBankViewModel)Session["DadosPagto"];
            return RedirectToAction("IniciarPagamento", new { id = pag });
        }

        public ActionResult VoltaPesquisa()
        {
            Session["VoltarPesquisa"] = 0;
            Session["VoltaPesquisa"] = null;
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        public ActionResult VoltarDashboardAdministracao()
        {
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAdministracao()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_ADMIN == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = ((PERFIL)Session["PerfilUsuario"]).PERF_SG_SIGLA;

                // Recupera listas usuarios
                List<USUARIO> listaTotal = CarregaUsuario();
                List<USUARIO> bloqueados = listaTotal.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList();

                Int32 numUsuarios = listaTotal.Count;
                Int32 numBloqueados = bloqueados.Count;

                ViewBag.NumUsuarios = numUsuarios;
                ViewBag.NumBloqueados = numBloqueados;

                Session["TotalUsuarios"] = listaTotal.Count;
                Session["Bloqueados"] = numBloqueados;

                // Recupera listas log
                List<LOG> listaLog = logApp.GetAllItensMesCorrente(idAss);
                Int32 log = listaLog.Count;
                Int32 logDia = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == DateTime.Today.Date).ToList().Count;
                Int32 logMes = listaLog.Count;
                List<LOG> listaDia = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == DateTime.Today.Date).ToList();
                List<LOG> listaMes = listaLog;

                ViewBag.Log = log;
                ViewBag.LogDia = logDia;
                ViewBag.LogMes = logMes;

                Session["TotalLog"] = log;
                Session["LogDia"] = logDia;
                Session["LogMes"] = logMes;

                // Recupera acessos
                List<USUARIO_LOGIN> listaAcessos = baseApp.GetAllLogin(idAss).Where(p => p.USLO_IN_SISTEMA == 6).ToList();
                Int32 acessos = listaAcessos.Count;
                Int32 acessosHoje = listaAcessos.Where(p => p.USLO_DT_LOGIN.Date == DateTime.Today.Date).ToList().Count;
                ViewBag.AcessosHoje = acessosHoje;

                // Resumo Log Diario
                List<DateTime> datasCR = listaMes.Where(m => m.LOG_DT_DATA.Value != null).OrderByDescending(m => m.LOG_DT_DATA.Value).Select(p => p.LOG_DT_DATA.Value.Date).Distinct().ToList();
                List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
                foreach (DateTime item in datasCR)
                {
                    Int32 conta = listaLog.Where(p => p.LOG_DT_DATA.Value.Date == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.DataEmissao = item;
                    mod1.Valor = conta;
                    listaLogDia.Add(mod1);
                }
                listaLogDia = listaLogDia.OrderBy(p => p.DataEmissao).ToList();
                ViewBag.ListaLogDia = listaLogDia;
                ViewBag.ContaLogDia = listaLogDia.Count;
                Session["ListaDatasLog"] = datasCR;
                Session["ListaLogResumo"] = listaLogDia;

                // Resumo Log Situacao  
                List<String> opLog = listaLog.Select(p => p.LOG_NM_OPERACAO).Distinct().ToList();
                List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
                foreach (String item in opLog)
                {
                    Int32 conta1 = listaLog.Where(p => p.LOG_NM_OPERACAO == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = item;
                    mod1.Valor = conta1;
                    lista2.Add(mod1);
                }
                ViewBag.ListaLogOp = lista2;
                ViewBag.ContaLogOp = lista2.Count;
                Session["ListaOpLog"] = opLog;
                Session["ListaLogOp"] = lista2;

                // Recupera acessos
                List<USUARIO_LOGIN> listaLogin = baseApp.GetAllLogin(idAss).Where(p => p.USLO_IN_SISTEMA == 6).ToList();
                List<USUARIO_LOGIN> listaLoginDia = listaLogin.Where(p => p.USLO_DT_LOGIN.Date == DateTime.Today.Date).ToList();
                List<USUARIO_LOGIN> listaLoginMes = listaLogin.Where(p => p.USLO_DT_LOGIN.Month == DateTime.Today.Date.Month & p.USLO_DT_LOGIN.Year == DateTime.Today.Date.Year).ToList();
                ViewBag.listaLogin = listaLogin.Count;
                ViewBag.listaLoginDia = listaLoginDia.Count;
                ViewBag.listaLoginMes = listaLoginMes.Count;
                Session["listaLogin"] = listaLogin;
                Session["listaLoginDia"] = listaLoginDia;
                Session["listaLoginMes"] = listaLoginMes;

                // Resumo Acesso Diario
                List<DateTime> datasAcesso = listaLoginMes.Where(m => m.USLO_DT_LOGIN != null).OrderByDescending(m => m.USLO_DT_LOGIN).Select(p => p.USLO_DT_LOGIN.Date).Distinct().ToList();
                List<ModeloViewModel> listaAcessoDia = new List<ModeloViewModel>();
                foreach (DateTime item in datasAcesso)
                {
                    Int32 conta = listaLoginMes.Where(p => p.USLO_DT_LOGIN.Date == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.DataEmissao = item;
                    mod1.Valor = conta;
                    listaAcessoDia.Add(mod1);
                }
                listaAcessoDia = listaAcessoDia.OrderBy(p => p.DataEmissao).ToList();
                ViewBag.listaAcessoDia = listaAcessoDia;
                ViewBag.listaAcessoDiaConta = listaAcessoDia.Count;
                Session["listaAcessoDia"] = listaAcessoDia;

                // Resumo Acesso por usuario
                List<Int32> usuAcesso = listaLoginMes.Where(m => m.USLO_DT_LOGIN != null).OrderByDescending(m => m.USUA_CD_ID).Select(p => p.USUA_CD_ID).Distinct().ToList();
                List<ModeloViewModel> listaAcessoUsuario = new List<ModeloViewModel>();
                foreach (Int32 item in usuAcesso)
                {
                    Int32 conta = listaLoginMes.Where(p => p.USUA_CD_ID == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = usuApp.GetItemById(item).USUA_NM_NOME;
                    mod1.Valor = conta;
                    listaAcessoUsuario.Add(mod1);
                }
                listaAcessoUsuario = listaAcessoUsuario.OrderBy(p => p.Nome).ToList();
                ViewBag.listaAcessoUsuario = listaAcessoUsuario;
                ViewBag.listaAcessoUsuarioConta = listaAcessoUsuario.Count;
                Session["listaAcessoUsuario"] = listaAcessoUsuario;

                Session["VoltaDash"] = 3;
                Session["VoltaUnidade"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/4/Ajuda4.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ADMIN_DASH", "BaseAdmin", "MontarTelaDashboardAdministracao");

                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Administração";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Administração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public JsonResult GetDadosGraficoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogResumo"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosGraficoAcessoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["listaAcessoDia"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosGraficoAcessoUsuario()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["listaAcessoUsuario"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosGraficoLogOper()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
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

        [HttpGet]
        public ActionResult PesquisarTudo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Mensagem
                if (Session["MensBase"] != null)
                {
                    if ((Int32)Session["MensBase"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0200", CultureInfo.CurrentCulture));
                    }
                }
                Session["MensBase"] = null;

                if ((List<VOLTA_PESQUISA>)Session["VoltaPesquisa"] != null)
                {
                    ViewBag.Listas = (List<VOLTA_PESQUISA>)Session["VoltaPesquisa"];
                }
                else
                {
                    ViewBag.Listas = null;
                }

                // Processa
                Session["VoltarPesquisa"] = 1;
                MensagemWidgetViewModel vm = new MensagemWidgetViewModel();
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult PesquisarTudo(MensagemWidgetViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.Descrição = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.Descrição);

                    // Verifica preenchimento
                    if (vm.Descrição == null)
                    {
                        Session["MensBase"] = 10;
                        return RedirectToAction("PesquisarTudo");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    List<VOLTA_PESQUISA> voltaPesq = usuApp.PesquisarTudo(vm.Descrição.Trim(), usuarioLogado, idAss);
                    Session["VoltaPesquisa"] = voltaPesq;

                    // Mensagens
                    Session["MensProntuario"] = 0;
                    Session["MensMensagem"] = 0;
                    Session["AbaProntuario"] = 1;
                    Session["RetornoPesquisa"] = 1;

                    // Verifica retorno
                    return RedirectToAction("PesquisarTudo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Pesquisa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Pesquisa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTabelasAuxiliares()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltarTabs"] = 1;
            Session["ListaLog"] = null;
            ViewBag.PermProntuario = (Int32)Session["PermProntuario"];

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TAB_AUX", "BaseAdmin", "MontarTelaTabelasAuxiliares");

            return View(usuario);
        }

        public ActionResult MontarTelaDashboardCadastros()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3.pdf";

            // Carrega valores dos cadastros
            List<TEMPLATE_EMAIL> tempMail = CarregaTemplateEMail();
            Int32 mails = tempMail.Count;

            List<PACIENTE> pacientes = CarregaPaciente();
            Int32 pacNum = pacientes.Count;

            List<GRUPO_PAC> grupos = CarregaGrupo();
            Int32 grupNum = grupos.Count;

            // Encerra
            ViewBag.Mails = mails;
            ViewBag.Pacientes = pacNum;
            ViewBag.Grupos = grupNum;
            return View(usuario);
        }

        public JsonResult GetDadosPacienteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
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

        public JsonResult GetDadosPacienteDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPacienteData"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosPacienteMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPacienteMes"];
            List<String> meses = new List<String>();
            List<Int32> valor = new List<Int32>();
            meses.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                meses.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("meses", meses);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosPacienteCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
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

        public JsonResult GetDadosPacienteCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
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

        public JsonResult GetDadosPacienteUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosPacienteCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteCidade"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosPacienteCategoriaLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaPacienteCats"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult EditarPacienteMark(Int32 id)
        {
            Session["NivelPaciente"] = 1;
            Session["VoltaMsg"] = 55;
            Session["VoltaPaciente"] = 99;
            Session["IncluirPaciente"] = 0;
            Session["AbaPaciente"] = 1;
            return RedirectToAction("EditarPaciente", "Paciente", new { id = id });
        }

        public ActionResult EditarPacienteBase(Int32 id)
        {
            return RedirectToAction("EditarPaciente", "Paciente", new { id = id });
        }

        public ActionResult VerPacienteBase(Int32 id)
        {
            return RedirectToAction("VerPaciente", "Paciente", new { id = id });
        }

        public ActionResult EditarItemPrescricaoBase(Int32 id)
        {
            return RedirectToAction("EditarItemPrescricao", "Paciente", new { id = id });
        }

        public ActionResult EditarAnamneseBase(Int32 id)
        {
            return RedirectToAction("EditarAnamnese", "Paciente", new { id = id });
        }

        public ActionResult VerAnamneseBase(Int32 id)
        {
            return RedirectToAction("VerAnamnese", "Paciente", new { id = id });
        }

        public ActionResult EditarExameBase(Int32 id)
        {
            return RedirectToAction("EditarExame", "Paciente", new { id = id });
        }

        public ActionResult VerExameBase(Int32 id)
        {
            return RedirectToAction("VerExame", "Paciente", new { id = id });
        }

        public ActionResult EditarExameFisicoBase(Int32 id)
        {
            return RedirectToAction("EditarExameFisico", "Paciente", new { id = id });
        }

        public ActionResult VerExameFisicoBase(Int32 id)
        {
            return RedirectToAction("VerExameFisico", "Paciente", new { id = id });
        }

        public ActionResult EditarAtestadoBase(Int32 id)
        {
            Session["ModoConsulta"] = 1;
            return RedirectToAction("EditarAtestado", "Paciente", new { id = id });
        }

        public ActionResult VerAtestadoBase(Int32 id)
        {
            Session["ModoConsulta"] = 1;
            return RedirectToAction("VerAtestado", "Paciente", new { id = id });
        }

        public ActionResult EditarSolicitacaoBase(Int32 id)
        {
            return RedirectToAction("EditarSolicitacao", "Paciente", new { id = id });
        }

        public ActionResult VerSolicitacaoBase(Int32 id)
        {
            return RedirectToAction("VerSolicitacao", "Paciente", new { id = id });
        }

        public JsonResult GetPlanos(Int32 id)
        {
            PLANO forn = assiApp.GetPlanoBaseById(id);
            var hash = new Hashtable();
            hash.Add("nome", forn.PLAN_NM_NOME);
            hash.Add("periodicidade", forn.PLANO_PERIODICIDADE.PLPE_NM_NOME);
            hash.Add("valor", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PRECO.Value));
            hash.Add("promo", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PROMOCAO.Value));
            DateTime data = DateTime.Today.Date.AddDays(Convert.ToDouble(forn.PLANO_PERIODICIDADE.PLPE_NR_DIAS));
            hash.Add("data", data.ToShortDateString());
            hash.Add("duracao", forn.PLAN_IN_DURACAO);
            return Json(hash);
        }

        public async Task<ActionResult> TrataExcecao()
        {
            try
            {
                // Recupera Assinante e configuração
                CONFIGURACAO conf = null;
                Int32 idAss = 0;
                ASSINANTE assi = null;
                USUARIO usuario = null;
                if (Session["IdAssinante"] != null)
                {
                    idAss = (Int32)Session["IdAssinante"];
                    conf = confApp.GetItemById(idAss);
                    assi = assiApp.GetItemById(idAss);
                    usuario = (USUARIO)Session["UserCredentials"];
                }

                // 



                // Monta Exceção
                ExcecaoViewModel exc = new ExcecaoViewModel();
                Exception ex = (Exception)Session["Excecao"];
                exc.DataExcecao = DateTime.Now;
                exc.Gerador = (String)Session["VoltaExcecao"];
                exc.Message = ex.Message;
                exc.Source = ex.Source;
                exc.StackTrace = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exc.Inner = ex.InnerException.Message;
                }
                exc.tipoExcecao = (String)Session["ExcecaoTipo"];
                exc.tipoVolta = (Int32)Session["TipoVolta"];
                if (conf != null)
                {
                    exc.SuporteMail = conf.CONF_EM_CRMSYS;
                    exc.SuporteZap = conf.CONF_NR_SUPORTE_ZAP;
                }
                else
                {
                    exc.SuporteMail = "suporte@rtiltda.net";
                    exc.SuporteZap = "(21)97302-4096";
                }
                Session["ExcecaoView"] = exc;

                // Gera mensagem automática para suporte
                if (assi != null)
                {
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = assi.ASSI_NM_NOME;
                    mens.ID = idAss;
                    mens.MODELO = assi.ASSI_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.EXCECAO = exc;
                    Int32 volta = await ProcessaEnvioEMailSuporte(mens, usuario);
                }

                // Mensagem
                ModelState.AddModelError("", "O processamento do WebDoctor detectou uma falha. Uma mensagem urgente já foi enviada ao suporte com as informações abaixo e logo voce receberá a resposta. Se desejar reenvie a mensagem usando os botões disponíveis nesta página." + " ID do envio: " + (String)Session["IdMail"]);
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7.pdf";
                return View(exc);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Exceção";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<Int32> TrataExcecaoCompleta(ErroViewModel erro)
        {
            try
            {
                // Recupera Assinante e configuração
                CONFIGURACAO conf = null;
                String mail1 = conf.CONF_EM_CRMSYS;
                String mail2 = conf.CONF_EM_CRMSYS1;

                Int32 idAss = 0;
                ASSINANTE assi = null;
                USUARIO usuario = null;
                if (Session["IdAssinante"] != null)
                {
                    idAss = (Int32)Session["IdAssinante"];
                    conf = confApp.GetItemById(idAss);
                    assi = assiApp.GetItemById(idAss);
                    usuario = (USUARIO)Session["UserCredentials"];
                }

                // Monta Exceção
                ExcecaoViewModel exc = new ExcecaoViewModel();
                Exception ex = (Exception)Session["Excecao"];
                exc.DataExcecao = DateTime.Now;
                exc.Gerador = (String)Session["VoltaExcecao"];
                exc.Message = ex.Message;
                exc.Source = ex.Source;
                exc.StackTrace = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exc.Inner = ex.InnerException.Message;
                }
                exc.tipoExcecao = (String)Session["ExcecaoTipo"];
                exc.tipoVolta = (Int32)Session["TipoVolta"];
                if (conf != null)
                {
                    exc.SuporteMail = conf.CONF_EM_CRMSYS;
                    exc.SuporteZap = conf.CONF_NR_SUPORTE_ZAP;
                }
                else
                {
                    exc.SuporteMail = "suporte@rtiltda.net";
                    exc.SuporteZap = "(21)97302-4096";
                }
                Session["ExcecaoView"] = exc;

                // Extrai a string Base64 (removendo o cabeçalho)
                string base64String = erro.ImagemBase64.Split(',')[1];
                byte[] imagemBytes = Convert.FromBase64String(base64String);

                // Cria um MemoryStream a partir dos bytes da imagem
                MemoryStream stream = new MemoryStream(imagemBytes);

                // Adiciona a imagem como um recurso embutido
                LinkedResource inlineImage = new LinkedResource(stream, new ContentType("image/png"));
                inlineImage.ContentId = "screenshot"; // ID que será referenciado no HTML

                // Prepara cabeçalho
                String cab = "Prezado <b>Suporte RTi</b>";

                // Prepara rodape
                String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

                // Prepara assinante
                String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
                String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

                // Prepara lista de destinos
                List<EmailAddress> emails = new List<EmailAddress>();
                EmailAddress add = new EmailAddress(address: mail1, displayName: "Suporte 1");
                emails.Add(add);
                EmailAddress add1 = new EmailAddress(address: mail2, displayName: "Suporte 2");
                emails.Add(add1);

                // Prepara corpo do e-mail
                String inner = String.Empty;
                String mens = String.Empty;
                String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu.<br />";
                String contato = "Para mais informações entre em contato pelo telefone <b>" + conf.CONF_NR_SUPORTE_ZAP + "</b> ou pelo e-mail <b>" + conf.CONF_EM_CRMSYS + ".</b><br /><br />";
                String final = "<br />Atenciosamente,<br /><br />";
                String aplicacao = "<b>Aplicação: </b> WebDoctor" + "<br />";
                String assinante = "<b>Assinante: </b>" + nome + "<br />";
                String data = "<b>Data: </b>" + DateTime.Today.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
                String modulo = "<b>Módulo: </b>" + exc.Gerador + "<br />";
                String origem = "<b>Origem: </b>" + exc.Source + "<br />";
                String tipo = "<b>Tipo da Exceção: </b>" + exc.tipoExcecao + "<br />";
                String message = "<b>Exceção: </b>" + exc.Message + "<br />";
                if (exc.Inner != null)
                {
                    inner = "<b>Exceção Interna: </b>" + exc.Inner + "<br /><br />";
                }
                String trace = "<b>Stack Trace: </b>" + exc.StackTrace + "<br />";
                String body = intro + contato + aplicacao + assinante + data + modulo + origem + tipo + message + inner + trace;

                body = body + mens + final;
                body = body.Replace("\r\n", "<br />");
                body += "< img src = 'cid:screenshot' alt = 'Screenshot do Erro' style = 'max-width: 100%; height: auto;' />";
                String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Solicitação de Suporte";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = conf.CONF_EM_CRMSYS;
                mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "WebDoctor";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
                String status = "Succeeded";
                String iD = "xyz";

                //// Adiciona a visualização alternativa (HTML) e o recurso
                //AlternateView altView = AlternateView.CreateAlternateViewFromString(emailBody, null, MediaTypeNames.Text.Html);
                //altView.LinkedResources.Add(inlineImage);
                //mail.AlternateViews.Add(altView);

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailListAsync(mensagem, null, emails);
                }
                catch (Exception ex1)
                {
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }




        [HttpGet]
        public ActionResult EnviarEMailSuporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                CONFIGURACAO conf = null;
                Int32 idAss = 0;
                ASSINANTE assi = null;
                if (Session["IdAssinante"] != null)
                {
                    idAss = (Int32)Session["IdAssinante"];
                    conf = confApp.GetItemById(idAss);
                    assi = assiApp.GetItemById(idAss);
                }
                Session["Assinante"] = assi;
                ViewBag.Configuracao = conf;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "SUPORTE_EMAIL", "BaseAdmin", "EnviarEMailSuporte");

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = assi.ASSI_NM_NOME;
                mens.ID = idAss;
                mens.MODELO = assi.ASSI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.EXCECAO = (ExcecaoViewModel)Session["ExcecaoView"];
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarEMailSuporte(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailSuporte(vm, usuarioLogado);

                    // Verifica retorno
                    // Sucesso
                    return RedirectToAction("TrataExcecao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Suporte";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailSuporte(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = 0;
            ASSINANTE assi = null;
            if (Session["IdAssinante"] != null)
            {
                idAss = (Int32)Session["IdAssinante"];
                assi = assiApp.GetItemById(idAss);
            }

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String mail1 = conf.CONF_EM_CRMSYS;
            String mail2 = conf.CONF_EM_CRMSYS1;

            // Prepara cabeçalho
            String cab = "Prezado <b>Suporte RTi</b>";

            // Prepara rodape
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara assinante
            String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
            String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

            // Prepara lista de destinos
            List<EmailAddress> emails = new List<EmailAddress>();
            EmailAddress add = new EmailAddress(address: mail1, displayName: "Suporte 1");
            emails.Add(add);
            EmailAddress add1 = new EmailAddress(address: mail2, displayName: "Suporte 2");
            emails.Add(add1);

            // Prepara corpo do e-mail
            String inner = String.Empty;
            String mens = String.Empty;
            String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu.<br />";
            String contato = "Para mais informações entre em contato pelo telefone <b>" + conf.CONF_NR_SUPORTE_ZAP + "</b> ou pelo e-mail <b>" + conf.CONF_EM_CRMSYS + ".</b><br /><br />";
            String final = "<br />Atenciosamente,<br /><br />";
            String aplicacao = "<b>Aplicação: </b> WebDoctor" + "<br />";
            String assinante = "<b>Assinante: </b>" + nome + "<br />";
            String data = "<b>Data: </b>" + DateTime.Today.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
            String modulo = "<b>Módulo: </b>" + vm.EXCECAO.Gerador + "<br />";
            String origem = "<b>Origem: </b>" + vm.EXCECAO.Source + "<br />";
            String tipo = "<b>Tipo da Exceção: </b>" + vm.EXCECAO.tipoExcecao + "<br />";
            String message = "<b>Exceção: </b>" + vm.EXCECAO.Message + "<br />";
            if (vm.EXCECAO.Inner != null)
            {
                inner = "<b>Exceção Interna: </b>" + vm.EXCECAO.Inner + "<br /><br />";
            }
            String trace = "<b>Stack Trace: </b>" + vm.EXCECAO.StackTrace + "<br />";
            String body = intro + contato + aplicacao + assinante + data + modulo + origem + tipo + message + inner + trace;

            if (vm.MENS_TX_TEXTO != null)
            {
                mens = vm.MENS_TX_TEXTO + "<br />";
            }
            body = body + mens + final;
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Solicitação de Suporte";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = conf.CONF_EM_CRMSYS;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "WebDoctor";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String status = "Succeeded";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String iD = "xyz";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailListAsync(mensagem, null, emails);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSSuporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                CONFIGURACAO conf = null;
                Int32 idAss = 0;
                ASSINANTE assi = null;
                if (Session["IdAssinante"] != null)
                {
                    idAss = (Int32)Session["IdAssinante"];
                    conf = confApp.GetItemById(idAss);
                    assi = assiApp.GetItemById(idAss);
                }
                Session["Assinante"] = assi;
                ViewBag.Configuracao = conf;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "SUPORTE_SMS", "BaseAdmin", "EnviarSMSSuporte");

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = assi.ASSI_NM_NOME;
                mens.ID = idAss;
                mens.MODELO = assi.ASSI_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.EXCECAO = (ExcecaoViewModel)Session["ExcecaoView"];
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EnviarSMSSuporte(MensagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSSuporte(vm, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("TrataExcecao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Suporte";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSSuporte(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String cel1 = conf.CONF_NR_SUPORTE_ZAP;
            String cel2 = conf.CONF_NR_SUPORTE_ZAP;

            // Prepara cabeçalho
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String cab = "Suporte RTi. ";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado

            // Prepara rodape
            String rod = ". " + assi.ASSI_NM_NOME;

            // Prepara assinante
            String doc = assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ;
            String nome = assi.ASSI_NM_NOME + (doc != null ? " - " + doc : String.Empty);

            // Decriptografa chaves
            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

            // Monta token
            String text = login + ":" + senha;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara corpo do SMS
            String mens = String.Empty;
            String intro = "Por favor verifiquem a exceção abaixo e as condições em que ela ocorreu. ";
            String contato = "Para mais informações entre em contato pelo telefone " + conf.CONF_NR_SUPORTE_ZAP + " ou pelo e-mail " + conf.CONF_EM_CRMSYS + ". ";
            String aviso = "Veja e-mail enviado para o suporte com maiores detalhes. ";

            String aplicacao = " Aplicação: WebDoctor. ";
            String assinante = "Assinante: " + nome + ". ";
            String data = "Data: " + DateTime.Today.Date.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ", ";
            String modulo = "Módulo: " + vm.EXCECAO.Gerador + ". ";
            String origem = "Origem: " + vm.EXCECAO.Source + ". ";
            String tipo = "Tipo da Exceção: " + vm.EXCECAO.tipoExcecao + ". ";
            String message = "Exceção: " + vm.EXCECAO.Message + ". ";
            if (vm.MENS_TX_TEXTO != null)
            {
                mens = vm.MENS_TX_TEXTO + ". ";
            }
            String body = intro + contato + aviso + aplicacao + assinante + data + modulo + origem + tipo + message + mens;
            String smsBody = body;
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                // Prepara envio
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String json = String.Empty;

                // Monta destinatarios
                String vetor = String.Empty;
                String listaDest = "55" + Regex.Replace(cel1, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                vetor = String.Concat("{\"to\": \"", listaDest, "\", \"text\": \"", smsBody, "\", \"customId\": \"" + customId + "\", \"from\": \"WebDoctor\"}");
                String listaDest1 = "55" + Regex.Replace(cel2, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                vetor += String.Concat(",{\"to\": \"", listaDest, "\", \"text\": \"", smsBody, "\", \"customId\": \"" + customId + "\", \"from\": \"WebDoctor\"}");

                // Envia mensagem
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [", vetor, "]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Suporte";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
            return 0;
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

        public List<USUARIO> CarregaUsuarioAdm()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["UsuariosAdm"] == null)
                {
                    conf = usuApp.GetAllUsuariosAdm(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllUsuariosAdm(idAss);
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                Session["UsuarioAlterada"] = 0;
                Session["UsuariosAdm"] = conf;
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

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = usuApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                conf = conf.Where(p => p.USUA_IN_SISTEMA == 6 || p.USUA_IN_SISTEMA == 0).ToList();
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

        public List<UF> CarregaUF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<UF> conf = new List<UF>();
                if (Session["UF"] == null)
                {
                    conf = empApp.GetAllUF();
                }
                else
                {
                    conf = (List<UF>)Session["UF"];
                }
                Session["UF"] = conf;
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

        public List<ASSINANTE> CarregaAssinante()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE> conf = new List<ASSINANTE>();
                if (Session["Assinantes"] == null)
                {
                    conf = assiApp.GetAllItens();
                }
                else
                {
                    if ((Int32)Session["AssinanteAlterada"] == 1)
                    {
                        conf = assiApp.GetAllItens();
                    }
                    else
                    {
                        conf = (List<ASSINANTE>)Session["Assinantes"];
                    }
                }
                conf = conf.Where(p => p.ASSI_CD_ID == idAss || p.ASSI_CD_ID == 83).ToList();
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

        public List<ASSINANTE> CarregaAssinanteRestrito()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ASSINANTE> conf = new List<ASSINANTE>();
                if (Session["Assinantes"] == null)
                {
                    conf = assiApp.GetAllItens();
                }
                else
                {
                    if ((Int32)Session["AssinanteAlterada"] == 1)
                    {
                        conf = assiApp.GetAllItens();
                    }
                    else
                    {
                        conf = (List<ASSINANTE>)Session["Assinantes"];
                    }
                }
                conf = conf.Where(p => p.ASSI_CD_ID == idAss || p.ASSI_CD_ID == 83).ToList();
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

        public List<PLANO> CarregaPlano()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PLANO> conf = new List<PLANO>();
                if (Session["PlanosCarga"] == null)
                {
                    conf = planApp.GetAllItens();
                }
                else
                {
                    if ((Int32)Session["PlanoAlterada"] == 1)
                    {
                        conf = planApp.GetAllItens();
                    }
                    else
                    {
                        conf = (List<PLANO>)Session["PlanosCarga"];
                    }
                }
                Session["PlanosCarga"] = conf;
                Session["PlanoAlterada"] = 0;
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

        public List<PACIENTE> CarregaPaciente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE> conf = new List<PACIENTE>();
                if (Session["Pacientes"] == null)
                {
                    conf = pacApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["PacienteAlterada"] == 1)
                    {
                        conf = pacApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<PACIENTE>)Session["Pacientes"];
                    }
                }
                Session["Pacientes"] = conf;
                Session["PacienteAlterada"] = 0;
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

        public List<TEMPLATE_EMAIL> CarregaTemplateEMail()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
                if (Session["TemplatesEMail"] == null)
                {
                    conf = mailApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["TemplatesEMailAlterada"] == 1)
                    {
                        conf = mailApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_EMAIL>)Session["TemplatesEMail"];
                    }
                }
                Session["TemplatesEMail"] = conf;
                Session["TemplatesEMailAlterada"] = 0;
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

        public List<TIPO_PESSOA> CarregaTipoPessoa()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_PESSOA> conf = new List<TIPO_PESSOA>();
                if (Session["TipoPessoa"] == null)
                {
                    conf = pacApp.GetAllTiposPessoa();
                }
                else
                {
                    conf = (List<TIPO_PESSOA>)Session["TipoPessoa"];
                }
                Session["TipoPessoa"] = conf;
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

        public List<SEXO> CarregaSexo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SEXO> conf = new List<SEXO>();
            if (Session["Sexos"] == null)
            {
                conf = pacApp.GetAllSexo();
            }
            else
            {
                conf = (List<SEXO>)Session["Sexos"];
            }
            Session["Sexos"] = conf;
            return conf;
        }

        public List<TIPO_PACIENTE> CarregaCatPaciente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_PACIENTE> conf = new List<TIPO_PACIENTE>();
                if (Session["CatPacientes"] == null)
                {
                    conf = pacApp.GetAllTipos(idAss);
                }
                else
                {
                    if ((Int32)Session["CatPacienteAlterada"] == 1)
                    {
                        conf = pacApp.GetAllTipos(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_PACIENTE>)Session["CatPacientes"];
                    }
                }
                Session["CatPacientes"] = conf;
                Session["CatPacienteAlterada"] = 0;
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

        public List<GRUPO_PAC> CarregaGrupo()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<GRUPO_PAC> conf = new List<GRUPO_PAC>();
                if (Session["Grupos"] == null)
                {
                    conf = gruApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["GrupoAlterada"] == 1)
                    {
                        conf = gruApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<GRUPO_PAC>)Session["Grupos"];
                    }
                }
                Session["Grupos"] = conf;
                Session["GrupoAlterada"] = 0;
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

        [HttpGet]
        public ActionResult MontarTelaMensagensEnviadas()
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
            Session["ModuloAtual"] = "Mensagens Enviadas";

            try
            {
                // Carrega listas
                if (Session["ListaMensagensEnviadas"] == null)
                {
                    listaMasterEnviada = CarregaMensagensEnviadasHoje();
                    Session["ListaMensagensEnviadas"] = listaMasterEnviada;
                }

                // Monta demais listas
                List<USUARIO> listaUsu = CarregaUsuario();
                ViewBag.Usuarios = new SelectList(listaUsu, "USUA_CD_ID", "USUA_NM_NOME");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "SMS", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                ViewBag.TipoCarga = (Int32)Session["TipoCargaMsgInt"];
                Session["VoltaTela"] = 0;

                // Restringe pelo perfil
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                List<MENSAGENS_ENVIADAS_SISTEMA> listaMens = ((List<MENSAGENS_ENVIADAS_SISTEMA>)Session["ListaMensagensEnviadas"]).ToList();
                if ((String)Session["PerfilSigla"] != "ADM")
                {
                    listaMens = listaMens.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.Listas = listaMens;

                // Indicadores
                ViewBag.Mensagens = listaMens.Count;
                ViewBag.EMails = listaMens.Where(p => p.MEEN_IN_TIPO == 1).Count();
                ViewBag.SMS = listaMens.Where(p => p.MEEN_IN_TIPO == 2).Count();
                ViewBag.Entregue = listaMens.Where(p => p.MEEN_IN_ENTREGUE == 1).Count();
                ViewBag.Falha = listaMens.Where(p => p.MEEN_IN_ENTREGUE == 0).Count();

                // Mensagem
                if (Session["MensEnviada"] != null)
                {
                    if ((Int32)Session["MensEnviada"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEnviada"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEnviada"] == 3)
                    {
                        String mens = CRMSys_Base.ResourceManager.GetString("M0279", CultureInfo.CurrentCulture) + " - ID: " + (String)Session["GUID"];
                        ModelState.AddModelError("", mens);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENS_ENVIADA", "BaseAdmin", "MontarTelaMensagensEnviadas");

                // Abre view
                Session["MensEnviada"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10.pdf";
                objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
                if (Session["FiltroMensagensEnviadas"] != null)
                {
                    objetoEnviada = (MENSAGENS_ENVIADAS_SISTEMA)Session["FiltroMensagensEnviadas"];
                }
                objetoEnviada.MEEN_IN_ATIVO = 1;
                objetoEnviada.MEEN_IN_ESCOPO = null;
                objetoEnviada.USUA_CD_ID = usuario.USUA_CD_ID;
                return View(objetoEnviada);
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


        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadas()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                if (Session["MensagensEnviadas"] == null)
                {
                    conf = envApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["MensagemEnviadaAlterada"] == 1)
                    {
                        conf = envApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["MensagensEnviadas"];
                    }
                }
                Session["MensagemEnviadaAlterada"] = 0;
                Session["MensagensEnviadas"] = conf;
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

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadasHoje()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                DateTime data = DateTime.Today.Date;
                conf = envApp.GetByDate(data, idAss);
                Session["MensagemEnviadaAlterada"] = 0;
                Session["MensagensEnviadas"] = conf;
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

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagensEnviadasMes()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                DateTime data = DateTime.Today.Date;
                conf = envApp.GetByMonth(data, idAss);
                Session["MensagemEnviadaAlterada"] = 0;
                Session["MensagensEnviadas"] = conf;
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

        public ActionResult RetirarFiltroMensagensEnviadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["MensagemEnviadaAlterada"] = 1;
                List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadasHoje();
                if ((String)Session["PerfilSigla"] != "ADM")
                {
                    lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["MostraTodasMensagens"] = 0;
                Session["ListaMensagensEnviadas"] = lm;
                Session["FiltroMensagensEnviadas"] = null;
                Session["TipoCargaMsgInt"] = 1;
                Session["EscopoMensagem"] = 0;
                return RedirectToAction("MontarTelaMensagensEnviadas");
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

        public ActionResult ExibirMesCorrente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["MensagemEnviadaAlterada"] = 1;
                List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadasMes();
                if ((String)Session["PerfilSigla"] != "ADM")
                {
                    lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["MostraTodasMensagens"] = 0;
                Session["ListaMensagensEnviadas"] = lm;
                Session["FiltroMensagensEnviadas"] = null;
                Session["TipoCargaMsgInt"] = 1;
                Session["EscopoMensagem"] = 0;
                return RedirectToAction("MontarTelaMensagensEnviadas");
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

        public ActionResult MostrarTodasMensagensEnviadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["MensagemEnviadaAlterada"] = 1;
                List<MENSAGENS_ENVIADAS_SISTEMA> lm = CarregaMensagensEnviadas();
                if ((String)Session["PerfilSigla"] == "ADM" || (String)Session["PerfilSigla"] == "GER")
                {
                    lm = lm.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["MostraTodasMensagens"] = 1;
                Session["ListaMensagensEnviadas"] = lm;
                Session["FiltroMensagensEnviadas"] = null;
                Session["TipoCargaMsgInt"] = 2;
                Session["EscopoMensagem"] = 1;
                return RedirectToAction("MontarTelaMensagensEnviadas");
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

        [HttpPost]
        public ActionResult FiltrarMensagensEnviadas(MENSAGENS_ENVIADAS_SISTEMA item)
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
                List<MENSAGENS_ENVIADAS_SISTEMA> listaObj = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                Session["FiltroMensagensEnviadas"] = item;
                Tuple<Int32, List<MENSAGENS_ENVIADAS_SISTEMA>, Boolean> volta = envApp.ExecuteFilterTuple(item.MEEN_IN_ESCOPO, item.MEEN_IN_TIPO, item.MEEN_DT_DATA_ENVIO, item.MEEN_DT_DUMMY, item.MEEN_EM_EMAIL_DESTINO, item.MEEN_NR_CELULAR_DESTINO, item.MEEN_NM_ORIGEM, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensEnviada"] = 1;
                    return RedirectToAction("MontarTelaMensagensEnviadas");
                }

                // Sucesso
                listaMasterEnviada = volta.Item2;
                if ((String)Session["PerfilSigla"] != "ADM")
                {
                    listaMasterEnviada = listaMasterEnviada.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagensEnviadas"] = listaMasterEnviada;
                Session["TipoCargaMsgInt"] = 2;
                return RedirectToAction("MontarTelaMensagensEnviadas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMensagemEnviada(Int32 id)
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
            Session["ModuloAtual"] = "Mensagens Enviadas - Visualização";

            try
            {
                // Recupera mensagem
                MENSAGENS_ENVIADAS_SISTEMA item = envApp.GetItemById(id);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENS_ENVIADA_VER", "BaseAdmin", "VerMensagemEnviada");

                // Prepara view
                MensagemEmitidaViewModel vm = Mapper.Map<MENSAGENS_ENVIADAS_SISTEMA, MensagemEmitidaViewModel>(item);
                vm.MEEN_TX_CORPO_EXIBE = CrossCutting.UtilitariosGeral.CleanStringTextoHTML(vm.MEEN_TX_CORPO);
                Session["MensagemEnviada"] = item;
                Session["IdVolta"] = id;
                Session["IdFilial"] = id;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens Enviadas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 flag = (Int32)Session["FlagMensagensEnviadas"];


            if ((Int32)Session["FlagMensagensEnviadas"] == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 12)
            {
                return RedirectToAction("MontarTelaDashboardMensagem", "Mensagem");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 11)
            {
                return RedirectToAction("MontarTelaPaciente", "Paciente");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 4)
            {
                return RedirectToAction("MontarTelaUsuario", "Usuario");
            }
            if ((Int32)Session["FlagMensagensEnviadas"] == 50)
            {
                return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public List<EMPRESA> CarregaEmpresa()
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

        public JsonResult GetDadosGraficoEmail()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaEMail"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasEMail"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMS()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMS"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMS"];
            List<MENSAGENS_DESTINOS> listaDia = new List<MENSAGENS_DESTINOS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                Int32 contaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_IN_STATUS == 2).Sum(x => x.MENS_IN_DESTINOS.Value);
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoSMSTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaSMSTudo"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasSMSTudo"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaFaleConosco()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Configuração
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

                // Mensagem
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0256", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 888)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Monta objeto
                Session["MensFC"] = 0;
                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = conf.CONF_NR_SUPORTE_ZAP;
                fale.EMail = conf.CONF_EM_CRMSYS;
                fale.Resposta = usuario.USUA_NM_EMAIL;
                fale.Nome = usuario.USUA_NM_NOME;

                List<SelectListItem> assunto = new List<SelectListItem>();
                assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
                assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
                assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
                assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
                assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
                ViewBag.Assunto = new SelectList(assunto, "Value", "Text");
                fale.Assunto = null;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "FALE_CONOSCO", "BaseAdmin", "MontarTelaFaleConosco");

                return View(fale);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Comunicacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Comunicacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaFaleConosco(FaleConoscoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<SelectListItem> assunto = new List<SelectListItem>();
            assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
            assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
            assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
            assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
            assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
            ViewBag.Assunto = new SelectList(assunto, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    if (vm.Mensagem != null)
                    {
                        // Valida informações
                        if (vm.Assunto == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0600", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Decodifica assunto
                        String assuntoDesc = vm.Assunto == 1 ? "Sugestões" : (vm.Assunto == 2 ? "Informações" : (vm.Assunto == 3 ? "Reclamações" : (vm.Assunto == 4 ? "Suporte Técnico" : "Outros Assuntos")));

                        // Prepara mensagem
                        MensagemViewModel mens = new MensagemViewModel();
                        mens.NOME = vm.Nome;
                        mens.ID = usuario.USUA_CD_ID;
                        mens.MODELO = vm.EMail;
                        mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                        mens.MENS_IN_TIPO = 1;
                        mens.MENS_TX_TEXTO = vm.Mensagem;
                        mens.MENS_NM_LINK = null;
                        mens.MENS_NM_NOME = "Solicitação Fale Conosco";
                        mens.MENS_NM_CAMPANHA = assuntoDesc;
                        await ProcessaEnvioEMailFaleConosco(mens, usuario);
                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaFaleConosco");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Comunicacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Comunicacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailFaleConosco(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Prepara corpo do e-mail e trata link
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

            // Monta mensagem
            ASSINANTE assi = assiApp.GetItemById(idAss);
            corpo = corpo + "<b style='color:darkblue'>Assinante:</b> " + assi.ASSI_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Num.Assinante:</b> " + assi.ASSI_CD_ID.ToString() + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Usuário:</b> " + usuario.USUA_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>CPF/CNPJ:</b> " + (assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ) + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Data Assinatura:</b> " + assi.ASSI_DT_INICIO.Value.ToShortDateString() + "<br />";

#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String status = "Succeeded";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String iD = "xyz";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MODELO;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            // Mensagem deenvio
            Session["MsgCRUD"] = "E-Mail de " + usuario.USUA_NM_NOME + " foi enviado com sucesso.";
            Session["MensFC"] = 888;
            return 0;
        }

        [HttpGet]
        public ActionResult MontarTelaSobre()
        {
            // Verifica se tem usuario logado
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Configuração
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_2.pdf";

            // Recupera versoes
            List<CONTROLE_VERSAO> versoes = pacApp.GetAllVersoes().OrderByDescending(p => p.COVE_DT_BUILD).ToList();
            CONTROLE_VERSAO ultima = versoes.Where(p => p.COVE_IN_STATUS == 1).OrderByDescending(p => p.COVE_DT_BUILD).FirstOrDefault();

            // Monta timeline
            List<ModeloViewModel> mods = new List<ModeloViewModel>();
            foreach (CONTROLE_VERSAO item in versoes)
            {
                ModeloViewModel mod = new ModeloViewModel();
                mod.DataEmissao = item.COVE_DT_BUILD.Value;
                mod.Nome = item.COVE_NR_VERSAO;
                mod.Valor = item.COVE_IN_STATUS.Value;
                mod.Valor1 = item.COVE_CD_ID;
                mod.Nome1 = item.COVE_DS_BUILD;
                mod.Nome3 = item.COVE_NM_ARQUIVO;
                mod.Nome4 = item.COVE_AQ_BUILD;
                mods.Add(mod);
            }
            ViewBag.Time = mods;

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "SOBRE", "BaseAdmin", "MontarTelaSobre");
            return View(ultima);

        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessarEnvioMensagemSuporte(ContatoSuporteViewModel vm)
        {
            // Inicializa
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            Int32 volta = 0;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            Int32 totMens = 0;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            CRMSysDBEntities Db = new CRMSysDBEntities();
            String guid = (String)Session["GuidAnexo"];
            Session["IdAssinante"] = 1;

            // Recupera configuracao
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Recupera modelo
            TEMPLATE temp = temApp.GetByCode("SUPORTE");
            String cabecalho = temp.TEMP_TX_CABECALHO;
            String corpo = temp.TEMP_TX_CORPO;
            String rodape = temp.TEMP_TX_DADOS;

            // Prepara texto
            corpo = corpo.Replace("{texto}", vm.COSU_TX_MENSAGEM);
            rodape = rodape.Replace("{nome}", vm.ASSI_NM_NOME);
            rodape = rodape.Replace("{data}", vm.COSU_DT_CONTATO.Value.ToShortDateString());
            rodape = rodape.Replace("{tipo}", vm.COSU_IN_TIPO == 1 ? "Dúvidas" : (vm.COSU_IN_TIPO == 2 ? "Reclamações" : "Solicitações"));
            rodape = rodape.Replace("{prior}", vm.COSU_IN_PRIORIDADE == 1 ? "Alta" : (vm.COSU_IN_PRIORIDADE == 2 ? "Média" : "Baixa"));
            rodape = rodape.Replace("{resp}", vm.COSU_IN_RESPOSTA == 1 ? "Telefone" : (vm.COSU_IN_RESPOSTA == 2 ? "Celular" : "E-Mail"));
            rodape = rodape.Replace("{hora}", vm.COSU_IN_HORARIO == 1 ? "Manhã" : (vm.COSU_IN_HORARIO == 2 ? "Tarde" : "Comercial"));
            String frase = String.Empty;
            if (vm.COSU_IN_RESPOSTA == 1)
            {
                frase = "Telefone: <b><font color=\"#ff0000\">" + vm.COSU_NR_TELEFONE + "</font></b>";
            }
            if (vm.COSU_IN_RESPOSTA == 2)
            {
                frase = "Celular: <b><font color=\"#ff0000\">" + vm.COSU_NR_CELULAR + "</font></b>";
            }
            if (vm.COSU_IN_RESPOSTA == 3)
            {
                frase = "E-Mail: <b><font color=\"#ff0000\">" + vm.COSU_EM_MAIL + "</font></b>";
            }
            rodape += "<br />" + frase;
            String body = cabecalho + "<br />" + corpo + "<br />" + rodape;

            // Formata texto
            body = body.Replace("\r\n", "<br />");
            body = body.Replace("<p>", "");
            body = body.Replace("</p>", "<br />");

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Solicitação de Informações - " + vm.ASSI_NM_NOME;
            mensagem.CORPO = body;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = conf.CONF_EM_CONTATO;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conn;

            // Envia mensagem
#pragma warning disable CS0168 // A variável foi declarada, mas nunca foi usada
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem);
                String gu = Xid.NewXid().ToString();
                Session["SuporteGUID"] = gu;
            }
            catch (Exception ex)
            {
                return 0;
            }
#pragma warning restore CS0168 // A variável foi declarada, mas nunca foi usada
            return 0;
        }


        public Int32 CarregaNumeroMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Recupera listas Mensagens
            List<MENSAGENS> lt = CarregarMensagem();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                lt = lt.Where(p => p.EMFI_CD_ID == usuario.EMFI_CD_ID).ToList();
            }

            // Recupera listas Mensagens - Email
            List<MENSAGENS> emails = lt.Where(p => p.MENS_IN_TIPO == 1).ToList();
            List<MENSAGENS> listaMesEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> listaDiaEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();

            List<MENSAGENS> emailsNormal = emails.Where(p => p.MENS_DT_AGENDAMENTO == null & p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsAguarda = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_ENVIADAS != p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsAgenda = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_STATUS != 4).ToList();
            List<MENSAGENS> emailsRecursiva = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_STATUS == 4).ToList();
            List<MENSAGENS> emailsEnviado = emails.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsFalha = emails.Where(p => p.MENS_IN_STATUS == 3).ToList();

            List<MENSAGENS> emailsEnviadoDia = listaDiaEMail.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsEnviadoMes = listaMesEMail.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();

            Int32 emailTot = emails.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 emailTotMes = listaMesEMail.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 emailTotDia = listaDiaEMail.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;

            Int32 aguarda = emailsAguarda.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 agenda = emailsAgenda.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 recursiva = emailsRecursiva.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviado = emailsEnviado.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 normal = emailsFalha.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 falha = emailsFalha.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviadoDia = emailsEnviadoDia.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviadoMes = emailsEnviadoMes.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;

            Int32? destinos = emails.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;

            // Viewbags
            ViewBag.Total = lt.Count;
            ViewBag.TotalEMails = emails.Count;

            ViewBag.EMailsAguarda = aguarda;
            ViewBag.EMailsEnviado = enviado;
            ViewBag.EMailsFalha = falha;
            ViewBag.EMailsMes = emailTotMes;
            ViewBag.EMailsDia = emailTotDia;
            ViewBag.EMailsAgenda = agenda;

            ViewBag.EMailsTotalEnvio = emailTot;
            ViewBag.EMailsTotalEnvioMes = enviadoMes;
            ViewBag.EMailsTotalEnvioDia = enviadoDia;

            ViewBag.EMailsTotalCriado = emails.Count;
            ViewBag.EMailsTotalCriadoMes = listaMesEMail.Count;
            ViewBag.EMailsTotalCriadoDia = listaDiaEMail.Count;

            ViewBag.Destinos = destinos;

            Session["EMailTotalEnvio"] = emailTot;
            Session["EMailTotalEnvioMes"] = emailTotMes;
            Session["EMailTotalEnvioDia"] = emailTotDia;
            Session["EMailAguarda"] = aguarda;
            Session["EMailAgenda"] = agenda;
            Session["EMailRecursiva"] = recursiva;
            Session["EMailFalha"] = falha;
            Session["EMailEnviado"] = enviado;
            Session["EMailnormal"] = enviado;
            Session["Destinos"] = destinos;

            Session["ListaMesEMail"] = listaMesEMail;
            Session["ListaEMailTudo"] = emails;
            Session["ListaDiaMail"] = listaDiaEMail;

            Session["ListaEMailAgenda"] = emailsAgenda;
            Session["ListaEMailAguarda"] = emailsAguarda;
            Session["ListaEMailRecursiva"] = emailsRecursiva;
            Session["ListaEMailEnviado"] = emailsEnviado;
            Session["ListaEMailFalha"] = emailsFalha;
            return 0;
        }

        public ActionResult ExibirAjuda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Recupera nível
            Int32 nivel = (Int32)Session["LocalAjuda"];
            if (nivel == 0)
            {
                return RedirectToAction("MontarTelaPaciente", "Paciente");
            }

            // Monta caminho e nome do arquivo
            try
            {
                String arquivo = "Ajuda" + nivel.ToString() + ".htm";
                String caminho = "/Ajuda/" + nivel.ToString() + "/";
                String path = Path.Combine(Server.MapPath(caminho), arquivo);

                // Le arquivo
                string html = System.IO.File.ReadAllText(path);
                ViewBag.Texto = html;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome1 = html;
                return View(mod);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Ajuda";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Ajuda", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAjuda()
        {
            Int32 nivel = (Int32)Session["LocalAjuda"];
            if (nivel == 1)
            {
                return RedirectToAction("MontarTelaPaciente", "Paciente");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        public ActionResult MontarTelaPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        [HttpGet]
        public ActionResult MontarTelaControleAcesso()
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
                if (usuario.PERFIL.PERF_IN_ACESSO_ADMIN == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Carrega configuração
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                ViewBag.Linhas = conf.CONF_NR_GRID_PRODUTO.Value;

                // Carrega totais
                List<USUARIO_LOGIN> listaAcesso = usuApp.GetAllLogin(idAss);
                List<USUARIO_LOGIN> listaAcessoDia = listaAcesso.Where(p => p.USLO_DT_LOGIN.Date == DateTime.Today.Date).ToList();
                List<USUARIO_LOGIN> listaAcessoMes = listaAcesso.Where(p => p.USLO_DT_LOGIN.Date.Month == DateTime.Today.Date.Month & p.USLO_DT_LOGIN.Date.Year == DateTime.Today.Date.Year).ToList();
                TimeSpan totalDia = new TimeSpan();
                foreach (USUARIO_LOGIN item in listaAcessoDia)
                {
                    totalDia += item.USLO_TM_DURACAO_SPAN.Value;
                }
                TimeSpan totalMes = new TimeSpan();
                foreach (USUARIO_LOGIN item in listaAcessoMes)
                {
                    totalMes += item.USLO_TM_DURACAO_SPAN.Value;
                }
                TimeSpan totalGeral = new TimeSpan();
                foreach (USUARIO_LOGIN item in listaAcesso)
                {
                    totalGeral += item.USLO_TM_DURACAO_SPAN.Value;
                }

                double totalDiaHoras = Math.Floor(totalDia.TotalHours);
                double totalDiaMinutos = totalDia.Minutes;
                String fraseDia = String.Empty;
                if (totalDiaHoras > 0)
                {
                    fraseDia += totalDiaHoras.ToString() + " hora(s) e ";
                }
                fraseDia += totalDiaMinutos.ToString() + " minuto(s)";
                ViewBag.TotalDia = fraseDia;

                double totalMesHoras = Math.Floor(totalMes.TotalHours);
                double totalMesMinutos = totalMes.Minutes;
                String fraseMes = String.Empty;
                if (totalMesHoras > 0)
                {
                    fraseMes += totalMesHoras.ToString() + " hora(s) e ";
                }
                fraseMes += totalMesMinutos.ToString() + " minuto(s)";
                ViewBag.TotalMes = fraseMes;

                double totalHoras = Math.Floor(totalGeral.TotalHours);
                double totalMinutos = totalGeral.Minutes;
                String fraseAno = String.Empty;
                if (totalHoras > 0)
                {
                    fraseAno += totalHoras.ToString() + " hora(s) e ";
                }
                fraseAno += totalMinutos.ToString() + " minuto(s)";
                ViewBag.TotalGeral = fraseAno;

                // Carrega Acessos
                if (Session["ListaAcessos"] == null)
                {
                    listaAcesso = listaAcesso.Where(p => p.USLO_IN_SISTEMA == 6 & p.USLO_DT_LOGOUT != null & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderBy(p => p.USLO_CD_ID).ToList();
                    Session["ListaAcessos"] = listaAcesso;
                }
                ViewBag.Listas = (List<USUARIO_LOGIN>)Session["ListaAcessos"];

                // Carrega Listas
                ViewBag.Usuarios = new SelectList(CarregaUsuario().OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensAcesso"] != null)
                {
                    if ((Int32)Session["MensAcesso"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensAcesso"] = 0;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTROLE_ACESSO", "BaseAdmin", "MontarTelaControleAcesso");

                // Abre view
                USUARIO_LOGIN login = new USUARIO_LOGIN();
                login.USLO_IN_ATIVO = 1;
                login.USLO_IN_SISTEMA = 6;
                login.USLO_DT_LOGIN = DateTime.Today.Date;
                //login.USLO_DT_DUMMY = DateTime.Today.Date;
                Session["MensAcesso"] = 0;
                return View(login);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroAcesso()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaAcessos"] = null;
                return RedirectToAction("MontarTelaControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        public ActionResult FiltrarAcesso(USUARIO_LOGIN item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            try
            {
                // Executa a operação
                CRMSysDBEntities Db = new CRMSysDBEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<USUARIO_LOGIN> query = Db.USUARIO_LOGIN;
                List<USUARIO_LOGIN> lista = new List<USUARIO_LOGIN>();

                // Monta condição
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.USLO_DT_LOGIN != DateTime.MinValue & item.USLO_DT_DUMMY == DateTime.MinValue)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.USLO_DT_LOGIN) >= DbFunctions.TruncateTime(item.USLO_DT_LOGIN));
                }
                if (item.USLO_DT_LOGIN == DateTime.MinValue & item.USLO_DT_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.USLO_DT_LOGIN) <= DbFunctions.TruncateTime(item.USLO_DT_DUMMY));
                }
                if (item.USLO_DT_LOGIN != DateTime.MinValue & item.USLO_DT_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.USLO_DT_LOGIN) >= DbFunctions.TruncateTime(item.USLO_DT_LOGIN) & DbFunctions.TruncateTime(p.USLO_DT_LOGIN) <= DbFunctions.TruncateTime(item.USLO_DT_DUMMY));
                }

                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.USLO_IN_ATIVO == 1);
                    query = query.Where(p => p.USLO_IN_SISTEMA == 6);
                    query = query.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID);
                    query = query.Where(p => p.USLO_DT_LOGOUT != null);
                    query = query.OrderBy(a => a.USLO_DT_LOGIN);
                    lista = query.ToList<USUARIO_LOGIN>();
                }

                // Sucesso
                Session["ListaAcessos"] = lista;
                return RedirectToAction("MontarTelaControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MontarTelaControleAcesso1()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<USUARIO_LOGIN> listaAcesso = usuApp.GetAllLogin(idAss);
            listaAcesso = listaAcesso.Where(p => p.USLO_DT_LOGIN.Date == DateTime.Today.Date).ToList();
            listaAcesso = listaAcesso.Where(p => p.USLO_IN_SISTEMA == 6 & p.USLO_DT_LOGOUT != null & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderBy(p => p.USLO_DT_LOGIN).ToList();
            Session["ListaAcessos"] = listaAcesso;

            return RedirectToAction("MontarTelaControleAcesso", "BaseAdmin");
        }

        public ActionResult MontarTelaControleAcesso2()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<USUARIO_LOGIN> listaAcesso = usuApp.GetAllLogin(idAss);
            listaAcesso = listaAcesso.Where(p => p.USLO_DT_LOGIN.Month == DateTime.Today.Date.Month & p.USLO_DT_LOGIN.Year == DateTime.Today.Date.Year).ToList();
            listaAcesso = listaAcesso.Where(p => p.USLO_IN_SISTEMA == 6 & p.USLO_DT_LOGOUT != null & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderBy(p => p.USLO_DT_LOGIN).ToList();
            Session["ListaAcessos"] = listaAcesso;

            return RedirectToAction("MontarTelaControleAcesso", "BaseAdmin");
        }

        public ActionResult BloquearUsuario()
        {
            Session["VoltaUsuario"] = 55;
            return RedirectToAction("BloquearUsuario", "Usuario");
        }

        public ActionResult DesbloquearUsuario()
        {
            Session["VoltaUsuario"] = 55;
            return RedirectToAction("DesbloquearUsuario", "Usuario");
        }

        public ActionResult ChamaCentralAssinante()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCentralAssinante", "Assinante", new { id = idAss });
        }


        public FileResult DownloadPower()
        {
            try
            {
                String arquivo = "../BaseAdmin/Apresentacao/WebDoctor_Apresentacao.ppsx";
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = arquivo.Substring(pos);
                String contentType = string.Empty;
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Session["NivelPaciente"] = 1;
                return File(arquivo, contentType, nomeDownload);
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

        [HttpGet]
        public ActionResult MontarTelaFaleConoscoInicio()
        {
            try
            {
                // Mensagem
                CONFIGURACAO_CHAVES conf = confApp.GetAllChaves().FirstOrDefault();
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0590", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0591", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 102)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0592", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 95)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0625", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 888)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Recupera assinante
                ASSINANTE assi = null;
                if ((Int32)Session["AssinantePendente"] == 1)
                {
                    assi = (ASSINANTE)Session["Assinante"];
                }

                // Monta objeto
                Session["MensFC"] = 0;
                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = conf.CONF_NR_SUPORTE_ZAP;
                fale.EMail = conf.CONF_EM_CRMSYS;
                fale.Resposta = String.Empty;
                fale.Nome = String.Empty;

                List<SelectListItem> assunto = new List<SelectListItem>();
                assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
                assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
                assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
                assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
                assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
                ViewBag.Assunto = new SelectList(assunto, "Value", "Text");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Telefone", Value = "2" });
                tipo.Add(new SelectListItem() { Text = "Celular", Value = "3" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                fale.Assunto = null;
                if ((Int32)Session["AssinantePendente"] == 1)
                {
                    fale.Nome = assi.ASSI_NM_NOME;
                    fale.Resposta = assi.ASSI_NM_EMAIL;
                    fale.TelefoneFixo = assi.ASSI_NR_TELEFONE;
                    fale.Celular = assi.ASSI_NR_CELULAR;
                }
                return View(fale);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Comunicacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Comunicacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaFaleConoscoInicio(FaleConoscoViewModel vm)
        {
            List<SelectListItem> assunto = new List<SelectListItem>();
            assunto.Add(new SelectListItem() { Text = "Sugestões", Value = "1" });
            assunto.Add(new SelectListItem() { Text = "Informações", Value = "2" });
            assunto.Add(new SelectListItem() { Text = "Reclamações", Value = "3" });
            assunto.Add(new SelectListItem() { Text = "Suporte Técnico", Value = "4" });
            assunto.Add(new SelectListItem() { Text = "Outros Assuntos", Value = "5" });
            ViewBag.Assunto = new SelectList(assunto, "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "E-Mail", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Telefone", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Celular", Value = "3" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    if (vm.Mensagem != null)
                    {
                        // Valida informações
                        if (vm.Nome == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0639", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Assunto == null || vm.Assunto == 0)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0642", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Resposta == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0640", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Tipo == 2)
                        {
                            if (vm.TelefoneFixo == null)
                            {
                                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0641", CultureInfo.CurrentCulture));
                                return View(vm);
                            }
                        }
                        if (vm.Tipo == 3)
                        {
                            if (vm.Celular == null)
                            {
                                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0613", CultureInfo.CurrentCulture));
                                return View(vm);
                            }
                        }

                        // Decodifica assunto
                        String assuntoDesc = vm.Assunto == 1 ? "Sugestões" : (vm.Assunto == 2 ? "Informações" : (vm.Assunto == 3 ? "Reclamações" : (vm.Assunto == 4 ? "Suporte Técnico" : "Outros Assuntos")));
                        vm.NomePlano = assuntoDesc;

                        // Prepara mensagem
                        MensagemViewModel mens = new MensagemViewModel();
                        mens.NOME = vm.Nome;
                        mens.ID = null;
                        mens.MODELO = vm.Resposta;
                        mens.MENS_NM_LINK = vm.EMail;
                        mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                        mens.MENS_IN_TIPO = 1;
                        mens.MENS_TX_TEXTO = vm.Mensagem;
                        mens.MENS_NM_NOME = "Solicitação de Contato";
                        mens.MENS_NM_CAMPANHA = assuntoDesc;
                        await ProcessaEnvioEMailFaleConoscoInicio(mens, vm);
                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaFaleConoscoInicio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Comunicacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Comunicacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDemo()
        {
            try
            {
                // Monta objeto
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");

                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 333)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0638", CultureInfo.CurrentCulture);
                        TempData["MensagemAcerto"] = frase;
                        TempData["TemMensagem"] = 1;
                    }
                }

                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = "(21)97302-4096";
                fale.EMail = "suporte@rtiltda.net";
                fale.Resposta = String.Empty;
                fale.Nome = String.Empty;
                fale.Assunto = null;
                fale.Comeco = DateTime.Today.Date.ToLongDateString();
                fale.Fim = DateTime.Today.Date.AddYears(1).ToLongDateString();
                return View(fale);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaDemo(FaleConoscoViewModel vm)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if (vm.Tipo == null || vm.Tipo == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0611", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Nome == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0592", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Resposta == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0591", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Tipo == 1)
                    {
                        if (vm.CPF == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0608", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.Tipo == 2)
                    {
                        if (vm.CNPJ == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0609", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Razao == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0612", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.CEPBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0614", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Endereco == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0615", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Numero == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0616", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Bairro == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0617", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Cidade == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0618", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.UF == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0619", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!ValidarItensDiversos.IsValidEmail(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica se assinante já tem demo
                    List<ASSINANTE> assList = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1).ToList();
                    ASSINANTE assBase = null;
                    Int32 novoPlano = 0;
                    if (vm.Tipo == 1)
                    {
                        assBase = assList.Where(p => p.ASSI_NR_CPF == vm.CPF & p.ASSI_IN_TIPO == 2).FirstOrDefault();
                        if (assBase != null)
                        {
                            novoPlano = 1;
                        }
                    }
                    else
                    {
                        assBase = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ & p.ASSI_IN_TIPO == 2).FirstOrDefault();
                        if (assBase != null)
                        {
                            novoPlano = 1;
                        }
                    }
                    if (novoPlano == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0637", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de login e Senha
                    if (vm.LoginBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0620", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0629", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0630", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma != vm.SenhaBase)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0631", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Length < 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0223", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(char.IsUpper) || !vm.SenhaBase.Any(char.IsLower) && !vm.SenhaBase.Any(char.IsDigit))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0224", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(p => !char.IsLetterOrDigit(p)))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Contains(vm.LoginBase) || vm.SenhaBase.Contains(vm.Nome) || vm.SenhaBase.Contains(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // verifica existencia login
                    if (usuApp.GetByLogin(vm.LoginBase, 1) != null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0633", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria assinante demo
                    vm.TipoAssinatura = 2;
                    vm.Plano = 24;
                    Int32 voltaCria = CriarAssinanteNormal(vm);
                    vm.LoginFinal = (String)Session["LoginDemo"];
                    vm.Senha = (String)Session["SenhaDemo"];
                    if (voltaCria == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0622", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Prepara mensagem
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = vm.Nome;
                    mens.ID = null;
                    mens.MODELO = vm.Resposta;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = String.Empty;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = vm.Nome;
                    mens.MENS_NM_CAMPANHA = "Solicitação de Assinatura Demonstração";
                    mens.MENS_NM_RODAPE = vm.Resposta;
                    mens.MENS_NM_LINK = "https://webdoctorpro.net/";
                    mens.CELULAR = vm.Telefone;
                    mens.MENS_NM_CABECALHO = vm.CPF;
                    mens.MENS_NM_ASSINATURA = vm.CNPJ;
                    mens.CIDADE = vm.Celular;
                    mens.MENS_IN_CRM = novoPlano;
                    await ProcessaEnvioEMailCompra(mens, vm);

                    // DIALOG AQUI
                    TempData["ExibirSucesso"] = true;

                    // Sucesso
                    Session["MensFC"] = 333;
                    return RedirectToAction("MontarTelaDemo");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return View(vm);
            }
        }

        public Int32 CriarAssinanteNormal(FaleConoscoViewModel fc)
        {
            // Criar assinante
            FaleConoscoViewModel vm = fc;
            ASSINANTE assi = new ASSINANTE();
            assi.TIPE_CD_ID = vm.Tipo;
            assi.PLAN_CD_ID = 3;
            assi.ASSI_NM_NOME = vm.Nome;
            assi.ASSI_IN_ATIVO = 1;
            assi.ASSI_DT_INICIO = DateTime.Today.Date;
            if (vm.TipoAssinatura == 1)
            {
                assi.ASSI_IN_TIPO = 3;
            }
            else
            {
                assi.ASSI_IN_TIPO = 2;
            }
            assi.ASSI_IN_STATUS = 1;
            assi.ASSI_NM_EMAIL = vm.Resposta;
            assi.ASSI_NR_CNPJ = vm.CNPJ;
            assi.ASSI_NR_CPF = vm.CPF;
            assi.ASSI_NR_CELULAR = vm.Celular;
            assi.ASSI_IN_BLOQUEADO = 0;
            assi.ASSI_IN_VENCIDO = 0;
            assi.ASSI_IN_VENCIDO = 0;
            assi.ASSI_NM_BAIRRO = vm.Bairro;
            assi.ASSI_NM_CIDADE = vm.Cidade;
            assi.ASSI_NM_COMPLEMENTO = vm.Complemento;
            assi.ASSI_NM_ENDERECO = vm.Endereco;
            assi.ASSI_NM_RAZAO_SOCIAL = vm.Razao;
            assi.ASSI_NR_CEP = vm.CEPBase;
            assi.ASSI_NR_NUMERO = vm.Numero;
            assi.ASSI_NR_TELEFONE = vm.TelefoneFixo;
            assi.UF_CD_ID = vm.UF;
            assi.ASSI_AQ_FOTO = "~/Imagens/Base/icone_morador.png";
            Int32 voltaC = assiApp.ValidateCreate(assi);

            Int32 idAss = assi.ASSI_CD_ID;
            Session["idNovoAssinante"] = idAss;
            ASSINANTE assinante = assiApp.GetItemById(idAss);

            //// Cria assinante plano
            //ASSINANTE_PLANO assPlano = new ASSINANTE_PLANO();
            //assPlano.ASSI_CD_ID = idAss;
            //assPlano.PLAN_CD_ID = 10;
            //assPlano.ASPL_IN_ATIVO = 1;
            //assPlano.ASPL_DT_INICIO = DateTime.Today.Date;
            //if (vm.TipoAssinatura == 1)
            //{
            //    assPlano.ASPL_DT_VALIDADE = DateTime.Today.Date.AddDays(365);
            //}
            //else
            //{
            //    assPlano.ASPL_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            //}
            //assPlano.ASPL_IN_PRECO = 1;
            //assPlano.ASPL_IN_SISTEMA = 6;
            //assinante.ASSINANTE_PLANO.Add(assPlano);

            // Cria assinante-plano-assinatura
            ASSINANTE_PLANO_ASSINATURA assPlanoAss = new ASSINANTE_PLANO_ASSINATURA();
            assPlanoAss.ASSI_CD_ID = idAss;
            if (vm.Plano == 18)
            {
                assPlanoAss.PLAS_CD_ID = 18;
                assPlanoAss.ASPA_IN_PRECO = 2000;
            }
            if (vm.Plano == 20)
            {
                assPlanoAss.PLAS_CD_ID = 20;
                assPlanoAss.ASPA_IN_PRECO = 3500;
            }
            if (vm.Plano == 24)
            {
                assPlanoAss.PLAS_CD_ID = 24;
                assPlanoAss.ASPA_IN_PRECO = 4000;
            }
            assPlanoAss.ASPA_IN_ATIVO = 1;
            assPlanoAss.ASPA_DT_INICIO = DateTime.Today.Date;
            if (vm.TipoAssinatura == 1)
            {
                assPlanoAss.ASPA_DT_VALIDADE = DateTime.Today.Date.AddDays(365);
            }
            else
            {
                assPlanoAss.ASPA_DT_VALIDADE = DateTime.Today.Date.AddDays(30);

            }
            assPlanoAss.ASPA_IN_SISTEMA = 6;
            assinante.ASSINANTE_PLANO_ASSINATURA.Add(assPlanoAss);

            // Salva assinante
            Int32 voltaAssi1 = assiApp.ValidateEdit(assinante);

            // Cria pastas assinante
            String caminho = "/Imagens/Assinante/" + idAss.ToString() + "/Anexos/";
            String map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/Assinante/" + idAss.ToString() + "/Foto/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/Assinante/" + idAss.ToString() + "/Pagamentos/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));


            // Criar empresa
            EMPRESA emp = new EMPRESA();
            emp.ASSI_CD_ID = idAss;
            emp.EMPR_AQ_LOGO = "~/Imagens/Base/prontuario_icon_3.png";
            emp.EMPR_DT_CADASTRO = DateTime.Today.Date;
            emp.EMPR_IN_ATIVO = 1;
            emp.EMPR_IN_MATRIZ = 1;
            emp.EMPR_NM_BAIRRO = vm.Bairro;
            emp.EMPR_NM_CIDADE = vm.Cidade;
            emp.EMPR_NM_COMPLEMENTO = vm.Complemento;
            emp.EMPR_NM_EMAIL = vm.Resposta;
            emp.EMPR_NM_ENDERECO = vm.Endereco;
            emp.EMPR_NM_GERENTE = "Nome";
            emp.EMPR_NM_GUERRA = "Nome";
            emp.EMPR_NM_NOME = vm.Nome;
            emp.EMPR_NM_NUMERO = vm.Numero;
            emp.EMPR_NM_RAZAO = vm.Razao;
            emp.EMPR_NR_CELULAR = vm.Celular;
            emp.EMPR_NR_CEP = vm.CEPBase;
            emp.EMPR_NR_CNPJ = vm.CNPJ;
            emp.EMPR_NR_CPF = vm.CPF;
            emp.EMPR_NR_TELEFONE = vm.TelefoneFixo;
            emp.RETR_CD_ID = 2;
            emp.TIPE_CD_ID = vm.Tipo;
            emp.UF_CD_ID = vm.UF;
            Int32 voltaE = empApp.ValidateCreate(emp);

            // Recupera emmpresa
            EMPRESA empresa = empApp.GetItemById(emp.EMPR_CD_ID);

            // Cria Empresa/Filial
            EMPRESA_FILIAL emfi = new EMPRESA_FILIAL();
            emfi.ASSI_CD_ID = idAss;
            emfi.EMFI_AQ_LOGO = "~/Imagens/Base/prontuario_icon_3.png";
            emfi.EMFI_DT_CADASTRO = DateTime.Today.Date;
            emfi.EMFI_IN_ATIVO = 1;
            emfi.EMFI_IN_MATRIZ = 1;
            emfi.EMFI_NM_BAIRRO = vm.Bairro;
            emfi.EMFI_NM_CIDADE = vm.Cidade;
            emfi.EMFI_NR_COMPLEMENTO = vm.Complemento;
            emfi.EMFI_NM_EMAIL = vm.Resposta;
            emfi.EMFI_NM_ENDERECO = vm.Endereco;
            emfi.EMFI_NM_GERENTE = "Nome";
            emfi.EMFI_NM_APELIDO = "Nome";
            emfi.EMFI_NM_NOME = vm.Nome;
            emfi.EMFI_NR_NUMERO = vm.Numero;
            emfi.EMFI_NM_RAZAO = vm.Razao;
            emfi.EMFI_NR_CELULAR = vm.Celular;
            emfi.EMFI_NR_CEP = vm.CEPBase;
            emfi.EMFI_NR_CNPJ = vm.CNPJ;
            emfi.EMFI_NR_CPF = vm.CPF;
            emfi.EMFI_NR_TELEFONE = vm.Telefone;
            emfi.EMPR_CD_ID = empresa.EMPR_CD_ID;
            emfi.TIPE_CD_ID = vm.Tipo;
            emfi.UF_CD_ID = vm.UF;
            empresa.EMPRESA_FILIAL.Add(emfi);
            voltaE = empApp.ValidateEdit(empresa, empresa);

            // Recupera empresa
            empresa = empApp.GetItemById(emp.EMPR_CD_ID);

            // Login
            Session["LoginDemo"] = vm.LoginBase;

            // Cria pasta empresa
            caminho = "/Imagens/" + idAss.ToString() + "/Empresa/" + empresa.EMPR_CD_ID.ToString() + "/Anexos/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Empresa/" + empresa.EMPR_CD_ID.ToString() + "/Logo/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Senha
            String senha = vm.SenhaBase;
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            Session["SenhaDemo"] = senha;

            // Cria especialidade
            ESPECIALIDADE esp = new ESPECIALIDADE();
            esp.ASSI_CD_ID = idAss;
            esp.ESPE_IN_ATIVO = 1;
            esp.ESPE_NM_NOME = "Especialista";
            Int32 voltaEsp = espApp.ValidateCreate(esp);
            ESPECIALIDADE especial = espApp.GetItemById(esp.ESPE_CD_ID);

            // Cria Perfil
            PERFIL perf = new PERFIL();
            perf.ASSI_CD_ID = idAss;
            perf.PERF_IN_ACESSO_ADMIN = 1;
            perf.PERF_IN_ACESSO_AUX = 1;
            perf.PERF_IN_ACESSO_CADASTRO = 1;
            perf.PERF_IN_ACESSO_CP = 1;
            perf.PERF_IN_ACESSO_CR = 1;
            perf.PERF_IN_ACESSO_EMPRESA = 1;
            perf.PERF_IN_ACESSO_FINANCEIRO = 1;
            perf.PERF_IN_ACESSO_GRUPO = 1;
            perf.PERF_IN_ACESSO_MENSAGEM = 1;
            perf.PERF_IN_ACESSO_PACIENTE = 1;
            perf.PERF_IN_ACESSO_PAGREC = 1;
            perf.PERF_IN_ACESSO_TEMPLATE = 1;
            perf.PERF_IN_ACESSO_USUARIO = 1;
            perf.PERF_IN_ALTERACAO_PAGREC = 1;
            perf.PERF_IN_ALTERAR_PACIENTE = 1;
            perf.PERF_IN_ANAMNESE_ACESSO = 1;
            perf.PERF_IN_ANAMNESE_ALTERAR = 1;
            perf.PERF_IN_ANAMNESE_EXCLUIR = 1;
            perf.PERF_IN_ANAMNESE_INCLUIR = 1;
            perf.PERF_IN_ATESTADO_ACESSO = 1;
            perf.PERF_IN_ATESTADO_ALTERAR = 1;
            perf.PERF_IN_ATESTADO_ENVIAR = 1;
            perf.PERF_IN_ATESTADO_EXCLUIR = 1;
            perf.PERF_IN_ATESTADO_INCLUIR = 1;
            perf.PERF_IN_ATIVO = 1;
            perf.PERF_IN_BLOQUEIO = 1;
            perf.PERF_IN_BLOQUEIO_USUARIO = 1;
            perf.PERF_IN_CONF_CANC_CONSULTA = 1;
            perf.PERF_IN_EDICAO_AUX = 1;
            perf.PERF_IN_EDICAO_CP = 1;
            perf.PERF_IN_EDICAO_CR = 1;
            perf.PERF_IN_EDICAO_EMPRESA = 1;
            perf.PERF_IN_EDICAO_FILIAL = 1;
            perf.PERF_IN_EDICAO_GRUPO = 1;
            perf.PERF_IN_EDICAO_USUARIO = 1;
            perf.PERF_IN_EDITAR_TEMPLATE = 1;
            perf.PERF_IN_EXAME_ACESSO = 1;
            perf.PERF_IN_EXAME_ALTERAR = 1;
            perf.PERF_IN_EXAME_EXCLUIR = 1;
            perf.PERF_IN_EXAME_INCLUIR = 1;
            perf.PERF_IN_EXCLUIR_PACIENTE = 1;
            perf.PERF_IN_EXCLUSAO_AUX = 1;
            perf.PERF_IN_EXCLUSAO_CP = 1;
            perf.PERF_IN_EXCLUSAO_CR = 1;
            perf.PERF_IN_EXCLUSAO_GRUPO = 1;
            perf.PERF_IN_EXCLUSAO_PAGREC = 1;
            perf.PERF_IN_EXCLUSAO_TEMPLATE = 1;
            perf.PERF_IN_EXCLUSAO_USUARIO = 1;
            perf.PERF_IN_FINANCEIRO_ACESSO = 1;
            perf.PERF_IN_FINANCEIRO_CONTADOR = 1;
            perf.PERF_IN_FINANCEIRO_PAG_ACESSO = 1;
            perf.PERF_IN_FINANCEIRO_PAG_ALTERAR = 1;
            perf.PERF_IN_FINANCEIRO_PAG_EXCLUIR = 1;
            perf.PERF_IN_FINANCEIRO_PAG_INCLUIR = 1;
            perf.PERF_IN_FINANCEIRO_REC_ACESSO = 1;
            perf.PERF_IN_FINANCEIRO_REC_ALTERAR = 1;
            perf.PERF_IN_FINANCEIRO_REC_EXCLUIR = 1;
            perf.PERF_IN_FINANCEIRO_REC_INCLUIR = 1;
            perf.PERF_IN_FINANCEIRO_RELATORIO = 1;
            perf.PERF_IN_FISICO_ACESSO = 1;
            perf.PERF_IN_FISICO_ALTERAR = 1;
            perf.PERF_IN_FISICO_EXCLUIR = 1;
            perf.PERF_IN_FISICO_INCLUIR = 1;
            perf.PERF_IN_INCLUIR_PACIENTE = 1;
            perf.PERF_IN_INCLUSAO_AUX = 1;
            perf.PERF_IN_INCLUSAO_CP = 1;
            perf.PERF_IN_INCLUSAO_CR = 1;
            perf.PERF_IN_INCLUSAO_GRUPO = 1;
            perf.PERF_IN_INCLUSAO_MENSAGEM = 1;
            perf.PERF_IN_INCLUSAO_PAGREC = 1;
            perf.PERF_IN_INCLUSAO_TEMPLATE = 1;
            perf.PERF_IN_INCLUSAO_USUARIO = 1;
            perf.PERF_IN_PACIENTE_ATESTADO = 1;
            perf.PERF_IN_PACIENTE_CONSULTA_ACESSO = 1;
            perf.PERF_IN_PACIENTE_CONSULTA_ALTERAR = 1;
            perf.PERF_IN_PACIENTE_CONSULTA_EXCLUIR = 1;
            perf.PERF_IN_PACIENTE_CONSULTA_INCLUIR = 1;
            perf.PERF_IN_PACIENTE_SOLICITACAO = 1;
            perf.PERF_IN_PAGAR_CP = 1;
            perf.PERF_IN_PRESCRICAO_ACESSO = 1;
            perf.PERF_IN_PRESCRICAO_ALTERAR = 1;
            perf.PERF_IN_PRESCRICAO_ENVIAR = 1;
            perf.PERF_IN_PRESCRICAO_EXCLUIR = 1;
            perf.PERF_IN_PRESCRICAO_INCLUIR = 1;
            perf.PERF_IN_REATIVACAO_AUX = 1;
            perf.PERF_IN_REATIVACAO_FILIAL = 1;
            perf.PERF_IN_REATIVACAO_USUARIO = 1;
            perf.PERF_IN_REATIVAR_CP = 1;
            perf.PERF_IN_REATIVAR_CR = 1;
            perf.PERF_IN_REATIVAR_GRUPO = 1;
            perf.PERF_IN_REATIVAR_PACIENTE = 1;
            perf.PERF_IN_REATIVA_PAGREC = 1;
            perf.PERF_IN_REATIVA_TEMPLATE = 1;
            perf.PERF_IN_RECEBER_CR = 1;
            perf.PERF_IN_SOLICITACAO_ACESSO = 1;
            perf.PERF_IN_SOLICITACAO_ALTERAR = 1;
            perf.PERF_IN_SOLICITACAO_ENVIAR = 1;
            perf.PERF_IN_SOLICITACAO_EXCLUIR = 1;
            perf.PERF_IN_SOLICITACAO_INCLUIR = 1;
            perf.PERF_IN_ACERTO_ESTOQUE = 1;
            perf.PERF_IN_ACESSO_ESTOQUE = 1;
            perf.PERF_IN_ATUALIZA_ESTOQUE = 1;
            perf.PERF_IN_COMPRA_MANUAL_ESTOQUE = 1;
            perf.PERF_IN_DESCARTE_ESTOQUE = 1;
            perf.PERF_IN_DEVOLUCAO_ESTOQUE = 1;
            perf.PERF_IN_EDITAR_MOVIMENTACAO_ESTOQUE = 1;
            perf.PERF_IN_EXCLUIR_MOVIMENTACAO_ESTOQUE = 1;
            perf.PERF_IN_INCLUIR_MOVIMENTACAO_ESTOQUE = 1;
            perf.PERF_IN_MANUTENCAO_ESTOQUE = 1;
            perf.PERF_IN_PERDA_ESTOQUE = 1;
            perf.PERF_IN_TRANSFERENCIA_ESTOQUE = 1;
            perf.PERF_IN_VER_MOVIMENTACAO_ESTOQUE = 1;
            perf.PERF_IN_LOCACAO_ACESSO = 1;
            perf.PERF_IN_LOCACAO_ALTERAR = 1;
            perf.PERF_IN_LOCACAO_ENCERRAR = 1;
            perf.PERF_IN_LOCACAO_INCLUIR = 1;
            perf.PERF_IN_LOCACAO_RENOVAR = 1;
            perf.PERF_IN_LOCACAO__EXCLUIR = 1;
            perf.PERF_NM_NOME = "Perfil Administrador";
            perf.PERF_SG_SIGLA = "ADM";
            perf.PERF_IN_FIXO = 1;
            Int32 voltaPerf = perfApp.ValidateCreate(perf);
            PERFIL perfil = perfApp.GetItemById(perf.PERF_CD_ID);

            // Cria usuario master
            USUARIO usu = new USUARIO();
            usu.ASSI_CD_ID = idAss;
            usu.CAUS_CD_ID = 1;
            usu.EMFI_CD_ID = empresa.EMPRESA_FILIAL.First().EMFI_CD_ID;
            usu.EMPR_CD_ID = empresa.EMPR_CD_ID;
            usu.ESPE_CD_ID = especial.ESPE_CD_ID;
            usu.PERF_CD_ID = perfil.PERF_CD_ID;
            usu.TICL_CD_ID = 8;
            usu.USUA_AQ_FOTO = "~/Imagens/Base/icone_morador.png";
            usu.USUA_DT_ACESSO = DateTime.Today.Date;
            usu.USUA_DT_ALTERACAO = DateTime.Today.Date;
            usu.USUA_DT_CADASTRO = DateTime.Today.Date;
            usu.USUA_DT_TROCA_SENHA = DateTime.Today.Date;
            usu.USUA_IN_ATIVO = 1;
            usu.USUA_IN_BLOQUEADO = 0;
            usu.USUA_IN_ESPECIAL = 1;
            usu.USUA_IN_HUMANO = 1;
            usu.USUA_IN_LOGADO = 0;
            usu.USUA_IN_LOGIN_PROVISORIO = 0;
            usu.USUA_IN_PENDENTE_CODIGO = 0;
            usu.USUA_IN_PROVISORIO = 0;
            usu.USUA_IN_SISTEMA = 6;
            usu.USUA_IN_TECNICO = 0;
            usu.USUA_NM_APELIDO = vm.Nome.Substring(0, vm.Nome.IndexOf(" "));
            usu.USUA_NM_EMAIL = vm.Resposta;
            usu.USUA_NM_ESPECIALIDADE = vm.Especialidade;
            usu.USUA_NM_LOGIN = vm.LoginBase;
            usu.USUA_NM_NOME = vm.Nome;
            usu.USUA_NR_ACESSOS = 1;
            usu.USUA_NR_CELULAR = vm.Celular;
            usu.USUA_NR_CLASSE = null;
            usu.USUA_NR_CPF = vm.CPF;
            usu.USUA_NR_FALHAS = 0;
            usu.USUA_NR_TELEFONE = vm.Telefone;
            usu.USUA_NM_SENHA = hashedPassword;
            usu.USUA_NM_SALT = salt;
            usu.USUA_NM_SENHA_CONFIRMA = vm.SenhaBase;
            Int32 voltaUsu = usuApp.ValidateCreate(usu);
            USUARIO usuario = usuApp.GetItemById(usu.USUA_CD_ID);

            // Cria pasta usuario
            caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + usuario.USUA_CD_ID.ToString() + "/Anexos/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + usuario.USUA_CD_ID.ToString() + "/Fotos/";
            map = Server.MapPath(caminho);

            // Cria demais pastas
            caminho = "/Imagens/" + idAss.ToString() + "/Envio/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Exames/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Locacao/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Medico/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Produto/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/TemplatesHTML/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/Videos/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));
            caminho = "/Imagens/" + idAss.ToString() + "/AreaPaciente/";
            map = Server.MapPath(caminho);
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Cria primeiro aviso
            AVISO_LEMBRETE av = new AVISO_LEMBRETE();
            av.ASSI_CD_ID = idAss;
            av.AVIS_DS_AVISO = "Bemvindo ao WebDoctorPro!";
            av.AVIS_DT_AVISO = DateTime.Now;
            av.AVIS_DT_CRIACAO = DateTime.Now;
            av.AVIS_IN_ATIVO = 1;
            av.AVIS_IN_CIENTE = 0;
            av.AVIS_IN_SISTEMA = 6;
            av.AVIS_NM_TITULO = "Primeiro Aviso";
            av.USUA_CD_ID = usuario.USUA_CD_ID;
            Int32 voltaAv = avApp.ValidateCreate(av);

            // Cria templates E-Mail
            List<TEMPLATE_EMAIL> tempMail = mailApp.GetAllItens(1);
            foreach (TEMPLATE_EMAIL item in tempMail)
            {
                TEMPLATE_EMAIL novo = new TEMPLATE_EMAIL();
                novo.ASSI_CD_ID = idAss;
                novo.EMPR_CD_ID = usuario.EMPR_CD_ID;
                novo.TEEM_AQ_ARQUIVO = item.TEEM_AQ_ARQUIVO;
                novo.TEEM_IN_ANIVERSARIO = item.TEEM_IN_ANIVERSARIO;
                novo.TEEM_IN_ATIVO = 1;
                novo.TEEM_IN_EDITAVEL = item.TEEM_IN_EDITAVEL;
                novo.TEEM_IN_FIXO = item.TEEM_IN_FIXO;
                novo.TEEM_IN_HTML = item.TEEM_IN_HTML;
                novo.TEEM_IN_IMAGEM = item.TEEM_IN_IMAGEM;
                novo.TEEM_IN_PESQUISA = item.TEEM_IN_IMAGEM;
                novo.TEEM_IN_ROBOT = item.TEEM_IN_ROBOT;
                novo.TEEM_IN_SISTEMA = 6;
                novo.TEEM_LK_LINK = item.TEEM_LK_LINK;
                novo.TEEM_NM_NOME = item.TEEM_NM_NOME;
                novo.TEEM_SG_SIGLA = item.TEEM_SG_SIGLA;
                novo.TEEM_TX_CABECALHO = item.TEEM_TX_CABECALHO;
                novo.TEEM_TX_COMPLETO = item.TEEM_TX_COMPLETO;
                novo.TEEM_TX_CORPO = item.TEEM_TX_CORPO;
                novo.TEEM_TX_DADOS = item.TEEM_TX_DADOS;
                Int32 voltaEM = mailApp.ValidateCreate(novo);
            }

            // Cria templates SMS
            List<TEMPLATE_SMS> tempSMS = smsApp.GetAllItens(1);
            foreach (TEMPLATE_SMS item in tempSMS)
            {
                TEMPLATE_SMS novo = new TEMPLATE_SMS();
                novo.ASSI_CD_ID = idAss;
                novo.EMPR_CD_ID = usuario.EMPR_CD_ID;
                novo.TSMS_IN_ATIVO = 1;
                novo.TSMS_IN_EDITAVEL = item.TSMS_IN_EDITAVEL;
                novo.TSMS_IN_FIXO = item.TSMS_IN_FIXO;
                novo.TSMS_IN_ROBOT = item.TSMS_IN_ROBOT;
                novo.TSMS_LK_LINK = item.TSMS_LK_LINK;
                novo.TSMS_NM_NOME = item.TSMS_NM_NOME;
                novo.TSMS_NR_SISTEMA = 6;
                novo.TSMS_SG_SIGLA = item.TSMS_SG_SIGLA;
                novo.TSMS_TX_CORPO = item.TSMS_TX_CORPO;
                Int32 voltaEM = smsApp.ValidateCreate(novo, usuario);
            }

            // Cria tipo de exame
            TIPO_EXAME ti = new TIPO_EXAME();
            ti.ASSI_CD_ID = idAss;
            ti.TIEX_NM_NOME = "Sangue";
            ti.TIEX_IN_ATIVO = 1;
            Int32 voltaTI = tiApp.ValidateCreate(ti);

            // Cria tipo de Atestado
            TIPO_ATESTADO ta = new TIPO_ATESTADO();
            ta.ASSI_CD_ID = idAss;
            ta.TIAT_NM_NOME = "Atestado Médico";
            ta.TIAT_IN_ATIVO = 1;
            Int32 voltaTA = taApp.ValidateCreate(ta, null);

            // Cria tipo de paciente
            TIPO_PACIENTE tp = new TIPO_PACIENTE();
            tp.ASSI_C_DID = idAss;
            tp.TIPA_NM_NOME = "Normal";
            tp.TIPA_IN_ATIVO = 1;
            Int32 voltaTP = tpApp.ValidateCreate(tp);

            // Cria tipo de pagamento
            TIPO_PAGAMENTO tg = new TIPO_PAGAMENTO();
            tg.ASSI_CD_ID = idAss;
            tg.TIPA_NM_PAGAMENTO = "PIX";
            tg.TIPA_IN_ATIVO = 1;
            tg.USUA_CD_ID = usuario.USUA_CD_ID;
            Int32 voltaTG = tgApp.ValidateCreate(tg);

            // Cria tipo de consulta
            TIPO_VALOR_CONSULTA tc = new TIPO_VALOR_CONSULTA();
            tc.ASSI_CD_ID = idAss;
            tc.TIVL_NM_TIPO = "Normal";
            tc.TIVL_IN_ATIVO = 1; 
            Int32 voltaTC = tcApp.ValidateCreate(tc);
            TIPO_VALOR_CONSULTA tipoValor = tcApp.GetItemById(tc.TIVL_CD_ID);

            // Cria tipo de contrato
            List<CONTRATO_LOCACAO> conts = locApp.GetAllContratos(1);
            foreach (CONTRATO_LOCACAO item in conts)
            {
                CONTRATO_LOCACAO novo = new CONTRATO_LOCACAO();
                novo.ASSI_CD_ID = idAss;
                novo.COLO_IN_ATIVO = 1;
                novo.COLO_DT_CRIACAO = item.COLO_DT_CRIACAO;
                novo.COLO_NM_NOME = item.COLO_NM_NOME;
                novo.COLO_TX_TEXTO = item.COLO_TX_TEXTO;
                novo.USUA_CD_ID = item.USUA_CD_ID;
                Int32 voltaEM = locApp.ValidateCreateContrato(novo);
            }

            // Cria tipo de Pagamento
            List<TIPO_PAGAMENTO> tps = tpgApp.GetAllItens(1);
            foreach (TIPO_PAGAMENTO item in tps)
            {
                TIPO_PAGAMENTO novo = new TIPO_PAGAMENTO();
                novo.ASSI_CD_ID = idAss;
                novo.TIPA_IN_ATIVO = 1;
                novo.TIPA_NM_PAGAMENTO = item.TIPA_NM_PAGAMENTO;
                novo.USUA_CD_ID = item.USUA_CD_ID;
                Int32 voltaEM = tpgApp.ValidateCreate(novo);
            }

            // Cria tipo de Consulta
            List<TIPO_VALOR_CONSULTA> ticos = ticoApp.GetAllItens(1);
            foreach (TIPO_VALOR_CONSULTA item in ticos)
            {
                TIPO_VALOR_CONSULTA novo = new TIPO_VALOR_CONSULTA();
                novo.ASSI_CD_ID = idAss;
                novo.TIVL_IN_ATIVO = 1;
                novo.TIVL_IN_PADRAO = item.TIVL_IN_PADRAO;
                novo.TIVL_NM_TIPO = item.TIVL_NM_TIPO;
                Int32 voltaEM = ticoApp.ValidateCreate(novo);
            }

            // Cria Unidade
            List<UNIDADE> uns = uniApp.GetAllItens(1);
            foreach (UNIDADE item in uns)
            {
                UNIDADE novo = new UNIDADE();
                novo.ASSI_CD_ID = idAss;
                novo.UNID_IN_ATIVO = 1;
                novo.UNID_IN_FRACIONADA = item.UNID_IN_FRACIONADA;
                novo.UNID_IN_TIPO_UNIDADE = item.UNID_IN_TIPO_UNIDADE;
                novo.UNID_NM_NOME = item.UNID_NM_NOME;
                novo.UNID_SG_SIGLA = item.UNID_SG_SIGLA;
                Int32 voltaEM = uniApp.ValidateCreate(novo, usuario);
            }

            // Cria valor de consulta
            List<VALOR_CONSULTA> vcs = vcApp.GetAllItens(1).Where(p => p.VACO_CD_ID < 3).ToList();
            foreach (VALOR_CONSULTA item in vcs)
            {
                VALOR_CONSULTA novo = new VALOR_CONSULTA();
                novo.ASSI_CD_ID = idAss;
                novo.VACO_IN_ATIVO = 1;
                novo.VACO_DT_REFERENCIA = item.VACO_DT_REFERENCIA;
                novo.VACO_IN_MATERIAL = item.VACO_IN_MATERIAL;
                novo.VACO_IN_PADRAO = item.VACO_IN_PADRAO;
                novo.VACO_NM_EXIBE = item.VACO_NM_EXIBE;
                novo.VACO_NM_NOME = item.VACO_NM_NOME;
                novo.VACO_NR_DESCONTO = item.VACO_NR_DESCONTO;
                novo.VACO_NR_VALOR = item.VACO_NR_VALOR;
                novo.TIVL_CD_ID = item.TIVL_CD_ID;
                novo.USUA_CD_ID = item.USUA_CD_ID;
                Int32 voltaEM = vcApp.ValidateCreate(novo, usuario);
            }

            // Cria periodicidades
            PERIODICIDADE_TAREFA pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Mensal";
            pt.PETA_NR_DIAS = 30;
            pt.PETA_IN_ATIVO = 1;
            Int32 voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Semanal";
            pt.PETA_NR_DIAS = 7;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Diária";
            pt.PETA_NR_DIAS = 1;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Trimestral";
            pt.PETA_NR_DIAS = 90;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Anual";
            pt.PETA_NR_DIAS = 365;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Semestral";
            pt.PETA_NR_DIAS = 180;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Bimestral";
            pt.PETA_NR_DIAS = 60;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);
            pt = new PERIODICIDADE_TAREFA();
            pt.ASSI_CD_ID = idAss;
            pt.PETA_NM_NOME = "Quinzenal";
            pt.PETA_NR_DIAS = 15;
            pt.PETA_IN_ATIVO = 1;
            voltaPT = peApp.ValidateCreate(pt, null);

            // Cria configuracao anamnese
            CONFIGURACAO_ANAMNESE ca = new CONFIGURACAO_ANAMNESE();
            ca.ASSI_CD_ID = idAss;
            ca.COAN_IN_ABDOMEM = 1;
            ca.COAN_IN_ATIVO = 1;
            ca.COAN_IN_CARDIOLOGICA = 1;
            ca.COAN_IN_CONDUTA = 1;
            ca.COAN_IN_DIAGNOSTICO_1 = 1;
            ca.COAN_IN_HISTORIA_DOENCA = 1;
            ca.COAN_IN_HISTORIA_FAMILIAR = 1;
            ca.COAN_IN_HISTORIA_PATOLOGIA = 1;
            ca.COAN_IN_HISTORIA_SOCIAL = 1;
            ca.COAN_IN_MEDICAMENTO = 1;
            ca.COAN_IN_MEMBROS = 1;
            ca.COAN_IN_MOTIVO_CONSULTA = 1;
            ca.COAN_IN_OBSERVACOES = 1;
            ca.COAN_IN_QUEIXA = 1;
            ca.COAN_IN_RESPIRATORIA = 1;
            ca.COAN_IN_CAMPO_1 = 1;
            ca.COAN_IN_CAMPO_10 = 0;
            ca.COAN_IN_CAMPO_2 = 0;
            ca.COAN_IN_CAMPO_3 = 0;
            ca.COAN_IN_CAMPO_4 = 0;
            ca.COAN_IN_CAMPO_5 = 0;
            ca.COAN_IN_CAMPO_6 = 0;
            ca.COAN_IN_CAMPO_7 = 0;
            ca.COAN_IN_CAMPO_8 = 0;
            ca.COAN_IN_CAMPO_9 = 0;
            ca.COAN_NM_CAMPO_1 = "Evolução Mental";
            ca.COAN_IN_PADRAO_FORMATO = 1;
            ca.COAN_IN_FORMATO_CONTINUA = 1;
            ca.COAN_IN_BLOCO_COMUM = 1;
            ca.COAN_IN_BLOCO_SONO = 0;
            Int32 voltaCA = caApp.ValidateCreate(ca);

            // Cria configuracao de calendario
            CONFIGURACAO_CALENDARIO cc = new CONFIGURACAO_CALENDARIO();
            cc.ASSI_CD_ID = idAss;
            cc.COCA_IN_ATIVO = 1;
            cc.COCA_IN_DOMINGO = 0;
            cc.COCA_IN_QUARTA_FEIRA = 1;
            cc.COCA_IN_QUINTA_FEIRA = 1;
            cc.COCA_IN_SABADO = 1;
            cc.COCA_IN_SEGUNDA_FEIRA = 1;
            cc.COCA_IN_SEXTA_FEIRA = 1;
            cc.COCA_IN_TERCA_FEIRA = 1;
            cc.USUA_CD_ID = usuario.USUA_CD_ID;
            cc.COCA_HR_COMERCIAL_QUA_FINAL = TimeSpan.Parse("17:00:00");
            cc.COCA_HR_COMERCIAL_QUA_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_COMERCIAL_QUI_FINAL = TimeSpan.Parse("17:00:00");
            cc.COCA_HR_COMERCIAL_QUI_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_COMERCIAL_SAB_FINAL = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_COMERCIAL_SAB_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_COMERCIAL_SEG_FINAL = TimeSpan.Parse("17:00:00");
            cc.COCA_HR_COMERCIAL_SEG_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_COMERCIAL_SEX_FINAL = TimeSpan.Parse("17:00:00");
            cc.COCA_HR_COMERCIAL_SEX_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_COMERCIAL_TER_FINAL = TimeSpan.Parse("17:00:00");
            cc.COCA_HR_COMERCIAL_TER_INICIO = TimeSpan.Parse("08:00:00");
            cc.COCA_HR_INTERVALO_QUA_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_QUA_INICIO = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_INTERVALO_QUI_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_QUI_INICIO = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_INTERVALO_SAB_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_SAB_INICIO = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_INTERVALO_SEG_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_SEG_INICIO = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_INTERVALO_SEX_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_SEX_INICIO = TimeSpan.Parse("12:00:00");
            cc.COCA_HR_INTERVALO_TER_FINAL = TimeSpan.Parse("13:00:00");
            cc.COCA_HR_INTERVALO_TER_INICIO = TimeSpan.Parse("12:00:00");
            Int32 voltaCC = ccApp.ValidateCreate(cc);

            // Cria configuracao base
            CONFIGURACAO cf = new CONFIGURACAO();
            cf.ASSI_CD_ID = idAss;
            cf.CONF_NR_FALHAS_DIA = 6;
            cf.CONF_NM_HOST_SMTP = "smtp.sendgrid.net";
            cf.CONF_NM_PORTA_SMTP = "587";
            cf.CONF_NM_EMAIL_EMISSOO = "sistema@systembr.net";
            cf.CONF_NM_SENHA_EMISSOR = "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";
            cf.CONF_NR_REFRESH_DASH = 3000;
            cf.CONF_NM_ARQUIVO_ALARME = "chimes.wav";
            cf.CONF_NR_REFRESH_NOTIFICACAO = 3000;
            cf.CONF_SG_LOGIN_SMS = "rtiltda";
            cf.CONF_SG_LOGIN_SMS_CRIP = "BJYLnSQbAbMyw53QzSKIQg==";
            cf.CONF_SG_SENHA_SMS = "a135701rt";
            cf.CONF_SG_SENHA_SMS_CRIP = "CmE7auWb/7NDBpN2J3bfjG5y1ctIWBbb1I9QgMGc0do=";
            cf.CONF_SG_LOGIN_SMS_PRIORITARIO = "rti2023";
            cf.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP = "nqLoN2fdhE/ZaVUtOVlxbQ==";
            cf.CONF_SG_SENHA_SMS_PRIORITARIO = "a135701##";
            cf.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP = "BByrcwy4/Pfjo5ZPIvordRCWhm8ATYBL3L6wAhVml58=";
            cf.CONF_NM_SENDGRID_LOGIN = "apikey";
            cf.CONF_NM_SENDGRID_LOGIN_CRIP = "9EK29rwRkBZeBBVLWxiSFg==";
            cf.CONF_NM_SENDGRID_PWD = "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";
            cf.CONF_NM_SENDGRID_PWD_CRIP = "IxJZX9rxQKDKsmU7Ed/KOlk+eZilr9/jiIOJTplnyzyJBNuNWo2Ihukc/jp4MskZE7/yzxiC9afRqyul8WqtIM2zEr3E0J/shhB07Y5IagBB/SfonXB1LHU7zkl0a5y6SaEZDqYbjAaSsoMfAKQXn1cLoKzGGvwPGset6DWGYaholV/IUm1FZyTAUKPJK1HT";
            cf.CONF_NM_SENDGRID_APIKEY = "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";
            cf.CONF_NM_SENDGRID_APIKEY_CRIP = "IxJZX9rxQKDKsmU7Ed/KOlk+eZilr9/jiIOJTplnyzyJBNuNWo2Ihukc/jp4MskZE7/yzxiC9afRqyul8WqtIM2zEr3E0J/shhB07Y5IagBB/SfonXB1LHU7zkl0a5y6SaEZDqYbjAaSsoMfAKQXn1cLoKzGGvwPGset6DWGYaholV/IUm1FZyTAUKPJK1HT";
            cf.CONF_NR_DIAS_ATENDIMENTO = 3;
            cf.CONF_NR_DIAS_ACAO = 3;
            cf.CONF_NR_DIAS_PROPOSTA = 3;
            cf.CONF_NR_MARGEM_ATRASO = 3;
            cf.CONF_IN_DIAS_RESERVA_ESTOQUE = 0;
            cf.CONF_IN_NUMERO_INICIAL_PROPOSTA = 1;
            cf.CONF_IN_NUMERO_INICIAL_PEDIDO = 1;
            cf.CONF_IN_CNPJ_DUPLICADO = 1;
            cf.CONF_IN_INCLUIR_SEM_ESTOQUE = 1;
            cf.CONF_IN_ASSINANTE_FILIAL = 1;
            cf.CONF_IN_FALHA_IMPORTACAO = 1;
            cf.CONF_IN_ETAPAS_CRM = 9;
            cf.CONF_IN_NOTIF_ACAO_ADM = 1;
            cf.CONF_IN_NOTIF_ACAO_GER = 0;
            cf.CONF_IN_NOTIF_ACAO_OPR = 0;
            cf.CONF_IN_NOTIF_ACAO_USU = 0;
            cf.CONF_IN_NOTIF_ACAO_VEN = 0;
            cf.CONF_LK_LINK_SISTEMA = "https://webdoctorpro.net";
            cf.CONF_EM_CRMSYS = "suporte@rtiltda.net";
            cf.CONF_EM_CRMSYS1 = "clayton@systembr.net";
            cf.CONF_NR_SUPORTE_ZAP = "(21)97302-4096";
            cf.CONF_NR_SUPORTE_ZAP1 = "(11)94170-6199";
            cf.CONF_NR_VALIDADE_SENHA = 360;
            cf.CONF_NR_TAMANHO_SENHA = 8;
            cf.CONF_IN_LOGO_EMPRESA = 1;
            cf.CONF_NR_GRID_CLIENTE = 100;
            cf.CONF_NR_GRID_MENSAGEM = 100;
            cf.CONF_CS_CONNECTION_STRING_AZURE = "endpoint=https://rticomunicacao.communication.azure.com/;accesskey=4xywYgHszNMkkjMideSlQ+HcsJzx/+xDcBfi2NA98Dwp9KiM7lgWeCuh455NWsDjTIso9QbMqnq++rIQo4rg1w==";
            cf.CONF_CS_CONNECTION_STRING_AZURE_CRIP = "ZK369HpFK9EzjM0Iq0tsTbhw7M1jcYfd97ETRRfom1cbdLvzqE6Tbga+c4NwLlyzFY+kXc5xXljrn03xa3xNmzweCRnoEkjRpKWKPccf/2JpdbjMvV9SrPS2m0cVphhb0NgpfVQ6VwJUo9qupvwWNsLq610iGDnuz4c7Oddtn2YXBDUmFVtqlZ6T/XEUCauFVZhASDh8JxNgf7nAQCutJLgRoeNQMfKMlcOnZ4bZorrK8jiGLzvccEI1FfxwBijGfUiqPZTkJKJtG9ySUdGc9S6QM9bselH3YQsj5pmZAGdbpbup2l5Y+lE8ugYcVbwfx3qJkec68k+FdDzAPRZ9XgbQmYSMCeoTTrNhwgNJ13nOYslIb5dMM0QXeZOlQVcak992+Mgw7c9jCFM/wR6Mz8diXATy794Ona57wBIHzAo=";
            cf.CONF_NM_KEY_AZURE = "4xywYgHszNMkkjMideSlQ+HcsJzx/+xDcBfi2NA98Dwp9KiM7lgWeCuh455NWsDjTIso9QbMqnq++rIQo4rg1w==";
            cf.CONF_NM_KEY_AZURE_CRIP = "ltHoPUjeS1qehj9uo/E5g4KUIO1C6RPRhhJ/nW1WGJxQGXPeBiy/S6/4wnWrHT8ktFWwJHw7HClESiuRlF/bwJESZaWzDJO/ufseow+S4THlTgqebXI5Ok8KGWv3alvmQkFCAYVXnCQden2yvX9DLnERRtf0YAQCb6oFfRTAGq6KC7dMbKoct6/7Co8jCPHw8g28E/CfQKJFdo9IMQ4MvTp1U1TWzXGgzTuloPTUwnHqGrK0S95V97NvvZCHvIcp";
            cf.CONF_NM_ENDPOINT_AZURE = "https://rticomunicacao.communication.azure.com/";
            cf.CONF_NM_ENDPOINT_AZURE_CRIP = "ec9uiUGjpFZMX+yw+QuxTAsQMclPkmopWKO7mja8ZJwqfPOcxnHpThrdZFLJuUZiSYsBHS9oKMx7ximskM1DTywQm/gkZPIfEJkjwFhDI0Oi2B2KffxH+YcNSp3h/cUm";
            cf.CONF_NM_EMISSOR_AZURE = "donotreply@15157d38-0d81-4693-8f1b-912e476da78b.azurecomm.net";
            cf.CONF_NM_EMISSOR_AZURE_CRIP = "z5xnxrI/7aDMiUIegW3ZrRu2u4FFqBbD3hRtsYEfJ/TWBCIBLglyUWB29BRxEOuJLPboRBlWyp/yQgyO1KaZbxii6pXqU+uxlgk65IMBckdgFH3MEvis/XmBFRXd0e68eahbOO+ycs121YSbPWyfX35uANG+paFhV6qmNzild4k=";
            cf.CONF_IN_VALIDADE_CODIGO = 10;
            cf.CONF_NR_GRID_DOCUMENTO = 30;
            cf.CONF_NR_DIAS_LOG = 50;
            cf.CONF_NR_DIAS_FIM_LOG = 5;
            cf.CONF_EM_CONTATO = "suporte@rtiltda.net";
            cf.CONF_NR_AVISO_CONTAS = 5;
            cf.CONF_IN_MENSAGENS_CP = 1;
            cf.CONF_IN_MENSAGENS_CR = 1;
            cf.CONF_IN_ROBOT = 1;
            cf.CONF_IN_CLIENTE_SISTEMA = 1;
            cf.CONF_IN_MODELO_MAIL_SISTEMA = 1;
            cf.CONF_IN_PAGAR_SISTEMA = 1;
            cf.CONF_IN_RECEBER_SISTEMA = 1;
            cf.CONF_IN_USUARIO_SISTEMA = 1;
            cf.CONF_NM_SUFIXO_RECORRENCIA = "Ocorrência - ";
            cf.CONF_NM_SUFIXO_NUMERO = "0";
            cf.CONF_IN_SUFIXO_NUMERO = 1;
            cf.CONF_IN_DASH_INICIAL = 0;
            cf.CONF_IN_MENSAGEM_FABRICANTE = 1;
            cf.CONF_IN_EMAIL_ROBOT = 1;
            cf.CONF_IN_SMS_ROBOT = 1;
            cf.CONF_IN_DIAS_ESTADO = 1;
            cf.CONF_IN_PACIENTE_ATRASO = 2;
            cf.CONF_IN_PACIENTE_AUSENCIA = 2;
            cf.CONF_IN_MENSAGEM_CONSULTA = 1;
            cf.CONF_LK_LINK_VALIDACAO = "https://crmsys.azurewebsites.net/Consulta/";
            cf.CONF_IN_EXIBE_LOGO = 1;
            cf.CONF_IN_EMAIL_AUTOMATICO = 1;
            cf.CONF_LK_LINK_VALIDACAO = "https://validaqrcode..azurewebsites.net/";
            cf.CONF_NR_DIAS_CONFIRMACAO = 5;
            cf.CONF_NR_MESES_RETORNO = 2;
            cf.CONF_IN_INCLUIR_REMEDIO = 1;
            cf.CONF_IN_INCLUIR_SOLICITACAO = 1;
            cf.CONF_IN_GERA_RECEBIMENTO = 1;
            cf.CONF_IN_INCLUIR_PACIENTE_SEGUE = 1;
            cf.CONF_IN_PACIENTE_FOTO_CAMERA = 1;
            cf.CONF_IN_PADRAO_ANAMNESE = 1;
            cf.CON_TK_TOKEN_API_PAGTO = "68635879-5c7c-4121-8dc7-9dbdc01fa0c8a6b915db4bc4b0f86a6fdf81cf0e2ba207a6-b3bf-46b4-a85f-8489e680a29d";
            cf.CONF_IN_CALCULA_PROXIMA_CONSULTA = 1;
            cf.CONF_IN_DIAS_PROXIMA_CONSULTA = 60;
            cf.CONF_LK_LINK_VALIDACAO = "https://webdoctorpro.net/";
            cf.CONF_NR_DIAS_CONFIRMACAO = 5;
            cf.CONF_NR_MESES_RETORNO = 6;
            cf.CONF_IN_INCLUIR_REMEDIO = 1;
            cf.CONF_IN_INCLUIR_SOLICITACAO = 1;
            cf.CONF_IN_ASSINA_DIGITAL_SOLICITACAO = 1;
            cf.CONF_IN_INCLUIR_PACIENTE_SEGUE = 0;
            cf.CONF_IN_PACIENTE_FOTO_CAMERA = 1;
            cf.CONF_IN_PADRAO_ANAMNESE = 1;
            cf.CONF_FD_FICHAS = @"c:\Fichas";
            cf.CON_TK_TOKEN_API_PAGTO = "68635879-5c7c-4121-8dc7-9dbdc01fa0c8a6b915db4bc4b0f86a6fdf81cf0e2ba207a6-b3bf-46b4-a85f-8489e680a29d";
            cf.CONF_IN_CALCULA_PROXIMA_CONSULTA = 1;
            cf.CONF_IN_DIAS_PROXIMA_CONSULTA = null;
            cf.CONF_LK_FORM_URL_BASE = null;
            cf.CONF_IN_PISCA = 1;
            cf.CONF_IN_ENVIA_ANIVERSARIO = 1;
            cf.CONF_IN_ENVIA_CONFIRMACAO = 1;
            cf.CONF_NR_WHAPSAPP = "(21)97302-4096";
            cf.CONF_IN_VALIDADE_FAIXA = 180;
            cf.CONF_IN_ENVIA_PACIENTE_CADASTRO = 1;
            cf.CONF_IN_ENVIA_ATRASO = 1;
            cf.CONF_IN_MAXIMO_ENVIO = 5;
            cf.CONF_IN_INTERVALO_ENVIO = 3;
            cf.CONF_IN_HORA_LIMITE = null;
            cf.CONF_IN_MENSAGEM_MARCACAO = null;
            cf.CONF_IN_MARCA_CONSULTA_HORA = 1;
            cf.CONF_NR_MARCA_CONSULTA_HORA = null;
            cf.CONF_IN_ASSINA_DIGITAL_ATESTADO = 1;
            cf.CONF_IN_ASSINA_DIGITAL_SOLICITACAO = 1;
            cf.CONF_IN_ASSINA_DIGITAL_PRESCRICAO = 1;
            cf.CONF_IN_ASSINA_DIGITAL_LOCAL_PFX = null;
            cf.CONF_IN_MODELO_ANAMNESE = 1;
            cf.CONF_IN_AVISO_ESTOQUE = 1;
            cf.CONF_IN_ASSINA_DIGITAL_LOCACAO = 1;
            cf.CONF_VL_ACRESCIMO_ATRASO_PARCELA = 10;
            cf.CONF_NM_SENHA_PACIENTE = "a123456A@";
            cf.CONF_IN_RECIBO_SRF = 1;
            cf.CONF_IN_DOC_PRONTUARIO = 1;
            Int32 voltaCF = confApp.ValidateCreate(cf);

            // Encerra
            return idAss;
        }

        public Int32 AlterarAssinanteNormal(FaleConoscoViewModel fc)
        {
            // Recupera assinante
            FaleConoscoViewModel vm = fc;
            ASSINANTE assi = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1 & p.ASSI_IN_TIPO == 3 & p.ASSI_NM_NOME == vm.Nome & p.ASSI_NR_CPF == vm.CPF & p.ASSI_NR_CNPJ == vm.CNPJ).FirstOrDefault();

            // Alterar assinante
            assi.TIPE_CD_ID = vm.Tipo;
            assi.PLAN_CD_ID = 3;
            assi.ASSI_NM_NOME = vm.Nome;
            assi.ASSI_IN_ATIVO = 1;
            assi.ASSI_DT_INICIO = DateTime.Today.Date;
            if (vm.TipoAssinatura == 1)
            {
                assi.ASSI_IN_TIPO = 3;
            }
            else
            {
                assi.ASSI_IN_TIPO = 2;
            }
            assi.ASSI_IN_STATUS = 1;
            assi.ASSI_NM_EMAIL = vm.Resposta;
            assi.ASSI_NR_CNPJ = vm.CNPJ;
            assi.ASSI_NR_CPF = vm.CPF;
            assi.ASSI_NR_CELULAR = vm.Celular;
            assi.ASSI_IN_BLOQUEADO = 0;
            assi.ASSI_IN_VENCIDO = 0;
            assi.ASSI_IN_VENCIDO = 0;
            assi.ASSI_NM_BAIRRO = vm.Bairro;
            assi.ASSI_NM_CIDADE = vm.Cidade;
            assi.ASSI_NM_COMPLEMENTO = vm.Complemento;
            assi.ASSI_NM_ENDERECO = vm.Endereco;
            assi.ASSI_NM_RAZAO_SOCIAL = vm.Razao;
            assi.ASSI_NR_CEP = vm.CEPBase;
            assi.ASSI_NR_NUMERO = vm.Numero;
            assi.ASSI_NR_TELEFONE = vm.TelefoneFixo;
            assi.UF_CD_ID = vm.UF;
            assi.ASSI_AQ_FOTO = "~/Imagens/Base/icone_morador.png";
            Int32 voltaC = assiApp.ValidateEdit(assi);

            Int32 idAss = assi.ASSI_CD_ID;
            Session["idNovoAssinante"] = idAss;
            ASSINANTE assinante = assiApp.GetItemById(idAss);

            // Alterar empresa
            EMPRESA emp = empApp.GetItemByAssinante(idAss);
            emp.ASSI_CD_ID = idAss;
            emp.EMPR_AQ_LOGO = "~/Imagens/Base/prontuario_icon_3.png";
            emp.EMPR_DT_CADASTRO = DateTime.Today.Date;
            emp.EMPR_IN_ATIVO = 1;
            emp.EMPR_IN_MATRIZ = 1;
            emp.EMPR_NM_BAIRRO = vm.Bairro;
            emp.EMPR_NM_CIDADE = vm.Cidade;
            emp.EMPR_NM_COMPLEMENTO = vm.Complemento;
            emp.EMPR_NM_EMAIL = vm.Resposta;
            emp.EMPR_NM_ENDERECO = vm.Endereco;
            emp.EMPR_NM_GERENTE = "Nome";
            emp.EMPR_NM_GUERRA = "Nome";
            emp.EMPR_NM_NOME = vm.Nome;
            emp.EMPR_NM_NUMERO = vm.Numero;
            emp.EMPR_NM_RAZAO = vm.Razao;
            emp.EMPR_NR_CELULAR = vm.Celular;
            emp.EMPR_NR_CEP = vm.CEPBase;
            emp.EMPR_NR_CNPJ = vm.CNPJ;
            emp.EMPR_NR_CPF = vm.CPF;
            emp.EMPR_NR_TELEFONE = vm.TelefoneFixo;
            emp.RETR_CD_ID = 2;
            emp.TIPE_CD_ID = vm.Tipo;
            emp.UF_CD_ID = vm.UF;
            Int32 voltaE = empApp.ValidateEdit(emp, emp);

            // Recupera emmpresa
            EMPRESA empresa = empApp.GetItemById(emp.EMPR_CD_ID);

            // Altera Empresa/Filial
            EMPRESA_FILIAL emfi = empresa.EMPRESA_FILIAL.FirstOrDefault();
            emfi.ASSI_CD_ID = idAss;
            emfi.EMFI_AQ_LOGO = "~/Imagens/Base/prontuario_icon_3.png";
            emfi.EMFI_DT_CADASTRO = DateTime.Today.Date;
            emfi.EMFI_IN_ATIVO = 1;
            emfi.EMFI_IN_MATRIZ = 1;
            emfi.EMFI_NM_BAIRRO = vm.Bairro;
            emfi.EMFI_NM_CIDADE = vm.Cidade;
            emfi.EMFI_NR_COMPLEMENTO = vm.Complemento;
            emfi.EMFI_NM_EMAIL = vm.Resposta;
            emfi.EMFI_NM_ENDERECO = vm.Endereco;
            emfi.EMFI_NM_GERENTE = "Nome";
            emfi.EMFI_NM_APELIDO = "Nome";
            emfi.EMFI_NM_NOME = vm.Nome;
            emfi.EMFI_NR_NUMERO = vm.Numero;
            emfi.EMFI_NM_RAZAO = vm.Razao;
            emfi.EMFI_NR_CELULAR = vm.Celular;
            emfi.EMFI_NR_CEP = vm.CEPBase;
            emfi.EMFI_NR_CNPJ = vm.CNPJ;
            emfi.EMFI_NR_CPF = vm.CPF;
            emfi.EMFI_NR_TELEFONE = vm.Telefone;
            emfi.EMPR_CD_ID = empresa.EMPR_CD_ID;
            emfi.TIPE_CD_ID = vm.Tipo;
            emfi.UF_CD_ID = vm.UF;
            voltaE = empApp.ValidateEdit(empresa, empresa);

            // Recupera empresa
            empresa = empApp.GetItemById(emp.EMPR_CD_ID);

            // Login
            Session["LoginDemo"] = vm.LoginBase;

            // Senha
            String senha = vm.SenhaBase;
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(senha, salt);
            Session["SenhaDemo"] = senha;

            // Altera usuario master
            USUARIO usu = usuApp.GetAllItens(idAss).FirstOrDefault();
            usu.ASSI_CD_ID = idAss;
            usu.CAUS_CD_ID = 1;
            usu.EMFI_CD_ID = empresa.EMPRESA_FILIAL.First().EMFI_CD_ID;
            usu.EMPR_CD_ID = empresa.EMPR_CD_ID;
            usu.ESPE_CD_ID = espApp.GetAllItens(idAss).FirstOrDefault().ESPE_CD_ID;
            usu.PERF_CD_ID = perfApp.GetAllItens(idAss).FirstOrDefault().PERF_CD_ID;
            usu.TICL_CD_ID = 8;
            usu.USUA_AQ_FOTO = "~/Imagens/Base/icone_morador.png";
            usu.USUA_DT_ACESSO = DateTime.Today.Date;
            usu.USUA_DT_ALTERACAO = DateTime.Today.Date;
            usu.USUA_DT_CADASTRO = DateTime.Today.Date;
            usu.USUA_DT_TROCA_SENHA = DateTime.Today.Date;
            usu.USUA_IN_ATIVO = 1;
            usu.USUA_IN_BLOQUEADO = 0;
            usu.USUA_IN_ESPECIAL = 1;
            usu.USUA_IN_HUMANO = 1;
            usu.USUA_IN_LOGADO = 0;
            usu.USUA_IN_LOGIN_PROVISORIO = 0;
            usu.USUA_IN_PENDENTE_CODIGO = 0;
            usu.USUA_IN_PROVISORIO = 0;
            usu.USUA_IN_SISTEMA = 6;
            usu.USUA_IN_TECNICO = 0;
            usu.USUA_NM_APELIDO = vm.Nome.Substring(0, vm.Nome.IndexOf(" "));
            usu.USUA_NM_EMAIL = vm.Resposta;
            usu.USUA_NM_ESPECIALIDADE = vm.Especialidade;
            usu.USUA_NM_LOGIN = vm.LoginBase;
            usu.USUA_NM_NOME = vm.Nome;
            usu.USUA_NR_ACESSOS = 1;
            usu.USUA_NR_CELULAR = vm.Celular;
            usu.USUA_NR_CLASSE = null;
            usu.USUA_NR_CPF = vm.CPF;
            usu.USUA_NR_FALHAS = 0;
            usu.USUA_NR_TELEFONE = vm.Telefone;
            usu.USUA_NM_SENHA = hashedPassword;
            usu.USUA_NM_SALT = salt;
            usu.USUA_NM_SENHA_CONFIRMA = vm.SenhaBase;
            Int32 voltaUsu = usuApp.ValidateEdit(usu, usu);

            // Encerra
            return idAss;
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailDemo(MensagemViewModel vm)
        {
            // Recupera chaves
            CONFIGURACAO_CHAVES conf = confApp.GetAllChaves().FirstOrDefault();

            // Prepara texto
            String texto = String.Empty;
            texto = "<br />Seguem abaixo as credenciais para você acessar a demonstração do <b>WebDoctor</b>.<br />";
            texto += "Trata-se de uma assinatura REAL com todas as funcionalidades do plano mais completo do <b>WebDoctor</b>.<br />";
            texto += "Essa assinatura já vem com alguns dados fictícios para facilitar a visualização das funcionalidades.<br />";
            texto += "Você poderá incluir, alterar ou excluir novas informações em qualquer das funcionalidades do <b>WebDoctor</b>.<br />";
            texto += "Para melhor visualização das mensagens enviadas, crie inicialmente um paciente e informe os seus dados de contato (e-mail e celular). Assim você poderá receber e visualizar as mensagens enviadas.<br />";

            // Prepara informações
            String info = String.Empty;
            texto = "<br />Credenciais de acesso:<br />";
            texto += "<b>Link:</b> " + "https://eprontuario.azurewebsites.net/ <br />";
            texto += "<b>Login:</b>" + vm.LOGIN_DEMO + "<br />";
            texto += "<b>Senha:</b> a135701P#<br />";

            // Prepara rodape
            String rodape = String.Empty;
            rodape = "<br />Enviado por <b>Suporte WebDoctor</b><br />";
            rodape += "<b>E-Mail:</b> " + vm.MODELO + "<br />";
            rodape += "<b>WhatsApp:</b> " + vm.CELULAR + "<br />";

            // Prepara cabecalho 
            String dados = String.Empty;
            dados = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Solicitação de Demonstração</p><br />";
            dados += "<b>Nome do Solicitante: " + vm.NOME + "</b><br />";
            if (vm.MENS_NM_CABECALHO != null)
            {
                dados += "<b>CPF:</b> " + vm.MENS_NM_CABECALHO + "<br />";
            }
            if (vm.MENS_NM_ASSINATURA != null)
            {
                dados += "<b>CNPJ:</b> " + vm.MENS_NM_ASSINATURA + "<br />";
            }
            dados += "<b>Celular:</b> " + vm.CIDADE + "<br />";
            dados += "<b>E-Mail para Resposta: </b>" + vm.MENS_NM_RODAPE + "<br />";
            dados += "<b>Data da Solicitação: </b>" + DateTime.Today.Date.ToLongDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";

            // Monta corpo da mensagem            
            String corpo = dados + texto + info + rodape + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String status = "Succeeded";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String iD = "xyz";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MODELO;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            // Mensagem deenvio
            Session["MsgCRUD"] = "Solicitação de Demonstração de " + vm.NOME + " foi enviada com sucesso. Aguarde o e-mail com as credenciais de acesso";
            Session["MensFC"] = 888;
            return 0;
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailFaleConoscoInicio(MensagemViewModel vm, FaleConoscoViewModel fc)
        {
            // Recupera chaves
            CONFIGURACAO_CHAVES conf = confApp.GetAllChaves().FirstOrDefault();

            // Prepara corpo do e-mail para suporte
            String dados = String.Empty;
            dados = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Solicitação de Informação</p><br />";
            dados += "Nome do Solicitante: <b>" + fc.Nome + "</b><br />";
            dados += "Tipo de Resposta: <b>" + (fc.Tipo == 1 ? "E-Mail" : (fc.Tipo == 2 ? "Telefone" : "Celular")) + "</b><br />";
            if (fc.Tipo == 1)
            {
                dados += "E-Mail para Resposta: <b>" + fc.Resposta + "</b><br />";
            }
            if (fc.Tipo == 2)
            {
                dados += "E-Mail de Contato: <b>" + fc.Resposta + "</b><br />";
                dados += "Telefone para Resposta: <b>" + fc.TelefoneFixo + "</b><br />";
            }
            if (fc.Tipo == 3)
            {
                dados += "E-Mail de Contato: <b>" + fc.Resposta + "</b><br />";
                dados += "Celular para Resposta: <b>" + fc.Celular + "</b><br />";
            }
            dados += "Data da Solicitação: " + DateTime.Today.Date.ToLongDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
            dados += "Assunto: " + fc.NomePlano + "<br /><br />";
            String corpo = dados + vm.MENS_TX_TEXTO + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

            // Prepara corpo do e-mail para usuario
            String dados1 = String.Empty;
            dados1 = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Solicitação de Informação</p><br />";
            dados1 += "A sua solicitação de contato descrita abaixo foi enviada com sucesso.<br />";
            dados1 += "Logo a nossa equipe vai responder a sua solicitação na forma de resposta que você informou. É só aguardar.<br />";
            dados1 += "<br /><b>Dados da Solicitação:</b><br />";
            dados1 += "Nome do Solicitante: <b>" + vm.NOME + "</b><br />";
            dados1 += "Tipo de Resposta: <b>" + (fc.Tipo == 1 ? "E-Mail" : (fc.Tipo == 2 ? "Telefone" : "Celular")) + "</b><br />";
            if (fc.Tipo == 1)
            {
                dados1 += "E-Mail para Resposta: <b>" + fc.Resposta + "</b><br />";
            }
            if (fc.Tipo == 2)
            {
                dados1 += "E-Mail de Contato: <b>" + fc.Resposta + "</b><br />";
                dados1 += "Telefone para Resposta: <b>" + fc.TelefoneFixo + "</b><br />";
            }
            if (fc.Tipo == 3)
            {
                dados1 += "E-Mail de Contato: <b>" + fc.Resposta + "</b><br />";
                dados1 += "Celular para Resposta: <b>" + fc.Celular + "</b><br />";
            }
            dados1 += "Data da Solicitação: " + DateTime.Today.Date.ToLongDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
            dados1 += "Assunto: " + fc.NomePlano + "<br /><br />";
            String corpo1 = dados1 + vm.MENS_TX_TEXTO + "<br /><br />";
            corpo1 = corpo1.Replace("\r\n", "<br />");

#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String status = "Succeeded";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String iD = "xyz";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail para suporte
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = fc.NomePlano;
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MENS_NM_LINK;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            // Monta e-mail para usuario
            net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            mensagem = new EmailAzure();
            mensagem.ASSUNTO = fc.NomePlano;
            mensagem.CORPO = corpo1;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MODELO;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            // Mensagem deenvio
            Session["MsgCRUD"] = "Solicitação de " + vm.NOME + " foi enviada com sucesso.";
            Session["MensFC"] = 888;
            return 0;
        }

        [HttpGet]
        public ActionResult MontarTelaControleAcessoNova()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
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
                List<ACESSO_METODO> acessos = aceApp.GetAllItens(idAss);
                Session["Acessos"] = acessos;

                List<LOG_EXCECAO_NOVO> falhas = baseApp.GetAllLogExcecao(idAss);
                Session["Falhas"] = falhas;

                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<ModeloViewModel> listaAcessoDia = new List<ModeloViewModel>();
                List<ModeloViewModel> listaAcessoMes = new List<ModeloViewModel>();

                // Carrega listas de filtros
                List<USUARIO> usus = baseApp.GetAllItens(idAss);
                ViewBag.Usuarios = new SelectList(usus.OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");

                // Carrega widgets e grid
                List<ACESSO_METODO> acessosMes = acessos.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Date.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Date.Year).ToList();
                List<ACESSO_METODO> acessosDia = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                Session["AcessosMes"] = acessosMes;
                Session["AcessosDia"] = acessosDia;
                ViewBag.Acessos = acessos;
                ViewBag.AcessosMes = acessosMes;
                ViewBag.AcessosDia = acessosDia;
                ViewBag.AcessosConta = acessos.Count();
                ViewBag.AcessosMesConta = acessosMes.Count();
                ViewBag.AcessosDiaConta = acessosDia.Count();

                List<LOG_EXCECAO_NOVO> falhasMes = falhas.Where(p => p.LOEX_DT_DATA.Month == DateTime.Today.Date.Month & p.LOEX_DT_DATA.Year == DateTime.Today.Date.Year).ToList();
                List<LOG_EXCECAO_NOVO> falhasDia = falhas.Where(p => p.LOEX_DT_DATA.Date == DateTime.Today.Date).ToList();
                Session["FalhasMes"] = falhasMes;
                Session["FalhasDia"] = falhasDia;
                ViewBag.Falhas = falhas;
                ViewBag.FalhasMes = falhasMes;
                ViewBag.FalhasDia = falhasDia;
                ViewBag.FalhasConta = falhas.Count();
                ViewBag.FalhasMesConta = falhasMes.Count();
                ViewBag.FalhasDiaConta = falhasDia.Count();

                // Acessos por dia - Mes corrente
                List<DateTime> datas = acessos.Where(p => p.ACES_DT_ACESSO.Value.Month == DateTime.Today.Month & p.ACES_DT_ACESSO.Value.Year == DateTime.Today.Year).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
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
                datas = acessos.Where(p => p.ACES_DT_ACESSO.Value > limite).Select(p => p.ACES_DT_ACESSO.Value.Date).Distinct().ToList();
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
                    USUARIO usu = baseApp.GetItemById(item);
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTROLE_ACESSO", "BaseAdmin", "MontarTelaControleAcessoNova");
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<FaixaHoraViewModel> CarregarFaixas()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<FaixaHoraViewModel> conf = new List<FaixaHoraViewModel>();
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            TimeSpan? inicio = null;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            TimeSpan? final = null;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
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

        public JsonResult GetAcessosUsuarios()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaUsuario"];
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

        public List<MENSAGENS> CarregarMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            if (Session["Mensagens"] == null)
            {
                conf = mensApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["MensagemAlterada"] == 1)
                {
                    conf = mensApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<MENSAGENS>)Session["Mensagens"];
                }
            }
            Session["MensagemAlterada"] = 0;
            Session["Mensagens"] = conf;
            return conf;
        }

        public ActionResult VerTotalAcessos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 1;
            return RedirectToAction("MontarTelaTodosAcessos", "BaseAdmin");
        }

        public ActionResult VerTotalAcessosMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 2;
            return RedirectToAction("MontarTelaTodosAcessos", "BaseAdmin");
        }

        public ActionResult VerTotalAcessosData()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EscopoAcesso"] = 3;
            return RedirectToAction("MontarTelaTodosAcessos", "BaseAdmin");
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
                    acessos = CarregaAcessos();

                    // Restringe pelo perfil
                    ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                    if ((String)Session["PerfilSigla"] != "ADM")
                    {
                        acessos = acessos.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaAcessoTotal"] = acessos;
                }

                // Aplica filtros pre definidos
                acessos = (List<ACESSO_METODO>)Session["ListaAcessoTotal"];
                if ((Int32)Session["EscopoAcesso"] == 2)
                {
                    acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == DateTime.Today.Date.Month & p.ACES_DT_ACESSO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                }
                if ((Int32)Session["EscopoAcesso"] == 3)
                {
                    acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                }
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL", "BaseAdmin", "MontarTelaTodosAcessos");

                // Abre view
                Session["MensPaciente"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10.pdf";
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTodosAcessosMes()
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
                DateTime hoje = DateTime.Today.Date;
                List<ACESSO_METODO> acessos = new List<ACESSO_METODO>();
                if (Session["ListaAcessoTotal"] == null)
                {
                    acessos = CarregaAcessos().Where(p => p.ACES_DT_ACESSO.Value.Month == hoje.Month & p.ACES_DT_ACESSO.Value.Year == hoje.Year).ToList();

                    // Restringe pelo perfil
                    ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                    Session["ListaAcessoTotal"] = acessos;
                }

                // Aplica filtros pre definidos
                acessos = (List<ACESSO_METODO>)Session["ListaAcessoTotal"];
                if ((Int32)Session["EscopoAcesso"] == 2)
                {
                    acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date.Month == DateTime.Today.Date.Month & p.ACES_DT_ACESSO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                }
                if ((Int32)Session["EscopoAcesso"] == 3)
                {
                    acessos = acessos.Where(p => p.ACES_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList();
                }
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL", "BaseAdmin", "MontarTelaTodosAcessos");

                // Abre view
                Session["MensPaciente"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10.pdf";
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_USUARIOS", "BaseAdmin", "MontarTelaTodosAcessosUsuarios");

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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_FUNCOES", "BaseAdmin", "MontarTelaTodosAcessosFuncoes");

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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ACESSOS_TOTAL_FAIXAS", "BaseAdmin", "MontarTelaTodosAcessosFaixas");

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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<ACESSO_METODO> CarregaAcessos()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ACESSO_METODO> conf = new List<ACESSO_METODO>();
                conf = aceApp.GetAllItens(idAss);
                Session["Acessos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acessos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
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
                Tuple<Int32, List<ACESSO_METODO>, Boolean> volta = aceApp.ExecuteFilter(item.ASSI_CD_ID, item.USUA_CD_ID, item.ACES_DT_ACESSO, item.ACES_DT_DUMMY, item.ACES_SG_ACESSO, item.ACES_NM_CONTROLLER, item.ACES_NM_METHOD, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaTodosAcessos");
                }

                // Sucesso
                List<ACESSO_METODO> listaVolta = volta.Item2;
                if ((String)Session["PerfilSigla"] != "ADM")
                {
                    listaVolta = listaVolta.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "BaseAdmin", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaCompraBasico()
        {
            try
            {
                // Monta objeto
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
                List<SelectListItem> plano = new List<SelectListItem>();
                plano.Add(new SelectListItem() { Text = "Básico - R$ 2.000,00/Ano", Value = "18" });
                plano.Add(new SelectListItem() { Text = "Super - R$ 3.500,00/Ano", Value = "20" });
                plano.Add(new SelectListItem() { Text = "Super Financeiro - R$ 4.000,00/Ano", Value = "24" });
                ViewBag.Planos = new SelectList(plano, "Value", "Text");

                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 333)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0627", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 334)
                    {
                        String frase = (String)Session["FraseDemoVencido"];
                        ModelState.AddModelError("", frase);
                    }
                }

                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = "(21)97302-4096";
                fale.EMail = "suporte@rtiltda.net";
                fale.Resposta = String.Empty;
                fale.Nome = String.Empty;
                fale.Assunto = null;
                fale.Comeco = DateTime.Today.Date.ToLongDateString();
                fale.Fim = DateTime.Today.Date.AddYears(1).ToLongDateString();
                return View(fale);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaCompraBasico(FaleConoscoViewModel vm)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<SelectListItem> plano = new List<SelectListItem>();
            plano.Add(new SelectListItem() { Text = "Básico - R$ 2.000,00/Ano", Value = "18" });
            plano.Add(new SelectListItem() { Text = "Super - R$ 3.500,00/Ano", Value = "20" });
            plano.Add(new SelectListItem() { Text = "Super Financeiro - R$ 4.000,00/Ano", Value = "24" });
            ViewBag.Planos = new SelectList(plano, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if (vm.Plano == null || vm.Plano == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0621", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    else
                    {
                        vm.NomePlano = vm.Plano == 1 ? "Básico" : (vm.Plano == 2 ? "Super" : "Super - Financeiro");
                    }
                    if (vm.Tipo == null || vm.Tipo == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0611", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Nome == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0592", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Resposta == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0591", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Celular == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0613", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Tipo == 1)
                    {
                        if (vm.CPF == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0608", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.Tipo == 2)
                    {
                        if (vm.CNPJ == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0609", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Razao == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0612", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.CEP == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0614", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Endereco == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0615", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Numero == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0616", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Bairro == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0617", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Cidade == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0618", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.UF == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0619", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!ValidarItensDiversos.IsValidEmail(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica assinante
                    List<ASSINANTE> assList = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1).ToList();
                    ASSINANTE assBase = null;
                    ASSINANTE assBase1 = null;
                    Int32 novoPlano = 0;
                    if (vm.Tipo == 1)
                    {
                        assBase = assList.Where(p => p.ASSI_NR_CPF == vm.CPF).FirstOrDefault();
                        if (assBase != null)
                        {
                            assBase1 = assList.Where(p => p.ASSI_NR_CPF == vm.CPF & p.ASSINANTE_PLANO_ASSINATURA.FirstOrDefault().PLAS_CD_ID == vm.Plano).FirstOrDefault();
                            if (assBase1 == null)
                            {
                                novoPlano = 1;
                            }
                        }
                    }
                    else
                    {
                        assBase = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ).FirstOrDefault();
                        if (assBase != null)
                        {
                            assBase1 = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ & p.ASSINANTE_PLANO_ASSINATURA.FirstOrDefault().PLAS_CD_ID == vm.Plano).FirstOrDefault();
                            if (assBase1 == null)
                            {
                                novoPlano = 1;
                            }
                        }
                    }
                    if (assBase != null & novoPlano == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0628", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de login e Senha
                    if (vm.LoginBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0620", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0629", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0630", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma != vm.SenhaBase)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0631", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Length < 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0223", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(char.IsUpper) || !vm.SenhaBase.Any(char.IsLower) && !vm.SenhaBase.Any(char.IsDigit))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0224", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(p => !char.IsLetterOrDigit(p)))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Contains(vm.LoginBase) || vm.SenhaBase.Contains(vm.Nome) || vm.SenhaBase.Contains(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // verifica existencia login
                    if (usuApp.GetByLogin(vm.LoginBase, 1) != null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0633", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria/Altera assinante
                    if (novoPlano == 0)
                    {
                        // Cria assinante
                        vm.TipoAssinatura = 1;
                        Int32 voltaCria = CriarAssinanteNormal(vm);
                        vm.LoginFinal = (String)Session["LoginDemo"];
                        vm.Senha = (String)Session["SenhaDemo"];
                        if (voltaCria == 0)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0622", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    else
                    {
                        // Altera assinante
                        Int32 voltaAtl= AlterarAssinante(vm);
                        if (voltaAtl == 1)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0634", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (voltaAtl == 2)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0635", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Prepara mensagem
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = vm.Nome;
                    mens.ID = null;
                    mens.MODELO = vm.Resposta;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = String.Empty;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = vm.Nome;
                    mens.MENS_NM_CAMPANHA = "Contratação/Alteração de Assinatura";
                    mens.MENS_NM_RODAPE = vm.Resposta;
                    mens.MENS_NM_LINK = "https://eprontuario.azurewebsites.net/";
                    mens.CELULAR = vm.Telefone;
                    mens.MENS_NM_CABECALHO = vm.CPF;
                    mens.MENS_NM_ASSINATURA = vm.CNPJ;
                    mens.CIDADE = vm.Celular;
                    mens.MENS_IN_CRM = novoPlano;
                    await ProcessaEnvioEMailCompra(mens, vm);

                    // Sucesso
                    Session["MensFC"] = 333;
                    return RedirectToAction("MontarTelaCompraBasico");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return View(vm);
            }
        }

        public Int32 AlterarAssinante(FaleConoscoViewModel vm)
        {
            // Recupera assinante
            ASSINANTE assBase = null;
            List<ASSINANTE> assList = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1).ToList();
            if (vm.Tipo == 1)
            {
                assBase = assList.Where(p => p.ASSI_NR_CPF == vm.CPF).FirstOrDefault();
                if (assBase == null)
                {
                    return 1;
                }
            }
            else
            {
                assBase = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ).FirstOrDefault();
                if (assBase == null)
                {
                    return 1;
                }
            }

            // Recupera plano ativo
            List<ASSINANTE_PLANO_ASSINATURA> planos = assBase.ASSINANTE_PLANO_ASSINATURA.ToList();
            ASSINANTE_PLANO_ASSINATURA plano = planos.Where(p => p.ASPA_IN_ATIVO == 1 & p.ASPA_IN_SISTEMA == 6).FirstOrDefault();

            // Verifica o plano
            if (vm.Plano == plano.PLAS_CD_ID)
            {
                return 2;
            }

            // Desativa plano antigo
            plano.ASPA_IN_ATIVO = 0;
            Int32 voltaPlan = assiApp.ValidateEditPlanoAss(plano);

            // Cria novo plano
            ASSINANTE_PLANO_ASSINATURA novo = new ASSINANTE_PLANO_ASSINATURA();
            novo.ASPA_DT_INICIO = DateTime.Today.Date;
            novo.ASPA_DT_VALIDADE = DateTime.Today.Date.AddYears(1);
            novo.ASPA_IN_ATIVO = 1;
            novo.ASPA_IN_SISTEMA = 6;
            novo.ASSI_CD_ID = assBase.ASSI_CD_ID;
            novo.PLAS_CD_ID = vm.Plano.Value;
            if (vm.Plano == 18)
            {
                novo.ASPA_IN_PRECO = 2000;
            }
            else if (vm.Plano == 20)
            {
                novo.ASPA_IN_PRECO = 3500;
            }
            else
            {
                novo.ASPA_IN_PRECO = 4000;
            }
            voltaPlan = assiApp.ValidateCreatePlanoAss(novo);
            return 0;
        }



        public JsonResult GetLogin(String cpf)
        {
            ASSINANTE assinante = null;
            if (cpf.Length == 14)
            {
                assinante = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1 & p.ASSI_NR_CPF == cpf).FirstOrDefault();
            }
            else
            {
                assinante = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1 & p.ASSI_NR_CNPJ == cpf).FirstOrDefault();
            }

            var hash = new Hashtable();
            hash.Add("cnpj", assinante.ASSI_NR_CNPJ);
            hash.Add("cpf", assinante.ASSI_NR_CPF);
            hash.Add("nome", assinante.ASSI_NM_NOME);
            hash.Add("razao", assinante.ASSI_NM_RAZAO_SOCIAL);
            hash.Add("mail", assinante.ASSI_NM_EMAIL);
            hash.Add("tel", assinante.ASSI_NR_TELEFONE);
            hash.Add("cel", assinante.ASSI_NR_CELULAR);
            hash.Add("cep", assinante.ASSI_NR_CEP);
            hash.Add("endereco", assinante.ASSI_NM_ENDERECO);
            hash.Add("nmr", assinante.ASSI_NR_NUMERO);
            hash.Add("complemento", assinante.ASSI_NM_COMPLEMENTO);
            hash.Add("bairro", assinante.ASSI_NM_BAIRRO);
            hash.Add("cidade", assinante.ASSI_NM_CIDADE);
            hash.Add("uf", assinante.UF_CD_ID);
            return Json(hash);
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailCompra(MensagemViewModel vm, FaleConoscoViewModel fc)
        {
            // Recupera chaves
            CONFIGURACAO_CHAVES conf = confApp.GetAllChaves().FirstOrDefault();
            Int32? acao = vm.MENS_IN_CRM;

            // Prepara texto
            String texto = String.Empty;
            if (fc.TipoAssinatura == 1)
            {
                if (acao == 0)
                {
                    texto = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Contratação de Assinatura</p><br />";
                    texto += "Sua assinatura do plano <b>" + fc.NomePlano + "</b> do <b>WebDoctorPro</b> foi criada com sucesso.<br />";
                    texto += "Estamos apenas esperando a confirmação de seu pagamento. Assim que for confirmado você receberá mensagem avisando que está pronto para utilização<br />";
                    texto += "<br />Seguem abaixo as credenciais para você acessar o <b>WebDoctorPro</b>.<br />";
                }
                else
                {
                    texto = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Contratação de Assinatura</p><br />";
                    texto += "Sua assinatura foi migrada com sucesso para o plano <b>" + fc.NomePlano + "</b> do <b>WebDoctorPro</b><br />";
                    texto += "Estamos apenas esperando a confirmação de seu pagamento. Assim que for confirmado você receberá mensagem avisando que está pronto para utilização<br />";
                    texto += "<br />As suas credenciais para você acessar o <b>WebDoctorPro</b> permanecem as mesmas<br />";
                }
            }
            else
            {
                texto = "<p style='background-color: darkseagreen; font-size: 24px; font-weight: bold; color: darkgreen'>Solicitação de Assinatura de Demonstração</p><br />";
                texto += "Sua solicitação de assinatura de demonstração <b>WebDoctorPro</b> foi criada com sucesso.<br />";
                texto += "A assinatura de demonstração contempla o plano <b>Super - Financeiro</b> com todas as funcionalidades disponíveis e nenhum limite de utilização<br />";
                texto += "A assinatura de demonstração ficará ativa por 30 dias a contar de <b>" + DateTime.Today.Date.ToLongDateString() + "</b><br />";
                texto += "<br />Seguem abaixo as credenciais para você acessar o <b>WebDoctorPro</b>.<br />";
            }

            // Prepara informações
            String info = String.Empty;
            if (acao == 0)
            {
                info = "<br />Credenciais de acesso:<br />";
                info += "<b>Link: </b> " + "https://webdoctorpro.net/ <br />";
                info += "<b>Login: </b>" + fc.LoginBase + "<br />";
                info += "<b>Senha: </b>" + fc.SenhaBase + "<br />";
            }

            // Prepara rodape
            String rodape = String.Empty;
            rodape = "<br />Enviado por <b>Suporte WebDoctorPro</b><br />";
            rodape += "<b>E-Mail: </b> suporte@rtiltda.net<br />";
            rodape += "<b>WhatsApp: </b>(21)97302-4096<br />";

            // Prepara cabecalho 
            String dados = String.Empty;
            dados = "<b>Nome do Solicitante: " + vm.NOME + "</b><br />";
            if (vm.MENS_NM_CABECALHO != null)
            {
                dados += "<b>CPF:</b> " + vm.MENS_NM_CABECALHO + "<br />";
            }
            if (vm.MENS_NM_ASSINATURA != null)
            {
                dados += "<b>CNPJ:</b> " + vm.MENS_NM_ASSINATURA + "<br />";
            }
            dados += "<b>Celular: </b> " + vm.CIDADE + "<br />";
            dados += "<b>E-Mail para Resposta: </b>" + vm.MENS_NM_RODAPE + "<br />";
            dados += "<b>Data da Assinatura: </b>" + DateTime.Today.Date.ToLongDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
            if (fc.TipoAssinatura == 1)
            {
                dados += "<b>Validade da Assinatura: </b>" + DateTime.Today.Date.AddYears(1).ToLongDateString() + "<br />";
            }
            else
            {
                dados += "<b>Validade da Assinatura: </b>" + DateTime.Today.Date.AddDays(30).ToLongDateString() + "<br />";
            }

            // Monta corpo da mensagem            
            String corpo = texto + dados + info + rodape + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

            String status = "Succeeded";
            String iD = "xyz";
            String erro = null;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = vm.MENS_NM_CAMPANHA;
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = vm.MODELO;
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            // Mensagem para a RTi
            net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            mensagem = new EmailAzure();
            if (fc.TipoAssinatura == 1)
            {
                mensagem.ASSUNTO = "Contratação/Alteração de Assinatura";
            }
            else
            {
                mensagem.ASSUNTO = "Solicitação de Demonstração de Assinatura";

            }
            mensagem.CORPO = corpo;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = "suporte@rtiltda.net";
            mensagem.NOME_EMISSOR_AZURE = emissor;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = vm.NOME;
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
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                erro = ex.Message;
                throw;
            }

            return 0;
        }

        public void Sitemap()
        {
            Response.ContentType = "application/xml";
            Response.ContentEncoding = Encoding.UTF8;

            // Exemplo de URLs estáticas - adicione suas URLs dinâmicas aqui!
            var urls = new List<string>
            {
                Url.Action("MontarTelaPaciente", "Paciente", null, Request.Url.Scheme),
                Url.Action("MontarTelaCentralPaciente", "Paciente", null, Request.Url.Scheme),
                Url.Action("IncluirPaciente", "Paciente", null, Request.Url.Scheme)
            };

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    urls.ConvertAll(url =>
                        new XElement(ns + "url",
                            new XElement(ns + "loc", url),
                            new XElement(ns + "changefreq", "weekly"),
                            new XElement(ns + "priority", "0.8")
                        )
                    )
                )
            );

            // Escreve diretamente no output da resposta HTTP
            Response.Write(sitemap.ToString());
        }

        [HttpGet]
        public ActionResult MontarTelaCompraBasicoNova()
        {
            try
            {
                // Monta Listas
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
                List<PLANO_ASSINATURA> planos = assiApp.GetAllPlanosAssinatura();
                ViewBag.PlanosAss = new SelectList(planos, "PLAS_CD_ID", "PLAS_NM_EXIBE");

                // Mensagens
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 333)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0627", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 334)
                    {
                        String frase = (String)Session["FraseDemoVencido"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 888)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0644", CultureInfo.CurrentCulture);
                        frase += " " + (String)Session["ErroPagto"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Monta objeto
                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                if ((FaleConoscoViewModel)Session["CompraState"] != null)
                {
                    fale = (FaleConoscoViewModel)Session["CompraState"];
                    fale.Telefone = "(21)97302-4096";
                    fale.EMail = "suporte@rtiltda.net";
                    Session["TestaUsuario"] = 0;
                    Session["PlanoSelec"] = fale.Plano;
                }
                else
                {
                    fale = new FaleConoscoViewModel();
                    fale.Telefone = "(21)97302-4096";
                    fale.EMail = "suporte@rtiltda.net";
                    fale.Resposta = String.Empty;
                    fale.Nome = String.Empty;
                    fale.Assunto = null;
                    fale.Comeco = DateTime.Today.Date.ToLongDateString();
                    fale.Fim = DateTime.Today.Date.AddYears(1).ToLongDateString();
                    Session["TestaUsuario"] = 1;
                }
                return View(fale);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaCompraBasicoNova(FaleConoscoViewModel vm)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Pessoa Física", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Pessoa Jurídica", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.UF = new SelectList(empApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            List<PLANO_ASSINATURA> planos = assiApp.GetAllPlanosAssinatura();
            ViewBag.PlanosAss = new SelectList(planos, "PLAS_CD_ID", "PLAS_NM_EXIBE");

            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if ((FaleConoscoViewModel)Session["CompraState"] != null)
                    {
                        vm.Plano = (Int32)Session["PlanoSelec"];
                    }
                    if (vm.Plano == null || vm.Plano == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0621", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    else
                    {
                        PLANO_ASSINATURA plas = assiApp.GetPlanoAssinaturaById(vm.Plano.Value);
                        vm.NomePlano = plas.PLAS_NM_NOME;
                        vm.Preco = plas.PLAS_VL_PRECO;
                    }
                    if (vm.Tipo == null || vm.Tipo == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0611", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    vm.TipoPessoa = vm.Tipo == 1 ? "Pessoa Física" : "Pessoa Jurídica";
                    if (vm.Nome == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0592", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Resposta == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0591", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Celular == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0613", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Tipo == 1)
                    {
                        if (vm.CPF == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0608", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.Tipo == 2)
                    {
                        if (vm.CNPJ == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0609", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.Razao == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0612", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.CEP == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0614", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Endereco == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0615", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Numero == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0616", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Bairro == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0617", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Cidade == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0618", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.UF == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0619", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    vm.UFSigla = assiApp.GetAllUF().Where(p => p.UF_CD_ID == vm.UF).FirstOrDefault().UF_SG_SIGLA;
                    if (!ValidarItensDiversos.IsValidEmail(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica assinante
                    ASSINANTE assBase = null;
                    ASSINANTE assBase1 = null;
                    Int32 novoPlano = 0;
                    if ((Int32)Session["TestaUsuario"] == 1)
                    {
                        List<ASSINANTE> assList = assiApp.GetAllItens().Where(p => p.ASSI_IN_ATIVO == 1).ToList();
                        if (vm.Tipo == 1)
                        {
                            assBase = assList.Where(p => p.ASSI_NR_CPF == vm.CPF).FirstOrDefault();
                            if (assBase != null)
                            {
                                assBase1 = assList.Where(p => p.ASSI_NR_CPF == vm.CPF & p.ASSINANTE_PLANO_ASSINATURA.FirstOrDefault().PLAS_CD_ID == vm.Plano).FirstOrDefault();
                                if (assBase1 == null)
                                {
                                    novoPlano = 1;
                                }
                            }
                        }
                        else
                        {
                            assBase = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ).FirstOrDefault();
                            if (assBase != null)
                            {
                                assBase1 = assList.Where(p => p.ASSI_NR_CNPJ == vm.CNPJ & p.ASSINANTE_PLANO_ASSINATURA.FirstOrDefault().PLAS_CD_ID == vm.Plano).FirstOrDefault();
                                if (assBase1 == null)
                                {
                                    novoPlano = 1;
                                }
                            }
                        }
                        if (assBase != null & novoPlano == 0)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0628", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Critica de login e Senha
                    if (vm.LoginBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0620", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0629", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0630", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBaseConfirma != vm.SenhaBase)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0631", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Length < 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0223", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(char.IsUpper) || !vm.SenhaBase.Any(char.IsLower) && !vm.SenhaBase.Any(char.IsDigit))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0224", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (!vm.SenhaBase.Any(p => !char.IsLetterOrDigit(p)))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SenhaBase.Contains(vm.LoginBase) || vm.SenhaBase.Contains(vm.Nome) || vm.SenhaBase.Contains(vm.Resposta))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // verifica existencia login
                    USUARIO usuLogin = usuApp.GetByLogin(vm.LoginBase, 1);
                    if (usuLogin != null)
                    {
                        if (usuLogin.USUA_NM_NOME != vm.Nome)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0633", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Salva estado
                    Session["CompraState"] = vm;
                    Session["idNovoAssinante"] = vm;

                    // Cria/altera assinante
                    vm.TipoAssinatura = 1;
                    if ((Int32)Session["TestaUsuario"] == 1)
                    {
                        Int32 voltaCria = CriarAssinanteNormal(vm);

                        // Prepara mensagem
                        MensagemViewModel mens = new MensagemViewModel();
                        mens.NOME = vm.Nome;
                        mens.ID = null;
                        mens.MODELO = vm.Resposta;
                        mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                        mens.MENS_IN_TIPO = 1;
                        mens.MENS_TX_TEXTO = String.Empty;
                        mens.MENS_NM_LINK = null;
                        mens.MENS_NM_NOME = vm.Nome;
                        mens.MENS_NM_CAMPANHA = "Contratação de Assinatura";
                        mens.MENS_NM_RODAPE = vm.Resposta;
                        mens.MENS_NM_LINK = "https://eprontuario.azurewebsites.net/";
                        mens.CELULAR = vm.Telefone;
                        mens.MENS_NM_CABECALHO = vm.CPF;
                        mens.MENS_NM_ASSINATURA = vm.CNPJ;
                        mens.CIDADE = vm.Celular;
                        mens.MENS_IN_CRM = novoPlano;
                        await ProcessaEnvioEMailCompra(mens, vm);
                    }
                    else
                    {
                        Int32 voltaCria = AlterarAssinanteNormal(vm);
                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaCheckoutNova");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaCheckoutNova()
        {
            try
            {
                // Monta objeto
                FaleConoscoViewModel fale = (FaleConoscoViewModel)Session["CompraState"];

                // Monta informações
                String info = String.Empty;
                info = "******** Informações da Assinatura ********" + "\r\n" + "\r\n";
                info = "---- Identificação do Assinante ----" + "\r\n";
                info += "Nome do Assinante: " + fale.Nome + "\r\n";
                if (fale.Tipo == 1)
                {
                    info += "CPF: " + fale.CPF + "\r\n";
                }
                else
                {
                    info += "Razão Social: " + fale.Razao + "\r\n";
                    info += "CNPJ: " + fale.CNPJ + "\r\n";
                }
                info += "Login: " + fale.LoginBase + "\r\n";

                info +=  "\r\n" + "---- Plano de Assinantura ----" + "\r\n";
                info += "Nome do Plano: " + fale.NomePlano + "\r\n";
                info += "Início: " + fale.Comeco + "\r\n";
                info += "Término: " + fale.Fim + "\r\n";
                info += "Valor: R$ " + CrossCutting.Formatters.DecimalFormatter(fale.Preco) + "\r\n";

                info += "\r\n" + "---- Contatos ----" + "\r\n";
                info += "E-Mail: " + fale.Resposta + "\r\n";
                info += "Celular: " + fale.Celular + "\r\n";
                if (fale.TelefoneFixo != null)
                {
                    info += "Telefone: " + fale.TelefoneFixo + "\r\n";
                }

                info += "\r\n" + "---- Endereço ----" + "\r\n";
                info += "Endereço: " + fale.Endereco + "\r\n";
                info += "Número: " + fale.Numero + "\r\n";
                if (fale.Complemento != null)
                {
                    info += "Complemento: " + fale.Complemento + "\r\n";
                }
                info += "Bairro: " + fale.Bairro + "\r\n";
                info += "Cidade: " + fale.Cidade + "\r\n";
                info += "UF: " + fale.UFSigla + "\r\n";
                info += "CEP: " + fale.CEP + "\r\n";
                fale.Informacoes = info;

                // Mensagens
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 333)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0627", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 334)
                    {
                        String frase = (String)Session["FraseDemoVencido"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 888)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0644", CultureInfo.CurrentCulture);
                        frase += " " + (String)Session["ErroPagto"];
                        ModelState.AddModelError("", frase);
                    }
                }

                fale.Telefone = "(21)97302-4096";
                fale.EMail = "suporte@rtiltda.net";
                return View(fale);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<ActionResult> MontarTelaCheckout()
        {
            try
            {
                // Mensagens
                if (Session["MensFC"] != null)
                {
                    if ((Int32)Session["MensFC"] == 333)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0627", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensFC"] == 334)
                    {
                        String frase = (String)Session["FraseDemoVencido"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Recupera informações
                FaleConoscoViewModel fc = (FaleConoscoViewModel)Session["InfoCheckout"];
                CONFIGURACAO_CHAVES conf = confApp.GetAllChaves().FirstOrDefault();
                return View(fc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> MontarTelaCheckout(FaleConoscoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Cria assinante
                    vm.TipoAssinatura = 1;
                    Int32 voltaCria = CriarAssinanteNormal(vm);
                    vm.LoginFinal = (String)Session["LoginDemo"];
                    vm.Senha = (String)Session["SenhaDemo"];
                    if (voltaCria == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0622", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Prepara mensagem
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = vm.Nome;
                    mens.ID = null;
                    mens.MODELO = vm.Resposta;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = String.Empty;
                    mens.MENS_NM_LINK = null;
                    mens.MENS_NM_NOME = vm.Nome;
                    mens.MENS_NM_CAMPANHA = "Contratação/Alteração de Assinatura";
                    mens.MENS_NM_RODAPE = vm.Resposta;
                    mens.MENS_NM_LINK = "https://eprontuario.azurewebsites.net/";
                    mens.CELULAR = vm.Telefone;
                    mens.MENS_NM_CABECALHO = vm.CPF;
                    mens.MENS_NM_ASSINATURA = vm.CNPJ;
                    mens.CIDADE = vm.Celular;
                    mens.MENS_IN_CRM = vm.Plano;
                    await ProcessaEnvioEMailCompra(mens, vm);

                    // Sucesso
                    Session["MensFC"] = 333;
                    return RedirectToAction("MontarTelaCompraBasico");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return View(vm);
            }
        }

        public async Task<ActionResult> IniciarPagamento()
        {
            try
            {
                // 1. Obter credenciais
                String pagBankEmail = System.Configuration.ConfigurationManager.AppSettings["PagBankEmail"];
                String pagBankToken = System.Configuration.ConfigurationManager.AppSettings["PagBankToken"];
                String pagBankTokenSandBox = System.Configuration.ConfigurationManager.AppSettings["PagBankTokenSandBox"];
                PagamentoPagBankViewModel model = (PagamentoPagBankViewModel)Session["DadosPagto"];
                String valor = model.ValorTotal.ToString().Replace(",", ".");

                // 2. Construir o XML da requisição
                // Este XML é um exemplo básico e precisa ser adaptado à documentação oficial do PagBank
                // da API de Checkout. Consulte a documentação para a estrutura exata.

                // Exemplo de como o XML pode ser construído (Checkout Padrão - Pagamentos)
                String celular = Regex.Replace(model.CelularCliente, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                String area = celular.Substring(0, 2);
                String numero = celular.Substring(2);
                String msg = Url.Action("PagamentoConcluido", "BaseAdmin", null, Request.Url.Scheme);

                // Escreve no log configurado
                Trace.WriteLine($"URL de acesso:");
                Trace.TraceInformation($"Informação: URL {msg} .");

                XElement paymentRequest = new XElement("checkout",
                                    new XElement("mode", "default"), // Ou "direct" para Checkout Transparente
                    new XElement("currency", "BRL"),
                    new XElement("items",
                        new XElement("item",
                            new XElement("id", "0001"),
                            new XElement("description", model.DescricaoItem),
                            new XElement("amount", valor), // Formato decimal com .
                            new XElement("quantity", model.QuantidadeItem.ToString())
                        )
                    ),
                    new XElement("sender",
                        new XElement("name", model.NomeCliente),
                        new XElement("email", model.EMailCliente),
                        new XElement("phone",
                            new XElement("areaCode", area),
                            new XElement("number", numero)
                        ),
                        new XElement("document",
                            new XElement("type", "CPF"),
                            new XElement("value", model.CpfCliente.Replace(".", "").Replace("-", "")) // Apenas números
                        )
                    ),
                    new XElement("redirectURL", Url.Action("PagamentoConcluido", "BaseAdmin", null, Request.Url.Scheme)), // URL de retorno
                    new XElement("notificationURL", Url.Action("NotificacaoPagBank", "BaseAdmin", null, Request.Url.Scheme)), // URL para notificações de status
                    new XElement("acceptedPaymentMethods",
                        new XElement("method",
                            new XElement("name", "CREDIT_CARD") 
                        )
                        //new XElement("method",
                        //    new XElement("group", "PIX")
                        //)
                    )
                //new XElement("accept", 
                //    new XElement("paymentMethod",
                //        new XElement("group", "CREDIT_CARD"),
                //        new XElement("group", "PIX")
                //    )
                //)
                // Para aceitar todas as formas, basta não enviar o <accept> ou <exclude>
                );

                String requestXml = paymentRequest.ToString();

                // 3. Enviar a requisição HTTP POST para a API do PagBank
                using (var client = new HttpClient())
                {
                    // URL da API de Checkout (Sandbox ou Produção)
                    String apiUrl = "https://ws.pagseguro.uol.com.br/v2/checkout/"; // URL de produção
                    //String apiUrl = "https://ws.sandbox.pagseguro.uol.com.br/v2/checkout/"; // URL de sandbox (testes)

                    var content = new StringContent(requestXml, Encoding.UTF8, "application/xml");

                    // Adicionar os parâmetros de credenciais na URL
                    var uriBuilder = new UriBuilder(apiUrl);
                    var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["email"] = pagBankEmail;
                    query["token"] = pagBankToken;
                    uriBuilder.Query = query.ToString();

                    HttpResponseMessage response = await client.PostAsync(uriBuilder.Uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        String responseContent = await response.Content.ReadAsStringAsync();
                        // 4. Processar a resposta do PagBank
                        XDocument doc = XDocument.Parse(responseContent);
                        String checkoutCode = doc.Element("checkout").Element("code").Value;

                        // Redirecionar o usuário para o PagBank para finalizar o pagamento
                        String redirectUrl = $"https://pagseguro.uol.com.br/v2/checkout/payment.html?code={checkoutCode}";
                        //string redirectUrl = $"https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html?code={checkoutCode}"; // Sandbox
                        return Redirect(redirectUrl);
                    }
                    else
                    {
                        // Tratar erro na comunicação com a API do PagBank
                        String errorContent = await response.Content.ReadAsStringAsync();
                        String erro = "Ocorreu um erro ao iniciar o pagamento: " + errorContent;
                        Session["ErroPagto"] = erro;
                        Session["MensFC"] = 888;
                        return View("MontarTelaCompraBasicoNova");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        // Ação definida na redirectURL
        public async Task<ActionResult> PagamentoConcluido(string transaction_id, string code)
        {
            try
            {
                // Recupera credenciais
                String pagBankEmail = ConfigurationManager.AppSettings["PagBankEmail"];
                String pagBankToken = ConfigurationManager.AppSettings["PagBankToken"];

                if (!string.IsNullOrEmpty(transaction_id))
                {
                    // Se o PagBank retornou o transaction_id diretamente
                    // Isso geralmente acontece no Checkout Transparente ou em alguns fluxos do Checkout Padrão.
                    // É a forma mais direta de consultar.
                    var status = await ConsultarStatusTransacao(transaction_id, pagBankEmail, pagBankToken);
                    ViewBag.StatusPagamento = status;
                    ViewBag.Mensagem = "Verificando o status do seu pagamento...";
                    ViewBag.TipoRetorno = "ID de Transação";
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    // Se o PagBank retornou um código de checkout/notificação (mais comum no Checkout Padrão)
                    // Você pode tentar consultar a transação por este código, mas o ideal é aguardar a notificação.
                    // Para ser preciso, o "code" na redirectURL após um checkout padrão normalmente não é um notificationCode,
                    // mas sim o code da transação que foi gerada.
                    // No entanto, para fins de demonstração, vamos considerar que um "code" aqui poderia ser usado para consulta.
                    // Atenção: A consulta por um "code" direto na API de Transações é mais complexa,
                    // pois o `code` de retorno da URL não é necessariamente o `transactionCode`.
                    // A melhor prática aqui é apenas exibir uma mensagem e aguardar a Notificação.
                    ViewBag.StatusPagamento = "Aguardando Notificação";
                    ViewBag.Mensagem = "Seu pedido foi recebido. Estamos aguardando a confirmação do pagamento pelo PagBank. Você será notificado por e-mail sobre o status.";
                    ViewBag.TipoRetorno = "Código de Checkout";

                    // Se você REALMENTE precisar consultar aqui, o PagBank tem uma API de consulta por código de referência
                    // ou pela Transaction API com o código. Mas é um pouco mais complexo de fazer direto.
                    // O `code` que vem na URL de retorno do Checkout Padrão é o código de referência da transação no PagBank.
                    // Exemplo de consulta (requer implementação da função de consulta por reference/code):
                    // var status = await ConsultarStatusPorCodigoOuReferencia(code, pagBankEmail, pagBankToken);
                    // ViewBag.StatusPagamento = status;
                }
                else
                {
                    // O PagBank redirecionou, mas sem parâmetros úteis.
                    ViewBag.StatusPagamento = "Desconhecido";
                    ViewBag.Mensagem = "Seu pedido foi recebido. Estamos aguardando a confirmação do pagamento pelo PagBank. Você será notificado por e-mail sobre o status.";
                    ViewBag.TipoRetorno = "Nenhum Parâmetro";
                }

                // Renderiza a view para o usuário
                FaleConoscoViewModel vm = (FaleConoscoViewModel)Session["InfoCheckout"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        // Função para consultar o status da transação via API
        private async Task<String> ConsultarStatusTransacao(String transactionCode, String email, String token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // URL da API de consulta de transações
                    String apiUrl = $"https://ws.pagseguro.uol.com.br/v2/transactions/{transactionCode}?email={email}&token={token}";
                    // string apiUrl = $"https://ws.sandbox.pagseguro.uol.com.br/v2/transactions/{transactionCode}?email={email}&token={token}"; // Sandbox

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        String content = await response.Content.ReadAsStringAsync();
                        XDocument doc = XDocument.Parse(content);
                        // Extrair o status do XML. Os status são numéricos (1=Aguardando, 3=Paga, etc.)
                        var statusElement = doc.Element("transaction")?.Element("status");
                        if (statusElement != null)
                        {
                            // Mapeie o código numérico para um texto mais amigável
                            return MapPagBankStatus(statusElement.Value);
                        }
                        return "Status não encontrado";
                    }
                    else
                    {
                        // Tratar erro na consulta
                        String errorContent = await response.Content.ReadAsStringAsync();
                        return $"Erro ao consultar: {response.StatusCode} - {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                throw ex;
            }
        }

        // Função auxiliar para mapear códigos de status do PagBank para texto
        private String MapPagBankStatus(String statusCode)
        {
            switch (statusCode)
            {
                case "1": return "Aguardando Pagamento";
                case "2": return "Em Análise";
                case "3": return "Paga";
                case "4": return "Disponível";
                case "5": return "Em Disputa";
                case "6": return "Devolvida";
                case "7": return "Cancelada";
                case "8": return "Chargeback Debitado";
                case "9": return "Em Contestação";
                default: return "Status Desconhecido";
            }
        }

         [HttpPost] // O PagBank sempre envia notificações via POST
        public async Task<ActionResult> NotificacaoPagBank()
        {
            try
            {
                // 1. Capturar os parâmetros enviados pelo PagBank
                // O PagBank envia 'notificationCode' e 'notificationType' no corpo da requisição POST
                string notificationCode = Request.Form["notificationCode"];
                string notificationType = Request.Form["notificationType"];

                // Logar para depuração (opcional, mas recomendado em ambiente de desenvolvimento)
                System.Diagnostics.Debug.WriteLine($"Notificação PagBank recebida: Code={notificationCode}, Type={notificationType}");

                // 2. Validar se os parâmetros essenciais foram recebidos
                if (string.IsNullOrEmpty(notificationCode) || string.IsNullOrEmpty(notificationType))
                {
                    // Se os parâmetros estiverem ausentes, o PagBank pode tentar reenviar.
                    // Retorne 200 OK para evitar que ele continue tentando.
                    // Você pode logar este evento para investigar.
                    System.Diagnostics.Debug.WriteLine("Notificação PagBank recebida sem notificationCode ou notificationType.");
                    return new HttpStatusCodeResult(200);
                }

                // 3. Obter as credenciais de acesso (Email e Token)
                string pagBankEmail = ConfigurationManager.AppSettings["PagBankEmail"];
                string pagBankToken = ConfigurationManager.AppSettings["PagBankToken"];

                // 4. Fazer uma requisição GET para a API de Notificações do PagBank
                // para obter os detalhes completos da transação.
                using (var client = new HttpClient())
                {
                    // URL da API de Notificações (use a URL de produção ou sandbox conforme seu ambiente)
                    string apiUrl = $"https://ws.pagseguro.uol.com.br/v2/transactions/notifications/{notificationCode}?email={pagBankEmail}&token={pagBankToken}";
                    // string apiUrl = $"https://ws.sandbox.pagseguro.uol.com.br/v2/transactions/notifications/{notificationCode}?email={pagBankEmail}&token={pagBankToken}"; // Para ambiente de testes (Sandbox)

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        XDocument doc = XDocument.Parse(content);

                        // 5. Extrair informações relevantes do XML de resposta
                        // Consulte a documentação do PagBank para a estrutura completa do XML de retorno.
                        string transactionCode = doc.Element("transaction")?.Element("code")?.Value; // Código da transação no PagBank
                        string transactionStatus = doc.Element("transaction")?.Element("status")?.Value; // Status numérico da transação
                        string reference = doc.Element("transaction")?.Element("reference")?.Value; // Sua referência interna (se você a enviou na requisição inicial)
                        string paymentMethodType = doc.Element("transaction")?.Element("paymentMethod")?.Element("type")?.Value; // Tipo de pagamento (ex: CREDIT_CARD, BOLETO)
                        string grossAmount = doc.Element("transaction")?.Element("grossAmount")?.Value; // Valor bruto da transação

                        System.Diagnostics.Debug.WriteLine($"Detalhes da Transação: Code={transactionCode}, Status={transactionStatus}, Referencia={reference}, TipoPagamento={paymentMethodType}, Valor={grossAmount}");

                        // 6. *** PONTO CRÍTICO: ATUALIZAR O STATUS DO SEU PEDIDO NO BANCO DE DADOS ***
                        if (transactionStatus == "3")
                        {
                            FaleConoscoViewModel vm = (FaleConoscoViewModel)Session["InfoCheckout"];
                            Int32 volta = await ProcessaCriacaoAssinante(vm);
                        }

                        // 7. Retorne um status HTTP 200 OK para o PagBank
                        // Isso informa ao PagBank que você recebeu e processou a notificação com sucesso.
                        // Se você não retornar 200 OK, o PagBank tentará reenviar a notificação.
                        return new HttpStatusCodeResult(200);
                    }
                    else
                    {
                        // 8. Tratar erros na consulta à API de Notificações
                        string errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Erro ao consultar detalhes da notificação {notificationCode}: {response.StatusCode} - {errorContent}");

                        // Mesmo em caso de erro na consulta, é uma boa prática retornar 200 OK para o PagBank,
                        // para evitar que ele continue tentando reenviar a mesma notificação repetidamente.
                        // Você deve logar o erro e investigá-lo.
                        return new HttpStatusCodeResult(200);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        public async Task<Int32> ProcessaCriacaoAssinante(FaleConoscoViewModel vm)
        {
            try
            {
                Int32 voltaCria = CriarAssinanteNormal(vm);
                vm.LoginFinal = (String)Session["LoginDemo"];
                vm.Senha = (String)Session["SenhaDemo"];
                if (voltaCria == 0)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0622", CultureInfo.CurrentCulture));
                    return 1;
                }

                // Prepara mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = vm.Nome;
                mens.ID = null;
                mens.MODELO = vm.Resposta;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_TX_TEXTO = String.Empty;
                mens.MENS_NM_LINK = null;
                mens.MENS_NM_NOME = vm.Nome;
                mens.MENS_NM_CAMPANHA = "Contratação/Alteração de Assinatura";
                mens.MENS_NM_RODAPE = vm.Resposta;
                mens.MENS_NM_LINK = "https://eprontuario.azurewebsites.net/";
                mens.CELULAR = vm.Telefone;
                mens.MENS_NM_CABECALHO = vm.CPF;
                mens.MENS_NM_ASSINATURA = vm.CNPJ;
                mens.CIDADE = vm.Celular;
                mens.MENS_IN_CRM = vm.Plano;
                await ProcessaEnvioEMailCompra(mens, vm);
                return 0;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public JsonResult GetPlanoAssinatura(Int32 id)
        {
            var plano = assiApp.GetPlanoAssinaturaById(id);
            var hash = new Hashtable();
            hash.Add("preco", CrossCutting.Formatters.DecimalFormatter(plano.PLAS_VL_PRECO));
            return Json(hash);
        }

    }
}