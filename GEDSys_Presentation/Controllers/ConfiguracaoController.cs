using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using System.Reflection;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using System.Globalization;
using System.Linq;
using XidNet;
using Ical.Net.Serialization.iCalendar;
using CrossCutting;


namespace ERP_Condominios_Solution.Controllers
{
    public class ConfiguracaoController : Controller
    {
        private readonly IConfiguracaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoCalendarioAppService calApp;
        private readonly IConfiguracaoAnamneseAppService anaApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IPacienteAppService pacApp;
        private readonly IAcessoMetodoAppService aceApp;

        CONFIGURACAO objeto = new CONFIGURACAO();
        CONFIGURACAO objetoAntes = new CONFIGURACAO();
        List<CONFIGURACAO> listaMaster = new List<CONFIGURACAO>();
        CONFIGURACAO_CALENDARIO objetoCal = new CONFIGURACAO_CALENDARIO();
        CONFIGURACAO_CALENDARIO objetoCalAntes = new CONFIGURACAO_CALENDARIO();
        List<CONFIGURACAO_CALENDARIO> listaMasterCal = new List<CONFIGURACAO_CALENDARIO>();
        CONFIGURACAO_ANAMNESE objetoAna = new CONFIGURACAO_ANAMNESE();
        CONFIGURACAO_ANAMNESE objetoAnaAntes = new CONFIGURACAO_ANAMNESE();
        List<CONFIGURACAO_ANAMNESE> listaMasterAna = new List<CONFIGURACAO_ANAMNESE>();


