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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.CodeDom;
#pragma warning disable CS0105 // Usando diretiva exibida anteriormente neste namespace
using CrossCutting;
#pragma warning restore CS0105 // Usando diretiva exibida anteriormente neste namespace
using nClam;
using iTextSharp.tool.xml;

namespace GEDSys_Presentation.Controllers
{
    public class AreaPacienteController : Controller
    {
        private readonly IPacienteAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IAssinanteAppService assApp;
        private readonly IConfiguracaoCalendarioAppService calApp;
        private readonly IConfiguracaoAnamneseAppService anaApp;
        private readonly ILogAppService logApp;
        private readonly ILocacaoAppService locaApp;
        private readonly IAreaPacienteAppService areaApp;

        private String msg;
        private Exception exception;
        private PACIENTE objeto = new PACIENTE();
        private PACIENTE objetoAntes = new PACIENTE();
        private List<PACIENTE> listaMaster = new List<PACIENTE>();
        private List<PACIENTE_CONSULTA> listaConsulta = new List<PACIENTE_CONSULTA>();
        private List<PACIENTE_CONSULTA> listaMasterConsulta = new List<PACIENTE_CONSULTA>();
        private List<PACIENTE_CONSULTA> listaMasterConsultaFutura = new List<PACIENTE_CONSULTA>();
        private List<PACIENTE_ATESTADO> listaMasterAtestado = new List<PACIENTE_ATESTADO>();
        private List<PACIENTE_EXAMES> listaMasterExame = new List<PACIENTE_EXAMES>();
        private List<PACIENTE_SOLICITACAO> listaMasterSolicitacao = new List<PACIENTE_SOLICITACAO>();
        private List<PACIENTE_PRESCRICAO> listaMasterPrescricao = new List<PACIENTE_PRESCRICAO>();
        private List<LOCACAO> listaMasterLocacao = new List<LOCACAO>();

        public AreaPacienteController(IPacienteAppService baseApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IConfiguracaoCalendarioAppService calApps, IConfiguracaoAnamneseAppService anaApps, ILogAppService logApps, ILocacaoAppService locaApps, IAreaPacienteAppService areaApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            usuApp = usuApps;
            aceApp = aceApps;
            assApp = assApps;
            calApp = calApps;
            anaApp = anaApps;
            logApp = logApps;
            locaApp = locaApps;
            areaApp = areaApps;
        }

