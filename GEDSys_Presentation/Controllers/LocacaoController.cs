using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using EntitiesServices.WorkClasses;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using iTextSharp.text;
using iTextSharp.text.pdf;
using XidNet;
using CrossCutting;
using System.Collections;
using System.Threading.Tasks;
using DataServices.Repositories;
using System.Text;
using System.Net;
using System.Net.Mime;
using ERP_Condominios_Solution.Controllers;
using Microsoft.Ajax.Utilities;
using System.Net.Mail;
using System.Data.Entity;
using System.Data;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using OfficeOpenXml.Drawing.Slicer.Style;
using EntitiesServices.Work_Classes;
using iText.IO.Codec;

namespace GEDSys_Presentation.Controllers
{
    public class LocacaoController : Controller
    {
        private readonly ILocacaoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IPacienteAppService pacApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly ITemplateEMailAppService temApp;
        private readonly IProdutoAppService prodApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IEmpresaAppService empApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly IRecebimentoAppService recApp;

        private String msg;
        private Exception exception;
        LOCACAO objeto = new LOCACAO();
        LOCACAO objetoAntes = new LOCACAO();
        List<LOCACAO> listaMaster = new List<LOCACAO>();
        LOCACAO_HISTORICO objetoHistorico = new LOCACAO_HISTORICO();
        List<LOCACAO_HISTORICO> listaMasterHistorico = new List<LOCACAO_HISTORICO>();
        LOCACAO_PARCELA objetoParcela = new LOCACAO_PARCELA();
        List<LOCACAO_PARCELA> listaMasterParcela = new List<LOCACAO_PARCELA>();
        CONTRATO_LOCACAO objetoContrato = new CONTRATO_LOCACAO();
        List<CONTRATO_LOCACAO> listaMasterContrato = new List<CONTRATO_LOCACAO>();
        CONTRATO_LOCACAO objetoAntesContrato = new CONTRATO_LOCACAO();
        String extensao;

        public LocacaoController(ILocacaoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IAcessoMetodoAppService aceApps, IPacienteAppService pacApps, IMensagemEnviadaSistemaAppService meApps, ITemplateEMailAppService temApps, IProdutoAppService prodApps, IPeriodicidadeAppService perApps, IEmpresaAppService empApps, IRecursividadeAppService recuApps, ITemplateSMSAppService smsApps, IRecebimentoAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            aceApp = aceApps;
            pacApp = pacApps;
            meApp = meApps;
            temApp = temApps;
            prodApp = prodApps;
            perApp = perApps;
            empApp = empApps;
            recuApp = recuApps;
            smsApp = smsApps;
            recApp = recApps;
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
        public ActionResult MontarTelaLocacao()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacao"] == null)
                {
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacao"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacao"];
                List<SelectListItem> status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Pendente", Value = "0" });
                status.Add(new SelectListItem() { Text = "Ativa", Value = "1" });
                status.Add(new SelectListItem() { Text = "Encerrada", Value = "2" });
                status.Add(new SelectListItem() { Text = "Vencida", Value = "3" });
                status.Add(new SelectListItem() { Text = "Cancelada", Value = "4" });
                ViewBag.Status = new SelectList(status, "Value", "Text");
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Relação de Locações*", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Locações/Data", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Locações/Mês", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Data", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Mês", Value = "5" });
                relat.Add(new SelectListItem() { Text = "Parcelas em Atraso", Value = "6" });
                relat.Add(new SelectListItem() { Text = "Pacientes/Locação", Value = "7" });
                relat.Add(new SelectListItem() { Text = "Produtos/Locação", Value = "8" });
                relat.Add(new SelectListItem() { Text = "Locações/Status", Value = "9" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Produto", Value = "10" });
                relat.Add(new SelectListItem() { Text = "Pendentes", Value = "11" });
                relat.Add(new SelectListItem() { Text = "Ativas", Value = "12" });
                relat.Add(new SelectListItem() { Text = "Canceladas", Value = "13" });
                relat.Add(new SelectListItem() { Text = "Encerradas", Value = "14" });
                relat.Add(new SelectListItem() { Text = "Vencidas", Value = "15" });
                relat.Add(new SelectListItem() { Text = "A Encerrar", Value = "16" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

                // Widgets
                Int32? locPendentes = locs.Where(p => p.LOCA_IN_STATUS == 0).Count();
                Int32? locAtivas = locs.Where(p => p.LOCA_IN_STATUS == 1).Count();
                Int32? locEncerradas = locs.Where(p => p.LOCA_IN_STATUS == 2).Count();
                Int32? locVencidas = locs.Where(p => p.LOCA_IN_STATUS == 3).Count();
                Int32? locCanceladas = locs.Where(p => p.LOCA_IN_STATUS == 4).Count();

                ViewBag.LocPendentes = locPendentes;
                ViewBag.LocAtivas = locAtivas;
                ViewBag.LocEncerradas = locEncerradas;
                ViewBag.LocVencidas = locVencidas;
                ViewBag.LocCanceladas = locCanceladas;


                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                    if ((Int32)Session["MensLocacao"] == 88)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0696", CultureInfo.CurrentCulture));
                    }
                }
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 62)
                    {
                        TempData["MensagemAcerto1"] = (String)Session["MsgCRUD1"];
                        TempData["TemMensagem1"] = 1;
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO", "Locacao", "MontarTelaLocacao");

                // Abre view
                Session["TipoLocacao"] = 2;
                Session["TipoLocacaoRel"] = 0;
                Session["MensLocacao"] = null;
                Session["MensProduto"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                Session["NivelLocacao"] = 1;
                Session["VoltaLocacaoBase"] = 2;
                Session["VoltaLocacaoParcela"] = 1;
                Session["VoltaPrintLocacao"] = 1;
                Session["VoltaProdLocacao"] = 1;
                Session["VoltaHistLocacao"] = 1;
                Session["TipoSolicitacao"] = 98;
                Session["VoltaProduto"] = 71;
                Session["VoltarLocacaoBase"] = 5;

                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacao"] = null;
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacao"] = listaMaster;
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacao");
        }

        [HttpPost]
        public ActionResult FiltrarLocacao(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacao");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2;
                Session["ListaLocacao"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirLocacao()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locacao - Inclusão";
                        return RedirectToAction("MontarTelaLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao - Inclusão";
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.Pacientes = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
                ViewBag.Produtos = new SelectList(CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 2 & p.PROD_IN_LOCACAO == 1), "PROD_CD_ID", "PROD_NM_NOME");
                ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 1), "COLO_CD_ID", "COLO_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                List<SelectListItem> garantia = new List<SelectListItem>();
                garantia.Add(new SelectListItem() { Text = "Não", Value = "0" });
                garantia.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Garantia = new SelectList(garantia, "Value", "Text");

                List<SelectListItem> contrato = new List<SelectListItem>();
                contrato.Add(new SelectListItem() { Text = "Não", Value = "0" });
                contrato.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Contrato = new SelectList(contrato, "Value", "Text");

                List<SelectListItem> preco = new List<SelectListItem>();
                preco.Add(new SelectListItem() { Text = "Normal", Value = "1" });
                preco.Add(new SelectListItem() { Text = "Promoção", Value = "2" });
                ViewBag.Preco = new SelectList(preco, "Value", "Text");

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_INCLUIR", "Locacao", "IncluirLocacao");

                // Prepara view
                Session["MensLocacao"] = null;
                LOCACAO item = new LOCACAO();
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.LOCA_IN_ATIVO = 1;
                vm.LOCA_GU_GUID = Xid.NewXid().ToString();
                vm.LOCA_IN_ENCERRADO = 0;
                vm.LOCA_IN_RENOVACAO = 0;
                vm.LOCA_IN_RENOVACOES = 0;
                vm.LOCA_IN_STATUS = 0;
                vm.LOCA_VL_PARCELA = 0;
                vm.LOCA_IN_QUANTIDADE = 1;
                vm.LOCA_VL_TOTAL = 0;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.LOCA_DT_INICIO = DateTime.Today.Date;
                vm.LOCA_NR_PRAZO = 12;
                vm.LOCA_NR_DIA = 30;
                vm.PETA_CD_ID = 1;
                vm.LOCA_IN_CONTRATO = 0;
                vm.LOCA_IN_GARANTIA = 0;
                vm.LOCA_TK_TOKEN = Cryptography.GenerateToken(6);
                vm.USUARIO = usuario;
                vm.LOCA_IN_ESTOQUE = 0;
                vm.LOCA_IN_CONTRATO_ASSINA = 0;
                vm.TIPO_PRECO = 1;
                vm.LOCA_IN_ASSINADO_DIGITAL = conf.CONF_IN_ASSINA_DIGITAL_LOCACAO;
                if ((Int32)Session["TipoLocacao"] == 1)
                {
                    PACIENTE pac = pacApp.GetItemById((Int32)Session["IdPaciente"]);
                    vm.PACI_CD_ID = (Int32)Session["IdPaciente"];
                    vm.PACIENTE = pac;
                    Session["PacienteGet"] = pac;
                    ViewBag.NomePaciente = pac.PACI_NM_NOME;
                }
                else
                {
                    vm.PACI_CD_ID = 0;
                    vm.PACIENTE = null;
                    ViewBag.NomePaciente = String.Empty;
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> IncluirLocacao(LocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Pacientes = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
            ViewBag.Produtos = new SelectList(CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 2 & p.PROD_IN_LOCACAO == 1), "PROD_CD_ID", "PROD_NM_NOME");
            ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 1), "COLO_CD_ID", "COLO_NM_NOME");
            List<SelectListItem> garantia = new List<SelectListItem>();
            garantia.Add(new SelectListItem() { Text = "Não", Value = "0" });
            garantia.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Garantia = new SelectList(garantia, "Value", "Text");
            List<SelectListItem> contrato = new List<SelectListItem>();
            contrato.Add(new SelectListItem() { Text = "Não", Value = "0" });
            contrato.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Contrato = new SelectList(contrato, "Value", "Text");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            preco.Add(new SelectListItem() { Text = "Promoção", Value = "2" });
            ViewBag.Preco = new SelectList(preco, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOCA_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_DS_DESCRICAO);
                    vm.LOCA_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_NM_TITULO);
                    vm.LOCA_NR_SERIE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_NR_SERIE);

                    // Critica
                    if (vm.LOCA_VL_PARCELA == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0691", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOCA_NR_PRAZO == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0692", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOCA_IN_QUANTIDADE == 0)
                    {
                        vm.LOCA_IN_QUANTIDADE = 1;
                    }
                    if (vm.LOCA_IN_CONTRATO == 1)
                    {
                        if (vm.COLO_CD_ID == 0 || vm.COLO_CD_ID == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0708", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Recupera
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    Int32 periodo = 30;

                    // Calcula total e data final
                    Decimal? total = vm.LOCA_VL_PARCELA * vm.LOCA_NR_PRAZO;
                    vm.LOCA_VL_TOTAL = total;
                    vm.LOCA_DT_FINAL = vm.LOCA_DT_INICIO.Value.AddMonths(vm.LOCA_NR_PRAZO.Value);
                    vm.PETA_CD_ID = 1;
                    vm.LOCA_DT_EMISSAO = DateTime.Now;

                    // Monta titulo e descrição
                    if (vm.LOCA_NM_TITULO == null || vm.LOCA_NM_TITULO == String.Empty)
                    {
                        String titulo = "Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper();
                    }
                    String descricao = "Dados da Locação: " + Environment.NewLine + "Produto: " + prod.PROD_NM_NOME + Environment.NewLine + "Paciente: " + pac.PACI_NM_NOME + Environment.NewLine + "Valor da parcela (R$): " + CrossCutting.Formatters.DecimalFormatter(vm.LOCA_VL_PARCELA.Value) + Environment.NewLine + "Número de parcelas: " + vm.LOCA_NR_PRAZO.ToString() + Environment.NewLine + "Total (R$): " + CrossCutting.Formatters.DecimalFormatter(vm.LOCA_VL_TOTAL.Value);
                    descricao += Environment.NewLine + vm.LOCA_DS_DESCRICAO;
                    vm.LOCA_DS_DESCRICAO = descricao;

                    // Calcula valor de atraso
                    if (conf.CONF_VL_ACRESCIMO_ATRASO_PARCELA > 0)
                    {
                        vm.LOCA_VL_VALOR_ATRASO = vm.LOCA_VL_PARCELA * (1 + (conf.CONF_VL_ACRESCIMO_ATRASO_PARCELA / 100));
                    }
                    else
                    {
                        vm.LOCA_VL_VALOR_ATRASO = vm.LOCA_VL_PARCELA;
                    }

                    // Preparação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Serializa
                    String json = JsonConvert.SerializeObject(item);
                    Session["JSONLocacao"] = json;

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);
                    Session["IdLocacao"] = item.LOCA_CD_ID;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensLocacao"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0693", CultureInfo.CurrentCulture));
                        vm.PROD_CD_ID = 0;
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Contrato/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/QRCode/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Distrato/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Assinado/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Cria parcelas
                    DateTime? venc = CalculaVencimento(vm.LOCA_NR_DIA, 1, periodo);
                    for (Int32 i = 1; i <= vm.LOCA_NR_PRAZO; i++)
                    {
                        Int32 numParc = i;

                        // Monta e grava
                        LOCACAO_PARCELA parc = new LOCACAO_PARCELA();
                        parc.ASSI_CD_ID = idAss;
                        parc.LOCA_CD_ID = item.LOCA_CD_ID;
                        parc.LOPA_DS_DESCRICAO = "Parcela " + numParc.ToString() + " da locação " + item.LOCA_NM_TITULO;
                        parc.LOPA_DT_VENCIMENTO = venc;
                        parc.LOPA_IN_ATIVO = 1;
                        parc.LOPA_IN_PARCELA = 1;
                        parc.LOPA_IN_QUITADA = 0;
                        parc.LOPA_IN_STATUS = 1;
                        parc.LOPA_NM_PARCELAS = "Parcela " + numParc.ToString();
                        parc.LOPA_NR_PACELAS = numParc;
                        parc.LOPA_VL_DESCONTO = 0;
                        parc.LOPA_VL_JUROS = 0;
                        parc.LOPA_VL_TAXAS = 0;
                        parc.LOPA_VL_VALOR = item.LOCA_VL_PARCELA;
                        parc.LOPA_VL_VALOR_PAGO = 0;
                        Int32 voltaParc = baseApp.ValidateCreateParcela(parc);

                        // Ajustes
                        venc = venc.Value.AddMonths(1);
                        if (venc.Value.Month != 2)
                        {
                            DateTime novaData = new DateTime(
                                venc.Value.Year,
                                venc.Value.Month,
                                vm.LOCA_NR_DIA.Value
                            );
                            venc = novaData;
                        }

                        // Monta Log
                        DTO_Parcela dto = MontarParcelaDTO(parc.LOPA_CD_ID);
                        String json1 = JsonConvert.SerializeObject(dto, settings);
                        LOG log = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuario.ASSI_CD_ID,
                            USUA_CD_ID = usuario.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Locação - Parcela - Inclusão",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json1,
                            LOG_IN_SISTEMA = 6
                        };
                        Int32 volta1 = logApp.ValidateCreate(log);
                    }