        public ConfiguracaoController(IConfiguracaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps,IConfiguracaoCalendarioAppService calApps, IConfiguracaoAnamneseAppService anaApps, IConfiguracaoAppService confApps, IPacienteAppService pacApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            calApp = calApps;
            anaApp = anaApps;
            confApp = confApps;
            pacApp = pacApps;
            aceApp = aceApps;
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
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult GetConfiguracao()
        {
            var config = baseApp.GetItemById(2);
            var serialConfig = new CONFIGURACAO
            {
                CONF_CD_ID = config.CONF_CD_ID,
                ASSI_CD_ID = config.ASSI_CD_ID,
                CONF_NR_FALHAS_DIA = config.CONF_NR_FALHAS_DIA,
                CONF_NM_HOST_SMTP = config.CONF_NM_HOST_SMTP,
                CONF_NM_PORTA_SMTP = config.CONF_NM_PORTA_SMTP,
                CONF_NM_EMAIL_EMISSOO = config.CONF_NM_EMAIL_EMISSOO,
                CONF_NM_SENHA_EMISSOR = config.CONF_NM_SENHA_EMISSOR,
                CONF_NR_REFRESH_DASH = config.CONF_NR_REFRESH_DASH,
                CONF_NM_ARQUIVO_ALARME = config.CONF_NM_ARQUIVO_ALARME,
                CONF_NR_REFRESH_NOTIFICACAO = config.CONF_NR_REFRESH_NOTIFICACAO,
                CONF_SG_LOGIN_SMS = config.CONF_SG_LOGIN_SMS,
                CONF_SG_SENHA_SMS = config.CONF_SG_SENHA_SMS,
                CONF_SG_LOGIN_SMS_PRIORITARIO = config.CONF_SG_LOGIN_SMS_PRIORITARIO,
                CONF_SG_SENHA_SMS_PRIORITARIO = config.CONF_SG_SENHA_SMS_PRIORITARIO,
                CONF_NR_DIAS_ACAO = config.CONF_NR_DIAS_ACAO,
                CONF_IN_CNPJ_DUPLICADO = config.CONF_IN_CNPJ_DUPLICADO,
                CONF_IN_ASSINANTE_FILIAL = config.CONF_IN_ASSINANTE_FILIAL,
                CONF_IN_FALHA_IMPORTACAO = config.CONF_IN_FALHA_IMPORTACAO,
                CONF_IN_ETAPAS_CRM = config.CONF_IN_ETAPAS_CRM,
                CONF_IN_NOTIF_ACAO_ADM = config.CONF_IN_NOTIF_ACAO_ADM,
                CONF_IN_NOTIF_ACAO_GER = config.CONF_IN_NOTIF_ACAO_GER,
                CONF_IN_NOTIF_ACAO_VEN = config.CONF_IN_NOTIF_ACAO_VEN,
                CONF_IN_NOTIF_ACAO_OPR = config.CONF_IN_NOTIF_ACAO_OPR,
                CONF_IN_NOTIF_ACAO_USU = config.CONF_IN_NOTIF_ACAO_USU,
                CONF_IN_LOGO_EMPRESA = config.CONF_IN_LOGO_EMPRESA,
                CONF_NR_GRID_CLIENTE = config.CONF_NR_GRID_CLIENTE,
                CONF_NR_GRID_MENSAGEM = config.CONF_NR_GRID_MENSAGEM,
                CONF_NR_GRID_PRODUTO = config.CONF_NR_GRID_PRODUTO,
            };
            return Json(serialConfig);
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracao()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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
            Session["ModuloAtual"] = "Configuração - Geral";

            // Carrega listas
            objeto = baseApp.GetItemById(idAss);
            Session["Configuracao"] = objeto;

            ViewBag.Listas = (CONFIGURACAO)Session["Configuracao"];
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");
            var logo = new List<SelectListItem>();
            logo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            logo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Logotipo = new SelectList(logo, "Value", "Text");
            var mensFab = new List<SelectListItem>();
            mensFab.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mensFab.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.MensFab = new SelectList(mensFab, "Value", "Text");
            var cons = new List<SelectListItem>();
            cons.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cons.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Consulta = new SelectList(cons, "Value", "Text");
            var logo1 = new List<SelectListItem>();
            logo1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            logo1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Logo = new SelectList(logo1, "Value", "Text");
            var mail = new List<SelectListItem>();
            mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Mail = new SelectList(mail, "Value", "Text");
            var consulta = new List<SelectListItem>();
            consulta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            consulta.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Consulta = new SelectList(consulta, "Value", "Text");
            var recebe = new List<SelectListItem>();
            recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
            var remedio = new List<SelectListItem>();
            remedio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            remedio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Remedio = new SelectList(remedio, "Value", "Text");
            var solic = new List<SelectListItem>();
            solic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            solic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Solicitacao = new SelectList(solic, "Value", "Text");
            var pacSegue = new List<SelectListItem>();
            pacSegue.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pacSegue.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.PacSegue = new SelectList(pacSegue, "Value", "Text");
            var camera = new List<SelectListItem>();
            camera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            camera.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Camera = new SelectList(camera, "Value", "Text");
            var anamnese = new List<SelectListItem>();
            anamnese.Add(new SelectListItem() { Text = "Padrão - Segmentada", Value = "1" });
            anamnese.Add(new SelectListItem() { Text = "Contínua", Value = "2" });
            ViewBag.Anamnese = new SelectList(anamnese, "Value", "Text");
            var consultaFutura = new List<SelectListItem>();
            consultaFutura.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            consultaFutura.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.ConsultaFutura = new SelectList(consultaFutura, "Value", "Text");
            var pisca = new List<SelectListItem>();
            pisca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pisca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Pisca = new SelectList(pisca, "Value", "Text");
            var zap = new List<SelectListItem>();
            zap.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            zap.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Zap = new SelectList(zap, "Value", "Text");
            var aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
            var confirma = new List<SelectListItem>();
            confirma.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            confirma.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Confirmacao = new SelectList(confirma, "Value", "Text");
            var pag = new List<SelectListItem>();
            pag.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pag.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Pagamento = new SelectList(pag, "Value", "Text");
            var cad = new List<SelectListItem>();
            cad.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cad.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Cadastro = new SelectList(cad, "Value", "Text");
            var atraso = new List<SelectListItem>();
            atraso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            atraso.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Atraso = new SelectList(atraso, "Value", "Text");
            var marca = new List<SelectListItem>();
            marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Marca = new SelectList(marca, "Value", "Text");
            var hora = new List<SelectListItem>();
            hora.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            hora.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Hora = new SelectList(hora, "Value", "Text");
            var assAtestado = new List<SelectListItem>();
            assAtestado.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assAtestado.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaAtestado = new SelectList(assAtestado, "Value", "Text");
            var assSolic = new List<SelectListItem>();
            assSolic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assSolic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaSolicitacao = new SelectList(assSolic, "Value", "Text");
            var assPresc = new List<SelectListItem>();
            assPresc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assPresc.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaPrescricao = new SelectList(assPresc, "Value", "Text");
            var modelo = new List<SelectListItem>();
            modelo.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            modelo.Add(new SelectListItem() { Text = "Fisioterapia - Sono", Value = "2" });
            ViewBag.Modelo = new SelectList(modelo, "Value", "Text");
            var estoque = new List<SelectListItem>();
            estoque.Add(new SelectListItem() { Text = "Único", Value = "1" });
            estoque.Add(new SelectListItem() { Text = "Diário", Value = "2" });
            estoque.Add(new SelectListItem() { Text = "Nunca", Value = "0" });
            ViewBag.Estoque = new SelectList(estoque, "Value", "Text");
            var assCont = new List<SelectListItem>();
            assCont.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assCont.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaContrato = new SelectList(assCont, "Value", "Text");
            var recibo = new List<SelectListItem>();
            recibo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recibo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recibo = new SelectList(recibo, "Value", "Text");

            // Indicadores

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONFIGURACAO", "Configuracao", "MontarTelaConfiguracao");

            // Abre view
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/18/Ajuda18.pdf";
            Session["MensConfiguracao"] = 0;
            objetoAntes = objeto;
            if (objeto.CONF_NR_FALHAS_DIA == null)
            {
                objeto.CONF_NR_FALHAS_DIA = 3;
            }
            Session["Configuracao"] = objeto;
            Session["IdConf"] = 1;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(objeto);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaConfiguracao(ConfiguracaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var listaGrid = new List<SelectListItem>();
            listaGrid.Add(new SelectListItem() { Text = "10", Value = "10" });
            listaGrid.Add(new SelectListItem() { Text = "25", Value = "25" });
            listaGrid.Add(new SelectListItem() { Text = "50", Value = "50" });
            listaGrid.Add(new SelectListItem() { Text = "100", Value = "100" });
            ViewBag.ListaGrid = new SelectList(listaGrid, "Value", "Text");
            var logo = new List<SelectListItem>();
            logo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            logo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Logotipo = new SelectList(logo, "Value", "Text");
            var mensFab = new List<SelectListItem>();
            mensFab.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mensFab.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.MensFab = new SelectList(mensFab, "Value", "Text");
            var cons = new List<SelectListItem>();
            cons.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cons.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Consulta = new SelectList(cons, "Value", "Text");
            var logo1 = new List<SelectListItem>();
            logo1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            logo1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Logo = new SelectList(logo1, "Value", "Text");
            var mail = new List<SelectListItem>();
            mail.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mail.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Mail = new SelectList(mail, "Value", "Text");
            var consulta = new List<SelectListItem>();
            consulta.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            consulta.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Consulta = new SelectList(consulta, "Value", "Text");
            var recebe = new List<SelectListItem>();
            recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
            var remedio = new List<SelectListItem>();
            remedio.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            remedio.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Remedio = new SelectList(remedio, "Value", "Text");
            var solic = new List<SelectListItem>();
            solic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            solic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Solicitacao = new SelectList(solic, "Value", "Text");
            var pacSegue = new List<SelectListItem>();
            pacSegue.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pacSegue.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.PacSegue = new SelectList(pacSegue, "Value", "Text");
            var camera = new List<SelectListItem>();
            camera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            camera.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Camera = new SelectList(camera, "Value", "Text");
            var anamnese = new List<SelectListItem>();
            anamnese.Add(new SelectListItem() { Text = "Padrão - Segmentada", Value = "1" });
            anamnese.Add(new SelectListItem() { Text = "Contínua", Value = "2" });
            ViewBag.Anamnese = new SelectList(anamnese, "Value", "Text");
            ViewBag.Anamnese = new SelectList(anamnese, "Value", "Text");
            var consultaFutura = new List<SelectListItem>();
            consultaFutura.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            consultaFutura.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.ConsultaFutura = new SelectList(consultaFutura, "Value", "Text");
            var pisca = new List<SelectListItem>();
            pisca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pisca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Pisca = new SelectList(pisca, "Value", "Text");
            var zap = new List<SelectListItem>();
            zap.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            zap.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Zap = new SelectList(zap, "Value", "Text");
            var aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Aniversario = new SelectList(aniv, "Value", "Text");
            var confirma = new List<SelectListItem>();
            confirma.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            confirma.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Confirmacao = new SelectList(confirma, "Value", "Text");
            var pag = new List<SelectListItem>();
            pag.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pag.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Pagamento = new SelectList(pag, "Value", "Text");
            var cad = new List<SelectListItem>();
            cad.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cad.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Cadastro = new SelectList(cad, "Value", "Text");
            var atraso = new List<SelectListItem>();
            atraso.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            atraso.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Atraso = new SelectList(atraso, "Value", "Text");
            var marca = new List<SelectListItem>();
            marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Marca = new SelectList(marca, "Value", "Text");
            var hora = new List<SelectListItem>();
            hora.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            hora.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Hora = new SelectList(hora, "Value", "Text");
            var assAtestado = new List<SelectListItem>();
            assAtestado.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assAtestado.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaAtestado = new SelectList(assAtestado, "Value", "Text");
            var assSolic = new List<SelectListItem>();
            assSolic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assSolic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaSolicitacao = new SelectList(assSolic, "Value", "Text");
            var assPresc = new List<SelectListItem>();
            assPresc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assPresc.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaPrescricao = new SelectList(assPresc, "Value", "Text");
            var modelo = new List<SelectListItem>();
            modelo.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            modelo.Add(new SelectListItem() { Text = "Fisioterapia - Sono", Value = "2" });
            ViewBag.Modelo = new SelectList(modelo, "Value", "Text");
            var estoque = new List<SelectListItem>();
            estoque.Add(new SelectListItem() { Text = "Único", Value = "1" });
            estoque.Add(new SelectListItem() { Text = "Diário", Value = "2" });
            estoque.Add(new SelectListItem() { Text = "Nunca", Value = "0" });
            ViewBag.Estoque = new SelectList(estoque, "Value", "Text");
            var assCont = new List<SelectListItem>();
            assCont.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            assCont.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.AssinaContrato = new SelectList(assCont, "Value", "Text");
            var recibo = new List<SelectListItem>();
            recibo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recibo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recibo = new SelectList(recibo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);

                    // Grava alteracoes
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["Configuracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarBaseConfiguracao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaConfiguracao");
        }

        [HttpGet]
        public ActionResult EditarConfiguracao(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONFIGURACAO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Configuracao"] = item;
            Session["IdVolta"] = id;
            ConfiguracaoViewModel vm = Mapper.Map<CONFIGURACAO, ConfiguracaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarConfiguracao(ConfiguracaoViewModel vm)
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
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO item = Mapper.Map<ConfiguracaoViewModel, CONFIGURACAO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Sucesso
                    objeto = new CONFIGURACAO();
                    Session["ListaConfiguracao"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult CriptoChave()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = baseApp.GetItemById(usuario.ASSI_CD_ID);

            conf.CONF_SG_LOGIN_SMS_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_LOGIN_SMS);
            conf.CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_LOGIN_SMS_PRIORITARIO);
            conf.CONF_SG_SENHA_SMS_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_SENHA_SMS);
            conf.CONF_SG_SENHA_SMS_PRIORITARIO_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_SG_SENHA_SMS_PRIORITARIO);