        [HttpGet]
        public ActionResult LoginAreaPaciente()
        {
            // Mensagens
            if (Session["MensagemLogin"] != null)
            {
                if ((Int32)Session["MensagemLogin"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0211", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0322", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 16)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 17)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 18)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 19)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 20)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 24)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 55)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0330", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 65)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0671", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0332", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 90)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0341", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 91)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0342", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 92)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0344", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 93)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0345", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 94)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0343", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 25)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0408", CultureInfo.CurrentCulture) + " CRMSys");
                }
                if ((Int32)Session["MensagemLogin"] == 66)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0442", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 94)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0624", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 95)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0625", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 97)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0626", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0636", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 100)
                {
                    ModelState.AddModelError("", "ERRO - " + (String)Session["MensagemErro"]);
                }
            }

            // Exibe tela
            Session.Clear();
            Session["MensSenha"] = null;
            Session["MensagemLogin"] = 0;
            Session["UserCredentials"] = null;
            Session["Close"] = false;
            PACIENTE item = new PACIENTE();
            PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);
            vm.PACI_IN_HUMANO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAreaPaciente(PacienteViewModel vm)
        {
            try
            {
                // Inicialização
                PACIENTE paciente;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                Session["MensSenha"] = 0;
                Session["MensagemLogin"] = 0;
                String senha = vm.PACI_NM_SENHA;

                // Verifica humano
                if (vm.PACI_IN_HUMANO == null || vm.PACI_IN_HUMANO == 0)
                {
                    Session["MensagemLogin"] = 66;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0442", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }

                // Sanitização
                vm.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CPF);
                vm.PACI_NM_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(vm.PACI_NM_SENHA);

                // Valida credenciais
                Int32 volta = baseApp.ValidateLoginArea(vm.PACI_NR_CPF, vm.PACI_NM_SENHA, out paciente);
                Session["UserCredentials"] = paciente;
                if (volta == 1)
                {
                    Session["MensagemLogin"] = 11;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                if (volta == 2)
                {
                    Session["MensagemLogin"] = 12;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0658", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                if (volta == 3)
                {
                    Session["MensagemLogin"] = 13;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0676", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                if (volta == 7)
                {
                    Session["MensagemLogin"] = 17;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                if (volta == 9)
                {
                    Session["MensagemLogin"] = 18;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                if (volta == 10)
                {
                    Session["MensagemLogin"] = 19;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0677", CultureInfo.CurrentCulture));
                    vm.PACI_NR_CPF = String.Empty;
                    vm.PACI_NM_SENHA = String.Empty;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = paciente;
                Session["Paciente"] = paciente;
                Session["IdAssinante"] = paciente.ASSI_CD_ID;

                // Configuraçoes de escopo de dados
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (conf ==  null)
                {
                    Session["MensagemLogin"] = 100;
                    return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
                }
                Session["MensagemFabricante"] = conf.CONF_IN_MENSAGEM_FABRICANTE;

                // Recupera ultimo login
                PACIENTE_LOGIN ultimoLogin = baseApp.GetAllLogin(paciente.ASSI_CD_ID).Where(p => p.PALO_IN_SISTEMA == 6 & p.PACI_CD_ID == paciente.PACI__CD_ID).OrderByDescending(p => p.PALO_DT_LOGIN).FirstOrDefault();
                Session["UltimoLogin"] = "Primeiro Acesso";
                if (ultimoLogin != null)
                {
                    Session["UltimoLogin"] = "Último Acesso: " + ultimoLogin.PALO_DT_LOGIN.ToString();
                }

                // Atualiza view
                String frase = String.Empty;
                String nome = paciente.PACI_NM_NOME;
                if (DateTime.Now.Hour <= 12)
                {
                    frase = "Bom dia, " + nome;
                }
                else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
                {
                    frase = "Boa tarde, " + nome;
                }
                else
                {
                    frase = "Boa noite, " + nome;
                }

                ViewBag.Greeting = frase;
                ViewBag.Nome = paciente.PACI_NM_NOME;
                ViewBag.Foto = paciente.PACI_AQ_FOTO;

                // Trata Nome
                String nomeMax = String.Empty;
                if (paciente.PACI_NM_NOME.Contains(" "))
                {
                    nomeMax = paciente.PACI_NM_NOME.Substring(0, paciente.PACI_NM_NOME.IndexOf(" "));
                }
                else
                {
                    nomeMax = paciente.PACI_NM_NOME;
                }

                // Prepara ambiente
                String[] partesDoNome = paciente.PACI_NM_NOME.Split(' ');
                Session["NomeMax"] = nomeMax;
                Session["Greeting"] = frase;
                //Session["Nome"] = paciente.PACI_NM_NOME.Length <= 15 ? paciente.PACI_NM_NOME : paciente.PACI_NM_NOME.Substring(0, 14);
                Session["Nome"] = partesDoNome[0];
                Session["Foto"] = paciente.PACI_AQ_FOTO;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["IdAssinante"] = paciente.ASSI_CD_ID;
                Session["IdUsuario"] = paciente.USUA_CD_ID;
                Session["ExtensoesPossiveis"] = ".PDF|.TXT|.JPG|.JPEG|.PNG|.GIF|.MP4|.MKV|.XLS|.XLSX|.PPT|.PPTX|.DOC|.DOCX|.ODS|.ODT|.ODP|.ODG";
                Session["ExtensoesPossiveisFicha"] = ".PDF|.JPG|.JPEG|.PNG|.GIF";
                Session["IdMarcacao"] = paciente.USUA_CD_ID;
                Session["VoltaSolicitacao"] = 0;
                Session["Ativa"] = "1";
                Session["Close"] = false;
                Session["MensSenha"] = null;
                Session["ConfAlterada"] = 0;
                Session["Configuracao"] = null;
                Session["MensagemLogin"] = null;
                Session["MensagemErro"] = null;
                Session["Excecao"] = null;
                Session["IdLoginSessao"] = null;
                Session["MensEnvioLogin"] = null;
                Session["ListaConsultasGeral"] = null;
                Session["ListaAtestados"] = null;
                Session["ListaExames"] = null;
                Session["ListaSolicitacao"] = null;
                Session["ListaPrescricao"] = null;
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 0;
                Session["NivelPaciente"] = 1;
                Session["UF"] = null;
                Session["Assinantes"] = null;
                Session["Usuarios"] = null;
                Session["idAss"] = null;
                Session["UsuarioProf"] = null;
                Session["Consultas"] = null;

                // Grava login no historico
                PACIENTE_LOGIN login = new PACIENTE_LOGIN();
                login.ASSI_CD_ID = paciente.ASSI_CD_ID;
                login.PALO_DT_LOGIN = DateTime.Now;
                login.PALO_IN_SISTEMA = 6;
                login.PALO_IN_ATIVO = 1;
                login.PACI_CD_ID = paciente.PACI__CD_ID;
                Int32 voltaLog = baseApp.ValidateCreateLogin(login);
                Session["IdLoginSessao"] = login.PALO_CD_ID;

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

                // Route
                return RedirectToAction("MontarTelaAreaPaciente", "AreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
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
                return null;
            }
        }

        [HttpGet]
        public ActionResult GerarNovaSenhaPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Mensagens
                if (Session["MensSenha"] != null)
                {
                    // Mensagens
                    if ((Int32)Session["MensSenha"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0658", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0676", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0653", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0658", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensagemLogin"] == 100)
                    {
                        ModelState.AddModelError("", "ERRO - " + (String)Session["MensagemErro"]);
                    }
                }

                // Abre tela
                PACIENTE item = new PACIENTE();
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GerarNovaSenhaPaciente(PacienteViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                vm.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CPF);

                // Processa
                Session["UserCredentials"] = null;
                PACIENTE item = Mapper.Map<PacienteViewModel, PACIENTE>(vm);
                Int32 volta = await baseApp.GerarNovaSenha(item);
                if (volta != 0)
                {
                    Session["MensSenha"] = volta;
                    return RedirectToAction("GerarNovaSenhaPaciente", "AreaPaciente");
                }
                Session["MensagemLogin"] = 55;
                return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult LogoutAreaPaciente()
        {
            // Grava flags de saida
            Session["IdLoginSessao"] = null;
            Session["MensagemLogin"] = 99;
            Session["UserCredentials"] = null;
            Session["MensEnvioLogin"] = 0;
            Session.Clear();
            return RedirectToAction("LoginAreaPaciente", "AreaPaciente");
        }

        public ActionResult SairWebDoctorAreaPaciente()
        {
            // Grava flags de saida
            Session["IdLoginSessao"] = null;
            Session["MensagemLogin"] = 99;
            Session["UserCredentials"] = null;
            Session["MensEnvioLogin"] = 0;
            Session.Clear();
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
            //return RedirectToAction("Login", "ControleAcesso");
            return RedirectToAction("CarregarLandingPage", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult TrocarSenhaAreaPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Verifica se tem usuario logado
                PACIENTE usuario = new PACIENTE();
                if ((PACIENTE)Session["UserCredentials"] != null)
                {
                    usuario = (PACIENTE)Session["UserCredentials"];
                }
                else
                {
                    return RedirectToAction("Logout", "AreaPaciente");
                }

                // Reseta senhas
                usuario.PACI_NM_NOVA_SENHA = null;
                usuario.PACI_NM_SENHA_CONFIRMA = null;
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(usuario);
                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaAreaPaciente(PacienteViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Checa credenciais e atualiza acessos
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                PACIENTE item = Mapper.Map<PacienteViewModel, PACIENTE>(vm);
                Int32 volta = await baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0658", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0676", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0677", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0665", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0666", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0667", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0660", CultureInfo.CurrentCulture) + ". Tamanho mínimo: " + conf.CONF_NR_TAMANHO_SENHA.ToString() + " caracteres.";
                    ModelState.AddModelError("", frase);
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0668", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaAreaPaciente()
        {
            try
            {
                // Verifica se tem usuario logado
                PACIENTE paciente = new PACIENTE();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((PACIENTE)Session["UserCredentials"] != null)
                {
                    paciente = (PACIENTE)Session["UserCredentials"];
                }
                else
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                String cpf = paciente.PACI_NR_CPF;
                Session["PacienteCPF"] = cpf;

                // Trata mensagens
                if (Session["MensArea"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensArea"] == 61)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                    if ((Int32)Session["MensArea"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Consultas
                if (Session["ListaConsultasGeral"] == null)
                {
                    listaConsulta = baseApp.GetConsultasByCPF(cpf);
                    listaMasterConsulta = listaConsulta.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date).ToList();
                    listaMasterConsulta = listaMasterConsulta.OrderBy(p => p.PACO_DT_CONSULTA).ThenBy(p => p.PACO_HR_INICIO).ToList();
                    listaMasterConsultaFutura = listaConsulta.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date).ToList();
                    Session["ListaConsultasGeral"] = listaMasterConsulta;
                    Session["ListaConsultas"] = listaConsulta;
                }

                // Montar demais listas de consultas
                listaMasterConsulta = (List<PACIENTE_CONSULTA>)Session["ListaConsultasGeral"];
                listaMasterConsulta = listaMasterConsulta.OrderBy(p => p.PACO_DT_CONSULTA).ThenBy(p => p.PACO_HR_INICIO).ToList();
                listaMasterConsultaFutura = listaMasterConsulta.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date).ToList();

                ViewBag.Consultas = listaMasterConsulta;
                ViewBag.NumConsultas = listaMasterConsulta.Count();
                ViewBag.NumConsultasFuturas = listaMasterConsultaFutura.Count();
                Session["ListaConsultasGeral"] = listaMasterConsulta;

                // Montar listas de atestados
                listaMasterAtestado = baseApp.GetAtestadosByCPF(cpf);
                ViewBag.NumAtestados= listaMasterAtestado.Count();
                Session["ListaAtestados"] = listaMasterAtestado;

                // Montar listas de exames
                listaMasterExame = baseApp.GetExamesByCPF(cpf);
                ViewBag.NumExames = listaMasterExame.Count();
                Session["ListaExames"] = listaMasterExame;

                // Montar listas de solicitacoes
                listaMasterSolicitacao = baseApp.GetSolicitacaoByCPF(cpf);
                ViewBag.NumSolicitacoes = listaMasterSolicitacao.Count();
                Session["ListaSolicitacao"] = listaMasterSolicitacao;

                // Montar listas de prescricoes
                listaMasterPrescricao = baseApp.GetPrescricaoByCPF(cpf);
                ViewBag.NumPrescricoes = listaMasterPrescricao.Count();
                Session["ListaPrescricao"] = listaMasterPrescricao;

                // Montar listas de locações
                listaMasterLocacao = locaApp.GetLocacaoByCPF(cpf);
                ViewBag.NumLocacoes = listaMasterLocacao.Count();
                Session["ListaLocacao"] = listaMasterLocacao;

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/1/Ajuda1.pdf";

                // Carrega view
                return View(paciente);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_CONSULTA> listaConsultas = (List<PACIENTE_CONSULTA>)Session["ListaConsultas"];
                listaMasterConsulta = listaConsultas.Where(p => p.PACO_DT_CONSULTA.Date == DateTime.Today.Date).ToList();
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirFuturas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_CONSULTA> listaConsultas = (List<PACIENTE_CONSULTA>)Session["ListaConsultas"];
                listaMasterConsulta = listaConsultas.Where(p => p.PACO_DT_CONSULTA.Date > DateTime.Today.Date).ToList();
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirTodas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_CONSULTA> listaConsultas = (List<PACIENTE_CONSULTA>)Session["ListaConsultas"];
                listaMasterConsulta = listaConsultas;
                Session["ListaConsultasGeral"] = listaMasterConsulta;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpGet]
        public ActionResult AlterarDadosCadastrais()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }

                // Recupera paciente
                PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
                Session["IdPaciente"] = paciente.PACI__CD_ID;
                paciente = baseApp.GetItemById(paciente.PACI__CD_ID);

                // Listas
                List<UF> uFs = CarregaUF();
                ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");

                // Carrega view
                PacienteViewModel vm = Mapper.Map<PACIENTE, PacienteViewModel>(paciente);
                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        public ActionResult AlterarDadosCadastrais(PacienteViewModel vm)
        {
            // Preparação
            Int32 idAss = (Int32)Session["IdAssinante"];
            PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
            List<UF> uFs = CarregaUF();
            ViewBag.UF = new SelectList(uFs, "UF_CD_ID", "UF_SG_SIGLA");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PACI_NR_CEP = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NR_CEP);
                    vm.PACI_NR_COMPLEMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_COMPLEMENTO);
                    vm.PACI_NM_ENDERECO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NM_ENDERECO);
                    vm.PACI_NR_NUMERO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PACI_NR_NUMERO);
                    vm.PACI_NM_BAIRRO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NM_BAIRRO);
                    vm.PACI_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NM_CIDADE);
                    vm.PACI_NM_PROFISSAO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.PACI_NM_PROFISSAO);
                    vm.PACI_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_TELEFONE);
                    vm.PACI_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.PACI_NR_CELULAR);
                    vm.PACI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.PACI_NM_EMAIL);
                    vm.PACI__CD_ID = (Int32)Session["IdPaciente"];

                    // Acerta dados
                    vm.PACI_DT_ALTERACAO = DateTime.Now;

                    // Executa a operação
                    Session["FlagAlteraPaciente"] = 1;
                    PACIENTE item = Mapper.Map<PacienteViewModel, PACIENTE>(vm);
                    item.PACI_DT_ACESSO = DateTime.Now;
                    item.PACI_IN_COMPLETADO = 1;
                    Int32 volta = baseApp.ValidateEditArea(item);

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    hist.ASSI_CD_ID = paciente.ASSI_CD_ID;
                    hist.USUA_CD_ID = paciente.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI__CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 1;
                    hist.PAHI_IN_CHAVE = item.PACI__CD_ID;
                    hist.PAHI_NM_OPERACAO = "Área do Paciente - Alteração";
                    hist.PAHI_DS_DESCRICAO = "Alteração do paciente " + item.PACI_NM_NOME;
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Remonta paciente
                    paciente = baseApp.GetItemByCPF((String)Session["PacienteCPF"]);
                    Session["UserCredentials"] = paciente;

                    // Trata retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
                }
            }
            else
            {
                return View(vm);
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
                    conf = baseApp.GetAllUF();
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
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        public List<ASSINANTE> CarregarAssinante()
        {
            try
            {
                List<ASSINANTE> conf = new List<ASSINANTE>();
                conf = assApp.GetAllItens();
                Session["Assinantes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        public List<USUARIO> CarregarUsuario()
        {
            try
            {
                List<USUARIO> conf = new List<USUARIO>();
                conf = usuApp.GetAll().ToList();
                conf = conf.Where(p => p.USUA_IN_SISTEMA == 6 & p.USUA_IN_ATIVO == 1 & p.USUA_IN_BLOQUEADO == 0 & p.USUA_IN_CONSULTA == 1).ToList();
                Session["Usuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        [HttpGet]
        public ActionResult MarcarConsulta()
        {
            try
            {
                // Verifica se tem usuario logado
                PACIENTE paciente = new PACIENTE();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((PACIENTE)Session["UserCredentials"] != null)
                {
                    paciente = (PACIENTE)Session["UserCredentials"];
                }
                else
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }

                // Carrega Assinantes
                ViewBag.Assinantes = new SelectList(CarregarAssinante().Where(p => p.ASSI_IN_ATIVO == 1).OrderBy(x => x.ASSI_NM_NOME).ToList<ASSINANTE>(), "ASSI_CD_ID", "ASSI_NM_NOME");
                ViewBag.Profissionais = new SelectList(CarregarUsuario().Where(p => p.USUA_IN_ATIVO == 1).OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

                // Mensagem
                if (Session["MensArea"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 500)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                Session["MensPaciente"] = null;
                Session["VoltaConfCalendario"] = 2;
                Session["VoltaBloqueio"] = 3;
                Session["VoltaInfoConsulta"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_1.pdf";
                PACIENTE_CONSULTA item = new PACIENTE_CONSULTA();
                PacienteConsultaViewModel vm = Mapper.Map<PACIENTE_CONSULTA, PacienteConsultaViewModel>(item);
                vm.PACI_CD_ID = paciente.PACI__CD_ID;
                vm.PACIENTE = paciente;
                vm.VACO_CD_ID = paciente.VACO_CD_ID;
                vm.PACO_IN_ATIVO = 1;
                vm.PACO_DT_CONSULTA = DateTime.Today.Date;
                vm.USUA_CD_ID = null;
                vm.ASSI_CD_ID = null;
                vm.PACO_IN_TIPO = 1;
                vm.PACO_IN_ENCERRADA = 0;
                vm.PACO_IN_NOVO_PACIENTE = 2;
                vm.PACO_IN_CONFIRMADA = 0;
                vm.PACI_IN_MENOR = paciente.PACI_IN_MENOR;
                vm.MODO_CONSULTA = 1;
                vm.DATA_INICIO = null;
                vm.REPETE = 0;
                vm.SEGUNDA_FEIRA = 0;
                vm.TERCA_FEIRA = 0;
                vm.QUARTA_FEIRA = 0;
                vm.QUINTA_FEIRA = 0;
                vm.SEXTA_FEIRA = 0;
                vm.SABADO = 0;
                vm.DOMINGO = 0;
                vm.PACO_IN_RECORRENTE = 0;
                vm.PACO_IN_RECEBE = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarConsulta(PacienteConsultaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            ViewBag.Assinantes = new SelectList(CarregarAssinante().Where(p => p.ASSI_IN_ATIVO == 1).OrderBy(x => x.ASSI_NM_NOME).ToList<ASSINANTE>(), "ASSI_CD_ID", "ASSI_NM_NOME");
            ViewBag.Profissionais = new SelectList(CarregarUsuario().Where(p => p.USUA_IN_ATIVO == 1).OrderBy(x => x.USUA_NM_NOME).ToList<USUARIO>(), "USUA_CD_ID", "USUA_NM_NOME");
            var tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Presencial", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Remota", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Criação da solicitação
                    Session["UsuarioProf"] = vm.USUA_CD_ID;
                    Session["idAss"] = vm.ASSI_CD_ID;
                    Int32? usuarioProf = vm.USUA_CD_ID;
                    Int32? idAss = vm.ASSI_CD_ID;

                    AREA_PACIENTE area = new AREA_PACIENTE();
                    area.ASSI_CD_ID = idAss.Value;
                    area.AREA_DT_ENTRADA = DateTime.Now;
                    area.PACI_CD_ID = usuario.PACI__CD_ID;
                    area.USUA_CD_ID = usuario.USUA_CD_ID;
                    area.AREA_IN_TIPO = 1;
                    area.AREA_IN_ATIVO = 1;
                    area.AREA_DT_CONSULTA = vm.PACO_DT_CONSULTA;
                    area.AREA_HR_INICIO = vm.PACO_HR_INICIO;
                    area.AREA_HR_FINAL = vm.PACO_HR_FINAL;
                    area.AREA_NM_TITULO = "Solicitação de marcação de consulta";
                    area.AREA_TX_CONTEUDO = "~Solicitação de marcação de consulta de " + usuario.PACI_NM_NOME.ToUpper() + " com o profissional " + usuario.USUARIO.USUA_NM_NOME.ToUpper() + " para o dia " + vm.PACO_DT_CONSULTA.ToShortDateString() + " - " + vm.PACO_HR_INICIO.ToString() + " até " + vm.PACO_HR_FINAL.ToString();
                    Int32 volta = areaApp.ValidateCreate(area);

                    // Mensagem
                    Session["MsgCRUD"] = "A solicitação de marcação de consulta de " + usuario.PACI_NM_NOME.ToUpper() + " com o profissional " + usuario.USUARIO.USUA_NM_NOME.ToUpper() + " para o dia " + vm.PACO_DT_CONSULTA.ToShortDateString() + " - " + vm.PACO_HR_INICIO.ToString() + " até " + vm.PACO_HR_FINAL.ToString() + " foi enviada com sucesso. Você receberá a confirmação de sua conaulta em seu e-mail cadastrado no WebDoctorPro";
                    Session["MensFC"] = 61;


                    // Retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public CONFIGURACAO_CALENDARIO CarregaConfiguracaoCalendario()
        {
            try
            {
                Int32 usuario = (Int32)Session["UsuarioProf"];
                Int32 idAss = (Int32)Session["IdAss"];
                CONFIGURACAO_CALENDARIO conf = new CONFIGURACAO_CALENDARIO();
                conf = calApp.GetAllItems(idAss).Where(p => p.USUA_CD_ID == usuario).FirstOrDefault();
                Session["ConfiguracaoCalendario"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        public List<PACIENTE_CONSULTA> CarregaConsultasNova()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAss"];
                List<PACIENTE_CONSULTA> conf = new List<PACIENTE_CONSULTA>();
                conf = baseApp.GetAllConsultas(idAss);
                Session["Consultas"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        [HttpPost]
        public JsonResult FiltrarProfissional(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaProfFiltrada = CarregarUsuario();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    listaProfFiltrada = listaProfFiltrada.Where(x => x.ASSI_CD_ID == id).ToList();
                }
                return Json(listaProfFiltrada.Select(x => new { x.USUA_CD_ID, x.USUA_NM_NOME }));
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                return null;
            }
        }

        public JsonResult GetProfissional(Int32 id)
        {
            USUARIO prof = usuApp.GetItemById(id);
            String especNome = null;
            String classe = null;
            if (prof.ESPECIALIDADE != null)
            {
                especNome = prof.ESPECIALIDADE.ESPE_NM_NOME;
            }
            else
            {
                especNome = "-";
            }
            if (prof.TIPO_CARTEIRA_CLASSE != null)
            {
                classe = prof.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + prof.USUA_NR_CLASSE;
            }
            else
            {
                classe = "-";
            }

            var hash = new Hashtable();
            hash.Add("nome", prof.USUA_NM_NOME);
            hash.Add("espec", especNome);
            hash.Add("cpf", prof.USUA_NR_CPF);
            hash.Add("classe", classe);
            return Json(hash);
        }

        [HttpGet]
        public ActionResult MontarTelaPerguntas()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];

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
                }

                // Monta objeto
                Session["MensFC"] = 0;
                FaleConoscoViewModel fale = new FaleConoscoViewModel();
                fale.Telefone = conf.CONF_NR_SUPORTE_ZAP;
                fale.EMail = conf.CONF_EM_CRMSYS;
                fale.Resposta = usuario.PACI_NM_EMAIL;
                fale.Nome = usuario.PACI_NM_NOME;

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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID.Value, usuario.ASSI_CD_ID, "PERGUNTAS", "AreaPaciente", "MontarTelaPerguntas");

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
        public async Task<ActionResult> MontarTelaPerguntas(FaleConoscoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
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
                        mens.MENS_NM_NOME = "Área do Paciente - Pergunta";
                        mens.MENS_NM_CAMPANHA = assuntoDesc;
                        await ProcessaEnvioEMailPergunta(mens, usuario);
                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaAreaPaciente");
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
        public async Task<Int32> ProcessaEnvioEMailPergunta(MensagemViewModel vm, PACIENTE usuario)
        {
            // Recupera usuario
            Int32 idAss = usuario.ASSI_CD_ID;

            // Prepara corpo do e-mail e trata link
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            corpo = corpo.Replace("\r\n", "<br />");

            // Monta mensagem
            ASSINANTE assi = assApp.GetItemById(idAss);
            corpo = corpo + "<b style='color:darkblue'>Assinante:</b> " + assi.ASSI_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Num.Assinante:</b> " + assi.ASSI_CD_ID.ToString() + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Usuário:</b> " + usuario.USUARIO.USUA_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>CPF/CNPJ:</b> " + (assi.TIPE_CD_ID == 1 ? assi.ASSI_NR_CPF : assi.ASSI_NR_CNPJ) + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Data Assinatura:</b> " + assi.ASSI_DT_INICIO.Value.ToShortDateString() + "<br />";

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
            Session["MsgCRUD"] = "E-Mail para " + usuario.USUARIO.USUA_NM_NOME + " foi enviado com sucesso.";
            Session["MensFC"] = 61;
            return 0;
        }

    }
}