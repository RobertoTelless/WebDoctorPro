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

        public AreaPacienteController(IPacienteAppService baseApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IConfiguracaoCalendarioAppService calApps, IConfiguracaoAnamneseAppService anaApps, ILogAppService logApps, ILocacaoAppService locaApps)
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
                if (Session["MensPaciente"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
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
                    if ((Int32)Session["MensPaciente"] == 502)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 503)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 504)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0545", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 701)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0542", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 702)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0543", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 703)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0544", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 800)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0551", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 801)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0552", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 802)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0553", CultureInfo.CurrentCulture));
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
                    // Sanitização

                    // Criação de consulta normal (única)
                    Session["UsuarioProf"] = vm.USUA_CD_ID;
                    Session["idAss"] = vm.ASSI_CD_ID;
                    Int32? usuarioProf = vm.USUA_CD_ID;
                    Int32? idAss = vm.ASSI_CD_ID;

                    CONFIGURACAO_CALENDARIO confCal = CarregaConfiguracaoCalendario();
                    if (vm.MODO_CONSULTA == 1)
                    {
                        // Critica dia util
                        DateTime dataCons = vm.PACO_DT_CONSULTA;
                        Int32 dia = (Int32)dataCons.DayOfWeek;
                        Int32 voltaCal = VerificacaoDataCalendario.ValidaDiaUtil(dia, confCal);
                        if (voltaCal == 1)
                        {
                            Session["MensPaciente"] = 800;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0551", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Critica de horario util
                        Int32 voltaUtil = VerificacaoDataCalendario.ValidaHoraUtil(dia, vm, confCal);
                        if (voltaUtil == 1)
                        {
                            Session["MensPaciente"] = 801;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0552", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (voltaUtil == 2)
                        {
                            Session["MensPaciente"] = 802;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0553", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Critica de data
                        if (vm.PACO_HR_INICIO == null || vm.PACO_HR_FINAL == null)
                        {
                            Session["MensPaciente"] = 504;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0545", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
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

                        // Verifica se já tem consulta na data selecionada para o mesmo paciente e mesmo tipo de consulta
                        if (vm.PACO_IN_NOVO_PACIENTE == 2)
                        {
                            List<PACIENTE_CONSULTA> conPaciente = CarregaConsultasNova().Where(p => p.USUA_CD_ID == vm.USUA_CD_ID & p.PACI_CD_ID == vm.PACI_CD_ID & p.PACO_DT_CONSULTA == vm.PACO_DT_CONSULTA & p.PACO_IN_ATIVO == 1 & p.PACO_IN_CONFIRMADA < 2).ToList();
                            if (conPaciente.Count > 0)
                            {
                                Session["MensPaciente"] = 600;
                                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0533", CultureInfo.CurrentCulture));
                                return View(vm);
                            }
                        }

                        // Critica de horario
                        List<PACIENTE_CONSULTA> lista = baseApp.GetAllConsultas(vm.ASSI_CD_ID.Value).Where(p => p.USUA_CD_ID == vm.USUA_CD_ID & p.PACO_DT_CONSULTA.Date == vm.PACO_DT_CONSULTA.Date & p.PACO_IN_ATIVO == 1 & p.PACO_IN_CONFIRMADA < 2).ToList();
                        List<PACIENTE_CONSULTA> lista1 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL & p.PACO_HR_INICIO < vm.PACO_HR_FINAL).ToList();
                        List<PACIENTE_CONSULTA> lista2 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL >= vm.PACO_HR_FINAL).ToList();
                        List<PACIENTE_CONSULTA> lista3 = lista.Where(p => p.PACO_HR_INICIO <= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL & p.PACO_HR_FINAL > vm.PACO_HR_INICIO).ToList();
                        List<PACIENTE_CONSULTA> lista4 = lista.Where(p => p.PACO_HR_INICIO >= vm.PACO_HR_INICIO & p.PACO_HR_FINAL <= vm.PACO_HR_FINAL).ToList();
                        if (lista1.Count > 0 || lista2.Count > 0 || lista3.Count > 0 || lista4.Count > 0)
                        {
                            Session["MensPaciente"] = 500;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Verifica bloqueios
                        List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(vm.ASSI_CD_ID.Value);
                        CONFIGURACAO_CALENDARIO cal = null;
                        cal = confs.Where(p => p.USUA_CD_ID == vm.USUA_CD_ID).FirstOrDefault();
                        List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = cal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                        Int32 bloqFlag = 0;
                        foreach (CONFIGURACAO_CALENDARIO_BLOQUEIO bloq in bloqs)
                        {
                            if (vm.PACO_DT_CONSULTA >= bloq.COCB_DT_BLOQUEIO_INICIO & vm.PACO_DT_CONSULTA <= bloq.COCB_DT_BLOQUEIO_FINAL)
                            {
                                if (bloq.COCB_HR_INICIO == null || bloq.COCB_HR_FINAL == null)
                                {
                                    bloqFlag = 1;
                                }
                                else
                                {
                                    if (vm.PACO_HR_INICIO >= bloq.COCB_HR_INICIO & vm.PACO_HR_FINAL <= bloq.COCB_HR_FINAL)
                                    {
                                        bloqFlag = 1;
                                    }
                                }
                            }
                        }

                        if (bloqFlag > 0)
                        {
                            Session["MensPaciente"] = 500;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0565", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Completa campo
                        vm.PACO_IN_RECORRENTE = 0;
                        vm.PACO_IN_RECEBE = 1;

                        // Executa a operação
                        PACIENTE_CONSULTA item = Mapper.Map<PacienteConsultaViewModel, PACIENTE_CONSULTA>(vm);
                        Int32 volta = baseApp.ValidateCreateConsulta(item);

                        // Recupera paciente
                        PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);

                        // Acerta estado
                        Session["PacienteConsulta"] = pac;
                        Session["PacienteAlterada"] = 1;
                        Session["NivelPaciente"] = 3;
                        Session["ConsultasAlterada"] = 1;
                        Session["ListaConsultasGeral"] = null;
                        Session["IdPaciente"] = item.PACI_CD_ID;
                        Session["IdConsulta"] = item.PACO_CD_ID;
                        Session["VoltaConfCalendario"] = 1;

                        // Verifica se já existe anamnese para o paciente
                        List<PACIENTE_CONSULTA> listaAnterior = baseApp.GetAllConsultas(vm.ASSI_CD_ID.Value).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACI_CD_ID == item.PACI_CD_ID & p.PACO_IN_ATIVO == 1 & p.PACO_IN_ENCERRADA == 0 & p.PACO_DT_CONSULTA != item.PACO_DT_CONSULTA).OrderBy(p => p.PACO_DT_CONSULTA).ToList();
                        PACIENTE_CONSULTA consultaAnterior = listaAnterior.LastOrDefault();
                        PACIENTE_ANAMNESE anam = pac.PACIENTE_ANAMNESE.Where(p => p.PAAM_IN_ATIVO == 1).FirstOrDefault();

                        // Cria anamnese em branco ou atualiza a da ultima consulta
                        CONFIGURACAO_ANAMNESE ana = anaApp.GetItemById(idAss.Value);
                        PACIENTE_ANAMNESE anamnese = new PACIENTE_ANAMNESE();
                        if (anam == null)
                        {
                            anamnese.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            anamnese.PAAM_DT_DATA = vm.PACO_DT_CONSULTA;
                            anamnese.PAAM_DT_ORIGINAL = vm.PACO_DT_CONSULTA;
                            anamnese.PAAM_IN_ATIVO = 1;
                            anamnese.PACI_CD_ID = item.PACI_CD_ID;
                            anamnese.USUA_CD_ID = usuario.USUA_CD_ID;
                            anamnese.PACO_CD_ID = item.PACO_CD_ID;
                            anamnese.PAAM_IN_PREENCHIDA = 0;
                            anamnese.PAAM_IN_FLAG_MOTIVO_CONSULTA = 1;
                            anamnese.PAAM_IN_FLAG_MEDICAMENTO = 1;
                            anamnese.PAAM_IN_FLAG__HISTORIA_FAMILIAR = 1;
                            anamnese.PAAM_IN_FLAG_HISTORIA_SOCIAL = ana.COAN_IN_HISTORIA_SOCIAL;
                            anamnese.PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA = ana.COAN_IN_CARDIOLOGICA;
                            anamnese.PAAM_IN_FLAG_RESPIRATORIO = ana.COAN_IN_RESPIRATORIA;
                            anamnese.PAAM_IN_FLAG_ABDOMEM = ana.COAN_IN_ABDOMEM;
                            anamnese.PAAM_IN_FLAG_MEMBROS_INFERIORES = ana.COAN_IN_MEMBROS;
                            anamnese.PAAM_IN_FLAG_QUEIXA_PRINCIPAL = 1;
                            anamnese.PAAM_IN_FLAG_HISTORIA_DOENCA_ATUAL = 1;
                            anamnese.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA = ana.COAN_IN_HISTORIA_PATOLOGIA;
                            anamnese.PAAM_IN_FLAG_DIAGNOSTICO_1 = 1;
                            anamnese.PAAM_IN_CAMPO_1 = ana.COAN_IN_CAMPO_1;
                            anamnese.PAAM_NM_CAMPO_1 = ana.COAN_NM_CAMPO_1;
                            anamnese.PAAM_IN_CAMPO_2 = ana.COAN_IN_CAMPO_2;
                            anamnese.PAAM_NM_CAMPO_2 = ana.COAN_NM_CAMPO_2;
                            anamnese.PAAM_IN_CAMPO_3 = ana.COAN_IN_CAMPO_3;
                            anamnese.PAAM_NM_CAMPO_3 = ana.COAN_NM_CAMPO_3;
                            anamnese.PAAM_IN_CAMPO_4 = ana.COAN_IN_CAMPO_4;
                            anamnese.PAAM_NM_CAMPO_4 = ana.COAN_NM_CAMPO_4;
                            anamnese.PAAM_IN_CAMPO_5 = ana.COAN_IN_CAMPO_5;
                            anamnese.PAAM_NM_CAMPO_5 = ana.COAN_NM_CAMPO_5;
                            anamnese.PAAM_IN_ALTERADA = 0;
                            Int32 voltaA = baseApp.ValidateCreateAnamnese(anamnese);
                        }

                        // Cria exame fisico em branco ou copia da ultima consulta
                        PACIENTE_EXAME_FISICOS fisi = pac.PACIENTE_EXAME_FISICOS.Where(p => p.PAEF_IN_ATIVO == 1).FirstOrDefault();
                        PACIENTE_EXAME_FISICOS fisico = new PACIENTE_EXAME_FISICOS();
                        if (fisi == null)
                        {
                            fisico.ASSI_CD_ID = usuario.ASSI_CD_ID;
                            fisico.PACI_CD_ID = item.PACI_CD_ID;
                            fisico.PAEF_DT_DATA = vm.PACO_DT_CONSULTA;
                            fisico.PAEF_DT_ORIGINAL = vm.PACO_DT_CONSULTA;
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
                            Int32 voltaF = baseApp.ValidateCreateExameFisico(fisico);
                        }

                        // Acerta proxima consulta do paciente
                        Int32? ret = conf.CONF_NR_MESES_RETORNO;
                        Int32? calc = conf.CONF_IN_CALCULA_PROXIMA_CONSULTA;
                        List<PACIENTE_CONSULTA> cons = pac.PACIENTE_CONSULTA.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date & p.PACO_IN_ATIVO == 1 & p.PACO_IN_CONFIRMADA < 2).ToList();
                        if (cons.Count == 0)
                        {
                            pac.PACI_DT_CONSULTA = item.PACO_DT_CONSULTA;
                            if (calc == 1)
                            {
                                pac.PACI_DT_PREVISAO_RETORNO = item.PACO_DT_CONSULTA.AddMonths(ret.Value);
                            }
                            else
                            {
                                pac.PACI_DT_PREVISAO_RETORNO = null;
                            }
                            Int32 voltaP = baseApp.ValidateEdit(pac, pac);
                        }
                        else
                        {
                            cons = pac.PACIENTE_CONSULTA.Where(p => p.PACO_DT_CONSULTA.Date < item.PACO_DT_CONSULTA.Date & p.PACO_IN_ATIVO == 1 & p.PACO_IN_CONFIRMADA < 2).ToList();
                            if (cons.Count == 0)
                            {
                                pac.PACI_DT_CONSULTA = item.PACO_DT_CONSULTA;
                                if (calc == 1)
                                {
                                    pac.PACI_DT_PREVISAO_RETORNO = item.PACO_DT_CONSULTA.AddMonths(ret.Value);
                                }
                                else
                                {
                                    pac.PACI_DT_PREVISAO_RETORNO = null;
                                }
                                Int32 voltaP = baseApp.ValidateEdit(pac, pac);
                            }
                        }

                        // Envia mensagem
                        if (pac.PACI_NM_EMAIL != null & conf.CONF_IN_MENSAGEM_CONSULTA == 1)
                        {
                            PACIENTE_CONSULTA consMensagem = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                            //Int32 voltaCons = EnviarEMailConsulta(consMensagem, 1);
                        }
                        if (pac.PACI_NR_CELULAR != null & conf.CONF_IN_MENSAGEM_CONSULTA == 1)
                        {
                            PACIENTE_CONSULTA consMensagem = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                            //Int32 voltaCons = EnviarSMSConsulta(consMensagem, 1);
                        }
                        //if (usuario.USUA_NM_EMAIL != null & conf.CONF_IN_MENSAGEM_CONSULTA == 1)
                        //{
                        //    PACIENTE_CONSULTA consMensagem = baseApp.GetConsultaById((Int32)Session["IdConsulta"]);
                        //    Int32 voltaCons = EnviarEMailConsulta(consMensagem, 5);
                        //}

                        // Monta Log
                        String frase = item.PACI_CD_ID.ToString() + "|" + item.PACI_CD_ID.ToString() + "|" + item.PACO_DT_CONSULTA.ToString() + "|" + item.PACO_HR_INICIO.ToString() + "|" + item.PACO_HR_FINAL.ToString() + "|" + item.VACO_CD_ID.ToString();
                        LOG log = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = vm.ASSI_CD_ID.Value,
                            USUA_CD_ID = vm.USUA_CD_ID.Value,
                            LOG_NM_OPERACAO = "icoPACI",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = frase,
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

                        Session["ListaPacienteBase"] = null;
                        Session["PacienteAlterada"] = 1;
                        Session["ListaPaciente"] = null;

                        // Mensagem do CRUD
                        Session["MsgCRUD"] = "A consulta do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " foi marcada com sucesso para " + item.PACO_DT_CONSULTA.ToLongDateString();
                        Session["MensPaciente"] = 888;
                        Session["ConsultaMarcada"] = 1;
                        Session["ConsultaFrase"] = item.PACO_DT_CONSULTA.ToShortDateString() + " de " + item.PACO_HR_INICIO.ToString() + " até " + item.PACO_HR_FINAL.ToString();
                    }

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

    }
}