                    // Trata anexos
                    if (Session["FileQueueLocacao"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueLocacao"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                Int32 volta3 = UploadFileQueueLocacao(file);
                            }
                        }
                        Session["FileQueuePaciente"] = null;
                    }

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Criação da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Criação de Locação Pendente";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Prepara QRCode
                    String fileNameQR = "Contrato_QRCode_" + item.LOCA_GU_GUID + ".png";
                    String caminhoQR = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/QRCode/";
                    String pathQR = Path.Combine(Server.MapPath(caminhoQR), fileNameQR);

                    // Gera e grava QRCode
                    LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                    String linkBase = "https://webdoctorformbase.azurewebsites.net/api/ExibirFormulario";
                    String sufixo = "?Token=" + loca.LOCA_TK_TOKEN;
                    sufixo += "&ID=" + loca.LOCA_CD_ID.ToString();
                    sufixo += "&Tipo=1";
                    String url = linkBase + sufixo;
                    QrCodeHelper.GenerateQrCodeAndSave(url, pathQR);
                    loca.LOCA_AQ_ARQUIVO_QRCODE = "~" + caminhoQR + fileNameQR;
                    Int32 voltaP = baseApp.ValidateEdit(loca, loca, usuario);

                    // Envia mensagem
                    LOCACAO locMensagem = baseApp.GetItemById(item.LOCA_CD_ID);
                    if (pac.PACI_NM_EMAIL != null)
                    {
                        Int32 voltaCons = await EnviarEMailLocacao(locMensagem, 1);
                    }
                    if (pac.PACI_NR_CELULAR != null)
                    {
                        Int32 voltaCons = EnviarSMSLocacao(locMensagem, 1);
                    }

                    // Sucesso
                    listaMaster = new List<LOCACAO>();
                    Session["ListaLocacao"] = null;
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["ListaHistoricoLocacaoGeral"] = null;
                    Session["LocacoesHistoricos"] = null;

                    Session["ListaLocacaoData"] = null;
                    Session["ListaParcelaData"] = null;
                    Session["ListaLocacaoMes"] = null;
                    Session["ListaParcelaMes"] = null;
                    Session["ListaLocacaoStatus"] = null;
                    Session["ListaLocacaoProduto"] = null;
                    Session["ListaLocacaoPaciente"] = null;
                    Session["ListaLocacaoVencida"] = null;
                    Session["ListaRecebeProduto"] = null;
                    Session["ListaParcelaAtraso"] = null;
                    Session["ListaLocacaoEncerra"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper() + " foi criada com sucesso. Foram geradas " + item.LOCA_NR_PRAZO.ToString() + " parcelas.";
                    Session["MensLocacao"] = 61;

                    // Trata contrato
                    if (item.LOCA_IN_CONTRATO == 1)
                    {
                        if (item.LOCA_IN_ASSINADO_DIGITAL == 0)
                        {
                            Int32 rel = GerarContratoPDFTeste();
                        }
                        else
                        {
                            Int32 rel = GerarContratoPDFTesteAssina();

                        }
                        Int32 voltaC = await ProcessaEnvioEMailContrato(item, usuario);
                    }

                    // Retorno
                    return RedirectToAction("VoltarAnexoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public DateTime? CalculaVencimento(Int32? diaBase, Int32 inicio, Int32 peta)
        {
            // Hoje
            DateTime hoje = DateTime.Today.Date;
            Int32 dia = hoje.Day;
            Int32 mes = hoje.Month;
            Int32 ano = hoje.Year;

            // Incremento
            Int32 incremento = peta;

            // Calculo
            DateTime dataBase = Convert.ToDateTime(diaBase + "/" + mes + "/" + ano);
            if (dataBase < hoje)
            {
                dataBase = dataBase.AddMonths(1);
            }

            return dataBase;
        }

        public async Task<Int32> EnviarEMailLocacao(LOCACAO locacao, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
            PRODUTO prod = prodApp.GetItemById(locacao.PROD_CD_ID);
            PERIODICIDADE_TAREFA peta = perApp.GetItemById(locacao.PETA_CD_ID);
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = new TEMPLATE_EMAIL();
                if (tipo == 1)
                {
                    template = temApp.GetByCode("CRIALOCA", idAss);
                }
                else if (tipo == 2)
                {
                    template = temApp.GetByCode("CANCLOCA", idAss);
                }
                else if (tipo == 3)
                {
                    template = temApp.GetByCode("APROLOCA", idAss);
                }
                else if (tipo == 4)
                {
                    template = temApp.GetByCode("ENCRLOCA", idAss);
                }

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;

                // Prepara assinatura
                String assinatura = String.Empty;
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
                assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
                assinatura += "Enviado por <b>WebDoctor</b><br />";

                // Prepara corpo da mensagem
                String texto = template.TEEM_TX_CORPO;
                String urlDestino = conf.CONF_LK_LINK_SISTEMA;
                String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";

                if (tipo == 1)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{final}"))
                    {
                        texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                    }
                    if (texto.Contains("{dia}"))
                    {
                        texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                    }
                    if (texto.Contains("{parcela}"))
                    {
                        texto = texto.Replace("{parcela}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                    }
                    if (texto.Contains("{vencimento}"))
                    {
                        texto = texto.Replace("{vencimento}", locacao.LOCA_NR_DIA.ToString());
                    }
                    if (texto.Contains("{link}"))
                    {
                        texto = texto.Replace("{link}", linkHtml);
                    }
                }
                else if (tipo == 2)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{final}"))
                    {
                        texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                    }
                    if (texto.Contains("{cancelamento}"))
                    {
                        texto = texto.Replace("{cancelamento}", locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{justificativa}"))
                    {
                        texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                    }
                    if (texto.Contains("{link}"))
                    {
                        texto = texto.Replace("{link}", linkHtml);
                    }
                }
                else if (tipo == 3)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{final}"))
                    {
                        texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                    }
                    if (texto.Contains("{aprovacao}"))
                    {
                        texto = texto.Replace("{aprovacao}", locacao.LOCA_DT_APROVACAO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{link}"))
                    {
                        texto = texto.Replace("{link}", linkHtml);
                    }
                }
                else if (tipo == 4)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{final}"))
                    {
                        texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                    }
                    if (texto.Contains("{encerramento}"))
                    {
                        texto = texto.Replace("{encerramento}", locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{observacao}"))
                    {
                        texto = texto.Replace("{observacao}", locacao.LOCA_DS_ENCERRA);
                    }
                    if (texto.Contains("{link}"))
                    {
                        texto = texto.Replace("{link}", linkHtml);
                    }
                }

                // Monta mensagem
                String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                if (tipo == 1)
                {
                    mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação";
                }
                else if (tipo == 2)
                {
                    mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - Cancelamento";
                }
                else if (tipo == 3)
                {
                    mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - Aprovação";
                }
                else if (tipo == 4)
                {
                    mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - Encerramento";
                }
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
                    await CrossCutting.CommunicationAzurePackage.SendMailAsyncNew(mensagem);
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
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                if (tipo == 1)
                {
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Locação: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 2)
                {
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Locação - Cancelamento: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 3)
                {
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Locação - Aprovação: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 4)
                {
                    mens.MENS_NM_NOME = "Mensagem para Paciente - Locação - Encerramento: " + paciente.PACI_NM_NOME.ToUpper();
                }
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = 0;
                if (tipo == 1)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de Locação - " + paciente.PACI_NM_NOME);
                }
                else if (tipo == 2)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Cancelamento de Locação - " + paciente.PACI_NM_NOME);
                }
                else if (tipo == 3)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Aprovação de Locação - " + paciente.PACI_NM_NOME);
                }
                else if (tipo == 4)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Encerramento de Locação - " + paciente.PACI_NM_NOME);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Envio de e-mail",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = (String)Session["JSONLocacao"],
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta3 = logApp.ValidateCreate(log);

                // Sucesso
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                return 1;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 EnviarSMSLocacao(LOCACAO locacao, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
            PRODUTO prod = prodApp.GetItemById(locacao.PROD_CD_ID);

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_SMS template = new TEMPLATE_SMS();
                if (tipo == 1)
                {
                    template = smsApp.GetByCode("CRIALOCA", idAss);
                }
                else if (tipo == 2)
                {
                    template = smsApp.GetByCode("CANCLOCA", idAss);
                }
                else if (tipo == 3)
                {
                    template = smsApp.GetByCode("APROLOCA", idAss);
                }
                else if (tipo == 4)
                {
                    template = smsApp.GetByCode("ENCRLOCA", idAss);
                }

                // Prepara assinatura
                String assinatura = String.Empty;
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                assinatura = emp.EMPR_NM_NOME;
                assinatura += " CNPJ: " + emp.EMPR_NR_CNPJ;
                assinatura += " Enviado por WebDoctor";

                // Prepara corpo da mensagem
                String texto = template.TSMS_TX_CORPO;
                if (tipo == 1)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{assinatura}"))
                    {
                        texto = texto.Replace("{assinatura}", assinatura);
                    }
                }
                else if (tipo == 2)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{cancelamento}"))
                    {
                        texto = texto.Replace("{cancelamento}", locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{assinatura}"))
                    {
                        texto = texto.Replace("{assinatura}", assinatura);
                    }
                }
                else if (tipo == 3)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{aprovacao}"))
                    {
                        texto = texto.Replace("{aprovacao}", locacao.LOCA_DT_APROVACAO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{assinatura}"))
                    {
                        texto = texto.Replace("{assinatura}", assinatura);
                    }
                }
                else if (tipo == 4)
                {
                    if (texto.Contains("{produto}"))
                    {
                        texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{paciente}"))
                    {
                        texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                    }
                    if (texto.Contains("{encerramento}"))
                    {
                        texto = texto.Replace("{encerramento}", locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{assinatura}"))
                    {
                        texto = texto.Replace("{assinatura}", assinatura);
                    }
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
                if (tipo == 1)
                {
                    mens.MENS_NM_NOME = "Mensagem SMS para Paciente - Locação: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 2)
                {
                    mens.MENS_NM_NOME = "Mensagem SMS para Paciente - Cancelamento: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 3)
                {
                    mens.MENS_NM_NOME = "Mensagem SMS para Paciente - Aprovação: " + paciente.PACI_NM_NOME.ToUpper();
                }
                else if (tipo == 4)
                {
                    mens.MENS_NM_NOME = "Mensagem SMS para Paciente - Encerramento: " + paciente.PACI_NM_NOME.ToUpper();
                }
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = smsBody;
                mens.MENS_IN_TIPO = 2;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = 0;
                if (tipo == 1)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de Locação - SMS - " + paciente.PACI_NM_NOME.ToUpper());
                }
                else if (tipo == 2)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Cancelamento de Locação - SMS - " + paciente.PACI_NM_NOME.ToUpper());
                }
                else if (tipo == 3)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Aprovação de Locação - SMS - " + paciente.PACI_NM_NOME.ToUpper());
                }
                else if (tipo == 4)
                {
                    volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Encerramento de Locação - SMS - " + paciente.PACI_NM_NOME.ToUpper());
                }

                // Sucesso
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                return 1;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
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
            Session["FileQueueLocacao"] = queue;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileLocacao(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdLocacao"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                LOCACAO item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensLocacao"] = 5;
                    return RedirectToAction("VoltarEditarLocacao");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensLocacao"] = 6;
                    return RedirectToAction("VoltarEditarLocacao");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensLocacao"] = 7;
                    return RedirectToAction("VoltarEditarLocacao");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensLocacao"] = 12;
                    return RedirectToAction("VoltarEditarLocacao");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                // Gravar registro
                LOCACAO_ANEXO foto = new LOCACAO_ANEXO();
                foto.LOAX_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.LOAX_DT_ANEXO = DateTime.Today;
                foto.LOAX_IN_ATIVO = 1;
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
                foto.LOAX_IN_TIPO = tipo;
                foto.LOAX_NM_TITULO = fileName;
                foto.LOCA_CD_ID = item.LOCA_CD_ID;

                item.LOCACAO_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEdit(item, item, usu);
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;
                Session["LocacaoAlterada"] = 1;
                return RedirectToAction("VoltarEditarLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarEditarLocacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        [HttpPost]
        public Int32 UploadFileQueueLocacao(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdLocacao"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensLocacao"] = 5;
                    return 1;
                }

                // Recupera exame
                LOCACAO item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensLocacao"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensLocacao"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensLocacao"] = 12;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                LOCACAO_ANEXO foto = new LOCACAO_ANEXO();
                foto.LOAX_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.LOAX_DT_ANEXO = DateTime.Today;
                foto.LOAX_IN_ATIVO = 1;
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
                foto.LOAX_IN_TIPO = tipo;
                foto.LOAX_NM_TITULO = fileName;
                foto.LOCA_CD_ID = item.LOCA_CD_ID;
                item.LOCACAO_ANEXO.Add(foto);
                Int32 volta = baseApp.ValidateEdit(item, item, usu);
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        public JsonResult GetProduto(Int32 id)
        {
            var produto = prodApp.GetItemById(id);
            Session["ProdutoGet"] = produto;
            String nome = String.Empty;
            if (Session["PacienteGet"] != null)
            {
                nome = ((PACIENTE)Session["PacienteGet"]).PACI_NM_NOME;

            }
            String tit = produto.PROD_NM_NOME.ToUpper() + " para " + nome.ToUpper();
            Decimal? tot = produto.PROD_VL_LOCACAO * 12;
            var hash = new Hashtable();
            hash.Add("marca", produto.PROD_NM_MARCA);
            hash.Add("modelo", produto.PROD_NM_MODELO);
            hash.Add("valor", CrossCutting.Formatters.DecimalFormatter(produto.PROD_VL_LOCACAO.Value));
            hash.Add("parc", CrossCutting.Formatters.DecimalFormatter(produto.PROD_VL_LOCACAO.Value));
            hash.Add("titulo", tit);
            hash.Add("total", CrossCutting.Formatters.DecimalFormatter(tot.Value));
            hash.Add("prazo", 12);
            hash.Add("quant", 1);
            if (produto.PROD_VL_PRECO_PROMOCAO != null)
            {
                hash.Add("promo", CrossCutting.Formatters.DecimalFormatter(produto.PROD_VL_PRECO_PROMOCAO.Value));
                hash.Add("desc", CrossCutting.Formatters.DecimalFormatter(produto.PROD_VL_PRECO_PROMOCAO.Value));
            }
            return Json(hash);
        }

        [HttpGet]
        public ActionResult EditarLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Edição";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locação - Edição";

                // Trata mensagens
                if (Session["MensLocacao"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensLocacao"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 69)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0539", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 70)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0540", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 88)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0696", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 91)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 62)
                    {
                        TempData["MensagemAcerto1"] = (String)Session["MsgCRUD1"];
                        TempData["TemMensagem1"] = 1;
                    }
                }

                // Prepara view
                Session["NivelPaciente"] = 17;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";
                Session["VoltaLocacao"] = 2;
                Session["VoltaMailLocacao"] = 1;
                Session["VoltaHistLocacao"] = 2;
                Session["MensProduto"] = null;
                Session["MensLocacao"] = null;
                Session["VoltaLocacaoParcela"] = 1;

                LOCACAO item = baseApp.GetItemById(id);
                Session["IdPaciente"] = item.PACI_CD_ID;
                Session["IdLocacao"] = item.LOCA_CD_ID;
                objetoAntes = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                Session["LocacaoAntes"] = item;

                PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID);
                vm.PACIENTE = pac;
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                // Verifica se tem contrato assinado
                String nomeArq = "Contrato_Locacao" + pac.PACI_NM_NOME + "_" + item.LOCA_GU_GUID + ".pdf";
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Assinado/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeArq);
                Boolean existe = System.IO.File.Exists(filePath);
                vm.CONTRATO_ASSINA = existe ? "Sim" : "Não";
                vm.CONTRATO_ASSINA = vm.LOCA_IN_CONTRATO_ASSINA == 1 ? "Sim" : "Não";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_EDITAR", "Locacao", "EditarLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLocacao(LocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOCA_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_DS_DESCRICAO);
                    vm.LOCA_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_NM_TITULO);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);

                    // Executa a operação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    LOCACAO antes = (LOCACAO)Session["LocacaoAntes"];
                    Int32 volta = baseApp.ValidateEdit(item, antes, usuarioLogado);

                    // Verifica retorno
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaLocacao"] = null;

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Alteração da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Alteração na Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper()+ " foi alterada com sucesso";
                    Session["MensLocacao"] = 61;

                    Session["ListaLocacaoData"] = null;
                    Session["ListaParcelaData"] = null;
                    Session["ListaLocacaoMes"] = null;
                    Session["ListaParcelaMes"] = null;
                    Session["ListaLocacaoStatus"] = null;
                    Session["ListaLocacaoProduto"] = null;
                    Session["ListaLocacaoPaciente"] = null;
                    Session["ListaLocacaoVencida"] = null;
                    Session["ListaRecebeProduto"] = null;
                    Session["ListaParcelaAtraso"] = null;
                    Session["ListaLocacaoEncerra"] = null;

                    // Retorno
                    if ((Int32)Session["TipoLocacao"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["VoltarLocacaoBase"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPaciente");
                    }
                    if ((Int32)Session["VoltarLocacaoBase"] == 3)
                    {
                        return RedirectToAction("VoltarAnexoProduto", "Produto");
                    }
                    return RedirectToAction("VoltarAnexoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarAnexoLocacao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        [HttpGet]
        public ActionResult ExcluirLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO__EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Exclusão";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LOCACAO item = baseApp.GetItemById(id);
                objetoAntes = (LOCACAO)Session["LocacaoAntes"];
                item.LOCA_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                Session["LocacaoAlterada"] = 1;
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                Session["ListaLocacao"] = null;
                Session["LocacaoAlterada"] = 1;

                // Grava historico
                LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                hist.ASSI_CD_ID = idAss;
                hist.LOCA_CD_ID = item.LOCA_CD_ID;
                hist.LOHI_DS_DESCRICAO = "Exclusão da Locação " + item.LOCA_NM_TITULO;
                hist.LOHI_DT_HISTORICO = DateTime.Now;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.LOHI_IN_ATIVO = 1;
                hist.LOHI_NM_OPERACAO = "Exclusão de Locação";
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " foi excluída com sucesso";
                Session["MensLocacao"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Consulta";
                        return RedirectToAction("MontarTelaLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";

                LOCACAO item = baseApp.GetItemById(id);
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                Session["IdLocacao"] = item.LOCA_CD_ID;
                Session["Locacao"] = item;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_VER", "Locacao", "VerLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoLocacao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara view
                LOCACAO_ANEXO item = baseApp.GetLocacaoAnexoById(id);
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;

                PACIENTE pac = pacApp.GetItemById(item.LOCACAO.PACI_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ANEXO", "Locacao", "VerAnexoLocacao");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoLocacaoAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara view
                LOCACAO_ANEXO item = baseApp.GetLocacaoAnexoById(id);
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;

                PACIENTE pac = pacApp.GetItemById(item.LOCACAO.PACI_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ANEXO", "Locacao", "VerAnexoLocacaoAudio");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexoLocacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LOCACAO_ANEXO item = baseApp.GetLocacaoAnexoById(id);
                PACIENTE pac = pacApp.GetItemById(item.LOCACAO.PACI_CD_ID);

                item.LOAX_IN_ATIVO = 0;
                DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                Int32 volta = baseApp.ValidateEditLocacaoAnexo(item);

                // Monta Log
                String json = JsonConvert.SerializeObject(dto);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;
                Session["PacienteAlterada"] = 1;
                return RedirectToAction("VoltarAnexoLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public FileResult DownloadLocacao(Int32 id)
        {
            try
            {
                LOCACAO_ANEXO item = baseApp.GetLocacaoAnexoById(id);
                String arquivo = item.LOAX_AQ_ARQUIVO;
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
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult IncluirAnotacaoLocacao()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Alteração";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 2;

                LOCACAO item = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LOCACAO_ANOTACAO coment = new LOCACAO_ANOTACAO();
                LocacaoAnotacaoViewModel vm = Mapper.Map<LOCACAO_ANOTACAO, LocacaoAnotacaoViewModel>(coment);
                vm.LOAN_DT_ANOTACAO = DateTime.Now;
                vm.LOAN_IN_ATIVO = 1;
                vm.LOCA_CD_ID = item.LOCA_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;

                PACIENTE cli = pacApp.GetItemById(item.PACI_CD_ID);
                ViewBag.NomePaciente = cli.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ANOTACAO_INCLUIR", "Locacao", "IncluirAnotacaoLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "Locacao", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirAnotacaoLocacao(LocacaoAnotacaoViewModel vm)
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
                    vm.LOAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.LOAN_TX_ANOTACAO);

                    // Executa a operação
                    LOCACAO_ANOTACAO item = Mapper.Map<LocacaoAnotacaoViewModel, LOCACAO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LOCACAO not = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                    String json = JsonConvert.SerializeObject(item);

                    item.USUARIO = null;
                    not.LOCACAO_ANOTACAO.Add(item);
                    Int32 volta = baseApp.ValidateEdit(not, not, usuarioLogado);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Anotação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 3;
                    Session["VoltarPesquisa"] = 0;
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    return RedirectToAction("VoltarAnexoLocacao");
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
        public ActionResult EditarAnotacaoLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Alteração";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 3;
                LOCACAO_ANOTACAO item = baseApp.GetAnotacaoById(id);
                LocacaoAnotacaoViewModel vm = Mapper.Map<LOCACAO_ANOTACAO, LocacaoAnotacaoViewModel>(item);

                PACIENTE cli = pacApp.GetItemById(item.LOCACAO.PACI_CD_ID);
                ViewBag.NomePaciente = cli.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ANOTACAO_EDITAR", "Locacao", "EditarAnotacaoLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoLocacao(LocacaoAnotacaoViewModel vm)
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
                    vm.LOAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.LOAN_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LOCACAO_ANOTACAO item = Mapper.Map<LocacaoAnotacaoViewModel, LOCACAO_ANOTACAO>(vm);
                    Int32 volta = baseApp.ValidateEditAnotacao(item);

                    // Monta Log
                    DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Anotação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 3;
                    return RedirectToAction("VoltarAnexoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacaoLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Alteração";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LOCACAO_ANOTACAO item = baseApp.GetAnotacaoById(id);
                item.LOAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnotacao(item);
                Session["LocacaoAlterada"] = 1;
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 3;

                // Monta Log
                DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                String json = JsonConvert.SerializeObject(dto);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Anotação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult CancelarLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Cancelamento";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                LOCACAO item = baseApp.GetItemById(id);
                Session["IdLocacao"] = item.LOCA_CD_ID;

                // Checa parcelas
                if (item.LOCA_IN_STATUS != 0)
                {
                    Int32 pago = item.LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 0 & p.LOPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date).ToList().Count();
                    if (pago > 0)
                    {
                        Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " não pode ser cancelada pois tem parcelas vencidas em aberto";
                        Session["MensLocacao"] = 71;
                        RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }

                PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                List<SelectListItem> recebe = new List<SelectListItem>();
                recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
                ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 2), "COLO_CD_ID", "COLO_NM_NOME");

                // Prepara view
                Session["LocacaoAntes"] = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                vm.LOCA_DT_CANCELAMENTO = DateTime.Today.Date;
                vm.LOCA_IN_ESTOQUE = 0;
                //vm.LOCA_IN_STATUS = 4;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelarLocacao(LocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> recebe = new List<SelectListItem>();
            recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
            ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 2), "COLO_CD_ID", "COLO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOCA_DS_JUSTIFICATIVA = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.LOCA_DS_JUSTIFICATIVA);

                    // Critica
                    if (vm.LOCA_DS_JUSTIFICATIVA == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0694", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOCA_CD_DISTRATO_ID == 0 || vm.LOCA_CD_DISTRATO_ID == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0708", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOCA_IN_ESTOQUE == 0 || vm.LOCA_IN_ESTOQUE == null)
                    {
                        vm.LOCA_IN_ESTOQUE = 0;
                    }

                    // Executa a operação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    Session["ProdAntes"] = prod;
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.LOCA_IN_STATUS = 4;
                    Int32 volta = baseApp.ValidateEdit(item, (LOCACAO)Session["LocacaoAntes"], usuario);

                    // Monta Log
                    DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Cancelamento",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Trata estoque
                    if (vm.LOCA_IN_ESTOQUE == 1)
                    {
                        // Atualiza estoque
                        prod.PROD_VL_ESTOQUE_ATUAL = prod.PROD_VL_ESTOQUE_ATUAL + item.LOCA_IN_QUANTIDADE;
                        Int32 voltaProd = prodApp.ValidateEdit(prod, (PRODUTO)Session["ProdAntes"]);

                        // Atualiza movimento de estoque
                        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                        mov.ASSI_CD_ID = idAss;
                        mov.EMFI_CD_ID = usuario.EMFI_CD_ID;
                        mov.EMPR_CD_ID = usuario.EMPR_CD_ID;
                        mov.MOEP_DS_JUSTIFICATIVA = vm.LOCA_DS_JUSTIFICATIVA;
                        mov.MOEP_DT_LANCAMENTO = DateTime.Today.Date;
                        mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                        mov.MOEP_GU_GUID = Xid.NewXid().ToString();
                        mov.MOEP_IN_ATIVO = 1;
                        mov.MOEP_IN_OPERACAO = 1;
                        mov.MOEP_IN_PENDENTE = 0;
                        mov.MOEP_IN_SISTEMA = 6;
                        mov.MOEP_IN_TIPO = 9;
                        mov.MOEP_IN_TIPO_LANCAMENTO = 1;
                        mov.MOEP_QN_QUANTIDADE = vm.LOCA_IN_QUANTIDADE.Value;
                        mov.MOEP_VL_QUANTIDADE_ANTERIOR = ((PRODUTO)Session["ProdAntes"]).PROD_VL_ESTOQUE_ATUAL;
                        mov.MOEP_VL_QUANTIDADE_MOVIMENTO = 1;
                        mov.MOEP_VL_VALOR_MOVIMENTO = 0;
                        mov.PROD_CD_ID = vm.PROD_CD_ID;
                        mov.USUA_CD_ID = usuario.USUA_CD_ID;
                        mov.MOEP_IN_TIPO_MOVIMENTO = 1;
                        mov.MOEP_IN_ORIGEM = "Cancelamento de Locação";
                        Int32 voltaC = prodApp.ValidateCreateMovimento(mov, usuario);

                        // Inclui historico de estoque
                        PRODUTO_ESTOQUE_HISTORICO est = new PRODUTO_ESTOQUE_HISTORICO();
                        est.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        est.PROD_CD_ID = vm.PROD_CD_ID;
                        est.PREH_IN_ATIVO = 1;
                        est.PREH_DT_DATA = DateTime.Today.Date;
                        est.PREH_DT_COMPLETA = DateTime.Now;
                        est.PREH_QN_ESTOQUE = vm.LOCA_IN_QUANTIDADE.Value;
                        est.PREH_IN_PENDENTE = 0;
                        est.PREH_NM_TIPO = "Entrada";
                        est.PREH_DS_ORIGEM = "Cancelamento de Locação";
                        est.MOEP_CD_ID = mov.MOEP_CD_ID;
                        Int32 voltaH = prodApp.ValidateCreateEstoqueHistorico(est, idAss);
                    }

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Cancelamento da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Cancelamento de Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Envia mensagem
                    LOCACAO locMensagem = baseApp.GetItemById(item.LOCA_CD_ID);
                    if (pac.PACI_NM_EMAIL != null)
                    {
                        Int32 voltaCons = await EnviarEMailLocacao(locMensagem, 2);
                    }
                    if (pac.PACI_NR_CELULAR != null)
                    {
                        Int32 voltaCons = EnviarSMSLocacao(locMensagem, 2);
                    }

                    // Mensages do CRUD
                    Session["MsgCRUD"] = "A Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper() + " - foi cancelada com sucesso.";
                    Session["MensLocacao"] = 61;
                    if (item.LOCA_IN_ESTOQUE == 0)
                    {
                        Session["MsgCRUD1"] = "Não foi feita a entrada no estoque para " + prod.PROD_NM_NOME.ToUpper() + " referente à locação de " + pac.PACI_NM_NOME.ToUpper() + " - Deve ser feita entrada manual.";
                        Session["MensProduto"] = 62;
                    }

                    // Trata distrato
                    if (item.LOCA_IN_ASSINADO_DIGITAL == 0)
                    {
                        Int32 rel = GerarDistratoPDFTeste();
                    }
                    else
                    {
                        Int32 rel = GerarDistratoPDFTesteAssina();

                    }
                    Int32 voltaD = await ProcessaEnvioEMailDistrato(item, usuario);

                    // Retorno
                    listaMaster = new List<LOCACAO>();
                    Session["ListaLocacao"] = null;
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaHistoricoLocacao"] = null;
                    Session["LocacoesHistoricos"] = null;
                    Session["MensLocacao"] = null;

                    if ((Int32)Session["VoltaLocacao"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                    return RedirectToAction("MontarTelaLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult CancelarLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("CancelarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        public async Task<Int32> EnviarEMailProcessaLocacao(LOCACAO locacao, Int32 tipo, Int32 operacao)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
            PRODUTO prod = prodApp.GetItemById(locacao.PROD_CD_ID);
            PERIODICIDADE_TAREFA peta = perApp.GetItemById(locacao.PETA_CD_ID);
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = new TEMPLATE_EMAIL();
                if (operacao == 1)
                {
                    template = temApp.GetByCode("CANCLOCA", idAss);
                }
                if (operacao == 2)
                {
                    template = temApp.GetByCode("RENOLOCA", idAss);
                }
                if (operacao == 3)
                {
                    template = temApp.GetByCode("ENCELOCA", idAss);
                }

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;

                // Prepara assinatura
                String assinatura = String.Empty;
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
                assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
                assinatura += "Enviado por <b>WebDoctor</b><br />";

                // Prepara corpo da mensagem
                String urlDestino = conf.CONF_LK_LINK_SISTEMA;
                String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";
                String texto = template.TEEM_TX_CORPO;
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (operacao == 1)
                {
                    if (texto.Contains("{parcela}"))
                    {
                        texto = texto.Replace("{parcela}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                    }
                    if (texto.Contains("{dia}"))
                    {
                        texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                    }
                }
                if (operacao == 2)
                {
                    if (texto.Contains("{cancelamento}"))
                    {
                        texto = texto.Replace("{cancelamento}", locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{justificativa}"))
                    {
                        texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                    }
                }
                if (operacao == 3)
                {
                    if (texto.Contains("{encerramento}"))
                    {
                        texto = texto.Replace("{encerramento}", locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString());
                    }
                    if (texto.Contains("{justificativa}"))
                    {
                        texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                    }
                }
                if (texto.Contains("{link}"))
                {
                    texto = texto.Replace("{link}", linkHtml);
                }
                String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - " + (operacao == 1 ? "Cancelamento" : (operacao == 2 ? "Renovacao" : "Encerramento"));
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
                    await CrossCutting.CommunicationAzurePackage.SendMailAsyncNew(mensagem);
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
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem para Paciente: " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - " + (operacao == 1 ? "Cancelamento" : (operacao == 2 ? "Renovacao" : "Encerramento"));;
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de " + (operacao == 1 ? "Cancelamento" : (operacao == 2 ? "Renovacao" : "Encerramento")) + " de Locação - " + paciente.PACI_NM_NOME.ToUpper());

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Envio de e-mail",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = (String)Session["JSONLocacao"],
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta3 = logApp.ValidateCreate(log);

                // Sucesso
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                return 1;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 EnviarSMSProcessaLocacao(LOCACAO locacao, Int32 tipo, Int32 operacao)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
            PRODUTO prod = prodApp.GetItemById(locacao.PROD_CD_ID);

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_SMS template = new TEMPLATE_SMS();
                if (operacao == 1)
                {
                    template = smsApp.GetByCode("CANCLOCA", idAss);
                }
                if (operacao == 2)
                {
                    template = smsApp.GetByCode("RENOLOCA", idAss);
                }
                if (operacao == 3)
                {
                    template = smsApp.GetByCode("ENCELOCA", idAss);
                }

                // Prepara assinatura
                String assinatura = String.Empty;
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                assinatura = emp.EMPR_NM_NOME;
                assinatura += "CNPJ: " + emp.EMPR_NR_CNPJ;
                assinatura += "Enviado por WebDoctor";

                // Prepara corpo da mensagem
                String texto = template.TSMS_TX_CORPO;
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (operacao == 1)
                {
                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                    }
                }
                if (operacao == 2)
                {
                    if (texto.Contains("{cancelamento}"))
                    {
                        texto = texto.Replace("{cancelamento}", locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString());
                    }
                }
                if (operacao == 3)
                {
                    if (texto.Contains("{encerramento}"))
                    {
                        texto = texto.Replace("{encerramento}", locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString());
                    }
                }
                if (texto.Contains("{assinatura}"))
                {
                    texto = texto.Replace("{assinatura}", assinatura);
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
                mens.MENS_NM_NOME = "Mensagem para Paciente: " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - " + (operacao == 1 ? "Cancelamento" : (operacao == 2 ? "Renovacao" : "Encerramento")); ;
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = smsBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Confirmação de " + (operacao == 1 ? "Cancelamento" : (operacao == 2 ? "Renovacao" : "Encerramento")) + " de Locação - " + paciente.PACI_NM_NOME);

                // Sucesso
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                return 1;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }


        public async Task<ActionResult> EnviarEMailParcela(Int32 id)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            LOCACAO_PARCELA parcela = baseApp.GetParcelaById(id);
            LOCACAO locacao = baseApp.GetItemById(parcela.LOCA_CD_ID);
            PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
            PRODUTO prod = prodApp.GetItemById(locacao.PROD_CD_ID);
            PERIODICIDADE_TAREFA peta = perApp.GetItemById(locacao.PETA_CD_ID);
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            DTO_Parcela parc = MontarParcelaDTO(id);
            String json = JsonConvert.SerializeObject(parc);

            // Processo
            try
            {
                // Recupera Template
                TEMPLATE_EMAIL template = temApp.GetByCode("PARCATRA", idAss);

                // Prepara cabeçalho
                String cab = template.TEEM_TX_CABECALHO;

                // Prepara assinatura
                String assinatura = String.Empty;
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
                assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
                assinatura += "Enviado por <b>WebDoctor</b><br />";

                // Prepara corpo da mensagem
                String texto = template.TEEM_TX_CORPO;
                String urlDestino = conf.CONF_LK_LINK_SISTEMA;
                String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", prod.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{parcela}"))
                {
                    texto = texto.Replace("{parcela}", parcela.LOPA_NR_PACELAS.ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(parcela.LOPA_VL_VALOR.Value));
                }
                if (texto.Contains("{vencimento}"))
                {
                    texto = texto.Replace("{vencimento}", parcela.LOPA_DT_VENCIMENTO.Value.ToLongDateString());
                }
                if (texto.Contains("{atraso}"))
                {
                    texto = texto.Replace("{atraso}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{link}"))
                {
                    texto = texto.Replace("{link}", linkHtml);
                }
                String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Paciente - " + paciente.PACI_NM_NOME.ToUpper() + " - Locação - Parcela em Atraso";
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
                    await CrossCutting.CommunicationAzurePackage.SendMailAsyncNew(mensagem);
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

                // Grava mensagem enviada
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = paciente.PACI__CD_ID;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Mensagem para Paciente - Locação: " + locacao.LOCA_NM_TITULO.ToUpper();
                mens.PACI_CD_ID = paciente.PACI__CD_ID;
                mens.MENS_TX_TEXTO = emailBody;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                String guid = Xid.NewXid().ToString();
                Int32 volta1 = envio.GravarMensagemEnviada(mens, usuario, mens.MENS_TX_TEXTO, "Succeeded", guid, null, "Parcela em Atraso - " + paciente.PACI_NM_NOME.ToUpper());

                // Grava historico
                LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                hist.ASSI_CD_ID = idAss;
                hist.LOCA_CD_ID = locacao.LOCA_CD_ID;
                hist.LOHI_DS_DESCRICAO = "Envio de Mensagem de parcela em atraso " + locacao.LOCA_NM_TITULO.ToUpper();
                hist.LOHI_DT_HISTORICO = DateTime.Now;
                hist.USUA_CD_ID = usuario.USUA_CD_ID;
                hist.LOHI_IN_ATIVO = 1;
                hist.LOHI_NM_OPERACAO = "Parcela em atraso - " + parcela.LOPA_NR_PACELAS;
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Parcela - Envio de e-mail",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta3 = logApp.ValidateCreate(log);

                // Sucesso
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;   
                Session["NivelLocacao"] = 4;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "Mensagem enviada para " + paciente.PACI_NM_NOME.ToUpper() + " - Parcela em atraso";
                Session["MensLocacao"] = 61;

                // Retorno
                if ((Int32)Session["VoltaMailLocacao"] == 2)
                {
                    return RedirectToAction("VerParcelasLocacoes");
                }
                return RedirectToAction("VoltarAnexoLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult RenovarLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Renovação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                LOCACAO item = baseApp.GetItemById(id);
                Session["IdLocacao"] = item.LOCA_CD_ID;

                // Verifica
                if (item.LOCA_IN_STATUS == 2 || item.LOCA_IN_STATUS == 4)
                {
                    Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " não pode ser renovada pois está cancelada ou encerrada";
                    Session["MensLocacao"] = 71;
                    RedirectToAction("MontarTelaLocacao", "Locacao");

                }
                DateTime limite = item.LOCA_DT_FINAL.Value.AddMonths(-1);
                if (limite.Date > DateTime.Today.Date)
                {
                    Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " não pode ser renovada pois ainda faltam mais de um mês para o final";
                    Session["MensLocacao"] = 71;
                    RedirectToAction("MontarTelaLocacao", "Locacao");
                }

                // Verifica parcelas
                Int32 parc = item.LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 0 & p.LOPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date).Count();
                if (parc > 0)
                {
                    Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " não pode ser renovada pois existem parcelas vencidas em aberto";
                    Session["MensLocacao"] = 71;
                    RedirectToAction("MontarTelaLocacao", "Locacao");
                }

                // Prepara view
                Session["LocacaoAntes"] = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                vm.LOCA_DT_RENOVACAO = DateTime.Today.Date;
                vm.LOCA_IN_STATUS = 1;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RenovarLocacao(LocacaoViewModel vm)
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
                    // Preparação
                    vm.LOCA_DT_INICIO = vm.LOCA_DT_RENOVACAO.Value.Date;
                    vm.LOCA_DT_FINAL = vm.LOCA_DT_RENOVACAO.Value.Date.AddMonths(vm.LOCA_NR_PRAZO.Value);
                    vm.LOCA_IN_STATUS = 1;
                    vm.LOCA_IN_RENOVACAO = 1;
                    vm.LOCA_IN_RENOVACOES++;

                    // Executa a operação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    PERIODICIDADE_TAREFA peta = perApp.GetItemById(vm.PETA_CD_ID);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, (LOCACAO)Session["LocacaoAntes"], usuario);

                    // Grava Parcelas
                    DateTime? venc = CalculaVencimento(vm.LOCA_NR_DIA, 1, 30);
                    for (Int32 i = 1; i <= vm.LOCA_NR_PRAZO; i++)
                    {
                        Int32 numParc = i;

                        // Monta e grava
                        LOCACAO_PARCELA parc = new LOCACAO_PARCELA();
                        parc.ASSI_CD_ID = idAss;
                        parc.LOCA_CD_ID = item.LOCA_CD_ID;
                        parc.LOPA_DS_DESCRICAO = "Parcela " + numParc.ToString() + " da locação " + item.LOCA_NM_TITULO;
                        parc.LOPA_DT_VENCIMENTO = venc;
                        parc.LOPA_IN_ATIVO = 1;
                        parc.LOPA_IN_PARCELA = 1;
                        parc.LOPA_IN_QUITADA = 0;
                        parc.LOPA_IN_STATUS = 1;
                        parc.LOPA_NM_PARCELAS = "Parcela " + numParc.ToString() + " da locação " + item.LOCA_NM_TITULO;
                        parc.LOPA_NR_PACELAS = numParc;
                        parc.LOPA_VL_DESCONTO = 0;
                        parc.LOPA_VL_JUROS = 0;
                        parc.LOPA_VL_TAXAS = 0;
                        parc.LOPA_VL_VALOR = item.LOCA_VL_PARCELA;
                        parc.LOPA_VL_VALOR_PAGO = 0;
                        Int32 voltaParc = baseApp.ValidateCreateParcela(parc);

                        // Ajustes
                        venc = venc.Value.AddMonths(1);
                    }

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Renovação da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Renovação de Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Emite mensagem
                    LOCACAO locMensagem = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                    if (pac.PACI_NM_EMAIL != null)
                    {
                        Int32 voltaCons = await EnviarEMailProcessaLocacao(locMensagem, 1, 2);
                    }
                    if (pac.PACI_NR_CELULAR != null)
                    {
                        Int32 voltaCons = EnviarSMSProcessaLocacao(locMensagem, 1, 2);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " foi renovada com sucesso. Foram geradas " + item.LOCA_NR_PRAZO.ToString() + " parcelas.";
                    Session["MensLocacao"] = 61;

                    // Retorno
                    listaMaster = new List<LOCACAO>();
                    Session["ListaLocacao"] = null;
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaHistoricoLocacao"] = null;
                    Session["LocacoesHistoricos"] = null;

                    if ((Int32)Session["VoltaLocacao"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                    return RedirectToAction("MontarTelaLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult RenovarLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("RenovarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        [HttpGet]
        public ActionResult EditarParcela(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação -Parcela - Edição";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locação - Quitação";

                // Trata mensagens
                if (Session["MensLocacao"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensLocacao"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 69)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0539", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 70)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0540", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                ViewBag.Forma = new SelectList(CarregaFormas(), "FORE_CD_ID", "FORE_NM_FORMA");
                List<SelectListItem> quitada = new List<SelectListItem>();
                quitada.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                quitada.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Quitada = new SelectList(quitada, "Value", "Text");
                List<SelectListItem> gera = new List<SelectListItem>();
                gera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                gera.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Gera = new SelectList(gera, "Value", "Text");
                Session["NivelPaciente"] = 14;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";
                Session["VoltaLocacao"] = 2;
                Session["NivelLocacao"] = 4;

                LOCACAO_PARCELA item = baseApp.GetParcelaById(id);
                LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                Session["IdPaciente"] = loca.PACI_CD_ID;
                Session["IdLocacao"] = item.LOCA_CD_ID;
                Session["ParcelaAntes"] = item;
                LocacaoParcelaViewModel vm = Mapper.Map<LOCACAO_PARCELA, LocacaoParcelaViewModel>(item);

                PACIENTE pac = pacApp.GetItemById(loca.PACI_CD_ID);
                vm.LOCACAO.PACIENTE = pac;
                vm.LOPA_DT_PAGAMENTO = DateTime.Today.Date;
                vm.LOPA_VL_VALOR_PAGO = vm.LOPA_VL_VALOR + vm.LOPA_VL_JUROS;
                vm.LOPA_IN_LANCAMENTO = 0;
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_PARCELA_EDITAR", "Locacao", "EditarParcela");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarParcela(LocacaoParcelaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ViewBag.Forma = new SelectList(CarregaFormas(), "FORE_CD_ID", "FORE_NM_FORMA");
            List<SelectListItem> quitada = new List<SelectListItem>();
            quitada.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            quitada.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Quitada = new SelectList(quitada, "Value", "Text");
            List<SelectListItem> gera = new List<SelectListItem>();
            gera.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            gera.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Gera = new SelectList(gera, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Saida rapida
                    if (vm.LOPA_IN_QUITADA == 0)
                    {
                        if ((Int32)Session["VoltaLocacaoParcela"] == 2)
                        {
                            return RedirectToAction("VerParcelasLocacoes");
                        }
                        return RedirectToAction("VoltarAnexoParcela");
                    }

                    // Critica
                    if (vm.LOPA_VL_VALOR_PAGO == 0 || vm.LOPA_VL_VALOR_PAGO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0699", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOPA_VL_VALOR_PAGO < (vm.LOPA_VL_VALOR + vm.LOPA_VL_JUROS))
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0698", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOPA_DT_PAGAMENTO ==  null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0697", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOPA_DT_PAGAMENTO > DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0700", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOPA_IN_LANCAMENTO == 0 || vm.LOPA_IN_LANCAMENTO == null)
                    {
                        vm.LOPA_IN_LANCAMENTO = 0;
                    }

                    // Executa a operação
                    LOCACAO_PARCELA item = Mapper.Map<LocacaoParcelaViewModel, LOCACAO_PARCELA>(vm);
                    Int32 volta = baseApp.ValidateEditParcela(item);
                    LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                    PACIENTE pac = pacApp.GetItemById(loca.PACI_CD_ID);

                    // Monta Log
                    DTO_Parcela dto = MontarParcelaDTO(item.LOPA_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Parcela - Quitação",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 4;
                    Session["ListaLocacao"] = null;

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Quitação de Parcela da Locação - " + item.LOPA_DS_DESCRICAO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Quitação da Parcela de Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Mensagem do CRUD
                    String crud = "A Parcela " + item.LOPA_NM_PARCELAS + " da locação " + loca.LOCA_NM_TITULO + " foi quitada com sucesso";

                    // Lancamento automatico
                    if (vm.LOPA_IN_LANCAMENTO == 1)
                    {
                        if ((Int32)Session["PermFinanceiro"] == 1)
                        {
                            List<CONSULTA_RECEBIMENTO> pagMes = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1).ToList();
                            Int32 num = pagMes.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                            if ((Int32)Session["NumRecebimentos"] >= num)
                            {
                                String nome = "Recebimento de parcela de locação de " + pac.PACI_NM_NOME + " - Locação: " + loca.LOCA_NM_TITULO;
                                CONSULTA_RECEBIMENTO rec = new CONSULTA_RECEBIMENTO();
                                rec.CORE_IN_ATIVO = 1;
                                rec.CORE_DT_RECEBIMENTO = DateTime.Today.Date;
                                rec.CORE_GU_GUID = Xid.NewXid().ToString();
                                rec.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                                rec.ASSI_CD_ID = idAss;
                                rec.CORE_IN_CONFERIDO = 0;
                                rec.CORE_NM_RECEBIMENTO = nome;
                                rec.PACI_CD_ID = pac.PACI__CD_ID;
                                rec.PACO_CD_ID = null;
                                rec.CORE_VL_VALOR = item.LOPA_VL_VALOR_PAGO;
                                rec.FORE_CD_ID = null;
                                recApp.ValidateCreate(rec, usuarioLogado);

                                // Configura serialização
                                JsonSerializerSettings settings = new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    NullValueHandling = NullValueHandling.Ignore
                                };

                                // Monta Log
                                DTO_Recebimento dto1 = MontarRecebimentoDTOObj(rec);
                                String json1 = JsonConvert.SerializeObject(dto1, settings);
                                CONSULTA_RECEBIMENTO pag = recApp.GetItemById(rec.CORE_CD_ID);
                                LOG log1 = new LOG
                                {
                                    LOG_DT_DATA = DateTime.Now,
                                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                                    LOG_NM_OPERACAO = "Recebimento - Inclusão",
                                    LOG_IN_ATIVO = 1,
                                    LOG_TX_REGISTRO = json1,
                                    LOG_IN_SISTEMA = 6
                                };
                                Int32 voltaU = logApp.ValidateCreate(log1);

                                crud += ". Um lançamento de recebimento foi gerado para esta consulta.";

                                // Cria pastas
                                String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + rec.CORE_CD_ID.ToString() + "/Anexos/";
                                String map = Server.MapPath(caminho);
                                Directory.CreateDirectory(Server.MapPath(caminho));
                            }
                            else
                            {
                                crud += ". O lançamento de recebimento não pode ser gerado pois o número de lançamentos do mês excedeu o limite contratado";
                            }
                        }
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = crud;
                    Session["MensLocacao"] = 61;

                    // Retorno
                    Session["LocacaoAlterada"] = 1;
                    Session["ListaLocacaoData"] = null;
                    Session["ListaParcelaData"] = null;
                    Session["ListaLocacaoMes"] = null;
                    Session["ListaParcelaMes"] = null;
                    Session["ListaLocacaoStatus"] = null;
                    Session["ListaLocacaoProduto"] = null;
                    Session["ListaLocacaoPaciente"] = null;
                    Session["ListaLocacaoVencida"] = null;
                    Session["ListaRecebeProduto"] = null;
                    Session["ListaParcelaAtraso"] = null;
                    Session["ListaLocacaoEncerra"] = null;
                    if ((Int32)Session["VoltaLocacaoParcela"] == 2)
                    {
                        return RedirectToAction("VerParcelasLocacoes");
                    }
                    return RedirectToAction("VoltarAnexoParcela");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public DTO_Recebimento MontarRecebimentoDTOObj(CONSULTA_RECEBIMENTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Recebimento()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    CORE_CD_ID = l.CORE_CD_ID,
                    FORE_CD_ID = l.FORE_CD_ID,
                    PACI_CD_ID = l.PACI_CD_ID,
                    PACO_CD_ID = l.PACO_CD_ID,
                    SERV_CD_ID = l.SERV_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    VACO_CD_ID = l.VACO_CD_ID,
                    VACV_CD_ID = l.VACV_CD_ID,
                    VASE_CD_ID = l.VASE_CD_ID,
                    CORE_DT_RECEBIMENTO = l.CORE_DT_RECEBIMENTO,
                    CORE_GU_GUID = l.CORE_GU_GUID,
                    CORE_IN_ATIVO = l.CORE_IN_ATIVO,
                    CORE_IN_CONFERIDO = l.CORE_IN_CONFERIDO,
                    CORE_NM_RECEBIMENTO = l.CORE_NM_RECEBIMENTO,
                    CORE_VL_CONVENIO = l.CORE_VL_CONVENIO,
                    CORE_VL_SERVICO = l.CORE_VL_SERVICO,
                    CORE_VL_VALOR = l.CORE_VL_VALOR,
                };
                return mediDTO;
            }
        }

        public ActionResult VoltarAnexoParcela()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        public ActionResult VerParcelasLocacao(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelLocacao"] = 4;
            return RedirectToAction("EditarLocacao", new { id = id });
        }

        [HttpGet]
        public ActionResult MontarTelaLocacaoAtivas()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacaoAtiva"] == null)
                {
                    locs = locs.Where(p => p.LOCA_IN_STATUS == 1).ToList();
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacaoAtiva"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacaoAtiva"];
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ATIVAS", "Locacao", "MontarTelaLocacaoAtivas");

                // Abre view
                Session["TipoLocacaoRel"] = 1;
                Session["TipoLocacao"] = 2;
                Session["MensLocacao"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacaoAtivas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacaoAtiva"] = null;
                return RedirectToAction("MontarTelaLocacaoAtivas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacaoAtivas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacaoAtiva"] = listaMaster;
                return RedirectToAction("MontarTelaLocacaoAtivas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacaoAtivas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacaoAtivas");
        }

        [HttpPost]
        public ActionResult FiltrarLocacaoAtivas(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacaoAtivas");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2.Where(p => p.LOCA_IN_STATUS == 1).ToList();
                Session["ListaLocacaoAtiva"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacaoAtivas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult MontarTelaLocacaoEncerradas()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacaoEncerrada"] == null)
                {
                    locs = locs.Where(p => p.LOCA_IN_STATUS == 2).ToList();
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacaoEncerrada"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacaoEncerrada"];
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ENCERRADAS", "Locacao", "MontarTelaLocacaoEncerradas");

                // Abre view
                Session["TipoLocacao"] = 2;
                Session["TipoLocacaoRel"] = 2;
                Session["MensLocacao"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacaoEncerradas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacaoEncerrada"] = null;
                return RedirectToAction("MontarTelaLocacaoEncerradas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacaoEncerradas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacaoEncerrada"] = listaMaster;
                return RedirectToAction("MontarTelaLocacaoEncerradas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacaoEncerradas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacaoEncerradas");
        }

        [HttpPost]
        public ActionResult FiltrarLocacaoEncerradas(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacaoEncerradas");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2.Where(p => p.LOCA_IN_STATUS == 2).ToList();
                Session["ListaLocacaoEncerrada"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacaoEncerradas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult MontarTelaLocacaoAtrasadas()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacaoAtrasada"] == null)
                {
                    locs = locs.Where(p => p.LOCA_IN_STATUS == 3).ToList();
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacaoAtrasada"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacaoAtrasada"];
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_ATRASADAS", "Locacao", "MontarTelaLocacaoAtrasadas");

                // Abre view
                Session["TipoLocacao"] = 2;
                Session["TipoLocacaoRel"] = 3;
                Session["MensLocacao"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacaoAtrasadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacaoAtrasada"] = null;
                return RedirectToAction("MontarTelaLocacaoEncerradas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacaoAtrasadas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacaoAtrasada"] = listaMaster;
                return RedirectToAction("MontarTelaLocacaoAtrasadas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacaoAtrasadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacaoAtrasadas");
        }

        [HttpPost]
        public ActionResult FiltrarLocacaoAtrasadas(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacaoAtrasadas");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2.Where(p => p.LOCA_IN_STATUS == 3).ToList();
                Session["ListaLocacaoAtrasada"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacaoAtrasadas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult MontarTelaLocacaoCanceladas()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacaoCancelada"] == null)
                {
                    locs = locs.Where(p => p.LOCA_IN_STATUS == 4).ToList();
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacaoCancelada"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacaoCancelada"];
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_CANCELADAS", "Locacao", "MontarTelaLocacaoCanceladas");

                // Abre view
                Session["TipoLocacao"] = 2;
                Session["TipoLocacaoRel"] = 4;
                Session["MensLocacao"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacaoCanceladas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacaoCancelada"] = null;
                return RedirectToAction("MontarTelaLocacaoCanceladas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacaoCanceladas()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacaoCancelada"] = listaMaster;
                return RedirectToAction("MontarTelaLocacaoCanceladas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacaoCanceladas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacaoCanceladas");
        }

        [HttpPost]
        public ActionResult FiltrarLocacaoCanceladas(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacaoCanceladas");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2.Where(p => p.LOCA_IN_STATUS == 4).ToList();
                Session["ListaLocacaoCancelada"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacaoCanceladas");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        public ActionResult GerarRelatorioListaLocacao(Int32 id)
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;

                List<LOCACAO> lista = new List<LOCACAO>();
                nomeRel = "LocacaoLista" + "_" + data + ".pdf";
                titulo = "Locações";
                if (id == 0)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacao"];
                    titulo = "Locações";
                }
                if (id == 1)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacaoAtiva"];
                    titulo = "Locações - Ativa";
                }
                if (id == 2)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacaoEncerrada"];
                    titulo = "Locações - Encerrada";
                }
                if (id == 3)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacaoAtrasada"];
                    titulo = "Locações - Atrasada";
                }
                if (id == 4)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacaoCancelada"];
                    titulo = "Locações - Cancelada";
                }
                if (id == 5)
                {
                    lista = (List<LOCACAO>)Session["ListaLocacao"];
                    titulo = "Locações - Pendentes";
                }
                lista = lista.OrderBy(p => p.PACIENTE.PACI_NM_NOME).ToList();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph(titulo, meuFont2))
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

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 150f, 180f, 80f, 70f, 70f, 70f, 70f, 60f, 50f });
                if (id == 1)
                {
                    table = new PdfPTable(new float[] { 150f, 180f, 80f, 70f, 70f, 70f, 70f, 50f });
                }
                if (id == 2)
                {
                    table = new PdfPTable(new float[] { 150f, 180f, 80f, 70f, 70f, 70f, 70f, 160f, 50f });
                }
                if (id == 3)
                {
                    table = new PdfPTable(new float[] { 150f, 180f, 80f, 70f, 70f, 70f, 70f, 50f });
                }
                if (id == 4)
                {
                    table = new PdfPTable(new float[] { 150f, 180f, 80f, 70f, 70f, 70f, 70f, 160f, 50f });
                }
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Final", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Parcela (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                if (id == 2)
                {
                    cell = new PdfPCell(new Paragraph("Encerramento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }
                else if (id == 3)
                {
                    cell = new PdfPCell(new Paragraph("Atraso (Dias)", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }
                else if (id == 4)
                {
                    cell = new PdfPCell(new Paragraph("Cancelamento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Dia de Vencimento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                if (id == 2 || id == 4)
                {
                    cell = new PdfPCell(new Paragraph("Justificativa", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }
                else if (id == 0)
                {
                    cell = new PdfPCell(new Paragraph("Status", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (LOCACAO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PACIENTE.PACI_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_DT_INICIO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_DT_FINAL.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_IN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOCA_VL_PARCELA.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (id == 2)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOCA_DT_ENCERRAMENTO.Value.ToShortDateString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (id == 3)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOCA_NR_ATRASO.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (id == 4)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOCA_DT_CANCELAMENTO.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph(item.LOCA_NR_DIA.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    if (id == 0)
                    {
                        if (item.LOCA_IN_STATUS == 1)
                        {
                            cell = new PdfPCell(new Paragraph("Ativa", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }
                        else if (item.LOCA_IN_STATUS == 0)
                        {
                            cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }

                        else if (item.LOCA_IN_STATUS == 2)
                        {
                            cell = new PdfPCell(new Paragraph("Encerrada", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }
                        else if (item.LOCA_IN_STATUS == 3)
                        {
                            cell = new PdfPCell(new Paragraph("Atrasada", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }
                        else if (item.LOCA_IN_STATUS == 4)
                        {
                            cell = new PdfPCell(new Paragraph("Cancelada", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }
                    }
                    else if (id == 2 || id == 4)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOCA_DS_JUSTIFICATIVA.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    if (System.IO.File.Exists(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        Image image = Image.GetInstance(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO));
                        image.ScaleAbsolute(20, 20);
                        cell.AddElement(image);
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(cell);
                    }
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

                // Retorno
                if (id == 1)
                {
                    return RedirectToAction("MontarTelaLocacaoAtivas");
                }
                else if (id == 2)
                {
                    return RedirectToAction("MontarTelaLocacaoEncerradas");
                }
                else if (id == 3)
                {
                    return RedirectToAction("MontarTelaLocacaoAtrasadas");
                }
                else if (id == 4)
                {
                    return RedirectToAction("MontarTelaLocacaoCanceladas");
                }
                else
                {
                    return RedirectToAction("MontarTelaLocacao");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "CRMsys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioListaLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("GerarRelatorioListaLocacao", new { id = (Int32)Session["TipoLocacaoRel"] });
        }

        [HttpGet]
        public ActionResult MontarTelaLocacaoPendentes()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao";

                // Carrega listas
                List<LOCACAO> locs = CarregarLocacao();
                if ((List<LOCACAO>)Session["ListaLocacaoPendente"] == null)
                {
                    locs = locs.Where(p => p.LOCA_IN_STATUS == 0).ToList();
                    listaMaster = locs.OrderBy(p => p.PACI_CD_ID).ThenBy(p => p.LOCA_NM_TITULO).ToList();
                    Session["ListaLocacaoPendente"] = listaMaster;
                }
                ViewBag.Listas = (List<LOCACAO>)Session["ListaLocacaoPendente"];
                Session["Locacao"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0685", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 71)
                    {
                        ModelState.AddModelError("",(String)Session["MsgCRUD"]);
                    }
                }
                if ((Int32)Session["MensPermissao"] == 2)
                {
                    String mens = CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture) + ". Módulo: " + (String)Session["ModuloPermissao"];
                    ModelState.AddModelError("", mens);
                    Session["MensPermissao"] = 0;
                }
               
                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_PENDENTES", "Locacao", "MontarTelaLocacaoPendentes");

                // Abre view
                Session["TipoLocacaoRel"] = 1;
                Session["TipoLocacao"] = 2;
                Session["MensLocacao"] = null;
                Session["ListaLog"] = null;
                Session["VoltaLocacao"] = 1;
                objeto = new LOCACAO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public ActionResult RetirarFiltroLocacaoPendentes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaLocacaoPendente"] = null;
                return RedirectToAction("MontarTelaLocacaoPendentes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoLocacaoPendentes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaLocacaoPendente"] = listaMaster;
                return RedirectToAction("MontarTelaLocacaoPendentes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLocacaoPendentes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLocacaoPendentes");
        }

        [HttpPost]
        public ActionResult FiltrarLocacaoPendentes(LOCACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<LOCACAO> listaObj = new List<LOCACAO>();
                Tuple<Int32, List<LOCACAO>, Boolean> volta = baseApp.ExecuteFilter(item.LOCA_NM_PACIENTE_DUMMY, item.LOCA_NM_PRODUTO_DUMMY, item.LOCA_DT_INICIO, item.LOCA_DT_DUMMY, item.LOCA_IN_STATUS, item.LOCA_NR_NUMERO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("MontarTelaLocacaoPendentes");
                }

                // Sucesso
                Session["MensLocacao"] = null;
                listaMaster = volta.Item2.Where(p => p.LOCA_IN_STATUS == 0).ToList();
                Session["ListaLocacaoPendente"] = volta.Item2;
                return RedirectToAction("MontarTelaLocacaoPendentes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        public Int32 GerarContratoPDFTeste()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Contrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Contrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.COLO_CD_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{equipamento}"))
                {
                    texto = texto.Replace("{equipamento}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME);
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{garantia}"))
                {
                    texto = texto.Replace("{garantia}", locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não");
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{total}"))
                {
                    texto = texto.Replace("{total}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value));
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("C O N T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 135f, 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);
                    String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                    cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Garantia: " + (locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não"), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Parcelas: " + locacao.LOCACAO_PARCELA.Where(p => p.LOPA_IN_ATIVO == 1).Count().ToString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor da Parcela (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Dia de Vencimento: " + locacao.LOCA_NR_DIA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor Total (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Parcelas
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("PARCELAS", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    if (parcs.Count() > 0)
                    {
                        // Grid
                        table = new PdfPTable(new float[] { 80f, 140f, 80f, 80f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Número", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (LOCACAO_PARCELA item in parcs)
                        {
                            cell = new PdfPCell(new Paragraph(item.LOPA_NM_PARCELAS, meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(item.LOPA_DS_DESCRICAO, meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            if (item.LOPA_DT_VENCIMENTO != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.LOPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                            if (item.LOPA_VL_VALOR != null)
                            {
                                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOPA_VL_VALOR.Value), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(cell);
                            }
                        }
                        pdfDoc.Add(table);
                    }

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarContratoPDFTesteAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Contrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Contrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.COLO_CD_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{equipamento}"))
                {
                    texto = texto.Replace("{equipamento}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{garantia}"))
                {
                    texto = texto.Replace("{garantia}", locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não");
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{total}"))
                {
                    texto = texto.Replace("{total}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value));
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("C O N T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                    cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Garantia: " + (locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não"), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Parcelas: " + locacao.LOCACAO_PARCELA.Where(p => p.LOPA_IN_ATIVO == 1).Count().ToString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor da Parcela (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Dia de Vencimento: " + locacao.LOCA_NR_DIA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor Total (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Parcelas
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("PARCELAS", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    if (parcs.Count() > 0)
                    {
                        // Grid
                        table = new PdfPTable(new float[] { 80f, 140f, 80f, 80f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Número", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        cell.Colspan = 1;
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (LOCACAO_PARCELA item in parcs)
                        {
                            cell = new PdfPCell(new Paragraph(item.LOPA_NM_PARCELAS, meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(item.LOPA_DS_DESCRICAO, meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            if (item.LOPA_DT_VENCIMENTO != null)
                            {
                                cell = new PdfPCell(new Paragraph(item.LOPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                            if (item.LOPA_VL_VALOR != null)
                            {
                                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOPA_VL_VALOR.Value), meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Paragraph("-", meuFont))
                                {
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_RIGHT
                                };
                                table.AddCell(cell);
                            }
                        }
                        pdfDoc.Add(table);
                    }

                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailContrato(LOCACAO vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE paciente = pacApp.GetItemById(vm.PACI_CD_ID);
            PRODUTO produto = prodApp.GetItemById(vm.PROD_CD_ID);

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Recupera Template
            TEMPLATE_EMAIL template = temApp.GetByCode("ENCOLOCA", idAss);

            // Prepara cabeçalho
            String cab = template.TEEM_TX_CABECALHO;

            // Prepara assinatura
            String assinatura = String.Empty;
            EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
            assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
            assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
            assinatura += "Enviado por <b>WebDoctor</b><br />";

            // Prepara corpo da mensagem
            String texto = template.TEEM_TX_CORPO;
            String urlDestino = conf.CONF_LK_LINK_SISTEMA;
            String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";
            if (texto.Contains("{produto}"))
            {
                texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
            }
            if (texto.Contains("{paciente}"))
            {
                texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
            }
            if (texto.Contains("{data}"))
            {
                texto = texto.Replace("{data}", vm.LOCA_DT_INICIO.Value.ToLongDateString());
            }
            if (texto.Contains("{final}"))
            {
                texto = texto.Replace("{final}", vm.LOCA_DT_FINAL.Value.ToLongDateString());
            }
            if (texto.Contains("{valor}"))
            {
                texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(vm.LOCA_VL_PARCELA.Value));
            }
            if (texto.Contains("{numParc}"))
            {
                texto = texto.Replace("{numParc}", vm.LOCACAO_PARCELA.Count().ToString());
            }
            if (texto.Contains("{link}"))
            {
                texto = texto.Replace("{link}", linkHtml);
            }
            String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

            // Incluir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + vm.LOCA_CD_ID.ToString() + "/Contrato/";
            String fileNamePDF = "Contrato_Locacao" + paciente.PACI_NM_NOME.ToUpper() + "_" + vm.LOCA_GU_GUID + ".pdf";
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
            mensagem.ASSUNTO = "Locação - " + vm.LOCA_NM_TITULO.ToUpper();
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
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = null;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Contrato de Locação para Paciente: " + paciente.PACI_NM_NOME;
                mens.MENS_GU_GUID = vm.LOCA_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = vm.LOCA_DT_EMISSAO;
                mens.MENS_DT_ENVIO = DateTime.Today.Date;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = 0;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                mens.MENS_NM_RODAPE = String.Empty;
                mens.CELULAR = paciente.PACI_NR_CELULAR;
                mens.TELEFONE = paciente.PACI_NR_TELEFONE;
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                mens.MENS_TX_TEXTO = texto;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Locacao - Envio Contrato");
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

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailDistrato(LOCACAO vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE paciente = pacApp.GetItemById(vm.PACI_CD_ID);
            PRODUTO produto = prodApp.GetItemById(vm.PROD_CD_ID);

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Recupera Template
            TEMPLATE_EMAIL template = temApp.GetByCode("DISTLOCA", idAss);

            // Prepara cabeçalho
            String cab = template.TEEM_TX_CABECALHO;

            // Prepara assinatura
            String assinatura = String.Empty;
            EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
            assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
            assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
            assinatura += "Enviado por <b>WebDoctor</b><br />";

            // Prepara corpo da mensagem
            String texto = template.TEEM_TX_CORPO;
            String urlDestino = conf.CONF_LK_LINK_SISTEMA;
            String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";
            if (texto.Contains("{produto}"))
            {
                texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
            }
            if (texto.Contains("{paciente}"))
            {
                texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
            }
            if (texto.Contains("{data}"))
            {
                texto = texto.Replace("{data}", vm.LOCA_DT_INICIO.Value.ToLongDateString());
            }
            if (texto.Contains("{canc}"))
            {
                texto = texto.Replace("{canc}", vm.LOCA_DT_CANCELAMENTO.Value.ToLongDateString());
            }
            if (texto.Contains("{justificativa}"))
            {
                texto = texto.Replace("{justificativa}", vm.LOCA_DS_JUSTIFICATIVA);
            }
            if (texto.Contains("{link}"))
            {
                texto = texto.Replace("{link}", linkHtml);
            }
            String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

            // Incluir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + vm.LOCA_CD_ID.ToString() + "/Distrato/";
            String fileNamePDF = "Distrato_Locacao" + paciente.PACI_NM_NOME.ToUpper() + "_" + vm.LOCA_GU_GUID + ".pdf";
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
            mensagem.ASSUNTO = "Locação - " + vm.LOCA_NM_TITULO.ToUpper();
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
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = null;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Distrato de Locação para Paciente: " + paciente.PACI_NM_NOME.ToUpper();
                mens.MENS_GU_GUID = vm.LOCA_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = vm.LOCA_DT_EMISSAO;
                mens.MENS_DT_ENVIO = DateTime.Today.Date;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = 0;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                mens.MENS_NM_RODAPE = String.Empty;
                mens.CELULAR = paciente.PACI_NR_CELULAR;
                mens.TELEFONE = paciente.PACI_NR_TELEFONE;
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                mens.MENS_TX_TEXTO = texto;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Locacao - Envio Distrato");
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

        public ActionResult GerarContratoPDF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Contrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Contrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.COLO_CD_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{equipamento}"))
                {
                    texto = texto.Replace("{equipamento}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{garantia}"))
                {
                    texto = texto.Replace("{garantia}", locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não");
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{total}"))
                {
                    texto = texto.Replace("{total}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value));
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("C O N T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 135f, 600f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);
                String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Garantia: " + (locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não"), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Parcelas: " + locacao.LOCACAO_PARCELA.Where(p => p.LOPA_IN_ATIVO == 1).Count().ToString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor da Parcela (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Dia de Vencimento: " + locacao.LOCA_NR_DIA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Total (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Parcelas
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("PARCELAS", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                if (parcs.Count() > 0)
                {
                    // Grid
                    table = new PdfPTable(new float[] { 80f, 140f, 80f, 80f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Número", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    foreach (LOCACAO_PARCELA item in parcs)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOPA_NM_PARCELAS, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.LOPA_DS_DESCRICAO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        if (item.LOPA_DT_VENCIMENTO != null)
                        {
                            cell = new PdfPCell(new Paragraph(item.LOPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                        if (item.LOPA_VL_VALOR != null)
                        {
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOPA_VL_VALOR.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph("-", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                    }
                    pdfDoc.Add(table);
                }

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                if ((Int32)Session["VoltaPrintLocacao"] == 2)
                {
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
                }
                return RedirectToAction("VoltarAnexoLocacao");
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

        public ActionResult GerarContratoPDFAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Contrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Contrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.COLO_CD_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{equipamento}"))
                {
                    texto = texto.Replace("{equipamento}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{garantia}"))
                {
                    texto = texto.Replace("{garantia}", locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não");
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{total}"))
                {
                    texto = texto.Replace("{total}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value));
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("C O N T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Garantia: " + (locacao.LOCA_IN_GARANTIA == 1 ? "Sim" : "Não"), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Parcelas: " + locacao.LOCACAO_PARCELA.Where(p => p.LOPA_IN_ATIVO == 1).Count().ToString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor da Parcela (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Dia de Vencimento: " + locacao.LOCA_NR_DIA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Total (R$): " + CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_TOTAL.Value), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Parcelas
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("PARCELAS", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                if (parcs.Count() > 0)
                {
                    // Grid
                    table = new PdfPTable(new float[] { 80f, 140f, 80f, 80f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Número", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Descrição", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.Colspan = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    foreach (LOCACAO_PARCELA item in parcs)
                    {
                        cell = new PdfPCell(new Paragraph(item.LOPA_NM_PARCELAS, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(item.LOPA_DS_DESCRICAO, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        if (item.LOPA_DT_VENCIMENTO != null)
                        {
                            cell = new PdfPCell(new Paragraph(item.LOPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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
                        if (item.LOPA_VL_VALOR != null)
                        {
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOPA_VL_VALOR.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph("-", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                    }
                    pdfDoc.Add(table);
                }

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
                return RedirectToAction("VoltarAnexoLocacao");
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

        public ActionResult ImprimirContratoLocacao(Int32 id)
        {
            Session["IdLocacao"] = id;
            LOCACAO loca = baseApp.GetItemById(id);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarContratoPDFAssina");
            }
            return RedirectToAction("GerarContratoPDF");
        }

        public ActionResult ImprimirDistratoLocacao(Int32 id)
        {
            Session["IdLocacao"] = id;
            LOCACAO loca = baseApp.GetItemById(id);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarDistratoPDFAssina");
            }
            return RedirectToAction("GerarDistratoPDF");
        }

        public ActionResult ImprimirContratoLocacaoDireto()
        {
            LOCACAO loca = baseApp.GetItemById((Int32)Session["IdLocacao"]);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarContratoPDFAssina");
            }
            return RedirectToAction("GerarContratoPDF");
        }

        [HttpGet]
        public ActionResult VerHistoricoLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locacao";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                LOCACAO loca = baseApp.GetItemById(id);
                listaMasterHistorico = loca.LOCACAO_HISTORICO.ToList();
                Session["ListaHistoricoLocacao"] = listaMasterHistorico;
                ViewBag.Listas = (List<LOCACAO_HISTORICO>)Session["ListaHistoricoLocacao"];
                ViewBag.NomePaciente = loca.PACIENTE.PACI_NM_NOME;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensLocacao"] = null;
                Session["VoltaLocacao"] = 1;
                Session["NivelLocacao"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_6.pdf";

                // Carrega view

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_HISTORICO", "Locacao", "VerLocacaoHistorico");
                return View(loca);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerHistoricoLocacoes()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locacao";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao - Histórico - Geral";

                // Configuração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Carrega listas
                if (Session["ListaHistoricoLocacaoGeral"] == null)
                {
                    if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                    {
                        listaMasterHistorico = CarregaHistoricoLocacao().ToList();
                    }
                    else
                    {
                        listaMasterHistorico = CarregaHistoricoLocacao().Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaHistoricoLocacaoGeral"] = listaMasterHistorico;
                }
                ViewBag.Listas = (List<LOCACAO_HISTORICO>)Session["ListaHistoricoLocacaoGeral"];

                // Monta demais listas
                List<PACIENTE> pacs = CarregaPaciente();
                ViewBag.Paciente = new SelectList(pacs, "PACI__CD_ID", "PACI_NM_NOME");
                List<LOCACAO> locas = CarregarLocacao();
                ViewBag.Locacao = new SelectList(locas, "LOCA_CD_ID", "LOCA_NM_TITULO");

                // Mensagem
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Acerta estado
                Session["MensLocacao"] = null;
                Session["VoltaLocacao"] = 1;
                Session["NivelLocacao"] = 1;
                Session["VoltaMsg"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_6.pdf";

                // Carrega view
                objetoHistorico = new LOCACAO_HISTORICO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_HISTORICO_GERAL", "Locacao", "VerHistoricoLocacoes");
                return View(objetoHistorico);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarHistoricoLocacao(LOCACAO_HISTORICO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.LOHI_NM_OPERACAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.LOHI_NM_OPERACAO);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<LOCACAO_HISTORICO> listaObj = new List<LOCACAO_HISTORICO>();
                Tuple<Int32, List<LOCACAO_HISTORICO>, Boolean> volta = baseApp.ExecuteFilterTupleHistorico(item.LOCA_CD_ID, item.LOHI_IN_PACIENTE_DUMMY, item.LOHI_DT_HISTORICO, item.LOHI_DT_DUMMY, item.LOHI_NM_OPERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("VerHistoricoLocacoes");
                }

                // Sucesso
                listaMasterHistorico = volta.Item2.ToList();
                Session["ListaHistoricoLocacaoGeral"] = listaMasterHistorico;
                return RedirectToAction("VerHistoricoLocacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroHistoricoLocacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaHistoricoLocacaoGeral"] = null;
                return RedirectToAction("VerHistoricoLocacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerHistoricoLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerHistoricoLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        public ActionResult VerParcelasLocacoes()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locacao";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locacao - Histórico - Geral";

                // Configuração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Carrega listas
                if (Session["ListaParcelaLocacaoGeral"] == null)
                {
                    listaMasterParcela = CarregarParcelas().ToList();
                    Session["ListaParcelaLocacaoGeral"] = listaMasterParcela;
                }
                ViewBag.Listas = (List<LOCACAO_PARCELA>)Session["ListaParcelaLocacaoGeral"];

                // Monta demais listas
                List<PACIENTE> pacs = CarregaPaciente();
                ViewBag.Paciente = new SelectList(pacs, "PACI__CD_ID", "PACI_NM_NOME");
                List<LOCACAO> locas = CarregarLocacao();
                ViewBag.Locacao = new SelectList(locas, "LOCA_CD_ID", "LOCA_NM_TITULO");

                // Mensagem
                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Acerta estado
                Session["MensLocacao"] = null;
                Session["VoltaLocacao"] = 1;
                Session["NivelLocacao"] = 1;
                Session["VoltaMsg"] = 0;
                Session["VoltaMailLocacao"] = 2;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/3/Ajuda3_6.pdf";

                // Carrega view
                objetoParcela = new LOCACAO_PARCELA();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_PARCELA_GERAL", "Locacao", "VerParcelaLocacoes");
                return View(objetoParcela);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarParcelaLocacao(LOCACAO_PARCELA item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.LOPA_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.LOPA_DS_DESCRICAO);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<LOCACAO_PARCELA> listaObj = new List<LOCACAO_PARCELA>();
                Tuple<Int32, List<LOCACAO_PARCELA>, Boolean> volta = baseApp.ExecuteFilterTupleParcela(item.LOCA_CD_ID, item.LOPA_IN_PACIENTE_DUMMY, item.LOPA_DT_VENCIMENTO, item.LOPA_DT_DUMMY, item.LOPA_NM_PARCELAS, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLocacao"] = 1;
                    return RedirectToAction("VerParcelasLocacoes");
                }

                // Sucesso
                listaMasterParcela = volta.Item2.ToList();
                Session["ListaParcelaLocacaoGeral"] = listaMasterParcela;
                return RedirectToAction("VerParcelasLocacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroParcelaLocacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaParcelaLocacaoGeral"] = null;
                return RedirectToAction("VerParcelasLocacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarDistratoPDF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Distrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_DISTRATO_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("D I S T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 135f, 600f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);
                String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cancelamento: " + locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Justificativa: " + locacao.LOCA_DS_JUSTIFICATIVA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
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

                if ((Int32)Session["VoltaPrintLocacao"] == 2)
                {
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
                }
                return RedirectToAction("VoltarAnexoLocacao");
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

        public ActionResult GerarDistratoPDFAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Distrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_DISTRATO_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("D I S T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Cancelamento: " + locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Justificativa: " + locacao.LOCA_DS_JUSTIFICATIVA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
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

                if ((Int32)Session["VoltaPrintLocacao"] == 2)
                {
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
                }
                return RedirectToAction("VoltarAnexoLocacao");
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

        public Int32 GerarDistratoPDFTeste()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Distrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_DISTRATO_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("D I S T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 135f, 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);
                    String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                    cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Cancelamento: " + locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Justificativa: " + locacao.LOCA_DS_JUSTIFICATIVA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);


                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarDistratoPDFTesteAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Distrato_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_DISTRATO_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA    == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("D I S T R A T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                    cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Cancelamento: " + locacao.LOCA_DT_CANCELAMENTO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Justificativa: " + locacao.LOCA_DS_JUSTIFICATIVA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);


                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        [HttpGet]
        public ActionResult AprovarLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Aprovação";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                LOCACAO item = baseApp.GetItemById(id);
                Session["IdLocacao"] = item.LOCA_CD_ID;

                PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                List<SelectListItem> saida = new List<SelectListItem>();
                saida.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                saida.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Saida = new SelectList(saida, "Value", "Text");

                // Verifica se tem contrato assinado
                String nomeArq = "Contrato_Locacao" + pac.PACI_NM_NOME + "_" + item.LOCA_GU_GUID + ".pdf";
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Assinado/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeArq);
                Boolean existe = System.IO.File.Exists(filePath);
                if (!existe)
                {
                    Session["MensLocacao"] = 88;
                    if ((Int32)Session["VoltaLocacao"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                    return RedirectToAction("MontarTelaLocacao");
                }

                // Prepara view
                Session["LocacaoAntes"] = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                vm.LOCA_DT_APROVACAO = DateTime.Today.Date;
                vm.CONTRATO_ASSINA = existe ? "Sim" : "Não";
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AprovarLocacao(LocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> saida = new List<SelectListItem>();
            saida.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            saida.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Saida = new SelectList(saida, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOCA_DS_JUSTIFICATIVA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOCA_DS_JUSTIFICATIVA);

                    // Verifica estoque
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    if (prod.PROD_VL_ESTOQUE_ATUAL < 1)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0695", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase + " - Estoque Atual: " + prod.PROD_VL_ESTOQUE_ATUAL.ToString());
                        return View(vm);
                    }

                    // Executa a operação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    Session["ProdAntes"] = prod;
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.LOCA_IN_STATUS = 1;
                    Int32 volta = baseApp.ValidateEdit(item, (LOCACAO)Session["LocacaoAntes"], usuario);

                    // Monta Log
                    DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Aprovação",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Trata estoque
                    if (vm.LOCA_IN_ESTOQUE == 1)
                    {
                        // Atualiza estoque
                        prod.PROD_VL_ESTOQUE_ATUAL = prod.PROD_VL_ESTOQUE_ATUAL - item.LOCA_IN_QUANTIDADE;
                        Int32 voltaProd = prodApp.ValidateEdit(prod, (PRODUTO)Session["ProdAntes"]);

                        // Atualiza movimento de estoque
                        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                        mov.ASSI_CD_ID = idAss;
                        mov.EMFI_CD_ID = usuario.EMFI_CD_ID;
                        mov.EMPR_CD_ID = usuario.EMPR_CD_ID;
                        mov.MOEP_DS_JUSTIFICATIVA = vm.LOCA_DS_ENTREGA;
                        mov.MOEP_DT_LANCAMENTO = DateTime.Today.Date;
                        mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                        mov.MOEP_GU_GUID = Xid.NewXid().ToString();
                        mov.MOEP_IN_ATIVO = 1;
                        mov.MOEP_IN_OPERACAO = 1;
                        mov.MOEP_IN_PENDENTE = 0;
                        mov.MOEP_IN_SISTEMA = 6;
                        mov.MOEP_IN_TIPO = 8;
                        mov.MOEP_IN_TIPO_LANCAMENTO = 2;
                        mov.MOEP_QN_QUANTIDADE = vm.LOCA_IN_QUANTIDADE.Value;
                        mov.MOEP_VL_QUANTIDADE_ANTERIOR = ((PRODUTO)Session["ProdAntes"]).PROD_VL_ESTOQUE_ATUAL;
                        mov.MOEP_VL_QUANTIDADE_MOVIMENTO = 1;
                        mov.MOEP_VL_VALOR_MOVIMENTO = 0;
                        mov.PROD_CD_ID = vm.PROD_CD_ID;
                        mov.USUA_CD_ID = usuario.USUA_CD_ID;
                        mov.MOEP_IN_TIPO_MOVIMENTO = 2;
                        mov.MOEP_IN_ORIGEM = "Aprovação de Locação";
                        Int32 voltaC = prodApp.ValidateCreateMovimento(mov, usuario);

                        // Inclui historico de estoque
                        PRODUTO_ESTOQUE_HISTORICO est = new PRODUTO_ESTOQUE_HISTORICO();
                        est.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        est.PROD_CD_ID = vm.PROD_CD_ID;
                        est.PREH_IN_ATIVO = 1;
                        est.PREH_DT_DATA = DateTime.Today.Date;
                        est.PREH_DT_COMPLETA = DateTime.Now;
                        est.PREH_QN_ESTOQUE = vm.LOCA_IN_QUANTIDADE.Value;
                        est.PREH_IN_PENDENTE = 0;
                        est.PREH_NM_TIPO = "Saída";
                        est.PREH_DS_ORIGEM = "Aprovação de Locação";
                        est.MOEP_CD_ID = mov.MOEP_CD_ID;
                        Int32 voltaH = prodApp.ValidateCreateEstoqueHistorico(est, idAss);
                    }

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Aprovação da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Aprovação de Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Envia mensagem
                    LOCACAO locMensagem = baseApp.GetItemById(item.LOCA_CD_ID);
                    if (pac.PACI_NM_EMAIL != null)
                    {
                        Int32 voltaCons = await EnviarEMailLocacao(locMensagem, 3);
                    }
                    if (pac.PACI_NR_CELULAR != null)
                    {
                        Int32 voltaCons = EnviarSMSLocacao(locMensagem, 3);
                    }

                    // Mensages do CRUD
                    Session["MsgCRUD"] = "A Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper() + " - foi aprovada com sucesso.";
                    Session["MensLocacao"] = 61;
                    if (item.LOCA_IN_ESTOQUE == 0)
                    {
                        Session["MsgCRUD1"] = "Não foi feita a saída no estoque para " + prod.PROD_NM_NOME.ToUpper() + " referente à locação de " + pac.PACI_NM_NOME.ToUpper() + " - Deve ser feita saída manual.";
                        Session["MensProduto"] = 62;
                    }

                    // Retorno
                    listaMaster = new List<LOCACAO>();
                    Session["ListaLocacao"] = null;
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaHistoricoLocacao"] = null;
                    Session["LocacoesHistoricos"] = null;

                    if ((Int32)Session["VoltaLocacao"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                    return RedirectToAction("MontarTelaLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult AprovarLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("AprovarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        [HttpGet]
        public ActionResult EncerrarLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Encerramento";
                        return RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                LOCACAO item = baseApp.GetItemById(id);
                Session["IdLocacao"] = item.LOCA_CD_ID;

                // Checa parcelas
                if (item.LOCA_IN_STATUS == 1)
                {
                    Int32 pago = item.LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 0 & p.LOPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date).ToList().Count();
                    if (pago > 0)
                    {
                        Session["MsgCRUD"] = "A locação " + item.LOCA_NM_TITULO + " não pode ser encerrada pois tem parcelas vencidas em aberto";
                        Session["MensLocacao"] = 71;
                        RedirectToAction("MontarTelaLocacao", "Locacao");
                    }
                }

                PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                List<SelectListItem> recebe = new List<SelectListItem>();
                recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
                ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 3), "COLO_CD_ID", "COLO_NM_NOME");

                // Prepara view
                Session["LocacaoAntes"] = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                vm.LOCA_DT_ENCERRAMENTO = DateTime.Today.Date;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EncerrarLocacao(LocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> recebe = new List<SelectListItem>();
            recebe.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            recebe.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Recebe = new SelectList(recebe, "Value", "Text");
            ViewBag.ContratoLocacao = new SelectList(CarregarContratoLocacao().Where(p => p.TICO_CD_ID == 3), "COLO_CD_ID", "COLO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOCA_DS_ENCERRA = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.LOCA_DS_ENCERRA);

                    // Critica
                    if (vm.LOCA_CD_ENCERRA_ID == 0 || vm.LOCA_CD_ENCERRA_ID == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0708", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOCA_IN_ESTOQUE == 0 || vm.LOCA_IN_ESTOQUE == null)
                    {
                        vm.LOCA_IN_ESTOQUE = 0;
                    }

                    // Executa a operação
                    LOCACAO item = Mapper.Map<LocacaoViewModel, LOCACAO>(vm);
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID);
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    Session["ProdAntes"] = prod;
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    item.LOCA_IN_STATUS = 2;
                    Int32 volta = baseApp.ValidateEdit(item, (LOCACAO)Session["LocacaoAntes"], usuario);

                    // Monta Log
                    DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Encerramento",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Trata estoque
                    if (vm.LOCA_IN_ESTOQUE == 1)
                    {
                        // Atualiza estoque
                        prod.PROD_VL_ESTOQUE_ATUAL = prod.PROD_VL_ESTOQUE_ATUAL + item.LOCA_IN_QUANTIDADE;
                        Int32 voltaProd = prodApp.ValidateEdit(prod, (PRODUTO)Session["ProdAntes"]);

                        // Atualiza movimento de estoque
                        MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                        mov.ASSI_CD_ID = idAss;
                        mov.EMFI_CD_ID = usuario.EMFI_CD_ID;
                        mov.EMPR_CD_ID = usuario.EMPR_CD_ID;
                        mov.MOEP_DS_JUSTIFICATIVA = vm.LOCA_DS_JUSTIFICATIVA;
                        mov.MOEP_DT_LANCAMENTO = DateTime.Today.Date;
                        mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                        mov.MOEP_GU_GUID = Xid.NewXid().ToString();
                        mov.MOEP_IN_ATIVO = 1;
                        mov.MOEP_IN_OPERACAO = 1;
                        mov.MOEP_IN_PENDENTE = 0;
                        mov.MOEP_IN_SISTEMA = 6;
                        mov.MOEP_IN_TIPO = 9;
                        mov.MOEP_IN_TIPO_LANCAMENTO = 1;
                        mov.MOEP_QN_QUANTIDADE = vm.LOCA_IN_QUANTIDADE.Value;
                        mov.MOEP_VL_QUANTIDADE_ANTERIOR = ((PRODUTO)Session["ProdAntes"]).PROD_VL_ESTOQUE_ATUAL;
                        mov.MOEP_VL_QUANTIDADE_MOVIMENTO = 1;
                        mov.MOEP_VL_VALOR_MOVIMENTO = 0;
                        mov.PROD_CD_ID = vm.PROD_CD_ID;
                        mov.USUA_CD_ID = usuario.USUA_CD_ID;
                        mov.MOEP_IN_ORIGEM = "Encerramento de Locação";
                        Int32 voltaC = prodApp.ValidateCreateMovimento(mov, usuario);

                        // Inclui historico de estoque
                        PRODUTO_ESTOQUE_HISTORICO est = new PRODUTO_ESTOQUE_HISTORICO();
                        est.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        est.PROD_CD_ID = vm.PROD_CD_ID;
                        est.PREH_IN_ATIVO = 1;
                        est.PREH_DT_DATA = DateTime.Today.Date;
                        est.PREH_DT_COMPLETA = DateTime.Now;
                        est.PREH_QN_ESTOQUE = 1;
                        est.PREH_IN_PENDENTE = 0;
                        est.PREH_NM_TIPO = "Entrada";
                        est.PREH_DS_ORIGEM = "Encerramento de Locação";
                        est.MOEP_CD_ID = mov.MOEP_CD_ID;
                        Int32 voltaH = prodApp.ValidateCreateEstoqueHistorico(est, idAss);
                    }

                    // Grava historico
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Encerramento da Locação " + item.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuario.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Encerramento de Locação";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Envia mensagem
                    LOCACAO locMensagem = baseApp.GetItemById(item.LOCA_CD_ID);
                    if (pac.PACI_NM_EMAIL != null)
                    {
                        Int32 voltaCons = await EnviarEMailLocacao(locMensagem, 4);
                    }
                    if (pac.PACI_NR_CELULAR != null)
                    {
                        Int32 voltaCons = EnviarSMSLocacao(locMensagem, 4);
                    }

                    // Mensages do CRUD
                    Session["MsgCRUD"] = "A Locação de " + prod.PROD_NM_NOME.ToUpper() + " para " + pac.PACI_NM_NOME.ToUpper() + " - foi encerrada com sucesso.";
                    Session["MensLocacao"] = 61;
                    if (item.LOCA_IN_ESTOQUE == 0)
                    {
                        Session["MsgCRUD1"] = "Não foi feita a entrada no estoque para " + prod.PROD_NM_NOME.ToUpper() + " referente à locação de " + pac.PACI_NM_NOME.ToUpper() + " - Deve ser feita entrada manual.";
                        Session["MensProduto"] = 62;
                    }

                    // Trata encerramento
                    if (item.LOCA_IN_ASSINADO_DIGITAL == 0)
                    {
                        Int32 rel = GerarEncerraPDFTeste();
                    }
                    else
                    {
                        Int32 rel = GerarEncerraPDFTesteAssina();

                    }
                    Int32 voltaD = await ProcessaEnvioEMailEncerra(item, usuario);

                    // Retorno
                    listaMaster = new List<LOCACAO>();
                    Session["ListaLocacao"] = null;
                    Session["IdLocacao"] = item.LOCA_CD_ID;
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaHistoricoLocacao"] = null;
                    Session["LocacoesHistoricos"] = null;

                    if ((Int32)Session["VoltaLocacao"] == 2)
                    {
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                    return RedirectToAction("MontarTelaLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult EncerrarLocacaoForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EncerrarLocacao", new { id = (Int32)Session["IdLocacao"] });
        }

        public Int32 GerarEncerraPDFTeste()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Encerra_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_ENCERRA_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("E N C E R R A M E N T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 135f, 600f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);
                    String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                    cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Encerramento: " + locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Observações: " + locacao.LOCA_DS_ENCERRA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);


                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        public Int32 GerarEncerraPDFTesteAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Encerra_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_ENCERRA_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }

                // Processamento
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    // Cabeçalho
                    PdfPTable headerTable = null;
                    PdfPCell cell = new PdfPCell();
                    Image image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        headerTable = new PdfPTable(new float[] { 20f, 700f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.Colspan = 1;
                        image = null;
                        if (conf.CONF_IN_LOGO_EMPRESA == 1)
                        {
                            image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                        }
                        else
                        {
                            image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                        }
                        image.ScaleAbsolute(80, 80);
                        cell.AddElement(image);
                        headerTable.AddCell(cell);
                    }
                    else
                    {
                        headerTable = new PdfPTable(new float[] { 750f });
                        headerTable.WidthPercentage = 100;
                        headerTable.HorizontalAlignment = 1;
                        headerTable.SpacingBefore = 1f;
                        headerTable.SpacingAfter = 1f;
                    }

                    PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("E N C E R R A M E N T O  D E  L O C A Ç Ã O", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table1.AddCell(cell);

                    PdfPCell innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    headerTable.AddCell(innerTableCell);

                    // Rodape
                    PdfPTable footerTable = new PdfPTable(1);
                    footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                    footerTable.WidthPercentage = 100;
                    footerTable.HorizontalAlignment = 1;
                    footerTable.SpacingBefore = 1f;
                    footerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                    {
                        image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                    }
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Dados da empresa
                    table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = 0;
                    table1.SpacingBefore = 1f;
                    table1.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

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

                    cell = new PdfPCell(new Paragraph(endereco, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                    cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("  ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.AddCell(cell);

                    innerTableCell = new PdfPCell(table1);
                    innerTableCell.Border = Rectangle.NO_BORDER;
                    innerTableCell.Colspan = 1;
                    footerTable.AddCell(innerTableCell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    footerTable.AddCell(cell);

                    // Cria documento
                    Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                    pdfDoc.Open();

                    // Dados do contrato
                    PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Texto do contrato
                    Paragraph line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Texto legal
                    Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk1);

                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Area de Assinatura do paciente
                    Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                    pdfDoc.Add(chunk2);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("_____________________________________________________________  ");
                    pdfDoc.Add(line1);
                    Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    pdfDoc.Add(chunk3);
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);
                    line1 = new Paragraph("=========================================================================");
                    pdfDoc.Add(line1);

                    // Dados do paciente
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do contratado
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados do equipamento
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph("  ");
                    pdfDoc.Add(line1);

                    // Dados da locação
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Encerramento: " + locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString(), meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Observações: " + locacao.LOCA_DS_ENCERRA, meuFont1));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);


                    // Finaliza
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailEncerra(LOCACAO vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["UserCredentials"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            PACIENTE paciente = pacApp.GetItemById(vm.PACI_CD_ID);
            PRODUTO produto = prodApp.GetItemById(vm.PROD_CD_ID);

            // Configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Recupera Template
            TEMPLATE_EMAIL template = temApp.GetByCode("ENCELOCA", idAss);

            // Prepara cabeçalho
            String cab = template.TEEM_TX_CABECALHO;

            // Prepara assinatura
            String assinatura = String.Empty;
            EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
            assinatura = "<b>" + emp.EMPR_NM_NOME + "</b><br />";
            assinatura += "<b>CNPJ: </b>" + emp.EMPR_NR_CNPJ + "<br />";
            assinatura += "Enviado por <b>WebDoctor</b><br />";

            // Prepara corpo da mensagem
            String texto = template.TEEM_TX_CORPO;
            String urlDestino = conf.CONF_LK_LINK_SISTEMA;
            String linkHtml = $"<a href=\"{urlDestino}\">{urlDestino}</a>";
            if (texto.Contains("{produto}"))
            {
                texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
            }
            if (texto.Contains("{paciente}"))
            {
                texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
            }
            if (texto.Contains("{data}"))
            {
                texto = texto.Replace("{data}", vm.LOCA_DT_INICIO.Value.ToLongDateString());
            }
            if (texto.Contains("{encerramento}"))
            {
                texto = texto.Replace("{encerramento}", vm.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString());
            }
            if (texto.Contains("{observacao}"))
            {
                texto = texto.Replace("{observacao}", vm.LOCA_DS_ENCERRA);
            }
            if (texto.Contains("{link}"))
            {
                texto = texto.Replace("{link}", linkHtml);
            }
            String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

            // Incluir PDF como anexo
            List<AttachmentModel> models = new List<AttachmentModel>();
            String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + vm.LOCA_CD_ID.ToString() + "/Distrato/";
            String fileNamePDF = "Encerra_Locacao" + paciente.PACI_NM_NOME.ToUpper() + "_" + vm.LOCA_GU_GUID + ".pdf";
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
            mensagem.ASSUNTO = "Locação - " + vm.LOCA_NM_TITULO.ToUpper();
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
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = paciente.PACI_NM_NOME;
                mens.ID = null;
                mens.MODELO = paciente.PACI_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_CAMPANHA = paciente.PACI_NM_EMAIL;
                mens.MENS_NM_NOME = "Envio de Encerramento de Locação para Paciente: " + paciente.PACI_NM_NOME.ToUpper();
                mens.MENS_GU_GUID = vm.LOCA_GU_GUID;
                mens.MENS_DT_AGENDAMENTO = vm.LOCA_DT_EMISSAO;
                mens.MENS_DT_ENVIO = DateTime.Today.Date;
                mens.MENS_NM_CABECALHO = paciente.PACI_NR_CPF;
                mens.MENS_NR_REPETICOES = 0;
                mens.MENS_NM_ASSINATURA = usuario.USUA_NM_NOME;
                mens.MENS_NM_RODAPE = String.Empty;
                mens.CELULAR = paciente.PACI_NR_CELULAR;
                mens.TELEFONE = paciente.PACI_NR_TELEFONE;
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                mens.MENS_TX_TEXTO = texto;

                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(usuApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(mens, usuario, emailBody, status, iD, erro, "Locacao - Envio Encerramento");
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

        public ActionResult ImprimirDistratoLocacaoDireto()
        {
            LOCACAO loca = baseApp.GetItemById((Int32)Session["IdLocacao"]);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarDistratoPDFAssina");
            }
            return RedirectToAction("GerarDistratoPDF");
        }

        public ActionResult ImprimirEncerraLocacaoDireto()
        {
            LOCACAO loca = baseApp.GetItemById((Int32)Session["IdLocacao"]);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("GerarEncerraPDFAssina");
            }
            return RedirectToAction("GerarEncerraPDF");
        }

        public ActionResult ImprimirEncerraLocacao(Int32 id)
        {
            Session["IdLocacao"] = id;
            LOCACAO loca = baseApp.GetItemById(id);
            if (loca.LOCA_IN_ASSINADO_DIGITAL == 1)
            {
                return RedirectToAction("   ");
            }
            return RedirectToAction("GerarEncerraPDF");
        }

        public ActionResult GerarEncerraPDF()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Encerra_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_ENCERRA_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{observacao}"))
                {
                    texto = texto.Replace("{observacao}", locacao.LOCA_DS_ENCERRA);
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("E N C E R R A M E N T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 135f, 600f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Token para validação: " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);
                String msg = "(*) Para validar este documento use o QR Code acima ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " usando o token de acesso " + token;
                cell.AddElement(new Chunk(msg, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK)));
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Encerramento: " + locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Observações: " + locacao.LOCA_DS_ENCERRA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
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

                if ((Int32)Session["VoltaPrintLocacao"] == 2)
                {
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
                }
                return RedirectToAction("VoltarAnexoLocacao");
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

        public ActionResult GerarEncerraPDFAssina()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                // Recupera informações
                LOCACAO locacao = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                List<LOCACAO_PARCELA> parcs = locacao.LOCACAO_PARCELA.ToList();
                PACIENTE paciente = pacApp.GetItemById(locacao.PACI_CD_ID);
                PRODUTO produto = prodApp.GetItemById(locacao.PROD_CD_ID);
                String nomeRel = "Encerra_Locacao" + paciente.PACI_NM_NOME + "_" + locacao.LOCA_GU_GUID + ".pdf";
                
                EMPRESA empresa = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                String token = locacao.LOCA_TK_TOKEN;

                // Monta endereços
                String enderecoEmp = String.Empty;
                if (empresa.EMPR_NM_ENDERECO != null)
                {
                    enderecoEmp += empresa.EMPR_NM_ENDERECO;
                    if (empresa.EMPR_NM_NUMERO != null)
                    {
                        enderecoEmp += " " + empresa.EMPR_NM_NUMERO;
                    }
                    if (empresa.EMPR_NM_COMPLEMENTO != null)
                    {
                        enderecoEmp += "/" + empresa.EMPR_NM_COMPLEMENTO;
                    }
                    if (empresa.EMPR_NM_BAIRRO != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_BAIRRO;
                    }
                    if (empresa.EMPR_NM_CIDADE != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NM_CIDADE;
                    }
                    if (empresa.UF != null)
                    {
                        enderecoEmp += ", " + empresa.UF.UF_SG_SIGLA;
                    }
                    if (empresa.EMPR_NR_CEP != null)
                    {
                        enderecoEmp += ", " + empresa.EMPR_NR_CEP;
                    }
                }

                String enderecoPac = String.Empty;
                if (paciente.PACI_NM_ENDERECO != null)
                {
                    enderecoPac += paciente.PACI_NM_ENDERECO;
                    if (paciente.PACI_NR_NUMERO != null)
                    {
                        enderecoPac += " " + paciente.PACI_NR_NUMERO;
                    }
                    if (paciente.PACI_NR_COMPLEMENTO != null)
                    {
                        enderecoPac += "/" + paciente.PACI_NR_COMPLEMENTO;
                    }
                    if (paciente.PACI_NM_BAIRRO != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_BAIRRO;
                    }
                    if (paciente.PACI_NM_CIDADE != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NM_CIDADE;
                    }
                    if (paciente.UF != null)
                    {
                        enderecoPac += ", " + paciente.UF.UF_SG_SIGLA;
                    }
                    if (paciente.PACI_NR_CEP != null)
                    {
                        enderecoPac += ", " + paciente.PACI_NR_CEP;
                    }
                }
                else
                {
                    enderecoPac += "NÃO INFORMADO";
                }

                // Monta data contrato
                String dataContrato = String.Empty;
                dataContrato = (empresa.EMPR_NM_CIDADE != null ? empresa.EMPR_NM_CIDADE : "Rio de Janeiro") + ", " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString();

                // Prepara fontes
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1Bold = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont3Bold = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont4Bold = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFont5Bold = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Caminho de saida
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + locacao.LOCA_CD_ID.ToString() + "/Distrato/";
                String filePath = Path.Combine(Server.MapPath(caminho), nomeRel);
                Directory.CreateDirectory(caminho);
                Boolean existe = System.IO.File.Exists(filePath);
                if (existe)
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }
                }

                // Recupera texto legal
                CONTRATO_LOCACAO template = baseApp.GetContratoById(locacao.LOCA_CD_ENCERRA_ID.Value);
                String texto = template.COLO_TX_TEXTO;

                // Monta texto legal
                if (texto.Contains("{assinante}"))
                {
                    texto = texto.Replace("{assinante}", empresa.EMPR_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cnpj}"))
                {
                    texto = texto.Replace("{cnpj}", empresa.EMPR_NR_CNPJ);
                }
                if (texto.Contains("{endereco}"))
                {
                    texto = texto.Replace("{endereco}", enderecoPac);
                }
                if (texto.Contains("{sede}"))
                {
                    texto = texto.Replace("{sede}", enderecoEmp);
                }
                if (texto.Contains("{contratado}"))
                {
                    texto = texto.Replace("{contratado}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{data}"))
                {
                    texto = texto.Replace("{data}", locacao.LOCA_DT_INICIO.Value.ToLongDateString());
                }
                if (texto.Contains("{final}"))
                {
                    texto = texto.Replace("{final}", locacao.LOCA_DT_FINAL.Value.ToLongDateString());
                }
                if (texto.Contains("{numParc}"))
                {
                    texto = texto.Replace("{numParc}", locacao.LOCACAO_PARCELA.Count().ToString());
                }
                if (texto.Contains("{valor}"))
                {
                    texto = texto.Replace("{valor}", CrossCutting.Formatters.DecimalFormatter(locacao.LOCA_VL_PARCELA.Value));
                }
                if (texto.Contains("{dia}"))
                {
                    texto = texto.Replace("{dia}", locacao.LOCA_NR_DIA.ToString());
                }
                if (texto.Contains("{paciente}"))
                {
                    texto = texto.Replace("{paciente}", paciente.PACI_NM_NOME.ToUpper());
                }
                if (texto.Contains("{cpf}"))
                {
                    texto = texto.Replace("{cpf}", paciente.PACI_NR_CPF);
                }
                if (texto.Contains("{produto}"))
                {
                    texto = texto.Replace("{produto}", produto.PROD_NM_NOME.ToUpper());
                }
                if (texto.Contains("{marca}"))
                {
                    texto = texto.Replace("{marca}", produto.PROD_NM_MARCA);
                }
                if (texto.Contains("{modelo}"))
                {
                    texto = texto.Replace("{modelo}", produto.PROD_NM_MODELO);
                }
                if (texto.Contains("{serie}"))
                {
                    texto = texto.Replace("{serie}", locacao.LOCA_NR_SERIE);
                }
                if (texto.Contains("{quant}"))
                {
                    texto = texto.Replace("{quant}", locacao.LOCA_IN_QUANTIDADE.ToString());
                }
                if (texto.Contains("{justificativa}"))
                {
                    texto = texto.Replace("{justificativa}", locacao.LOCA_DS_JUSTIFICATIVA);
                }

                // Cabeçalho
                PdfPTable headerTable = null;
                PdfPCell cell = new PdfPCell();
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    headerTable = new PdfPTable(new float[] { 20f, 700f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 1;
                    image = null;
                    if (conf.CONF_IN_LOGO_EMPRESA == 1)
                    {
                        image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    }
                    else
                    {
                        image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                    }
                    image.ScaleAbsolute(80, 80);
                    cell.AddElement(image);
                    headerTable.AddCell(cell);
                }
                else
                {
                    headerTable = new PdfPTable(new float[] { 750f });
                    headerTable.WidthPercentage = 100;
                    headerTable.HorizontalAlignment = 1;
                    headerTable.SpacingBefore = 1f;
                    headerTable.SpacingAfter = 1f;
                }

                PdfPTable table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("E N C E R R A M E N T O  D E  L O C A Ç Ã O", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont4Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell);

                PdfPCell innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                headerTable.AddCell(innerTableCell);

                // Rodape
                PdfPTable footerTable = new PdfPTable(1);
                footerTable = new PdfPTable(new float[] { 160f, 600f, 180f });
                footerTable.WidthPercentage = 100;
                footerTable.HorizontalAlignment = 1;
                footerTable.SpacingBefore = 1f;
                footerTable.SpacingAfter = 1f;

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                if (locacao.LOCA_AQ_ARQUIVO_QRCODE != null)
                {
                    image = Image.GetInstance(Server.MapPath(locacao.LOCA_AQ_ARQUIVO_QRCODE));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Imagens/Base/qrcode.png"));
                }
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Dados da empresa
                table1 = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

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

                cell = new PdfPCell(new Paragraph(endereco, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(enderecoCont, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("E-Mail: " + empresa.EMPR_NM_EMAIL, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefone: " + empresa.EMPR_NR_TELEFONE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                String fraseAssina = "Documento assinado digitalmente em " + locacao.LOCA_DT_EMISSAO.Value.ToShortDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToShortTimeString() + " conforme MP 2.200-2/01";
                cell = new PdfPCell(new Paragraph(fraseAssina, meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Para validar este documento use o código QR ao lado ou acesse " + conf.CONF_LK_LINK_VALIDACAO + " e use o token de acesso " + token, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(cell);

                innerTableCell = new PdfPCell(table1);
                innerTableCell.Border = Rectangle.NO_BORDER;
                innerTableCell.Colspan = 1;
                footerTable.AddCell(innerTableCell);

                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = null;
                image = Image.GetInstance(Server.MapPath("~/Imagens/Base/Selo_Digital.png"));
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                footerTable.AddCell(cell);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 70, 140);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados do contrato
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Data/Hora da Emissão: " + locacao.LOCA_DT_EMISSAO.Value.ToLongDateString() + " " + locacao.LOCA_DT_EMISSAO.Value.ToLongTimeString() + " (GMT-3)", meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador: " + locacao.LOCA_GU_GUID, meuFont1Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Texto do contrato
                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Texto legal
                Chunk chunk1 = new Chunk(texto, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Area de Assinatura do paciente
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                Chunk chunk2 = new Chunk(dataContrato, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk2);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("_____________________________________________________________  ");
                pdfDoc.Add(line1);
                Chunk chunk3 = new Chunk(paciente.PACI_NM_NOME, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk3);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);
                line1 = new Paragraph("=============================================================================");
                pdfDoc.Add(line1);

                // Dados do paciente
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATANTE", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + paciente.PACI_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF: " + paciente.PACI_NR_CPF, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do contratado
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO CONTRATADO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + empresa.EMPR_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CNPJ: " + empresa.EMPR_NR_CNPJ, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados do equipamento
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DO EQUIPAMENTO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome: " + produto.PROD_NM_NOME, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + produto.PROD_NM_MARCA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Num. de Série: " + locacao.LOCA_NR_SERIE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade: " + locacao.LOCA_IN_QUANTIDADE, meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados da locação
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("DADOS DA LOCAÇÃO", meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Título: " + locacao.LOCA_NM_TITULO, meuFont3Bold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Início: " + locacao.LOCA_DT_INICIO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Término: " + locacao.LOCA_DT_FINAL.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Encerramento: " + locacao.LOCA_DT_ENCERRAMENTO.Value.ToLongDateString(), meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Observações: " + locacao.LOCA_DS_ENCERRA, meuFont1));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
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

                if ((Int32)Session["VoltaPrintLocacao"] == 2)
                {
                    return RedirectToAction("VoltarAnexoProduto", "Produto");
                }
                return RedirectToAction("VoltarAnexoLocacao");
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
        public ActionResult ExcluirLocacaoOcorrencia(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO__EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Ocorrencia";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                LOCACAO_OCORRENCIA item = baseApp.GetOcorrenciaById(id);
                LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                item.LOOC_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditOcorrencia(item);

                Session["LocacaoAlterada"] = 1;
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 5;
                Session["ListaLocacao"] = null;

                // Grava historico
                LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                hist.ASSI_CD_ID = idAss;
                hist.LOCA_CD_ID = item.LOCA_CD_ID;
                hist.LOHI_DS_DESCRICAO = "Exclusão de Ocorrência da Locação " + loca.LOCA_NM_TITULO;
                hist.LOHI_DT_HISTORICO = DateTime.Now;
                hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                hist.LOHI_IN_ATIVO = 1;
                hist.LOHI_NM_OPERACAO = "Exclusão de Ocorrência";
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Monta Log
                DTO_Ocorrencia dto = MontarOcorrenciaDTO(item.LOOC_CD_ID);
                String json = JsonConvert.SerializeObject(dto);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Locação - Ocorrência - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A ocorrência " + item.LOOC_NM_TITULO.ToUpper() + " da locação " + loca.LOCA_NM_TITULO.ToUpper() + " foi excluída com sucesso";
                Session["MensLocacao"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirLocacaoOcorrencia()
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
                        Session["ModuloPermissao"] = "Locação - Ocorrência - Inclusão";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locação - Inclusão de Ocorrencia";

                // Prepara view
                LOCACAO loca = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                PACIENTE pac = pacApp.GetItemById(loca.PACI_CD_ID);
                PRODUTO prod = prodApp.GetItemById(loca.PROD_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;
                ViewBag.Tipos = new SelectList(baseApp.GetAllTipoOcorrencia(idAss), "TIOC_CD_ID", "TIOC_NM_NOME");

                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 5;
                LOCACAO_OCORRENCIA item = new LOCACAO_OCORRENCIA();
                LocacaoOcorrenciaViewModel vm = Mapper.Map<LOCACAO_OCORRENCIA, LocacaoOcorrenciaViewModel>(item);
                vm.LOCA_CD_ID = loca.LOCA_CD_ID;
                vm.LOOC_IN_ATIVO = 1;
                vm.LOOC_DT_OCORRENCIA = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.PACIENTE_BASE = pac;
                vm.PRODUTO_BASE = prod;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_OCORRENCIA_INCLUIR", "Locacao", "IncluirLocacaoOcorrencia");

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLocacaoOcorrencia(LocacaoOcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIOC_CD_ID", "TIOC_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOOC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_NM_TITULO);
                    vm.LOOC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_DS_DESCRICAO);
                    vm.LOOC_SERIE_ENTRADA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_SERIE_ENTRADA);
                    vm.LOOC_SERIE_SAIDA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_SERIE_SAIDA);

                    // Critica
                    if (vm.LOOC_DT_OCORRENCIA == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0701", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOOC_DT_OCORRENCIA > DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0702", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOOC_SERIE_ENTRADA == null & vm.LOOC_SERIE_SAIDA != null)
                    {
                        if (vm.LOOC_SERIE_SAIDA == vm.LOOC_SERIE_SAIDA)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0703", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.TIOC_CD_ID == null || vm.TIOC_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0704", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    LOCACAO_OCORRENCIA item = Mapper.Map<LocacaoOcorrenciaViewModel, LOCACAO_OCORRENCIA>(vm);
                    Int32 volta = baseApp.ValidateCreateOcorrencia(item);

                    // Acerta locação
                    LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                    loca.LOCA_NR_SERIE = item.LOOC_SERIE_SAIDA;
                    Int32 volta1 = baseApp.ValidateEdit(loca, loca, usuarioLogado);

                    // Verifica retorno
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 5;
                    Session["ListaLocacao"] = null;

                    // Grava historico
                    LOCACAO loca1 = baseApp.GetItemById(item.LOCA_CD_ID);
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Inclusão de Ocorrência da Locação " + loca1.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Inclusão de Ocorrência";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Monta Log
                    DTO_Ocorrencia dto = MontarOcorrenciaDTO(item.LOOC_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Ocorrência - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A ocorrência " + item.LOOC_NM_TITULO.ToUpper() + " da locação " + loca.LOCA_NM_TITULO.ToUpper() + " foi incluída com sucesso";
                    Session["MensLocacao"] = 61;

                    return RedirectToAction("VoltarAnexoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarLocacaoOcorrencia(Int32 id)
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
                        Session["ModuloPermissao"] = "Locação - Ocorrência - Edição";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Locação - Inclusão de Ocorrencia";

                // Prepara view
                LOCACAO loca = baseApp.GetItemById((Int32)Session["IdLocacao"]);
                PACIENTE pac = pacApp.GetItemById(loca.PACI_CD_ID);
                PRODUTO prod = prodApp.GetItemById(loca.PROD_CD_ID);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;
                ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIOC_CD_ID", "TIOC_NM_NOME");

                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 5;

                LOCACAO_OCORRENCIA item = baseApp.GetOcorrenciaById(id);
                LocacaoOcorrenciaViewModel vm = Mapper.Map<LOCACAO_OCORRENCIA, LocacaoOcorrenciaViewModel>(item);
                vm.PACIENTE_BASE = pac;
                vm.PRODUTO_BASE = prod;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_OCORRENCIA_ALTERAR", "Locacao", "EditarLocacaoOcorrencia");

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLocacaoOcorrencia(LocacaoOcorrenciaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(baseApp.GetAllTipos(idAss), "TIOC_CD_ID", "TIOC_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.LOOC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_NM_TITULO);
                    vm.LOOC_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_DS_DESCRICAO);
                    vm.LOOC_SERIE_ENTRADA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_SERIE_ENTRADA);
                    vm.LOOC_SERIE_SAIDA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.LOOC_SERIE_SAIDA);

                    // Critica
                    if (vm.LOOC_DT_OCORRENCIA == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0701", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOOC_DT_OCORRENCIA > DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0702", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.LOOC_SERIE_ENTRADA != null & vm.LOOC_SERIE_SAIDA != null)
                    {
                        if (vm.LOOC_SERIE_SAIDA == vm.LOOC_SERIE_SAIDA)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0703", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Executa a operação
                    LOCACAO_OCORRENCIA item = Mapper.Map<LocacaoOcorrenciaViewModel, LOCACAO_OCORRENCIA>(vm);
                    Int32 volta = baseApp.ValidateEditOcorrencia(item);

                    // Verifica retorno
                    Session["LocacaoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 5;
                    Session["ListaLocacao"] = null;

                    // Grava historico
                    LOCACAO loca = baseApp.GetItemById(item.LOCA_CD_ID);
                    LOCACAO_HISTORICO hist = new LOCACAO_HISTORICO();
                    hist.ASSI_CD_ID = idAss;
                    hist.LOCA_CD_ID = item.LOCA_CD_ID;
                    hist.LOHI_DS_DESCRICAO = "Alteração de Ocorrência da Locação " + loca.LOCA_NM_TITULO;
                    hist.LOHI_DT_HISTORICO = DateTime.Now;
                    hist.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    hist.LOHI_IN_ATIVO = 1;
                    hist.LOHI_NM_OPERACAO = "Alteração de Ocorrência";
                    Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                    // Monta Log
                    DTO_Ocorrencia dto = MontarOcorrenciaDTO(item.LOOC_CD_ID);
                    String json = JsonConvert.SerializeObject(dto);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Locação - Ocorrência - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A ocorrência " + item.LOOC_NM_TITULO.ToUpper() + " da locação " + loca.LOCA_NM_TITULO.ToUpper() + " foi alterada com sucesso";
                    Session["MensLocacao"] = 61;

                    return RedirectToAction("VoltarAnexoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


















        public List<TIPO_CONTRATO> CarregaTipoContrato()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_CONTRATO> conf = new List<TIPO_CONTRATO>();
            if (Session["TipoContratos"] == null)
            {
                conf = baseApp.GetAllTipoContrato(idAss);
            }
            else
            {
                conf = (List<TIPO_CONTRATO>)Session["TipoContratos"];
            }
            Session["TipoContratos"] = conf;
            return conf;
        }


        public List<CONTRATO_LOCACAO> CarregarContratoLocacao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CONTRATO_LOCACAO> conf = new List<CONTRATO_LOCACAO>();
                if (Session["ContratoLocacoes"] == null)
                {
                    conf = baseApp.GetAllContratos(idAss);
                }
                else
                {
                    if ((Int32)Session["ContratoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllContratos(idAss);
                    }
                    else
                    {
                        conf = (List<CONTRATO_LOCACAO>)Session["ContratoLocacoes"];
                    }
                }
                Session["ContratoAlterada"] = 0;
                Session["ContratoLocacoes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["LocacaoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
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
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<LOCACAO_HISTORICO> CarregaHistoricoLocacao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOCACAO_HISTORICO> conf = new List<LOCACAO_HISTORICO>();
                if (Session["LocacoesHistoricos"] == null)
                {
                    conf = baseApp.GetAllHistorico(idAss);
                }
                else
                {
                    if ((Int32)Session["LocacaoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllHistorico(idAss);
                    }
                    else
                    {
                        conf = (List<LOCACAO_HISTORICO>)Session["LocacoesHistoricos"];
                    }
                }
                Session["LocacaoAlterada"] = 0;
                Session["LocacoesHistoricos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PRODUTO> CarregarProduto()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PRODUTO> conf = new List<PRODUTO>();
                if (Session["Produtos"] == null)
                {
                    conf = prodApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ProdutoAlterada"] == 1)
                    {
                        conf = prodApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<PRODUTO>)Session["Produtos"];
                    }
                }
                Session["ProdutoAlterada"] = 0;
                Session["Produtos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PERIODICIDADE_TAREFA> CarregaPeriodicidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PERIODICIDADE_TAREFA> conf = new List<PERIODICIDADE_TAREFA>();
            if (Session["Periodicidades"] == null)
            {
                conf = perApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PeriodicidadeAlterada"] == 1)
                {
                    conf = perApp.GetAllItens();
                }
                else
                {
                    conf = (List<PERIODICIDADE_TAREFA>)Session["Periodicidades"];
                }
            }
            conf = conf.Where(p => p.ASSI_CD_ID == idAss).ToList();
            Session["PeriodicidadeAlterada"] = 0;
            Session["Periodicidades"] = conf;
            return conf;
        }

        public DTO_Locacao MontarLocacaoDTO(Int32 locacaoId)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var locacaoDTO = context.LOCACAO
                    .Where(l => l.LOCA_CD_ID == locacaoId) 
                    .Select(l => new DTO_Locacao
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        LOCA_CD_ID = l.LOCA_CD_ID,
                        PACI_CD_ID = l.PACI_CD_ID,
                        PROD_CD_ID = l.PROD_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        LOCA_DT_INICIO = l.LOCA_DT_INICIO,
                        LOCA_NR_PRAZO = l.LOCA_NR_PRAZO,
                        LOCA_IN_QUANTIDADE = l.LOCA_IN_QUANTIDADE,
                        LOCA_VL_PARCELA = l.LOCA_VL_PARCELA,
                        LOCA_VL_TOTAL = l.LOCA_VL_TOTAL,
                        LOCA_IN_ATIVO = l.LOCA_IN_ATIVO,
                        LOCA_GU_GUID = l.LOCA_GU_GUID,
                        LOCA_IN_STATUS = l.LOCA_IN_STATUS,
                        LOCA_NR_NUMERO = l.LOCA_NR_NUMERO,
                        LOCA_NM_TITULO = l.LOCA_NM_TITULO,
                        LOCA_DS_DESCRICAO = l.LOCA_DS_DESCRICAO,
                        LOCA_NR_DIA = l.LOCA_NR_DIA,
                        LOCA_DT_CANCELAMENTO = l.LOCA_DT_CANCELAMENTO,
                        LOCA_DS_JUSTIFICATIVA = l.LOCA_DS_JUSTIFICATIVA,
                        LOCA_DT_ENCERRAMENTO = l.LOCA_DT_ENCERRAMENTO,
                        LOCA_DT_FINAL = l.LOCA_DT_FINAL,
                        LOCA_DT_RENOVACAO = l.LOCA_DT_RENOVACAO,
                        LOCA_IN_RENOVACOES = l.LOCA_IN_RENOVACOES,
                        LOCA_IN_RENOVACAO = l.LOCA_IN_RENOVACAO,
                        LOCA_NR_SERIE = l.LOCA_NR_SERIE,
                        LOCA_IN_GARANTIA = l.LOCA_IN_GARANTIA,
                        LOCA_DT_GARANTIA = l.LOCA_DT_GARANTIA,
                        LOCA_NR_GARANTIA = l.LOCA_NR_GARANTIA,
                        LOCA_DT_APROVACAO = l.LOCA_DT_APROVACAO,
                        LOCA_IN_CONTRATO = l.LOCA_IN_CONTRATO,
                        LOCA_XM_NOTA_FISCAL = l.LOCA_XM_NOTA_FISCAL,
                        LOCA_TK_TOKEN = l.LOCA_TK_TOKEN,
                        LOCA_AQ_ARQUIVO_QRCODE = l.LOCA_AQ_ARQUIVO_QRCODE,
                        LOCA_DT_EMISSAO = l.LOCA_DT_EMISSAO,
                        LOCA_IN_ASSINADO_DIGITAL = l.LOCA_IN_ASSINADO_DIGITAL,
                    })
                    .FirstOrDefault();
                return locacaoDTO;
            }
        }

        public DTO_Parcela MontarParcelaDTO(Int32 parcId)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var locacaoParcela = context.LOCACAO_PARCELA
                    .Where(l => l.LOPA_CD_ID == parcId)
                    .Select(l => new DTO_Parcela
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        LOCA_CD_ID = l.LOCA_CD_ID,
                        LOPA_CD_ID = l.LOPA_CD_ID,
                        LOPA_DS_DESCRICAO = l.LOPA_DS_DESCRICAO,
                        LOPA_DT_DUMMY = l.LOPA_DT_DUMMY,
                        LOPA_DT_VENCIMENTO = l.LOPA_DT_VENCIMENTO,
                        LOPA_DT_PAGAMENTO = l.LOPA_DT_PAGAMENTO,
                        LOPA_IN_ATIVO = l.LOPA_IN_ATIVO,
                        LOPA_IN_ATRASO = l.LOPA_IN_ATRASO,
                        LOPA_IN_PACIENTE_DUMMY = l.LOPA_IN_PACIENTE_DUMMY,
                        LOPA_IN_PARCELA = l.LOPA_IN_PARCELA,
                        LOPA_IN_QUITADA = l.LOPA_IN_QUITADA,
                        LOPA_IN_STATUS = l.LOPA_IN_STATUS,
                        LOPA_NM_PARCELAS = l.LOPA_NM_PARCELAS,
                        LOPA_NR_PACELAS = l.LOPA_NR_PACELAS,
                        LOPA_VL_DESCONTO = l.LOPA_VL_DESCONTO,
                        LOPA_VL_JUROS = l.LOPA_VL_JUROS,
                        LOPA_VL_TAXAS = l.LOPA_VL_TAXAS,
                        LOPA_VL_VALOR = l.LOPA_VL_VALOR,
                        LOPA_VL_VALOR_PAGO = l.LOPA_VL_VALOR_PAGO,
                    })
                    .FirstOrDefault();
                return locacaoParcela;
            }
        }

        public DTO_Ocorrencia MontarOcorrenciaDTO(Int32 ocor)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var locacaoOcor = context.LOCACAO_OCORRENCIA
                    .Where(l => l.LOOC_CD_ID == ocor)
                    .Select(l => new DTO_Ocorrencia
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        LOCA_CD_ID = l.LOCA_CD_ID,
                        LOOC_CD_ID = l.LOCA_CD_ID,
                        LOOC_DS_DESCRICAO = l.LOOC_DS_DESCRICAO,
                        LOOC_DT_OCORRENCIA = l.LOOC_DT_OCORRENCIA,
                        LOOC_IN_ATIVO = l.LOOC_IN_ATIVO,
                        LOOC_NM_TITULO = l.LOOC_NM_TITULO,
                        LOOC_SERIE_ENTRADA = l.LOOC_SERIE_ENTRADA,
                        LOOC_SERIE_SAIDA = l.LOOC_SERIE_SAIDA,
                        TIOC_CD_ID = l.TIOC_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                    })
                    .FirstOrDefault();
                return locacaoOcor;
            }
        }

        public List<LOCACAO_PARCELA> CarregarParcelas()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOCACAO_PARCELA> conf = new List<LOCACAO_PARCELA>();
                if (Session["LocacoesParcelas"] == null)
                {
                    conf = baseApp.GetAllParcelas(idAss);
                }
                else
                {
                    conf = (List<LOCACAO_PARCELA>)Session["LocacoesParcelas"];
                }
                Session["LocacoesParcelas"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CONSULTA_RECEBIMENTO> CarregaRecebimento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CONSULTA_RECEBIMENTO> conf = new List<CONSULTA_RECEBIMENTO>();
                if (Session["Recebimentos"] == null)
                {
                    conf = recApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["RecebimentoAlterada"] == 1)
                    {
                        conf = recApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CONSULTA_RECEBIMENTO>)Session["Recebimentos"];
                    }
                }
                Session["Recebimentos"] = conf;
                Session["RecebimentoAlterada"] = 0;
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

        public ActionResult MontarTelaContratoLocacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Carrega listas
                if (Session["ListaContrato"] == null)
                {
                    listaMasterContrato = CarregarContratoLocacao().OrderBy(p => p.TICO_CD_ID).ToList();
                    Session["ListaContrato"] = listaMasterContrato;
                }
                ViewBag.Listas = (List<CONTRATO_LOCACAO>)Session["ListaContrato"];
                ViewBag.Tipos = new SelectList(CarregaTipoContrato(), "TICO_CD_ID", "TICO_NM_NOME");

                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 111)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0709", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 112)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0710", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTRATO_LOCACAO", "Locacao", "MontarTelaContratoLocacao");

                // Abre view
                Session["MensLocacao"] = null;
                Session["VoltaContratoLocacao"] = 2;
                Session["NivelLocacao"] = 1;
                objetoContrato = new CONTRATO_LOCACAO();
                return View(objetoContrato);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContratoLocacao()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locacao - Inclusão";
                        return RedirectToAction("MontarTelaLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Contrato de Locacao - Inclusão";
                CONFIGURACAO conf = CarregaConfiguracaoGeral();

                if (Session["MensLocacao"] != null)
                {
                    if ((Int32)Session["MensLocacao"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.Tipos = new SelectList(CarregaTipoContrato(), "TICO_CD_ID", "TICO_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/21/Ajuda21.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTRATO_LOCACAO_INCLUIR", "Locacao", "IncluirContratoLocacao");

                // Prepara view
                Session["MensLocacao"] = null;
                CONTRATO_LOCACAO item = new CONTRATO_LOCACAO();
                ContratoLocacaoViewModel vm = Mapper.Map<CONTRATO_LOCACAO, ContratoLocacaoViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.COLO_IN_ATIVO = 1;
                vm.COLO_DT_CRIACAO = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirContratoLocacao(ContratoLocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipos = new SelectList(CarregaTipoContrato(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.COLO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COLO_NM_NOME);
                    vm.COLO_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COLO_TX_TEXTO);

                    // Critica

                    // Recupera
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();

                    // Preparação
                    CONTRATO_LOCACAO item = Mapper.Map<ContratoLocacaoViewModel, CONTRATO_LOCACAO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Serializa

                    // Processa
                    Int32 volta = baseApp.ValidateCreateContrato(item);
                    Session["IdContrato"] = item.COLO_CD_ID;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0707", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterContrato = new List<CONTRATO_LOCACAO>();
                    Session["ListaContrato"] = null;
                    Session["IdContrato"] = item.COLO_CD_ID;
                    Session["ContratoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O contrato " + item.COLO_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensLocacao"] = 61;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_ContratoLocacao dto = MontarContratoLocacaoDTO(item.COLO_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Contrato Locação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta3 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("MontarTelaContratoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarContratoLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Contrato de Locação - Edição";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Contrato de Locação - Edição";

                // Trata mensagens
                if (Session["MensLocacao"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensLocacao"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara view
                Session["NivelPaciente"] = 17;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/6/Ajuda6_2.pdf";
                Session["MensLocacao"] = null;

                CONTRATO_LOCACAO item = baseApp.GetContratoById(id);
                Session["ContratoAntes"] = item;
                ContratoLocacaoViewModel vm = Mapper.Map<CONTRATO_LOCACAO, ContratoLocacaoViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONTRATO_LOCACAO_EDITAR", "Locacao", "EditarContratoLocacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarContratoLocacao(ContratoLocacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.COLO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COLO_NM_NOME);
                    vm.COLO_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COLO_TX_TEXTO);

                    // Executa a operação
                    CONTRATO_LOCACAO item = Mapper.Map<ContratoLocacaoViewModel, CONTRATO_LOCACAO>(vm);
                    Int32 volta = baseApp.ValidateEditContrato(item);

                    // Verifica retorno
                    Session["IdContrato"] = item.COLO_CD_ID;
                    Session["ContratoAlterada"] = 1;
                    Session["NivelPaciente"] = 17;
                    Session["NivelProduto"] = 13;
                    Session["NivelLocacao"] = 1;
                    Session["ListaContrato"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O contrato " + item.COLO_NM_NOME.ToUpper() + " foi alterado com sucesso";
                    Session["MensLocacao"] = 61;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_ContratoLocacao dto = MontarContratoLocacaoDTO(item.COLO_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    DTO_ContratoLocacao antes = MontarContratoLocacaoDTO(((CONTRATO_LOCACAO)Session["ContratoAntes"]).COLO_CD_ID);
                    String jsonAntes = JsonConvert.SerializeObject(antes, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Contrato de Locação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("MontarTelaContratoLocacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Locacao";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirContratoLocacao(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO__EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Contrato de Locação - Exclusão";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTRATO_LOCACAO item = baseApp.GetContratoById(id);

                // Checa integridade
                if (item.LOCACAO.Count() > 0)
                {
                    Session["MensLocacao"] = 111;
                    return RedirectToAction("MontarTelaContratoLocacao");
                }

                // Verifica a possibilidade de exclusao
                List<CONTRATO_LOCACAO> conts = CarregarContratoLocacao().ToList();
                Int32? quant = conts.Where(p => p.TICO_CD_ID == item.TICO_CD_ID).Count();
                if (quant == 1)
                {
                    Session["MensLocacao"] = 112;
                    return RedirectToAction("MontarTelaContratoLocacao");
                }

                // Processa
                item.COLO_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContrato(item);

                // Estado
                Session["ContratoAlterada"] = 1;
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["NivelLocacao"] = 1;
                Session["ListaContrato"] = null;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_ContratoLocacao dto = MontarContratoLocacaoDTO(item.COLO_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Contrato de Locação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta3 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O contrato de locação " + item.COLO_NM_NOME.ToUpper() + " foi excluído com sucesso";
                Session["MensLocacao"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaContratoLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public DTO_Contrato MontarContratoDTO(Int32 id)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var contrato = context.CONTRATO_LOCACAO
                    .Where(l => l.COLO_CD_ID == id)
                    .Select(l => new DTO_Contrato
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        COLO_CD_ID = l.COLO_CD_ID,
                        COLO_IN_ATIVO = l.COLO_IN_ATIVO,
                        COLO_DT_CRIACAO = l.COLO_DT_CRIACAO,
                        COLO_NM_NOME = l.COLO_NM_NOME,
                        COLO_TX_TEXTO = l.COLO_TX_TEXTO,
                        TICO_CD_ID = l.TICO_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                    })
                    .FirstOrDefault();
                return contrato;
            }
        }

        public DTO_ContratoLocacao MontarContratoLocacaoDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.CONTRATO_LOCACAO
                    .Where(l => l.COLO_CD_ID == mediId)
                    .Select(l => new DTO_ContratoLocacao
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        COLO_DT_CRIACAO = l.COLO_DT_CRIACAO,
                        COLO_IN_ATIVO = l.COLO_IN_ATIVO,
                        COLO_CD_ID = l.COLO_CD_ID,
                        COLO_NM_NOME = l.COLO_NM_NOME,
                        COLO_TX_TEXTO = l.COLO_TX_TEXTO,
                        TICO_CD_ID = l.TICO_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public List<FORMA_RECEBIMENTO> CarregaFormas()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<FORMA_RECEBIMENTO> conf = new List<FORMA_RECEBIMENTO>();
                if (Session["FormaRecebimentos"] == null)
                {
                    conf = recApp.GetAllForma(idAss);
                }
                else
                {
                    if ((Int32)Session["FormaRecebimentoAlterada"] == 1)
                    {
                        conf = recApp.GetAllForma(idAss);
                    }
                    else
                    {
                        conf = (List<FORMA_RECEBIMENTO>)Session["FormaRecebimentos"];
                    }
                }
                Session["FormaRecebimentos"] = conf;
                Session["FormaRecebimentoAlterada"] = 0;
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
        public ActionResult CarregarContrato()
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
                    if (usuario.PERFIL.PERF_IN_LOCACAO__EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Locação - Contrato";
                        return RedirectToAction("VoltarAnexoLocacao");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                // Recupera informações
                LOCACAO item = baseApp.GetItemById((Int32)Session["IdLocacao"] );
                PACIENTE pac = pacApp.GetItemById((Int32)Session["IdPaciente"]);
                ViewBag.NomePaciente = pac.PACI_NM_NOME;

                if (Session["MensLocacao"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensLocacao"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0726", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0727", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensLocacao"] == 9)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0727", CultureInfo.CurrentCulture) + " " + pac.PACI_NM_NOME.ToUpper();
                        ModelState.AddModelError("", frase);
                    }
                }

                // Prepara view
                Session["MensLocacao"] = null;
                Session["LocacaoAntes"] = item;
                LocacaoViewModel vm = Mapper.Map<LOCACAO, LocacaoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileLocacaoContrato(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdLocacao"];
                Int32 idPac = (Int32)Session["IdPaciente"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera dados
                LOCACAO item = baseApp.GetItemById(idNot);
                PACIENTE pac = pacApp.GetItemById(idPac);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensLocacao"] = 5;
                    return RedirectToAction("CarregarContrato");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensLocacao"] = 6;
                    return RedirectToAction("CarregarContrato");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensLocacao"] = 7;
                    return RedirectToAction("CarregarContrato");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName).ToUpper();
                if (extensao != ".PDF") 
                {
                    Session["MensLocacao"] = 8;
                    return RedirectToAction("CarregarContrato");
                }

                // Verifica exatidão do nome
                String nome = "Contrato_Locacao" + pac.PACI_NM_NOME + "_" + item.LOCA_GU_GUID + ".pdf";
                if (fileName.ToUpper() != nome.ToUpper())
                {
                    Session["MensLocacao"] = 9;
                    return RedirectToAction("CarregarContrato");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Locacao/" + item.LOCA_CD_ID.ToString() + "/Assinado/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                // Atualiza locacao
                item.LOCA_IN_CONTRATO_ASSINA = 1;
                Int32 volta = baseApp.ValidateEdit(item, item, usu);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O contrato de locação de " + pac.PACI_NM_NOME.ToUpper() + " foi incluído com sucesso";
                Session["MensLocacao"] = 91;

                // Finaliza
                Session["NivelLocacao"] = 1;
                Session["LocacaoAlterada"] = 1;
                return RedirectToAction("VoltarEditarLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardLocacao()
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
                Session["ModuloAtual"] = "Financeiro";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                List<LOCACAO> locs = new List<LOCACAO>();
                locs = CarregarLocacao().Where(p => p.LOCA_IN_ATIVO == 1).ToList();
                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);          
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                List<LOCACAO_PARCELA> parcs = new List<LOCACAO_PARCELA>();
                parcs = CarregarParcelas().Where(p => p.LOPA_IN_ATIVO == 1).ToList();

                // Carrega widgets
                List<LOCACAO> pendentes = locs.Where(p => p.LOCA_IN_STATUS == 0).ToList();
                List<LOCACAO> ativas = locs.Where(p => p.LOCA_IN_STATUS == 1).ToList();
                List<LOCACAO> canceladas = locs.Where(p => p.LOCA_IN_STATUS == 4).ToList();
                List<LOCACAO> encerradas = locs.Where(p => p.LOCA_IN_STATUS == 2).ToList();
                List<LOCACAO> vencidas = locs.Where(p => p.LOCA_DT_FINAL < DateTime.Today.Date & p.LOCA_IN_STATUS == 1).ToList();

                ViewBag.Pendentes = pendentes.Count();
                ViewBag.Ativas = ativas.Count();
                ViewBag.Canceladas = canceladas.Count();
                ViewBag.Encerradas = encerradas.Count();
                ViewBag.Vencidas = vencidas.Count();
                ViewBag.Total = locs.Count();

                // Locacoes e recebimentos no mês
                List<LOCACAO> locMes = locs.Where(p => p.LOCA_DT_INICIO.Value.Month == DateTime.Today.Date.Month & p.LOCA_DT_INICIO.Value.Year == DateTime.Today.Date.Year).ToList();
                List<LOCACAO_PARCELA> parcMes = parcs.Where(p => p.LOPA_DT_PAGAMENTO != null).ToList();
                parcMes = parcMes.Where(p => p.LOPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Date.Month & p.LOPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Date.Year & p.LOPA_IN_QUITADA == 1).ToList();

                ViewBag.LocacoesMes = locMes;
                ViewBag.RecebimentosMes = parcMes;

                // Locacoes por data - Mes corrente
                List<DateTime> datas = locs.Where(p => p.LOCA_DT_INICIO.Value.Month == DateTime.Today.Month & p.LOCA_DT_INICIO.Value.Year == DateTime.Today.Year).Select(p => p.LOCA_DT_INICIO.Value.Date).Distinct().ToList();
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaLocacaoData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = locs.Where(p => p.LOCA_DT_INICIO.Value.Date == item.Date).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaLocacaoData = lista;
                    Session["ListaLocacaoData"] = lista;
                }
                else
                {
                    ViewBag.ListaLocacaoData = (List<ModeloViewModel>)Session["ListaLocacaoData"];
                }

                // Recebimentos por data - Mes corrente
                datas = parcMes.Where(p => p.LOPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Month & p.LOPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Year & p.LOPA_IN_QUITADA == 1).Select(p => p.LOPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaParcelaData"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Decimal? conta = parcMes.Where(p => p.LOPA_DT_PAGAMENTO.Value.Date == item.Date & p.LOPA_IN_QUITADA == 1).Sum(p => p.LOPA_VL_VALOR_PAGO);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.ValorDec = conta.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaParcelaData = lista;
                    Session["ListaParcelaData"] = lista;
                }
                else
                {
                    ViewBag.ListaParcelaData = (List<ModeloViewModel>)Session["ListaParcelaData"];
                }

                // Resumo Mensal Locacoes
                List<DateTime> datasLocacao = locs.Where(p => p.LOCA_DT_INICIO != null).Select(p => p.LOCA_DT_INICIO.Value.Date).Distinct().ToList();
                datasLocacao.Sort((i, j) => i.Date.CompareTo(j.Date));
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaLocacaoMes"] == null)
                {
                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    String mes2 = null;
                    String mesFeito2 = null;
                    foreach (DateTime item in datasLocacao)
                    {
                        if (item.Date > limite)
                        {
                            mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes2 != mesFeito2)
                            {
                                Int32 conta = locs.Where(p => p.LOCA_DT_INICIO.Value.Date.Month == item.Month & p.LOCA_DT_INICIO.Value.Date.Year == item.Year & p.LOCA_DT_INICIO > limite).Count();
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
                    ViewBag.ListaLocacaoMes = listaMes;
                    Session["ListaLocacaoMes"] = listaMes;
                }
                else
                {
                    ViewBag.ListaLocacaoMes = (List<ModeloViewModel>)Session["ListaLocacaoMes"];
                }

                // Resumo Mensal Recebimentos
                List<DateTime> datasRec = parcs.Where(p => p.LOPA_DT_PAGAMENTO != null).Select(p => p.LOPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                datasRec.Sort((i, j) => i.Date.CompareTo(j.Date));
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaParcelaMes"] == null)
                {
                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    String mes2 = null;
                    String mesFeito2 = null;
                    foreach (DateTime item in datasRec)
                    {
                        if (item.Date > limite)
                        {
                            mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes2 != mesFeito2)
                            {
                                List<LOCACAO_PARCELA> pc = parcs.Where(p => p.LOPA_DT_PAGAMENTO != null).ToList();
                                Decimal? conta = pc.Where(p => p.LOPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.LOPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.LOPA_DT_PAGAMENTO > limite & p.LOPA_IN_QUITADA == 1).Sum(p => p.LOPA_VL_VALOR_PAGO);
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes2;
                                mod.ValorDec = conta.Value;
                                listaMes.Add(mod);
                                mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                            }
                        }
                    }

                    mes2 = null;
                    mesFeito2 = null;
                    ViewBag.ListaParcelaMes = listaMes;
                    Session["ListaParcelaMes"] = listaMes;
                }
                else
                {
                    ViewBag.ListaParcelaMes = (List<ModeloViewModel>)Session["ListaParcelaMes"];
                }

                // Recupera Locacao por Status
                if (Session["ListaLocacaoStatus"] == null)
                {
                    List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
                    for (int i = 0; i < 5; i++)
                    {
                        Int32 num = locs.Where(p => p.LOCA_IN_STATUS == i).ToList().Count;
                        if (num > 0)
                        {
                            String nome = String.Empty;
                            if (i == 0)
                            {
                                nome = "Pendente";
                            }
                            else if(i == 1)
                            {
                                nome = "Ativa";
                            }
                            else if (i == 2)
                            {
                                nome = "Encerrada";
                            }
                            else if (i == 3)
                            {
                                nome = "Vencida";
                            }
                            else if (i == 4)
                            {
                                nome = "Cancelada   ";
                            }

                            ModeloViewModel mod3 = new ModeloViewModel();
                            mod3.Nome = nome;
                            mod3.Valor = num;
                            lista9.Add(mod3);
                        }
                    }
                    ViewBag.ListaLocacaoStatus = lista9;
                    Session["ListaLocacaoStatus"] = lista9;
                }
                else
                {
                    ViewBag.ListaLocacaoStatus = (List<ModeloViewModel>)Session["ListaLocacaoStatus"];
                }

                // Locacoes por Produto
                List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
                if (Session["ListaLocacaoProduto"] == null)
                {
                    List<PRODUTO> prodx = CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 2 & p.PROD_IN_LOCACAO == 1).ToList();
                    foreach (PRODUTO item in prodx)
                    {
                        Int32 num = locs.Where(p => p.PROD_CD_ID == item.PROD_CD_ID & p.LOCA_IN_STATUS == 1).ToList().Count;
                        if (num > 0)
                        {
                            ModeloViewModel mod1 = new ModeloViewModel();
                            mod1.Valor1 = item.PROD_CD_ID;
                            mod1.Nome = item.PROD_NM_NOME;
                            mod1.Valor = num;
                            lista7.Add(mod1);
                        }
                    }
                    ViewBag.ListaLocacaoProduto = lista7;
                    Session["ListaLocacaoProduto"] = lista7;
                }
                else
                {
                    ViewBag.ListaLocacaoProduto = (List<ModeloViewModel>)Session["ListaLocacaoProduto"];
                }

                // Locacoes por paciente
                List<ModeloViewModel> lista12 = new List<ModeloViewModel>();
                if (Session["ListaLocacaoPaciente"] == null)
                {
                    List<PACIENTE> pacs = CarregaPaciente().Where(p => p.PACI_IN_ATIVO == 1).ToList();
                    foreach (PACIENTE item in pacs)
                    {
                        Int32 num = locs.Where(p => p.PACI_CD_ID == item.PACI__CD_ID & p.LOCA_IN_STATUS == 1).ToList().Count;
                        if (num > 0)
                        {
                            ModeloViewModel mod1 = new ModeloViewModel();
                            mod1.Nome = item.PACI_NM_NOME;
                            mod1.Valor = num;
                            lista12.Add(mod1);
                        }
                    }
                    ViewBag.ListaLocacaoPaciente = lista12;
                    Session["ListaLocacaoPaciente"] = lista12;
                }
                else
                {
                    ViewBag.ListaLocacaoPaciente = (List<ModeloViewModel>)Session["ListaLocacaoPaciente"];
                }

                // Locacoes vencidas e não encerradas
                List<ModeloViewModel> lista13 = new List<ModeloViewModel>();
                if (Session["ListaLocacaoVencida"] == null)
                {
                    foreach (LOCACAO item in locs)
                    {
                        if (item.LOCA_DT_FINAL < DateTime.Today.Date & item.LOCA_IN_STATUS == 1)
                        {
                            ModeloViewModel mod1 = new ModeloViewModel();
                            mod1.Nome = item.LOCA_NM_TITULO;
                            mod1.DataEmissao = item.LOCA_DT_FINAL.Value;
                            lista13.Add(mod1);
                        }
                    }
                    ViewBag.ListaLocacaoVencida = lista13;
                    Session["ListaLocacaoVencida"] = lista13;
                }
                else
                {
                    ViewBag.ListaLocacaoVencida = (List<ModeloViewModel>)Session["ListaLocacaoVencida"];
                }

                // Recebimento por produto
                List<Int32> prods = locs.Where(p => p.LOCA_IN_ATIVO == 1).Select(p => p.PROD_CD_ID).Distinct().ToList();
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaRecebeProduto"] == null)
                {
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    Decimal? soma = 0;
                    foreach (Int32 item in prods)
                    {
                        List<LOCACAO> locPro = locs.Where(p => p.PROD_CD_ID == item).ToList();
                        foreach (LOCACAO locItem in locPro)
                        {
                            Decimal? valRec = locItem.LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 1).Sum(p => p.LOPA_VL_VALOR_PAGO);
                            soma += valRec;
                        }

                        if (soma > 0)
                        {
                            PRODUTO pro = prodApp.GetItemById(item);
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.Valor = item;
                            mod.Nome = pro.PROD_NM_NOME;
                            mod.ValorDec = soma.Value;
                            lista.Add(mod);
                        }
                        soma = 0;
                    }

                    ViewBag.ListaRecebeProduto = lista;
                    Session["ListaRecebeProduto"] = lista;
                }
                else
                {
                    ViewBag.ListaRecebeProduto = (List<ModeloViewModel>)Session["ListaRecebeProduto"];
                }

                // Parcelas em atraso por numero de dias
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaParcelaAtraso"] == null)
                {
                    List<LOCACAO_PARCELA> atraso = parcs.Where(p => p.LOPA_IN_QUITADA == 0 & p.LOPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date).ToList();
                    List<ModeloViewModel> listaAtraso = new List<ModeloViewModel>();
                    Decimal? valor = 0;
                    Int32 diasDeAtraso = 0;
                    Int32 atraso10 = 0;
                    Int32 atraso30 = 0;
                    Int32 atraso60 = 0;
                    Int32 atraso00 = 0;
                    Decimal? atrasoVal10 = 0;
                    Decimal? atrasoVal30 = 0;
                    Decimal? atrasoVal60 = 0;
                    Decimal? atrasoVal00 = 0;

                    DateTime hoje = DateTime.Today.Date;
                    foreach (LOCACAO_PARCELA item in atraso)
                    {
                        TimeSpan diferenca = hoje - item.LOPA_DT_VENCIMENTO.Value.Date;
                        diasDeAtraso = diferenca.Days;
                        if (diasDeAtraso <= 10)
                        {
                            atraso10++;
                            atrasoVal10 += item.LOPA_VL_VALOR;
                        }
                        else if (diasDeAtraso > 10 & diasDeAtraso <= 30)
                        {
                            atraso30++;
                            atrasoVal30 += item.LOPA_VL_VALOR;
                        }
                        else if (diasDeAtraso > 30 & diasDeAtraso <= 60)
                        {
                            atraso60++;
                            atrasoVal60 += item.LOPA_VL_VALOR;
                        }
                        else if (diasDeAtraso > 60)
                        {
                            atraso00++;
                            atrasoVal00 += item.LOPA_VL_VALOR;
                        }
                    }

                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = "Até 10 dias de atraso";
                    mod.Valor = atraso10;
                    mod.ValorDec = atrasoVal10.Value;
                    listaAtraso.Add(mod);

                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = "Entre 10 e 30 dias de atraso";
                    mod1.Valor = atraso30;
                    mod1.ValorDec = atrasoVal30.Value;
                    listaAtraso.Add(mod1);

                    ModeloViewModel mod2 = new ModeloViewModel();
                    mod2.Nome = "Entre 30 e 60 dias de atraso";
                    mod2.Valor = atraso60;
                    mod2.ValorDec = atrasoVal60.Value;
                    listaAtraso.Add(mod2);

                    ModeloViewModel mod3 = new ModeloViewModel();
                    mod3.Nome = "Mais que 60 dias de atraso";
                    mod3.Valor = atraso00;
                    mod3.ValorDec = atrasoVal00.Value;
                    listaAtraso.Add(mod3);

                    ViewBag.ListaParcelaAtraso = listaAtraso;
                    Session["ListaParcelaAtraso"] = listaAtraso;
                }
                else
                {
                    ViewBag.ListaParcelaAtraso = (List<ModeloViewModel>)Session["ListaParcelaAtraso"];
                }

                // Locações encerrando por numero de dias
                if ((Int32)Session["LocacaoAlterada"] == 1 || Session["ListaLocacaoEncerra"] == null)
                {
                    List<LOCACAO> encerra = locs.Where(p => p.LOCA_IN_STATUS == 1 & p.LOCA_DT_FINAL.Value.Date < DateTime.Today.Date.AddDays(60)).ToList();
                    List<ModeloViewModel> listaEncerra = new List<ModeloViewModel>();
                    DateTime hoje = DateTime.Today.Date;
                    Int32 diasParaVencer = 0;
                    Int32 venc10 = 0;
                    Int32 venc30 = 0;
                    Int32 venc60 = 0;

                    foreach (LOCACAO item in encerra)
                    {
                        TimeSpan diferenca = item.LOCA_DT_FINAL.Value.Date - hoje;
                        diasParaVencer = diferenca.Days;
                        if (diasParaVencer <= 10)
                        {
                            venc10++;
                        }
                        else if (diasParaVencer > 10 & diasParaVencer <= 30)
                        {
                            venc30++;
                        }
                        else if (diasParaVencer > 30 & diasParaVencer <= 60)
                        {
                            venc60++;
                        }
                    }

                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = "Vencendo em até 10 dias";
                    mod.Valor = venc10;
                    listaEncerra.Add(mod);
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = "Vencendo em até 30 dias";
                    mod1.Valor = venc30;
                    listaEncerra.Add(mod1);
                    ModeloViewModel mod2 = new ModeloViewModel();
                    mod2.Nome = "Vencendo em até 60 dias";
                    mod2.Valor = venc60;
                    listaEncerra.Add(mod2);

                    ViewBag.ListaLocacaoEncerra = listaEncerra;
                    Session["ListaLocacaoEncerra"] = listaEncerra;

                }
                else
                {
                    ViewBag.ListaLocacaoEncerra = (List<ModeloViewModel>)Session["ListaLocacaoEncerra"];
                }

                // Acerta estado    
                Session["LocacaoAlterada"] = 1;
                Session["NivelPaciente"] = 17;
                Session["NivelProduto"] = 13;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20.pdf";

                // Carrega view
                objeto = new LOCACAO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOCACAO_DASHBOARD", "Locacao", "MontarTelaDashboardLocacao");
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult ProcessaRelatorioLocacao(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarRelatorioListaLocacaoForm");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarListagemLocacaoData");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarListagemLocacaoMes");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarListagemRecebimentoData");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarListagemRecebimentoMes");
            }
            if (tipoRel == 6)
            {
                return RedirectToAction("GerarListagemParcelaAtraso");
            }
            if (tipoRel == 7)
            {
                return RedirectToAction("GerarListagemPacienteLocacao");
            }
            if (tipoRel == 8)
            {
                return RedirectToAction("GerarListagemProdutoLocacao");
            }
            if (tipoRel == 9)
            {
                return RedirectToAction("GerarListagemLocacaoStatus");
            }
            if (tipoRel == 10)
            {
                return RedirectToAction("GerarListagemRecebimentoProduto");
            }
            if (tipoRel == 11)
            {
                return RedirectToAction("GerarListagemLocacaoPendente");
            }
            if (tipoRel == 12)
            {
                return RedirectToAction("GerarListagemLocacaoAtiva");
            }
            if (tipoRel == 13)
            {
                return RedirectToAction("GerarListagemLocacaoCancelada");
            }
            if (tipoRel == 14)
            {
                return RedirectToAction("GerarListagemLocacaoEncerrada");
            }
            if (tipoRel == 15)
            {
                return RedirectToAction("GerarListagemLocacaoVencida");
            }
            if (tipoRel == 16)
            {
                return RedirectToAction("GerarListagemLocacaoEncerrar");
            }
            return RedirectToAction("MontarTelaLocacao");
        }

        public JsonResult GetDadosLocacaoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLocacaoData"];
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

        public JsonResult GetDadosRecebimentoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaParcelaData"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.ValorDec);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosLocacaoMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLocacaoMes"];
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

        public JsonResult GetDadosRecebimentoMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaParcelaMes"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.Nome);
                valor.Add(item.ValorDec);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosLocacaoStatus()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLocacaoStatus"];
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

        public JsonResult GetDadosProdutoLocacao()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLocacaoProduto"];
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

        public ActionResult GerarListagemLocacaoData()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;

                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);
                mes = mes + " de " + DateTime.Today.Date.Year.ToString();

                List<LOCACAO> lista = new List<LOCACAO>();
                nomeRel = "LocacaoData" + "_" + data + ".pdf";
                titulo = "Locações por Data - " + mes;
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                // Monta lista
                lista = CarregarLocacao().Where(p => p.LOCA_DT_INICIO.Value.Month == DateTime.Today.Month & p.LOCA_DT_INICIO.Value.Year == DateTime.Today.Year).ToList();
                lista = lista.OrderBy(p => p.LOCA_DT_INICIO).ToList();

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
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph(titulo, meuFont2))
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

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 70f, 70f, 150f, 180f, 80f, 70f, 70f, 70f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Início", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Final", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Parcela (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Dia de Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Status", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (LOCACAO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.LOCA_DT_INICIO.Value.ToShortDateString(), meuFontBold))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_DT_FINAL.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_IN_QUANTIDADE.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.LOCA_VL_PARCELA.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.LOCA_NR_DIA.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.LOCA_IN_STATUS == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Ativa", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LOCA_IN_STATUS == 0)
                    {
                        cell = new PdfPCell(new Paragraph("Pendente", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    else if (item.LOCA_IN_STATUS == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Encerrada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LOCA_IN_STATUS == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Atrasada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.LOCA_IN_STATUS == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Cancelada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    if (System.IO.File.Exists(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        Image image = Image.GetInstance(Server.MapPath(item.PRODUTO.PROD_AQ_FOTO));
                        image.ScaleAbsolute(20, 20);
                        cell.AddElement(image);
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(cell);
                    }
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

                // Retorno
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "CRMsys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}