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
using Newtonsoft.Json;
using EntitiesServices.Work_Classes;

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
        private readonly ITemplateEMailAppService temApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly INoticiaAppService notApp;

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
        private List<NOTICIA> listaMasterNoticia = new List<NOTICIA>();

        public AreaPacienteController(IPacienteAppService baseApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IConfiguracaoCalendarioAppService calApps, IConfiguracaoAnamneseAppService anaApps, ILogAppService logApps, ILocacaoAppService locaApps, IAreaPacienteAppService areaApps, ITemplateEMailAppService temApps, IMensagemEnviadaSistemaAppService meApps, ITemplateSMSAppService smsApps, INoticiaAppService notApps)
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
            temApp = temApps;
            meApp = meApps;
            smsApp = smsApps;
            notApp = notApps;
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

                USUARIO usu = usuApp.GetItemById(paciente.USUA_CD_ID.Value);
                Session["UsuarioArea"] = usu;

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
                Session["ModoEntrada"] = 2;
                Session["ListaLocacao"] = null;
                Session["ListaAtestados"] = null;
                Session["ListaSolicitacao"] = null;
                Session["ListaPrescricao"] = null;
                Session["ListaNoticia"] = null;

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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensArea"] == 111)
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
                if (Session["ListaAtestados"] == null)
                {
                    listaMasterAtestado = baseApp.GetAtestadosByCPF(cpf);
                    Session["ListaAtestados"] = listaMasterAtestado;
                }
                listaMasterAtestado = (List<PACIENTE_ATESTADO>)Session["ListaAtestados"];
                ViewBag.NumAtestados = listaMasterAtestado.Count();
                ViewBag.Atestados = listaMasterAtestado;

                // Montar listas de exames
                listaMasterExame = baseApp.GetExamesByCPF(cpf);
                ViewBag.NumExames = listaMasterExame.Count();
                Session["ListaExames"] = listaMasterExame;

                // Montar listas de solicitacoes
                if (Session["ListaSolicitacao"] == null)
                {
                    listaMasterSolicitacao = baseApp.GetSolicitacaoByCPF(cpf);
                    Session["ListaSolicitacao"] = listaMasterSolicitacao;
                }
                listaMasterSolicitacao = (List<PACIENTE_SOLICITACAO>)Session["ListaSolicitacao"];
                ViewBag.NumSolicitacao = listaMasterSolicitacao.Count();
                ViewBag.Solicitacoes = listaMasterSolicitacao;

                // Montar listas de prescricoes
                if (Session["ListaPrescricao"] == null)
                {
                    listaMasterPrescricao = baseApp.GetPrescricaoByCPF(cpf);
                    Session["ListaPrescricao"] = listaMasterPrescricao;
                }
                listaMasterPrescricao = (List<  PACIENTE_PRESCRICAO>)Session["ListaPrescricao"];
                ViewBag.NumPrescricoes = listaMasterPrescricao.Count();
                ViewBag.Prescricoes = listaMasterPrescricao;

                // Montar listas de locações
                if (Session["ListaLocacao"] == null)
                {
                    listaMasterLocacao = locaApp.GetLocacaoByCPF(cpf).Where(p => p.LOCA_IN_STATUS == 1).ToList();
                    Session["ListaLocacao"] = listaMasterLocacao;
                }
                listaMasterLocacao = (List<LOCACAO>)Session["ListaLocacao"];
                ViewBag.NumLocacoes = listaMasterLocacao.Count();
                ViewBag.Locacoes = listaMasterLocacao;

                // Montar listas de noticias
                if (Session["ListaNoticia"] == null)
                {
                    listaMasterNoticia = CarregaNoticiaGeral();
                    Session["ListaNoticia"] = listaMasterNoticia;
                }
                listaMasterNoticia = (List<NOTICIA>)Session["ListaNoticia"];
                ViewBag.NumNoticias = listaMasterNoticia.Count();
                ViewBag.Noticias = listaMasterNoticia;

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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Os dados cadastrais de " + item.PACI_NM_NOME.ToUpper() + " foram alterados com sucesso";
                    Session["MensArea"] = 61;

                    // Trata retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                    if ((Int32)Session["MensArea"] == 500)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0524", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 501)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0526", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 502)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 503)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 504)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0545", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 800)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0551", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 801)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0552", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 802)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0553", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                Session["MensPaciente"] = null;
                Session["VoltaConfCalendario"] = 2;
                Session["VoltaBloqueio"] = 3;
                Session["VoltaInfoConsulta"] = 1;

                USUARIO usu = usuApp.GetItemById(paciente.USUA_CD_ID.Value);

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
                vm.USUARIO = usu;
                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                    // Critica dia util
                    Int32 idAss = usuario.ASSI_CD_ID;
                    Session["UsuarioProf"] = usuario.USUA_CD_ID;

                    //Criticas
                    if (vm.PACO_HR_INICIO == null)
                    {
                        Session["MensArea"] = 504;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0545", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_DT_CONSULTA == null)
                    {
                        Session["MensArea"] = 504;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0737", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PACO_DT_CONSULTA.Date < DateTime.Today.Date)
                    {
                        Session["MensArea"] = 501;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0526", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Criação da solicitação
                    Session["UsuarioProf"] = vm.USUA_CD_ID;
                    Session["idAss"] = vm.ASSI_CD_ID;
                    Int32? usuarioProf = vm.USUA_CD_ID;

                    AREA_PACIENTE area = new AREA_PACIENTE();
                    area.ASSI_CD_ID = idAss;
                    area.AREA_DT_ENTRADA = DateTime.Now;
                    area.PACI_CD_ID = usuario.PACI__CD_ID;
                    area.USUA_CD_ID = usuario.USUA_CD_ID;
                    area.AREA_IN_TIPO = 1;
                    area.AREA_IN_ATIVO = 1;
                    area.AREA_IN_PROCESSADA = 0;
                    area.AREA_IN_VISTA = 0;
                    area.AREA_DT_CONSULTA = vm.PACO_DT_CONSULTA;
                    area.AREA_HR_INICIO = vm.PACO_HR_INICIO;
                    area.AREA_IN_TIPO_CONSULTA = vm.PACO_IN_TIPO;
                    area.AREA_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                    area.AREA_NM_TITULO = "Solicitação de marcação de consulta";
                    area.AREA_TX_CONTEUDO = "Solicitação de marcação de consulta de " + usuario.PACI_NM_NOME.ToUpper() + " com o profissional " + usuario.USUARIO.USUA_NM_NOME.ToUpper() + " para o dia " + vm.PACO_DT_CONSULTA.ToShortDateString() + " - " + vm.PACO_HR_INICIO.ToString() + " até " + vm.PACO_HR_FINAL.ToString();
                    Int32 volta = areaApp.ValidateCreate(area);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/AreaPaciente/" + area.AREA_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Mensagem
                    Session["MsgCRUD"] = "A solicitação de marcação de consulta de " + usuario.PACI_NM_NOME.ToUpper() + " com o profissional " + usuario.USUARIO.USUA_NM_NOME.ToUpper() + " para o dia " + vm.PACO_DT_CONSULTA.ToShortDateString() + " - " + vm.PACO_HR_INICIO.ToString() + " até " + vm.PACO_HR_FINAL.ToString() + " foi enviada com sucesso. Você receberá a confirmação de sua consulta em seu e-mail cadastrado no WebDoctorPro";
                    Session["MensArea"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
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

                // Trata mensagens
                if (Session["MensArea"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensArea"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
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
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
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

                        // Mensagem do CRUD
                        Session["MsgCRUD"] = "A mensagem para o suporte do WebDoctorPro foi enviada com sucesso";
                        Session["MensArea"] = 61;

                    }

                    // Sucesso
                    return RedirectToAction("MontarTelaPerguntas");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                    return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
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
            ASSINANTE assi = assApp.GetItemById(idAss);
            String corpo = String.Empty;
            corpo = corpo + "<b style='color:darkblue'>Paciente:</b> " + usuario.PACI_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Id.Paciente:</b> " + usuario.PACI__CD_ID.ToString() + "<br />";
            corpo = corpo + "<b style='color:darkblue'>CPF:</b> " + usuario.PACI_NR_CPF + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Assinante:</b> " + assi.ASSI_NM_NOME + "<br />";
            corpo = corpo + "<b style='color:darkblue'>Motivo:</b> " + vm.MENS_NM_CAMPANHA + "<br /><br />";
            corpo += vm.MENS_TX_TEXTO + "<br /><br />";
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

            return 0;
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarConsulta(Int32 id)
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

                // Recupera dados
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);

                // Recupera paciente
                PACIENTE pac = baseApp.GetItemById(paciente.PACI__CD_ID);
                USUARIO usuarioLogado = usuApp.GetItemById(pac.USUA_CD_ID.Value);

                // Verifica se pode confirmar
                List<PACIENTE_CONSULTA> cons = pac.PACIENTE_CONSULTA.Where(p => p.PACO_IN_ATIVO == 1 & p.PACO_IN_CONFIRMADA == 1 & p.PACO_IN_ENCERRADA == 0 & p.PACO_DT_CONSULTA.Date < DateTime.Today.Date).ToList();
                if (cons.Count > 0)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0593", CultureInfo.CurrentCulture);
                    String frase1 = CRMSys_Base.ResourceManager.GetString("M0594", CultureInfo.CurrentCulture);
                    frase += " de " + pac.PACI_NM_NOME + " em " + item.PACO_DT_CONSULTA.ToShortDateString() + ". " + frase1;
                    Session["MensArea"] = 111;
                    Session["MsgCRUD"] = frase;
                    return RedirectToAction("MontarTelaAreaPaciente");
                }

                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACO_IN_CONFIRMADA = 1;
                Int32 volta = baseApp.ValidateEditConsultaConfirma(item);

                // Acerta anamnese
                PACIENTE_ANAMNESE anam = pac.PACIENTE_ANAMNESE.Where(p => p.PAAM_IN_ATIVO == 1).FirstOrDefault();
                if (anam != null)
                {
                    PACIENTE_ANAMNESE anamnese = RemontarAnamnese(anam);
                    anamnese.PAAM_DT_DATA = item.PACO_DT_CONSULTA;
                    anamnese.PACO_CD_ID = item.PACO_CD_ID;
                    Int32 voltaA = baseApp.ValidateEditAnamnese(anamnese);
                }

                // Acerta exame fisico
                PACIENTE_EXAME_FISICOS fisi = pac.PACIENTE_EXAME_FISICOS.Where(p => p.PAEF_IN_ATIVO == 1).FirstOrDefault();
                if (fisi != null)
                {
                    PACIENTE_EXAME_FISICOS fisico = RemontarFisico(fisi);
                    fisico.PAEF_DT_DATA = item.PACO_DT_CONSULTA;
                    fisico.PACO_CD_ID = item.PACO_CD_ID;
                    Int32 voltaF = baseApp.ValidateEditExameFisico(fisico);
                }

                // Acerta paciente
                PACIENTE pac1 = baseApp.GetItemById(item.PACI_CD_ID);
                paciente.PACI_DT_CONSULTA = item.PACO_DT_CONSULTA;
                if (conf.CONF_IN_CALCULA_PROXIMA_CONSULTA == 1)
                {
                    paciente.PACI_DT_PREVISAO_RETORNO = item.PACO_DT_CONSULTA.AddMonths(conf.CONF_NR_MESES_RETORNO.Value);
                }
                Int32 voltaP = baseApp.ValidateEdit(paciente, paciente);

                // Acerta estado
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 3;
                Session["ListaConsultasGeral"] = null;
                Session["ConsultasAlterada"] = 1;
                Session["ListaConfirma"] = null;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Paciente_Consulta dto = MontarPacienteConsultaDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Consulta - Confirmação",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 10;
                hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Confirmação de Consulta";
                hist.PAHI_DS_DESCRICAO = "Paciente " + paciente.PACI_NM_NOME + " - Consulta confirmada " + item.PACO_DT_CONSULTA.ToShortDateString();
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A consulta do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " marcada para " + item.PACO_DT_CONSULTA.ToLongDateString() + " foi confirmada com sucesso";
                Session["MensArea"] = 61;

                // Envia mensagem
                if (pac.PACI_NM_EMAIL != null & conf.CONF_IN_ENVIA_CONFIRMACAO == 1)
                {
                    Int32 voltaCons = await EnviarEMailConsulta(item, 3);
                }
                if (pac.PACI_NR_CELULAR != null & conf.CONF_IN_ENVIA_CONFIRMACAO == 1)
                {
                    Int32 voltaCons = EnviarSMSConsulta(item, 3);
                }
                if (usuarioLogado.USUA_NM_EMAIL != null & conf.CONF_IN_ENVIA_CONFIRMACAO == 1)
                {
                    Int32 voltaCons = await EnviarEMailConsulta(item, 6);
                }

                // Retorno
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public PACIENTE_ANAMNESE RemontarAnamnese(PACIENTE_ANAMNESE anamneseAnterior)
        {
            PACIENTE_ANAMNESE anamnese = new PACIENTE_ANAMNESE();
            anamnese.PAAM_CD_ID = anamneseAnterior.PAAM_CD_ID;
            anamnese.ASSI_CD_ID = anamneseAnterior.ASSI_CD_ID;
            anamnese.PAAM_DT_DATA = anamneseAnterior.PAAM_DT_DATA;
            anamnese.PAAM_IN_ATIVO = 1;
            anamnese.PACI_CD_ID = anamneseAnterior.PACI_CD_ID;
            anamnese.USUA_CD_ID = anamneseAnterior.USUA_CD_ID;
            anamnese.PACO_CD_ID = anamneseAnterior.PACO_CD_ID;
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
            anamnese.PAAM_NM_MEDICAMENTO = anamneseAnterior.PAAM_NM_MEDICAMENTO;
            anamnese.PAAN_NM_AVALIACAO_CARDIOLOGICA = anamneseAnterior.PAAN_NM_AVALIACAO_CARDIOLOGICA;
            anamnese.PAAN_NM_MEMBROS_INFERIORES = anamneseAnterior.PAAN_NM_MEMBROS_INFERIORES;
            anamnese.PAAN_NM_RESPIRATORIO = anamneseAnterior.PAAN_NM_RESPIRATORIO;
            anamnese.PAAM_DT_COPIA = anamneseAnterior.PAAM_DT_COPIA;
            anamnese.PAAM_DT_ORIGINAL = anamneseAnterior.PAAM_DT_ORIGINAL;
            anamnese.PAAM_TX_TEXTO_LIVRE = anamneseAnterior.PAAM_TX_TEXTO_LIVRE;

            anamnese.PAAM_DS_SONO_ACESSO_SAUDE = anamneseAnterior.PAAM_DS_SONO_ACESSO_SAUDE;
            anamnese.PAAM_DS_SONO_ACONCHEGANTE = anamneseAnterior.PAAM_DS_SONO_ACONCHEGANTE;
            anamnese.PAAM_DS_SONO_AGRESSIVO = anamneseAnterior.PAAM_DS_SONO_AGRESSIVO;
            anamnese.PAAM_DS_SONO_ALMOCO = anamneseAnterior.PAAM_DS_SONO_ALMOCO;
            anamnese.PAAM_DS_SONO_ANIMAIS = anamneseAnterior.PAAM_DS_SONO_ANIMAIS;
            anamnese.PAAM_DS_SONO_APNEIA = anamneseAnterior.PAAM_DS_SONO_APNEIA;
            anamnese.PAAM_DS_SONO_ATIVIDADES = anamneseAnterior.PAAM_DS_SONO_ATIVIDADES;
            anamnese.PAAM_DS_SONO_ATIVIDADES_OLD = anamneseAnterior.PAAM_DS_SONO_ATIVIDADES_OLD;
            anamnese.PAAM_DS_SONO_AZIA = anamneseAnterior.PAAM_DS_SONO_AZIA;
            anamnese.PAAM_DS_SONO_BARULHO = anamneseAnterior.PAAM_DS_SONO_BARULHO;
            anamnese.PAAM_DS_SONO_BOCA_SECA = anamneseAnterior.PAAM_DS_SONO_BOCA_SECA;
            anamnese.PAAM_DS_SONO_CAFE = anamneseAnterior.PAAM_DS_SONO_CAFE;
            anamnese.PAAM_DS_SONO_CAIBRAS = anamneseAnterior.PAAM_DS_SONO_CAIBRAS;
            anamnese.PAAM_DS_SONO_CANSACO = anamneseAnterior.PAAM_DS_SONO_CANSACO;
            anamnese.PAAM_DS_SONO_CELULAR_CAMA = anamneseAnterior.PAAM_DS_SONO_CELULAR_CAMA;
            anamnese.PAAM_DS_SONO_CIRURGIAS = anamneseAnterior.PAAM_DS_SONO_CIRURGIAS;
            anamnese.PAAM_DS_SONO_CIRURGIAS_OLD = anamneseAnterior.PAAM_DS_SONO_CIRURGIAS_OLD;
            anamnese.PAAM_DS_SONO_COCHILOS = anamneseAnterior.PAAM_DS_SONO_COCHILOS;
            anamnese.PAAM_DS_SONO_COMORBIDADES = anamneseAnterior.PAAM_DS_SONO_COMORBIDADES;
            anamnese.PAAM_DS_SONO_COMORBIDADES_OLD = anamneseAnterior.PAAM_DS_SONO_COMORBIDADES_OLD;
            anamnese.PAAM_DS_SONO_CONGESTAO = anamneseAnterior.PAAM_DS_SONO_CONGESTAO;
            anamnese.PAAM_DS_SONO_DEFICIT = anamneseAnterior.PAAM_DS_SONO_DEFICIT;
            anamnese.PAAM_DS_SONO_DEFICIT_CONCENTRA = anamneseAnterior.PAAM_DS_SONO_DEFICIT_CONCENTRA;
            anamnese.PAAM_DS_SONO_DEFICIT_MEMO = anamneseAnterior.PAAM_DS_SONO_DEFICIT_MEMO;
            anamnese.PAAM_DS_SONO_DEFICIT_MEMORIA = anamneseAnterior.PAAM_DS_SONO_DEFICIT_MEMORIA;
            anamnese.PAAM_DS_SONO_DEITADO_PERDE_SONO = anamneseAnterior.PAAM_DS_SONO_DEITADO_PERDE_SONO;
            anamnese.PAAM_DS_SONO_DEITADO_SONO = anamneseAnterior.PAAM_DS_SONO_DEITADO_SONO;
            anamnese.PAAM_DS_SONO_DISFUNCAO = anamneseAnterior.PAAM_DS_SONO_DISFUNCAO;
            anamnese.PAAM_DS_SONO_DOR = anamneseAnterior.PAAM_DS_SONO_DOR;
            anamnese.PAAM_DS_SONO_DOR_CABECA = anamneseAnterior.PAAM_DS_SONO_DOR_CABECA;
            anamnese.PAAM_DS_SONO_DURACAO = anamneseAnterior.PAAM_DS_SONO_DURACAO;
            anamnese.PAAM_DS_SONO_DURACAO_OLD = anamneseAnterior.PAAM_DS_SONO_DURACAO_OLD;
            anamnese.PAAM_DS_SONO_ENCENACAO = anamneseAnterior.PAAM_DS_SONO_ENCENACAO;
            anamnese.PAAM_DS_SONO_ENGASGOS = anamneseAnterior.PAAM_DS_SONO_ENGASGOS;
            anamnese.PAAM_DS_SONO_EXERCICIO = anamneseAnterior.PAAM_DS_SONO_EXERCICIO;
            anamnese.PAAM_DS_SONO_EXERCICIO_FREQ = anamneseAnterior.PAAM_DS_SONO_EXERCICIO_FREQ;
            anamnese.PAAM_DS_SONO_EXERCICIO_FREQ_OLD = anamneseAnterior.PAAM_DS_SONO_EXERCICIO_FREQ_OLD;
            anamnese.PAAM_DS_SONO_EXERCICIO_HORARIO = anamneseAnterior.PAAM_DS_SONO_EXERCICIO_HORARIO;
            anamnese.PAAM_DS_SONO_FADIGA = anamneseAnterior.PAAM_DS_SONO_FADIGA;
            anamnese.PAAM_DS_SONO_FALA = anamneseAnterior.PAAM_DS_SONO_FALA;
            anamnese.PAAM_DS_SONO_FINANCAS = anamneseAnterior.PAAM_DS_SONO_FINANCAS;
            anamnese.PAAM_DS_SONO_FUMA_NOITE = anamneseAnterior.PAAM_DS_SONO_FUMA_NOITE;
            anamnese.PAAM_DS_SONO_HORARIO_REGULAR = anamneseAnterior.PAAM_DS_SONO_HORARIO_REGULAR;
            anamnese.PAAM_DS_SONO_HORARIO_REGULAR_NOVO = anamneseAnterior.PAAM_DS_SONO_HORARIO_REGULAR_NOVO;
            anamnese.PAAM_DS_SONO_HORARIO_REGULAR_NOVO_OLD = anamneseAnterior.PAAM_DS_SONO_HORARIO_REGULAR_NOVO_OLD;
            anamnese.PAAM_DS_SONO_IRRITA = anamneseAnterior.PAAM_DS_SONO_IRRITA;
            anamnese.PAAM_DS_SONO_JANTAR = anamneseAnterior.PAAM_DS_SONO_JANTAR;
            anamnese.PAAM_DS_SONO_LANCHE = anamneseAnterior.PAAM_DS_SONO_LANCHE;
            anamnese.PAAM_DS_SONO_LATENCIA = anamneseAnterior.PAAM_DS_SONO_LATENCIA;
            anamnese.PAAM_DS_SONO_LATENCIA_NOVO = anamneseAnterior.PAAM_DS_SONO_LATENCIA_NOVO;
            anamnese.PAAM_DS_SONO_LATENCIA_NOVO_OLD = anamneseAnterior.PAAM_DS_SONO_LATENCIA_NOVO_OLD;
            anamnese.PAAM_DS_SONO_LECAMA = anamneseAnterior.PAAM_DS_SONO_LECAMA;
            anamnese.PAAM_DS_SONO_MALLAMPATI = anamneseAnterior.PAAM_DS_SONO_MALLAMPATI;
            anamnese.PAAM_DS_SONO_MALLAMPATI_OLD = anamneseAnterior.PAAM_DS_SONO_MALLAMPATI_OLD;
            anamnese.PAAM_DS_SONO_MEDICAMENTOS = anamneseAnterior.PAAM_DS_SONO_MEDICAMENTOS;
            anamnese.PAAM_DS_SONO_MEDICAMENTOS_OLD = anamneseAnterior.PAAM_DS_SONO_MEDICAMENTOS_OLD;
            anamnese.PAAM_DS_SONO_MOTIVOS_DESPERTA = anamneseAnterior.PAAM_DS_SONO_MOTIVOS_DESPERTA;
            anamnese.PAAM_DS_SONO_MOTIVOS_DESPERTA_OLD = anamneseAnterior.PAAM_DS_SONO_MOTIVOS_DESPERTA_OLD;
            anamnese.PAAM_DS_SONO_MOVE_MEMBRO = anamneseAnterior.PAAM_DS_SONO_MOVE_MEMBRO;
            anamnese.PAAM_DS_SONO_OVERLAP = anamneseAnterior.PAAM_DS_SONO_OVERLAP;
            anamnese.PAAM_DS_SONO_OVERLAP_OLD = anamneseAnterior.PAAM_DS_SONO_OVERLAP_OLD;
            anamnese.PAAM_DS_SONO_PATOLOGIAS = anamneseAnterior.PAAM_DS_SONO_PATOLOGIAS;
            anamnese.PAAM_DS_SONO_PATOLOGIAS_OLD = anamneseAnterior.PAAM_DS_SONO_PATOLOGIAS_OLD;
            anamnese.PAAM_DS_SONO_PESADELO = anamneseAnterior.PAAM_DS_SONO_PESADELO;
            anamnese.PAAM_DS_SONO_PESSOAS = anamneseAnterior.PAAM_DS_SONO_PESSOAS;
            anamnese.PAAM_DS_SONO_POLISONO = anamneseAnterior.PAAM_DS_SONO_POLISONO;
            anamnese.PAAM_DS_SONO_POLISONO_OLD = anamneseAnterior.PAAM_DS_SONO_POLISONO_OLD;
            anamnese.PAAM_DS_SONO_PONDERAL = anamneseAnterior.PAAM_DS_SONO_PONDERAL;
            anamnese.PAAM_DS_SONO_POSICAO_DORMIR = anamneseAnterior.PAAM_DS_SONO_POSICAO_DORMIR;
            anamnese.PAAM_DS_SONO_POSICAO_DORMIR_OLD = anamneseAnterior.PAAM_DS_SONO_POSICAO_DORMIR_OLD;
            anamnese.PAAM_DS_SONO_PRINCIPAL_QUEIXA = anamneseAnterior.PAAM_DS_SONO_PRINCIPAL_QUEIXA;
            anamnese.PAAM_DS_SONO_PRINCIPAL_QUEIXA_OLD = anamneseAnterior.PAAM_DS_SONO_PRINCIPAL_QUEIXA_OLD;
            anamnese.PAAM_DS_SONO_QUANTAS_DESPERTA = anamneseAnterior.PAAM_DS_SONO_QUANTAS_DESPERTA;
            anamnese.PAAM_DS_SONO_QUANTAS_DESPERTA_OLD = anamneseAnterior.PAAM_DS_SONO_QUANTAS_DESPERTA_OLD;
            anamnese.PAAM_DS_SONO_RANGE = anamneseAnterior.PAAM_DS_SONO_RANGE;
            anamnese.PAAM_DS_SONO_REFEICAO_PESADA = anamneseAnterior.PAAM_DS_SONO_REFEICAO_PESADA;
            anamnese.PAAM_DS_SONO_REFLUXO = anamneseAnterior.PAAM_DS_SONO_REFLUXO;
            anamnese.PAAM_DS_SONO_REPARADOR = anamneseAnterior.PAAM_DS_SONO_REPARADOR;
            anamnese.PAAM_DS_SONO_RIGIDEZ_FACE = anamneseAnterior.PAAM_DS_SONO_RIGIDEZ_FACE;
            anamnese.PAAM_DS_SONO_RIGIDEZ_FACE_OUTROS = anamneseAnterior.PAAM_DS_SONO_RIGIDEZ_FACE_OUTROS;
            anamnese.PAAM_DS_SONO_RONCO = anamneseAnterior.PAAM_DS_SONO_RONCO;
            anamnese.PAAM_DS_SONO_ROTINA_FDS = anamneseAnterior.PAAM_DS_SONO_ROTINA_FDS;
            anamnese.PAAM_DS_SONO_SENSACAO_PERNA = anamneseAnterior.PAAM_DS_SONO_SENSACAO_PERNA;
            anamnese.PAAM_DS_SONO_SINTOMAS = anamneseAnterior.PAAM_DS_SONO_SINTOMAS;
            anamnese.PAAM_DS_SONO_SINTOMAS_OLD = anamneseAnterior.PAAM_DS_SONO_SINTOMAS_OLD;
            anamnese.PAAM_DS_SONO_SONANBULISMO = anamneseAnterior.PAAM_DS_SONO_SONANBULISMO;
            anamnese.PAAM_DS_SONO_SONOLENCIA = anamneseAnterior.PAAM_DS_SONO_SONOLENCIA;
            anamnese.PAAM_DS_SONO_SONOLENCIA_DIURNA = anamneseAnterior.PAAM_DS_SONO_SONOLENCIA_DIURNA;
            anamnese.PAAM_DS_SONO_SUDORESE = anamneseAnterior.PAAM_DS_SONO_SUDORESE;
            anamnese.PAAM_DS_SONO_TEMPERATURA = anamneseAnterior.PAAM_DS_SONO_TEMPERATURA;
            anamnese.PAAM_DS_SONO_TEMPO_PEGAR_SONO = anamneseAnterior.PAAM_DS_SONO_TEMPO_PEGAR_SONO;
            anamnese.PAAM_DS_SONO_TEMPO_PEGAR_SONO_OLD = anamneseAnterior.PAAM_DS_SONO_TEMPO_PEGAR_SONO_OLD;
            anamnese.PAAM_DS_SONO_TIPO_RESPIRACAO = anamneseAnterior.PAAM_DS_SONO_TIPO_RESPIRACAO;
            anamnese.PAAM_DS_SONO_TODAS_REFEICOES = anamneseAnterior.PAAM_DS_SONO_TODAS_REFEICOES;
            anamnese.PAAM_DS_SONO_TOSSE = anamneseAnterior.PAAM_DS_SONO_TOSSE;
            anamnese.PAAM_DS_SONO_TURNO = anamneseAnterior.PAAM_DS_SONO_TURNO;
            anamnese.PAAM_DS_SONO_TVCAMA = anamneseAnterior.PAAM_DS_SONO_TVCAMA;
            anamnese.PAAM_DS_SONO_ULTIMO_ALCOOL = anamneseAnterior.PAAM_DS_SONO_ULTIMO_ALCOOL;
            anamnese.PAAM_DS_SONO_ULTIMO_ALCOOL_OLD = anamneseAnterior.PAAM_DS_SONO_ULTIMO_ALCOOL_OLD;
            anamnese.PAAM_DS_SONO_URINA_NOITE = anamneseAnterior.PAAM_DS_SONO_URINA_NOITE;
            anamnese.PAAM_DS_SONO_URINA_NOITE_OLD = anamneseAnterior.PAAM_DS_SONO_URINA_NOITE_OLD;

            anamnese.PAAM_IN_FLAG_MOTIVO_CONSULTA = 1;
            anamnese.PAAM_IN_FLAG__HISTORIA_FAMILIAR = 1;
            anamnese.PAAM_IN_FLAG_HISTORIA_SOCIAL = anamneseAnterior.PAAM_IN_FLAG_HISTORIA_SOCIAL;
            anamnese.PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA = anamneseAnterior.PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA;
            anamnese.PAAM_IN_FLAG_RESPIRATORIO = anamneseAnterior.PAAM_IN_FLAG_RESPIRATORIO;
            anamnese.PAAM_IN_FLAG_ABDOMEM = anamneseAnterior.PAAM_IN_FLAG_ABDOMEM;
            anamnese.PAAM_IN_FLAG_MEDICAMENTO = anamneseAnterior.PAAM_IN_FLAG_MEDICAMENTO;
            anamnese.PAAM_IN_FLAG_MEMBROS_INFERIORES = anamneseAnterior.PAAM_IN_FLAG_MEMBROS_INFERIORES;
            anamnese.PAAM_IN_FLAG_QUEIXA_PRINCIPAL = 1;
            anamnese.PAAM_IN_FLAG_HISTORIA_DOENCA_ATUAL = 1;
            anamnese.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA = anamneseAnterior.PAAM_IN_FLAG_HISTORIA_PROGRESSIVA;
            anamnese.PAAM_IN_FLAG_DIAGNOSTICO_1 = 1;
            anamnese.PAAM_IN_CAMPO_1 = anamneseAnterior.PAAM_IN_CAMPO_1;
            anamnese.PAAM_NM_CAMPO_1 = anamneseAnterior.PAAM_NM_CAMPO_1;
            anamnese.PAAM_DS_CAMPO_1 = anamneseAnterior.PAAM_DS_CAMPO_1;
            anamnese.PAAM_IN_CAMPO_2 = anamneseAnterior.PAAM_IN_CAMPO_2;
            anamnese.PAAM_NM_CAMPO_2 = anamneseAnterior.PAAM_NM_CAMPO_2;
            anamnese.PAAM_DS_CAMPO_2 = anamneseAnterior.PAAM_DS_CAMPO_2;
            anamnese.PAAM_IN_CAMPO_3 = anamneseAnterior.PAAM_IN_CAMPO_3;
            anamnese.PAAM_NM_CAMPO_3 = anamneseAnterior.PAAM_NM_CAMPO_3;
            anamnese.PAAM_DS_CAMPO_3 = anamneseAnterior.PAAM_DS_CAMPO_3;
            anamnese.PAAM_IN_CAMPO_4 = anamneseAnterior.PAAM_IN_CAMPO_4;
            anamnese.PAAM_NM_CAMPO_4 = anamneseAnterior.PAAM_NM_CAMPO_4;
            anamnese.PAAM_DS_CAMPO_4 = anamneseAnterior.PAAM_DS_CAMPO_4;
            anamnese.PAAM_IN_CAMPO_5 = anamneseAnterior.PAAM_IN_CAMPO_5;
            anamnese.PAAM_NM_CAMPO_5 = anamneseAnterior.PAAM_NM_CAMPO_5;
            anamnese.PAAM_DS_CAMPO_5 = anamneseAnterior.PAAM_DS_CAMPO_5;
            anamnese.PAAM_IN_ALTERADA = 0;
            return anamnese;
        }

        public PACIENTE_EXAME_FISICOS RemontarFisico(PACIENTE_EXAME_FISICOS fisicoAnterior)
        {
            PACIENTE_EXAME_FISICOS fisico = new PACIENTE_EXAME_FISICOS();
            fisico.ASSI_CD_ID = fisicoAnterior.ASSI_CD_ID;
            fisico.PAEF_CD_ID = fisicoAnterior.PAEF_CD_ID;
            fisico.PACI_CD_ID = fisicoAnterior.PACI_CD_ID;
            fisico.PAEF_DT_DATA = fisicoAnterior.PAEF_DT_DATA;
            fisico.PAEF_IN_ATIVO = 1;
            fisico.USUA_CD_ID = fisicoAnterior.USUA_CD_ID;
            fisico.PACO_CD_ID = fisicoAnterior.PACO_CD_ID;
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
            fisico.PAEF_TX_RESULTADOS = fisicoAnterior.PAEF_TX_RESULTADOS;
            fisico.PAEF_DT_ORIGINAL = fisicoAnterior.PAEF_DT_ORIGINAL;
            fisico.PAEF_DS_ALCOOLISMO_LONG = fisicoAnterior.PAEF_DS_ALCOOLISMO_LONG;
            fisico.PAEF_DS_ALERGICO_LONG = fisicoAnterior.PAEF_DS_ALERGICO_LONG;
            fisico.PAEF_DS_EXERCICIO_FISICO_LONG = fisicoAnterior.PAEF_DS_EXERCICIO_FISICO_LONG;
            fisico.PAEF_DS_MARCAPASSO_LONG = fisicoAnterior.PAEF_DS_MARCAPASSO_LONG;
            fisico.PAEF_DS_ONCOLOGICO_LONG = fisicoAnterior.PAEF_DS_ONCOLOGICO_LONG;
            fisico.PAEF_DS_TABAGISMO_LONG = fisicoAnterior.PAEF_DS_TABAGISMO_LONG;
            fisico.PAEF_DS_FICHA_AVALIACAO = fisicoAnterior.PAEF_DS_FICHA_AVALIACAO;
            fisico.PAEF_NM_TIPO_SANGUE = fisicoAnterior.PAEF_NM_TIPO_SANGUE;
            return fisico;
        }

        public DTO_Paciente_Consulta MontarPacienteConsultaDTOObj(PACIENTE_CONSULTA l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Paciente_Consulta()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    PACO_CD_ID = l.PACO_CD_ID,
                    PACO_DT_CONSULTA = l.PACO_DT_CONSULTA,
                    PACO_DT_DUMMY = l.PACO_DT_DUMMY,
                    PACO_DT_PROXIMA = l.PACO_DT_PROXIMA,
                    PACO_HR_FINAL = l.PACO_HR_FINAL,
                    PACO_HR_INICIO = l.PACO_HR_INICIO,
                    PACO_IN_ATIVO = l.PACO_IN_ATIVO,
                    PACO_IN_CONFIRMADA = l.PACO_IN_CONFIRMADA,
                    PACO_IN_ENCERRADA = l.PACO_IN_ENCERRADA,
                    PACO_IN_RECEBE = l.PACO_IN_RECEBE,
                    PACO_IN_RECORRENTE = l.PACO_IN_RECORRENTE,
                    PACO_IN_TIPO = l.PACO_IN_TIPO,
                    PACO_TX_JUSTIFICATIVA_CANCELA = l.PACO_TX_JUSTIFICATIVA_CANCELA,
                    PACI_CD_ID = l.PACI_CD_ID,
                    PACO_TX_RESUMO = l.PACO_TX_RESUMO,
                    USUA_CD_ID = l.USUA_CD_ID,
                    VACO_CD_ID = l.VACO_CD_ID,
                };
                return mediDTO;
            }

        }

        public async Task<Int32> EnviarEMailConsulta(PACIENTE_CONSULTA consulta, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
            USUARIO usuario = usuApp.GetItemById(paciente.USUA_CD_ID.Value);
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = null;
                if (tipo == 1)
                {
                    template = temApp.GetByCode("CRIACONS", idAss);
                }
                else if (tipo == 2)
                {
                    template = temApp.GetByCode("ALTCONS", idAss);
                }
                else if (tipo == 3)
                {
                    template = temApp.GetByCode("CONFCONS", idAss);
                }
                else if (tipo == 4)
                {
                    template = temApp.GetByCode("CANCCONS", idAss);
                }
                else if (tipo == 5)
                {
                    template = temApp.GetByCode("CRMEDCONS", idAss);
                }
                else if (tipo == 6)
                {
                    template = temApp.GetByCode("CFMEDCONS", idAss);
                }
                else if (tipo == 7)
                {
                    template = temApp.GetByCode("CCMEDCONS", idAss);
                }

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;
                String cor = template.TEEM_TX_CORPO;
                if (tipo < 5)
                {
                    if (cab.Contains("{nome}"))
                    {
                        cab = cab.Replace("{nome}", paciente.PACI_NM_NOME);
                    }
                }
                else
                {
                    if (cab.Contains("{medico}"))
                    {
                        cab = cab.Replace("{medico}", usuario.USUA_NM_NOME);
                    }
                }

                // Prepara assinatura
                String assinatura = String.Empty;
                if (tipo < 5)
                {
                    String classe = String.Empty;
                    if (usuario.TIPO_CARTEIRA_CLASSE != null)
                    {
                        classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                    }
                    assinatura = "<b>" + usuario.USUA_NM_NOME + "</b><br />";
                    if (usuario.ESPECIALIDADE != null)
                    {
                        assinatura += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
                    }
                    else
                    {
                        assinatura += usuario.USUA_NM_ESPECIALIDADE + "<br />";
                    }
                    assinatura += classe + "  CPF: " + usuario.USUA_NR_CPF + "<br />";
                }
                else
                {
                    assinatura = "Enviado por <b>WebDoctor</b><br />";
                }

                // Prepara corpo da mensagem
                String texto = template.TEEM_TX_CORPO;
                if (texto.Contains("{medico}"))
                {
                    texto = texto.Replace("{medico}", usuario.USUA_NM_NOME);
                }
                if (texto.Contains("{nome}"))
                {
                    texto = texto.Replace("{nome}", paciente.PACI_NM_NOME);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", consulta.PACO_DT_CONSULTA.ToLongDateString());
                }
                if (texto.Contains("{inicio}"))
                {
                    texto = texto.Replace("{inicio}", consulta.PACO_HR_INICIO.ToString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", consulta.PACO_HR_FINAL.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME);
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", consulta.PACO_TX_JUSTIFICATIVA_CANCELA);
                }
                String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);
                List<AttachmentModel> models = new List<AttachmentModel>();

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                if (tipo < 5)
                {
                    mensagem.ASSUNTO = "Envio de Mensagem para Paciente - " + paciente.PACI_NM_NOME + " - " + DateTime.Now.ToString();
                }
                else
                {
                    mensagem.ASSUNTO = "Envio de Mensagem para Médico - " + usuario.USUA_NM_NOME + " - " + DateTime.Now.ToString();
                }
                mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME + " - Consulta";
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
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                    return 0;
                }

                // Grava mensagem enviada
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = paciente.PACI__CD_ID;
                if (tipo < 5)
                {
                    mens.MODELO = paciente.PACI_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Consulta: " + paciente.PACI_NM_NOME;
                    mens.PACI_CD_ID = paciente.PACI__CD_ID;
                }
                else
                {
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.MENS_NM_NOME = "Mensagem para Médico - Consulta: " + usuario.USUA_NM_NOME;
                    mens.PACI_CD_ID = null;
                }
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de Consulta de Paciente - " + paciente.PACI_NM_NOME);

                // Sucesso
                return 1;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return 1;
            }
        }

        public Int32 EnviarSMSConsulta(PACIENTE_CONSULTA consulta, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
            USUARIO usuario = usuApp.GetItemById(consulta.USUA_CD_ID.Value);

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_SMS template = null;
                if (tipo == 1)
                {
                    template = smsApp.GetByCode("CRIACONS", idAss);
                }
                else if (tipo == 2)
                {
                    template = smsApp.GetByCode("ALTCONS", idAss);
                }
                else if (tipo == 3)
                {
                    template = smsApp.GetByCode("CONFCONS", idAss);
                }
                else if (tipo == 4)
                {
                    template = smsApp.GetByCode("CANCCONS", idAss);
                }

                // Prepara assinatura
                String classe = String.Empty;
                if (usuario.TIPO_CARTEIRA_CLASSE != null)
                {
                    classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                }
                String assinatura = usuario.USUA_NM_NOME + " - ";
                assinatura += usuario.USUA_NM_ESPECIALIDADE + " - ";
                assinatura += classe + " - CPF: " + usuario.USUA_NR_CPF;

                // Prepara corpo da mensagem
                String texto = template.TSMS_TX_CORPO;
                if (texto.Contains("{nome}"))
                {
                    texto = texto.Replace("{nome}", paciente.PACI_NM_NOME);
                }
                if (texto.Contains("{medico}"))
                {       
                    texto = texto.Replace("{medico}", usuario.USUA_NM_NOME);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", consulta.PACO_DT_CONSULTA.ToLongDateString());
                }
                if (texto.Contains("{inicio}"))
                {
                    texto = texto.Replace("{inicio}", consulta.PACO_HR_INICIO.ToString());
                }
                if (texto.Contains("{classe}"))
                {
                    texto = texto.Replace("{classe}", assinatura);
                }
                String smsBody = texto + ".";

                // Carraga configuracao
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                // Decriptografa chaves
                String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
                String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

                // Monta token
                String text = login + ":" + senha;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // inicia processo
                String resposta = String.Empty;

                // processa envio
                String listaDest = "55" + Regex.Replace(paciente.PACI_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

                // Grava mensagem enviada
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = paciente.PACI__CD_ID;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem SMS para Paciente - Consulta - Marcação: " + paciente.PACI_NM_NOME;
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = smsBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Marcação de Consulta de Paciente - SMS - " + paciente.PACI_NM_NOME);

                // Sucesso
                Session["NivelPaciente"] = 1;
                return 1;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return 1;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CancelarConsulta(Int32 id, String justificativa)
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

                // Processa cancelamento
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuarioLogado = usuApp.GetItemById(paciente.USUA_CD_ID.Value);
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);
                objetoAntes = (PACIENTE)Session["Paciente"];
                item.PACO_IN_CONFIRMADA = 2;
                item.PACO_TX_JUSTIFICATIVA_CANCELA = justificativa;
                Int32 volta = baseApp.ValidateEditConsultaConfirma(item);

                // Acerta estado
                Session["PacienteAlterada"] = 1;
                Session["NivelPaciente"] = 3;
                Session["ListaConsultasGeral"] = null;
                Session["ConsultasAlterada"] = 1;
                Session["ListaConfirma"] = null;
                Session["ListaConsultaAberta"] = null;

                // Recupera paciente
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Paciente_Consulta dto = MontarPacienteConsultaDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Consulta - Cancelamento",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Grava historico
                PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.PACI_CD_ID = item.PACI_CD_ID;
                hist.PAHI_DT_DATA = DateTime.Now;
                hist.PAHI_IN_TIPO = 10;
                hist.PAHI_IN_CHAVE = item.PACO_CD_ID;
                hist.PAHI_NM_OPERACAO = "Paciente - Cancelamento de Consulta";
                hist.PAHI_DS_DESCRICAO = "Paciente " + pac.PACI_NM_NOME + " - Consulta cancelada " + item.PACO_DT_CONSULTA.ToShortDateString() + " - Justificativa: " + justificativa;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);
                Session["ListaConsultasGeral"] = null;
                Session["ConsultasAlterada"] = 1;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A consulta do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " marcada para " + item.PACO_DT_CONSULTA.ToLongDateString() + " foi cancelada com sucesso";
                Session["MensPaciente"] = 888;

                // Envia mensagem
                if (pac.PACI_NM_EMAIL != null)
                {
                    Int32 voltaCons = await EnviarEMailConsultaEspecial(item, 4);
                }
                if (pac.PACI_NR_CELULAR != null)
                {
                    Int32 voltaCons = await EnviarEMailConsultaEspecial(item, 4);
                }
                if (usuarioLogado.USUA_NM_EMAIL != null)
                {
                    Int32 voltaCons = await EnviarEMailConsultaEspecial(item, 7);
                }

                // Retorno
                Session["Consultas"] = null;
                if ((Int32)Session["TipoSolicitacao"] == 1)
                {
                    if ((Int32)Session["VoltaAtestado"] == 1)
                    {
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                    return RedirectToAction("VoltarAnexoPaciente");
                }
                if ((Int32)Session["VoltaTelaEncerra"] == 1)
                {
                    return RedirectToAction("MontarTelaEncerrarConsulta", "Financeiro");
                }
                if ((Int32)Session["VoltaConfirmar"] == 1)
                {
                    return RedirectToAction("ConfirmarCancelarConsulta", "Paciente");
                }
                if ((Int32)Session["VoltaCalendario"] == 1)
                {
                    return RedirectToAction("VerCalendarioConsulta");
                }
                return RedirectToAction("MontarTelaConsultas", "Paciente");
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

        public async Task<Int32> EnviarEMailConsultaEspecial(PACIENTE_CONSULTA consulta, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE paciente = baseApp.GetItemById(consulta.PACI_CD_ID);
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = null;
                if (tipo == 1)
                {
                    template = temApp.GetByCode("CRIACONS", idAss);
                }
                else if (tipo == 2)
                {
                    template = temApp.GetByCode("ALTCONS", idAss);
                }
                else if (tipo == 3)
                {
                    template = temApp.GetByCode("CONFCONS", idAss);
                }
                else if (tipo == 4)
                {
                    template = temApp.GetByCode("CANCCONS", idAss);
                }
                else if (tipo == 5)
                {
                    template = temApp.GetByCode("CRMEDCONS", idAss);
                }
                else if (tipo == 6)
                {
                    template = temApp.GetByCode("CFMEDCONS", idAss);
                }
                else if (tipo == 7)
                {
                    template = temApp.GetByCode("CCMEDCONS", idAss);
                }

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;
                String cor = template.TEEM_TX_CORPO;
                if (tipo < 5)
                {
                    if (cab.Contains("{nome}"))
                    {
                        cab = cab.Replace("{nome}", paciente.PACI_NM_NOME);
                    }
                }
                else
                {
                    if (cab.Contains("{medico}"))
                    {
                        cab = cab.Replace("{medico}", usuario.USUA_NM_NOME);
                    }
                }

                // Prepara assinatura
                String assinatura = String.Empty;
                if (tipo < 5)
                {
                    String classe = String.Empty;
                    if (usuario.TIPO_CARTEIRA_CLASSE != null)
                    {
                        classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                    }
                    assinatura = "<b>" + usuario.USUA_NM_NOME + "</b><br />";
                    if (usuario.ESPECIALIDADE != null)
                    {
                        assinatura += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
                    }
                    else
                    {
                        assinatura += usuario.USUA_NM_ESPECIALIDADE + "<br />";
                    }
                    assinatura += classe + "  CPF: " + usuario.USUA_NR_CPF + "<br />";
                }
                else
                {
                    assinatura = "Enviado por <b>WebDoctor</b><br />";
                }

                // Prepara corpo da mensagem
                String texto = template.TEEM_TX_CORPO;
                if (texto.Contains("{medico}"))
                {
                    texto = texto.Replace("{medico}", usuario.USUA_NM_NOME);
                }
                if (texto.Contains("{nome}"))
                {
                    texto = texto.Replace("{nome}", paciente.PACI_NM_NOME);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", consulta.PACO_DT_CONSULTA.ToLongDateString());
                }
                if (texto.Contains("{inicio}"))
                {
                    texto = texto.Replace("{inicio}", consulta.PACO_HR_INICIO.ToString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", consulta.PACO_HR_FINAL.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME);
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", consulta.PACO_TX_JUSTIFICATIVA_CANCELA);
                }
                String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);
                List<AttachmentModel> models = new List<AttachmentModel>();

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                if (tipo < 5)
                {
                    mensagem.ASSUNTO = "Envio de Mensagem para Paciente - " + paciente.PACI_NM_NOME + " - " + DateTime.Now.ToString();
                }
                else
                {
                    mensagem.ASSUNTO = "Envio de Mensagem para Médico - " + usuario.USUA_NM_NOME + " - " + DateTime.Now.ToString();
                }
                mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME + " - Consulta";
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

                // Grava mensagem enviada
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = paciente.PACI__CD_ID;
                if (tipo < 5)
                {
                    mens.MODELO = paciente.PACI_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Consulta: " + paciente.PACI_NM_NOME;
                    mens.PACI_CD_ID = paciente.PACI__CD_ID;
                }
                else
                {
                    mens.MODELO = usuario.USUA_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_CAMPANHA = usuario.USUA_NM_EMAIL;
                    mens.MENS_NM_NOME = "Mensagem para Médico - Consulta: " + usuario.USUA_NM_NOME;
                    mens.PACI_CD_ID = null;
                }
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de Consulta de Paciente - " + paciente.PACI_NM_NOME);

                // Sucesso
                return 1;
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
                return 1;
            }
        }

        [HttpGet]
        public ActionResult EnviarInformacaoConsulta(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Recupera paciente
                PACIENTE cont = (PACIENTE)Session["UserCredentials"];
                USUARIO usuario = usuApp.GetItemById(cont.USUA_CD_ID.Value);
                Session["Paciente"] = cont;
                ViewBag.Paciente = cont;
                ViewBag.NomePaciente = cont.PACI_NM_NOME;
                ViewBag.Profissional = cont.USUARIO.USUA_NM_NOME;
                ViewBag.EMail = cont.USUARIO.USUA_NM_EMAIL;

                // Recupera Consulta
                PACIENTE_CONSULTA cons = baseApp.GetConsultaById(id);
                ViewBag.Data = cons.PACO_DT_CONSULTA.ToShortDateString();
                ViewBag.Inicio = cons.PACO_HR_INICIO.ToString();
                ViewBag.Final = cons.PACO_HR_FINAL.ToString();

                if (Session["MensArea"] != null)
                {
                    if ((Int32)Session["MensArea"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0729", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AREA_PACIENTE_ENVIO_PREVIA", "AreaPaciente", "EnviarInformacaoConsulta");
                
                AREA_PACIENTE area = new AREA_PACIENTE();
                AreaPacienteViewModel vm = Mapper.Map<AREA_PACIENTE, AreaPacienteViewModel>(area);
                vm.AREA_DT_CONSULTA = cons.PACO_DT_CONSULTA;
                vm.AREA_DT_ENTRADA = DateTime.Now;
                vm.AREA_HR_FINAL = cons.PACO_HR_FINAL;
                vm.AREA_HR_INICIO = cons.PACO_HR_INICIO;
                vm.AREA_IN_ATIVO = 1;
                vm.AREA_IN_TIPO = 2;
                vm.AREA_IN_PROCESSADA = 0;
                vm.AREA_IN_VISTA = 0;
                vm.AREA_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                vm.AREA_NM_TITULO = "Envio de informações de consulta - " + "Paciente: " + cont.PACI_NM_NOME.ToUpper();
                vm.ASSI_CD_ID = cont.ASSI_CD_ID;
                vm.PACI_CD_ID = cont.PACI__CD_ID;
                vm.USUA_CD_ID = cont.USUA_CD_ID;
                vm.NOME_PACIENTE = cont.PACI_NM_NOME;
                vm.NOME_PROFISSIONAL = cont.USUARIO.USUA_NM_NOME;
                vm.EMAIL_PROFISSIONAL = cont.USUARIO.USUA_NM_EMAIL;
                vm.HORARIO = cons.PACO_HR_INICIO.ToString() + " até " + cons.PACO_HR_FINAL.ToString();

                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EnviarInformacaoConsulta(AreaPacienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            PACIENTE cont = (PACIENTE)Session["UserCredentials"];
            USUARIO usuario = usuApp.GetItemById(cont.USUA_CD_ID.Value);
            Int32 idAss = cont.ASSI_CD_ID;
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AREA_TX_CONTEUDO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AREA_TX_CONTEUDO);

                    // Critica basica
                    if (vm.AREA_TX_CONTEUDO == null)
                    {
                        Session["MensArea"] = 3;
                        return View(vm);
                    }

                    // Monta Area
                    AREA_PACIENTE item = Mapper.Map<AreaPacienteViewModel, AREA_PACIENTE>(vm);

                    // Executa criação
                    Int32 volta = areaApp.ValidateCreate(item, usuario);
                    Session["IdArea"] = item.AREA_CD_ID;

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/AreaPaciente/" + item.AREA_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    if (Session["FileQueueArea"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueArea"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                Int32 volta1 = UploadFileQueueArea(file);
                            }
                        }
                        Session["FileQueueArea"] = null;
                    }

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Area_Paciente dto = MontarAreaPacienteDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Paciente - Envio de Informação de Consulta",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Mensagem de cadastramento
                    if (usuario.USUA_NM_EMAIL != null)
                    {
                        var voltaCons = await EnviarEMailAvisoArea(usuario, 1);
                    }

                    // Mensagem
                    Session["MsgCRUD"] = "O envio de informações de " + cont.PACI_NM_NOME.ToUpper() + " para o profissional " + usuario.USUA_NM_NOME.ToUpper() + " foi realizado com sucesso. Identificador do envio: " + item.AREA_GU_IDENTIFICADOR;
                    Session["MensFC"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                    return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
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
            Session["FileQueueAreaPaciente"] = queue;
        }

        [HttpPost]
        public Int32 UploadFileQueueArea(FileQueue file)
        {
            try
            {
                if (file == null)
                {
                    Session["MensArea"] = 5;
                    return 1;
                }

                // Recupera paciente
                PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
                USUARIO item = usuApp.GetItemById(paciente.USUA_CD_ID.Value);
                Int32 idNot = paciente.PACI__CD_ID;
                Int32 idAss = paciente.ASSI_CD_ID;
                Int32 idArea = (Int32)Session["IdArea"];
                AREA_PACIENTE area = areaApp.GetItemById(idArea);

                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensArea"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensArea"] = 7;
                    return 3;
                }

                // Recupera tipo de arquivo
                String extensao = Path.GetExtension(fileName);
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensArea"] = 8;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" +idAss.ToString() + "/AreaPaciente/" + idArea.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                AREA_PACIENTE_ANEXO foto = new AREA_PACIENTE_ANEXO();
                foto.APAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.APAN_DT_ANEXO = DateTime.Today;
                foto.APAN_IN_ATIVO = 1;
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
                foto.APAN_IN_TIPO = tipo;
                foto.APAN_NM_TITULO = fileName;
                foto.AREA_CD_ID = idArea;
                area.AREA_PACIENTE_ANEXO.Add(foto);
                Int32 volta = areaApp.ValidateEdit(area, item);

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Area_Paciente_Anexo dto = MontarAreaPacienteAnexoDTOObj(foto);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = item.ASSI_CD_ID,
                    USUA_CD_ID = item.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Anexação de arquivos à informação da área do paciente",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return 0;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return 0;
            }
        }

        public DTO_Area_Paciente MontarAreaPacienteDTOObj(AREA_PACIENTE l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Area_Paciente()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    PACI_CD_ID = l.PACI_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    AREA_CD_ID = l.ASSI_CD_ID,
                    AREA_DT_CONSULTA = l.AREA_DT_CONSULTA,
                    AREA_HR_FINAL = l.AREA_HR_FINAL,
                    AREA_HR_INICIO = l.AREA_HR_INICIO,
                    AREA_DT_ENTRADA = l.AREA_DT_ENTRADA,
                    AREA_IN_ATIVO = l.AREA_IN_ATIVO,
                    AREA_IN_TIPO = l.AREA_IN_TIPO,
                    AREA_NM_TITULO = l.AREA_NM_TITULO,
                    AREA_TX_CONTEUDO = l.AREA_TX_CONTEUDO,
                    AREA_GU_IDENTIFICADOR = l.AREA_GU_IDENTIFICADOR,
                };
                return mediDTO;
            }

        }

        public DTO_Area_Paciente_Anexo MontarAreaPacienteAnexoDTOObj(AREA_PACIENTE_ANEXO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Area_Paciente_Anexo()
                {
                    APAN_AQ_ARQUIVO = l.APAN_AQ_ARQUIVO,
                    APAN_DT_ANEXO = l.APAN_DT_ANEXO,
                    APAN_CD_ID = l.APAN_CD_ID,
                    APAN_IN_ATIVO = l.APAN_IN_ATIVO,
                    APAN_IN_TIPO = l.APAN_IN_TIPO,
                    APAN_NM_TITULO = l.APAN_NM_TITULO,
                    AREA_CD_ID = l.AREA_CD_ID,
                    ASSI_CD_ID = l.ASSI_CD_ID,
                };
                return mediDTO;
            }

        }

        public async Task<Int32> EnviarEMailAvisoArea(USUARIO paciente, Int32 tipo)
        {
            // Recupera informações
            PACIENTE cont = (PACIENTE)Session["UserCredentials"];
            Int32 idAss = cont.ASSI_CD_ID;
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            AREA_PACIENTE area = areaApp.GetItemById((Int32)Session["IdArea"]);
            USUARIO usu = usuApp.GetItemById(cont.USUA_CD_ID.Value);

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = null;
                if (tipo == 1)
                {
                    template = temApp.GetByCode("INFOPAC", idAss);
                }
                if (tipo == 2)
                {
                    template = temApp.GetByCode("INFODOC", idAss);
                }

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;
                if (cab.Contains("{usuario}"))
                {
                    cab = cab.Replace("{usuario}", usu.USUA_NM_NOME.ToUpper());
                }
                if (cab.Contains("{nome}"))
                {
                    cab = cab.Replace("{nome}", usu.USUA_NM_NOME.ToUpper());
                }

                // Prepara corpo da mensagem
                String texto = template.TEEM_TX_CORPO;
                String rodape = template.TEEM_TX_DADOS;
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", cont.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", DateTime.Today.Date.ToLongDateString());
                }
                if (texto.Contains("{nomeDoc}"))
                {
                    texto = texto.Replace("{nomeDoc}", area.AREA_NM_EXAME.ToUpper());
                }
                String emailBody = cab + "<br />" + texto + "<br /><br />" + rodape;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);
                List<AttachmentModel> models = new List<AttachmentModel>();

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                if (tipo == 1)
                {
                    mensagem.ASSUNTO = "Paciente - " + cont.PACI_NM_NOME.ToUpper() + " - Envio de Informações Relevantes";
                }
                if (tipo == 2)
                {
                    mensagem.ASSUNTO = "Paciente - " + cont.PACI_NM_NOME.ToUpper() + " - Envio de Documento";
                }
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = paciente.USUA_NM_EMAIL;
                mensagem.NOME_EMISSOR_AZURE = emissor;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = cont.PACI_NM_NOME;
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
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    return 0;
                }
                
                // Grava mensagem enviada
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.USUA_NM_NOME;
                mens.ID = paciente.USUA_CD_ID;
                mens.MODELO = paciente.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.USUA_NM_EMAIL;
                if (tipo == 1)
                {
                    mens.MENS_NM_NOME = "Mensagem para Profissional - Envio de Informações do Paciente: " + cont.PACI_NM_NOME.ToUpper();
                }
                if (tipo == 2)
                {
                    mens.MENS_NM_NOME = "Mensagem para Profissional - Envio de Documento: " + area.AREA_NM_EXAME.ToUpper() + " do paciente: " + cont.PACI_NM_NOME.ToUpper();
                }
                mens.PACI_CD_ID = cont.PACI__CD_ID;
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, paciente, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Envio de Informações do Paciente - " + cont.PACI_NM_NOME.ToUpper());

                // Sucesso
                return 1;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return 1;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaEnvioDocumento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Recupera paciente
                PACIENTE cont = (PACIENTE)Session["UserCredentials"];
                USUARIO usuario = usuApp.GetItemById(cont.USUA_CD_ID.Value);
                Session["Paciente"] = cont;
                Session["IdPaciente"] = cont.PACI__CD_ID;
                ViewBag.Paciente = cont;
                ViewBag.NomePaciente = cont.PACI_NM_NOME;
                ViewBag.Profissional = cont.USUARIO.USUA_NM_NOME;
                ViewBag.EMail = cont.USUARIO.USUA_NM_EMAIL;

                // Monta listas
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Contrato de Locação Assinado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Distrato de Loação Assinado", Value = "2" });
                tipo.Add(new SelectListItem() { Text = "Resultado de Exames", Value = "3" });
                tipo.Add(new SelectListItem() { Text = "Documentos Diversos", Value = "4" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                ViewBag.TipoExame = new SelectList(CarregaTipoExame(), "TIEX_CD_ID", "TIEX_NM_NOME");
                ViewBag.Labs = new SelectList(CarregaLaboratorio(), "LABS_CD_ID", "LABS_NM_NOME");
                ViewBag.Locacoes = new SelectList(CarregarLocacao().Where(p => p.PACI_CD_ID == cont.PACI__CD_ID), "LOCA_CD_ID", "LOCA_NM_TITULO");

                if (Session["MensArea"] != null)
                {
                    if ((Int32)Session["MensArea"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0730", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0731", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0732", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0733", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0734", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensArea"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0735", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "AREA_PACIENTE_ENVIO_PREVIA", "AreaPaciente", "EnviarInformacaoConsulta");
                
                AREA_PACIENTE area = new AREA_PACIENTE();
                AreaPacienteViewModel vm = Mapper.Map<AREA_PACIENTE, AreaPacienteViewModel>(area);
                vm.AREA_DT_CONSULTA = null;
                vm.AREA_DT_ENTRADA = DateTime.Now;
                vm.AREA_HR_FINAL = null;
                vm.AREA_HR_INICIO = null;
                vm.AREA_IN_ATIVO = 1;
                vm.AREA_IN_TIPO = 3;
                area.AREA_IN_PROCESSADA = 0;
                area.AREA_IN_VISTA = 0;
                vm.AREA_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                vm.AREA_NM_TITULO = "Envio de Documentos - " + "Paciente: " + cont.PACI_NM_NOME.ToUpper();
                vm.ASSI_CD_ID = cont.ASSI_CD_ID;
                vm.PACI_CD_ID = cont.PACI__CD_ID;
                vm.USUA_CD_ID = cont.USUA_CD_ID;
                vm.NOME_PACIENTE = cont.PACI_NM_NOME;
                vm.NOME_PROFISSIONAL = cont.USUARIO.USUA_NM_NOME;
                vm.EMAIL_PROFISSIONAL = cont.USUARIO.USUA_NM_EMAIL;
                vm.HORARIO = null;
                vm.AREA_IN_TIPO_EXAME = 0;

                return View(vm);
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> MontarTelaEnvioDocumento(AreaPacienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            PACIENTE cont = (PACIENTE)Session["UserCredentials"];
            USUARIO usuario = usuApp.GetItemById(cont.USUA_CD_ID.Value);
            Int32 idAss = cont.ASSI_CD_ID;
            var tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Contrato de Locação Assinado", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Distrato de Loação Assinado", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Resultado de Exames", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Documentos Diversos", Value = "4" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.TipoExame = new SelectList(CarregaTipoExame(), "TIEX_CD_ID", "TIEX_NM_NOME");
            ViewBag.Labs = new SelectList(CarregaLaboratorio(), "LABS_CD_ID", "LABS_NM_NOME");
            ViewBag.Locacoes = new SelectList(CarregarLocacao().Where(p => p.PACI_CD_ID == cont.PACI__CD_ID), "LOCA_CD_ID", "LOCA_NM_TITULO");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.AREA_TX_CONTEUDO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AREA_TX_CONTEUDO);
                    vm.AREA_NM_EXAME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.AREA_NM_EXAME);

                    // Critica basica
                    if (vm.AREA_TX_CONTEUDO == null)
                    {
                        Session["MensArea"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0730", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Demais criticas
                    if (vm.AREA_IN_TIPO_EXAME == 0 || vm.AREA_IN_TIPO_EXAME == null)
                    {
                        Session["MensArea"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0731", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.AREA_NM_EXAME == null)
                    {
                        Session["MensArea"] = 5;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0732", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.AREA_IN_TIPO_EXAME == 3)
                    {
                        if (vm.TIEX_CD_ID == 0 || vm.TIEX_CD_ID == null)
                        {
                            Session["MensArea"] = 6;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0733", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.AREA_DT_DATA_EXAME.Value.Date > DateTime.Today.Date)
                        {
                            Session["MensArea"] = 9;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0736", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.AREA_IN_TIPO_EXAME == 2 || vm.AREA_IN_TIPO_EXAME == 1)
                    {
                        if (vm.LOCA_CD_ID == 0 || vm.LOCA_CD_ID == null)
                        {
                            Session["MensArea"] = 7;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0734", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (Session["FileQueueAreaPaciente"] == null)
                    {
                        Session["MensArea"] = 8;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0735", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Monta Area
                    AREA_PACIENTE item = Mapper.Map<AreaPacienteViewModel, AREA_PACIENTE>(vm);

                    // Executa criação
                    Int32 volta = areaApp.ValidateCreate(item, usuario);
                    Session["IdArea"] = item.AREA_CD_ID;

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/AreaPaciente/" + item.AREA_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    if (Session["FileQueueAreaPaciente"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAreaPaciente"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                Int32 volta1 = UploadFileQueueArea(file);
                            }
                        }
                        Session["FileQueueAreaPaciente"] = null;
                    }

                    // Grava historico
                    PACIENTE_HISTORICO hist = new PACIENTE_HISTORICO();
                    PACIENTE pac1 = baseApp.GetItemById(vm.PACI_CD_ID.Value);
                    hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.PACI_CD_ID = item.PACI_CD_ID;
                    hist.PAHI_DT_DATA = DateTime.Now;
                    hist.PAHI_IN_TIPO = 14;
                    hist.PAHI_IN_CHAVE = item.AREA_CD_ID;
                    hist.PAHI_NM_OPERACAO = "Paciente - Envio de Documento";
                    hist.PAHI_DS_DESCRICAO = "Documento: " + item.AREA_NM_EXAME.ToUpper() + " - Paciente: " + pac1.PACI_NM_NOME + " - Enviado em: " + item.AREA_DT_ENTRADA.Value.ToShortDateString();
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Mensagem de aviso
                    if (usuario.USUA_NM_EMAIL != null)
                    {
                        var voltaCons = await EnviarEMailAvisoArea(usuario, 2);
                    }

                    // Mensagem
                    Session["MsgCRUD"] = "O documento " + vm.AREA_NM_EXAME.ToUpper() + " do paciente " + cont.PACI_NM_NOME.ToUpper() + " para o profissional " + usuario.USUA_NM_NOME.ToUpper() + " foi enviado com sucesso. Identificador do envio: " + item.AREA_GU_IDENTIFICADOR;
                    Session["MensArea"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaAreaPaciente");
                }
                catch (Exception ex)
                {
                    Session["MensagemLogin"] = 100;
                    Session["MensagemErro"] = ex.Message;
                    Session["Excecao"] = ex;
                    Session["TipoVolta"] = 2;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    Session["VoltaExcecao"] = "AreaPaciente";
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                    return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<TIPO_EXAME> CarregaTipoExame()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_EXAME> conf = new List<TIPO_EXAME>();
                if (Session["TipoExames"] == null)
                {
                    conf = baseApp.GetAllTipoExame(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoExameAlterada"] == 1)
                    {
                        conf = baseApp.GetAllTipoExame(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_EXAME>)Session["TipoExames"];
                    }
                }
                Session["TipoExames"] = conf;
                Session["TipoExameAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return null;
            }
        }

        public List<LABORATORIO> CarregaLaboratorio()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LABORATORIO> conf = new List<LABORATORIO>();
                if (Session["Laboratorios"] == null)
                {
                    conf = baseApp.GetAllLaboratorios(idAss);
                }
                else
                {
                    if ((Int32)Session["LaboratorioAlterada"] == 1)
                    {
                        conf = baseApp.GetAllLaboratorios(idAss);
                    }
                    else
                    {
                        conf = (List<LABORATORIO>)Session["Laboratorios"];
                    }
                }
                Session["Laboratorios"] = conf;
                Session["LaboratorioAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return null;
            }
        }

        public List<LOCACAO> CarregarLocacao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOCACAO> conf = new List<LOCACAO>();
                if (Session["Locacoes"] == null)
                {
                    conf = locaApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["LocacaoAlterada"] == 1)
                    {
                        conf = locaApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<LOCACAO>)Session["Locacoes"];
                    }
                }
                Session["LocacaoAlterada"] = 0;
                Session["Locacoes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return null;
            }
        }

        public async Task<ActionResult> ErroGeralAreaPaciente()
        {
            try
            {
                // Recupera Assinante e configuração
                CONFIGURACAO conf = null;
                PACIENTE cont = (PACIENTE)Session["UserCredentials"];
                USUARIO usuario = usuApp.GetItemById(cont.USUA_CD_ID.Value);
                Int32 idAss = cont.ASSI_CD_ID;
                ASSINANTE assi = assApp.GetItemById(idAss);

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
                ModelState.AddModelError("", "O processamento do WebDoctorPro detectou uma falha. Uma mensagem urgente já foi enviada ao suporte com as informações abaixo e logo voce receberá a resposta. Se desejar reenvie a mensagem usando os botões disponíveis nesta página." + " ID do envio: " + (String)Session["IdMail"]);
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7.pdf";
                return View(exc);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Exceção";
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("MontaTelaAreaPaciente", "AreaPaciente");
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
                assi = assApp.GetItemById(idAss);
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
            mensagem.NOME_EMISSOR = "WebDoctorPro";
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
                Int32 voltaX = grava.GravarLogExcecao(ex, "Suporte", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return 0;
            }
            return 0;
        }

        public ActionResult ExibirAtestadoHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_ATESTADO> listaConsultas = baseApp.GetAtestadosByCPF(usuario.PACI_NR_CPF);
                listaMasterAtestado = listaConsultas.Where(p => p.PAAT_DT_DATA.Value.Year == DateTime.Today.Year).ToList();
                Session["ListaAtestados"] = listaMasterAtestado;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirAtestadoAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_ATESTADO> listaConsultas = baseApp.GetAtestadosByCPF(usuario.PACI_NR_CPF);
                listaMasterAtestado = listaConsultas.Where(p => p.PAAT_DT_DATA.Value.Year < DateTime.Today.Year).ToList();
                Session["ListaAtestados"] = listaMasterAtestado;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirAtestadosTodos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_ATESTADO> listaConsultas = baseApp.GetAtestadosByCPF(usuario.PACI_NR_CPF);
                listaMasterAtestado = listaConsultas;
                Session["ListaAtestados"] = listaMasterAtestado;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult SelecionarImprimirAtestado(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Session["IdAtestado"] = id;
                if (conf.CONF_IN_ASSINA_DIGITAL_ATESTADO == 1)
                {
                    return RedirectToAction("GerarAtestadoPDFNovaAssina", "Paciente");
                }
                else
                {
                    return RedirectToAction("GerarAtestadoPDFNova", "Paciente");
                }
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirSolicitacaoHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_SOLICITACAO> listaConsultas = baseApp.GetSolicitacaoByCPF(usuario.PACI_NR_CPF);
                listaMasterSolicitacao = listaConsultas.Where(p => p.PASO_DT_EMISSAO_COMPLETA.Value.Year == DateTime.Today.Year).ToList();
                Session["ListaSolicitacao"] = listaMasterSolicitacao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirSolicitacaoAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_SOLICITACAO> listaConsultas = baseApp.GetSolicitacaoByCPF(usuario.PACI_NR_CPF);
                listaMasterSolicitacao = listaConsultas.Where(p => p.PASO_DT_EMISSAO_COMPLETA.Value.Year < DateTime.Today.Year).ToList();
                Session["ListaSolicitacao"] = listaMasterSolicitacao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirSolicitacaoTodos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_SOLICITACAO> listaConsultas = baseApp.GetSolicitacaoByCPF(usuario.PACI_NR_CPF);
                listaMasterSolicitacao = listaConsultas;
                Session["ListaSolicitacao"] = listaMasterSolicitacao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult SelecionarImprimirSolicitacao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Session["IdSolicitacao"] = id;
                if (conf.CONF_IN_ASSINA_DIGITAL_ATESTADO == 1)
                {
                    return RedirectToAction("GerarSolicitacaoPDFNovaAssina", "Paciente");
                }
                else
                {
                    return RedirectToAction("GerarSolicitacaoPDFNova", "Paciente");
                }
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirPrescricaoHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_PRESCRICAO> listaConsultas = baseApp.GetPrescricaoByCPF(usuario.PACI_NR_CPF);
                listaMasterPrescricao = listaConsultas.Where(p => p.PAPR_DT_EMISSAO_COMPLETA.Value.Year == DateTime.Today.Year).ToList();
                Session["ListaPrescricao"] = listaMasterPrescricao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirPrescricaoAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_PRESCRICAO> listaConsultas = baseApp.GetPrescricaoByCPF(usuario.PACI_NR_CPF);
                listaMasterPrescricao = listaConsultas.Where(p => p.PAPR_DT_EMISSAO_COMPLETA.Value.Year < DateTime.Today.Year).ToList();
                Session["ListaPrescricao"] = listaMasterPrescricao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirPrescricaoTodos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<PACIENTE_PRESCRICAO> listaConsultas = baseApp.GetPrescricaoByCPF(usuario.PACI_NR_CPF);
                listaMasterPrescricao = listaConsultas;
                Session["ListaPrescricao"] = listaMasterPrescricao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult SelecionarImprimirPrescricao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Session["IdPrescricao"] = id;
                PACIENTE_PRESCRICAO pres = baseApp.GetPrescricaoById(id);
                if (conf.CONF_IN_ASSINA_DIGITAL_ATESTADO == 1)
                {
                    if (pres.TICO_CD_ID == 1)
                    {
                        return RedirectToAction("GerarPrescricaoPDF_ComumNovaAssina", "Paciente");
                    }
                    else
                    {
                        return RedirectToAction("GerarPrescricaoPDF_EspecialNovaAssina", "Paciente");
                    }
                }
                else
                {
                    if (pres.TICO_CD_ID == 1)
                    {
                        return RedirectToAction("GerarPrescricaoPDF_ComumNova", "Paciente");
                    }
                    else
                    {
                        return RedirectToAction("GerarPrescricaoPDF_EspecialNova", "Paciente");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public List<NOTICIA> CarregaNoticiaGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> conf = new List<NOTICIA>();
                if (Session["ListaNoticia"] == null)
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
                        conf = (List<NOTICIA>)Session["ListaNoticia"];
                    }
                }
                Session["ListaNoticia"] = conf;
                Session["NoticiaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "AreaPaciente", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return null;
            }
        }

        public ActionResult ExibirLocacaoAtiva()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<LOCACAO> listaLocacao = locaApp.GetLocacaoByCPF(usuario.PACI_NR_CPF);
                listaMasterLocacao = listaLocacao.Where(p => p.LOCA_IN_STATUS == 1).ToList();
                Session["ListaLocacao"] = listaMasterLocacao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirLocacaoTodos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<LOCACAO> listaLocacao = locaApp.GetLocacaoByCPF(usuario.PACI_NR_CPF);
                listaMasterLocacao = listaLocacao.ToList();
                Session["ListaLocacao"] = listaMasterLocacao;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "AreaPaciente", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        [HttpGet]
        public ActionResult VerLocacao(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE paciente = (PACIENTE)Session["UserCredentials"];
                USUARIO usuario = usuApp.GetItemById(paciente.USUA_CD_ID.Value);

                // Configuração
                CONFIGURACAO conf = confApp.GetItemById(paciente.ASSI_CD_ID);

                // Prepara view
                LOCACAO item = locaApp.GetItemById(id);
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                Session["IdLocacao"] = item.LOCA_CD_ID;
                Session["Locacao"] = item;
                Session["IdPaciente"] = paciente.PACI__CD_ID;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, paciente.ASSI_CD_ID, "LOCACAO_VER", "AreaPaciente", "VerLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "AreaPaciente", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult ImprimirContratoLocacaoDireto()
        {
            LOCACAO loca = locaApp.GetItemById((Int32)Session["IdLocacao"]);
            Session["VoltaContrato"] = 2;
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarContratoPDFAssina", "Locacao");
            }
            return RedirectToAction("GerarContratoPDF", "Locacao");
        }

        public ActionResult VoltarAnexoAreaPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("VerLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        public ActionResult ImprimirDistratoLocacaoDireto()
        {
            LOCACAO loca = locaApp.GetItemById((Int32)Session["IdLocacao"]);
            Session["VoltaContrato"] = 2;
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarDistratoPDFAssina", "Locacao");
            }
            return RedirectToAction("GerarDistratoPDF", "Locacao");
        }

        public ActionResult ImprimirEncerraLocacaoDireto()
        {
            LOCACAO loca = locaApp.GetItemById((Int32)Session["IdLocacao"]);
            Session["VoltaContrato"] = 2;
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarEncerraPDFAssina", "Locacao");
            }
            return RedirectToAction("GerarEncerraPDF", "Locacao");
        }

        public ActionResult CarregarContratoDireto()
        {
            LOCACAO loca = locaApp.GetItemById((Int32)Session["IdLocacao"]);
            Session["VoltaContrato"] = 2;
            return RedirectToAction("CarregarContrato", "Locacao");
        }

        public ActionResult ExibirNoticiaHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<NOTICIA> lista = notApp.GetAllItens(usuario.ASSI_CD_ID);
                listaMasterNoticia = lista.Where(p => p.NOTC_DT_EMISSAO.Value.Date == DateTime.Today.Date).ToList();
                Session["ListaNoticia"] = listaMasterNoticia;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult ExibirNoticiaTodos()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("LogoutAreaPaciente", "AreaPaciente");
                }
                PACIENTE usuario = (PACIENTE)Session["UserCredentials"];
                List<NOTICIA> lista = notApp.GetAllItens(usuario.ASSI_CD_ID);
                listaMasterNoticia = lista;
                Session["ListaNoticia"] = listaMasterNoticia;
                return RedirectToAction("MontarTelaAreaPaciente");
            }
            catch (Exception ex)
            {
                Session["MensagemLogin"] = 100;
                Session["MensagemErro"] = ex.Message;
                Session["Excecao"] = ex;
                Session["TipoVolta"] = 2;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                Session["VoltaExcecao"] = "AreaPaciente";
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Exceção", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("ErroGeralAreaPaciente", "AreaPaciente");
            }
        }

        public ActionResult VerNoticia(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            PACIENTE pac = (PACIENTE)Session["UserCredentials"];
            ViewBag.NomePaciente = pac.PACI_NM_NOME;

            Session["IdNoticia"] = id;
            NOTICIA item = notApp.GetItemById(id);
            item.NOTC_NR_ACESSO = ++item.NOTC_NR_ACESSO;
            Int32 volta = notApp.ValidateEdit(item, item);

            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            return View(vm);
        }

        public ActionResult IncluirComentario()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                NOTICIA item = notApp.GetItemById((Int32)Session["IdNoticia"]);
                PACIENTE pac = (PACIENTE)Session["UserCredentials"];
                USUARIO usu = usuApp.GetItemById(pac.USUA_CD_ID.Value);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                NOTICIA_COMENTARIO coment = new NOTICIA_COMENTARIO();
                NoticiaComentarioViewModel vm = Mapper.Map<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>(coment);
                vm.NOCO_DT_COMENTARIO = DateTime.Now;
                vm.NOCO_IN_ATIVO = 1;
                vm.NOTC_CD_ID = item.NOTC_CD_ID;
                vm.USUARIO = usu;
                vm.USUA_CD_ID = usu.USUA_CD_ID;
                vm.PACI_CD_ID = pac.PACI__CD_ID;
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usu.USUA_CD_ID, usu.ASSI_CD_ID, "NOTICIA_COMENTARIO_INCLUIR", "AreaPaciente", "IncluirComentario");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "AreaPaciente", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirComentario(NoticiaComentarioViewModel vm)
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
                    vm.NOCO_DS_COMENTARIO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOCO_DS_COMENTARIO);

                    // Executa a operação
                    NOTICIA_COMENTARIO item = Mapper.Map<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    NOTICIA not = notApp.GetItemById((Int32)Session["IdNoticia"]);
                    String json = JsonConvert.SerializeObject(item);

                    item.USUARIO = null;
                    not.NOTICIA_COMENTARIO.Add(item);
                    Int32 volta = notApp.ValidateEdit(not, not, usuarioLogado);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Notícia - Comentário - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    return RedirectToAction("VoltarVerNoticia");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Paciente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarVerNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("VerNoticia", new { id = (Int32)Session["IdNoticia"] });
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoNoticia(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                PACIENTE pac = (PACIENTE)Session["UserCredentials"];
                USUARIO usu = usuApp.GetItemById(pac.USUA_CD_ID.Value);

                NOTICIA_COMENTARIO item = notApp.GetComentarioById(id);
                item.NOCO_IN_ATIVO = 0;
                Int32 volta = notApp.ValidateEditComentario(item);
                Session["NoticiaAlterada"] = 1;

                return RedirectToAction("VoltarVerNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "AreaPaciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "AreaPaciente", "WebDoctor", 1, (USUARIO)Session["UsuarioArea"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}