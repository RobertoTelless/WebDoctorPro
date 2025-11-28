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
using iTextSharp.text;
using iTextSharp.text.pdf;
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
using XidNet;
using Image = iTextSharp.text.Image;

namespace GEDSys_Presentation.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IGrupoAppService gruApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAssinanteAppService assApp;
        private readonly IControleMensagemAppService cmApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly IMensagemAppService mensApp;
        private readonly ITipoPacienteAppService tpaApp;

        private String msg;
        private Exception exception;
        private PACIENTE objeto = new PACIENTE();
        private PACIENTE objetoAntes = new PACIENTE();
        private List<PACIENTE> listaMaster = new List<PACIENTE>();
        private List<PACIENTE> listaMasterAtraso = new List<PACIENTE>();
        private List<PACIENTE> listaMasterAusencia = new List<PACIENTE>();
        private List<PACIENTE_CONSULTA> listaMasterConsulta = new List<PACIENTE_CONSULTA>();
        private List<PACIENTE_SOLICITACAO> listaMasterSolicitacao = new List<PACIENTE_SOLICITACAO>();
        private PACIENTE_SOLICITACAO objetoSolicitacao = new PACIENTE_SOLICITACAO();
        private List<PACIENTE_ATESTADO> listaMasterAtestado = new List<PACIENTE_ATESTADO>();
        private PACIENTE_ATESTADO objetoAtestado = new PACIENTE_ATESTADO();
        private List<PACIENTE_EXAMES> listaMasterExame = new List<PACIENTE_EXAMES>();
        private PACIENTE_EXAMES objetoExame = new PACIENTE_EXAMES();
        private List<PACIENTE_PRESCRICAO> listaMasterPrescricao = new List<PACIENTE_PRESCRICAO>();
        private PACIENTE_PRESCRICAO objetoPrescricao = new PACIENTE_PRESCRICAO();
        private List<PACIENTE_PRESCRICAO_ITEM> listaMasterItem = new List<PACIENTE_PRESCRICAO_ITEM>();
        private PACIENTE_PRESCRICAO_ITEM objetoItem = new PACIENTE_PRESCRICAO_ITEM();
        private PACIENTE_CONSULTA objetoConsulta = new PACIENTE_CONSULTA();
        private List<PACIENTE_CONSULTA> listaMasterCalendario = new List<PACIENTE_CONSULTA>();
        private MedicamentoViewModel objetoRemedio = new MedicamentoViewModel();
        private List<MedicamentoViewModel> listaMasterRemedio = new List<MedicamentoViewModel>();
        private PACIENTE_HISTORICO objetoHistorico = new PACIENTE_HISTORICO();
        private List<PACIENTE_HISTORICO> listaMasterHistorico = new List<PACIENTE_HISTORICO>();
        private String extensao;

        public PacienteController(IPacienteAppService baseApps, ILogAppService logApps, ITipoPessoaAppService tpApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IGrupoAppService gruApps, IMensagemEnviadaSistemaAppService meApps, IEmpresaAppService empApps, IAssinanteAppService assApps, IControleMensagemAppService cmApps, IRecursividadeAppService recuApps, IMensagemAppService mensApps, ITipoPacienteAppService tpaApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            tpApp = tpApps;
            usuApp = usuApps;
            confApp = confApps;
            gruApp = gruApps;
            meApp = meApps;
            empApp = empApps;
            assApp = assApps;
            cmApp = cmApps;
            recuApp = recuApps;
            mensApp = mensApps;
            tpaApp = tpaApps;
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


        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<PACIENTE> clientes = baseApp.GetAllItens(idAss);
            Session["Pacientes"] = clientes;

            if (nome != null)
            {
                List<PACIENTE> lstCliente = clientes.Where(x => x.PACI_NM_NOME != null && x.PACI_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<PACIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.PACI_NM_SOCIAL != null).ToList<PACIENTE>();
                    lstCliente = lstCliente.Where(x => x.PACI_NM_SOCIAL.ToLower().Contains(nome.ToLower())).ToList<PACIENTE>();
                }

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    lstCliente = lstCliente.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.PACI__CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.PACI_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.PACI_NM_NOME + " (" + item.PACI_NM_SOCIAL + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaPaciente()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
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

                // Carrega listas
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                if (Session["ListaPacienteBase"] == null)
                {
                    listaMaster = (List<PACIENTE>)cache.CarregaCacheGeralListaPacienteUltimo("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]);
                    Session["PacienteAlterada"] = 0;
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaPacienteBase"] = listaMaster;
                }
                ViewBag.Listas = (List<PACIENTE>)Session["ListaPacienteBase"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Widgets
                List<PACIENTE> listaCli = ((List<PACIENTE>)Session["ListaPacienteBase"]).ToList();
                Int32 totalCli = listaCli.Count;
                ViewBag.Pacientes = totalCli;

                List<PACIENTE> listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null).ToList();
                List<PACIENTE> listaAtraso = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_ATRASO.Value) < DateTime.Today.Date).ToList();
                ViewBag.Atrasos = listaAtraso.Count;
                Session["PacientesAtraso"] = listaAtraso;
                Session["Atrasos"] = listaAtraso.Count;
                ViewBag.AtrasoDias = conf.CONF_IN_PACIENTE_ATRASO.ToString();

                List<PACIENTE> listaAusencia = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_AUSENCIA.Value) < DateTime.Today.Date).ToList();
                ViewBag.Ausencia = listaAusencia.Count;
                Session["PacientesAusente"] = listaAusencia;
                Session["Ausencia"] = listaAusencia.Count;
                ViewBag.AusenciaDias = conf.CONF_IN_PACIENTE_AUSENCIA.ToString();

                // Listas
                List<PACIENTE_CONSULTA> cons = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultaAlterada"]);;
                List<PACIENTE_CONSULTA> consultas = cons.Where(p => p.PACO_DT_CONSULTA == DateTime.Today.Date & p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_IN_ATIVO == 1).ToList();
                Session["ConsultaAlterada"] = 0;
                ViewBag.Consultas = consultas;
                ViewBag.NumConsultas = consultas.Count;

                // Consultas por data
                List<DateTime> datas = cons.Select(p => p.PACO_DT_CONSULTA.Date).Distinct().ToList();
                DateTime limite = DateTime.Today.Date.AddDays(-30);
                if ((Int32)Session["PacienteAlterada"] == 1 || (Int32)Session["FlagPaciente"] == 1 || Session["ListaConsultaData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        if (item.Date > limite)
                        {
                            Int32 conta = cons.Where(p => p.PACO_DT_CONSULTA.Date == item.Date).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.DataEmissao = item;
                            mod.Valor = conta;
                            lista.Add(mod);
                        }
                    }
                    ViewBag.ListaConsultaData = lista;
                    Session["ListaConsultaData"] = lista;
                    Session["NumListaConsultaData"] = lista.Count;
                    ViewBag.NumListaConsultaData = lista.Count;
                }
                else
                {
                    ViewBag.ListaConsultaData = (List<ModeloViewModel>)Session["ListaConsultaData"];
                    ViewBag.NumListaConsultaData = ((List<ModeloViewModel>)Session["ListaConsultaData"]).Count;
                }

                // Aniversariantes do dia
                if ((Int32)Session["PacienteAlterada"] == 1 || (Int32)Session["FlagPaciente"] == 1 || Session["ListaPacienteAnivDia"] == null)
                {
                    List<PACIENTE> listaCli1 = listaCli.Where(p => p.PACI_DT_NASCIMENTO != null).ToList();
                    List<ModeloViewModel> listaAniv = new List<ModeloViewModel>();
                    List<PACIENTE> aniv = listaCli1.Where(p => p.PACI_DT_NASCIMENTO.Value.Month == DateTime.Today.Month & p.PACI_DT_NASCIMENTO.Value.Day == DateTime.Today.Day).ToList();
                    Session["ListaAnivDia"] = aniv;
                    foreach (PACIENTE item in aniv)
                    {
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item.PACI_NM_NOME;
                        mod.DataEmissao = item.PACI_DT_NASCIMENTO.Value;
                        mod.Valor1 = item.PACI__CD_ID;
                        mod.Nome1 = item.PACI_NM_EMAIL;
                        listaAniv.Add(mod);
                    }
                    ViewBag.ListaPacienteAnivDia = listaAniv;
                    Session["ListaPacienteAnivDia"] = listaAniv;
                    ViewBag.ListaPacienteAnivDiaConta = listaAniv.Count;
                }
                else
                {
                    ViewBag.ListaPacienteAnivDia = (List<ModeloViewModel>)Session["ListaPacienteAnivDiaConta"];
                    ViewBag.ListaPacienteAnivDiaConta = ((List<ModeloViewModel>)Session["ListaPacienteAnivDiaConta"]).Count;
                }

                // Mensagens do Fabricante
                ViewBag.MensFab = null;
                ViewBag.MensFabPrim = null;
                ViewBag.NumSlides = 0;
                List<MENSAGEM_FABRICANTE> mensFab = usuApp.GetAllMensFab(idAss).OrderBy(p => p.MEFA_IN_TIPO).ThenByDescending(p => p.MEFA_DT_CADASTRO).ToList();
                ViewBag.ListasFabricante = mensFab;
                ViewBag.NumListasFabricante = mensFab.Count;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensCliente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 11;
                Session["PacienteAlterada"] = 0;
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["VoltaAtestado"] = 1;
                Session["VoltarCentral"] = 1;
                Session["TipoSolicitacao"] = 1;
                Session["VoltarConsulta"] = 0;
                Session["IdModoConsulta"] = 0;
                Session["VoltaListaConsulta"] = 0;
                Session["EditarVer"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["ProximaConsulta"] = 0;
                Session["VoltaMensagem"] = 0;
                Session["EscopoMensagem"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/1/Ajuda1.pdf";

                // Carrega view
                objeto = new PACIENTE();
                if (Session["FiltroPaciente"] != null)
                {
                    objeto = (PACIENTE)Session["FiltroPaciente"];
                }
                return View(objeto);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaPaciente"] = null;
                Session["ListaPacienteBase"] = null;
                Session["FiltroPaciente"] = null;
                if ((Int32)Session["VoltaPaciente"] == 2)
                {
                    return RedirectToAction("VerCardsPaciente");
                }
                if ((Int32)Session["VoltaPaciente"] == 22)
                {
                    return RedirectToAction("MontarTelaCentralPaciente");
                }
                return RedirectToAction("MontarTelaPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                listaMaster = (List<PACIENTE>)cache.CarregaCacheGeralListaPacienteAdm("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]);
                Session["PacienteAlterada"] = 0;

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["FiltroPaciente"] = null;
                Session["ListaPaciente"] = listaMaster;
                if ((Int32)Session["VoltaPaciente"] == 2)
                {
                    return RedirectToAction("VerCardsPaciente");
                }
                return RedirectToAction("MontarTelaCentralPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarPaciente(PACIENTE item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.PACI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_NOME);
                item.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_CPF);
                item.PACI_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_MATRICULA);
                item.PACI_NM_INDICACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_INDICACAO);
                item.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(item.PACI_NR_CELULAR);
                item.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(item.PACI_NM_EMAIL);
                item.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_CIDADE);

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<PACIENTE> listaObj = new List<PACIENTE>();
                Session["FiltroPaciente"] = item;
                Tuple<Int32, List<PACIENTE>, Boolean> volta = baseApp.ExecuteFilterTuple(item.PACI__CD_ID, item.TIPA_CD_ID, item.SEXO_CD_ID, item.PACI_NM_NOME, item.PACI_NR_CPF, item.PACI_NR_MATRICULA, item.PACI_NM_INDICACAO, item.PACI_NR_CELULAR, item.PACI_NM_EMAIL, item.PACI_NM_CIDADE, item.UF_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    if ((Int32)Session["VoltaPaciente"] == 2)
                    {
                        return RedirectToAction("VerCardsPaciente");
                    }
                    if ((Int32)Session["VoltaPaciente"] == 3)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("MontarTelaPaciente");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaPaciente"] = listaMaster;
                Session["ListaPacienteBase"] = listaMaster;
                if ((Int32)Session["VoltaPaciente"] == 2)
                {
                    return RedirectToAction("VerCardsPaciente");
                }
                if ((Int32)Session["VoltaPaciente"] == 22)
                {
                    return RedirectToAction("MontarTelaCentralPaciente");
                }
                return RedirectToAction("MontarTelaPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBasePaciente()
        {
            try
            {
                if ((Int32)Session["VoltaPaciente"] == 1)
                {
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
                if ((Int32)Session["VoltaPaciente"] == 66)
                {
                    return RedirectToAction("IncluirMensagemEMail", "Mensagem");
                }
                if ((Int32)Session["VoltaMsg"] == 55)
                {
                    return RedirectToAction("PesquisarTudo", "BaseAdmin");
                }
                if ((Int32)Session["VoltaPaciente"] == 2)
                {
                    return RedirectToAction("VerCardsPaciente");
                }
                if ((Int32)Session["VoltaPaciente"] == 3)
                {
                    return RedirectToAction("VerPacientesFalta6Meses()");
                }
                if ((Int32)Session["VoltaPaciente"] == 22)
                {
                    return RedirectToAction("MontarTelaCentralPaciente");
                }
                return RedirectToAction("MontarTelaPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaCentralPaciente()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
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

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carrega listas
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                if ((List<PACIENTE>)Session["ListaPaciente"] == null)
                {
                    listaMaster = (List<PACIENTE>)cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]);
                    Session["PacienteAlterada"] = 0;
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaPaciente"] = listaMaster;
                }

                // Verifica se houve alteração
                if ((Int32)Session["PacienteAlterada"] == 1)
                {
                    List<PACIENTE> lista1 = (List<PACIENTE>)Session["ListaPaciente"];
                    lista1 = lista1.Where(p => p.PACI__CD_ID == (Int32)Session["IdPaciente"]).ToList();
                    Session["ListaPaciente"] = lista1;
                    Session["MensPaciente"] = 77;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE>)Session["ListaPaciente"];
                ViewBag.Tipos = (List<TIPO_PACIENTE>)cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]);
                Session["TipoPacienteAlterada"] = 0;
                ViewBag.UF = (List<UF>)cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
                ViewBag.Sexo = (List<SEXO>)cache.CarregaCacheGeralListaSexo("SEXO", usuario.ASSI_CD_ID, 0);
                ViewBag.Usuarios = (List<USUARIO>)cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
                Session["UsuarioAlterada"] = 0;

                // Ambiente
                Session["Paciente"] = null;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                ViewBag.CodigoPaciente = Session["IdPaciente"];
                Session["LinhaAlterada"] = 0;

                // Widgets
                List<PACIENTE> listaCli = ((List<PACIENTE>)Session["ListaPaciente"]).ToList();
                Int32 totalCli = listaCli.Count;
                ViewBag.Pacientes = totalCli;

                List<PACIENTE> listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null & p.PACI_DT_CONSULTA != null).ToList();
                List<PACIENTE> listaAtraso = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_ATRASO.Value) < DateTime.Today.Date).ToList();
                ViewBag.Atrasos = listaAtraso.Count;
                Session["PacientesAtraso"] = listaAtraso;
                Session["Atrasos"] = listaAtraso.Count;

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Relação de Pacientes(*)", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Pacientes em Atraso", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Pacientes Ausentes", Value = "3" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

                List<SelectListItem> situacao = new List<SelectListItem>();
                situacao.Add(new SelectListItem() { Text = "Normal", Value = "1" });
                situacao.Add(new SelectListItem() { Text = "Atrasados", Value = "2" });
                situacao.Add(new SelectListItem() { Text = "Ausentes", Value = "3" });
                ViewBag.Situacao = new SelectList(situacao, "Value", "Text");

                List<PACIENTE> listaAusencia = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_AUSENCIA.Value) < DateTime.Today.Date).ToList();
                ViewBag.Ausencias = listaAusencia.Count;
                Session["PacientesAusente"] = listaAusencia;
                Session["Ausencias"] = listaAusencia.Count;

                // Verifica possibilidade E-Mail
                List<MENSAGENS_ENVIADAS_SISTEMA> mens = (List<MENSAGENS_ENVIADAS_SISTEMA>)cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                ViewBag.EMail = 1;
                Int32 num = mens.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mens.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0515", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 77)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0511", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0512", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 400)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 401)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 100)
                    {
                        String frase = "Foram enviadas " + (String)Session["TotMensagens"] + " mensagens para pacientes";
                        ModelState.AddModelError("", frase);
                    }
                }

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 11;
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 22;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["VoltaAtestado"] = 1;
                Session["VoltarCentral"] = 2;
                Session["TipoSolicitacao"] = 1;
                Session["IdModoConsulta"] = 0;
                Session["VoltaListaConsulta"] = 0;
                Session["EditarVer"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["ProximaConsulta"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3.pdf";

                // Carrega view
                objeto = new PACIENTE();
                if (Session["FiltroPaciente"] != null)
                {
                    objeto = (PACIENTE)Session["FiltroPaciente"];
                }
                return View(objeto);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirPaciente()
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
                    if (usuario.PERFIL.PERF_IN_INCLUIR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Inclusão";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Verifica possibilidade
                List<PACIENTE> pacs = (List<PACIENTE>)cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]);
                Int32 num = pacs.Count;
                Session["PacienteAlterada"] = 0;
                if ((Int32)Session["NumPacientes"] <= num)
                {
                    Session["MensPaciente"] = 50;
                    return RedirectToAction("MontarTelaCentralPaciente", "Paciente");
                }

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0513", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 79)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0514", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.Tipos = new SelectList(cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIPA_CD_ID", "TIPA_NM_NOME");
                Session["TipoPacienteAlterada"] = 0;
                List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
                ViewBag.Sexo = cache.CarregaCacheGeralListaSexo("SEXO", usuario.ASSI_CD_ID, 0);
                List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
                ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");
                ViewBag.Cor = new SelectList(cache.CarregaCacheGeralListaCor("COR", usuario.ASSI_CD_ID, 0), "COR_CD_ID", "COR_NM_NOME");
                ViewBag.EstadoCivil = new SelectList(cache.CarregaCacheGeralListaEstadoCivil("ESTADO_CIVIL", usuario.ASSI_CD_ID, 0), "ESCI_CD_ID", "ESCI_NM_NOME");
                ViewBag.Convenio = new SelectList(cache.CarregaCacheGeralListaConvenio("CONVENIO", usuario.ASSI_CD_ID, 0), "CONV_CD_ID", "CONV_NM_NOME");
                ViewBag.Nacionalidades = new SelectList(cache.CarregaCacheGeralListaNacionalidade("NACIONALIDADE", usuario.ASSI_CD_ID, 0), "NACI_CD_ID", "NACI_NM_NOME");
                ViewBag.Municipios = new SelectList(cache.CarregaCacheGeralListaMunicipio("MUNICIPIO", usuario.ASSI_CD_ID, 0), "MUNI_CD_ID", "MUNI_NM_NOME");
                ViewBag.GrauInstrucao = new SelectList(cache.CarregaCacheGeralListaGrauInstrucao("GRAU_INSTRUCAO", usuario.ASSI_CD_ID, 0), "GRAU_CD_ID", "GRAU_NM_NOME");
                List<SelectListItem> consulta = new List<SelectListItem>();
                consulta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                consulta.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Consulta = new SelectList(consulta, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_1.pdf";
                Session["VoltaCatPaciente"] = 2;

                // Preprara outras listas
                List<SelectListItem> ufNatur = new List<SelectListItem>();
                foreach (UF uf in uFs)
                {
                    ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                }
                ViewBag.UFNatur = new SelectList(ufNatur, "Value", "Text");

                // Prepara objeto
                PACIENTE item = new PACIENTE();
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.PACI_DT_CADASTRO = DateTime.Today.Date;
                vm.PACI_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.TIPA_CD_ID = 0;
                vm.PACI_GU_GUID = Xid.NewXid().ToString();
                vm.PACI_DT_ALTERACAO = DateTime.Today.Date;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirPaciente(PacienteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Replica listas
            ViewBag.Tipos = new SelectList(cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIPA_CD_ID", "TIPA_NM_NOME");
            Session["TipoPacienteAlterada"] = 0;
            List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
            ViewBag.Sexo = cache.CarregaCacheGeralListaSexo("SEXO", usuario.ASSI_CD_ID, 0);
            List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Cor = new SelectList(cache.CarregaCacheGeralListaCor("COR", usuario.ASSI_CD_ID, 0), "COR_CD_ID", "COR_NM_NOME");
            ViewBag.EstadoCivil = new SelectList(cache.CarregaCacheGeralListaEstadoCivil("ESTADO_CIVIL", usuario.ASSI_CD_ID, 0), "ESCI_CD_ID", "ESCI_NM_NOME");
            ViewBag.Convenio = new SelectList(cache.CarregaCacheGeralListaConvenio("CONVENIO", usuario.ASSI_CD_ID, 0), "CONV_CD_ID", "CONV_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(cache.CarregaCacheGeralListaNacionalidade("NACIONALIDADE", usuario.ASSI_CD_ID, 0), "NACI_CD_ID", "NACI_NM_NOME");
            ViewBag.Municipios = new SelectList(cache.CarregaCacheGeralListaMunicipio("MUNICIPIO", usuario.ASSI_CD_ID, 0), "MUNI_CD_ID", "MUNI_NM_NOME");
            ViewBag.GrauInstrucao = new SelectList(cache.CarregaCacheGeralListaGrauInstrucao("GRAU_INSTRUCAO", usuario.ASSI_CD_ID, 0), "GRAU_CD_ID", "GRAU_NM_NOME");
            List<SelectListItem> consulta = new List<SelectListItem>();
            consulta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            consulta.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Consulta = new SelectList(consulta, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> ufNatur = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur = new SelectList(ufNatur, "Value", "Text");

            // Processo
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PACI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NOME);
                    vm.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CPF);
                    vm.PACI_NM_SOCIAL = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_SOCIAL);
                    vm.PACI_NM_PAI = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_PAI);
                    vm.PACI_NM_MAE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_MAE);
                    vm.PACI_NR_CEP = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CEP);
                    vm.PACI_NR_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_COMPLEMENTO);
                    vm.PACI_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_ENDERECO);
                    vm.PACI_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_NUMERO);
                    vm.PACI_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_BAIRRO);
                    vm.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_CIDADE);
                    vm.PACI_NM_PROFISSAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_PROFISSAO);
                    vm.PACI_NM_NATURALIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NATURALIDADE);
                    vm.PACI_NM_NACIONALIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NACIONALIDADE);
                    vm.PACI_NR_RG = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_RG);
                    vm.PACI_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_TELEFONE);
                    vm.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_CELULAR);
                    vm.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.PACI_NM_EMAIL);
                    vm.PACI_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_MATRICULA);
                    vm.PACI_NM_INDICACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_INDICACAO);
                    vm.PACI_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_TX_OBSERVACOES);

                    // Critica basica
                    if (vm.PACI_NM_NOME == null)
                    {
                        Session["MensPaciente"] = 3;
                        return View(vm);
                    }
                    if (vm.USUA_CD_ID == null)
                    {
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    }
                    if (vm.PACI_DT_NASCIMENTO.Value.Date >= DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0531", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de consulta
                    if (vm.CRIA_CONSULTA == null)
                    {
                        vm.CRIA_CONSULTA = 0;
                    }
                    if (vm.CRIA_CONSULTA == 0)
                    {
                        vm.PACO_DT_CONSULTA = null;
                    }

                    // Critica de tabelas auxiliares
                    if (vm.TIPA_CD_ID == null || vm.TIPA_CD_ID == 0)
                    {
                        vm.TIPA_CD_ID = CarregaCatPaciente().Where(p => p.TIPA_NM_NOME.Contains("Informad")).FirstOrDefault().TIPA_CD_ID;
                    }
                    if (vm.CONV_CD_ID == null || vm.CONV_CD_ID == 0)
                    {
                        vm.CONV_CD_ID = CarregaConvenio().Where(p => p.CONV_NM_NOME.Contains("Informad")).FirstOrDefault().CONV_CD_ID;
                    }
                    if (vm.SEXO_CD_ID == null || vm.SEXO_CD_ID == 0)
                    {
                        vm.SEXO_CD_ID = CarregaSexo().Where(p => p.SEXO_NM_NOME.Contains("Informad")).FirstOrDefault().SEXO_CD_ID;
                    }
                    if (vm.COR1_CD_ID == null || vm.COR1_CD_ID == 0)
                    {
                        vm.COR1_CD_ID = CarregaCor().Where(p => p.COR1_NM_NOME.Contains("Informad")).FirstOrDefault().COR1_CD_ID;
                    }
                    if (vm.ESCI_CD_ID == null || vm.ESCI_CD_ID == 0)
                    {
                        vm.ESCI_CD_ID = CarregaEstadoCivil().Where(p => p.ESCI_NM_NOME.Contains("Informad")).FirstOrDefault().ESCI_CD_ID;
                    }
                    if (vm.GRAU_CD_ID == null || vm.GRAU_CD_ID == 0)
                    {
                        vm.GRAU_CD_ID = CarregaGrauInstrucao().Where(p => p.GRAU_NM_NOME.Contains("Informad")).FirstOrDefault().GRAU_CD_ID;
                    }

                    // Monta paciente
                    Session["NomePacienteIncluir"] = vm.PACI_NM_NOME;
                    PACIENTE item = Mapper.Map<PacienteViewModel, PACIENTE>(vm);

                    // Carrega foto e processa alteracao
                    if (item.PACI_AQ_FOTO == null)
                    {
                        item.PACI_AQ_FOTO = "~/Images/icon_morador.png";
                    }

                    // Executa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Fotos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Prescricao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/QRCode/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Atestado/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Solicitacao/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Exames/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Consultas/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Atualiza estado
                    List<PACIENTE> pacs = cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]);
                    Session["PacienteAlterada"] = 0;

                    listaMaster = new List<PACIENTE>();
                    Session["ListaPaciente"] = null;
                    Session["ListaPacienteBase"] = null;
                    Session["IncluirPaciente"] = 1;
                    Session["PacienteNovo"] = item.PACI__CD_ID;
                    Session["Pacientes"] = pacs;
                    Session["IdVolta"] = item.PACI__CD_ID;
                    Session["IdPaciente"] = item.PACI__CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["LinhaAlterada"] = item.PACI__CD_ID;
                    Session["FlagPaciente"] = 1;

                    // Trata anexos
                    if (Session["FileQueuePaciente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePaciente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueuePaciente(file);
                            }
                            else
                            {
                                UploadFotoQueuePaciente(file);
                            }
                        }
                        Session["FileQueuePaciente"] = null;
                    }

                    // Trata consulta
                    if (vm.CRIA_CONSULTA == 1)
                    {
                        // Monta consulta
                        PACIENTE_CONSULTA cons = new PACIENTE_CONSULTA();
                        cons.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        cons.PACI_CD_ID = item.PACI__CD_ID;
                        cons.PACO_DT_CONSULTA = vm.PACO_DT_CONSULTA.Value;
                        cons.PACO_HR_INICIO = vm.HORA_INICIO;
                        cons.PACO_HR_FINAL = vm.HORA_FINAL;
                        cons.PACO_IN_ATIVO = 1;
                        cons.PACO_IN_TIPO = 1;
                        cons.USUA_CD_ID = usuario.USUA_CD_ID;

                        // Checa horario
                        List<PACIENTE_CONSULTA> lista = baseApp.GetAllConsultas(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_DT_CONSULTA == vm.PACO_DT_CONSULTA).ToList();
                        List<PACIENTE_CONSULTA> lista1 = lista.Where(p => p.PACO_HR_INICIO <= vm.HORA_INICIO & p.PACO_HR_FINAL >= vm.HORA_FINAL).ToList();
                        List<PACIENTE_CONSULTA> lista2 = lista.Where(p => p.PACO_HR_INICIO <= vm.HORA_INICIO & p.PACO_HR_FINAL <= vm.HORA_FINAL & p.PACO_HR_FINAL >= vm.HORA_INICIO).ToList();
                        List<PACIENTE_CONSULTA> lista3 = lista.Where(p => p.PACO_HR_INICIO >= vm.HORA_INICIO & p.PACO_HR_FINAL >= vm.HORA_FINAL & p.PACO_HR_INICIO <= vm.HORA_FINAL).ToList();
                        List<PACIENTE_CONSULTA> lista4 = lista.Where(p => p.PACO_HR_INICIO >= vm.HORA_INICIO & p.PACO_HR_FINAL <= vm.HORA_FINAL).ToList();
                        if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                        {
                            Session["MensPaciente"] = 600;
                        }

                        // Cria consulta
                        Int32 voltaC = baseApp.ValidateCreateConsulta(cons);

                        // Cria anamnese em branco
                        PACIENTE_ANAMNESE anamnese = new PACIENTE_ANAMNESE();
                        anamnese.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        anamnese.PAAM_DT_DATA = cons.PACO_DT_CONSULTA;
                        anamnese.PAAM_IN_ATIVO = 1;
                        anamnese.PACI_CD_ID = item.PACI__CD_ID;
                        anamnese.USUA_CD_ID = usuario.USUA_CD_ID;
                        anamnese.PACO_CD_ID = cons.PACO_CD_ID;
                        Int32 voltaA = baseApp.ValidateCreateAnamnese(anamnese);

                        // Cria exame fisico em branco
                        PACIENTE_EXAME_FISICOS fisico = new PACIENTE_EXAME_FISICOS();
                        fisico.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        fisico.PACI_CD_ID = item.PACI__CD_ID;
                        fisico.PAEF_DT_DATA = cons.PACO_DT_CONSULTA;
                        fisico.PAEF_IN_ALCOOLISMO = 0;
                        fisico.PAEF_IN_ALCOOLISMO_FREQUENCIA = 0;
                        fisico.PAEF_IN_ANTE_ALERGICO = 0;
                        fisico.PAEF_IN_ANTE_ONCOLOGICO = 0;
                        fisico.PAEF_IN_ANTICONCEPCIONAL = 0;
                        fisico.PAEF_IN_ATIVO = 1;
                        fisico.PAEF_IN_CIRURGIAS = 0;
                        fisico.PAEF_IN_DIABETE = 0;
                        fisico.PAEF_IN_EPILEPSIA = 0;
                        fisico.PAEF_IN_EXERCICIO_FISICO = 0;
                        fisico.PAEF_IN_EXERCICIO_FISICO_FREQUENCIA = 0;
                        fisico.PAEF_IN_GESTANTE = 0;
                        fisico.PAEF_IN_HIPERTENSAO = 0;
                        fisico.PAEF_IN_HIPOTENSAO = 0;
                        fisico.PAEF_IN_MARCAPASSO = 0;
                        fisico.PAEF_IN_TABAGISMO = 0;
                        fisico.PAEF_IN_VARIZES = 0;
                        fisico.PAEF_VL_IMC = 0;
                        fisico.PACO_CD_ID = cons.PACO_CD_ID;
                        fisico.USUA_CD_ID = usuario.USUA_CD_ID;
                        Int32 voltaF = baseApp.ValidateCreateExameFisico(fisico);

                        // Acerta paciente
                        PACIENTE pac = baseApp.GetItemById(item.PACI__CD_ID);
                        pac.PACI_DT_PREVISAO_RETORNO = vm.PACO_DT_CONSULTA.Value;
                        Int32 voltaPac = baseApp.ValidateEdit(pac, pac);
                    }

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI__CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 1;
                    hist.PAHI_IN_CHAVE = item.PACI__CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão";
                    hist.PAHI_DS_DESCRICAO = "Inclusão do paciente: " + item.PACI_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Trata retorno
                    if ((Int32)Session["VoltaPaciente"] == 66)
                    {
                        return RedirectToAction("IncluirMensagemEMail", "Mensagem");
                    }
                    if ((Int32)Session["VoltaPaciente"] == 3)
                    {
                        Session["VoltaPaciente"] = 0;
                        return RedirectToAction("VoltarAnexoPaciente", "Paciente");
                    }
                    Session["NivelPaciente"] = 1;
                    Session["MensPaciente"] = 79;
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Paciente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "epront", 1, (USUARIO)Session["UserCredentials"]);
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
            Session["FileQueuePaciente"] = queue;
        }

        [HttpPost]
        public void UploadFileToSessionMensagem(IEnumerable<HttpPostedFileBase> files, String profile)
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
            Session["FileQueuePacienteMensagem"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePaciente(FileQueue file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPaciente"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Recupera cliente
                PACIENTE item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PACIENTE_ANEXO foto = new PACIENTE_ANEXO();
                foto.PAAX_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAAX_DT_ANEXO = DateTime.Today;
                foto.PAAX_IN_ATIVO = 1;
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
                foto.PAAX_IN_TIPO = tipo;
                foto.PAAX_NM_TITULO = fileName;
                foto.PACI_CD_ID = item.PACI__CD_ID;
                item.PACIENTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueuePaciente(FileQueue file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPaciente"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Recupera cliente
                PACIENTE item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }


                // Copia imagem
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item.PACI_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                listaMaster = new List<PACIENTE>();
                Session["ListaPaciente"] = null;
                return RedirectToAction("VoltarAnexoPaciente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "eP   ront", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarPaciente", new { id = (Int32)Session["IdPaciente"] });
        }

        public ActionResult VoltarVerPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("VerPaciente", new { id = (Int32)Session["IdPaciente"] });
        }

        public ActionResult VoltarProcederConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("ProcederConsulta", new { id = (Int32)Session["IdConsulta"] });
        }

        public ActionResult VoltarEditarPrescricao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPrescricaoNova", new { id = (Int32)Session["IdPrescricao"] });
        }

        public ActionResult VoltarEditarPrescricaoItem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            // Recupera informações
            PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById((Int32)Session["IdPrescricao"]);
            PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);

            // Processo
            if (prescricao.TICO_CD_ID == 1)
            {
                if (prescricao.PACIENTE_PRESCRICAO_ITEM.Count > 0)
                {
                    if (prescricao.PAPR_IN_ENVIADO == 0)
                    {
                        // Gera prescricao HTML
                        String fileName = "Prescricao_" + paciente.PACI_NM_NOME + "_" + prescricao.PAPR_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                        String caminho = "/Imagens/" + prescricao.ASSI_CD_ID.ToString() + "/Pacientes/" + prescricao.PACI_CD_ID.ToString() + "/Prescricao/";
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        String prescricaoHTML = GerarPrescricaoHTML();
                        System.IO.File.WriteAllText(path, prescricaoHTML);

                        // Transforma em PDF
                        String fileNamePDF = "Prescricao_" + paciente.PACI_NM_NOME + "_" + prescricao.PAPR_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                        String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                        if (System.IO.File.Exists(pathPDF))
                        {
                            System.IO.File.Delete(pathPDF);
                        }
                        PdfCreator envio = new PdfCreator();
                        String prescricaoPDF = envio.ConvertHtmlToPdf(prescricaoHTML, fileNamePDF, pathPDF);

                        prescricao.PAPR_HT_TEXTO_HTML = prescricaoHTML;
                        prescricao.PAPR_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                        prescricao.PAPR_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                        prescricao.PAPR_IN_PDF = 1;
                        prescricao.PAPR_DT_GERACAO_PDF = DateTime.Today.Date;
                        Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);
                    }
                }
            }
            else
            {
                prescricao.PAPR_HT_TEXTO_HTML = null;
                prescricao.PAPR_AQ_ARQUIVO_HTML = null;
                prescricao.PAPR_AQ_ARQUIVO_PDF = null;
                prescricao.PAPR_IN_PDF = 0;
                prescricao.PAPR_DT_GERACAO_PDF = null;
                Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);
            }
            return RedirectToAction("EditarPrescricaoNova", new { id = (Int32)Session["IdPrescricao"] });
        }

        public ActionResult VoltarEditarExame()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarExame", new { id = (Int32)Session["IdExame"] });
        }

        public ActionResult VoltarEditarAtestado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarAtestado", new { id = (Int32)Session["IdAtestado"] });
        }

        public ActionResult VoltarEditarUltimaAnamnese()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarAnamnese", new { id = (Int32)Session["IdUltimaAnamnese"] });
        }


        public ActionResult VoltarVerUltimaAnamnese()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerAnamnese", new { id = (Int32)Session["IdUltimaAnamnese"] });
        }

        public ActionResult VoltarEditarUltimoFisico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarExameFisico", new { id = (Int32)Session["IdUltimoFisico"] });
        }

        public ActionResult VoltarVerUltimoFisico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerExameFisico", new { id = (Int32)Session["IdUltimoFisico"] });
        }

        public ActionResult VoltarEditarSolicitacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarSolicitacao", new { id = (Int32)Session["IdSolicitacao"] });
        }

        public ActionResult VoltarCentrais()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltarCentral"] == 1)
            {
                return RedirectToAction("MontarTelaPaciente");
            }
            if ((Int32)Session["VoltarCentral"] == 2)
            {
                return RedirectToAction("MontarTelaCentralPaciente");
            }
            return RedirectToAction("MontarTelaPaciente");
        }

        public ActionResult VoltarConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltarConsulta"] == 1)
            {
                return RedirectToAction("MontarTelaPaciente");
            }
            if ((Int32)Session["VoltarConsulta"] == 2)
            {
                return RedirectToAction("MontarTelaCentralPaciente");
            }
            if ((Int32)Session["VoltarConsulta"] == 3)
            {
                return RedirectToAction("VoltarAnexoPaciente");
            }
            if ((Int32)Session["VoltarConsulta"] == 4)
            {
                return RedirectToAction("MontarTelaConsultas");
            }
            return RedirectToAction("MontarTelaPaciente");
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();
            if (end.Address == String.Empty)
            {
                hash.Add("Sucesso", 0);
            }
            else
            {
                if (tipoEnd == 1)
                {
                    hash.Add("Sucesso", 1);
                    hash.Add("PACI_NM_ENDERECO", end.Address);
                    hash.Add("PACI_NR_NUMERO", end.Complement);
                    hash.Add("PACI_NM_BAIRRO", end.District);
                    hash.Add("PACI_NM_CIDADE", end.City);
                    hash.Add("UF_CD_ID", baseApp.GetUFbySigla(end.Uf).UF_CD_ID);
                    hash.Add("PACI_NR_CEP", cep);
                }
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public JsonResult FiltrarMunicipio(Int32? id)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia cache
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            var listaFiltrada = (List<MUNICIPIO>)cache.CarregaCacheGeralListaMunicipio("MUNICIPIO", usuario.ASSI_CD_ID, 0).ToList();

            // Filtro para caso o placeholder seja selecionado
            if (id != null)
            {
                listaFiltrada = listaFiltrada.Where(x => x.UF_CD_ID == id).ToList();
            }
            return Json(listaFiltrada.Select(x => new { x.MUNI_CD_ID, x.MUNI_NM_NOME }));
        }

        [HttpGet]
        public ActionResult ExcluirPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUIR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exclusáo";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar exclusão
                PACIENTE item = baseApp.GetItemById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPaciente"] = 4;
                    return RedirectToAction("MontarTelaCentralPaciente", "Paciente");
                }

                // Acerta estado
                listaMaster = new List<PACIENTE>();
                Session["ListaPaciente"] = null;
                Session["ListaPacienteBase"] = null;
                Session["FiltroPaciente"] = null;
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["FlagPaciente"] = 1;
                Session["PacientesGeral"] = null;

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 1;
                hist.PAHI_IN_CHAVE = item.PACI__CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão";
                hist.PAHI_DS_DESCRICAO = "Exclusão do paciente: " + item.PACI_NM_NOME;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                return RedirectToAction("MontarTelaCentralPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVA_CLIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Reativação";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Verifica possibilidade
                Int32 num = cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]).Count;
                Session["PacienteAlterada"] = 0;
                if ((Int32)Session["NumPacientes"] <= num)
                {
                    Session["MensPaciente"] = 50;
                    return RedirectToAction("MontarTelaCentralPaciente", "Paciente");
                }

                // Executar reativação
                PACIENTE item = baseApp.GetItemById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Acerta estado
                listaMaster = new List<PACIENTE>();
                Session["ListaPaciente"] = null;
                Session["FiltroPaciente"] = null;
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["FlagPaciente"] = 1;
                Session["PacientesGeral"] = null;

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 1;
                hist.PAHI_IN_CHAVE = item.PACI__CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Reativação";
                hist.PAHI_DS_DESCRICAO = "Reativação do paciente: " + item.PACI_NM_NOME;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                return RedirectToAction("MontarTelaCentralPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EnviarEMailPacienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMsg"] = 2;
            return RedirectToAction("EnviarEMailPaciente", new { id = (Int32)Session["IdPaciente"] });
        }

        public ActionResult EnviarSMSPacienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaMsg"] = 2;
            return RedirectToAction("EnviarSMSPaciente", new { id = (Int32)Session["IdPaciente"] });
        }

        [HttpGet]
        public ActionResult EnviarEMailPaciente(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = (List<MENSAGENS_ENVIADAS_SISTEMA>)cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensPaciente"] = 400;
                    return RedirectToAction("MontarTelaCentralPaciente");
                }

                // Recupera paciente
                PACIENTE cont = baseApp.GetItemById(id);
                Session["Paciente"] = cont;
                ViewBag.Paciente = cont;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_9.pdf";

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.PACI_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = cont.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem para Paciente: " + cont.PACI_NM_NOME;
                mens.PACI_CD_ID = cont.PACI__CD_ID;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarEMailPaciente(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE cont = (PACIENTE)Session["Paciente"];

            // Processo
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Paciente: " + cont.PACI_NM_NOME;

                    // Prepara cabeçalho
                    String cab = "Prezado Sr(a). <b>" + cont.PACI_NM_NOME + "</b><br />";

                    // Prepara assinatura
                    vm.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                    String rod = String.Empty;
                    rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";

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

                    // Prepara registro de mensagem
                    String id = Xid.NewXid().ToString();
                    MENSAGENS mens = new MENSAGENS();
                    mens.ASSI_CD_ID = idAss;
                    mens.MENS_DT_CRIACAO = DateTime.Now;
                    mens.MENS_IN_ATIVO = 1;
                    mens.USUA_CD_ID = usuario.USUA_CD_ID;
                    mens.EMFI_CD_ID = usuario.EMFI_CD_ID;
                    mens.EMPR_CD_ID = usuario.EMPR_CD_ID;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_TX_TEXTO = null;
                    mens.MENS_IN_REPETICAO = null;
                    mens.MENS_NR_REPETICOES = 0;
                    mens.MENS_IN_STATUS = 1;
                    mens.MENS_IN_OCORRENCIAS = 1;
                    mens.MENS_IN_ENVIADAS = 0;
                    mens.MENS_DT_AGENDAMENTO = DateTime.Now;
                    mens.MENS_IN_AGENDAMENTO = 0;
                    mens.MENS_IN_SISTEMA = 6;
                    mens.MENS_NM_LINK = vm.MENS_NM_LINK;
                    mens.MENS_TX_TEXTO = emailBody;
                    mens.MENS_GU_GUID = id;
                    mens.MENS_ID_IDENTIFICADOR = id;
                    mens.MENS_IN_DESTINOS = 1;
                    mens.MENS_IN_STATUS = 2;
                    mens.MENS_IN_TIPO_EMAIL = 1;
                    mens.MENS_IN_DESTINOS = 1;
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    Int32 volta = mensApp.ValidateCreate(mens, usuario);
                    Session["IdMensagem"] = mens.MENS_CD_ID;

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + mens.MENS_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    if (Session["FileQueuePaciente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePaciente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                Int32 x = UploadFileQueuePacienteMensagem(file);
                            }
                        }
                        Session["FileQueuePaciente"] = null;
                    }

                    // Prepara
                    MENSAGENS item1 = mensApp.GetItemById(mens.MENS_CD_ID);
                    MensagemViewModel vm1 = Mapper.Map<MENSAGENS, MensagemViewModel>(item1);
                    vm1.MENS_CD_ID = item1.MENS_CD_ID;
                    vm1.MENSAGEM_ANEXO = item1.MENSAGEM_ANEXO;
                    vm1.ID = cont.PACI__CD_ID;
                    Session["IdMensagem"] = item1.MENS_CD_ID;

                    // Monta recursividade
                    RECURSIVIDADE rec = new RECURSIVIDADE();
                    rec.ASSI_CD_ID = idAss;
                    rec.MENS_CD_ID = null;
                    rec.EMPR_CD_ID = usuario.EMPR_CD_ID;
                    rec.RECU_IN_TIPO_MENSAGEM = 1;
                    rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                    rec.RECU_IN_TIPO_SMS = 0;
                    rec.RECU_NM_NOME = "Envio de Mensagem para Paciente - " + cont.PACI_NM_NOME + " - " + DateTime.Now.ToString();
                    rec.RECU_LK_LINK = null;
                    rec.RECU_IN_SISTEMA = 6;
                    rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                    rec.RECU_IN_TIPO_ENVIO = 4;
                    rec.RECU_TX_TEXTO = emailBody;
                    rec.RECU_IN_ATIVO = 1;

                    // Monta destinos
                    RECURSIVIDADE_DESTINO dest1 = new RECURSIVIDADE_DESTINO();
                    dest1.FORN_CD_ID = null;
                    dest1.CLIE_CD_ID = null;
                    dest1.REDE_EM_EMAIL = cont.PACI_NM_EMAIL;
                    dest1.REDE_NM_NOME = cont.PACI_NM_NOME;
                    dest1.REDE_TX_CORPO = emailBody;
                    dest1.REDE_IN_ATIVO = 1;
                    rec.RECURSIVIDADE_DESTINO.Add(dest1);

                    // Monta Datas
                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                    data1.REDA_DT_PROGRAMADA = DateTime.Now.AddMinutes(10);
                    data1.REDA_IN_PROCESSADA = 0;
                    data1.REDA_IN_ATIVO = 1;
                    rec.RECURSIVIDADE_DATA.Add(data1);

                    // Grava recursividade
                    Int32 voltaRec = recuApp.ValidateCreate(rec, usuario);

                    // Grava envio
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaX = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    String guid = Xid.NewXid().ToString();
                    Int32 volta1 = envio.GravarMensagemEnviada(vm, usuario, vm.MENS_TX_TEXTO, "Success", guid, null, "Mensagem E-Mail para Paciente");

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "emaPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.PACI_NM_NOME + " | Data:" + DateTime.Today.Date.ToShortDateString(),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 1;
                    if ((Int32)Session["VoltaMsg"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    return RedirectToAction("VoltarBasePaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }
        
        [HttpPost]
        public Int32 UploadFileQueuePacienteMensagem(FileQueue file)
        {
            try
            {
                Int32 idNot = (Int32)Session["IdMensagem"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensPaciente"] = 10;
                    return 1;
                }

                MENSAGENS item = mensApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensPaciente"] = 11;
                    return 1;
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                MENSAGEM_ANEXO foto = new MENSAGEM_ANEXO();
                foto.MEAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.MEAN_DT_ANEXO = DateTime.Today;
                foto.MEAN_IN_ATIVO = 1;
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
                foto.MEAN_IN_TIPO = tipo;
                foto.MEAN_NM_TITULO = fileName;
                foto.MENS_CD_ID = item.MENS_CD_ID;
                foto.MEAN_BN_BINARIO = System.IO.File.ReadAllBytes(path);
                item.MENSAGEM_ANEXO.Add(foto);
                Int32 volta = mensApp.ValidateEdit(item, item);
                return 0;
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
                return 0;
            }
        }

        [HttpGet]
        public ActionResult EnviarSMSPaciente(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia caches
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = (List<MENSAGENS_ENVIADAS_SISTEMA>)cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    Session["MensPaciente"] = 401;
                    return RedirectToAction("MontarTelaCentralPaciente");
                }

                PACIENTE item = baseApp.GetItemById(id);
                Session["Paciente"] = item;
                ViewBag.Paciente = item;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_5.pdf";

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.PACI_NM_NOME;
                mens.ID = id;
                mens.MODELO = item.PACI_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.MENS_NM_NOME = "Mensagem para Paciente: " + item.PACI_NM_NOME;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarSMSPaciente(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.MENS_NM_LINK);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSPaciente(vm, usuarioLogado);

                    // Monta Log
                    PACIENTE cont = (PACIENTE)Session["Paciente"];
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "smsPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.PACI_NM_NOME + " | Data:" + DateTime.Today.Date.ToShortDateString(),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 1;
                    if ((Int32)Session["VoltaMsg"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    return RedirectToAction("MontarTelaCentralPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSPaciente(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                String erro = null;
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE cont = (PACIENTE)Session["Paciente"];

                // Prepara cabeçalho
                String cab = "Prezado Sr(a). " + cont.PACI_NM_NOME;

                // Prepara rodape
                String rod = usuario.USUA_NM_NOME;

                // Carrega Configuração
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = cab + ". " + vm.MENS_TX_SMS + " " + rod;

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

                // inicia processo
                String resposta = String.Empty;

                // Chama envio
                String listaDest = "55" + Regex.Replace(cont.PACI_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"WebDoctor\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.PACI_CD_ID = cont.PACI__CD_ID;
                env.MEEN_IN_TIPO = 2;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_NR_CELULAR_DESTINO = cont.PACI_NR_CELULAR;
                env.MEEN_NM_TITULO = "Mensagem SMS para Paciente";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_TX_CORPO_COMPLETO = texto;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 2;
                env.MEEN_NM_ORIGEM = "Paciente : " + cont.PACI_NM_NOME;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                throw;
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSContato(MensagemViewModel vm, USUARIO usuario)
        {
            try
            {
                // Recupera contatos
                String erro = null;
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE cont = (PACIENTE)Session["Paciente"];

                // Prepara cabeçalho
                String cab = "Prezado Sr(a). " + vm.NOME;

                // Prepara rodape
                String rod = usuario.USUA_NM_NOME;

                // Carrega Configuração
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Prepara texto
                String texto = cab + ". " + vm.MENS_TX_SMS + " " + rod;

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

                // inicia processo
                String resposta = String.Empty;

                // Chama envio
                String listaDest = "55" + Regex.Replace(vm.MODELO, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"WebDoctor\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Grava envio
                MENSAGENS_ENVIADAS_SISTEMA env = new MENSAGENS_ENVIADAS_SISTEMA();
                env.ASSI_CD_ID = idAss;
                env.USUA_CD_ID = usuario.USUA_CD_ID;
                env.PACI_CD_ID = vm.PACI_CD_ID;
                env.MEEN_IN_TIPO = 2;
                env.EMPR_CD_ID = usuario.EMPR_CD_ID;
                env.MEEN_DT_DATA_ENVIO = DateTime.Now;
                env.MEEN_NR_CELULAR_DESTINO = cont.PACI_NR_CELULAR;
                env.MEEN_NM_TITULO = "Mensagem SMS para Paciente/Contato";
                env.MEEN_TX_CORPO = vm.MENS_TX_SMS;
                env.MEEN_TX_CORPO_COMPLETO = texto;
                env.MEEN_IN_ANEXOS = 0;
                env.MEEN_IN_ATIVO = 1;
                env.MEEN_IN_ESCOPO = 2;
                env.MEEN_SG_STATUS = "Succeeded";
                env.MEEN_NM_ORIGEM = "Contato de paciente : " + cont.PACI_NM_NOME;
                env.MEEN_GU_ID_MENSAGEM = Guid.NewGuid().ToString();
                env.MEEN_ID_IDENTIFICADOR = Xid.NewXid().ToString();
                env.MEEN_IN_SISTEMA = 6;
                env.MEEN_IN_ENTREGUE = 1;
                Int32 volta5 = meApp.ValidateCreate(env);
                return 0;
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
                throw;
            }
        }

        [NonAction]
        public List<SelectListItem> CarregarModelosHtml()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            String caminho = "/TemplateEMail/Modelos/" + idAss.ToString() + "/";
            String path = Path.Combine(Server.MapPath(caminho));
            String[] files = Directory.GetFiles(path, "*.html");
            List<SelectListItem> mod = new List<SelectListItem>();
            foreach (String file in files)
            {
                mod.Add(new SelectListItem() { Text = System.IO.Path.GetFileNameWithoutExtension(file), Value = file });
            }

            return mod;
        }

        [HttpGet]
        public ActionResult EditarPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Edição";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Prepara listas
                ViewBag.Tipos = new SelectList(cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIPA_CD_ID", "TIPA_NM_NOME");
                Session["TipoPacienteAlterada"] = 0;
                List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
                ViewBag.Sexo = cache.CarregaCacheGeralListaSexo("SEXO", usuario.ASSI_CD_ID, 0);
                List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
                ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");
                ViewBag.Cor = new SelectList(cache.CarregaCacheGeralListaCor("COR", usuario.ASSI_CD_ID, 0), "COR_CD_ID", "COR_NM_NOME");
                ViewBag.EstadoCivil = new SelectList(cache.CarregaCacheGeralListaEstadoCivil("ESTADO_CIVIL", usuario.ASSI_CD_ID, 0), "ESCI_CD_ID", "ESCI_NM_NOME");
                ViewBag.Convenio = new SelectList(cache.CarregaCacheGeralListaConvenio("CONVENIO", usuario.ASSI_CD_ID, 0), "CONV_CD_ID", "CONV_NM_NOME");
                ViewBag.Nacionalidades = new SelectList(cache.CarregaCacheGeralListaNacionalidade("NACIONALIDADE", usuario.ASSI_CD_ID, 0), "NACI_CD_ID", "NACI_NM_NOME");
                ViewBag.Municipios = new SelectList(cache.CarregaCacheGeralListaMunicipio("MUNICIPIO", usuario.ASSI_CD_ID, 0), "MUNI_CD_ID", "MUNI_NM_NOME");
                ViewBag.GrauInstrucao = new SelectList(cache.CarregaCacheGeralListaGrauInstrucao("GRAU_INSTRUCAO", usuario.ASSI_CD_ID, 0), "GRAU_CD_ID", "GRAU_NM_NOME");
                List<SelectListItem> consulta = new List<SelectListItem>();
                consulta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                consulta.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Consulta = new SelectList(consulta, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> ufNatur = new List<SelectListItem>();
                List<SelectListItem> ufNatur1 = new List<SelectListItem>();
                foreach (UF uf in uFs)
                {
                    ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                    ufNatur1.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                }
                ViewBag.UFNatur1 = new SelectList(ufNatur, "Value", "Text");
                ViewBag.UFNatur2 = new SelectList(ufNatur1, "Value", "Text");

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Carrega paciente
                PACIENTE item = baseApp.GetItemById(id);
                Session["Paciente"] = item;

                // Carrega Listas
                List<PACIENTE_PRESCRICAO> listaPrescricao = item.PACIENTE_PRESCRICAO.Where(p => p.PAPR_IN_ATIVO == 1).ToList();
                Session["ListaPrescricao"] = listaPrescricao;
                List<PACIENTE_ATESTADO> listaAtestado = item.PACIENTE_ATESTADO.Where(p => p.PAAT_IN_ATIVO == 1).ToList();
                Session["ListaAtestado"] = listaAtestado;
                List<PACIENTE_SOLICITACAO> listaSolicitacao = item.PACIENTE_SOLICITACAO.Where(p => p.PASO_IN_ATIVO == 1).ToList();
                Session["ListaSolicitacao"] = listaSolicitacao;
                List<PACIENTE_EXAMES> listaExame = item.PACIENTE_EXAMES.Where(p => p.PAEX_IN_ATIVO == 1).ToList();
                Session["ListaExame"] = listaExame;
                List<PACIENTE_CONSULTA> listaConsulta = item.PACIENTE_CONSULTA.Where(p => p.PACO_IN_ATIVO == 1).ToList();
                Session["ListaConsulta"] = listaConsulta;
                List<PACIENTE_PRESCRICAO_ITEM> listaRemedio = item.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                Session["ListaRemedio"] = listaRemedio;

                // Trata mensagens
                if (Session["MensPaciente"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 66)
                    {
                        String msg = "O paciente " + item.PACI_NM_NOME + " teve suas informações alteradas.";
                        ModelState.AddModelError("", msg);
                    }
                    if ((Int32)Session["MensPaciente"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 201)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 555)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0516", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 400)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 401)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 998)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0518", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 999)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0517", CultureInfo.CurrentCulture) + ": " + (String)Session["IdEnvio"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 600)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0519", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 990)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0520", CultureInfo.CurrentCulture) + ": " + (String)Session["IdEnvio"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 991)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0521", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 992)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0522", CultureInfo.CurrentCulture) + ": " + (String)Session["IdEnvio"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 993)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0523", CultureInfo.CurrentCulture));
                    }
                }

                // Indicadores
                ViewBag.NivelPaciente = (Int32)Session["NivelPaciente"]; 

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 2;
                ViewBag.Incluir = 1;
                objetoAntes = item;
                Session["Paciente"] = item;
                Session["IdPaciente"] = id;
                Session["IdVolta"] = id;
                Session["VoltaCEP"] = 1;
                Session["VoltaMsg"] = 2;
                Session["NivelExame"] = 1;
                Session["MensPaciente"] = null;
                Session["VoltaGrupo"] = 22;
                Session["VoltaAtestado"] = 2;
                Session["VoltaAnamnese"] = 1;
                Session["VoltaFisico"] = 1;
                Session["VoltarConsulta"] = 3;
                Session["ModoConsulta"] = 0;
                Session["EditarVer"] = 0;
                Session["ProximaConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["VoltaMensagem"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_2.pdf";
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);

                // Carrega ultima anamnese
                List<PACIENTE_ANAMNESE> anamneses = item.PACIENTE_ANAMNESE.Where(p => p.PAAM_DT_DATA.Date <= DateTime.Today.Date & p.PAAM_IN_ATIVO == 1).ToList();
                PACIENTE_ANAMNESE anamnese = null;
                Int32 temAnamnese = 0;
                if (anamneses.Count() > 0)
                {
                    temAnamnese = 1;
                    anamnese = anamneses.OrderByDescending(p => p.PAAM_DT_DATA).ToList().FirstOrDefault();
                    vm.PAAN_DS_MOTIVO_CONSULTA = anamnese.PAAM_DS_MOTIVO_CONSULTA;
                    vm.PAAN_DS_QUEIXA_PRINCIPAL = anamnese.PAAM_DS_QUEIXA_PRINCIPAL;
                    vm.PAAN_DS_HISTORIA_FAMILIAR = anamnese.PAAM_DS_HISTORIA_FAMILIAR;
                    vm.PAAN_DS_HISTORIA_DOENCA_ATUAL = anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL;
                    vm.PAAN_DS_DIAGNOSTICO_1 = anamnese.PAAM_DS_DIAGNOSTICO_1;
                    vm.PAAN_DS_CONDUTA = anamnese.PAAM_DS_CONDUTA;
                    vm.PAAN_DT_DATA = anamnese.PAAM_DT_DATA;
                    Session["IdUltimaAnamnese"] = anamnese.PAAM_CD_ID;

                }
                Session["TemAnamnese"] = temAnamnese;

                // Carrega ultimo exame físico
                List<PACIENTE_EXAME_FISICOS> fisicos = item.PACIENTE_EXAME_FISICOS.Where(p => p.PAEF_DT_DATA.Value.Date <= DateTime.Today.Date & p.PAEF_IN_ATIVO == 1).ToList();
                PACIENTE_EXAME_FISICOS fisico = null;
                Int32 temFisico= 0;
                if (fisicos.Count() > 0)
                {
                    temFisico = 1;
                    fisico = fisicos.OrderByDescending(p => p.PAEF_DT_DATA).ToList().FirstOrDefault();
                    vm.PAEF_DT_DATA = fisico.PAEF_DT_DATA;
                    vm.PAEF_NR_PA_ALTA = fisico.PAEF_NR_PA_ALTA;
                    vm.PAEF_NR_PA_BAIXA = fisico.PAEF_NR_PA_BAIXA;
                    vm.PAEF_NR_FREQUENCIA_CARDIACA = fisico.PAEF_NR_FREQUENCIA_CARDIACA;
                    vm.PAEF_NR_PESO = fisico.PAEF_NR_PESO;
                    vm.PAEF_NR_TEMPERATURA = fisico.PAEF_NR_TEMPERATURA;
                    vm.PAEF_NR_ALTURA = fisico.PAEF_NR_ALTURA;
                    vm.PAEF_VL_IMC = fisico.PAEF_VL_IMC;
                    vm.PAEF_DS_EXAME_FISICO = fisico.PAEF_DS_EXAME_FISICO;
                    Session["IdUltimoFisico"] = fisico.PAEF_CD_ID;
                }
                Session["TemFisico"] = temFisico;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarPaciente(PacienteViewModel vm)
        {
            // Instancia caches
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Preparação
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<TIPO_PACIENTE> tps = cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]);
            ViewBag.Tipos = new SelectList(tps, "TIPA_CD_ID", "TIPA_NM_NOME");
            Session["TipoPacienteAlterada"] = 0;
            List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
            ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");
            List<SEXO> sexos = cache.CarregaCacheGeralListaSexo("SEXO", usuario.ASSI_CD_ID, 0);
            ViewBag.Sexo = new SelectList(sexos, "SEXO_CD_ID", "SEXO_NM_NOME");
            List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
            ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
            List<COR> cores = cache.CarregaCacheGeralListaCor("COR", usuario.ASSI_CD_ID, 0);
            ViewBag.Cor = new SelectList(cores, "COR_CD_ID", "COR_NM_NOME");
            List<ESTADO_CIVIL> ests = cache.CarregaCacheGeralListaEstadoCivil("ESTADO_CIVIL", usuario.ASSI_CD_ID, 0);
            ViewBag.EstadoCivil = new SelectList(ests, "ESCI_CD_ID", "ESCI_NM_NOME");
            List<CONVENIO> convs = cache.CarregaCacheGeralListaConvenio("CONVENIO", usuario.ASSI_CD_ID, 0);
            ViewBag.Convenios = new SelectList(convs, "CONV_CD_ID", "CONV_NM_NOME");
            ViewBag.Nacionalidades = new SelectList(cache.CarregaCacheGeralListaNacionalidade("NACIONALIDADE", usuario.ASSI_CD_ID, 0), "NACI_CD_ID", "NACI_NM_NOME");
            ViewBag.Municipios = new SelectList(cache.CarregaCacheGeralListaMunicipio("MUNICIPIO", usuario.ASSI_CD_ID, 0), "MUNI_CD_ID", "MUNI_NM_NOME");
            List<GRAU_INSTRUCAO> graus = cache.CarregaCacheGeralListaGrauInstrucao("GRAU_INSTRUCAO", usuario.ASSI_CD_ID, 0);
            ViewBag.GrauInstrucao = new SelectList(cores, "GRAU_CD_ID", "GRAU_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            List<SelectListItem> ufNatur = new List<SelectListItem>();
            List<SelectListItem> ufNatur1 = new List<SelectListItem>();
            foreach (UF uf in uFs)
            {
                ufNatur.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
                ufNatur1.Add(new SelectListItem() { Text = uf.UF_NM_NOME, Value = uf.UF_CD_ID.ToString() });
            }
            ViewBag.UFNatur1 = new SelectList(ufNatur, "Value", "Text");
            ViewBag.UFNatur2 = new SelectList(ufNatur1, "Value", "Text");

            // Processo
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PACI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NOME);
                    vm.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CPF);
                    vm.PACI_NM_SOCIAL = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_SOCIAL);
                    vm.PACI_NM_PAI = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_PAI);
                    vm.PACI_NM_MAE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_MAE);
                    vm.PACI_NR_CEP = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CEP);
                    vm.PACI_NR_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_COMPLEMENTO);
                    vm.PACI_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_ENDERECO);
                    vm.PACI_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_NUMERO);
                    vm.PACI_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_BAIRRO);
                    vm.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_CIDADE);
                    vm.PACI_NM_PROFISSAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_PROFISSAO);
                    vm.PACI_NM_NATURALIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NATURALIDADE);
                    vm.PACI_NM_NACIONALIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_NACIONALIDADE);
                    vm.PACI_NR_RG = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_RG);
                    vm.PACI_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_TELEFONE);
                    vm.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_CELULAR);
                    vm.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.PACI_NM_EMAIL);
                    vm.PACI_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_MATRICULA);
                    vm.PACI_NM_INDICACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NM_INDICACAO);
                    vm.PACI_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_TX_OBSERVACOES);
                    vm.PACI__CD_ID = (Int32)Session["IdPaciente"];

                    // Criticas
                    if (vm.PACI_NM_NOME == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0527", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACI_DT_NASCIMENTO.Value.Date >= DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0531", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de tabelas auxiliares
                    if (vm.TIPA_CD_ID == null || vm.TIPA_CD_ID == 0)
                    {
                        vm.TIPA_CD_ID = tps.Where(p => p.TIPA_NM_NOME.Contains("Informad")).FirstOrDefault().TIPA_CD_ID;
                    }
                    if (vm.CONV_CD_ID == null || vm.CONV_CD_ID == 0)
                    {
                        vm.CONV_CD_ID = convs.Where(p => p.CONV_NM_NOME.Contains("Informad")).FirstOrDefault().CONV_CD_ID;
                    }
                    if (vm.SEXO_CD_ID == null || vm.SEXO_CD_ID == 0)
                    {
                        vm.SEXO_CD_ID = sexos.Where(p => p.SEXO_NM_NOME.Contains("Informad")).FirstOrDefault().SEXO_CD_ID;
                    }
                    if (vm.COR1_CD_ID == null || vm.COR1_CD_ID == 0)
                    {
                        vm.COR1_CD_ID = cores.Where(p => p.COR1_NM_NOME.Contains("Informad")).FirstOrDefault().COR1_CD_ID;
                    }
                    if (vm.ESCI_CD_ID == null || vm.ESCI_CD_ID == 0)
                    {
                        vm.ESCI_CD_ID = ests.Where(p => p.ESCI_NM_NOME.Contains("Informad")).FirstOrDefault().ESCI_CD_ID;
                    }
                    if (vm.GRAU_CD_ID == null || vm.GRAU_CD_ID == 0)
                    {
                        vm.GRAU_CD_ID = graus.Where(p => p.GRAU_NM_NOME.Contains("Informad")).FirstOrDefault().GRAU_CD_ID;
                    }

                    // Acerta dados
                    vm.PACI_DT_ALTERACAO = DateTime.Now;

                    // Executa a operação
                    Session["FlagAlteraPaciente"] = 1;
                    PACIENTE item = Mapper.Map<PacienteViewModel, PACIENTE>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica se não houve alteração
                    if (item == objetoAntes)
                    {
                        Session["FlagAlteraPaciente"] = 0;
                    }

                    // Atualiza estado
                    listaMaster = new List<PACIENTE>();
                    Session["ListaPaciente"] = null;
                    Session["ListaPacienteBase"] = null;
                    Session["IncluirPaciente"] = 0;
                    Session["PacienteAlterada"] = 1;
                    Session["LinhaAlterada"] = item.PACI__CD_ID;
                    Session["FlagPaciente"] = 1;

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI__CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 1;
                    hist.PAHI_IN_CHAVE = item.PACI__CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição";
                    hist.PAHI_DS_DESCRICAO = "Edição do paciente: " + item.PACI_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Trata retorno
                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 2)
                    {
                        return RedirectToAction("MontarTelaSolicitacoes");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 3)
                    {
                        return RedirectToAction("MontarTelaAtestados");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 8)
                    {
                        return RedirectToAction("VerMedicamentosPacienteForm");
                    }
                    if (Session["FiltroPaciente"] != null)
                    {
                        FiltrarPaciente((PACIENTE)Session["FiltroPaciente"]);
                    }

                    if ((Int32)Session["VoltaPaciente"] == 2)
                    {
                        return RedirectToAction("VerCardsPaciente");
                    }

                    // Retorno normal
                    Session["MensPaciente"] = 66;
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Prepara listas
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Carrega paciente
                PACIENTE item = baseApp.GetItemById(id);
                Session["Paciente"] = item;

                // Carrega Listas
                List<PACIENTE_PRESCRICAO> listaPrescricao = item.PACIENTE_PRESCRICAO.Where(p => p.PAPR_IN_ATIVO == 1).ToList();
                Session["ListaPrescricao"] = listaPrescricao;
                List<PACIENTE_ATESTADO> listaAtestado = item.PACIENTE_ATESTADO.Where(p => p.PAAT_IN_ATIVO == 1).ToList();
                Session["ListaAtestado"] = listaAtestado;
                List<PACIENTE_SOLICITACAO> listaSolicitacao = item.PACIENTE_SOLICITACAO.Where(p => p.PASO_IN_ATIVO == 1).ToList();
                Session["ListaSolicitacao"] = listaSolicitacao;
                List<PACIENTE_EXAMES> listaExame = item.PACIENTE_EXAMES.Where(p => p.PAEX_IN_ATIVO == 1).ToList();
                Session["ListaExame"] = listaExame;
                List<PACIENTE_CONSULTA> listaConsulta = item.PACIENTE_CONSULTA.Where(p => p.PACO_IN_ATIVO == 1).ToList();
                Session["ListaConsulta"] = listaConsulta;
                List<PACIENTE_PRESCRICAO_ITEM> listaRemedio = item.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                Session["ListaRemedio"] = listaRemedio;

                // Trata mensagens
                if (Session["MensPaciente"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                }

                // Indicadores
                ViewBag.NivelPaciente = 1;

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 2;
                ViewBag.Incluir = 1;
                objetoAntes = item;
                Session["Paciente"] = item;
                Session["IdPaciente"] = id;
                Session["IdVolta"] = id;
                Session["VoltaCEP"] = 1;
                Session["VoltaMsg"] = 2;
                Session["NivelExame"] = 1;
                Session["MensPaciente"] = null;
                Session["PacienteAlterada"] = 1;
                Session["VoltaGrupo"] = 22;
                Session["VoltaAtestado"] = 2;
                Session["VoltaAnamnese"] = 1;
                Session["VoltaFisico"] = 1;
                Session["VoltarConsulta"] = 3;
                Session["ModoConsulta"] = 0;
                Session["EditarVer"] = 1;
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);

                // Carrega ultima anamnese
                List<PACIENTE_ANAMNESE> anamneses = item.PACIENTE_ANAMNESE.Where(p => p.PAAM_DT_DATA.Date <= DateTime.Today.Date & p.PAAM_IN_ATIVO == 1).ToList();
                PACIENTE_ANAMNESE anamnese = null;
                Int32 temAnamnese = 0;
                if (anamneses.Count() > 0)
                {
                    temAnamnese = 1;
                    anamnese = anamneses.OrderByDescending(p => p.PAAM_DT_DATA).ToList().FirstOrDefault();
                    vm.PAAN_DS_MOTIVO_CONSULTA = anamnese.PAAM_DS_MOTIVO_CONSULTA;
                    vm.PAAN_DS_QUEIXA_PRINCIPAL = anamnese.PAAM_DS_QUEIXA_PRINCIPAL;
                    vm.PAAN_DS_HISTORIA_FAMILIAR = anamnese.PAAM_DS_HISTORIA_FAMILIAR;
                    vm.PAAN_DS_HISTORIA_DOENCA_ATUAL = anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL;
                    vm.PAAN_DS_DIAGNOSTICO_1 = anamnese.PAAM_DS_DIAGNOSTICO_1;
                    vm.PAAN_DS_CONDUTA = anamnese.PAAM_DS_CONDUTA;
                    vm.PAAN_DT_DATA = anamnese.PAAM_DT_DATA;
                    Session["IdUltimaAnamnese"] = anamnese.PAAM_CD_ID;

                }
                Session["TemAnamnese"] = temAnamnese;

                // Carrega ultimo exame físico
                List<PACIENTE_EXAME_FISICOS> fisicos = item.PACIENTE_EXAME_FISICOS.Where(p => p.PAEF_DT_DATA.Value.Date <= DateTime.Today.Date & p.PAEF_IN_ATIVO == 1).ToList();
                PACIENTE_EXAME_FISICOS fisico = null;
                Int32 temFisico= 0;
                if (fisicos.Count() > 0)
                {
                    temFisico = 1;
                    fisico = fisicos.OrderByDescending(p => p.PAEF_DT_DATA).ToList().FirstOrDefault();
                    vm.PAEF_DT_DATA = fisico.PAEF_DT_DATA;
                    vm.PAEF_NR_PA_ALTA = fisico.PAEF_NR_PA_ALTA;
                    vm.PAEF_NR_PA_BAIXA = fisico.PAEF_NR_PA_BAIXA;
                    vm.PAEF_NR_FREQUENCIA_CARDIACA = fisico.PAEF_NR_FREQUENCIA_CARDIACA;
                    vm.PAEF_NR_PESO = fisico.PAEF_NR_PESO;
                    vm.PAEF_NR_TEMPERATURA = fisico.PAEF_NR_TEMPERATURA;
                    vm.PAEF_NR_ALTURA = fisico.PAEF_NR_ALTURA;
                    vm.PAEF_VL_IMC = fisico.PAEF_VL_IMC;
                    vm.PAEF_DS_EXAME_FISICO = fisico.PAEF_DS_EXAME_FISICO;
                    Session["IdUltimoFisico"] = fisico.PAEF_CD_ID;
                }
                Session["TemFisico"] = temFisico;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadPaciente(Int32 id)
        {
            try
            {
                PACIENTE_ANEXO item = baseApp.GetAnexoById(id);
                String arquivo = item.PAAX_AQ_ARQUIVO;
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = arquivo.Substring(pos);
                String contentType = string.Empty;
                if (arquivo.Contains(".pdf"))
                {
                    contentType = "application/pdf";
                }
                else if (arquivo.Contains(".jpg") || arquivo.Contains(".jpeg"))
                {
                    contentType = "image/jpg";
                }
                else if (arquivo.Contains(".png"))
                {
                    contentType = "image/png";
                }
                else if (arquivo.Contains(".docx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (arquivo.Contains(".xlsx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else if (arquivo.Contains(".pptx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                }
                else if (arquivo.Contains(".mp3"))
                {
                    contentType = "audio/mpeg";
                }
                else if (arquivo.Contains(".mpeg"))
                {
                    contentType = "audio/mpeg";
                }
                Session["NivelPaciente"] = 2;
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

        public ActionResult UploadFotoPaciente(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPaciente"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Recupera cliente
                PACIENTE item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }


                // Copia imagem
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item.PACI_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                listaMaster = new List<PACIENTE>();
                Session["ListaPaciente"] = null;
                Session["NivelPaciente"] = 2;
                Session["PacienteAlterada"] = 1;

                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPaciente(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                PACIENTE_ANEXO item = baseApp.GetAnexoById(id);
                Session["NivelPaciente"] = 2;
                return View(item);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPacienteAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                PACIENTE_ANEXO item = baseApp.GetAnexoById(id);
                Session["NivelPaciente"] = 2;
                return View(item);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_ANEXO item = baseApp.GetAnexoById(id);
                item.PAAX_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xanPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " | Anexo: " + item.PAAX_NM_TITULO + " | Data: " + item.PAAX_DT_ANEXO.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 2;
                Session["PacienteAlterada"] = 1;

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACIENTE.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 1;
                hist.PAHI_IN_CHAVE = item.PACIENTE.PACI__CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Anexo";
                hist.PAHI_DS_DESCRICAO = "Anexo excluído: " + item.PAAX_NM_TITULO;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAnotacaoPaciente()
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Alteração";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processo
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE item = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_ANOTACAO coment = new PACIENTE_ANOTACAO();
                PacienteAnotacaoViewModel vm = Mapper.Map<PACIENTE_ANOTACAO, PacienteAnotacaoViewModel>(coment);
                vm.PAAN_DT_ANOTACAO = DateTime.Now;
                vm.PAAN_IN_ATIVO = 1;
                vm.PACI_CD_ID = item.PACI__CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirAnotacaoPaciente(PacienteAnotacaoViewModel vm)
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
                    vm.PAAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_TX_ANOTACAO);

                    // Executa a operação
                    PACIENTE_ANOTACAO item = Mapper.Map<PacienteAnotacaoViewModel, PACIENTE_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PACIENTE not = baseApp.GetItemById((Int32)Session["IdPaciente"]);

                    item.USUARIO = null;
                    not.PACIENTE_ANOTACAO.Add(item);
                    objetoAntes = not;
                    Int32 volta = baseApp.ValidateEdit(not, objetoAntes);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "aanPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " | Data: " + item.PAAN_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAAN_TX_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 5;
                    return RedirectToAction("EditarPaciente", new { id = (Int32)Session["IdPaciente"] });
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Alteração";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["VoltaTela"] = 2;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_ANOTACAO item = baseApp.GetAnotacaoById(id);
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID);
                PacienteAnotacaoViewModel vm = Mapper.Map<PACIENTE_ANOTACAO, PacienteAnotacaoViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoPaciente(PacienteAnotacaoViewModel vm)
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
                    vm.PAAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PACIENTE_ANOTACAO item = Mapper.Map<PacienteAnotacaoViewModel, PACIENTE_ANOTACAO>(vm);
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eanPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + pac.PACI_NM_NOME + " | Data: " + item.PAAN_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAAN_TX_ANOTACAO,
                        LOG_IN_SISTEMA = 2
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Alteração";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                Session["VoltaTela"] = 2;
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_ANOTACAO item = baseApp.GetAnotacaoById(id);
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                item.PAAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 5;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xanPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + pac.PACI_NM_NOME + " | Data: " + item.PAAN_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAAN_TX_ANOTACAO,
                    LOG_IN_SISTEMA = 2
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerGrupoTela(Int32 id)
        {
            Session["VoltaTela"] = 3;
            ViewBag.Incluir = (Int32)Session["VoltaTela"];
            Session["VoltaCliGrupo"] = 10;
            Session["VoltaGrupo"] = 11;
            Session["NivelPaciente"] = 11;
            return RedirectToAction("VerGrupo", "Grupo", new { id = id });
        }

        [HttpGet]
        public ActionResult ExcluirGrupoPaciente(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Pacientes - Alteração";
                    return RedirectToAction("MontarTelaPaciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                GRUPO_PACIENTE item = baseApp.GetGrupoById(id);
                item.GRCL_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditGrupo(item);
                Session["NivelPaciente"] = 11;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarGrupoPaciente(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Pacientes - Alteração";
                    return RedirectToAction("MontarTelaPaciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];

                // Instancia caches
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Verifica possibilidade
                Int32 num = cacheMens.CarregaCacheGeralListaGrupo("GRUPO", usuario.ASSI_CD_ID, (Int32)Session["GrupoAlterada"]).Count;
                Session["GrupoAlterada"] = 0;
                if ((Int32)Session["NumGrupos"] <= num)
                {
                    Session["MensPaciente"] = 555;
                    Session["NivelPaciente"] = 11;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Processo
                GRUPO_PACIENTE item = baseApp.GetGrupoById(id);
                item.GRCL_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditGrupo(item);
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirGrupoPaciente()
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
                if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Pacientes - Alteração";
                    return RedirectToAction("MontarTelaPaciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            try
            {
                // Instancia caches
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Verifica possibilidade
                List<GRUPO_PAC> grupos = cacheMens.CarregaCacheGeralListaGrupo("GRUPO", usuario.ASSI_CD_ID, (Int32)Session["GrupoAlterada"]);
                Int32 num = grupos.Count;
                Session["GrupoAlterada"] = 0;
                if ((Int32)Session["NumGrupos"] <= num)
                {
                    Session["MensPaciente"] = 555;
                    Session["NivelPaciente"] = 11;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Processo
                Session["VoltaTela"] = 3;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    grupos = grupos.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.Grupos = new SelectList(grupos.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
                GRUPO_PACIENTE item = new GRUPO_PACIENTE();
                GrupoContatoViewModel vm = Mapper.Map<GRUPO_PACIENTE, GrupoContatoViewModel>(item);
                vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                vm.GRCL_IN_ATIVO = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirGrupoPaciente(GrupoContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Verifica possibilidade
            List<GRUPO_PAC> grupos = cacheMens.CarregaCacheGeralListaGrupo("GRUPO", usuario.ASSI_CD_ID, (Int32)Session["GrupoAlterada"]);
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                grupos = grupos.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            ViewBag.Grupos = new SelectList(grupos.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");

            // Processo
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    GRUPO_PACIENTE item = Mapper.Map<GrupoContatoViewModel, GRUPO_PACIENTE>(vm);
                    Int32 volta = baseApp.ValidateCreateGrupo(item);

                    // Verifica retorno
                    Session["NivelPaciente"] = 11;
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Carrega listas
                Session["GrausParente"] = null;
                List<GRAU_PARENTESCO> graus = cache.CarregaCacheGeralListaGrauParentesco("GRAU_PARENTE", usuario.ASSI_CD_ID, 0);
                ViewBag.Graus = new SelectList(graus, "GRPA_CD_ID", "GRPA_NM_NOME");
                Session["NivelPaciente"] = 10;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteContatoViewModel vm = Mapper.Map<PACIENTE_CONTATO, PacienteContatoViewModel>(item);
                Session["Contato"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContato(PacienteContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            Session["GrausParente"] = null;
            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Carrega listas
            Session["GrausParente"] = null;
            List<GRAU_PARENTESCO> graus = cache.CarregaCacheGeralListaGrauParentesco("GRAU_PARENTE", usuario.ASSI_CD_ID, 0);
            ViewBag.Graus = new SelectList(graus, "GRPA_CD_ID", "GRPA_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PACO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACO_NM_NOME);
                    vm.PACO_EM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.PACO_EM_EMAIL);
                    vm.PACO_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACO_NR_TELEFONE);
                    vm.PACO_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACO_NR_CELULAR);

                    // Critica de tabelas auxiliares
                    if (vm.GRPA_CD_ID == null || vm.GRPA_CD_ID == 0)
                    {
                        vm.GRPA_CD_ID = graus.Where(p => p.GRPA_NM_NOME.Contains("Informad")).FirstOrDefault().GRPA_CD_ID;
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PACIENTE_CONTATO item = Mapper.Map<PacienteContatoViewModel, PACIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateEditContato(item);

                    // Monta Log
                    PACIENTE_CONTATO cont = (PACIENTE_CONTATO)Session["Contato"];
                    PACIENTE cli = baseApp.GetItemById(cont.PACI_CD_ID.Value);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ecoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Contato: " + item.PACO_NM_NOME,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 10;

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.PACI_CD_ID = cli.PACI__CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 2;
                    hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Contato";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + cli.PACI_NM_NOME + " - Contato editado: " + item.PACO_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                Session["VoltaTela"] = 1;
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 10;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xcoPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Contato: " + item.PACO_NM_NOME,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = cli.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 2;
                hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Contato";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + cli.PACI_NM_NOME + " - Contato excluído: " + item.PACO_NM_NOME;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarContato(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Reativação";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_CONTATO item = baseApp.GetContatoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACO_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 10;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rcoPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Contato: " + item.PACO_NM_NOME,
                    LOG_IN_SISTEMA = 2
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACIENTE.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 2;
                hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Reativação de Contato";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " - Contato reativado: " + item.PACO_NM_NOME;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContato()
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
                    if (usuario.PERFIL.PERF_IN_ALTERAR_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Carrega listas
                Session["GrausParente"] = null;
                List<GRAU_PARENTESCO> graus = cache.CarregaCacheGeralListaGrauParentesco("GRAU_PARENTE", usuario.ASSI_CD_ID, 0);
                ViewBag.Graus = new SelectList(graus, "GRPA_CD_ID", "GRPA_NM_NOME");

                // Prepara view
                Session["GrausParente"] = null;
                Session["NivelPaciente"] = 10;
                Session["VoltaTela"] = 1;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                PACIENTE_CONTATO item = new PACIENTE_CONTATO();
                PacienteContatoViewModel vm = Mapper.Map<PACIENTE_CONTATO, PacienteContatoViewModel>(item);
                vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                vm.PACO_IN_ATIVO = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirContato(PacienteContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["GrausParente"] = null;

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Carrega listas
            Session["GrausParente"] = null;
            List<GRAU_PARENTESCO> graus = cache.CarregaCacheGeralListaGrauParentesco("GRAU_PARENTE", usuario.ASSI_CD_ID, 0);
            ViewBag.Graus = new SelectList(graus, "GRPA_CD_ID", "GRPA_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PACO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACO_NM_NOME);
                    vm.PACO_EM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.PACO_EM_EMAIL);
                    vm.PACO_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACO_NR_TELEFONE);
                    vm.PACO_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACO_NR_CELULAR);

                    // Critica de tabelas auxiliares
                    if (vm.GRPA_CD_ID == null || vm.GRPA_CD_ID == 0)
                    {
                        vm.GRPA_CD_ID = graus.Where(p => p.GRPA_NM_NOME.Contains("Informad")).FirstOrDefault().GRPA_CD_ID;
                    }

                    // Executa a operação
                    PACIENTE_CONTATO item = Mapper.Map<PacienteContatoViewModel, PACIENTE_CONTATO>(vm);
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    
                    // Verifica retorno
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 10;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_CONTATO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);
                    Session["NivelPaciente"] = 10;

                    // Grava historico
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 2;
                    hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Contato";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Contato incluído: " + item.PACO_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EnviarEMailContato(Int32 id)
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
                    if (usuario.PERFIL.PAERF_IN_PACIENTE_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Mensagem";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = (List<MENSAGENS_ENVIADAS_SISTEMA>)cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensPaciente"] = 400;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Recupera contato
                PACIENTE_CONTATO cont = baseApp.GetContatoById(id);
                Session["Contato"] = cont;
                ViewBag.Contato = cont;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_4.pdf";

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.PACO_NM_NOME;
                mens.ID = id;
                mens.MODELO = cont.PACO_EM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = cont.PACO_EM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem para Contato de Paciente: " + cont.PACO_NM_NOME;
                mens.PACI_CD_ID = cont.PACI_CD_ID;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarEMailContato(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE_CONTATO cont = (PACIENTE_CONTATO)Session["Contato"];
            PACIENTE pac = (PACIENTE)Session["Paciente"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.MENS_NM_LINK);
                    vm.MENS_NM_NOME = "Contato de Paciente: " + cont.PACO_NM_NOME;

                    // Prepara cabeçalho
                    String cab = "Prezado Sr(a). <b>" + cont.PACO_NM_NOME + "</b><br />";

                    // Prepara assinatura
                    vm.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                    String rod = String.Empty;
                    rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";

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

                    // Monta recursividade
                    RECURSIVIDADE rec = new RECURSIVIDADE();
                    rec.ASSI_CD_ID = idAss;
                    rec.MENS_CD_ID = null;
                    rec.EMPR_CD_ID = usuario.EMPR_CD_ID;
                    rec.RECU_IN_TIPO_MENSAGEM = 1;
                    rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                    rec.RECU_IN_TIPO_SMS = 0;
                    rec.RECU_NM_NOME = "Envio de Mensagem para Contato - " + cont.PACO_NM_NOME + " - " + DateTime.Now.ToString();
                    rec.RECU_LK_LINK = null;
                    rec.RECU_IN_SISTEMA = 6;
                    rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                    rec.RECU_IN_TIPO_ENVIO = 2;
                    rec.RECU_TX_TEXTO = emailBody;
                    rec.RECU_IN_ATIVO = 1;

                    // Monta destinos
                    RECURSIVIDADE_DESTINO dest1 = new RECURSIVIDADE_DESTINO();
                    dest1.FORN_CD_ID = null;
                    dest1.CLIE_CD_ID = null;
                    dest1.REDE_EM_EMAIL = cont.PACO_EM_EMAIL;
                    dest1.REDE_NM_NOME = cont.PACO_NM_NOME;
                    dest1.REDE_TX_CORPO = emailBody;
                    dest1.REDE_IN_ATIVO = 1;
                    rec.RECURSIVIDADE_DESTINO.Add(dest1);

                    // Monta Datas
                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                    data1.REDA_DT_PROGRAMADA = DateTime.Now.AddMinutes(10);
                    data1.REDA_IN_PROCESSADA = 0;
                    data1.REDA_IN_ATIVO = 1;
                    rec.RECURSIVIDADE_DATA.Add(data1);

                    // Grava recursividade
                    Int32 voltaRec = recuApp.ValidateCreate(rec, usuario);

                    // Grava envio
                    EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                    Int32 voltaZ = await envio.ProcessaEnvioEMailGeral(vm, usuario);
                    String guid = Xid.NewXid().ToString();
                    Int32 volta1 = envio.GravarMensagemEnviada(vm, usuario, vm.MENS_TX_TEXTO, "Success", guid, null, "Mensagem E-Mail para Cliente");

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "emcPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.PACO_NM_NOME + " | Data:" + DateTime.Today.Date.ToShortDateString(),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = pac.PACI__CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 2;
                    hist.PAHI_IN_CHAVE = cont.PACO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Envio de Mensagem E-Mail para Contato";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Contato:  " + cont.PACO_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Sucesso
                    Session["NivelPaciente"] = 10;
                    Session["VoltaPaciente"] = 1;
                    if ((Int32)Session["EditarVer"] == 1)
                    {
                        return RedirectToAction("VoltarVerPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EnviarSMSContato(Int32 id)
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
                    if (usuario.PERFIL.PAERF_IN_PACIENTE_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Contatos - Mensagem";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = (List<MENSAGENS_ENVIADAS_SISTEMA>)cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade
                Session["NivelPaciente"] = 10;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensPaciente"] = 401;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                PACIENTE_CONTATO item = baseApp.GetContatoById(id);
                Session["Contato"] = item;
                ViewBag.Contato = item;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_5.pdf";

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.PACO_NM_NOME;
                mens.ID = id;
                mens.MODELO = item.PACO_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.MENS_NM_NOME = "Mensagem para Contato de Paciente: " + item.PACO_NM_NOME;
                mens.PACI_CD_ID = item.PACI_CD_ID;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarSMSContato(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.MENS_NM_LINK);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSContato(vm, usuarioLogado);

                    // Monta Log
                    PACIENTE_CONTATO cont = (PACIENTE_CONTATO)Session["Contato"];
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "smcPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.PACO_NM_NOME + " | Data:" + DateTime.Today.Date.ToShortDateString(),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 10;
                    if ((Int32)Session["EditarVer"] == 1)
                    {
                        return RedirectToAction("VoltarVerPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ConsultarPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Acesso";
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Carrega paciente
                PACIENTE item = baseApp.GetItemById(id);

                // Trata mensagens
                if (Session["MensPaciente"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 201)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 555)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0516", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 400)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 401)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                }

                // Indicadores
                Session["Nivel"] = 1;
                ViewBag.NivelPaciente = (Int32)Session["NivelPaciente"];

                // Carrega view
                Session["FlagMensagensEnviadas"] = 2;
                ViewBag.Incluir = 1;
                Session["VoltaPaciente"] = 1;
                objetoAntes = item;
                Session["Paciente"] = item;
                Session["IdPaciente"] = id;
                Session["IdVolta"] = id;
                Session["VoltaCEP"] = 1;
                Session["VoltaMsg"] = 2;
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirPrescricaoDireto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdPaciente"] = id;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirPrescricaoNova");
        }

        [HttpGet]
        public ActionResult IncluirPrescricaoNova()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Precrição - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Prepara view
                ViewBag.TipoControle = new SelectList(cache.CarregaCacheGeralListaTipoControle("TIPO_CONTROLE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TICO_CD_ID", "TICO_NM_NOME");
                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");
                ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "PACI_CD_ID", "PACI_NM_NOME");

                Session["NivelPaciente"] = 8;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_1.pdf";
                PACIENTE_PRESCRICAO item = new PACIENTE_PRESCRICAO();
                PacientePrescricaoViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO, PacientePrescricaoViewModel>(item);
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    PACIENTE pac = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                }
                vm.PAPR_IN_ATIVO = 1;
                vm.PAPR_DT_DATA = DateTime.Today.Date;
                vm.PAPR_GU_GUID = Xid.NewXid().ToString();
                if ((Int32)Session["ModoConsulta"] != 0 )
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                    PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                    vm.PACIENTE_CONSULTA = consulta;
                }
                else
                {
                    vm.PACO_CD_ID = null;
                }
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.PAPR_GU_GUID_ENVIO = null;
                vm.PAPR_IN_ENVIADO = 0;
                vm.PAPR_DT_ENVIO = null;
                vm.PAPR_IN_PDF = 0;
                vm.PAPR_NR_ENVIOS = 0;
                vm.PAPR_DT_GERACAO_PDF = null;
                vm.PAPR_DT_EMISSAO = DateTime.Now;
                vm.PAPR_IN_DATA = 1;
                vm.TICO_CD_ID = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IncluirPrescricaoNova(PacientePrescricaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Prepara view
            ViewBag.TipoControle = new SelectList(cache.CarregaCacheGeralListaTipoControle("TIPO_CONTROLE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TICO_CD_ID", "TICO_NM_NOME");
            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");
            ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "PACI_CD_ID", "PACI_NM_NOME");
           
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAPR_NM_REMEDIO = "-";
                    vm.PAPR_IN_ENVIADO = 0;

                    // Critica
                    if (vm.PAPR_IN_DATA == null)
                    {
                        vm.PAPR_IN_DATA = 1;
                    }
                    if (vm.TICO_CD_ID == null || vm.TICO_CD_ID == 0)
                    {
                        vm.TICO_CD_ID = 1;
                    }

                    // Executa a operação
                    PACIENTE_PRESCRICAO item = Mapper.Map<PacientePrescricaoViewModel, PACIENTE_PRESCRICAO>(vm);
                    Int32 volta = baseApp.ValidateCreatePrescricao(item);

                    // Verifica retorno
                    Session["IdPrescricao"] = item.PAPR_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 8;
                    Session["PrescricoesAlterada"] = 1;
                    Session["ListaPrescricoes"] = null;
                    Session["ListaPrescricao"] = null;
                    Session["IdPaciente"] = item.PACI_CD_ID;

                    // Gerar e gravar QRCode
                    String fileNameQR = "Prescricao_QRCode_" + item.PAPR_GU_GUID + ".png";
                    String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/QRCode/";
                    String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);

                    String fileNameOrigem = "qrcode.png";
                    String caminhoOrigem = "/Imagens/Base/";
                    String pathOrigem = Path.Combine(Server.MapPath(caminhoOrigem), fileNameOrigem);
                    System.IO.File.Copy(pathOrigem, pathQR);

                    PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                    prescricao.PAPR_AQ_ARQUIVO_QRCODE = "~" + caminhoQR + fileNameQR;
                    Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "iprPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_PRESCRICAO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 3;
                    hist.PAHI_IN_CHAVE = item.PAPR_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Prescrição";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição incluída: " + item.PAPR_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    return RedirectToAction("EditarPrescricaoNova", new { id = (Int32)Session["IdPrescricao"] });
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirPrescricaoItem()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Precrição - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara view
                ViewBag.TipoForma = new SelectList(cache.CarregaCacheGeralListaTipoForma("TIPO_FORMA", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIFO_CD_ID", "TIFO_NM_NOME");
                Session["NivelPaciente"] = 8;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_3.pdf";

                PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById((Int32)Session["IdPrescricao"]);
                PACIENTE paciente = baseApp.GetItemById(prescricao.PACI_CD_ID);
                PACIENTE_PRESCRICAO_ITEM item = new PACIENTE_PRESCRICAO_ITEM();
                List<PACIENTE_PRESCRICAO_ITEM> itens = prescricao.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                ViewBag.Itens = itens;
                PacientePrescricaoItemViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO_ITEM, PacientePrescricaoItemViewModel>(item);
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.PAPR_CD_ID = prescricao.PAPR_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.PAPI_IN_ATIVO = 1;
                vm.PACIENTE = paciente;
                vm.PACIENTE_PRESCRICAO = prescricao;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IncluirPrescricaoItem(PacientePrescricaoItemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara view
            ViewBag.TipoForma = new SelectList(cache.CarregaCacheGeralListaTipoForma("TIPO_FORMA", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIFO_CD_ID", "TIFO_NM_NOME");
            Session["NivelPaciente"] = 8;
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_3.pdf";

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAPI_NM_REMEDIO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAPI_NM_REMEDIO);
                    vm.PAPI_DS_POSOLOGIA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAPI_DS_POSOLOGIA);
                    vm.PAPI_NM_GENERICO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAPI_NM_GENERICO);
                    vm.PACIENTE_PRESCRICAO.PAPR_NM_REMEDIO = "-";

                    // Executa a operação
                    PACIENTE_PRESCRICAO_ITEM item = Mapper.Map<PacientePrescricaoItemViewModel, PACIENTE_PRESCRICAO_ITEM>(vm);
                    Int32 volta = baseApp.ValidateCreatePrescricaoItem(item);

                    // Verifica retorno
                    Session["IdPrescricao"] = item.PAPR_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 8;
                    Session["ListaMedicamentos"] = null;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ipiPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_PRESCRICAO_ITEM>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    PACIENTE_PRESCRICAO pres = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 4;
                    hist.PAHI_IN_CHAVE = item.PAPI_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Item de Prescrição";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição: " + pres.PAPR_GU_GUID + " - Item incluído: " + item.PAPI_NM_REMEDIO;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    return RedirectToAction("IncluirPrescricaoItem");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarItemPrescricao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Precrição - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 666)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M00494", CultureInfo.CurrentCulture) + " - " + (String)Session["InfoEstoque"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara view
                ViewBag.TipoForma = new SelectList(cache.CarregaCacheGeralListaTipoForma("TIPO_FORMA", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIFO_CD_ID", "TIFO_NM_NOME");
                Session["MensPaciente"] = null;
                Session["VoltaPrescricao"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_4.pdf";
                PACIENTE_PRESCRICAO_ITEM item = baseApp.GetPrescricaoItemById(id);
                PacientePrescricaoItemViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO_ITEM, PacientePrescricaoItemViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarItemPrescricao(PacientePrescricaoItemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara view
            ViewBag.TipoForma = new SelectList(cache.CarregaCacheGeralListaTipoForma("TIPO_FORMA", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIFO_CD_ID", "TIFO_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAPI_NM_REMEDIO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAPI_NM_REMEDIO);
                    vm.PAPI_DS_POSOLOGIA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAPI_DS_POSOLOGIA);
                    vm.PAPI_NM_GENERICO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAPI_NM_GENERICO);

                    // Executa a operação
                    PACIENTE_PRESCRICAO_ITEM item = Mapper.Map<PacientePrescricaoItemViewModel, PACIENTE_PRESCRICAO_ITEM>(vm);
                    Int32 volta = baseApp.ValidateEditPrescricaoItem(item);

                    // Verifica retorno
                    Session["IdPrescricao"] = item.PAPR_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 8;
                    Session["ListaMedicamentos"] = null;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epiPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_PRESCRICAO_ITEM>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    PACIENTE_PRESCRICAO pres = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 4;
                    hist.PAHI_IN_CHAVE = item.PAPI_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Item de Prescrição";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição: " + pres.PAPR_GU_GUID + " - Item editado: " + item.PAPI_NM_REMEDIO;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    return RedirectToAction("VoltarEditarPrescricao");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult ExcluirItemPrescricao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                // Executa a operação
                PACIENTE_PRESCRICAO_ITEM item = baseApp.GetPrescricaoItemById(id);
                item.PAPI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditPrescricaoItem(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xpiCRPI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.PAPI_NM_REMEDIO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);

                // Verifica retorno
                Session["IdPrescricao"] = item.PAPR_CD_ID;
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 8;

                // Grava historico
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                PACIENTE_PRESCRICAO pres = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 4;
                hist.PAHI_IN_CHAVE = item.PAPI_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Item de Prescrição";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição: " + pres.PAPR_GU_GUID + " - Item excluído: " + item.PAPI_NM_REMEDIO;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                if ((Int32)Session["VoltaPrescricao"] == 0)
                {
                    return RedirectToAction("VoltarEditarPrescricaoItem");
                }
                return RedirectToAction("IncluirItemPrescricao");
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
        public ActionResult EditarPrescricaoNova(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Precrição - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Prepara listas
                ViewBag.TipoControle = new SelectList(cache.CarregaCacheGeralListaTipoControle("TIPO_CONTROLE", usuario.ASSI_CD_ID, (Int32)Session["TipoControleAlterada"]), "TICO_CD_ID", "TICO_NM_NOME");
                Session["TipoControleAlterada"] = 0;
                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");

                Session["NivelPaciente"] = 8;
                Session["VoltaPrescricao"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_2.pdf";
                Session["VoltarPesquisa"] = 0;
                Session["ListaMedicamentos"] = null;

                PACIENTE_PRESCRICAO item = baseApp.GetPrescricaoById(id);
                List<PACIENTE_PRESCRICAO_ITEM> itens = item.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                ViewBag.Itens = itens;
                objetoAntes = (PACIENTE)Session["Paciente"];
                Session["IdPaciente"] = item.PACI_CD_ID;
                PacientePrescricaoViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO, PacientePrescricaoViewModel>(item);
                Session["Prescricao"] = item;
                Session["IdPrescricao"] = item.PAPR_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarPrescricaoNova(PacientePrescricaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Prepara listas
            ViewBag.TipoControle = new SelectList(cache.CarregaCacheGeralListaTipoControle("TIPO_CONTROLE", usuario.ASSI_CD_ID, (Int32)Session["TipoControleAlterada"]), "TICO_CD_ID", "TICO_NM_NOME");
            Session["TipoControleAlterada"] = 0;
            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAPR_NM_REMEDIO = "-";

                    // Executa a operação
                    PACIENTE_PRESCRICAO item = Mapper.Map<PacientePrescricaoViewModel, PACIENTE_PRESCRICAO>(vm);
                    Int32 volta = baseApp.ValidateEditPrescricao(item);

                    // Verifica retorno
                    Session["IdPrescricao"] = item.PAPR_CD_ID;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 8;
                    Session["PrescricoesAlterada"] = 1;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["ListaPrescricoes"] = null;
                    Session["ListaPrescricao"] = null;

                    // Geração da prescrição em PDF
                    PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                    PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    if (prescricao.TICO_CD_ID != 3)
                    {
                        if (prescricao.PACIENTE_PRESCRICAO_ITEM.Count > 0)
                        {
                            if (prescricao.PAPR_IN_ENVIADO == 0)
                            {
                                // Gera prescricao HTML
                                String fileName = "Prescricao_" + paciente.PACI_NM_NOME + "_" + prescricao.PAPR_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                                String caminho = "/Imagens/" + prescricao.ASSI_CD_ID.ToString() + "/Pacientes/" + prescricao.PACI_CD_ID.ToString() + "/Prescricao/";
                                String path = Path.Combine(Server.MapPath(caminho), fileName);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                String prescricaoHTML = GerarPrescricaoHTML();
                                System.IO.File.WriteAllText(path, prescricaoHTML);

                                // Transforma em PDF
                                String fileNamePDF = "Prescricao_" + paciente.PACI_NM_NOME + "_" + prescricao.PAPR_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                                String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                                if (System.IO.File.Exists(pathPDF))
                                {
                                    System.IO.File.Delete(pathPDF);
                                }
                                PdfCreator envio = new PdfCreator();
                                String prescricaoPDF = envio.ConvertHtmlToPdf(prescricaoHTML, fileNamePDF, pathPDF);

                                prescricao.PAPR_HT_TEXTO_HTML = prescricaoHTML;
                                prescricao.PAPR_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                                prescricao.PAPR_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                                prescricao.PAPR_IN_PDF = 1;
                                prescricao.PAPR_DT_GERACAO_PDF = DateTime.Today.Date;
                                Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);
                            }
                        }
                    }
                    else
                    {
                        prescricao.PAPR_HT_TEXTO_HTML = null;
                        prescricao.PAPR_AQ_ARQUIVO_HTML = null;
                        prescricao.PAPR_AQ_ARQUIVO_PDF = null;
                        prescricao.PAPR_IN_PDF = 0;
                        prescricao.PAPR_DT_GERACAO_PDF = null;
                        Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eprPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + paciente.PACI_NM_NOME + " - Prescrição: " + prescricao.PAPR_GU_GUID,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 3;
                    hist.PAHI_IN_CHAVE = item.PAPR_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Prescrição";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição editada: " + item.PAPR_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["ModoConsulta"] == 2)
                    {
                        return RedirectToAction("VerListaPrescricaoConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 5)
                    {
                        return RedirectToAction("MontarTelaPrescricoes");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 8)
                    {
                        return RedirectToAction("VerMedicamentosPacienteForm");
                    }
                    return RedirectToAction("MontarTelaAtestados");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPrescricao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Prescrição - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_PRESCRICAO item = baseApp.GetPrescricaoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PAPR_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditPrescricao(item);

                // Exclui itens da prescricao
                List<PACIENTE_PRESCRICAO_ITEM> itens = item.PACIENTE_PRESCRICAO_ITEM.ToList();
                foreach (PACIENTE_PRESCRICAO_ITEM it in itens)
                {
                    PACIENTE_PRESCRICAO_ITEM novo = new PACIENTE_PRESCRICAO_ITEM();
                    novo.ASSI_CD_ID = it.ASSI_CD_ID;
                    novo.PACI_CD_ID = it.PACI_CD_ID;
                    novo.PAPI_CD_ID = it.PAPI_CD_ID;
                    novo.PAPI_DS_POSOLOGIA = it.PAPI_DS_POSOLOGIA;
                    novo.PAPI_DT_DATA_1 = it.PAPI_DT_DATA_1;
                    novo.PAPI_DT_DATA_2 = it.PAPI_DT_DATA_2;
                    novo.PAPI_IN_ATIVO = 0;
                    novo.PAPI_NM_GENERICO = it.PAPI_NM_GENERICO;
                    novo.PAPI_NM_REMEDIO = it.PAPI_NM_REMEDIO;
                    novo.PAPR_CD_ID = it.PAPR_CD_ID;
                    novo.USUA_CD_ID = it.USUA_CD_ID;
                    novo.TIFO_CD_ID = it.TIFO_CD_ID;
                    Int32 x = baseApp.ValidateEditPrescricaoItem(novo);
                }

                // Retorno
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 8;
                Session["ListaPrescricoes"] = null;
                Session["ListaPrescricao"] = null;
                Session["PrescricoesAlterada"] = 1;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xprPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Data: " + item.PAPR_DT_DATA + " | Medicamento: " + item.PAPR_NM_REMEDIO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACIENTE.PACI__CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 3;
                hist.PAHI_IN_CHAVE = item.PAPR_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Prescrição";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Prescrição excluída: " + item.PAPR_GU_GUID;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VerListaPrescricaoConsulta");
                    }
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                return RedirectToAction("MontarTelaPrescricoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerPrescricaoNova(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Precrição - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 8;

                PACIENTE_PRESCRICAO item = baseApp.GetPrescricaoById(id);
                List<PACIENTE_PRESCRICAO_ITEM> itens = item.PACIENTE_PRESCRICAO_ITEM.Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                ViewBag.Itens = itens;
                objetoAntes = (PACIENTE)Session["Paciente"];
                Session["IdPaciente"] = item.PACI_CD_ID;
                PacientePrescricaoViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO, PacientePrescricaoViewModel>(item);
                Session["Prescricao"] = item;
                Session["IdPrescricao"] = item.PAPR_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAtestadoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirAtestado");
        }

        public ActionResult IncluirAtestadoDireto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdPaciente"] = id;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirAtestado");
        }

        [HttpGet]
        public ActionResult IncluirAtestado()
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoAtestado = new SelectList(cache.CarregaCacheGeralListaTipoAtestado("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["TipoAtestadoAlterada"]), "TIAT_CD_ID", "TIAT_NM_NOME");
                Session["TipoAtestadoAlterada"] = 0;
                ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
                Session["PacienteAlterada"] = 0;

                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_1.pdf";

                Session["NivelPaciente"] = 9;
                PACIENTE_ATESTADO item = new PACIENTE_ATESTADO();
                PacienteAtestadoViewModel vm = Mapper.Map<PACIENTE_ATESTADO, PacienteAtestadoViewModel>(item);
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    PACIENTE pac = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                }
                vm.PAAT_IN_ATIVO = 1;
                vm.PAAT_DT_DATA = DateTime.Today.Date;
                vm.PAAT_GU_GUID = Xid.NewXid().ToString();
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.PAAT_GU_GUID_ENVIO = null;
                vm.PAAT__IN_ENVIADO = 0;
                vm.PAAT_DT_ENVIO = null;
                vm.PAAT_IN_PDF = 0;
                vm.PAAT_NR_ENVIOS = 0;
                vm.PAAT_IN_DATA = 1;
                if ((Int32)Session["IdConsultaCria"] == 0)
                {
                    vm.PACO_CD_ID = null;
                }
                else
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                }
                if ((Int32)Session["ModoConsulta"] != 0 )
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                    PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                    vm.PACIENTE_CONSULTA = consulta;
                }
                else
                {
                    vm.PACO_CD_ID = null;
                }
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAtestado(PacienteAtestadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoAtestado = new SelectList(cache.CarregaCacheGeralListaTipoAtestado("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["TipoAtestadoAlterada"]), "TIAT_CD_ID", "TIAT_NM_NOME");
            Session["TipoAtestadoAlterada"] = 0;
            ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
            Session["PacienteAlterada"] = 0;

            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAAT_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAAT_NM_TITULO);
                    vm.PAAT_NM_DESTINO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAAT_NM_DESTINO);
                    vm.PAAT_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAT_TX_TEXTO);
                    vm.PAAT__IN_ENVIADO = 0;

                    // Critica
                    if (vm.PAAT_IN_DATA == null)
                    {
                        vm.PAAT_IN_DATA = 1;
                    }

                    // Executa a operação
                    PACIENTE pac = baseApp.GetItemById(vm.PACI_CD_ID.Value);
                    PACIENTE_ATESTADO item = Mapper.Map<PacienteAtestadoViewModel, PACIENTE_ATESTADO>(vm);
                    Int32 volta = baseApp.ValidateCreateAtestado(item);

                    // Verifica retorno
                    Session["IdAtestado"] = item.PAAT_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 9;
                    Session["AtestadosAlterada"] = 1;
                    Session["ListaAtestados"] = null;
                    Session["ListaAtestado"] = null;
                    Session["IdPaciente"] = item.PACI_CD_ID;

                    // Gerar e gravar QRCode
                    String fileNameQR = "Atestado_QRCode_" + item.PAAT_GU_GUID + ".png";
                    String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/QRCode/";
                    String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);

                    String fileNameOrigem = "qrcode.png";
                    String caminhoOrigem = "/Imagens/Base/";
                    String pathOrigem = Path.Combine(Server.MapPath(caminhoOrigem), fileNameOrigem);
                    System.IO.File.Copy(pathOrigem, pathQR);

                    PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById(item.PAAT_CD_ID);
                    atestado.PAAT_AQ_ARQUIVO_QRCODE = "~" + caminhoQR + fileNameQR;
                    Int32 voltaP = baseApp.ValidateEditAtestado(atestado);

                    // Geração do atestado em PDF
                    if (item.PAAT__IN_ENVIADO == 0)
                    {
                        // Gera atestado HTML
                        PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                        String fileName = "Atestado_" + paciente.PACI_NM_NOME + "_" + item.PAAT_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                        String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/Atestado/";
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        String atestadoHTML = GerarAtestadoHTML();
                        System.IO.File.WriteAllText(path, atestadoHTML);

                        // Transforma em PDF
                        String fileNamePDF = "Atestado_" + paciente.PACI_NM_NOME + "_" + item.PAAT_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                        String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                        if (System.IO.File.Exists(pathPDF))
                        {
                            System.IO.File.Delete(pathPDF);
                        }
                        PdfCreator envio = new PdfCreator();
                        String atestadoPDF = envio.ConvertHtmlToPdf(atestadoHTML, fileNamePDF, pathPDF);

                        // Acerta atestado
                        PACIENTE_ATESTADO atestado1 = baseApp.GetAtestadoById(atestado.PAAT_CD_ID);
                        atestado1 = baseApp.GetAtestadoById(item.PAAT_CD_ID);
                        atestado1.PAAT_HT_TEXTO_HTML = atestadoHTML;
                        atestado1.PAAT_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                        atestado1.PAAT_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                        atestado1.PAAT_IN_PDF = 1;
                        atestado1.PAAT__IN_ENVIADO = 0;
                        atestado1.PAAT_DT_GERACAO_PDF = DateTime.Today.Date;
                        Int32 voltaA = baseApp.ValidateEditAtestado(atestado1);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "iatPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_ATESTADO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac1 = baseApp.GetItemById(item.PACI_CD_ID.Value);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 5;
                    hist.PAHI_IN_CHAVE = item.PAAT_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Atestado";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac1.PACI_NM_NOME + " - Atestato incluído: " + item.PAAT_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["ModoConsulta"] == 2)
                    {
                        return RedirectToAction("VerListaAtestadoConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 3)
                    {
                        return RedirectToAction("MontarTelaAtestados");
                    }
                    return RedirectToAction("MontarTelaSolicitacoes");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoAtestado = new SelectList(cache.CarregaCacheGeralListaTipoAtestado("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["TipoAtestadoAlterada"]), "TIAT_CD_ID", "TIAT_NM_NOME");
                Session["TipoAtestadoAlterada"] = 0;
                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");
                Session["NivelPaciente"] = 9;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_2.pdf";

                PACIENTE_ATESTADO item = baseApp.GetAtestadoById(id);
                Session["IdPaciente"] = item.PACI_CD_ID;
                Session["IdAtestado"] = item.PAAT_CD_ID;
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteAtestadoViewModel vm = Mapper.Map<PACIENTE_ATESTADO, PacienteAtestadoViewModel>(item);
                Session["Atestado"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarAtestado(PacienteAtestadoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoAtestado = new SelectList(cache.CarregaCacheGeralListaTipoAtestado("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["TipoAtestadoAlterada"]), "TIAT_CD_ID", "TIAT_NM_NOME");
            Session["TipoAtestadoAlterada"] = 0;
            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAAT_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAAT_NM_TITULO);
                    vm.PAAT_NM_DESTINO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAAT_NM_DESTINO);
                    vm.PAAT_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAT_TX_TEXTO);
                    vm.PAAT__IN_ENVIADO = 0;

                    // Critica
                    if (vm.PAAT_IN_DATA == null)
                    {
                        vm.PAAT_IN_DATA = 1;
                    }

                    // Executa a operação
                    PACIENTE_ATESTADO item = Mapper.Map<PacienteAtestadoViewModel, PACIENTE_ATESTADO>(vm);
                    Int32 volta = baseApp.ValidateEditAtestado(item);

                    // Verifica retorno
                    Session["IdAtestado"] = item.PAAT_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 9;
                    Session["AtestadosAlterada"] = 1;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["ListaAtestados"] = null;
                    Session["ListaAtestado"] = null;

                    // Geração do atestado em PDF
                    if (item.PAAT__IN_ENVIADO == 0)
                    {
                        // Gera atestado HTML
                        PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                        String fileName = "Atestado_" + paciente.PACI_NM_NOME + "_" + item.PAAT_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                        String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/Atestado/";
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        String atestadoHTML = GerarAtestadoHTML();
                        System.IO.File.WriteAllText(path, atestadoHTML);

                        // Transforma em PDF
                        String fileNamePDF = "Atestado_" + paciente.PACI_NM_NOME + "_" + item.PAAT_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                        String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                        if (System.IO.File.Exists(pathPDF))
                        {
                            System.IO.File.Delete(pathPDF);
                        }
                        PdfCreator envio = new PdfCreator();
                        String atestadoPDF = envio.ConvertHtmlToPdf(atestadoHTML, fileNamePDF, pathPDF);

                        item = baseApp.GetAtestadoById(item.PAAT_CD_ID);
                        item.PAAT_HT_TEXTO_HTML = atestadoHTML;
                        item.PAAT_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                        item.PAAT_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                        item.PAAT_IN_PDF = 1;
                        item.PAAT__IN_ENVIADO = 0;
                        item.PAAT_DT_GERACAO_PDF = DateTime.Today.Date;
                        Int32 voltaA = baseApp.ValidateEditAtestado(item);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eatPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_ATESTADO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 5;
                    hist.PAHI_IN_CHAVE = item.PAAT_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Atestado";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Atestato editado: " + item.PAAT_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 3)
                    {
                        return RedirectToAction("MontarTelaAtestados");
                    }
                    return RedirectToAction("MontarTelaAtestados");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_ATESTADO item = baseApp.GetAtestadoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PAAT_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAtestado(item);

                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 9;
                Session["ListaAtestados"] = null;
                Session["ListaAtestado"] = null;
                Session["AtestadosAlterada"] = 1;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xatPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Data: " + item.PAAT_DT_DATA + " | Atestado: " + item.PAAT_NM_TITULO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 5;
                hist.PAHI_IN_CHAVE = item.PAAT_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Atestado";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Atestato excluído: " + item.PAAT_GU_GUID;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VerListaAtestadoConsulta");
                    }
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                return RedirectToAction("MontarTelaAtestados");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 9;

                PACIENTE_ATESTADO item = baseApp.GetAtestadoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteAtestadoViewModel vm = Mapper.Map<PACIENTE_ATESTADO, PacienteAtestadoViewModel>(item);
                Session["Atestado"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirSolicitacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirSolicitacao");
        }

        public ActionResult IncluirSolicitacaoDireto(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdPaciente"] = id;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirSolicitacao");
        }

        [HttpGet]
        public ActionResult IncluirSolicitacao()
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0527", CultureInfo.CurrentCulture));
                    }
                }

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoAtestadoAlterada"] = 0;
                ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
                Session["PacienteAlterada"] = 0;

                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");

                Session["NivelPaciente"] = 12;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/8/Ajuda8_1.pdf";
                PACIENTE_SOLICITACAO item = new PACIENTE_SOLICITACAO();
                PacienteSolicitacaoViewModel vm = Mapper.Map<PACIENTE_SOLICITACAO, PacienteSolicitacaoViewModel>(item);
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    PACIENTE pac = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                }
                vm.PASO_IN_ATIVO = 1;
                vm.PASO_DT_EMISSAO = DateTime.Today.Date;
                vm.PASO_GU_GUID = Xid.NewXid().ToString();
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.PASO_GU_GUID_ENVIO = null;
                vm.PASO_IN_ENVIADO = 0;
                vm.PASO_DT_ENVIO = null;
                vm.PASO_IN_PDF = 0;
                vm.PASO_NR_ENVIOS = 0;
                vm.PASO_IN_DATA = 1;
                if ((Int32)Session["IdConsultaCria"] == 0)
                {
                    vm.PACO_CD_ID = null;
                }
                else
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                }
                if ((Int32)Session["ModoConsulta"] != 0)
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                    PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                    vm.PACIENTE_CONSULTA = consulta;
                }
                else
                {
                    vm.PACO_CD_ID = null;
                }
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirSolicitacao(PacienteSolicitacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
            Session["TipoAtestadoAlterada"] = 0;
            ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
            Session["PacienteAlterada"] = 0;
            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PASO_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PASO_NM_TITULO);
                    vm.PASO_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PASO_TX_TEXTO);
                    vm.PASO_DS_INDICACAO_CLINICA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PASO_DS_INDICACAO_CLINICA);

                    // Critica
                    if (vm.PACI_CD_ID == 0 || vm.PACI_CD_ID == null)
                    {
                        Session["MensPaciente"] = 1;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0527", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PASO_IN_DATA == null)
                    {
                        vm.PASO_IN_DATA = 1;
                    }

                    // Executa a operação
                    PACIENTE_SOLICITACAO item = Mapper.Map<PacienteSolicitacaoViewModel, PACIENTE_SOLICITACAO>(vm);
                    Int32 volta = baseApp.ValidateCreateSolicitacao(item);

                    // Verifica retorno
                    Session["IdSolicitacao"] = item.PASO_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 12;
                    Session["SolicitacoesAlterada"] = 1;
                    Session["ListaSolicitacoes"] = null;
                    Session["ListaSolicitacao"] = null;
                    Session["IdPaciente"] = item.PACI_CD_ID;

                    // Gerar e gravar QRCode
                    String fileNameQR = "Solicitacao_QRCode_" + item.PASO_GU_GUID + ".png";
                    String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/QRCode/";
                    String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);

                    String fileNameOrigem = "qrcode.png";
                    String caminhoOrigem = "/Imagens/Base/";
                    String pathOrigem = Path.Combine(Server.MapPath(caminhoOrigem), fileNameOrigem);
                    System.IO.File.Copy(pathOrigem, pathQR);

                    PACIENTE_SOLICITACAO solicitacao = baseApp.GetSolicitacaoById(item.PASO_CD_ID);
                    solicitacao.PASO_AQ_ARQUIVO_QRCODE = "~" + caminhoQR + fileNameQR;
                    Int32 voltaP = baseApp.ValidateEditSolicitacao(solicitacao);

                    // Geração do atestado em PDF
                    if (item.PASO_IN_ENVIADO == 0)
                    {
                        // Gera solicitacao HTML
                        PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                        String fileName = "Solicitacao_" + paciente.PACI_NM_NOME + "_" + item.PASO_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                        String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/Solicitacao/";
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        String solicitacaoHTML = GerarSolicitacaoHTML();
                        System.IO.File.WriteAllText(path, solicitacaoHTML);

                        // Transforma em PDF
                        String fileNamePDF = "Solicitacao_" + paciente.PACI_NM_NOME + "_" + item.PASO_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                        String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                        if (System.IO.File.Exists(pathPDF))
                        {
                            System.IO.File.Delete(pathPDF);
                        }
                        PdfCreator envio = new PdfCreator();
                        String solicitacaoPDF = envio.ConvertHtmlToPdf(solicitacaoHTML, fileNamePDF, pathPDF);

                        // Acerta solicitacao
                        PACIENTE_SOLICITACAO solicitacao1 = baseApp.GetSolicitacaoById(solicitacao.PASO_CD_ID);
                        solicitacao1.PASO_HT_TEXT_HTML = solicitacaoHTML;
                        solicitacao1.PASO_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                        solicitacao1.PASO_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                        solicitacao1.PASO_IN_PDF = 1;
                        solicitacao1.PASO_DT_GERACAO_PDF = DateTime.Today.Date;
                        solicitacao1.PASO_IN_ENVIADO = 0;
                        Int32 voltaA = baseApp.ValidateEditSolicitacao(solicitacao1);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "isoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_SOLICITACAO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 6;
                    hist.PAHI_IN_CHAVE = item.PASO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Solicitação";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Solicitação incluída: " + item.PASO_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["ModoConsulta"] == 2)
                    {
                        return RedirectToAction("VerListaSolicitacaoConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    return RedirectToAction("MontarTelaSolicitacoes");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarSolicitacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoAtestadoAlterada"] = 0;
                var trata = new List<SelectListItem>();
                trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.TrataData = new SelectList(trata, "Value", "Text");
                Session["NivelPaciente"] = 12;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/8/Ajuda8_2.pdf";

                PACIENTE_SOLICITACAO item = baseApp.GetSolicitacaoById(id);
                Session["IdPaciente"] = item.PACI_CD_ID;
                Session["IdSolicitacao"] = item.PASO_CD_ID;
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteSolicitacaoViewModel vm = Mapper.Map<PACIENTE_SOLICITACAO, PacienteSolicitacaoViewModel>(item);
                Session["Solicitacao"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarSolicitacao(PacienteSolicitacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
            Session["TipoAtestadoAlterada"] = 0;
            var trata = new List<SelectListItem>();
            trata.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            trata.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.TrataData = new SelectList(trata, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PASO_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PASO_NM_TITULO);
                    vm.PASO_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PASO_TX_TEXTO);
                    vm.PASO_DS_INDICACAO_CLINICA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PASO_DS_INDICACAO_CLINICA);

                    // Critica
                    if (vm.PASO_IN_DATA == null)
                    {
                        vm.PASO_IN_DATA = 1;
                    }

                    // Executa a operação
                    PACIENTE_SOLICITACAO item = Mapper.Map<PacienteSolicitacaoViewModel, PACIENTE_SOLICITACAO>(vm);
                    Int32 volta = baseApp.ValidateEditSolicitacao(item);

                    // Verifica retorno
                    Session["IdSolicitacao"] = item.PASO_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 12;
                    Session["SolicitacoesAlterada"] = 1;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["ListaSolicitacoes"] = null;
                    Session["ListaSolicitacao"] = null;

                    // Geração do atestado em PDF
                    if (item.PASO_IN_ENVIADO == 0)
                    {
                        // Gera solicitacao HTML
                        PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                        String fileName = "Solicitacao_" + paciente.PACI_NM_NOME + "_" + item.PASO_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString() + ".htm");
                        String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI_CD_ID.ToString() + "/Solicitacao/";
                        String path = Path.Combine(Server.MapPath(caminho), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        String solicitacaoHTML = GerarSolicitacaoHTML();
                        System.IO.File.WriteAllText(path, solicitacaoHTML);

                        // Transforma em PDF
                        String fileNamePDF = "Solicitacao_" + paciente.PACI_NM_NOME + "_" + item.PASO_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString());
                        String pathPDF = Path.Combine(Server.MapPath(caminho), fileNamePDF);
                        if (System.IO.File.Exists(pathPDF))
                        {
                            System.IO.File.Delete(pathPDF);
                        }
                        PdfCreator envio = new PdfCreator();
                        String solicitacaoPDF = envio.ConvertHtmlToPdf(solicitacaoHTML, fileNamePDF, pathPDF);

                        // Acerta solicitacao
                        PACIENTE_SOLICITACAO solicitacao1 = baseApp.GetSolicitacaoById(item.PASO_CD_ID);
                        solicitacao1.PASO_HT_TEXT_HTML = solicitacaoHTML;
                        solicitacao1.PASO_AQ_ARQUIVO_HTML = "~" + caminho + fileName;
                        solicitacao1.PASO_AQ_ARQUIVO_PDF = "~" + caminho + fileNamePDF + ".pdf";
                        solicitacao1.PASO_IN_PDF = 1;
                        solicitacao1.PASO_DT_GERACAO_PDF = DateTime.Today.Date;
                        Int32 voltaA = baseApp.ValidateEditSolicitacao(solicitacao1);
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "esoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_SOLICITACAO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 6;
                    hist.PAHI_IN_CHAVE = item.PASO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Solicitação";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Solicitação editada: " + item.PASO_GU_GUID;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    return RedirectToAction("MontarTelaSolicitacoes");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirSolicitacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_SOLICITACAO item = baseApp.GetSolicitacaoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PASO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditSolicitacao(item);

                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 12;
                Session["ListaSolicitacoes"] = null;
                Session["ListaSolicitacao"] = null;
                Session["SolicitacoesAlterada"] = 1;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xsoPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Data: " + item.PASO_DT_EMISSAO + " | Solicitação: " + item.PASO_NM_TITULO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID.Value);
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 6;
                hist.PAHI_IN_CHAVE = item.PASO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Solicitação";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Solicitação excluída: " + item.PASO_GU_GUID;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                if ((Int32)Session["ModoConsulta"] == 1)
                {
                    return RedirectToAction("VoltarProcederConsulta");
                }
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                return RedirectToAction("MontarTelaSolicitacoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerSolicitacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 12;

                PACIENTE_SOLICITACAO item = baseApp.GetSolicitacaoById(id);
                Session["IdPaciente"] = item.PACI_CD_ID;
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteSolicitacaoViewModel vm = Mapper.Map<PACIENTE_SOLICITACAO, PacienteSolicitacaoViewModel>(item);
                Session["Solicitacao"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirExameForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirExame");
        }

        [HttpGet]
        public ActionResult IncluirExame()
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
                    if (usuario.PERFIL.PERF_IN_EXAME_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Exames - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoExameAlterada"] = 0;
                ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
                Session["PacienteAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara objeto
                Session["NivelPaciente"] = 7;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_1.pdf";

                PACIENTE_EXAMES item = new PACIENTE_EXAMES();
                PacienteExameViewModel vm = Mapper.Map<PACIENTE_EXAMES, PacienteExameViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.PAEX_DT_DATA = DateTime.Today.Date;
                vm.PAEX_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.TIEX_CD_ID = 0;
                if ((Int32)Session["IdConsultaCria"] == 0)
                {
                    vm.PACO_CD_ID = null;
                }
                else
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                }
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    PACIENTE pac = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                }
                if ((Int32)Session["ModoConsulta"] != 0)
                {
                    vm.PACO_CD_ID = (Int32)Session["IdConsulta"];
                    PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                    vm.PACIENTE_CONSULTA = consulta;
                }
                else
                {
                    vm.PACO_CD_ID = null;
                }
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirExame(PacienteExameViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
            Session["TipoExameAlterada"] = 0;
            ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
            Session["PacienteAlterada"] = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAEX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEX_NM_NOME);
                    vm.PAEX_DS_COMENTARIOS = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEX_DS_COMENTARIOS);
                    vm.PAEX_DS_DIAGNOSTICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEX_DS_DIAGNOSTICO);

                    // Monta paciente
                    PACIENTE_EXAMES item = Mapper.Map<PacienteExameViewModel, PACIENTE_EXAMES>(vm);

                    // Executa
                    Int32 volta = baseApp.ValidateCreateExame(item);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Exames/" + item.PAEX_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Verifica retorno
                    Session["IdExame"] = item.PAEX_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 7;
                    Session["ExamesAlterada"] = 1;
                    Session["ListaExames"] = null;
                    Session["ListaExame"] = null;
                    Session["IdPaciente"] = item.PACI_CD_ID;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "iexPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_EXAMES>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Trata anexos
                    if (Session["FileQueuePaciente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePaciente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueExamePaciente(file);
                            }
                        }
                        Session["FileQueuePaciente"] = null;
                    }

                    // Acerta exame
                    if ((Int32)Session["ModoConsulta"] != 0)
                    {
                        PACIENTE_EXAMES exame = baseApp.GetExameById((Int32)Session["IdExame"]);
                        exame.PACO_CD_ID = (Int32)Session["IdConsulta"];
                        Int32 x = baseApp.ValidateEditExame(exame);
                    }

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 7;
                    hist.PAHI_IN_CHAVE = item.PAEX_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Exame";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Exame incluído: " + item.PAEX_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["ModoConsulta"] == 2)
                    {
                        return RedirectToAction("VerlistaExameConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 4)
                    {
                        return RedirectToAction("MontarTelaExames");
                    }
                    return RedirectToAction("MontarTelaSolicitacoes");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Paciente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "epront", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult UploadFileQueueExamePaciente(FileQueue file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdExame"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Recupera exame
                PACIENTE_EXAMES item = baseApp.GetExameById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACIENTE.PACI__CD_ID.ToString() + "/Exames/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PACIENTE_EXAME_ANEXO foto = new PACIENTE_EXAME_ANEXO();
                foto.PAEO_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAEO_DT_ANEXO = DateTime.Today;
                foto.PAEO_IN_ATIVO = 1;
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
                foto.PAEO_IN_TIPO = tipo;
                foto.PAEO_NM_TITULO = fileName;
                foto.PAEX_CD_ID = item.PAEX_CD_ID;
                foto.PACI_CD_ID = item.PACI_CD_ID;
                item.PACIENTE_EXAME_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditExame(item);
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exames - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoExameAlterada"] = 0;
                Session["NivelPaciente"] = 7;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";

                PACIENTE_EXAMES item = baseApp.GetExameById(id);
                Session["IdPaciente"] = item.PACI_CD_ID;
                Session["IdExame"] = item.PAEX_CD_ID;
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteExameViewModel vm = Mapper.Map<PACIENTE_EXAMES, PacienteExameViewModel>(item);
                Session["Exame"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarExame(PacienteExameViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario= (USUARIO)Session["UserCredentials"];

            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.TipoExame = new SelectList(cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]), "TIEX_CD_ID", "TIEX_NM_NOME");
            Session["TipoExameAlterada"] = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAEX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEX_NM_NOME);
                    vm.PAEX_DS_COMENTARIOS = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEX_DS_COMENTARIOS);
                    vm.PAEX_DS_DIAGNOSTICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEX_DS_DIAGNOSTICO);

                    // Executa a operação
                    PACIENTE_EXAMES item = Mapper.Map<PacienteExameViewModel, PACIENTE_EXAMES>(vm);
                    Int32 volta = baseApp.ValidateEditExame(item);

                    // Verifica retorno
                    Session["IdExame"] = item.PAEX_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 7;
                    Session["NivelExame"] = 1;
                    Session["ExamesAlterada"] = 1;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["ListaExames"] = null;
                    Session["ListaExame"] = null;

                    // Monta Log
                    String frase = item.PAEX_CD_ID.ToString() + "|" + item.PAEX_DT_DATA.ToString() + item.PAEX_NM_NOME + "|" + item.TIEX_CD_ID.ToString() + "|" + item.PAEX_DS_COMENTARIOS;
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eexPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = frase,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 7;
                    hist.PAHI_IN_CHAVE = item.PAEX_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Exame";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Exame editado: " + item.PAEX_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 4)
                    {
                        return RedirectToAction("MontarTelaExames");
                    }
                    return RedirectToAction("MontarTelaExames");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXAME_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exame - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_EXAMES item = baseApp.GetExameById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PAEX_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditExame(item);

                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 7;
                Session["ListaExames"] = null;
                Session["ListaExame"] = null;
                Session["ExamesAlterada"] = 1;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xexPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Data: " + item.PAEX_DT_DATA + " | Exame: " + item.PAEX_NM_NOME,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 7;
                hist.PAHI_IN_CHAVE = item.PAEX_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Exame";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Exame excluído: " + item.PAEX_NM_NOME;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                if ((Int32)Session["ModoConsulta"] == 1)
                {
                    return RedirectToAction("VoltarProcederConsulta");
                }
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                return RedirectToAction("MontarTelaExames");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exame - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 7;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";

                PACIENTE_EXAMES item = baseApp.GetExameById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteExameViewModel vm = Mapper.Map<PACIENTE_EXAMES, PacienteExameViewModel>(item);
                Session["Exame"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFilePaciente(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPaciente"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera cliente
                PACIENTE item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Pacientes/" + item.PACI__CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PACIENTE_ANEXO foto = new PACIENTE_ANEXO();
                foto.PAAX_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAAX_DT_ANEXO = DateTime.Today;
                foto.PAAX_IN_ATIVO = 1;
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
                foto.PAAX_IN_TIPO = tipo;
                foto.PAAX_NM_TITULO = fileName;
                foto.PACI_CD_ID = item.PACI__CD_ID;

                item.PACIENTE_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                Session["NivelPaciente"] = 2;
                Session["PacienteAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileExamePaciente(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdExame"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                PACIENTE_EXAMES item = baseApp.GetExameById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + item.PACIENTE.PACI__CD_ID.ToString() + "/Exames/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PACIENTE_EXAME_ANEXO foto = new PACIENTE_EXAME_ANEXO();
                foto.PAEO_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAEO_DT_ANEXO = DateTime.Today;
                foto.PAEO_IN_ATIVO = 1;
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
                foto.PAEO_IN_TIPO = tipo;
                foto.PAEO_NM_TITULO = fileName;
                foto.PAEX_CD_ID = item.PAEX_CD_ID;
                foto.PACI_CD_ID = item.PACI_CD_ID;

                item.PACIENTE_EXAME_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEditExame(item);
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 2;
                Session["PacienteAlterada"] = 1;
                return RedirectToAction("VoltarEditarExame");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoExamePaciente(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                PACIENTE_EXAME_ANEXO item = baseApp.GetExameAnexoById(id);
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 2;
                return View(item);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoExamePacienteAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                PACIENTE_EXAME_ANEXO item = baseApp.GetExameAnexoById(id);
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 2;
                return View(item);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexoExame(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_EXAME_ANEXO item = baseApp.GetExameAnexoById(id);
                item.PAEO_IN_ATIVO = 0;
                Int32 volta = baseApp.EditExameAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xaePACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " | Anexo: " + item.PAEO_NM_TITULO + " | Data: " + item.PAEO_DT_ANEXO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 2;
                Session["PacienteAlterada"] = 1;
                return RedirectToAction("VoltarEditarExame");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadPacienteExame(Int32 id)
        {
            try
            {
                PACIENTE_EXAME_ANEXO item = baseApp.GetExameAnexoById(id);
                String arquivo = item.PAEO_AQ_ARQUIVO;
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = arquivo.Substring(pos);
                String contentType = string.Empty;
                if (arquivo.Contains(".pdf"))
                {
                    contentType = "application/pdf";
                }
                else if (arquivo.Contains(".jpg") || arquivo.Contains(".jpeg"))
                {
                    contentType = "image/jpg";
                }
                else if (arquivo.Contains(".png"))
                {
                    contentType = "image/png";
                }
                else if (arquivo.Contains(".docx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (arquivo.Contains(".xlsx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else if (arquivo.Contains(".pptx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                }
                else if (arquivo.Contains(".mp3"))
                {
                    contentType = "audio/mpeg";
                }
                else if (arquivo.Contains(".mpeg"))
                {
                    contentType = "audio/mpeg";
                }
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 2;
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

        public ActionResult IncluirAnotacaoPacienteExame()
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Exames - Alteração";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 3;

                PACIENTE_EXAMES item = baseApp.GetExameById((Int32)Session["IdExame"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_EXAME_ANOTACAO coment = new PACIENTE_EXAME_ANOTACAO();
                PacienteExameAnotacaoViewModel vm = Mapper.Map<PACIENTE_EXAME_ANOTACAO, PacienteExameAnotacaoViewModel>(coment);
                vm.PAET_DT_ANOTACAO = DateTime.Now;
                vm.PAET_IN_ATIVO = 1;
                vm.PAEX_CD_ID = item.PAEX_CD_ID;
                vm.PACI_CD_ID = item.PACI_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirAnotacaoPacienteExame(PacienteExameAnotacaoViewModel vm)
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
                    vm.PAET_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAET_TX_ANOTACAO);

                    // Executa a operação
                    PACIENTE_EXAME_ANOTACAO item = Mapper.Map<PacienteExameAnotacaoViewModel, PACIENTE_EXAME_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PACIENTE_EXAMES not = baseApp.GetExameById((Int32)Session["IdExame"]);

                    item.USUARIO = null;
                    not.PACIENTE_EXAME_ANOTACAO.Add(item);
                    Int32 volta = baseApp.ValidateEditExame(not);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "iaePACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " | Data: " + item.PAET_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAET_TX_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 7;
                    Session["NivelExame"] = 3;
                    Session["VoltarPesquisa"] = 0;
                    return RedirectToAction("EditarExame", new { id = (Int32)Session["IdExame"] });
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoPacienteExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Exames - Alteração";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 3;
                PACIENTE_EXAME_ANOTACAO item = baseApp.GetExameAnotacaoById(id);
                PacienteExameAnotacaoViewModel vm = Mapper.Map<PACIENTE_EXAME_ANOTACAO, PacienteExameAnotacaoViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoPacienteExame(PacienteExameAnotacaoViewModel vm)
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
                    vm.PAET_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAET_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PACIENTE_EXAME_ANOTACAO item = Mapper.Map<PacienteExameAnotacaoViewModel, PACIENTE_EXAME_ANOTACAO>(vm);
                    PACIENTE paciente = baseApp.GetItemById(item.PACI_CD_ID);
                    Int32 volta = baseApp.EditExameAnotacao(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eaePACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Paciente: " + paciente.PACI_NM_NOME + " | Data: " + item.PAET_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAET_TX_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 7;
                    Session["NivelExame"] = 3;
                    return RedirectToAction("VoltarEditarExame");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoPacienteExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pacientes - Exames - Alteração";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_EXAME_ANOTACAO item = baseApp.GetExameAnotacaoById(id);
                item.PAET_IN_ATIVO = 0;
                Int32 volta = baseApp.EditExameAnotacao(item);
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 7;
                Session["NivelExame"] = 3;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xaePACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + item.PACIENTE.PACI_NM_NOME + " | Data: " + item.PAET_DT_ANOTACAO.ToString() + " | Anotação: " + item.PAET_TX_ANOTACAO,
                    LOG_IN_SISTEMA = 2
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarEditarExame");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public String GerarPrescricaoHTML()
        {
            // Instancia cache
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Carega configuracao
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Carrega paciente e prescricao
            PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
            PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById((Int32)Session["IdPrescricao"]);
            List<PACIENTE_PRESCRICAO_ITEM> itens = prescricao.PACIENTE_PRESCRICAO_ITEM.ToList();

            // Carrega Usuario, empresa e configuracao
            EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

            // Monta estrutura base inicio
            String baseInicio = String.Empty;
            baseInicio = "<!DOCTYPE html>";
            baseInicio += "<html>";
            baseInicio += "<head>";
            baseInicio += "<meta charset=\"UTF-8\"></meta>";
            baseInicio += "<title>Prescrição</title>";
            baseInicio += "<style>";
            baseInicio += "html, body { height: 297mm; width: 210mm; margin-left: auto; margin-right: auto; }";
            baseInicio += "header, .header-space, footer, .footer-space {height: 100px; font-weight: bold; width: 100%; padding: 10pt; margin: 10pt 0; }";
            baseInicio += "header { position: fixed; top: 0; }";
            baseInicio += "footer { position: fixed; bottom: 0; }";
            baseInicio += "</style>";
            baseInicio += "</head>";

            // Monta estrutura base final
            String baseFinal = String.Empty;
            baseFinal += "</html>";

            // Monta cabecalho
            String classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            String header = String.Empty;
            header += "<header id=\"pageHeader\">";
            header += "<div style=\"width: 100%; line-height: 0.3\">";
            header += "<h1 style=\"text-align:center; font-family:Calibri\"><i>" + usuario.USUA_NM_APELIDO + "</i></h1>";
            header += "<h3 style=\"text-align:center; font-family:Calibri\">" + usuario.USUA_NM_ESPECIALIDADE + "</h3>";
            header += "<h4 style=\"text-align:center; font-family:Calibri\">" + classe + "</h4>";
            header += "</div>";
            header += "<hr />";
            header += "</header>";

            // Monta rodape
            String fileNameQR = "Prescricao_QRCode_" + prescricao.PAPR_GU_GUID + ".png";
            String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/QRCode/";
            String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);
            pathQR = "\"" + pathQR + "\"";

            String footer = String.Empty;
            footer += "<footer id=\"pageFooter\">";
            footer += "<hr />";
            footer += "<div style=\"width: 100%; line-height: 0.3\">";
            footer += "<div style=\"width: 12%; float: left; border: 0.5px solid black; line-height: 0.3; height: 90px\">";
            footer += "<img src=" + pathQR + " alt=\"QRCode\" style=\"width: 82px; height: 82px; max-height: 85px; max-width: 85px; margin-left: 4px; margin-right: 4px; margin-top: 4px; margin-bottom: 4px\"></img>";
            footer += "</div>";
            footer += "<div style=\"width: 84%; float: left; border: 0.5px solid black; height: 90px\">";
            footer += "<div style=\"line-height: 0.05px\">";
            footer += "<p style=\"text-align:left; font-family:Calibri; margin-left: 5px\"><b>" + usuario.USUA_NM_APELIDO + "</b></p>";
            footer += "<p style=\"text-align:left; font-family:Calibri; font-weight:normal; margin-left: 5px\">CPF: " + usuario.USUA_NR_CPF + "</p>";

            String endereco = String.Empty;
            String enderecoCont = String.Empty;
            if (empresa.EMPR_NM_ENDERECO != null)
            {
                endereco += empresa.EMPR_NM_ENDERECO;
                if (empresa.EMPR_NM_NUMERO != null)
                {
                    endereco += " " + empresa.EMPR_NM_NUMERO;
                }
                if (empresa.EMPR_NM_COMPLEMENTO != null)
                {
                    endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                }
                if (empresa.EMPR_NM_BAIRRO != null)
                {
                    enderecoCont += empresa.EMPR_NM_BAIRRO;
                }
                if (empresa.EMPR_NM_CIDADE != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                }
                if (empresa.UF != null)
                {
                    enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                }
                if (empresa.EMPR_NR_CEP != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NR_CEP;
                }
            }

            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + endereco + "</p>";
            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + enderecoCont + "</p>";
            footer += "</div>";
            footer += "</div>";
            footer += "</div>";
            footer += "</footer>";

            // Monta corpo
            String body = String.Empty;
            body += "<body>";
            body += header;
            body += "<table>";
            body += "<thead><tr><td>";
            body += "<div class=\"header-space\">&nbsp;</div>";
            body += "</td></tr>";
            body += "</thead>";
            body += "<tbody><tr><td>";
            body += "<div id=\"pageHost\" class=\"content\">";
            body += "<div style=\"width: 100%; line-height: 0.5\">";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">" + paciente.PACI_NM_NOME + "</h3>";
            body += "<br />";

            foreach (PACIENTE_PRESCRICAO_ITEM item in itens)
            {
                body += "<p style=\"width: 100%; text-align:left; font-family:Calibri;\"><b><u>" + item.TIPO_FORMA.TIFO_NM_NOME + "</u></b></p>";
                body += "<br /><br />";
                body += "<p style=\"width: 100%; text-align:left; font-family:Calibri;\"><b>" + item.PAPI_NM_REMEDIO + "</b></p>";
                body += "<br /><br />";
                body += "<p style=\"width: 100%; text-align:left; font-family:Calibri;\">" + item.PAPI_DS_POSOLOGIA + "</p>";
                body += "<br /><br /><br /><br />";
            }
            body += "<br /><br /><br /><br />";
            body += "<br /><br /><br /><br />";
            String local = empresa.EMPR_NM_CIDADE + ", " + DateTime.Today.Date.Day.ToString() + " de " + CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Month) + " de " + DateTime.Today.Date.Year.ToString();
            body += "<p style=\"text-align:center; font-family:Calibri\">" + local + "</p>";
            body += "</div>";
            body += "</div>";
            body += "</td></tr></tbody>";
            body += "<tfoot><tr><td>";
            body += "<div class=\"footer-space\">&nbsp;</div>";
            body += "</td></tr></tfoot>";
            body += "</table>";
            body += footer;
            body += "</body>";

            // Monta documento
            String documento = String.Empty;
            documento = baseInicio + body + baseFinal;
            return documento;
        }

        public String GerarAtestadoHTML()
        {
            // Instancia cache
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Carega configuracao
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Carrega paciente e prescricao
            PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
            PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById((Int32)Session["IdAtestado"]);

            // Carrega Usuario, empresa e configuracao
            EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

            // Monta estrutura base inicio
            String baseInicio = String.Empty;
            baseInicio = "<!DOCTYPE html>";
            baseInicio += "<html>";
            baseInicio += "<head>";
            baseInicio += "<meta charset=\"UTF-8\"></meta>";
            baseInicio += "<title>Atestado</title>";
            baseInicio += "<style>";
            baseInicio += "html, body { height: 297mm; width: 210mm; margin-left: auto; margin-right: auto; }";
            baseInicio += "header, .header-space, footer, .footer-space {height: 100px; font-weight: bold; width: 100%; padding: 10pt; margin: 10pt 0; }";
            baseInicio += "header { position: fixed; top: 0; }";
            baseInicio += "footer { position: fixed; bottom: 0; }";
            baseInicio += "</style>";
            baseInicio += "</head>";

            // Monta estrutura base final
            String baseFinal = String.Empty;
            baseFinal += "</html>";

            // Monta cabecalho
            String classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            String header = String.Empty;
            header += "<header id=\"pageHeader\">";
            header += "<div style=\"width: 100%; line-height: 0.3\">";
            header += "<h1 style=\"text-align:center; font-family:Calibri\"><i>" + usuario.USUA_NM_APELIDO + "</i></h1>";
            header += "<h3 style=\"text-align:center; font-family:Calibri\">" + usuario.USUA_NM_ESPECIALIDADE + "</h3>";
            header += "<h4 style=\"text-align:center; font-family:Calibri\">" + classe + "</h4>";
            header += "</div>";
            header += "<hr />";
            header += "</header>";

            // Monta rodape
            String fileNameQR = "Atestado_QRCode_" + atestado.PAAT_GU_GUID + ".png";
            String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/QRCode/";
            String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);
            pathQR = "\"" + pathQR + "\"";

            String footer = String.Empty;
            footer += "<footer id=\"pageFooter\">";
            footer += "<hr />";
            footer += "<div style=\"width: 100%; line-height: 0.3\">";
            footer += "<div style=\"width: 12%; float: left; border: 0.5px solid black; line-height: 0.3; height: 90px\">";
            footer += "<img src=" + pathQR + " alt=\"QRCode\" style=\"width: 82px; height: 82px; max-height: 85px; max-width: 85px; margin-left: 4px; margin-right: 4px; margin-top: 4px; margin-bottom: 4px\"></img>";
            footer += "</div>";
            footer += "<div style=\"width: 84%; float: left; border: 0.5px solid black; height: 90px\">";
            footer += "<div style=\"line-height: 0.05px\">";
            footer += "<p style=\"text-align:left; font-family:Calibri; margin-left: 5px\"><b>" + usuario.USUA_NM_APELIDO + "</b></p>";
            footer += "<p style=\"text-align:left; font-family:Calibri; font-weight:normal; margin-left: 5px\">CPF: " + usuario.USUA_NR_CPF + "</p>";

            String endereco = String.Empty;
            String enderecoCont = String.Empty;
            if (empresa.EMPR_NM_ENDERECO != null)
            {
                endereco += empresa.EMPR_NM_ENDERECO;
                if (empresa.EMPR_NM_NUMERO != null)
                {
                    endereco += " " + empresa.EMPR_NM_NUMERO;
                }
                if (empresa.EMPR_NM_COMPLEMENTO != null)
                {
                    endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                }
                if (empresa.EMPR_NM_BAIRRO != null)
                {
                    enderecoCont += empresa.EMPR_NM_BAIRRO;
                }
                if (empresa.EMPR_NM_CIDADE != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                }
                if (empresa.UF != null)
                {
                    enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                }
                if (empresa.EMPR_NR_CEP != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NR_CEP;
                }
            }

            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + endereco + "</p>";
            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + enderecoCont + "</p>";
            footer += "</div>";
            footer += "</div>";
            footer += "</div>";
            footer += "</footer>";

            // Monta corpo
            String body = String.Empty;
            body += "<body>";
            body += header;
            body += "<table>";
            body += "<thead><tr><td>";
            body += "<div class=\"header-space\">&nbsp;</div>";
            body += "</td></tr>";
            body += "</thead>";
            body += "<tbody><tr><td>";
            body += "<div id=\"pageHost\" class=\"content\">";
            body += "<div style=\"width: 100%; line-height: 0.5\">";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">Destino: " + atestado.PAAT_NM_DESTINO + "</h3>";
            body += "<br />";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">Finalidade: " + atestado.PAAT_NM_TITULO + "</h3>";
            body += "<br />";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">Referente a: " + paciente.PACI_NM_NOME + "</h3>";
            body += "<br />";
            body += "<br />";
            body += "<p style=\"width: 100%; text-align:left; font-family:Calibri;\"><b>" + atestado.PAAT_TX_TEXTO + "</b></p>";
            body += "<br /><br /><br /><br />";
            body += "<br /><br /><br /><br />";
            body += "<br /><br /><br /><br />";
            String local = String.Empty;
            if (atestado.PAAT_IN_DATA == 1)
            {
                local = empresa.EMPR_NM_CIDADE + ", " + DateTime.Today.Date.Day.ToString() + " de " + CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Month) + " de " + DateTime.Today.Date.Year.ToString();
            }
            else
            {
                local = empresa.EMPR_NM_CIDADE + ",        " + " de                         " + " de ";
            }
            body += "<p style=\"text-align:center; font-family:Calibri\">" + local + "</p>";
            body += "</div>";
            body += "</div>";
            body += "</td></tr></tbody>";
            body += "<tfoot><tr><td>";
            body += "<div class=\"footer-space\">&nbsp;</div>";
            body += "</td></tr></tfoot>";
            body += "</table>";
            body += footer;
            body += "</body>";

            // Monta documento
            String documento = String.Empty;
            documento = baseInicio + body + baseFinal;
            return documento;
        }

        public String GerarSolicitacaoHTML()
        {
            // Instancia cache
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
            CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
            CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

            // Carega configuracao
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Carrega paciente e prescricao
            PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
            PACIENTE_SOLICITACAO solicitacao = baseApp.GetSolicitacaoById((Int32)Session["IdSolicitacao"]);

            // Carrega Usuario, empresa e configuracao
            EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);

            // Monta estrutura base inicio
            String baseInicio = String.Empty;
            baseInicio = "<!DOCTYPE html>";
            baseInicio += "<html>";
            baseInicio += "<head>";
            baseInicio += "<meta charset=\"UTF-8\"></meta>";
            baseInicio += "<title>Atestado</title>";
            baseInicio += "<style>";
            baseInicio += "html, body { height: 297mm; width: 210mm; margin-left: auto; margin-right: auto; }";
            baseInicio += "header, .header-space, footer, .footer-space {height: 100px; font-weight: bold; width: 100%; padding: 10pt; margin: 10pt 0; }";
            baseInicio += "header { position: fixed; top: 0; }";
            baseInicio += "footer { position: fixed; bottom: 0; }";
            baseInicio += "</style>";
            baseInicio += "</head>";

            // Monta estrutura base final
            String baseFinal = String.Empty;
            baseFinal += "</html>";

            // Monta cabecalho
            String classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            String header = String.Empty;
            header += "<header id=\"pageHeader\">";
            header += "<div style=\"width: 100%; line-height: 0.3\">";
            header += "<h1 style=\"text-align:center; font-family:Calibri\"><i>" + usuario.USUA_NM_APELIDO + "</i></h1>";
            header += "<h3 style=\"text-align:center; font-family:Calibri\">" + usuario.USUA_NM_ESPECIALIDADE + "</h3>";
            header += "<h4 style=\"text-align:center; font-family:Calibri\">" + classe + "</h4>";
            header += "</div>";
            header += "<hr />";
            header += "</header>";

            // Monta rodape
            String fileNameQR = "Solicitacao_QRCode_" + solicitacao.PASO_GU_GUID + ".png";
            String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/QRCode/";
            String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);
            pathQR = "\"" + pathQR + "\"";

            String footer = String.Empty;
            footer += "<footer id=\"pageFooter\">";
            footer += "<hr />";
            footer += "<div style=\"width: 100%; line-height: 0.3\">";
            footer += "<div style=\"width: 12%; float: left; border: 0.5px solid black; line-height: 0.3; height: 90px\">";
            footer += "<img src=" + pathQR + " alt=\"QRCode\" style=\"width: 82px; height: 82px; max-height: 85px; max-width: 85px; margin-left: 4px; margin-right: 4px; margin-top: 4px; margin-bottom: 4px\"></img>";
            footer += "</div>";
            footer += "<div style=\"width: 84%; float: left; border: 0.5px solid black; height: 90px\">";
            footer += "<div style=\"line-height: 0.05px\">";
            footer += "<p style=\"text-align:left; font-family:Calibri; margin-left: 5px\"><b>" + usuario.USUA_NM_APELIDO + "</b></p>";
            footer += "<p style=\"text-align:left; font-family:Calibri; font-weight:normal; margin-left: 5px\">CPF: " + usuario.USUA_NR_CPF + "</p>";

            String endereco = String.Empty;
            String enderecoCont = String.Empty;
            if (empresa.EMPR_NM_ENDERECO != null)
            {
                endereco += empresa.EMPR_NM_ENDERECO;
                if (empresa.EMPR_NM_NUMERO != null)
                {
                    endereco += " " + empresa.EMPR_NM_NUMERO;
                }
                if (empresa.EMPR_NM_COMPLEMENTO != null)
                {
                    endereco += " " + empresa.EMPR_NM_COMPLEMENTO;
                }
                if (empresa.EMPR_NM_BAIRRO != null)
                {
                    enderecoCont += empresa.EMPR_NM_BAIRRO;
                }
                if (empresa.EMPR_NM_CIDADE != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NM_CIDADE;
                }
                if (empresa.UF != null)
                {
                    enderecoCont += " - " + empresa.UF.UF_SG_SIGLA;
                }
                if (empresa.EMPR_NR_CEP != null)
                {
                    enderecoCont += " - " + empresa.EMPR_NR_CEP;
                }
            }

            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + endereco + "</p>";
            footer += "<p style=\"text-align:left; font-weight:normal; font-family:Calibri; margin-left: 5px\">" + enderecoCont + "</p>";
            footer += "</div>";
            footer += "</div>";
            footer += "</div>";
            footer += "</footer>";

            // Monta corpo
            String body = String.Empty;
            body += "<body>";
            body += header;
            body += "<table>";
            body += "<thead><tr><td>";
            body += "<div class=\"header-space\">&nbsp;</div>";
            body += "</td></tr>";
            body += "</thead>";
            body += "<tbody><tr><td>";
            body += "<div id=\"pageHost\" class=\"content\">";
            body += "<div style=\"width: 100%; line-height: 0.5\">";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">" + solicitacao.PASO_NM_TITULO + "</h3>";
            body += "<br />";
            body += "<h3 style=\"margin-left: 10px; font-family:Calibri\">Paciente: " + paciente.PACI_NM_NOME + "</h3>";
            body += "<br />";
            body += "<br />";
            body += "<p style=\"width: 100%; text-align:left; font-family:Calibri;\"><b>" + solicitacao.PASO_TX_TEXTO + "</b></p>";
            body += "<br /><br /><br /><br />";
            body += "<br /><br /><br /><br />";
            body += "<br /><br /><br /><br />";
            String local = String.Empty;
            if (solicitacao.PASO_IN_DATA == 1)
            {
                local = empresa.EMPR_NM_CIDADE + ", " + DateTime.Today.Date.Day.ToString() + " de " + CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Month) + " de " + DateTime.Today.Date.Year.ToString();
            }
            else
            {
                local = empresa.EMPR_NM_CIDADE + ",        " + " de                         " + " de ";
            }
            body += "<p style=\"text-align:center; font-family:Calibri\">" + local + "</p>";
            body += "</div>";
            body += "</div>";
            body += "</td></tr></tbody>";
            body += "<tfoot><tr><td>";
            body += "<div class=\"footer-space\">&nbsp;</div>";
            body += "</td></tr></tfoot>";
            body += "</table>";
            body += footer;
            body += "</body>";

            // Monta documento
            String documento = String.Empty;
            documento = baseInicio + body + baseFinal;
            return documento;
        }

        [HttpGet]
        public ActionResult EnviarPrescricaoEMail(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ENVIAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Prescrição - Envio";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera paciente e prescricao
                PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(id);
                PACIENTE paciente = baseApp.GetItemById(prescricao.PACI_CD_ID);
                Session["Paciente"] = paciente;
                ViewBag.Paciente = paciente;
                Session["Prescricao"] = prescricao;
                ViewBag.Prescricao = prescricao;
                Session["NivelPaciente"] = 8;

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = id;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Prescrição para Paciente: " + paciente.PACI_NM_NOME;
                mens.MENS_GU_GUID = prescricao.PAPR_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = prescricao.PAPR_DT_DATA;
                mens.MENS_DT_ENVIO = prescricao.PAPR_DT_ENVIO;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = prescricao.PAPR_NR_ENVIOS;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarPrescricaoEMail(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);

                    // Executa a operação
                    PACIENTE_PRESCRICAO prescricao = (PACIENTE_PRESCRICAO)Session["Prescricao"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailPrescricao(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epeUSUA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Prescrição : " + prescricao.PAPR_GU_GUID,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 8;
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailPrescricao(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE_PRESCRICAO prescricao = (PACIENTE_PRESCRICAO)Session["Prescricao"];
            PACIENTE paciente = baseApp.GetItemById(prescricao.PACI_CD_ID);

            // Carrega Configuração
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + paciente.PACI_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Inlcuir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/Prescricao/";
            String fileNamePDF = "Prescricao_" + paciente.PACI_NM_NOME + "_" + prescricao.PAPR_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString()) + ".pdf";
            String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

            AttachmentModel model = new AttachmentModel();
            model.PATH = path;
            model.ATTACHMENT_NAME = fileNamePDF;
            model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
            models.Add(model);

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME + " - Prescrição";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = paciente.PACI_NM_EMAIL;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Acerta prescricao
            String idEnvio = Xid.NewXid().ToString();
            Session["IdEnvio"] = idEnvio;
            prescricao.PAPR_DT_ENVIO = DateTime.Now;
            prescricao.PAPR_IN_ENVIADO = 1;
            prescricao.PAPR_NR_ENVIOS = prescricao.PAPR_NR_ENVIOS + 1;
            prescricao.PAPR_GU_GUID_ENVIO = idEnvio;
            Int32 voltaP = baseApp.ValidateEditPrescricao(prescricao);

            // Grava envio
            if (status == "Succeeded")
            {
                vm.MENS_NM_NOME = "Paciente - " + paciente.PACI_NM_NOME + " - Prescrição";
                vm.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null; 
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Paciente - Envio Prescrição");
                Session["MensPaciente"] = 999;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensPaciente"] = 998;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        public ActionResult GerarRelatorioLista()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PacienteLista" + "_" + data + ".pdf";
                List<PACIENTE> lista = (List<PACIENTE>)Session["ListaPaciente"];
                PACIENTE filtro = (PACIENTE)Session["FiltroPaciente"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Listagem", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 100f, 70f, 60f, 100f, 60f, 80f, 40f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Pacientes selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 10;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("UF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ult.Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Retorno", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("   ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.TIPO_PACIENTE.TIPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CPF != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CPF, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CELULAR != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CELULAR, meuFont))
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
                    if (item.PACI_NM_CIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NM_CIDADE, meuFont))
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
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACI_DT_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_CONSULTA.Value.ToShortDateString(), meuFont))
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
                    if (item.PACI_DT_PREVISAO_RETORNO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_PREVISAO_RETORNO.Value.ToShortDateString(), meuFont))
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
                    if (System.IO.File.Exists(Server.MapPath(item.PACI_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.PACI_AQ_FOTO));
                        image.ScaleAbsolute(40, 40);
                        cell.AddElement(image);
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.TIPA_CD_ID > 0)
                    {
                        TIPO_PACIENTE cat = tpaApp.GetItemById(filtro.TIPA_CD_ID);
                        parametros += "Categoria: " + cat.TIPA_NM_NOME;
                        ja = 1;
                    }
                    if (filtro.PACI__CD_ID > 0)
                    {
                        PACIENTE cli = baseApp.GetItemById(filtro.PACI__CD_ID);
                        if (ja == 0)
                        {
                            parametros += "Nome: *" + cli.PACI_NM_NOME + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: *" + cli.PACI_NM_NOME + "*";
                        }
                    }
                    if (filtro.PACI_NR_CPF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "CPF: " + filtro.PACI_NR_CPF;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e CPF: " + filtro.PACI_NR_CPF;
                        }
                    }
                    if (filtro.PACI_NM_EMAIL != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                        }
                    }
                    if (filtro.PACI_NM_CIDADE != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                        }
                    }
                    if (filtro.UF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
                        }
                    }
                    if (filtro.PACI_NR_MATRICULA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                        }
                    }
                    if (filtro.PACI_NM_INDICACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                        }
                    }
                    if (filtro.SEXO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                        }
                    }
                    if (ja == 0)
                    {
                        parametros = "Nenhum filtro definido.";
                    }
                }
                else
                {
                    parametros = "Nenhum filtro definido.";
                }
                Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                return RedirectToAction("MontarTelaCentralPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EnviarAtestadoEMail(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ENVIAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Envio";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera paciente e prescricao
                PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById(id);
                PACIENTE paciente = baseApp.GetItemById(atestado.PACI_CD_ID.Value);
                Session["Paciente"] = paciente;
                ViewBag.Paciente = paciente;
                Session["Atestado"] = atestado;
                ViewBag.Atestado = atestado;
                Session["NivelPaciente"] = 9;

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = id;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Atestado para Paciente: " + paciente.PACI_NM_NOME;
                mens.MENS_GU_GUID = atestado.PAAT_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = atestado.PAAT_DT_DATA;
                mens.MENS_DT_ENVIO = atestado.PAAT_DT_ENVIO;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = atestado.PAAT_NR_ENVIOS;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarAtestadoEMail(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);

                    // Executa a operação
                    PACIENTE_ATESTADO atestado = (PACIENTE_ATESTADO)Session["Atestado"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailAtestado(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epaPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Atestado : " + atestado.PAAT_GU_GUID,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 9;
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailAtestado(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE_ATESTADO atestado = (PACIENTE_ATESTADO)Session["Atestado"];
            PACIENTE paciente = baseApp.GetItemById(atestado.PACI_CD_ID.Value);

            // Carrega Configuração
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + paciente.PACI_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Inlcuir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/Atestado/";
            String fileNamePDF = "Atestado_" + paciente.PACI_NM_NOME + "_" + atestado.PAAT_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString()) + ".pdf";
            String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

            AttachmentModel model = new AttachmentModel();
            model.PATH = path;
            model.ATTACHMENT_NAME = fileNamePDF;
            model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
            models.Add(model);

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME + " - Atestado";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = paciente.PACI_NM_EMAIL;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Acerta prescricao
            String idEnvio = Xid.NewXid().ToString();
            Session["IdEnvio"] = idEnvio;
            atestado.PAAT_DT_ENVIO = DateTime.Now;
            atestado.PAAT__IN_ENVIADO = 1;
            atestado.PAAT_NR_ENVIOS = atestado.PAAT_NR_ENVIOS + 1;
            atestado.PAAT_GU_GUID_ENVIO = idEnvio;
            Int32 voltaP = baseApp.ValidateEditAtestado(atestado);

            // Grava envio
            if (status == "Succeeded")
            {
                vm.MENS_NM_NOME = "Paciente - " + paciente.PACI_NM_NOME + " - Prescrição";
                vm.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null; 
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Paciente - Envio Atestado");
                Session["MensPaciente"] = 990;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensPaciente"] = 991;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        public ActionResult GerarListagemPrescricao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PrescricaoLista" + "_" + data + ".pdf";
                List<PACIENTE_PRESCRICAO> lista = (List<PACIENTE_PRESCRICAO>)Session["ListaPrescricao"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Prescrições", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 70f, 80f, 80f, 60f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Paciente: " + paciente.PACI_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 10;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Controle", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num.Envios", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_PRESCRICAO item in lista)
                {
                    if (item.PAPR_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAPR_DT_DATA.ToShortDateString() + " " + item.PAPR_DT_DATA.ToShortTimeString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.TIPO_CONTROLE.TICO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PAPR_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAPR_IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PAPR_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAPR_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_PRESCRICAO_ITEM != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_PRESCRICAO_ITEM.Count().ToString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 8;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EnviarSolicitacaoEMail(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_ENVIAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Envio";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera paciente e soliictacao
                PACIENTE_SOLICITACAO solicitacao = baseApp.GetSolicitacaoById(id);
                PACIENTE paciente = baseApp.GetItemById(solicitacao.PACI_CD_ID.Value);
                Session["Paciente"] = paciente;
                ViewBag.Paciente = paciente;
                Session["Solicitacao"] = solicitacao;
                ViewBag.Solicitacao = solicitacao;
                Session["NivelPaciente"] = 12;

                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = id;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Solicitação de Exame para Paciente: " + paciente.PACI_NM_NOME;
                mens.MENS_GU_GUID = solicitacao.PASO_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = solicitacao.PASO_DT_EMISSAO;
                mens.MENS_DT_ENVIO = solicitacao.PASO_DT_ENVIO;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = solicitacao.PASO_NR_ENVIOS;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                return View(mens);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");

            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarSolicitacaoEMail(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);

                    // Executa a operação
                    PACIENTE_SOLICITACAO solicitacao = (PACIENTE_SOLICITACAO)Session["Solicitacao"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailSolicitacao(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epsPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Solicitação : " + solicitacao.PASO_GU_GUID,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 12;
                    // Retorno
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    return RedirectToAction("MontarTelaSolicitacoes");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailSolicitacao(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE_SOLICITACAO solicitacao = (PACIENTE_SOLICITACAO)Session["Solicitacao"];
            PACIENTE paciente = baseApp.GetItemById(solicitacao.PACI_CD_ID.Value);

            // Carrega Configuração
            CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
            CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
            Session["ConfiguracaoAlterada"] = 0;

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + paciente.PACI_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + vm.MENS_NM_ASSINATURA + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Inlcuir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Pacientes/" + paciente.PACI__CD_ID.ToString() + "/Solicitacao/";
            String fileNamePDF = "Solicitacao_" + paciente.PACI_NM_NOME + "_" + solicitacao.PASO_GU_GUID + "_" + CrossCutting.StringLibrary.RemoveSlashDateString(DateTime.Today.Date.ToShortDateString()) + ".pdf";
            String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

            AttachmentModel model = new AttachmentModel();
            model.PATH = path;
            model.ATTACHMENT_NAME = fileNamePDF;
            model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
            models.Add(model);

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME + " - Solicitação de Exame";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = paciente.PACI_NM_EMAIL;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Acerta prescricao
            String idEnvio = Xid.NewXid().ToString();
            Session["IdEnvio"] = idEnvio;
            solicitacao.PASO_DT_ENVIO = DateTime.Now;
            solicitacao.PASO_IN_ENVIADO = 1;
            solicitacao.PASO_NR_ENVIOS = solicitacao.PASO_NR_ENVIOS + 1;
            solicitacao.PASO_GU_GUID_ENVIO = idEnvio;
            Int32 voltaP = baseApp.ValidateEditSolicitacao(solicitacao);

            // Grava envio
            if (status == "Succeeded")
            {
                vm.MENS_NM_NOME = "Paciente - " + paciente.PACI_NM_NOME + " - Solicitação de Exame";
                vm.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null; 
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Paciente - Envio Solicitação");
                Session["MensPaciente"] = 992;
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensPaciente"] = 993;
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        public ActionResult GerarListagemAtestado()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "AtestadoLista" + "_" + data + ".pdf";
                List<PACIENTE_ATESTADO> lista = (List<PACIENTE_ATESTADO>)Session["ListaAtestado"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Atestados", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 70f, 60f, 120f, 150f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Paciente: " + paciente.PACI_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 7;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Atestado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Finalidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_ATESTADO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PAAT_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAAT_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAAT_DT_DATA.Value.ToShortDateString(), meuFont))
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
                    if (item.TIPO_ATESTADO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_ATESTADO.TIAT_NM_NOME, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PAAT_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAAT__IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PAAT_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAAT_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemConsulta()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ConsultaLista" + "_" + data + ".pdf";
                List<PACIENTE_CONSULTA> lista = (List<PACIENTE_CONSULTA>)Session["ListaConsulta"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Consultas", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 60f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Paciente: " + paciente.PACI_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 5;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Hora de Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Hora de Término", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_CONSULTA item in lista)
                {
                    if (item.PACO_DT_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACO_HR_INICIO.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PACO_HR_FINAL.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACO_IN_TIPO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Presencial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Remota", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarAnamnese(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Anamnese - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 4;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_7.pdf";

                PACIENTE_ANAMNESE item = baseApp.GetAnamneseById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteAnamneseViewModel vm = Mapper.Map<PACIENTE_ANAMNESE, PacienteAnamneseViewModel>(item);
                Session["Anamnese"] = item;
                vm.PACO_DT_CONSULTA = vm.PACIENTE_CONSULTA.PACO_DT_CONSULTA;
                vm.PACO_IN_ENCERRADA = vm.PACIENTE_CONSULTA.PACO_IN_ENCERRADA.Value;
                vm.PACO_IN_TIPO = vm.PACIENTE_CONSULTA.PACO_IN_TIPO.Value;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnamnese(PacienteAnamneseViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAAM_DS_MOTIVO_CONSULTA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_MOTIVO_CONSULTA);
                    vm.PAAM_DS_HISTORIA_FAMILIAR = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_HISTORIA_FAMILIAR);
                    vm.PAAM_DS_HISTORIA_SOCIAL = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_HISTORIA_SOCIAL);
                    vm.PAAN_NM_AVALIACAO_CARDIOLOGICA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_NM_AVALIACAO_CARDIOLOGICA);
                    vm.PAAN_NM_RESPIRATORIO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_NM_RESPIRATORIO);
                    vm.PAAN_NM_ABDOMEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_NM_ABDOMEM);
                    vm.PAAN_NM_MEMBROS_INFERIORES = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAN_NM_MEMBROS_INFERIORES);
                    vm.PAAM_DS_QUEIXA_PRINCIPAL = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_QUEIXA_PRINCIPAL);
                    vm.PAAM_DS_HISTORIA_DOENCA_ATUAL = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_HISTORIA_DOENCA_ATUAL);
                    vm.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA);
                    vm.PAAM_DS_DIAGNOSTICO_1 = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_DIAGNOSTICO_1);
                    vm.PAAM_DS_CONDUTA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_DS_CONDUTA);
                    vm.PAAM_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAAM_TX_OBSERVACOES);

                    // Executa a operação
                    PACIENTE_ANAMNESE item = Mapper.Map<PacienteAnamneseViewModel, PACIENTE_ANAMNESE>(vm);
                    Int32 volta = baseApp.ValidateEditAnamnese(item);

                    // Verifica retorno
                    Session["IdAnamnese"] = item.PAAM_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 4;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eanPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_ANAMNESE>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 8;
                    hist.PAHI_IN_CHAVE = item.PAAM_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Anamnese";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Anamnese editada: " + item.PAAM_DT_DATA.ToShortDateString();
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["VoltaAnamnese"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    else if ((Int32)Session["VoltaAnamnese"] == 2)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerAnamnese(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Anamnese - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 4;

                PACIENTE_ANAMNESE item = baseApp.GetAnamneseById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteAnamneseViewModel vm = Mapper.Map<PACIENTE_ANAMNESE, PacienteAnamneseViewModel>(item);
                Session["Anamnese"] = item;
                vm.PACO_DT_CONSULTA = vm.PACIENTE_CONSULTA.PACO_DT_CONSULTA;
                vm.PACO_IN_ENCERRADA = vm.PACIENTE_CONSULTA.PACO_IN_ENCERRADA.Value;
                vm.PACO_IN_TIPO = vm.PACIENTE_CONSULTA.PACO_IN_TIPO.Value;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemSolicitacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "SolicitacaoLista" + "_" + data + ".pdf";
                List<PACIENTE_SOLICITACAO> lista = (List<PACIENTE_SOLICITACAO>)Session["ListaSolicitacao"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Solicitações de Exames", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 70f, 60f, 150f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Paciente: " + paciente.PACI_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 6;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_SOLICITACAO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PASO_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PASO_DT_EMISSAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PASO_DT_EMISSAO.Value.ToShortDateString(), meuFont))
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
                    if (item.TIPO_EXAME != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_EXAME.TIEX_NM_NOME, meuFont))
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
                    if (item.PASO_IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PASO_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PASO_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemExame()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["Usuario"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ExameLista" + "_" + data + ".pdf";
                List<PACIENTE_EXAMES> lista = (List<PACIENTE_EXAMES>)Session["ListaExames"];
                PACIENTE paciente = (PACIENTE)Session["Paciente"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes - Resultados de Exames", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 120f, 60f, 120f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Paciente: " + paciente.PACI_NM_NOME, meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 4;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome do Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Anexos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_EXAMES item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PAEX_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAEX_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAEX_DT_DATA.Value.ToShortDateString(), meuFont))
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
                    if (item.TIPO_EXAME != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_EXAME.TIEX_NM_NOME, meuFont))
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
                    if (item.PACIENTE_EXAME_ANEXO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_EXAME_ANEXO.Count().ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("VoltarAnexoPaciente");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarExameFisico(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exame Físico - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 6;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_8.pdf";

                PACIENTE_EXAME_FISICOS item = baseApp.GetExameFisicoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteExameFisicoViewModel vm = Mapper.Map<PACIENTE_EXAME_FISICOS, PacienteExameFisicoViewModel>(item);
                Session["ExameFisico"] = item;
                vm.PACO_DT_CONSULTA = vm.PACIENTE_CONSULTA.PACO_DT_CONSULTA;
                vm.PACO_IN_ENCERRADA = vm.PACIENTE_CONSULTA.PACO_IN_ENCERRADA.Value;
                vm.PACO_IN_TIPO = vm.PACIENTE_CONSULTA.PACO_IN_TIPO.Value;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarExameFisico(PacienteExameFisicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PAEF_DS_TABAGISMO     = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEF_DS_TABAGISMO);
                    vm.PAEF_DS_EXERCICIO_FISICO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEF_DS_EXERCICIO_FISICO);
                    vm.PAEF_DS_ALCOOLISMO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEF_DS_ALCOOLISMO);
                    vm.PAEF_DS_ANTICONCEPCIONAL = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEF_DS_ANTICONCEPCIONAL);
                    vm.PAEF_DS_MARCAPASSO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PAEF_DS_MARCAPASSO);
                    vm.PAEF_TX_CIRURGIAS = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEF_TX_CIRURGIAS);
                    vm.PAEF_DS_ALERGICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEF_DS_ALERGICO);
                    vm.PAEF_DS_ONCOLOGICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEF_DS_ONCOLOGICO);
                    vm.PAEF_DS_EXAME_FISICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PAEF_DS_EXAME_FISICO);

                    // Verficar marcações das checkboxes
                    if (vm.PAEF_DS_TABAGISMO != null)
                    {
                        vm.PAEF_IN_TABAGISMO = 1;
                    }
                    if (vm.PAEF_DS_EXERCICIO_FISICO != null)
                    {
                        vm.PAEF_IN_EXERCICIO_FISICO = 1;
                    }
                    if (vm.PAEF_DS_ALCOOLISMO != null)
                    {
                        vm.PAEF_IN_ALCOOLISMO = 1;
                    }
                    if (vm.PAEF_DS_ANTICONCEPCIONAL != null)
                    {
                        vm.PAEF_IN_ANTICONCEPCIONAL = 1;
                    }
                    if (vm.PAEF_DS_MARCAPASSO != null)
                    {
                        vm.PAEF_IN_MARCAPASSO = 1;
                    }
                    if (vm.PAEF_TX_CIRURGIAS != null)
                    {
                        vm.PAEF_IN_CIRURGIAS = 1;
                    }
                    if (vm.PAEF_DS_ALERGICO != null)
                    {
                        vm.PAEF_IN_ANTE_ALERGICO = 1;
                    }
                    if (vm.PAEF_DS_ONCOLOGICO != null)
                    {
                        vm.PAEF_IN_ANTE_ONCOLOGICO = 1;
                    }

                    // Recalcula IMC
                    Double? imc = 0;
                    if (vm.PAEF_NR_ALTURA > 0 & vm.PAEF_NR_PESO > 0)
                    {
                        imc = Convert.ToDouble(vm.PAEF_NR_PESO) / Math.Pow((Convert.ToDouble(vm.PAEF_NR_ALTURA / 100)), 2);
                    }
                    vm.PAEF_VL_IMC = Convert.ToDecimal(imc);

                    // Executa a operação
                    PACIENTE_EXAME_FISICOS item = Mapper.Map<PacienteExameFisicoViewModel, PACIENTE_EXAME_FISICOS>(vm);
                    Int32 volta = baseApp.ValidateEditExameFisico(item);

                    // Verifica retorno
                    Session["IdExameFisico"] = item.PAEF_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 6;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eefPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_EXAME_FISICOS>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 9;
                    hist.PAHI_IN_CHAVE = item.PAEF_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Exame Físico";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Exame Físico editado: " + item.PAEF_DT_DATA.Value.ToShortDateString();
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    if ((Int32)Session["VoltarPesquisa"] == 1)
                    {
                        return RedirectToAction("PesquisarTudo", "BaseAdmin");
                    }
                    if ((Int32)Session["VoltaFisico"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    else if ((Int32)Session["VoltaFisico"] == 2)
                    {
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerExameFisico(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exame Físico - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 6;

                PACIENTE_EXAME_FISICOS item = baseApp.GetExameFisicoById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                PacienteExameFisicoViewModel vm = Mapper.Map<PACIENTE_EXAME_FISICOS, PacienteExameFisicoViewModel>(item);
                Session["ExameFisico"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirConsulta()
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta - Inclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];


                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

                // Prepara listas
                ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
                Session["PacienteAlterada"] = 0;
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 500)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 501)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0526", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_1.pdf";
                PACIENTE_CONSULTA item = new PACIENTE_CONSULTA();
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    PACIENTE pac = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                }
                vm.PACO_IN_ATIVO = 1;
                vm.PACO_DT_CONSULTA = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.PACO_IN_TIPO = 1;
                vm.PACO_IN_ENCERRADA = 0;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConsulta(PacienteConsultaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Instancia caches
            CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);

            // Prepara listas
            ViewBag.Paciente = new SelectList(cache.CarregaCacheGeralListaPaciente("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["PacienteAlterada"]), "PACI__CD_ID", "PACI_NM_NOME");
            Session["PacienteAlterada"] = 0;
            var tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica de data
                    if (vm.PACO_DT_CONSULTA.Date < DateTime.Today.Date)
                    {
                        Session["MensPaciente"] = 501;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0526", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_HR_INICIO == vm.PACO_HR_FINAL)
                    {
                        Session["MensPaciente"] = 502;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_HR_INICIO > vm.PACO_HR_FINAL)
                    {
                        Session["MensPaciente"] = 503;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica se já tem consulta na data selecionada para o mesmo paciente
                    List<PACIENTE_CONSULTA> conPaciente = baseApp.GetAllConsultas(idAss).Where(p => p.PACI_CD_ID == vm.PACI_CD_ID & p.PACO_DT_CONSULTA == vm.PACO_DT_CONSULTA & p.PACO_IN_ATIVO == 1).ToList();
                    if (conPaciente.Count > 0)
                    {
                        Session["MensPaciente"] = 600;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0533", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de horario
                    List<PACIENTE_CONSULTA> lista = baseApp.GetAllConsultas(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_DT_CONSULTA == vm.PACO_DT_CONSULTA  & p.PACO_IN_ATIVO == 1).ToList();
                    List<PACIENTE_CONSULTA> lista1 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL).ToList();
                    List<PACIENTE_CONSULTA> lista2 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL & p.PACO_HR_FINAL >= vm.PACO_HR_INICIO).ToList();
                    List<PACIENTE_CONSULTA> lista3 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL & p.PACO_HR_INICIO <= vm.PACO_HR_FINAL).ToList();
                    List<PACIENTE_CONSULTA> lista4 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL).ToList();
                    if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                    {
                        Session["MensPaciente"] = 500;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    PACIENTE_CONSULTA item = Mapper.Map<PacienteConsultaViewModel, PACIENTE_CONSULTA>(vm);
                    Int32 volta = baseApp.ValidateCreateConsulta(item);

                    // Verifica retorno
                    Session["IdConsulta"] = item.PACO_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 3;
                    Session["ConsultasAlterada"] = 1;
                    Session["ListaConsultasGeral"] = null;
                    Session["IdPaciente"] = item.PACI_CD_ID;

                    // Verifica se tem consulta anterior para o paciente
                    List<PACIENTE_CONSULTA> consLista = cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultaAlterada"]);
                    Session["ConsultaAlterada"] = 0;
                    PACIENTE_CONSULTA consultaAnterior = consLista.Where(p => p.PACI_CD_ID == item.PACI_CD_ID & p.PACO_IN_ATIVO == 1 & p.PACO_DT_CONSULTA != item.PACO_DT_CONSULTA).OrderByDescending(p => p.PACO_DT_CONSULTA).FirstOrDefault();

                    // Cria anamnese em branco ou copia da ultima consulta
                    PACIENTE_ANAMNESE anamnese = new PACIENTE_ANAMNESE();
                    if (consultaAnterior == null)
                    {
                        anamnese.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        anamnese.PAAM_DT_DATA = vm.PACO_DT_CONSULTA;
                        anamnese.PAAM_IN_ATIVO = 1;
                        anamnese.PACI_CD_ID = item.PACI_CD_ID;
                        anamnese.USUA_CD_ID = usuario.USUA_CD_ID;
                        anamnese.PACO_CD_ID = item.PACO_CD_ID;
                        anamnese.PAAM_IN_PREENCHIDA = 0;
                    }
                    else
                    {
                        PACIENTE_ANAMNESE anamneseAnterior = baseApp.GetAllAnamnese(idAss).Where(p => p.PACO_CD_ID == consultaAnterior.PACO_CD_ID).First();
                        anamnese.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        anamnese.PAAM_DT_DATA = vm.PACO_DT_CONSULTA;
                        anamnese.PAAM_IN_ATIVO = 1;
                        anamnese.PACI_CD_ID = item.PACI_CD_ID;
                        anamnese.USUA_CD_ID = usuario.USUA_CD_ID;
                        anamnese.PACO_CD_ID = item.PACO_CD_ID;
                        anamnese.PAAM_IN_PREENCHIDA = 0;
                        anamnese.PAAM_DS_CONDUTA = anamneseAnterior.PAAM_DS_CONDUTA;
                        anamnese.PAAM_DS_DIAGNOSTICO_1 = anamneseAnterior.PAAM_DS_DIAGNOSTICO_1;
                        anamnese.PAAM_DS_DIAGNOSTICO_2 = anamneseAnterior.PAAM_DS_DIAGNOSTICO_2;
                        anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL = anamneseAnterior.PAAM_DS_HISTORIA_DOENCA_ATUAL;
                        anamnese.PAAM_DS_HISTORIA_FAMILIAR = anamneseAnterior.PAAM_DS_HISTORIA_FAMILIAR;
                        anamnese.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA = anamneseAnterior.PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA;
                        anamnese.PAAM_DS_HISTORIA_SOCIAL = anamneseAnterior.PAAM_DS_HISTORIA_SOCIAL;
                        anamnese.PAAM_DS_MOTIVO_CONSULTA = anamneseAnterior.PAAM_DS_MOTIVO_CONSULTA;
                        anamnese.PAAM_DS_QUEIXA_PRINCIPAL = anamneseAnterior.PAAM_DS_QUEIXA_PRINCIPAL;
                        anamnese.PAAM_TX_OBSERVACOES = anamneseAnterior.PAAM_TX_OBSERVACOES;
                        anamnese.PAAN_NM_ABDOMEM = anamneseAnterior.PAAN_NM_ABDOMEM;
                        anamnese.PAAN_NM_AVALIACAO_CARDIOLOGICA = anamneseAnterior.PAAN_NM_AVALIACAO_CARDIOLOGICA;
                        anamnese.PAAN_NM_MEMBROS_INFERIORES = anamneseAnterior.PAAN_NM_MEMBROS_INFERIORES;
                        anamnese.PAAN_NM_RESPIRATORIO = anamneseAnterior.PAAN_NM_RESPIRATORIO;
                        anamnese.PAAM_DT_COPIA = anamneseAnterior.PAAM_DT_DATA;
                    }
                    Int32 voltaA = baseApp.ValidateCreateAnamnese(anamnese);

                    // Cria exame fisico em branco ou copia da ultima consulta
                    PACIENTE_EXAME_FISICOS fisico = new PACIENTE_EXAME_FISICOS();
                    if (consultaAnterior == null)
                    {
                        fisico.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        fisico.PACI_CD_ID = item.PACI_CD_ID;
                        fisico.PAEF_DT_DATA = vm.PACO_DT_CONSULTA;
                        fisico.PAEF_IN_ALCOOLISMO = 0;
                        fisico.PAEF_IN_ALCOOLISMO_FREQUENCIA = 0;
                        fisico.PAEF_IN_ANTE_ALERGICO = 0;
                        fisico.PAEF_IN_ANTE_ONCOLOGICO = 0;
                        fisico.PAEF_IN_ANTICONCEPCIONAL = 0;
                        fisico.PAEF_IN_ATIVO = 1;
                        fisico.PAEF_IN_CIRURGIAS = 0;
                        fisico.PAEF_IN_DIABETE = 0;
                        fisico.PAEF_IN_EPILEPSIA = 0;
                        fisico.PAEF_IN_EXERCICIO_FISICO = 0;
                        fisico.PAEF_IN_EXERCICIO_FISICO_FREQUENCIA = 0;
                        fisico.PAEF_IN_GESTANTE = 0;
                        fisico.PAEF_IN_HIPERTENSAO = 0;
                        fisico.PAEF_IN_HIPOTENSAO = 0;
                        fisico.PAEF_IN_MARCAPASSO = 0;
                        fisico.PAEF_IN_TABAGISMO = 0;
                        fisico.PAEF_IN_VARIZES = 0;
                        fisico.USUA_CD_ID = usuario.USUA_CD_ID;
                        fisico.PACO_CD_ID = item.PACO_CD_ID;
                        fisico.PAEF_IN_PREENCHIDO = 0;
                        fisico.PAEF_VL_IMC = 0;
                    }
                    else
                    {
                        PACIENTE_EXAME_FISICOS fisicoAnterior = baseApp.GetAllExameFisico(idAss).Where(p => p.PACO_CD_ID == consultaAnterior.PACO_CD_ID).First();
                        fisico.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        fisico.PACI_CD_ID = item.PACI_CD_ID;
                        fisico.PAEF_DT_DATA = vm.PACO_DT_CONSULTA;
                        fisico.PAEF_IN_ATIVO = 1;
                        fisico.USUA_CD_ID = usuario.USUA_CD_ID;
                        fisico.PACO_CD_ID = item.PACO_CD_ID;
                        fisico.PAEF_IN_PREENCHIDO = 0;
                        fisico.PAEF_VL_IMC = 0;
                        fisico.PAEF_DS_ALCOOLISMO = fisicoAnterior.PAEF_DS_ALCOOLISMO;
                        fisico.PAEF_DS_ALERGICO = fisicoAnterior.PAEF_DS_ALERGICO;
                        fisico.PAEF_DS_ANTICONCEPCIONAL = fisicoAnterior.PAEF_DS_ANTICONCEPCIONAL;
                        fisico.PAEF_DS_EXAME_FISICO = fisicoAnterior.PAEF_DS_EXAME_FISICO;
                        fisico.PAEF_DS_EXERCICIO_FISICO = fisicoAnterior.PAEF_DS_EXERCICIO_FISICO;
                        fisico.PAEF_DS_MARCAPASSO = fisicoAnterior.PAEF_DS_MARCAPASSO;
                        fisico.PAEF_DS_ONCOLOGICO = fisicoAnterior.PAEF_DS_ONCOLOGICO;
                        fisico.PAEF_DS_TABAGISMO = fisicoAnterior.PAEF_DS_TABAGISMO;
                        fisico.PAEF_IN_ALCOOLISMO = fisicoAnterior.PAEF_IN_ALCOOLISMO;
                        fisico.PAEF_IN_ALCOOLISMO_FREQUENCIA = fisicoAnterior.PAEF_IN_ALCOOLISMO_FREQUENCIA;
                        fisico.PAEF_IN_ANTE_ALERGICO = fisicoAnterior.PAEF_IN_ANTE_ALERGICO;
                        fisico.PAEF_IN_ANTE_ONCOLOGICO = fisicoAnterior.PAEF_IN_ANTE_ONCOLOGICO;
                        fisico.PAEF_IN_ANTICONCEPCIONAL = fisicoAnterior.PAEF_IN_ANTICONCEPCIONAL;
                        fisico.PAEF_IN_CIRURGIAS = fisicoAnterior.PAEF_IN_CIRURGIAS;
                        fisico.PAEF_IN_DIABETE = fisicoAnterior.PAEF_IN_DIABETE;
                        fisico.PAEF_IN_EPILEPSIA = fisicoAnterior.PAEF_IN_EPILEPSIA;
                        fisico.PAEF_IN_EXERCICIO_FISICO = fisicoAnterior.PAEF_IN_EXERCICIO_FISICO;
                        fisico.PAEF_IN_EXERCICIO_FISICO_FREQUENCIA = fisicoAnterior.PAEF_IN_EXERCICIO_FISICO_FREQUENCIA;
                        fisico.PAEF_IN_GESTANTE = fisicoAnterior.PAEF_IN_GESTANTE;
                        fisico.PAEF_IN_HIPERTENSAO = fisicoAnterior.PAEF_IN_HIPERTENSAO;
                        fisico.PAEF_IN_HIPOTENSAO = fisicoAnterior.PAEF_IN_HIPOTENSAO;
                        fisico.PAEF_IN_MARCAPASSO = fisicoAnterior.PAEF_IN_MARCAPASSO;
                        fisico.PAEF_IN_TABAGISMO = fisicoAnterior.PAEF_IN_TABAGISMO;
                        fisico.PAEF_IN_VARIZES = fisicoAnterior.PAEF_IN_VARIZES;
                        fisico.PAEF_NR_ALTURA = fisicoAnterior.PAEF_NR_ALTURA;
                        fisico.PAEF_NR_FREQUENCIA_CARDIACA = fisicoAnterior.PAEF_NR_FREQUENCIA_CARDIACA;
                        fisico.PAEF_NR_MES_GESTANTE = fisicoAnterior.PAEF_NR_MES_GESTANTE;
                        fisico.PAEF_NR_PA_ALTA = fisicoAnterior.PAEF_NR_PA_ALTA;
                        fisico.PAEF_NR_PA_BAIXA = fisicoAnterior.PAEF_NR_PA_BAIXA;
                        fisico.PAEF_NR_PESO = fisicoAnterior.PAEF_NR_PESO;
                        fisico.PAEF_NR_TEMPERATURA = fisicoAnterior.PAEF_NR_TEMPERATURA;
                        fisico.PAEF_TX_CIRURGIAS = fisicoAnterior.PAEF_TX_CIRURGIAS;
                        fisico.PAEF_VL_IMC = fisicoAnterior.PAEF_VL_IMC;
                        fisico.PAEF_DT_COPIA = fisicoAnterior.PAEF_DT_DATA;
                    }
                    Int32 voltaF = baseApp.ValidateCreateExameFisico(fisico);

                    // Acerta proxima consulta do paciente
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    List<PACIENTE_CONSULTA> cons = pac.PACIENTE_CONSULTA.Where(p => p.PACO_DT_CONSULTA.Date > DateTime.Today.Date).ToList();
                    if (cons.Count == 0)
                    {
                        pac.PACI_DT_PREVISAO_RETORNO = item.PACO_DT_CONSULTA;
                        Int32 voltaP = baseApp.ValidateEdit(pac, pac);
                    }
                    else
                    {
                        cons = cons.Where(p => p.PACO_DT_CONSULTA.Date < item.PACO_DT_CONSULTA.Date).ToList();
                        if (cons.Count == 0)
                        {
                            pac.PACI_DT_PREVISAO_RETORNO = item.PACO_DT_CONSULTA;
                            Int32 voltaP = baseApp.ValidateEdit(pac, pac);
                        }
                    }

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "icoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_CONSULTA>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 10;
                    hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Inclusão de Consulta";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Consulta incluída: " + item.PACO_DT_CONSULTA.ToShortDateString();
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    if ((Int32)Session["ModoConsulta"] == 1)
                    {
                        Session["ConsultaMarcada"] = 1;
                        Session["ConsultaFrase"] = item.PACO_DT_CONSULTA.ToShortDateString() + " de " + item.PACO_HR_INICIO.ToString() + " até " + item.PACO_HR_FINAL.ToString();
                        return RedirectToAction("VoltarProcederConsulta");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        if ((Int32)Session["ProximaConsulta"] == 1)
                        {
                            return RedirectToAction("VoltarProcederConsulta");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 6)
                    {
                        return RedirectToAction("MontarTelaConsultas");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta - Edição";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                Session["NivelPaciente"] = 3;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_2.pdf";
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                Session["IdPaciente"] = item.PACI_CD_ID;
                Session["IdConsulta"] = item.PACO_CD_ID;
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarConsulta(PacienteConsultaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            var tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica de data
                    if (vm.PACO_DT_CONSULTA.Date < DateTime.Today.Date)
                    {
                        Session["MensPaciente"] = 501;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0526", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_HR_INICIO == vm.PACO_HR_FINAL)
                    {
                        Session["MensPaciente"] = 502;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_HR_INICIO > vm.PACO_HR_FINAL)
                    {
                        Session["MensPaciente"] = 503;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de horario
                    List<PACIENTE_CONSULTA> lista = baseApp.GetAllConsultas(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_DT_CONSULTA == vm.PACO_DT_CONSULTA & p.PACO_CD_ID != vm.PACO_CD_ID).ToList();
                    List<PACIENTE_CONSULTA> lista1 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL).ToList();
                    List<PACIENTE_CONSULTA> lista2 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL & p.PACO_HR_FINAL >= vm.PACO_HR_INICIO).ToList();
                    List<PACIENTE_CONSULTA> lista3 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL & p.PACO_HR_INICIO <= vm.PACO_HR_FINAL).ToList();
                    List<PACIENTE_CONSULTA> lista4 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL).ToList();
                    if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                    {
                        Session["MensPaciente"] = 500;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    PACIENTE_CONSULTA item = Mapper.Map<PacienteConsultaViewModel, PACIENTE_CONSULTA>(vm);
                    Int32 volta = baseApp.ValidateEditConsulta(item);

                    // Verifica retorno
                    Session["IdConsulta"] = item.PACO_CD_ID;
                    Session["PacienteAlterada"] = 1;
                    Session["NivelPaciente"] = 3;
                    Session["ConsultasAlterada"] = 1;
                    Session["IdPaciente"] = item.PACI_CD_ID;
                    Session["ListaConsultasGeral"] = null;
                    Session["ListaConsultas"] = null;

                    // Acerta paciente
                    PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                    pac.PACI_DT_PREVISAO_RETORNO = vm.PACO_DT_CONSULTA;
                    Int32 voltaP = baseApp.ValidateEdit(pac, pac);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ecoPACI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PACIENTE_CONSULTA>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 10;
                    hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Edição de Consulta";
                    hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Consulta editada: " + item.PACO_DT_CONSULTA.ToShortDateString();
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Retorno
                    if ((Int32)Session["TipoSolicitacao"] == 1)
                    {
                        if ((Int32)Session["VoltaAtestado"] == 1)
                        {
                            return RedirectToAction("MontarTelaCentralPaciente");
                        }
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["TipoSolicitacao"] == 6)
                    {
                        return RedirectToAction("MontarTelaConsultas");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
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
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PACIENTE_CONSULTA_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta - Exclusão";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditConsulta(item);

                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 3;
                Session["ListaConsultasGeral"] = null;
                Session["ConsultasAlterada"] = 1;

                // Monta Log
                PACIENTE cli = baseApp.GetItemById(item.PACI_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xcoPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Paciente: " + cli.PACI_NM_NOME + " | Data: " + item.PACO_DT_CONSULTA,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 10;
                hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Exclusão de Consulta";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME + " - Consulta excluída: " + item.PACO_DT_CONSULTA.ToShortDateString();
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaCentralPaciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PACIENTE_CONSULTA_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta - Consulta";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_2.pdf";
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);
                Session["IdConsulta"] = item.PACO_CD_ID;

                // Carrega anamnese
                PACIENTE_ANAMNESE anamnese = item.PACIENTE_ANAMNESE.ToList().FirstOrDefault();
                vm.PAAN_DS_MOTIVO_CONSULTA = anamnese.PAAM_DS_MOTIVO_CONSULTA;
                vm.PAAN_DS_QUEIXA_PRINCIPAL = anamnese.PAAM_DS_QUEIXA_PRINCIPAL;
                vm.PAAN_DS_HISTORIA_FAMILIAR = anamnese.PAAM_DS_HISTORIA_FAMILIAR;
                vm.PAAN_DS_HISTORIA_DOENCA_ATUAL = anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL;
                vm.PAAN_DS_DIAGNOSTICO_1 = anamnese.PAAM_DS_DIAGNOSTICO_1;
                vm.PAAN_DS_CONDUTA = anamnese.PAAM_DS_CONDUTA;
                Session["IdAnamneseConsulta"] = anamnese.PAAM_CD_ID;
                Session["EditaAnamnese"] = 0;

                // Carrega exame físico
                PACIENTE_EXAME_FISICOS fisico = item.PACIENTE_EXAME_FISICOS.ToList().FirstOrDefault();
                vm.PAEF_DT_DATA = fisico.PAEF_DT_DATA;
                vm.PAEF_NR_PA_ALTA = fisico.PAEF_NR_PA_ALTA;
                vm.PAEF_NR_PA_BAIXA = fisico.PAEF_NR_PA_BAIXA;
                vm.PAEF_NR_FREQUENCIA_CARDIACA = fisico.PAEF_NR_FREQUENCIA_CARDIACA;
                vm.PAEF_NR_PESO = fisico.PAEF_NR_PESO;
                vm.PAEF_NR_ALTURA = fisico.PAEF_NR_ALTURA;
                vm.PAEF_VL_IMC = fisico.PAEF_VL_IMC;
                vm.PAEF_DS_EXAME_FISICO = fisico.PAEF_DS_EXAME_FISICO;
                Session["IdFisicoConsulta"] = fisico.PAEF_CD_ID;
                Session["EditaFisico"] = 0;

                // Prepara view
                objetoAntes = (PACIENTE)Session["Paciente"];
                Session["Consulta"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerPacientesPrevistoAtraso()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
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

                // Instancia caches
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carrega Configuração
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                List<PACIENTE> listaCli = ((List<PACIENTE>)Session["ListaPaciente"]).ToList();
                List<PACIENTE> listaAtraso = new List<PACIENTE>();
                List<PACIENTE> listaBase = new List<PACIENTE>();
                if ((List<PACIENTE>)Session["PacientesAtraso"] == null)
                {
                    listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null & p.PACI_DT_CONSULTA != null).ToList();
                    listaAtraso = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_ATRASO.Value) < DateTime.Today.Date).ToList();
                    listaMasterAtraso = listaAtraso;
                    Session["PacientesAtraso"] = listaMasterAtraso;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE>)Session["PacientesAtraso"];

                ViewBag.Tipos = new SelectList(cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIPA_CD_ID", "TIPA_NM_NOME");
                Session["TipoPacienteAlterada"] = 0;
                List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
                List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
                ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");

                Session["Paciente"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_1.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Widgets
                Int32 totalCli = listaCli.Count;
                ViewBag.Pacientes = totalCli;
                ViewBag.Atrasos = listaAtraso.Count;
                List<PACIENTE> listaAusencia = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_AUSENCIA.Value) < DateTime.Today.Date).ToList();
                ViewBag.Ausencias = listaAusencia.Count;
                ViewBag.Atraso = conf.CONF_IN_PACIENTE_ATRASO;

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0515", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 77)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0511", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0512", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 400)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 401)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 100)
                    {
                        String frase = "Foram enviadas " + (String)Session["TotMensagens"] + " mensagens para pacientes";
                        ModelState.AddModelError("", frase);
                    }
                }

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 11;
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 3;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;

                // Carrega view
                objeto = new PACIENTE();
                if (Session["FiltroPacienteAtraso"] != null)
                {
                    objeto = (PACIENTE)Session["FiltroPacienteAtraso"];
                }
                return View(objeto);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarPacienteAtraso(PACIENTE item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.PACI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_NOME);
                item.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_CPF);
                item.PACI_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_MATRICULA);
                item.PACI_NM_INDICACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_INDICACAO);
                item.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(item.PACI_NR_CELULAR);
                item.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(item.PACI_NM_EMAIL);
                item.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_CIDADE);

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE> listaObj = new List<PACIENTE>();
                Session["FiltroPacienteAtraso"] = item;
                Tuple<Int32, List<PACIENTE>, Boolean> volta = baseApp.ExecuteFilterTuple(item.PACI__CD_ID, item.TIPA_CD_ID, item.SEXO_CD_ID, item.PACI_NM_NOME, item.PACI_NR_CPF, item.PACI_NR_MATRICULA, item.PACI_NM_INDICACAO, item.PACI_NR_CELULAR, item.PACI_NM_EMAIL, item.PACI_NM_CIDADE, item.UF_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("VerPacientesPrevistoAtraso");
                }

                // Trata atraso
                List<PACIENTE> listaCli = volta.Item2.ToList();
                List<PACIENTE> listaAtraso = new List<PACIENTE>();
                List<PACIENTE> listaBase = new List<PACIENTE>();
                listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null & p.PACI_DT_CONSULTA != null).ToList();
                listaAtraso = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_ATRASO.Value) < DateTime.Today.Date).ToList();

                // Sucesso
                listaMaster = listaAtraso;
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["PacientesAtraso"] = listaMaster;
                return RedirectToAction("VerPacientesPrevistoAtraso");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroPacienteAtraso()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["PacientesAtraso"] = null;
                Session["FiltroPacienteAtraso"] = null;
                return RedirectToAction("VerPacientesPrevistoAtraso");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioListaAtraso()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Carrega Configuração
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PacienteAtrasoLista" + "_" + data + ".pdf";
                List<PACIENTE> lista = (List<PACIENTE>)Session["PacientesAtraso"];
                PACIENTE filtro = (PACIENTE)Session["FiltroPacienteAtraso"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes em Atraso de Retorno - Maior que " + conf.CONF_IN_PACIENTE_ATRASO.ToString() + " dias", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 100f, 70f, 60f, 100f, 60f, 80f, 40f, 60f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Pacientes selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 10;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("UF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ult.Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Retorno", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Atraso (Dias)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("   ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.TIPO_PACIENTE.TIPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CPF != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CPF, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CELULAR != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CELULAR, meuFont))
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
                    if (item.PACI_NM_CIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NM_CIDADE, meuFont))
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
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACI_DT_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_CONSULTA.Value.ToShortDateString(), meuFont))
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
                    if (item.PACI_DT_PREVISAO_RETORNO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_PREVISAO_RETORNO.Value.ToShortDateString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(DateTime.Today.Date.Subtract(item.PACI_DT_PREVISAO_RETORNO.Value).TotalDays.ToString(), meuFont))
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
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (System.IO.File.Exists(Server.MapPath(item.PACI_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.PACI_AQ_FOTO));
                        image.ScaleAbsolute(40, 40);
                        cell.AddElement(image);
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.TIPA_CD_ID > 0)
                    {
                        TIPO_PACIENTE cat = tpaApp.GetItemById(filtro.TIPA_CD_ID);
                        parametros += "Categoria: " + cat.TIPA_NM_NOME;
                        ja = 1;
                    }
                    if (filtro.PACI__CD_ID > 0)
                    {
                        PACIENTE cli = baseApp.GetItemById(filtro.PACI__CD_ID);
                        if (ja == 0)
                        {
                            parametros += "Nome: *" + cli.PACI_NM_NOME + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: *" + cli.PACI_NM_NOME + "*";
                        }
                    }
                    if (filtro.PACI_NR_CPF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "CPF: " + filtro.PACI_NR_CPF;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e CPF: " + filtro.PACI_NR_CPF;
                        }
                    }
                    if (filtro.PACI_NM_EMAIL != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                        }
                    }
                    if (filtro.PACI_NM_CIDADE != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                        }
                    }
                    if (filtro.UF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
                        }
                    }
                    if (filtro.PACI_NR_MATRICULA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                        }
                    }
                    if (filtro.PACI_NM_INDICACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                        }
                    }
                    if (filtro.SEXO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                        }
                    }
                    if (ja == 0)
                    {
                        parametros = "Nenhum filtro definido.";
                    }
                }
                else
                {
                    parametros = "Nenhum filtro definido.";
                }
                Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                return RedirectToAction("VerPacientesPrevistoAtraso");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerPacientesAusentes()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                List<PACIENTE> listaCli = ((List<PACIENTE>)Session["ListaPaciente"]).ToList();
                List<PACIENTE> listaAusencia = new List<PACIENTE>();
                List<PACIENTE> listaBase = new List<PACIENTE>();
                if ((List<PACIENTE>)Session["PacientesAusente"] == null)
                {
                    listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null & p.PACI_DT_CONSULTA != null).ToList();
                    listaAusencia = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_AUSENCIA.Value) < DateTime.Today.Date).ToList();
                    listaMasterAusencia = listaAusencia;
                    Session["PacientesAusente"] = listaMasterAusencia;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE>)Session["PacientesAusente"];
                ViewBag.Tipos = new SelectList(cacheTab.CarregaCacheGeralListaTipoPaciente("TIPO_PACIENTE", usuario.ASSI_CD_ID, (Int32)Session["TipoPacienteAlterada"]), "TIPA_CD_ID", "TIPA_NM_NOME");
                Session["TipoPacienteAlterada"] = 0;
                List<UF> uFs = cache.CarregaCacheGeralListaUF("UF", usuario.ASSI_CD_ID, 0);
                List<USUARIO> usuarios = cache.CarregaCacheGeralListaUsuario("USUARIO", usuario.ASSI_CD_ID, (Int32)Session["UsuarioAlterada"]);
                ViewBag.Usuarios = new SelectList(usuarios, "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");
                Session["Paciente"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_1.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Widgets
                Int32 totalCli = listaCli.Count;
                ViewBag.Pacientes = totalCli;
                ViewBag.Ausencias = listaAusencia.Count;
                ViewBag.Ausencia = conf.CONF_IN_PACIENTE_AUSENCIA;

                // Recupera mensagens enviadas
                List<MENSAGENS_ENVIADAS_SISTEMA> mensTotal = cacheMens.CarregaCacheGeralListaMensagensEnviadas("MENSAGEM_ENVIADA", usuario.ASSI_CD_ID, (Int32)Session["MensagensEnviadaAlterada"]);
                Session["MensagensEnviadaAlterada"] = 0;

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = mensTotal.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0515", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 77)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0511", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0512", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 400)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 401)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 100)
                    {
                        String frase = "Foram enviadas " + (String)Session["TotMensagens"] + " mensagens para pacientes";
                        ModelState.AddModelError("", frase);
                    }
                }

                // Acerta estado
                Session["FlagMensagensEnviadas"] = 11;
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 3;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;

                // Carrega view
                objeto = new PACIENTE();
                if (Session["FiltroPacienteAusencia"] != null)
                {
                    objeto = (PACIENTE)Session["FiltroPacienteAusencia"];
                }
                return View(objeto);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarPacienteAusencia(PACIENTE item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Sanitização
                item.PACI_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_NOME);
                item.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_CPF);
                item.PACI_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(item.PACI_NR_MATRICULA);
                item.PACI_NM_INDICACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_INDICACAO);
                item.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(item.PACI_NR_CELULAR);
                item.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(item.PACI_NM_EMAIL);
                item.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACI_NM_CIDADE);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE> listaObj = new List<PACIENTE>();
                Session["FiltroPacienteAusencia"] = item;
                Tuple<Int32, List<PACIENTE>, Boolean> volta = baseApp.ExecuteFilterTuple(item.PACI__CD_ID, item.TIPA_CD_ID, item.SEXO_CD_ID, item.PACI_NM_NOME, item.PACI_NR_CPF, item.PACI_NR_MATRICULA, item.PACI_NM_INDICACAO, item.PACI_NR_CELULAR, item.PACI_NM_EMAIL, item.PACI_NM_CIDADE, item.UF_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("VerPacientesAusentes");
                }

                // Trata atraso
                List<PACIENTE> listaCli = volta.Item2.ToList();
                List<PACIENTE> listaAusencia = new List<PACIENTE>();
                List<PACIENTE> listaBase = new List<PACIENTE>();
                listaBase = listaCli.Where(p => p.PACI_DT_PREVISAO_RETORNO != null & p.PACI_DT_CONSULTA != null).ToList();
                listaAusencia = listaBase.Where(p => p.PACI_DT_PREVISAO_RETORNO > p.PACI_DT_CONSULTA & p.PACI_DT_PREVISAO_RETORNO.Value.AddMonths(conf.CONF_IN_PACIENTE_AUSENCIA.Value) < DateTime.Today.Date).ToList();

                // Sucesso
                listaMaster = listaAusencia;
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["PacientesAusencia"] = listaMaster;
                return RedirectToAction("VerPacientesAusentes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroPacienteAusencia()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["PacientesAusencia"] = null;
                Session["FiltroPacienteAusenia"] = null;
                return RedirectToAction("VerPacientesAusentes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioListaAusencia()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PacienteAusenciaLista" + "_" + data + ".pdf";
                List<PACIENTE> lista = (List<PACIENTE>)Session["PacientesAusencia"];
                PACIENTE filtro = (PACIENTE)Session["FiltroPacienteAusencia"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Pacientes Ausentes - Atraso maior que " + conf.CONF_IN_PACIENTE_AUSENCIA.ToString() + " dias", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 100f, 70f, 60f, 100f, 60f, 80f, 40f, 60f, 60f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell = new PdfPCell(new Paragraph("Pacientes selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 10;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("UF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Ult.Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Retorno", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Atraso (Dias)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("   ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.TIPO_PACIENTE.TIPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CPF != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CPF, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACI_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACI_NR_CELULAR != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NR_CELULAR, meuFont))
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
                    if (item.PACI_NM_CIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_NM_CIDADE, meuFont))
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
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACI_DT_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_CONSULTA.Value.ToShortDateString(), meuFont))
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
                    if (item.PACI_DT_PREVISAO_RETORNO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACI_DT_PREVISAO_RETORNO.Value.ToShortDateString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(DateTime.Today.Date.Subtract(item.PACI_DT_PREVISAO_RETORNO.Value).TotalDays.ToString(), meuFont))
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
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (System.IO.File.Exists(Server.MapPath(item.PACI_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.PACI_AQ_FOTO));
                        image.ScaleAbsolute(40, 40);
                        cell.AddElement(image);
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.TIPA_CD_ID > 0)
                    {
                        TIPO_PACIENTE cat = tpaApp.GetItemById(filtro.TIPA_CD_ID);
                        parametros += "Categoria: " + cat.TIPA_NM_NOME;
                        ja = 1;
                    }
                    if (filtro.PACI__CD_ID > 0)
                    {
                        PACIENTE cli = baseApp.GetItemById(filtro.PACI__CD_ID);
                        if (ja == 0)
                        {
                            parametros += "Nome: *" + cli.PACI_NM_NOME + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: *" + cli.PACI_NM_NOME + "*";
                        }
                    }
                    if (filtro.PACI_NR_CPF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "CPF: " + filtro.PACI_NR_CPF;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e CPF: " + filtro.PACI_NR_CPF;
                        }
                    }
                    if (filtro.PACI_NM_EMAIL != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e E-Mail: *" + filtro.PACI_NM_EMAIL + "*";
                        }
                    }
                    if (filtro.PACI_NM_CIDADE != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Cidade: *" + filtro.PACI_NM_CIDADE + "*";
                        }
                    }
                    if (filtro.UF != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
                        }
                    }
                    if (filtro.PACI_NR_MATRICULA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Matrícula: *" + filtro.PACI_NR_MATRICULA + "*";
                        }
                    }
                    if (filtro.PACI_NM_INDICACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Indicação: *" + filtro.PACI_NM_INDICACAO + "*";
                        }
                    }
                    if (filtro.SEXO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Gênero: " + filtro.SEXO.SEXO_NM_NOME;
                        }
                    }
                    if (ja == 0)
                    {
                        parametros = "Nenhum filtro definido.";
                    }
                }
                else
                {
                    parametros = "Nenhum filtro definido.";
                }
                Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                return RedirectToAction("VerPacientesPrevistoAtraso");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerListaPrescricaoConsulta()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Prescrição - Lista";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera precricoes da consulta
                PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(consulta);
                List<PACIENTE_PRESCRICAO> prescricoes = consulta.PACIENTE_PRESCRICAO.ToList();
                ViewBag.Listas = prescricoes;

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["ModoConsulta"] = 2;
                Session["VoltaListaConsulta"] = 1;
                Session["TipoSolicitacao"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerListaAtestadoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditarAtestado"] = 0;
            return RedirectToAction("VerListaAtestadoConsulta");
        }

        public ActionResult VerListaSolicitacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditarSolicitacao"] = 0;
            return RedirectToAction("VerListaSolicitacaoConsulta");
        }

        public ActionResult VerListaPrescricaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditarPrescricao"] = 0;
            return RedirectToAction("VerListaPrescricaoConsulta");
        }

        public ActionResult VerListaExameForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditarExame"] = 0;
            return RedirectToAction("VerListaExameConsulta");
        }

        [HttpGet]
        public ActionResult ProcederConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PACIENTE_CONSULTA_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Consulta - Proceder";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera consulta
                Session["NivelPaciente"] = 3;
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);
                Session["IdConsulta"] = item.PACO_CD_ID;

                // Recupera paciente
                PACIENTE paciente = baseApp.GetItemById(item.PACI_CD_ID);
                Session["IdPaciente"] = paciente.PACI__CD_ID; 
                
                // Carrega anamnese
                PACIENTE_ANAMNESE anamnese = item.PACIENTE_ANAMNESE.ToList().FirstOrDefault();
                vm.PAAN_DS_MOTIVO_CONSULTA = anamnese.PAAM_DS_MOTIVO_CONSULTA;
                vm.PAAN_DS_QUEIXA_PRINCIPAL = anamnese.PAAM_DS_QUEIXA_PRINCIPAL;
                vm.PAAN_DS_HISTORIA_FAMILIAR = anamnese.PAAM_DS_HISTORIA_FAMILIAR;
                vm.PAAN_DS_HISTORIA_DOENCA_ATUAL = anamnese.PAAM_DS_HISTORIA_DOENCA_ATUAL;
                vm.PAAN_DS_DIAGNOSTICO_1 = anamnese.PAAM_DS_DIAGNOSTICO_1;
                vm.PAAN_DS_CONDUTA = anamnese.PAAM_DS_CONDUTA;
                if (anamnese.PAAM_DT_COPIA != null)
                {
                    vm.PAAN_DT_COPIA = anamnese.PAAM_DT_COPIA.Value;
                }
                Session["IdAnamneseConsulta"] = anamnese.PAAM_CD_ID;
                Session["EditaAnamnese"] = 1;
                Session["VoltaAnamnese"] = 2;
                Session["VoltaFisico"] = 2;
                Session["ModoConsulta"] = 1;
                Session["VerTodosPaciente"] = 0;
                Session["VoltaListaConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/11/Ajuda11.pdf";

                // Carrega exame físico
                PACIENTE_EXAME_FISICOS fisico = item.PACIENTE_EXAME_FISICOS.ToList().FirstOrDefault();
                vm.PAEF_DT_DATA = fisico.PAEF_DT_DATA;
                vm.PAEF_NR_PA_ALTA = fisico.PAEF_NR_PA_ALTA;
                vm.PAEF_NR_PA_BAIXA = fisico.PAEF_NR_PA_BAIXA;
                vm.PAEF_NR_FREQUENCIA_CARDIACA = fisico.PAEF_NR_FREQUENCIA_CARDIACA;
                vm.PAEF_NR_PESO = fisico.PAEF_NR_PESO;
                vm.PAEF_NR_ALTURA = fisico.PAEF_NR_ALTURA;
                vm.PAEF_VL_IMC = fisico.PAEF_VL_IMC;
                vm.PAEF_DS_EXAME_FISICO = fisico.PAEF_DS_EXAME_FISICO;
                Session["IdFisicoConsulta"] = fisico.PAEF_CD_ID;
                Session["EditaFisico"] = 1;

                // Mensagem
                if ((Int32)Session["ConsultaMarcada"] == 1)
                {
                    String frase = (String)Session["ConsultaFrase"];
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0534", CultureInfo.CurrentCulture) + " " + frase);
                }

                // Prepara view
                Session["ConsultaMarcada"] = 0;
                objetoAntes = (PACIENTE)Session["Paciente"];
                Session["Consulta"] = item;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerListaAtestadoFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditaAtestado"] = 1;
            return RedirectToAction("VerListaAtestadoConsulta");
        }

        public ActionResult VerListaSolicitacaoFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditaSolicitacao"] = 1;
            return RedirectToAction("VerListaSolicitacaoConsulta");
        }

        public ActionResult VerListaPrescricaoFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditaPrescricao"] = 1;
            Session["EscopoConsulta"] = 1;
            return RedirectToAction("VerListaPrescricaoConsulta");
        }

        public ActionResult VerListaPrescricaoFormConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditaPrescricao"] = 1;
            Session["EscopoConsulta"] = 2;
            return RedirectToAction("VerListaPrescricaoConsulta");
        }


        public ActionResult VerListaExameFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["EditaExame"] = 1;
            return RedirectToAction("VerListaExameConsulta");
        }

        [HttpGet]
        public ActionResult VerListaAtestadoConsulta()
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Atestado - Lista";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera atestados da consulta
                PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(consulta);
                List<PACIENTE_ATESTADO> atestados = consulta.PACIENTE_ATESTADO.ToList();
                ViewBag.Listas = atestados;

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["ModoConsulta"] = 2;
                Session["IdConsultaCria"] = 0;
                Session["VoltaListaConsulta"] = 1;
                Session["TipoSolicitacao"] = 1;
                Session["IdConsultaCria"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerListaConsultasConsulta()
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
                    if (usuario.PERFIL.PERF_IN_PACIENTE_CONSULTA_ACESSO == 0)
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

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0528", CultureInfo.CurrentCulture));
                    }
                }

                // Recupera consultas do paciente
                PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(paciente);
                List<PACIENTE_CONSULTA> consultas = paciente.PACIENTE_CONSULTA.OrderByDescending(p => p.PACO_DT_CONSULTA).ToList();
                ViewBag.Listas = consultas;

                // Prepara view
                Session["ModoConsulta"] = 2;
                Session["IdConsultaCria"] = 0;
                Session["VoltaListaConsulta"] = 1;
                Session["TipoSolicitacao"] = 1;
                Session["IdConsultaCria"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerListaSolicitacaoConsulta()
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Solicitação - Lista";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera solicitacoes da consulta
                PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(consulta);
                List<PACIENTE_SOLICITACAO> solicitacoes = consulta.PACIENTE_SOLICITACAO.ToList();
                ViewBag.Listas = solicitacoes;

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["ModoConsulta"] = 2;
                Session["IdConsultaCria"] = 0;
                Session["VoltaListaConsulta"] = 1;
                Session["TipoSolicitacao"] = 1;
                Session["IdConsultaCria"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerListaExameConsulta()
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Paciente - Exames - Lista";
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exames da consulta
                PACIENTE_CONSULTA consulta = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(consulta);
                List<PACIENTE_EXAMES> exames = consulta.PACIENTE_EXAMES.ToList();
                ViewBag.Lista = exames;

                // Prepara view
                Session["NivelPaciente"] = 3;
                Session["NivelPaciente"] = 3;
                Session["ModoConsulta"] = 2;    
                Session["IdConsultaCria"] = 0;
                Session["VoltaListaConsulta"] = 1;
                Session["TipoSolicitacao"] = 1;
                Session["IdConsultaCria"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarVerConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerConsulta", new { id = (Int32)Session["IdConsulta"] });
        }

        [HttpGet]
        public ActionResult MontarTelaSolicitacoes()
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
                    if (usuario.PERFIL.PERF_IN_SOLICITACAO_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaSolicitacoes"] == null)
                {
                    listaMasterSolicitacao = (List<PACIENTE_SOLICITACAO>)cache.CarregaCacheGeralListaSolicitacao("SOLICITACAO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["SolicitacoesAlterada"]).ToList();
                    Session["SolicitacoesAlterada"] = 0;

                    listaMasterSolicitacao = listaMasterSolicitacao.Where(p => p.PASO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderByDescending(p => p.PASO_DT_EMISSAO).ToList();
                    Session["ListaSolicitacoes"] = listaMasterSolicitacao;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_SOLICITACAO>)Session["ListaSolicitacoes"];
                ViewBag.Tipos = new SelectList((List<TIPO_EXAME>)cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]).ToList(), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoExameAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 2;
                Session["ModoConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/8/Ajuda8.pdf";

                // Carrega view
                objetoSolicitacao = new PACIENTE_SOLICITACAO();
                return View(objetoSolicitacao);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarSolicitacoes(PACIENTE_SOLICITACAO item)
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
                item.PASO_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PASO_NM_TITULO);
                item.PASO_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PASO_TX_TEXTO);
                item.PASO_AQ_ARQUIVO_HTML = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PASO_AQ_ARQUIVO_HTML);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                List<PACIENTE_SOLICITACAO> listaObj = new List<PACIENTE_SOLICITACAO>();
                Tuple<Int32, List<PACIENTE_SOLICITACAO>, Boolean> volta = baseApp.ExecuteFilterTupleSolicitacao(item.TIEX_CD_ID, item.PASO_AQ_ARQUIVO_HTML, item.PASO_DT_EMISSAO, item.PASO_DT_ENVIO, item.PASO_NM_TITULO, item.PASO_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaSolicitacoes");
                }

                // Sucesso
                listaMasterSolicitacao = volta.Item2.ToList();;
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMasterSolicitacao = listaMasterSolicitacao.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaSolicitacoes"] = listaMasterSolicitacao;
                return RedirectToAction("MontarTelaSolicitacoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroSolicitacoes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaSolicitacoes"] = null;
                return RedirectToAction("MontarTelaSolicitacoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioSolicitacoes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "SolicitacaoLista" + "_" + data + ".pdf";
                List<PACIENTE_SOLICITACAO> lista = (List<PACIENTE_SOLICITACAO>)Session["ListaSolicitacoes"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Solicitações de Exames", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 60f, 120f, 80f, 120f, 120f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_SOLICITACAO item in lista)
                {
                    if (item.PASO_DT_EMISSAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PASO_DT_EMISSAO.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PASO_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.TIPO_EXAME != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_EXAME.TIEX_NM_NOME, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PASO_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PASO_IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PASO_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PASO_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaSolicitacoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirSolicitacaoCompleta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 2;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirSolicitacao");
        }

        public JsonResult GetPaciente(Int32 id)
        {
            var paciente = baseApp.GetItemById(id);
            var hash = new Hashtable();
            hash.Add("dataNasc", paciente.PACI_DT_NASCIMENTO.Value.ToShortDateString());
            hash.Add("cpf", paciente.PACI_NR_CPF);
            hash.Add("email", paciente.PACI_NM_EMAIL);
            return Json(hash);
        }

        public ActionResult EditarSolicitacaoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 2;
            Session["IdConsultaCria"] = 0;
            Session["VoltarPesquisa"] = 0;
            return RedirectToAction("EditarSolicitacao", new { id = id });
        }

        public ActionResult VerSolicitacaoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 2;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("VerSolicitacao", new { id = id });
        }

        public ActionResult EditarPacienteSolicitacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 2;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacienteSolicitacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 2;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaAtestados()
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
                    if (usuario.PERFIL.PERF_IN_ATESTADO_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaAtestados"] == null)
                {
                    listaMasterAtestado = (List<PACIENTE_ATESTADO>)cache.CarregaCacheGeralListaAtestados("ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["AtestadosAlterada"]).ToList();
                    Session["AtestadosAlterada"] = 0;
                    listaMasterAtestado = listaMasterAtestado.Where(p => p.PAAT_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Session["ListaAtestados"] = listaMasterAtestado;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_ATESTADO>)Session["ListaAtestados"];
                ViewBag.Tipos = new SelectList((List<TIPO_ATESTADO>)cache.CarregaCacheGeralListaTipoAtestado("TIPO_ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["TipoAtestadoAlterada"]).ToList(), "TIAT_CD_ID", "TIAT_NM_NOME");
                Session["TipoAtestadoAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 3;
                Session["ModoConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                // Carrega view
                objetoAtestado = new PACIENTE_ATESTADO();
                return View(objetoAtestado);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarAtestados(PACIENTE_ATESTADO item)
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
                item.PAAT_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAAT_NM_TITULO);
                item.PAAT_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAAT_TX_TEXTO);
                item.PAAT_AQ_ARQUIVO_HTML = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAAT_AQ_ARQUIVO_HTML);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                List<PACIENTE_ATESTADO> listaObj = new List<PACIENTE_ATESTADO>();
                Tuple<Int32, List<PACIENTE_ATESTADO>, Boolean> volta = baseApp.ExecuteFilterTupleAtestado(item.TIAT_CD_ID, item.PAAT_AQ_ARQUIVO_HTML, item.PAAT_DT_DATA, item.PAAT_DT_ENVIO, item.PAAT_NM_TITULO, item.PAAT_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaAtestados");
                }

                // Sucesso
                listaMasterAtestado = volta.Item2.ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMasterAtestado = listaMasterAtestado.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaAtestados"] = listaMasterAtestado;
                return RedirectToAction("MontarTelaAtestados");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroAtestados()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaAtestados"] = null;
                return RedirectToAction("MontarTelaAtestados");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioAtestados()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "AtestadoLista" + "_" + data + ".pdf";
                List<PACIENTE_ATESTADO> lista = (List<PACIENTE_ATESTADO>)Session["ListaAtestados"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Atestados", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 60f, 120f, 90f, 120f, 150f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Atestado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título do Atestado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_ATESTADO item in lista)
                {
                    if (item.PAAT_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAAT_DT_DATA.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PAAT_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.TIPO_ATESTADO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_ATESTADO.TIAT_NM_NOME, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PAAT_NM_TITULO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAAT__IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PAAT_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAAT_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaAtestados");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAtestadoCompleta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 3;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirAtestado");
        }

        public ActionResult EditarAtestadoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 3;
            Session["IdConsultaCria"] = 0;
            Session["VoltarPesquisa"] = 0;
            Int32 s = (Int32)Session["VoltarPesquisa"];
            return RedirectToAction("EditarAtestado", new { id = id });
        }

        public ActionResult VerAtestadoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 3;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("VerAtestado", new { id = id });
        }

        public ActionResult EditarPacienteAtestado(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 3;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacienteAtestado(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 3;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaExames()
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
                    if (usuario.PERFIL.PERF_IN_EXAME_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaExames"] == null)
                {
                    listaMasterExame = (List<PACIENTE_EXAMES>)cache.CarregaCacheGeralListaExames("EXAME", usuario.ASSI_CD_ID, (Int32)Session["ExamesAlterada"]).ToList();
                    Session["ExamesAlterada"] = 0;
                    listaMasterExame = listaMasterExame.Where(p => p.PAEX_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Session["ListaExames"] = listaMasterExame;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_EXAMES>)Session["ListaExames"];
                ViewBag.Tipos = new SelectList((List<TIPO_EXAME>)cache.CarregaCacheGeralListaTipoExame("TIPO_EXAME", usuario.ASSI_CD_ID, (Int32)Session["TipoExameAlterada"]).ToList(), "TIEX_CD_ID", "TIEX_NM_NOME");
                Session["TipoExameAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 4;
                Session["VoltarPesquisa"] = 0;
                Session["ModoConsulta"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6.pdf";

                // Carrega view
                objetoExame = new PACIENTE_EXAMES();
                return View(objetoExame);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarExames(PACIENTE_EXAMES item)
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
                item.PAEX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAEX_NM_NOME);
                item.PAEX_DS_COMENTARIOS = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAEX_DS_COMENTARIOS);
                item.PAEX_DS_DIAGNOSTICO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAEX_DS_DIAGNOSTICO);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;


                // Executa a operação
                List<PACIENTE_EXAMES> listaObj = new List<PACIENTE_EXAMES>();
                Tuple<Int32, List<PACIENTE_EXAMES>, Boolean> volta = baseApp.ExecuteFilterTupleExame(item.TIEX_CD_ID, item.PAEX_DS_DIAGNOSTICO, item.PAEX_DT_DATA, item.PAEX_DT_DUMMY, item.PAEX_NM_NOME, item.PAEX_DS_COMENTARIOS, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaExames");
                }

                // Sucesso
                listaMasterExame = volta.Item2.ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMasterExame = listaMasterExame.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaExames"] = listaMasterExame;
                return RedirectToAction("MontarTelaExames");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroExames()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaExames"] = null;
                return RedirectToAction("MontarTelaExames");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirExameCompleta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 4;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirExame");
        }

        public ActionResult EditarExameCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 4;
            Session["IdConsultaCria"] = 0;
            Session["VoltarPesquisa"] = 0;
            return RedirectToAction("EditarExame", new { id = id });
        }

        public ActionResult VerExameCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 4;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("VerAtestado", new { id = id });
        }

        public ActionResult EditarAnamneseCompletaFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarAnamnese", new { id = (Int32)Session["IdAnamneseConsulta"] });
        }

        public ActionResult VerAnamneseCompletaFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerAnamnese", new { id = (Int32)Session["IdAnamneseConsulta"] });
        }

        public ActionResult MarcarProximaConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 1;
            Session["ProximaConsulta"] = 1;
            return RedirectToAction("IncluirConsulta");
        }

        public ActionResult EditarFisicoCompletoFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarExameFisico", new { id = (Int32)Session["IdFisicoConsulta"] });
        }

        public ActionResult VerFisicoCompletoFormProcesso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerExameFisico", new { id = (Int32)Session["IdFisicoConsulta"] });
        }

        public ActionResult EditarPacienteExame(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 4;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacienteExame(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 4;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        public ActionResult GerarRelatorioExames()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ExameLista" + "_" + data + ".pdf";
                List<PACIENTE_EXAMES> lista = (List<PACIENTE_EXAMES>)Session["ListaExames"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Resultados de Exames", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 60f, 120f, 120f, 120f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Exame", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Anexos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_EXAMES item in lista)
                {
                    if (item.PAEX_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAEX_DT_DATA.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PAEX_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.TIPO_EXAME != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.TIPO_EXAME.TIEX_NM_NOME, meuFont))
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
                    if (item.PACIENTE_EXAME_ANEXO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_EXAME_ANEXO.Count().ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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

                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaExames");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaPrescricoes()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaPrescricoes"] == null)
                {
                    listaMasterPrescricao = (List<PACIENTE_PRESCRICAO>)cache.CarregaCacheGeralListaPrescricao("PRESCRICAO", usuario.ASSI_CD_ID, (Int32)Session["PrescricoesAlterada"]).ToList();
                    Session["PrescricoesAlterada"] = 0;
                    listaMasterPrescricao = listaMasterPrescricao.Where(p => p.PAPR_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Session["ListaPrescricoes"] = listaMasterPrescricao;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_PRESCRICAO>)Session["ListaPrescricoes"];
                ViewBag.Tipos = new SelectList((List<TIPO_CONTROLE>)cache.CarregaCacheGeralListaTipoControle("TIPO_CONTROLE", usuario.ASSI_CD_ID, (Int32)Session["TipoControleAlterada"]).ToList(), "TICO_CD_ID", "TICO_NM_NOME");
                Session["TipoControleAlterada"] = 0;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 5;
                Session["ModoConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9.pdf";

                // Carrega view
                objetoPrescricao = new PACIENTE_PRESCRICAO();
                return View(objetoPrescricao);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarPrescricoes(PACIENTE_PRESCRICAO item)
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
                item.PAPR_NM_POSOLOGIA = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAPR_NM_POSOLOGIA);
                item.PAPR_NM_REMEDIO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAPR_NM_REMEDIO);
                item.PAPR_NM_DOSAGEM = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAPR_NM_DOSAGEM);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                List<PACIENTE_PRESCRICAO> listaObj = new List<PACIENTE_PRESCRICAO>();
                Tuple<Int32, List<PACIENTE_PRESCRICAO>, Boolean> volta = baseApp.ExecuteFilterTuplePrescricao(item.TICO_CD_ID, item.PAPR_NM_POSOLOGIA, item.PAPR_DT_EMISSAO, item.PAPR_DT_ENVIO, item.PAPR_NM_REMEDIO, item.PAPR_NM_DOSAGEM, usuario, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaPrescricoes");
                }

                // Sucesso
                listaMasterPrescricao = volta.Item2.ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMasterPrescricao = listaMasterPrescricao.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaPrescricoes"] = listaMasterPrescricao;
                return RedirectToAction("MontarTelaPrescricoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroPrescricoes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaPrescricoes"] = null;
                return RedirectToAction("MontarTelaPrescricoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirPrescricaoConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 1;
            Session["ModoConsulta"] = 1;
            return RedirectToAction("IncluirPrescricaoNova");
        }

        public ActionResult IncluirPrescricaoCompleta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 5;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirPrescricaoNova");
        }

        public ActionResult EditarPrescricaoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 5;
            Session["IdConsultaCria"] = 0;
            Session["VoltarPesquisa"] = 0;
            return RedirectToAction("EditarPrescricaoNova", new { id = id });
        }

        public ActionResult VerPrescricaoCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 5;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("VerPrescricaoNova", new { id = id });
        }

        public ActionResult EditarPacientePrescricao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 5;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacientePrescricao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 5;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        public ActionResult GerarRelatorioPrescricoes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PrescricaoLista" + "_" + data + ".pdf";
                List<PACIENTE_PRESCRICAO> lista = (List<PACIENTE_PRESCRICAO>)Session["ListaPrescricoes"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Prescrições", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 70f, 80f, 120f, 80f, 60f, 60f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Controle", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Enviado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Envio", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num.Envios", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_PRESCRICAO item in lista)
                {
                    if (item.PAPR_DT_DATA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAPR_DT_DATA.ToShortDateString() + " " + item.PAPR_DT_DATA.ToShortTimeString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.TIPO_CONTROLE.TICO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PAPR_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PAPR_IN_ENVIADO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Sim", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PAPR_DT_ENVIO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PAPR_DT_ENVIO.Value.ToShortDateString(), meuFont))
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
                    if (item.PACIENTE_PRESCRICAO_ITEM != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_PRESCRICAO_ITEM.Count().ToString(), meuFont))
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
                    if (item.PACIENTE_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACIENTE_CONSULTA.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 8;
                return RedirectToAction("MontarTelaPrescricoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMedicamentos()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaMedicamentos"] == null)
                {
                    listaMasterItem = (List<PACIENTE_PRESCRICAO_ITEM>)cache.CarregaCacheGeralListaItemPrescricao("PRESCRICAO_ITEM", usuario.ASSI_CD_ID, (Int32)Session["ItensPrescricoesAlterada"]).ToList();
                    Session["ItensPrescricoesAlterada"] = 0;

                    listaMasterItem = listaMasterItem.Where(p => p.PAPI_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    List<String> remedios = listaMasterItem.Select(p => p.PAPI_NM_REMEDIO).Distinct().ToList();
                    List<MedicamentoViewModel> meds = new List<MedicamentoViewModel>();
                    foreach (String item in remedios)
                    {
                        Int32 num = listaMasterItem.Where(p => p.PAPI_NM_REMEDIO == item).ToList().Count;
                        MedicamentoViewModel med = new MedicamentoViewModel();
                        med.Nome = item;
                        med.Prescricoes = num;
                            meds.Add(med);
                    }
                    listaMasterRemedio = meds;
                    Session["ListaMedicamentos"] = meds;
                }

                // Monta demais listas
                ViewBag.Listas = (List<MedicamentoViewModel>)Session["ListaMedicamentos"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/9/Ajuda9_5.pdf";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 7;

                // Carrega view
                objetoRemedio = new MedicamentoViewModel();
                return View(objetoRemedio);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMedicamentos(MedicamentoViewModel item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.Nome = CrossCutting.UtilitariosGeral.CleanStringGeral(item.Nome);

                // Processa filtro
                List<MedicamentoViewModel> meds = (List<MedicamentoViewModel>)Session["ListaMedicamentos"];
                meds = meds.Where(p => p.Nome.ToUpper().Contains(item.Nome.ToUpper())).ToList();

                // Verifica retorno
                if (meds.Count == 0)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("VerMedicamentos");
                }

                // Sucesso
                listaMasterRemedio = meds;
                Session["ListaMedicamentos"] = meds;
                return RedirectToAction("VerMedicamentos");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMedicamentos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMedicamentos"] = null;
                return RedirectToAction("VerMedicamentos");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMedicamentosPaciente(String nome)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                if (Session["ListaMedicamentosPaciente"] == null)
                {
                    listaMasterItem = (List<PACIENTE_PRESCRICAO_ITEM>)cache.CarregaCacheGeralListaItemPrescricao("PRESCRICAO_ITEM", usuario.ASSI_CD_ID, (Int32)Session["ItensPrescricoesAlterada"]).ToList();
                    Session["ItensPrescricoesAlterada"] = 0;

                    listaMasterItem = listaMasterItem.Where(p => p.PAPI_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    listaMasterItem = listaMasterItem.Where(p => p.PAPI_NM_REMEDIO.Contains(nome)).OrderByDescending(p => p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO).ToList();
                    Session["ListaMedicamentosPaciente"] = listaMasterItem;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_PRESCRICAO_ITEM>)Session["ListaMedicamentosPaciente"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 7;
                Session["NomeRemedio"] = nome;

                // Carrega view
                objetoItem = new PACIENTE_PRESCRICAO_ITEM();
                objetoItem.PAPI_NM_REMEDIO = nome;
                return View(objetoItem);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMedicamentosPaciente(PACIENTE_PRESCRICAO_ITEM item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.PAPI_NM_GENERICO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAPI_NM_GENERICO);

                // Montagem
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);
                PACIENTE_PRESCRICAO pres = baseApp.GetPrescricaoById(item.PAPR_CD_ID);
                item.PACIENTE = pac;
                item.PACIENTE_PRESCRICAO = pres;

                // Processa filtro
                List<PACIENTE_PRESCRICAO_ITEM> itens = (List<PACIENTE_PRESCRICAO_ITEM>)Session["ListaMedicamentosPaciente"];
                Int32 tot = itens.Count;
                List<PACIENTE_PRESCRICAO_ITEM> saida = new List<PACIENTE_PRESCRICAO_ITEM>();
                if (item.PAPI_NM_GENERICO != null)
                {
                    itens = itens.Where(p => p.PACIENTE.PACI_NM_NOME.ToUpper().Contains(item.PAPI_NM_GENERICO.ToUpper())).ToList();
                }
                if (item.PAPI_DT_DATA_1 != null & item.PAPI_DT_DATA_2 == null)
                {
                    itens = itens.Where(p => p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO.Value.Date > item.PAPI_DT_DATA_1.Value.Date).ToList();
                }
                if (item.PAPI_DT_DATA_1 == null & item.PAPI_DT_DATA_2 != null)
                {
                    itens = itens.Where(p => p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO.Value.Date < item.PAPI_DT_DATA_2.Value.Date).ToList();
                }
                if (item.PAPI_DT_DATA_1 != null & item.PAPI_DT_DATA_2 != null)
                {
                    itens = itens.Where(p => p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO.Value.Date > item.PAPI_DT_DATA_1.Value.Date & p.PACIENTE_PRESCRICAO.PAPR_DT_EMISSAO.Value.Date < item.PAPI_DT_DATA_2.Value.Date).ToList();
                }

                // Verifica retorno
                if (itens.Count == tot)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("VerMedicamentosPaciente");
                }

                // Sucesso
                listaMasterItem = itens;
                Session["ListaMedicamentosPaciente"] = itens;
                return RedirectToAction("VerMedicamentosPacienteForm");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMedicamentosPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMedicamentosPaciente"] = null;
                return RedirectToAction("VerMedicamentosPacienteForm");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult EditarPacienteMedicamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 8;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacienteMedicamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 8;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        public ActionResult EditarPrescricaoMedicamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 8;
            return RedirectToAction("EditarPrescricaoNova", new { id = id });
        }

        public ActionResult VerPrescricaoMedicamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 8;
            return RedirectToAction("VerPrescricaoNova", new { id = id });
        }

        public ActionResult VerMedicamentosPacienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerMedicamentosPaciente", new { nome = (String)Session["NomeRemedio"] });
        }

        [HttpGet]
        public ActionResult MontarTelaConsultas()
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
                    if (usuario.PERFIL.PERF_IN_PACIENTE_CONSULTA_ACESSO == 0)
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

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0528", CultureInfo.CurrentCulture));
                    }
                }

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                Int32 temHoje = 0;
                List<PACIENTE_CONSULTA> listaHoje = new List<PACIENTE_CONSULTA>();
                if (Session["ListaConsultasGeral"] == null)
                {
                    listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                    Session["ConsultasAlterada"] = 0;

                    listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    listaHoje = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date == DateTime.Today.Date).ToList();
                    if (listaHoje.Count > 0)
                    {
                        listaMasterConsulta = listaHoje;
                        temHoje = 1;
                    }                 
                    Session["ListaConsultasGeral"] = listaMasterConsulta;
                    Session["TemHoje"] = temHoje;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_CONSULTA>)Session["ListaConsultasGeral"];
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                var conc = new List<SelectListItem>();
                conc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                conc.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Encerrada = new SelectList(conc, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 6;
                Session["VoltarConsulta"] = 4;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5.pdf";

                // Carrega view
                objetoConsulta = new PACIENTE_CONSULTA();
                return View(objetoConsulta);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarConsultas(PACIENTE_CONSULTA item)
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
                item.PACO_TX_RESUMO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACO_TX_RESUMO);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                List<PACIENTE_CONSULTA> listaObj = new List<PACIENTE_CONSULTA>();
                Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> volta = baseApp.ExecuteFilterTupleConsulta(item.PACO_IN_TIPO, item.PACO_TX_RESUMO, item.PACO_DT_DUMMY, item.PACO_DT_PROXIMA, item.PACO_IN_ENCERRADA, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaConsultas");
                }

                // Sucesso
                listaMasterConsulta = volta.Item2.ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMasterConsulta = listaMasterConsulta.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroConsultas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderBy(p => p.PACO_DT_CONSULTA).ToList();
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerConsultasHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                List<PACIENTE_CONSULTA> listaHoje = new List<PACIENTE_CONSULTA>();
                listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date == DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterConsulta = listaHoje;
                }
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerConsultasFuturas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                List<PACIENTE_CONSULTA> listaHoje = new List<PACIENTE_CONSULTA>();
                listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date > DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterConsulta = listaHoje;
                }
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerConsultasAnteriores()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                List<PACIENTE_CONSULTA> listaHoje = new List<PACIENTE_CONSULTA>();
                listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date < DateTime.Today.Date).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterConsulta = listaHoje;
                }
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerConsultasMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                List<PACIENTE_CONSULTA> listaHoje = new List<PACIENTE_CONSULTA>();
                listaMasterConsulta = listaMasterConsulta.Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date.Month == DateTime.Today.Date.Month & p.PACO_DT_CONSULTA.Date.Year == DateTime.Today.Date.Year ).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterConsulta = listaHoje;
                }
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                Session["ListaConsultas"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerExamesMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterExame = (List<PACIENTE_EXAMES>)cache.CarregaCacheGeralListaExames("EXAME", usuario.ASSI_CD_ID, (Int32)Session["ExamesAlterada"]).ToList();
                Session["ExamesAlterada"] = 0;

                List<PACIENTE_EXAMES> listaHoje = new List<PACIENTE_EXAMES>();
                listaMasterExame = listaMasterExame.Where(p => p.PAEX_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterExame.Where(p => p.PAEX_DT_DATA.Value.Date.Month == DateTime.Today.Date.Month & p.PAEX_DT_DATA.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterExame = listaHoje;
                }
                Session["ListaExamesGeral"] = listaMasterExame;
                Session["ListaExames"] = listaMasterExame;
                return RedirectToAction("MontarTelaExames");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerPrescricoesMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterPrescricao = (List<PACIENTE_PRESCRICAO>)cache.CarregaCacheGeralListaPrescricao("PRESCRICAO", usuario.ASSI_CD_ID, (Int32)Session["PrescricoesAlterada"]).ToList();
                Session["PrescricoesAlterada"] = 0;

                List<PACIENTE_PRESCRICAO> listaHoje = new List<PACIENTE_PRESCRICAO>();
                listaMasterPrescricao = listaMasterPrescricao.Where(p => p.PAPR_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterPrescricao.Where(p => p.PAPR_DT_EMISSAO.Value.Date.Month == DateTime.Today.Date.Month & p.PAPR_DT_EMISSAO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterPrescricao = listaHoje;
                }
                Session["ListaPrescricoesGeral"] = listaMasterPrescricao;
                Session["ListaPrescricoes"] = listaMasterPrescricao;
                return RedirectToAction("MontarTelaPrescricoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerAtestadosMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterAtestado = (List<PACIENTE_ATESTADO>)cache.CarregaCacheGeralListaAtestados("ATESTADO", usuario.ASSI_CD_ID, (Int32)Session["AtestadosAlterada"]).ToList();
                Session["AtestadosAlterada"] = 0;

                List<PACIENTE_ATESTADO> listaHoje = new List<PACIENTE_ATESTADO>();
                listaMasterAtestado = listaMasterAtestado.Where(p => p.PAAT_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterAtestado.Where(p => p.PAAT_DT_DATA.Value.Date.Month == DateTime.Today.Date.Month & p.PAAT_DT_DATA.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterAtestado = listaHoje;
                }
                Session["ListaAtestadosGeral"] = listaMasterAtestado;
                Session["ListaAtestados"] = listaMasterAtestado;
                return RedirectToAction("MontarTelaAtestados");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerSolicitacoesMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterSolicitacao = (List<PACIENTE_SOLICITACAO>)cache.CarregaCacheGeralListaSolicitacao("SOLICITACAO", usuario.ASSI_CD_ID, (Int32)Session["SolicitacoesAlterada"]).ToList();
                Session["SolicitacoesAlterada"] = 0;

                List<PACIENTE_SOLICITACAO> listaHoje = new List<PACIENTE_SOLICITACAO>();
                listaMasterSolicitacao = listaMasterSolicitacao.Where(p => p.PASO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaHoje = listaMasterSolicitacao.Where(p => p.PASO_DT_EMISSAO.Value.Date.Month == DateTime.Today.Date.Month & p.PASO_DT_EMISSAO.Value.Date.Year == DateTime.Today.Date.Year).ToList();
                if (listaHoje.Count == 0)
                {
                    Session["MensPaciente"] = 2;
                }
                else
                {
                    listaMasterSolicitacao = listaHoje;
                }
                Session["ListaSolicitacoesGeral"] = listaMasterSolicitacao;
                Session["ListaSolicitacoes"] = listaMasterSolicitacao;
                return RedirectToAction("MontarTelaSolicitacoes");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirConsultaCompleta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 6;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("IncluirConsulta");
        }

        public ActionResult EditarConsultaCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 6;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("EditarConsulta", new { id = id });
        }

        public ActionResult VerConsultaCompleta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 6;
            Session["IdConsultaCria"] = 0;
            return RedirectToAction("VerConsulta", new { id = id });
        }

        public ActionResult EditarPacienteConsulta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 6;
            return RedirectToAction("EditarPaciente", new { id = id });
        }

        public ActionResult VerPacienteConsulta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["TipoSolicitacao"] = 6;
            return RedirectToAction("VerPaciente", new { id = id });
        }

        public ActionResult GerarRelatorioConsultas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Prepara geração
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ConsultaLista" + "_" + data + ".pdf";
                List<PACIENTE_CONSULTA> lista = (List<PACIENTE_CONSULTA>)Session["ListaConsultasGeral"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Consultas", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 60f, 60f, 60f, 120f, 60f, 60f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Hora de Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Hora de Término", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo de Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Realizada", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PACIENTE_CONSULTA item in lista)
                {
                    if (item.PACO_DT_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PACO_DT_CONSULTA.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PACO_HR_INICIO.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PACO_HR_FINAL.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.PACO_IN_TIPO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Presencial", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Remota", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (item.PACO_IN_ENCERRADA == 1)
                    {
                        cell = new PdfPCell(new Paragraph("SIm", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaConsultas");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerCalendarioConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                PACIENTE_CONSULTA item = new PACIENTE_CONSULTA();
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;

                listaMasterCalendario = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Month == DateTime.Today.Date.Month & p.PACO_DT_CONSULTA.Year == DateTime.Today.Date.Year).ToList();
                Session["ListaAgenda"] = listaMasterCalendario;
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                Session["NaoFezNada"] = 0;
                Session["TipoAgenda"] = 1;
                return View(vm);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public JsonResult GetEventosCalendario()
        {
            try
            {
                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<Hashtable> listaCalendario = new List<Hashtable>();

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                listaMasterConsulta = (List<PACIENTE_CONSULTA>)cache.CarregaCacheGeralListaConsultas("CONSULTA", usuario.ASSI_CD_ID, (Int32)Session["ConsultasAlterada"]).ToList();
                Session["ConsultasAlterada"] = 0;
                listaMasterCalendario = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Month == DateTime.Today.Date.Month & p.PACO_DT_CONSULTA.Month == DateTime.Today.Date.Month).ToList();
                Session["ListaAgenda"] = listaMasterCalendario;

                foreach (var item in listaMasterCalendario)
                {
                    var hash = new Hashtable();

                    hash.Add("id", item.PACO_CD_ID);
                    hash.Add("title", item.PACIENTE.PACI_NM_NOME);
                    hash.Add("start", (item.PACO_DT_CONSULTA + item.PACO_HR_INICIO).ToString());
                    hash.Add("description", (new DateTime() + item.PACO_HR_INICIO).ToString());

                    listaCalendario.Add(hash);
                }
                return Json(listaCalendario);
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
        public JsonResult GetDetalhesEvento(Int32 id)
        {
            try
            {
                var evento = baseApp.GetConsultaById(id);

                var hash = new Hashtable();
                hash.Add("data", evento.PACO_DT_CONSULTA.ToShortDateString());
                hash.Add("hora", evento.PACO_HR_INICIO.ToString());
                hash.Add("final", evento.PACO_HR_FINAL.ToString());
                hash.Add("tipo", evento.PACO_IN_TIPO == 1 ? "Presencial" : "Remota");
                hash.Add("nome", evento.PACIENTE.PACI_NM_NOME);
                hash.Add("encerrada", evento.PACO_IN_ENCERRADA == 1 ? "Encerrada" : "Aberta");
                return Json(hash);
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

        public ActionResult ProcessaRelatorioPaciente(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarRelatorioLista");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("MontarTelaPacienteAtraso");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("MontarTelaPacienteAusente");
            }
            return RedirectToAction("MontarTelaCentralPaciente");
        }

        [HttpGet]
        public ActionResult MontarTelaPacienteHistorico()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PACIENTE == 0)
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

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Carrega listas
                PACIENTE paciente = baseApp.GetItemById((Int32)Session["IdPaciente"]);
                if (Session["ListaHistorico"] == null)
                {
                    listaMasterHistorico = paciente.PACIENTE_HISTORICO.ToList();
                    Session["ListaHistorico"] = listaMasterHistorico;
                }

                // Monta demais listas
                ViewBag.Listas = (List<PACIENTE_HISTORICO>)Session["ListaHistorico"];
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Paciente", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Contato de Paciente", Value = "2" });
                tipo.Add(new SelectListItem() { Text = "Prescrição", Value = "3" });
                tipo.Add(new SelectListItem() { Text = "Item de Prescrição", Value = "4" });
                tipo.Add(new SelectListItem() { Text = "Atestado", Value = "5" });
                tipo.Add(new SelectListItem() { Text = "Solicitação de Exame", Value = "6" });
                tipo.Add(new SelectListItem() { Text = "Resultado de Exame", Value = "7" });
                tipo.Add(new SelectListItem() { Text = "Anamese", Value = "8" });
                tipo.Add(new SelectListItem() { Text = "Exame Físico", Value = "9" });
                tipo.Add(new SelectListItem() { Text = "Consulta", Value = "10" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_6.pdf";

                // Carrega view
                objetoHistorico = new PACIENTE_HISTORICO();
                objetoHistorico.PACIENTE = paciente;
                return View(objetoHistorico);
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarHistorico(PACIENTE_HISTORICO item)
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
                item.PAHI_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAHI_NM_OPERACAO);
                item.PAHI_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PAHI_DS_DESCRICAO);

                // Instancia cache
                CacheSingletonAcesso cacheAc = new CacheSingletonAcesso(usuApp, confApp);
                CacheSingletonPaciente cache = new CacheSingletonPaciente(baseApp, usuApp);
                CacheSingletonTabelas cacheTab = new CacheSingletonTabelas(tpApp, tpaApp);
                CacheSingletonMensagem cacheMens = new CacheSingletonMensagem(gruApp, meApp);

                // Carega configuracao
                CONFIGURACAO conf = (CONFIGURACAO)cacheAc.CarregaCacheGeralConfiguracao("CONFIGURACAO", usuario.ASSI_CD_ID, (Int32)Session["ConfiguracaoAlterada"]);
                Session["ConfiguracaoAlterada"] = 0;

                // Executa a operação
                List<PACIENTE_HISTORICO> listaObj = new List<PACIENTE_HISTORICO>();
                Tuple<Int32, List<PACIENTE_HISTORICO>, Boolean> volta = baseApp.ExecuteFilterTupleHistorico(item.PAHI_IN_TIPO, item.PAHI_NM_OPERACAO, item.PAHI_DT_DUMMY, item.PAHI_DT_DUMMY_1, item.PAHI_DS_DESCRICAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaPacienteHistorico");
                }

                // Sucesso
                listaMasterHistorico = volta.Item2.ToList();
                Session["ListaHistorico"] = listaMasterHistorico;
                return RedirectToAction("MontarTelaPacienteHistorico");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroHistorico()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaHistorico"] = null;
                return RedirectToAction("MontarTelaPacienteHistorico");
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
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
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

        public JsonResult GetDadosPacienteAlteradoMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPacienteAlteradoMes"];
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

        public JsonResult GetDadosConsultaDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaConsultaData"];
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

        public List<TIPO_PACIENTE> CarregaCatPaciente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_PACIENTE> conf = new List<TIPO_PACIENTE>();
                if (Session["TipoPacientes"] == null)
                {
                    conf = baseApp.GetAllTipos(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoPacienteAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipos(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_PACIENTE>)Session["TipoPacientes"];
                    }
                }
                Session["TipoPacientes"] = conf;
                Session["TipoPacienteAlterada"] = 0;
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

                public List<COR> CarregaCor()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<COR> conf = new List<COR>();
                if (Session["Cores"] == null)
                {
                    conf = baseApp.GetAllCor();
                }
                else
                {
                    conf = (List<COR>)Session["Cores"];
                }
                Session["Cores"] = conf;
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

        public List<ESTADO_CIVIL> CarregaEstadoCivil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ESTADO_CIVIL> conf = new List<ESTADO_CIVIL>();
                if (Session["EstadosCivil"] == null)
                {
                    conf = baseApp.GetAllEstadoCivil();
                }
                else
                {
                    conf = (List<ESTADO_CIVIL>)Session["EstadosCivil"];
                }
                Session["EstadosCivil"] = conf;
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

        public List<CONVENIO> CarregaConvenio()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CONVENIO> conf = new List<CONVENIO>();
                if (Session["Convenios"] == null)
                {
                    conf = baseApp.GetAllConvenio(idAss);
                }
                else
                {
                    if ((Int32)Session["ConvenioAlterada"] == 1)
                    {
                        conf = baseApp.GetAllConvenio(idAss);
                    }
                    else
                    {
                        conf = (List<CONVENIO>)Session["Convenios"];
                    }
                }
                Session["Convenios"] = conf;
                Session["ConvenioAlterada"] = 0;
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

        public List<GRAU_INSTRUCAO> CarregaGrauInstrucao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<GRAU_INSTRUCAO> conf = new List<GRAU_INSTRUCAO>();
                if (Session["Graus"] == null)
                {
                    conf = baseApp.GetAllGrau();
                }
                else
                {
                    conf = (List<GRAU_INSTRUCAO>)Session["Graus"];
                }
                Session["Graus"] = conf;
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

        public List<GRAU_PARENTESCO> CarregaGrauParente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<GRAU_PARENTESCO> conf = new List<GRAU_PARENTESCO>();
                if (Session["GrausParente"] == null)
                {
                    conf = baseApp.GetAllGrauParentesco();
                }
                else
                {
                    conf = (List<GRAU_PARENTESCO>)Session["GrausParente"];
                }
                Session["GrausParente"] = conf;
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

        public List<SEXO> CarregaSexo()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<SEXO> conf = new List<SEXO>();
                if (Session["Sexos"] == null)
                {
                    conf = baseApp.GetAllSexo();
                }
                else
                {
                    conf = (List<SEXO>)Session["Sexos"];
                }
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

    }
}