            conf.CONF_NM_SENDGRID_APIKEY_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_APIKEY);
            conf.CONF_NM_SENDGRID_LOGIN_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_LOGIN);
            conf.CONF_NM_SENDGRID_PWD_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_SENDGRID_PWD);

            conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_CS_CONNECTION_STRING_AZURE);
            conf.CONF_NM_EMISSOR_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_EMISSOR_AZURE);
            conf.CONF_NM_ENDPOINT_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_ENDPOINT_AZURE);
            conf.CONF_NM_KEY_AZURE_CRIP = CrossCutting.Cryptography.Encrypt(conf.CONF_NM_KEY_AZURE);

            Int32 volta = baseApp.ValidateEdit(conf);
            return RedirectToAction("MontarTelaConfiguracao", "Configuracao");
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracaoCalendario()
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
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ModuloAtual"] = "Configuração - Calendário";


            // Recupera configuração
            List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
            objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfiguracaoCal"] = objetoCal.COCA_CD_ID;
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/18/Ajuda18_1.pdf";
            ViewBag.Usuario = objetoCal.USUARIO.USUA_NM_NOME;

            // Recupera bloqueios
            ViewBag.Itens = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();

            // Monta listas
            ViewBag.Listas = (CONFIGURACAO_CALENDARIO)Session["ConfiguracaoCal"];
            var seg = new List<SelectListItem>();
            seg.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seg.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Segunda = new SelectList(seg, "Value", "Text");
            var ter = new List<SelectListItem>();
            ter.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ter.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Terca = new SelectList(ter, "Value", "Text");
            var qua = new List<SelectListItem>();
            qua.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            qua.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Quarta = new SelectList(qua, "Value", "Text");
            var qui = new List<SelectListItem>();
            qui.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            qui.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Quinta = new SelectList(qui, "Value", "Text");
            var sex = new List<SelectListItem>();
            sex.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sex.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Sexta = new SelectList(sex, "Value", "Text");
            var sab = new List<SelectListItem>();
            sab.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sab.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Sabado = new SelectList(sab, "Value", "Text");
            var dom = new List<SelectListItem>();
            dom.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dom.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Domingo = new SelectList(dom, "Value", "Text");

            // Mensagem
            if (Session["MensConfiguracao"] != null)
            {
                if ((Int32)Session["MensConfiguracao"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                }
            }

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONFIGURACAO_CALENDARIO", "Configuracao", "MontarTelaConfiguracaoCalendario");

            // Abre view
            Session["MensConfiguracao"] = 0;
            Session["VoltaBloqueio"] = 1;
            objetoCalAntes = objetoCal;
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfCal"] = 1;
            ConfiguracaoCalendarioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO, ConfiguracaoCalendarioViewModel>(objetoCal);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaConfiguracaoCalendario(ConfiguracaoCalendarioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Listas = (CONFIGURACAO_CALENDARIO)Session["ConfiguracaoCal"];
            var seg = new List<SelectListItem>();
            seg.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            seg.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Segunda = new SelectList(seg, "Value", "Text");
            var ter = new List<SelectListItem>();
            ter.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ter.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Terca = new SelectList(ter, "Value", "Text");
            var qua = new List<SelectListItem>();
            qua.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            qua.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Quarta = new SelectList(qua, "Value", "Text");
            var qui = new List<SelectListItem>();
            qui.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            qui.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Quinta = new SelectList(qui, "Value", "Text");
            var sex = new List<SelectListItem>();
            sex.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sex.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Sexta = new SelectList(sex, "Value", "Text");
            var sab = new List<SelectListItem>();
            sab.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            sab.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Sabado = new SelectList(sab, "Value", "Text");
            var dom = new List<SelectListItem>();
            dom.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dom.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Domingo = new SelectList(dom, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica de horarios
                    if (vm.COCA_IN_SEGUNDA_FEIRA == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_SEG_INICIO == null || vm.COCA_HR_COMERCIAL_SEG_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SEG_INICIO == vm.COCA_HR_INTERVALO_SEG_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SEG_INICIO > vm.COCA_HR_INTERVALO_SEG_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEG_INICIO > vm.COCA_HR_INTERVALO_SEG_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEG_INICIO < vm.COCA_HR_COMERCIAL_SEG_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEG_INICIO > vm.COCA_HR_COMERCIAL_SEG_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_TERCA_FEIRA == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_TER_INICIO == null || vm.COCA_HR_COMERCIAL_TER_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_TER_INICIO == vm.COCA_HR_COMERCIAL_TER_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_TER_INICIO > vm.COCA_HR_COMERCIAL_TER_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_TER_INICIO > vm.COCA_HR_INTERVALO_TER_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_TER_INICIO < vm.COCA_HR_COMERCIAL_TER_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_TER_INICIO > vm.COCA_HR_COMERCIAL_TER_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_QUARTA_FEIRA == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_QUA_INICIO == null || vm.COCA_HR_COMERCIAL_QUA_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_QUA_INICIO == vm.COCA_HR_COMERCIAL_QUA_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_QUA_INICIO > vm.COCA_HR_COMERCIAL_QUA_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUA_INICIO > vm.COCA_HR_INTERVALO_QUA_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUA_INICIO < vm.COCA_HR_COMERCIAL_QUA_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUA_INICIO > vm.COCA_HR_COMERCIAL_QUA_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_QUINTA_FEIRA == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_QUI_INICIO == null || vm.COCA_HR_COMERCIAL_QUI_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_QUI_INICIO == vm.COCA_HR_COMERCIAL_QUI_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_QUI_INICIO > vm.COCA_HR_COMERCIAL_QUI_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUI_INICIO > vm.COCA_HR_INTERVALO_QUI_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUI_INICIO < vm.COCA_HR_COMERCIAL_QUI_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_QUI_INICIO > vm.COCA_HR_COMERCIAL_QUI_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_SEXTA_FEIRA == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_SEX_INICIO == null || vm.COCA_HR_COMERCIAL_SEX_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SEX_INICIO == vm.COCA_HR_COMERCIAL_SEX_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SEX_INICIO > vm.COCA_HR_COMERCIAL_SEX_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEX_INICIO > vm.COCA_HR_INTERVALO_SEX_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEX_INICIO < vm.COCA_HR_COMERCIAL_SEX_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SEX_INICIO > vm.COCA_HR_COMERCIAL_SEX_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_SABADO == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_SAB_INICIO == null || vm.COCA_HR_COMERCIAL_SAB_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SAB_INICIO == vm.COCA_HR_COMERCIAL_SAB_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_SAB_INICIO > vm.COCA_HR_COMERCIAL_SAB_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SAB_INICIO > vm.COCA_HR_INTERVALO_SAB_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SAB_INICIO < vm.COCA_HR_COMERCIAL_SAB_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_SAB_INICIO > vm.COCA_HR_COMERCIAL_SAB_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.COCA_IN_DOMINGO == 1)
                    {
                        if (vm.COCA_HR_COMERCIAL_DOM_INICIO == null || vm.COCA_HR_COMERCIAL_DOM_FINAL == null)
                        {
                            Session["MensConfiguracao"] = 1;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_DOM_INICIO == vm.COCA_HR_COMERCIAL_DOM_FINAL)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_COMERCIAL_DOM_INICIO > vm.COCA_HR_COMERCIAL_DOM_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_DOM_INICIO > vm.COCA_HR_INTERVALO_DOM_FINAL)
                        {
                            Session["MensConfiguracao"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_DOM_INICIO < vm.COCA_HR_COMERCIAL_DOM_INICIO)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCA_HR_INTERVALO_DOM_INICIO > vm.COCA_HR_COMERCIAL_DOM_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO_CALENDARIO item = Mapper.Map<ConfiguracaoCalendarioViewModel, CONFIGURACAO_CALENDARIO>(vm);

                    // Grava alteracoes
                    Int32 volta = calApp.ValidateEdit(item, item, usuarioLogado);

                    // Sucesso
                    objetoCal = new CONFIGURACAO_CALENDARIO();
                    Session["ConfiguracaoCal"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracaoCalendario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaVerConfiguracaoCalendario()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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


            // Recupera configuração
            objetoCal = calApp.GetItemById(idAss);
            Session["ConfiguracaoCal"] = objetoCal;

            // Monta listas
            ViewBag.Listas = (CONFIGURACAO_CALENDARIO)Session["ConfiguracaoCal"];

            // Mensagem
            if (Session["MensConfiguracao"] != null)
            {
                if ((Int32)Session["MensConfiguracao"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                }
            }

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONFIGURACAO_CALENDARIO_VER", "Configuracao", "MontarTelaVerConfiguracaoCalendario");

            // Abre view
            Session["MensConfiguracao"] = 0;
            objetoCalAntes = objetoCal;
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfCal"] = 1;
            ConfiguracaoCalendarioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO, ConfiguracaoCalendarioViewModel>(objetoCal);
            return View(vm);
        }

        [HttpGet]
        public ActionResult MontarTelaConfiguracaoAnamnese()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
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
            Session["ModuloAtual"] = "Configuração - Anamnese";

            // Recupera configuração
            objetoAna = anaApp.GetItemById(idAss);
            Session["ConfiguracaoAna"] = objetoAna;

            // Monta listas
            ViewBag.Listas = (CONFIGURACAO_ANAMNESE)Session["ConfiguracaoAna"];
            var mot = new List<SelectListItem>();
            mot.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mot.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Motivo = new SelectList(mot, "Value", "Text");
            var fam = new List<SelectListItem>();
            fam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fam.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Familia = new SelectList(fam, "Value", "Text");
            var soc = new List<SelectListItem>();
            soc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            soc.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Social = new SelectList(soc, "Value", "Text");
            var car = new List<SelectListItem>();
            car.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            car.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Cardio = new SelectList(car, "Value", "Text");
            var res = new List<SelectListItem>();
            res.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            res.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Respira = new SelectList(res, "Value", "Text");
            var abd = new List<SelectListItem>();
            abd.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            abd.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Abdomem = new SelectList(abd, "Value", "Text");
            var mem = new List<SelectListItem>();
            mem.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mem.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Membro = new SelectList(mem, "Value", "Text");
            var que = new List<SelectListItem>();
            que.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            que.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Queixa = new SelectList(que, "Value", "Text");
            var doe = new List<SelectListItem>();
            doe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            doe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Doenca = new SelectList(doe, "Value", "Text");
            var pat = new List<SelectListItem>();
            pat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pat.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Patolo = new SelectList(pat, "Value", "Text");
            var dia1 = new List<SelectListItem>();
            dia1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dia1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Diag1 = new SelectList(dia1, "Value", "Text");
            var dia2 = new List<SelectListItem>();
            dia2.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dia2.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Diag2 = new SelectList(dia2, "Value", "Text");
            var con = new List<SelectListItem>();
            con.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            con.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Conduta = new SelectList(con, "Value", "Text");
            var obs = new List<SelectListItem>();
            obs.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obs.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Observa = new SelectList(obs, "Value", "Text");
            var cp1 = new List<SelectListItem>();
            cp1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo1 = new SelectList(cp1, "Value", "Text");
            var cp2 = new List<SelectListItem>();
            cp2.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp2.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo2 = new SelectList(cp2, "Value", "Text");
            var cp3 = new List<SelectListItem>();
            cp3.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp3.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo3 = new SelectList(cp3, "Value", "Text");
            var medic = new List<SelectListItem>();
            medic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            medic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Medic = new SelectList(medic, "Value", "Text");
            var cp4 = new List<SelectListItem>();
            cp4.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp4.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo4 = new SelectList(cp4, "Value", "Text");
            var cp5 = new List<SelectListItem>();
            cp5.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp5.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo5 = new SelectList(cp5, "Value", "Text");
            var cp6 = new List<SelectListItem>();
            cp6.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp6.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo6 = new SelectList(cp6, "Value", "Text");
            var cp7 = new List<SelectListItem>();
            cp7.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp7.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo7 = new SelectList(cp6, "Value", "Text");
            var cp8 = new List<SelectListItem>();
            cp8.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp8.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo8 = new SelectList(cp6, "Value", "Text");
            var cp9 = new List<SelectListItem>();
            cp9.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp9.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo9 = new SelectList(cp6, "Value", "Text");
            var cp10 = new List<SelectListItem>();
            cp10.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp10.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo10 = new SelectList(cp10, "Value", "Text");
            var anamnese = new List<SelectListItem>();
            anamnese.Add(new SelectListItem() { Text = "Padrão - Segmentada", Value = "1" });
            anamnese.Add(new SelectListItem() { Text = "Contínua", Value = "2" });
            ViewBag.Anamnese = new SelectList(anamnese, "Value", "Text");
            var continua = new List<SelectListItem>();
            continua.Add(new SelectListItem() { Text = "Por Data", Value = "1" });
            continua.Add(new SelectListItem() { Text = "Livre", Value = "0" });
            ViewBag.Continua = new SelectList(continua, "Value", "Text");

            var comum = new List<SelectListItem>();
            comum.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            comum.Add(new SelectListItem() { Text = "Fisioterapia do Sono", Value = "2" });
            ViewBag.Comum = new SelectList(comum, "Value", "Text");

            // Trata mensagens
            if (Session["MensConfiguracao"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensConfiguracao"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0546", CultureInfo.CurrentCulture));
                }
            }

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONFIGURACAO_ANAMNESE", "Configuracao", "MontarTelaConfiguracaoAnamnese");

            // Abre view
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/18/Ajuda18_2.pdf";
            Session["MensConfiguracao"] = 0;
            objetoAnaAntes = objetoAna;
            Session["ConfiguracaoAna"] = objetoAna;
            Session["IdConfCal"] = 1;
            ConfiguracaoAnamneseViewModel vm = Mapper.Map<CONFIGURACAO_ANAMNESE, ConfiguracaoAnamneseViewModel>(objetoAna);
            return View(vm);
        }


        [HttpPost]
        public ActionResult MontarTelaConfiguracaoAnamnese(ConfiguracaoAnamneseViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Listas = (CONFIGURACAO_ANAMNESE)Session["ConfiguracaoAna"];
            var mot = new List<SelectListItem>();
            mot.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mot.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Motivo = new SelectList(mot, "Value", "Text");
            var fam = new List<SelectListItem>();
            fam.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            fam.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Familia = new SelectList(fam, "Value", "Text");
            var soc = new List<SelectListItem>();
            soc.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            soc.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Social = new SelectList(soc, "Value", "Text");
            var car = new List<SelectListItem>();
            car.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            car.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Cardio = new SelectList(car, "Value", "Text");
            var res = new List<SelectListItem>();
            res.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            res.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Respira = new SelectList(res, "Value", "Text");
            var abd = new List<SelectListItem>();
            abd.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            abd.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Abdomem = new SelectList(abd, "Value", "Text");
            var mem = new List<SelectListItem>();
            mem.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mem.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Membro = new SelectList(mem, "Value", "Text");
            var que = new List<SelectListItem>();
            que.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            que.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Queixa = new SelectList(que, "Value", "Text");
            var doe = new List<SelectListItem>();
            doe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            doe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Doenca = new SelectList(doe, "Value", "Text");
            var pat = new List<SelectListItem>();
            pat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            pat.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Patolo = new SelectList(pat, "Value", "Text");
            var dia1 = new List<SelectListItem>();
            dia1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dia1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Diag1 = new SelectList(dia1, "Value", "Text");
            var dia2 = new List<SelectListItem>();
            dia2.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            dia2.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Diag2 = new SelectList(dia2, "Value", "Text");
            var con = new List<SelectListItem>();
            con.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            con.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Conduta = new SelectList(con, "Value", "Text");
            var obs = new List<SelectListItem>();
            obs.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            obs.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Observa = new SelectList(obs, "Value", "Text");
            var cp1 = new List<SelectListItem>();
            cp1.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp1.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo1 = new SelectList(cp1, "Value", "Text");
            var cp2 = new List<SelectListItem>();
            cp2.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp2.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo2 = new SelectList(cp2, "Value", "Text");
            var cp3 = new List<SelectListItem>();
            cp3.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp3.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo3 = new SelectList(cp3, "Value", "Text");
            var medic = new List<SelectListItem>();
            medic.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            medic.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Medic = new SelectList(medic, "Value", "Text");
            var cp4 = new List<SelectListItem>();
            cp4.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp4.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo4 = new SelectList(cp4, "Value", "Text");
            var cp5 = new List<SelectListItem>();
            cp5.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp5.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo5 = new SelectList(cp5, "Value", "Text");
            var cp6 = new List<SelectListItem>();
            cp6.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp6.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo6 = new SelectList(cp6, "Value", "Text");
            var cp7 = new List<SelectListItem>();
            cp7.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp7.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo7 = new SelectList(cp6, "Value", "Text");
            var cp8 = new List<SelectListItem>();
            cp8.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp8.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo8 = new SelectList(cp6, "Value", "Text");
            var cp9 = new List<SelectListItem>();
            cp9.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp9.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo9 = new SelectList(cp6, "Value", "Text");
            var cp10 = new List<SelectListItem>();
            cp10.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp10.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Campo10 = new SelectList(cp10, "Value", "Text");
            var anamnese = new List<SelectListItem>();
            anamnese.Add(new SelectListItem() { Text = "Padrão - Segmentada", Value = "1" });
            anamnese.Add(new SelectListItem() { Text = "Contínua", Value = "2" });
            ViewBag.Anamnese = new SelectList(anamnese, "Value", "Text");
            var continua = new List<SelectListItem>();
            continua.Add(new SelectListItem() { Text = "Por Data", Value = "1" });
            continua.Add(new SelectListItem() { Text = "Livre", Value = "0" });
            ViewBag.Continua = new SelectList(continua, "Value", "Text");
            var comum = new List<SelectListItem>();
            comum.Add(new SelectListItem() { Text = "Padrão", Value = "1" });
            comum.Add(new SelectListItem() { Text = "Fisioterapia do Sono", Value = "2" });
            ViewBag.Comum = new SelectList(comum, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if (vm.COAN_IN_CAMPO_1 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_1 == null || vm.COAN_NM_CAMPO_1 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_2 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_2 == null || vm.COAN_NM_CAMPO_2 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_3 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_3 == null || vm.COAN_NM_CAMPO_3 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_4 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_4 == null || vm.COAN_NM_CAMPO_4 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_5 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_5 == null || vm.COAN_NM_CAMPO_5 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_6 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_6 == null || vm.COAN_NM_CAMPO_6 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_7 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_7 == null || vm.COAN_NM_CAMPO_7 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_8 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_8 == null || vm.COAN_NM_CAMPO_8 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_9 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_9 == null || vm.COAN_NM_CAMPO_9 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_CAMPO_10 == 1)
                    {
                        if (vm.COAN_NM_CAMPO_10 == null || vm.COAN_NM_CAMPO_10 == String.Empty)
                        {
                            Session["MensConfiguracao"] = 1;
                            return View(vm);
                        }
                    }
                    if (vm.COAN_IN_BLOCO_COMUM == 0 || vm.COAN_IN_BLOCO_COMUM == null)
                    {
                        vm.COAN_IN_BLOCO_COMUM = 1;
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONFIGURACAO_ANAMNESE item = Mapper.Map<ConfiguracaoAnamneseViewModel, CONFIGURACAO_ANAMNESE>(vm);

                    // Grava alteracoes
                        Int32 volta = anaApp.ValidateEdit(item, item, usuarioLogado);

                    // Sucesso
                    objetoAna = new CONFIGURACAO_ANAMNESE();
                    Session["BlocoAnamnese"] = item.COAN_IN_BLOCO_COMUM;
                    Session["ConfiguracaoAna"] = null;
                    Session["MensConfiguracao"] = 0;
                    return RedirectToAction("MontarTelaConfiguracaoAnamnese");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuração", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult IncluirBloqueio()
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Configuração - Bloqueio - Inclusão";

                // Prepara listas

                // Mensagem
                if (Session["MensConfiguracao"] != null)
                {
                    if ((Int32)Session["MensConfiguracao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0558", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0559", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0560", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0563", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0564", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 10)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "BLOQUEIO_INCLUIR", "Configuracao", "IncluirBloqueio");

                // Prepara view
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/15/Ajuda15_1.pdf";
                CONFIGURACAO_CALENDARIO_BLOQUEIO item = new CONFIGURACAO_CALENDARIO_BLOQUEIO();
                ConfiguracaoCalendarioBloqueioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO_BLOQUEIO, ConfiguracaoCalendarioBloqueioViewModel>(item);
                vm.COCB_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.COCB_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                vm.COCA_CD_ID = (Int32)Session["IdConfiguracaoCal"];
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirBloqueio(ConfiguracaoCalendarioBloqueioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização

                    // Critica de data
                    if (vm.COCB_DT_BLOQUEIO_INICIO == null)
                    {
                        Session["MensConfiguracao"] = 1;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0558", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.COCB_DT_BLOQUEIO_FINAL != null)
                    {
                        if (vm.COCB_DT_BLOQUEIO_FINAL.Value.Date < DateTime.Today.Date)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0559", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCB_DT_BLOQUEIO_INICIO > vm.COCB_DT_BLOQUEIO_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0560", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Critica de hora
                    if ((vm.COCB_HR_INICIO != null & vm.COCB_HR_FINAL == null))
                    {
                        Session["MensConfiguracao"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if ((vm.COCB_HR_INICIO == null & vm.COCB_HR_FINAL != null))
                    {
                        Session["MensConfiguracao"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    if (vm.COCB_HR_FINAL != null & vm.COCB_HR_INICIO != null)
                    {
                        if (vm.COCB_HR_INICIO == vm.COCB_HR_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCB_HR_INICIO > vm.COCB_HR_FINAL)
                        {
                            Session["MensConfiguracao"] = 6;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0563", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        Double? DifferenceInMinutes = Math.Abs((vm.COCB_HR_FINAL.Value - vm.COCB_HR_INICIO.Value).TotalMinutes);
                        if (DifferenceInMinutes < 60)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Verifica se tem consulta marcada nas faixas
                    List<PACIENTE_CONSULTA> cons = pacApp.GetAllConsultas(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_IN_ATIVO == 1).ToList();
                    if (vm.COCB_DT_BLOQUEIO_FINAL == null)
                    {
                        cons = cons.Where(p => p.PACO_DT_CONSULTA.Date == vm.COCB_DT_BLOQUEIO_INICIO.Value.Date).ToList();
                    }
                    else
                    {
                        cons = cons.Where(p => p.PACO_DT_CONSULTA.Date >= vm.COCB_DT_BLOQUEIO_INICIO.Value.Date & p.PACO_DT_CONSULTA.Date <= vm.COCB_DT_BLOQUEIO_FINAL.Value.Date).ToList();
                    }
                    if (vm.COCB_HR_INICIO != null & vm.COCB_HR_FINAL != null)
                    {
                        cons = cons.Where(p => p.PACO_HR_INICIO >= vm.COCB_HR_INICIO & p.PACO_HR_FINAL <= vm.COCB_HR_FINAL).ToList();
                    }
                    if (cons.Count > 0)
                    {
                        Session["MensConfiguracao"] = 7;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0564", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CONFIGURACAO_CALENDARIO_BLOQUEIO item = Mapper.Map<ConfiguracaoCalendarioBloqueioViewModel, CONFIGURACAO_CALENDARIO_BLOQUEIO>(vm);
                    Int32 volta = calApp.ValidateCreateBloqueio(item);

                    // Acerta estado
                    Session["IdBloqueio"] = item.COCB_CD_ID;
                    Session["ListaBloqueios"] = null;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "iblCOCA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CONFIGURACAO_CALENDARIO_BLOQUEIO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Bloqueio de horário incluído com sucesso";
                    Session["ListaBloqueios"] = null;
                    Session["MensConfiguracao"] = 10;

                    // retorno
                    if ((Int32)Session["VoltaBloqueio"] == 1)
                    {
                        return RedirectToAction("MontarTelaConfiguracaoCalendario");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaBloqueio");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarBloqueio(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas

                // Mensagem
                if (Session["MensConfiguracao"] != null)
                {
                    if ((Int32)Session["MensConfiguracao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0558", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0559", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0560", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0563", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0564", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensConfiguracao"] == 10)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "BLOQUEIO_EDITAR", "Configuracao", "EditarBloqueio");

                // Prepara view
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/15/Ajuda15_2.pdf";
                CONFIGURACAO_CALENDARIO_BLOQUEIO item = calApp.GetBloqueioById(id);
                ConfiguracaoCalendarioBloqueioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO_BLOQUEIO, ConfiguracaoCalendarioBloqueioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarBloqueio(ConfiguracaoCalendarioBloqueioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização

                    // Critica de data
                    if (vm.COCB_DT_BLOQUEIO_INICIO == null)
                    {
                        Session["MensConfiguracao"] = 1;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0558", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.COCB_DT_BLOQUEIO_FINAL != null)
                    {
                        if (vm.COCB_DT_BLOQUEIO_FINAL.Value.Date < DateTime.Today.Date)
                        {
                            Session["MensConfiguracao"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0559", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCB_DT_BLOQUEIO_INICIO > vm.COCB_DT_BLOQUEIO_FINAL)
                        {
                            Session["MensConfiguracao"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0560", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Critica de hora
                    if ((vm.COCB_HR_INICIO != null & vm.COCB_HR_FINAL == null))
                    {
                        Session["MensConfiguracao"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if ((vm.COCB_HR_INICIO == null & vm.COCB_HR_FINAL != null))
                    {
                        Session["MensConfiguracao"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0561", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.COCB_HR_FINAL != null & vm.COCB_HR_INICIO != null)
                    {
                        if (vm.COCB_HR_INICIO == vm.COCB_HR_FINAL)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.COCB_HR_INICIO > vm.COCB_HR_FINAL)
                        {
                            Session["MensConfiguracao"] = 6;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0563", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        Double? DifferenceInMinutes = Math.Abs((vm.COCB_HR_FINAL.Value - vm.COCB_HR_INICIO.Value).TotalMinutes);
                        if (DifferenceInMinutes < 60)
                        {
                            Session["MensConfiguracao"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0562", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Verifica se tem consulta marcada nas faixas
                    List<PACIENTE_CONSULTA> cons = pacApp.GetAllConsultas(idAss).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID & p.PACO_IN_ATIVO == 1).ToList();
                    if (vm.COCB_DT_BLOQUEIO_FINAL == null)
                    {
                        cons = cons.Where(p => p.PACO_DT_CONSULTA.Date == vm.COCB_DT_BLOQUEIO_INICIO.Value.Date).ToList();
                    }
                    else
                    {
                        cons = cons.Where(p => p.PACO_DT_CONSULTA.Date >= vm.COCB_DT_BLOQUEIO_INICIO.Value.Date & p.PACO_DT_CONSULTA.Date <= vm.COCB_DT_BLOQUEIO_FINAL.Value.Date).ToList();
                    }
                    if (vm.COCB_HR_INICIO != null & vm.COCB_HR_FINAL != null)
                    {
                        cons = cons.Where(p => p.PACO_HR_INICIO >= vm.COCB_HR_INICIO & p.PACO_HR_FINAL <= vm.COCB_HR_FINAL).ToList();
                    }
                    if (cons.Count > 0)
                    {
                        Session["MensConfiguracao"] = 7;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0564", CultureInfo.CurrentCulture));
                    }

                    // Executa a operação
                    CONFIGURACAO_CALENDARIO_BLOQUEIO item = Mapper.Map<ConfiguracaoCalendarioBloqueioViewModel, CONFIGURACAO_CALENDARIO_BLOQUEIO>(vm);
                    Int32 volta = calApp.ValidateEditBloqueio(item);

                    // Acerta estado
                    Session["IdBloqueio"] = item.COCB_CD_ID;
                    Session["ListaBloqueios"] = null;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eblCOCA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CONFIGURACAO_CALENDARIO_BLOQUEIO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Bloqueio de horário alterado com sucesso";
                    Session["MensConfiguracao"] = 10;
                    return RedirectToAction("MontarTelaConfiguracaoCalendario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Configuracao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirBloqueio(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONFIGURACAO_CALENDARIO_BLOQUEIO item = calApp.GetBloqueioById(id);
                item.COCB_IN_ATIVO = 0;
                Int32 volta = calApp.ValidateEditBloqueio(item);

                Session["BloqueioAlterada"] = 1;
                Session["ListaBloqueios"] = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xblCOCA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Identificador: " + item.COCB_GU_IDENTIFICADOR,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Retorno
                if ((Int32)Session["VoltaBloqueio"] == 1)
                {
                    return RedirectToAction("MontarTelaConfiguracaoCalendario");
                }
                else
                {
                    return RedirectToAction("MontarTelaBloqueio");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
        public ActionResult MontarTelaBloqueio()
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
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ModuloAtual"] = "Configuração - Bloqueio";

            // Recupera configuração
            List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
            objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfiguracaoCal"] = objetoCal.COCA_CD_ID;

            // Recupera bloqueios
            if (Session["ListaBloqueios"] == null)
            {
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs1 = bloqs.Where(p => p.COCB_DT_BLOQUEIO_INICIO == DateTime.Today.Date).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs2 = bloqs.Where(p => p.COCB_DT_BLOQUEIO_FINAL != null).ToList();
                bloqs2 = bloqs2.Where(p => p.COCB_DT_BLOQUEIO_INICIO < DateTime.Today.Date & p.COCB_DT_BLOQUEIO_FINAL >= DateTime.Today.Date).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqsFim = bloqs1;
                bloqsFim.AddRange(bloqs2);          
                Session["ListaBloqueios"] = bloqsFim;
            }
            ViewBag.Itens = (List<CONFIGURACAO_CALENDARIO_BLOQUEIO>)Session["ListaBloqueios"];

            // Monta listas

            // Mensagem
            if (Session["MensConfiguracao"] != null)
            {
                if ((Int32)Session["MensConfiguracao"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 10)
                {
                    ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                }
            }

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "BLOQUEIO", "Configuracao", "MontarTelaBloqueio");

            // Abre view
            Session["AjudaNivel"] = "../BaseAdmin/Ajuda/13/Ajuda13.pdf";
            Session["MensConfiguracao"] = 0;
            Session["VoltaBloqueio"] = 2;
            objetoCalAntes = objetoCal;
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfCal"] = 1;
            ConfiguracaoCalendarioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO, ConfiguracaoCalendarioViewModel>(objetoCal);
            return View(vm);
        }

        public ActionResult VerBloqueioAtivo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
                objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs1 = bloqs.Where(p => p.COCB_DT_BLOQUEIO_INICIO == DateTime.Today.Date).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs2 = bloqs.Where(p => p.COCB_DT_BLOQUEIO_FINAL != null).ToList();
                bloqs2 = bloqs2.Where(p => p.COCB_DT_BLOQUEIO_INICIO < DateTime.Today.Date & p.COCB_DT_BLOQUEIO_FINAL >= DateTime.Today.Date).ToList();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqsFim = bloqs1;
                bloqsFim.AddRange(bloqs2);
                Session["ListaBloqueios"] = bloqsFim;
                Session["VoltaInfoConsulta"] = 1;
                return RedirectToAction("MontarTelaBloqueio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerBloqueioAnterior()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
                objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_INICIO < DateTime.Today.Date).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_FINAL < DateTime.Today.Date || p.COCB_DT_BLOQUEIO_FINAL == null).ToList();
                Session["ListaBloqueios"] = bloqs;
                Session["VoltaInfoConsulta"] = 1;
                return RedirectToAction("MontarTelaBloqueio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerBloqueioFuturo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
                objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_INICIO > DateTime.Today.Date).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_FINAL > DateTime.Today.Date || p.COCB_DT_BLOQUEIO_FINAL == null).ToList();
                Session["ListaBloqueios"] = bloqs;
                Session["VoltaInfoConsulta"] = 1;
                return RedirectToAction("MontarTelaBloqueio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerBloqueio()
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
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];


            // Recupera configuração
            List<CONFIGURACAO_CALENDARIO> confs = calApp.GetAllItems(idAss);
            objetoCal = confs.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).FirstOrDefault();
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfiguracaoCal"] = objetoCal.COCA_CD_ID;

            // Recupera bloqueios
            if (Session["ListaBloqueios"] == null)
            {
                List<CONFIGURACAO_CALENDARIO_BLOQUEIO> bloqs = objetoCal.CONFIGURACAO_CALENDARIO_BLOQUEIO.Where(p => p.COCB_IN_ATIVO == 1).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_INICIO <= DateTime.Today.Date).ToList();
                bloqs = bloqs.Where(p => p.COCB_DT_BLOQUEIO_FINAL >= DateTime.Today.Date || p.COCB_DT_BLOQUEIO_FINAL == null).ToList();
                Session["ListaBloqueios"] = bloqs;
            }
            ViewBag.Itens = (List<CONFIGURACAO_CALENDARIO_BLOQUEIO>)Session["ListaBloqueios"];

            // Monta listas

            // Mensagem
            if (Session["MensConfiguracao"] != null)
            {
                if ((Int32)Session["MensConfiguracao"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0548", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0529", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0530", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0550", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensConfiguracao"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0549", CultureInfo.CurrentCulture));
                }
            }

            // Grava Acesso
            ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "BLOQUEIO_VER", "Configuracao", "VerBloqueio");

            // Abre view
            Session["MensConfiguracao"] = 0;
            Session["VoltaBloqueio"] = 3;
            objetoCalAntes = objetoCal;
            Session["ConfiguracaoCal"] = objetoCal;
            Session["IdConfCal"] = 1;
            ConfiguracaoCalendarioViewModel vm = Mapper.Map<CONFIGURACAO_CALENDARIO, ConfiguracaoCalendarioViewModel>(objetoCal);
            return View(vm);
        }

        public FileResult DownloadListaSono()
        {
            try
            {
                String arquivo = "../BaseAdmin/Documentacao/Perguntas_Anamnese_Sono.pdf";
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = "Perguntas_Anamnese.pdf";
                String contentType = "application/pdf";
                Session["NivelPaciente"] = 2;
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Configuracao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Configuracao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }


    }
}