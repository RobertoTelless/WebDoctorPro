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
using System.Xml.Linq;
using iText.Signatures;
using Microsoft.Extensions.Azure;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;


namespace GEDSys_Presentation.Controllers
{
    public class FinanceiroController : Controller
    {
        private readonly IPacienteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IValorConsultaAppService vcApp;
        private readonly IValorServicoAppService vsApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IValorConvenioAppService vvApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ITipoValorConsultaAppService tvcApp;
        private readonly IPagamentoAppService pagApp;
        private readonly ITipoPagamentoAppService tpApp;
        private readonly IEmpresaAppService empApp;
        private readonly IRecebimentoAppService recApp;
        private readonly IPacienteAppService pacApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IProdutoAppService prodApp;

        private VALOR_CONSULTA objetoVC = new VALOR_CONSULTA();
        private VALOR_CONSULTA objetoAntesVC = new VALOR_CONSULTA();
        private List<VALOR_CONSULTA> listaMasterVC = new List<VALOR_CONSULTA>();
        private VALOR_SERVICO objetoVS = new VALOR_SERVICO();
        private VALOR_SERVICO objetoAntesVS = new VALOR_SERVICO();
        private List<VALOR_SERVICO> listaMasterVS = new List<VALOR_SERVICO>();
        private VALOR_CONVENIO objetoVV = new VALOR_CONVENIO();
        private VALOR_CONVENIO objetoAntesVV = new VALOR_CONVENIO();
        private List<VALOR_CONVENIO> listaMasterVV = new List<VALOR_CONVENIO>();
        private CONSULTA_PAGAMENTO objetoPag = new CONSULTA_PAGAMENTO();
        private CONSULTA_PAGAMENTO objetoAntesPag = new CONSULTA_PAGAMENTO();
        private List<CONSULTA_PAGAMENTO> listaMasterPag = new List<CONSULTA_PAGAMENTO>();
        private CONSULTA_RECEBIMENTO objetoRec = new CONSULTA_RECEBIMENTO();
        private CONSULTA_RECEBIMENTO objetoAntesREc = new CONSULTA_RECEBIMENTO();
        private List<CONSULTA_RECEBIMENTO> listaMasterRec = new List<CONSULTA_RECEBIMENTO>();
        private List<PACIENTE_CONSULTA> listaMasterPC = new List<PACIENTE_CONSULTA>();
        private PACIENTE_CONSULTA objetoPC = new PACIENTE_CONSULTA();
        private String extensao;

        public FinanceiroController(IPacienteAppService baseApps, ILogAppService logApps, IValorConsultaAppService vcApps, IValorServicoAppService vsApps, IConfiguracaoAppService confApps, IValorConvenioAppService vvApps, IUsuarioAppService usuApps, ITipoValorConsultaAppService tvcApps, IPagamentoAppService pagApps, ITipoPagamentoAppService tpApps, IEmpresaAppService empApps, IRecebimentoAppService recApps, IPacienteAppService pacApps, IAcessoMetodoAppService aceApps, IPeriodicidadeAppService perApps, IProdutoAppService prodApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            vcApp = vcApps;
            vsApp = vsApps;
            confApp = confApps;
            vvApp = vvApps;
            usuApp = usuApps;
            tvcApp = tvcApps;
            pagApp = pagApps;
            tpApp = tpApps;
            empApp = empApps;
            recApp = recApps;
            pacApp = pacApps;
            aceApp = aceApps;
            perApp = perApps;
            prodApp = prodApps;
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
        public ActionResult MontarTelaValorConsulta()
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
                        Session["ModuloPermissao"] = "Financeiro";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Consultas - Valores";

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (Session["ListaValorConsulta"] == null)
                {
                    if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                    {
                        listaMasterVC = CarregaValorConsulta().Where(p => p.VACO_IN_ATIVO == 1).ToList();
                    }
                    else
                    {
                        listaMasterVC = CarregaValorConsulta().Where(p => p.VACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaValorConsulta"] = listaMasterVC;
                }

                // Monta demais listas
                ViewBag.Listas = (List<VALOR_CONSULTA>)Session["ListaValorConsulta"];
                ViewBag.Tipos = new SelectList(CarregaTipoValorConsulta(), "TIVL_CD_ID", "TIVL_NM_TIPO");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0582", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "VALOR_CONSULTA", "Financeiro", "MontarTelaValorConsulta");

                // Acerta estado
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 3;
                Session["ModoConsulta"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["VoltaMail"] = 2;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/17/Ajuda17.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                // Carrega view
                objetoVC = new VALOR_CONSULTA();
                return View(objetoVC);
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
        public ActionResult IncluirValorConsulta()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Inclusão";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Consultas - Valores - Inclusão";

                // Prepara listas
                var padrao = new List<SelectListItem>();
                padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
                ViewBag.TipoConsulta = new SelectList(CarregaTipoValorConsulta(), "TIVL_CD_ID", "TIVL_NM_TIPO");
                var mat = new List<SelectListItem>();
                mat.Add(new SelectListItem() { Text = "Não", Value = "0" });
                mat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Material = new SelectList(mat, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/17/Ajuda17_1.pdf";
                Session["NivelPaciente"] = 1;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 55)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0584", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0596", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "VALOR_CONSULTA_INCLUIR", "Financeiro", "IncluirValorConsulta");

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONSULTA item = new VALOR_CONSULTA();
                ValorConsulta1ViewModel vm = Mapper.Map<VALOR_CONSULTA, ValorConsulta1ViewModel>(item);
                vm.VACO_IN_ATIVO = 1;
                vm.VACO_DT_REFERENCIA = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.VACO_NR_VALOR = 0;
                vm.VACO_NR_DESCONTO = 0;
                vm.VACO_IN_PADRAO = 0;
                vm.VACO_IN_MATERIAL = 0;
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
        public ActionResult IncluirValorConsulta(ValorConsulta1ViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.TipoConsulta = new SelectList(CarregaTipoValorConsulta(), "TIVL_CD_ID", "TIVL_NM_TIPO");
            var padrao = new List<SelectListItem>();
            padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
            var mat = new List<SelectListItem>();
            mat.Add(new SelectListItem() { Text = "Não", Value = "0" });
            mat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Material = new SelectList(mat, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.VACO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.VACO_NM_NOME);

                    // Criticas
                    if (vm.VACO_DT_REFERENCIA == null)
                    {
                        vm.VACO_DT_REFERENCIA = DateTime.Today.Date.AddMonths(6);
                    }
                    if (vm.VACO_DT_REFERENCIA.Value.Date > DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.VACO_NM_NOME == String.Empty || vm.VACO_NM_NOME == null)
                    {
                        TIPO_VALOR_CONSULTA tvc = tvcApp.GetItemById(vm.TIVL_CD_ID.Value);
                        vm.VACO_NM_NOME = tvc.TIVL_NM_TIPO;
                    }
                    if (vm.VACO_NR_DESCONTO == null)
                    {
                        vm.VACO_NR_DESCONTO = 0;
                    }
                    if (vm.VACO_NR_DESCONTO > vm.VACO_NR_VALOR)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0596", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.VACO_NR_VALOR == null || vm.VACO_NR_VALOR == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0722", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    VALOR_CONSULTA item = Mapper.Map<ValorConsulta1ViewModel, VALOR_CONSULTA>(vm);
                    Int32 volta = vcApp.ValidateCreate(item, usuario);
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0577", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0672", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica retorno
                    Session["IdValorConsulta"] = item.VACO_CD_ID;
                    Session["ValorConsultaAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaValorConsulta"] = null;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                    DTO_ValorConsulta dto = MontarValorConsultaDTO(vc.VACO_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Valor de Consulta - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O tipo de consulta " + item.VACO_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    if (vc.VACO_IN_MATERIAL == 1)
                    {
                        return RedirectToAction("EditarValorConsulta", new { id = (Int32)Session["IdValorConsulta"] });
                    }
                    return RedirectToAction("MontarTelaValorConsulta");
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
        public ActionResult EditarValorConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Edição";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Consultas - Valores - Edição";

                // Prepara Listas
                var padrao = new List<SelectListItem>();
                padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
                var mat = new List<SelectListItem>();
                mat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                mat.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Material = new SelectList(mat, "Value", "Text");
                ViewBag.TipoConsulta = new SelectList(CarregaTipoValorConsulta(), "TIVL_CD_ID", "TIVL_NM_TIPO");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/17/Ajuda17_2.pdf";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "VALOR_CONSULTA_EDITAR", "Financeiro", "EditarValorConsulta");

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONSULTA item = vcApp.GetItemById(id);
                Session["IdValorConsulta"] = item.VACO_CD_ID;
                ViewBag.Lista = item.VALOR_CONSULTA_MATERIAL.Where(p => p.VCMA_IN_ATIVO == 1).ToList();
                ValorConsulta1ViewModel vm = Mapper.Map<VALOR_CONSULTA, ValorConsulta1ViewModel>(item);
                Session["ValorConsultaAntes"] = item;
                vm.VACO_DT_REFERENCIA = DateTime.Today.Date;
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
        public ActionResult EditarValorConsulta(ValorConsulta1ViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.TipoConsulta = new SelectList(CarregaTipoValorConsulta(), "TIVL_CD_ID", "TIVL_NM_TIPO");
            var padrao = new List<SelectListItem>();
            padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
            var mat = new List<SelectListItem>();
            mat.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            mat.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Material = new SelectList(mat, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.VACO_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.VACO_NM_NOME);

                    // Criticas
                    if (vm.VACO_NM_NOME == String.Empty || vm.VACO_NM_NOME == null)
                    {
                        TIPO_VALOR_CONSULTA tvc = tvcApp.GetItemById(vm.TIVL_CD_ID.Value);
                        vm.VACO_NM_NOME = tvc.TIVL_NM_TIPO;
                    }
                    if (vm.VACO_NR_DESCONTO == null)
                    {
                        vm.VACO_NR_DESCONTO = 0;
                    }
                    if (vm.VACO_NR_VALOR == null || vm.VACO_NR_VALOR == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0722", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    VALOR_CONSULTA item = Mapper.Map<ValorConsulta1ViewModel, VALOR_CONSULTA>(vm);
                    Int32 volta = vcApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0672", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    Session["IdValorConsulta"] = item.VACO_CD_ID;
                    Session["ValorConsultaAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaValorConsulta"] = null;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                    VALOR_CONSULTA vcAntes = (VALOR_CONSULTA)Session["ValorConsultaAntes"];
                    DTO_ValorConsulta dto = MontarValorConsultaDTO(vc.VACO_CD_ID);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    DTO_ValorConsulta antes = MontarValorConsultaDTO(vcAntes.VACO_CD_ID);
                    String jsonAntes = JsonConvert.SerializeObject(antes, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Valor de Consulta - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O tipo de consulta " + item.VACO_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaValorConsulta");
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
        public ActionResult ExcluirValorConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Exclusão";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                VALOR_CONSULTA item = vcApp.GetItemById(id);
                item.VACO_IN_ATIVO = 0;
                Int32 volta = vcApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPaciente"] = 4;
                    return RedirectToAction("MontarTelaValorConsulta", "Financeiro");
                }

                Session["ValorConsultaAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaValorConsulta"] = null;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                DTO_ValorConsulta dto = MontarValorConsultaDTO(vc.VACO_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Valor de Consulta - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O tipo de consulta " + item.VACO_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensPaciente"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaValorConsulta");
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
        public ActionResult MontarTelaValorConvenio()
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
                        Session["ModuloPermissao"] = "Financeiro";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (Session["ListaValorConvenio"] == null)
                {
                    listaMasterVV = CarregaValorConvenio().Where(p => p.VACV_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Session["ListaValorConvenio"] = listaMasterVV;
                }

                // Monta demais listas
                ViewBag.Listas = (List<VALOR_CONVENIO>)Session["ListaValorConvenio"];
                ViewBag.Convenio = new SelectList(CarregaConvenio(), "CONV_CD_ID", "CONV_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0583", CultureInfo.CurrentCulture));
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
                Session["VoltaMail"] = 2;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                // Carrega view
                objetoVV = new VALOR_CONVENIO();
                return View(objetoVV);
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
        public ActionResult IncluirValorConvenio()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Convenio - Inclusão";
                        return RedirectToAction("MontarTelaValorConvenio");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara listas
                ViewBag.Convenio = new SelectList(CarregaConvenio(), "CONV_CD_ID", "CONV_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_1.pdf";
                Session["NivelPaciente"] = 1;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 55)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0585", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONVENIO item = new VALOR_CONVENIO();
                ValorConvenioViewModel vm = Mapper.Map<VALOR_CONVENIO, ValorConvenioViewModel>(item);
                vm.VACV_IN_ATIVO = 1;
                vm.VACV_DT_REFERENCIA = DateTime.Today.Date;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.VACV_NR_VALOR = 0;
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
        public ActionResult IncluirValorConvenio(ValorConvenioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Convenio = new SelectList(CarregaConvenio(), "CONV_CD_ID", "CONV_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização

                    // Criticas
                    if (vm.VACV_DT_REFERENCIA.Value.Date > DateTime.Today.Date)
                    {
                        Session["MensPaciente"] = 50;
                        return View(vm);
                    }

                    // Executa a operação
                    VALOR_CONVENIO item = Mapper.Map<ValorConvenioViewModel, VALOR_CONVENIO>(vm);
                    Int32 volta = vvApp.ValidateCreate(item, usuario);
                    if (volta == 1)
                    {
                        Session["MensPaciente"] = 55;
                        return RedirectToAction("IncluirValorConvenio");
                    }

                    // Verifica retorno
                    Session["IdValorConvenio"] = item.VACV_CD_ID;
                    Session["ValorConvenioAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaValorConvenio"] = null;

                    // Monta Log
                    VALOR_CONVENIO vc = vvApp.GetItemById(item.VACV_CD_ID);
                    String frase = vc.VACV_CD_ID.ToString() + "|" + vc.CONV_CD_ID.ToString() + "|" + vc.USUA_CD_ID.ToString() + "|" + vc.VACV_DT_REFERENCIA.ToString() + "|" + vc.VACV_NR_VALOR.ToString();
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ivvVACV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = frase,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O valor de convênio " + vc.CONVENIO.CONV_NM_NOME + " foi incluído com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaValorConvenio");
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
        public ActionResult EditarValorConvenio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Convenio - Edição";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara Listas
                ViewBag.Convenio = new SelectList(CarregaConvenio(), "CONV_CD_ID", "CONV_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_2.pdf";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONVENIO item = vvApp.GetItemById(id);
                Session["IdValorConvenio"] = item.VACV_CD_ID;
                ValorConvenioViewModel vm = Mapper.Map<VALOR_CONVENIO, ValorConvenioViewModel>(item);
                Session["ValorConvenio"] = item;
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
        public ActionResult EditarValorConvenio(ValorConvenioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Convenio = new SelectList(CarregaConvenio(), "CONV_CD_ID", "CONV_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização

                    // Criticas
                    if (vm.VACV_DT_REFERENCIA.Value.Date > DateTime.Today.Date)
                    {
                        Session["MensPaciente"] = 50;
                        return View(vm);
                    }

                    // Executa a operação
                    VALOR_CONVENIO item = Mapper.Map<ValorConvenioViewModel, VALOR_CONVENIO>(vm);
                    Int32 volta = vvApp.ValidateEdit(item, item, usuario);

                    // Verifica retorno
                    Session["IdValorConvenio"] = item.VACV_CD_ID;
                    Session["ValorConvenioAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaValorConvenio"] = null;

                    // Monta Log
                    VALOR_CONVENIO vc = vvApp.GetItemById(item.VACV_CD_ID);
                    String frase = vc.VACV_CD_ID.ToString() + "|" + vc.CONV_CD_ID.ToString() + "|" + vc.USUA_CD_ID.ToString() + "|" + vc.VACV_DT_REFERENCIA.ToString() + "|" + vc.CONVENIO.CONV_NM_NOME + "|" + vc.VACV_NR_VALOR.ToString();
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "evcVACV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = frase,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O valor de convênio " + item.CONVENIO.CONV_NM_NOME + " foi alterado com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("EditarRecebimento", new { id = (Int32)Session["IdRecebimento"] });
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
        public ActionResult ExcluirValorConvenio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Convenio - Exclusão";
                        return RedirectToAction("MontarTelaValorConvenio");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                VALOR_CONVENIO item = vvApp.GetItemById(id);
                item.VACV_IN_ATIVO = 0;
                Int32 volta = vvApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPaciente"] = 4;
                    return RedirectToAction("MontarTelaValorConvenio", "Financeiro");
                }

                Session["ValorConvenioAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaValorConvenio"] = null;

                // Monta Log
                VALOR_CONVENIO vc = vvApp.GetItemById(item.VACV_CD_ID);
                String frase = vc.VACV_CD_ID.ToString() + "|" + vc.CONV_CD_ID.ToString() + "|" + vc.USUA_CD_ID.ToString() + "|" + vc.VACV_DT_REFERENCIA.ToString() + "|" + vc.CONVENIO.CONV_NM_NOME + "|" + vc.VACV_NR_VALOR.ToString();
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "evcVACV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = frase,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O valor de convenio " + vc.CONVENIO.CONV_NM_NOME + " foi excluído com sucesso.";
                Session["MensPaciente"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaValorConvenio");
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

        public List<VALOR_CONSULTA> CarregaValorConsulta()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<VALOR_CONSULTA> conf = new List<VALOR_CONSULTA>();
                if (Session["ValorConsultas"] == null)
                {
                    conf = vcApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ValorConsultaAlterada"] == 1)
                    {
                        conf = vcApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<VALOR_CONSULTA>)Session["ValorConsultas"];
                    }
                }
                Session["ValorConsultas"] = conf;
                Session["ValorConsultaAlterada"] = 0;
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

        public List<CONSULTA_PAGAMENTO> CarregaPagamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CONSULTA_PAGAMENTO> conf = new List<CONSULTA_PAGAMENTO>();
                if (Session["Pagamentos"] == null)
                {
                    conf = pagApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["PagamentoAlterada"] == 1)
                    {
                        conf = pagApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<CONSULTA_PAGAMENTO>)Session["Pagamentos"];
                    }
                }
                Session["Pagamentos"] = conf;
                Session["PagamentoAlterada"] = 0;
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

        public List<TIPO_VALOR_CONSULTA> CarregaTipoValorConsulta()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_VALOR_CONSULTA> conf = new List<TIPO_VALOR_CONSULTA>();
                //if (Session["TipoValorConsultas"] == null)
                //{
                    conf = vcApp.GetAllTipos(idAss);
                //}
                //else
                //{
                //    if ((Int32)Session["TipoValorConsultaAlterada"] == 1)
                //    {
                //        conf = vcApp.GetAllTipos(idAss);
                //    }
                //    else
                //    {
                //        conf = (List<TIPO_VALOR_CONSULTA>)Session["TipoValorConsultas"];
                //    }
                //}
                Session["TipoValorConsultas"] = conf;
                Session["TipoValorConsultaAlterada"] = 0;
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

        public List<TIPO_PAGAMENTO> CarregaTipoPagamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_PAGAMENTO> conf = new List<TIPO_PAGAMENTO>();
                if (Session["TipoPagamentos"] == null)
                {
                    conf = tpApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoPagamentoAlterada"] == 1)
                    {
                        conf = tpApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_PAGAMENTO>)Session["TipoPagamentos"];
                    }
                }
                Session["TipoPagamentos"] = conf;
                Session["TipoPagamentoAlterada"] = 0;
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

        public List<VALOR_CONVENIO> CarregaValorConvenio()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<VALOR_CONVENIO> conf = new List<VALOR_CONVENIO>();
                if (Session["ValorConvenios"] == null)
                {
                    conf = vvApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ValorConvenioAlterada"] == 1)
                    {
                        conf = vvApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<VALOR_CONVENIO>)Session["ValorConvenios"];
                    }
                }
                Session["ValorConvenios"] = conf;
                Session["ValorConvenioAlterada"] = 0;
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
        public ActionResult MontarTelaRecebimento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ACESSO == 0)
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
                Session["ModuloAtual"] = "Financeiro - Recebimentos";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0538", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0537", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 6)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 7)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 12)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 temHoje = 0;
                List<CONSULTA_RECEBIMENTO> listaHoje = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] == null)
                {
                    if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                    {
                        listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1).ToList();
                    }
                    else
                    {
                        listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    listaHoje = listaMasterRec.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == DateTime.Today.Date).ToList();
                    listaMasterRec = listaHoje;
                    if (listaHoje.Count > 0)
                    {
                        temHoje = 1;
                    }
                    listaMasterRec = listaMasterRec.OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                    Session["ListaRecebimento"] = listaMasterRec;
                    Session["TemHoje"] = temHoje;
                }

                // Monta demais listas
                listaMasterRec = listaMasterRec.OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                ViewBag.Forma = new SelectList(CarregaFormas(), "FORE_CD_ID", "FORE_NM_FORMA");
                ViewBag.TipoConsulta = new SelectList(CarregaTipoConsulta(), "VACO_CD_ID", "VACO_NM_NOME");
                ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
                ViewBag.Consulta = new SelectList(CarregaConsultas(), "PACO_CD_ID", "PACO_DT_CONSULTA");
                ViewBag.Listas = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                var confere = new List<SelectListItem>();
                confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
                confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Confere = new SelectList(confere, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Lista de Recebimentos*", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Total por Data*", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Total por Mês*", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Total por Ano", Value = "5" });
                relat.Add(new SelectListItem() { Text = "Total por Paciente*", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Total por Profissional*", Value = "6" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

                // Verifica possibilidade de criação
                List<CONSULTA_RECEBIMENTO> pagMes = new List<CONSULTA_RECEBIMENTO>();
                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    pagMes = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1).ToList();
                }
                else
                {
                    pagMes = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.RectoPode = 1;
                Int32 num = pagMes.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                if ((Int32)Session["NumRecebimentos"] <= num)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0588", CultureInfo.CurrentCulture);
                    ModelState.AddModelError("", frase);
                    ViewBag.RectoPode = 0;
                }

                // Acerta estado    
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 6;
                Session["VoltarConsulta"] = 4;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_4.pdf";
                Session["ModoConsulta"] = 0;
                Session["NivelRecebimento"] = 1;

                // Carrega view
                objetoRec = new CONSULTA_RECEBIMENTO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO", "Financeiro", "MontarTelaRecebimento");
                return View(objetoRec);
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
        public ActionResult FiltrarRecebimento(CONSULTA_RECEBIMENTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.CORE_NM_RECEBIMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.CORE_NM_RECEBIMENTO);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<CONSULTA_RECEBIMENTO> listaObj = new List<CONSULTA_RECEBIMENTO>();
                Tuple<Int32, List<CONSULTA_RECEBIMENTO>, Boolean> volta = recApp.ExecuteFilterTuple(item.VACO_CD_ID, item.PACI_CD_ID, item.PACO_CD_ID, item.FORE_CD_ID, item.CORE_NM_RECEBIMENTO, item.CORE_DT_DUMMY, item.CORE_DT_RECEBIMENTO, null, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaRecebimento");
                }

                // Sucesso
                listaMasterRec = volta.Item2.ToList();
                listaMasterRec = listaMasterRec.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                Session["ListaRecebimento"] = listaMasterRec;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult RetirarFiltroRecebimento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                Session["ListaRecebimento"] = null;
                Session["TipoExibicaoRecebimento"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult VerTodosRecebimento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1).OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                }
                else
                {
                    listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                }
                Session["ListaRecebimento"] = listaMasterRec;
                Session["TipoExibicaoRecebimento"] = 3;
                return RedirectToAction("MontarTelaRecebimento");
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
        public ActionResult IncluirRecebimento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Inclusão";
                        return RedirectToAction("MontarTelaRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Financeiro - Recebimentos - Inclusão";

                // Prepara view
                ViewBag.Forma = new SelectList(CarregaFormas(), "FORE_CD_ID", "FORE_NM_FORMA");
                ViewBag.TipoConsulta = new SelectList(CarregaTipoConsulta(), "VACO_CD_ID", "VACO_NM_NOME");
                ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
                ViewBag.Consulta = new SelectList(CarregaConsultas(), "PACO_CD_ID", "PACO_DT_CONSULTA");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_5.pdf";
                Session["NivelPaciente"] = 1;

                // Prepara registro
                CONSULTA_RECEBIMENTO item = new CONSULTA_RECEBIMENTO();
                RecebimentoViewModel vm = Mapper.Map<CONSULTA_RECEBIMENTO, RecebimentoViewModel>(item);
                vm.CORE_IN_ATIVO = 1;
                vm.CORE_DT_RECEBIMENTO = DateTime.Today.Date;
                vm.CORE_GU_GUID = Xid.NewXid().ToString();
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.CORE_IN_CONFERIDO = 0;
                vm.CORE_VL_VALOR = 0;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_INCLUIR", "Financeiro", "IncluirRecebimento");
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
        public ActionResult IncluirRecebimento(RecebimentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Forma = new SelectList(CarregaFormas(), "FORE_CD_ID", "FORE_NM_FORMA");
            ViewBag.TipoConsulta = new SelectList(CarregaTipoConsulta(), "VACO_CD_ID", "VACO_NM_NOME");
            ViewBag.Paciente = new SelectList(CarregaPaciente(), "PACI__CD_ID", "PACI_NM_NOME");
            ViewBag.Consulta = new SelectList(CarregaConsultas(), "PACO_CD_ID", "PACO_DT_CONSULTA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CORE_NM_RECEBIMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CORE_NM_RECEBIMENTO);
                    vm.CORE_IN_CONFERIDO = 0;

                    // Carrega paciente e consulta
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID.Value);

                    // Critica
                    if (vm.CORE_NM_RECEBIMENTO == null)
                    {
                        String nome = "Recebimento de " + pac.PACI_NM_NOME.ToUpper() + " em " + vm.CORE_DT_RECEBIMENTO.Value.ToLongDateString() + " referente a consulta";
                        vm.CORE_NM_RECEBIMENTO = nome;
                    }
                    if (vm.CORE_VL_VALOR == 0 || vm.CORE_VL_VALOR == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0716", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CONSULTA_RECEBIMENTO item = Mapper.Map<RecebimentoViewModel, CONSULTA_RECEBIMENTO>(vm);
                    Int32 volta = recApp.ValidateCreate(item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + item.CORE_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + item.CORE_CD_ID.ToString() + "/Recibo/";
                    map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Acerta estado
                    Session["IdRecebimento"] = item.CORE_CD_ID;
                    Session["RecebimentoAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaRecebimento"] = null;
                    Session["ListaRectoMes"] = null;
                    Int32 volta3 = 0;

                    // Trata anexos
                    if (Session["FileQueueRecebimento"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueRecebimento"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                volta3 = UploadFileQueueRecebimentoNovo(file);
                            }
                        }
                        Session["FileQueueRecebimento"] = null;
                    }

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Recebimento dto = MontarRecebimentoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    CONSULTA_RECEBIMENTO pag = recApp.GetItemById(item.CORE_CD_ID);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Recebimento - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O recebimento " + item.CORE_GU_GUID + " de " + pac.PACI_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    Session["NivelRecebimento"] = 1;
                    return RedirectToAction("MontarTelaRecebimento");
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
        public ActionResult EditarRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Edição";
                        return RedirectToAction("MontarTelaRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Financeiro - Recebimentos - Edição";

                // Prepara view
                var confere = new List<SelectListItem>();
                confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Confere = new SelectList(confere, "Value", "Text");
                Session["NivelPaciente"] = 1;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_6.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0604", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 55)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0605", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 56)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0606", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara registro
                CONSULTA_RECEBIMENTO item = recApp.GetItemById(id);
                Session["IdRecebimento"] = item.CORE_CD_ID;
                RecebimentoViewModel vm = Mapper.Map<CONSULTA_RECEBIMENTO, RecebimentoViewModel>(item);
                Session["Recebimento"] = item;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_EDITAR", "Financeiro", "EditarRecebimento");
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
        public ActionResult EditarRecebimento(RecebimentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            var confere = new List<SelectListItem>();
            confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Confere = new SelectList(confere, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.CORE_NM_RECEBIMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.CORE_NM_RECEBIMENTO);

                    // Carrega paciente
                    PACIENTE pac = pacApp.GetItemById(vm.PACI_CD_ID.Value);

                    // Critica
                    if (vm.CORE_NM_RECEBIMENTO == null)
                    {
                        String nome = "Recebimento de " + pac.PACI_NM_NOME + " em " + vm.CORE_DT_RECEBIMENTO.Value.ToLongDateString();
                        vm.CORE_NM_RECEBIMENTO = nome;
                    }
                    if (vm.CORE_IN_CONFERIDO == null)
                    {
                        vm.CORE_IN_CONFERIDO = 0;
                    }

                    // Executa a operação
                    CONSULTA_RECEBIMENTO item = Mapper.Map<RecebimentoViewModel, CONSULTA_RECEBIMENTO>(vm);
                    Int32 volta = recApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno
                    Session["IdRecebimento"] = item.CORE_CD_ID;
                    Session["RecebimentoAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaRecebimento"] = null;
                    Session["NivelRecebimento"] = 1;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    CONSULTA_RECEBIMENTO pag = recApp.GetItemById(item.CORE_CD_ID);
                    DTO_Recebimento dto = MontarRecebimentoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    DTO_Recebimento dtoAntes = MontarRecebimentoDTOObj((CONSULTA_RECEBIMENTO)Session["Recebimento"]);
                    String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Recebimento - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O Recebimento " + item.CORE_GU_GUID + " de " + pac.PACI_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaRecebimento");
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
        public ActionResult ExcluirRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Exclusão";
                        return RedirectToAction("MontarTelaRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONSULTA_RECEBIMENTO item = recApp.GetItemById(id);
                item.CORE_IN_ATIVO = 0;
                Int32 volta = recApp.ValidateDelete(item, usuarioLogado);
                PACIENTE pac = pacApp.GetItemById(item.PACI_CD_ID.Value);

                Session["RecebimentoAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaRecebimento"] = null;

                // Configura serialização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Recebimento dto = MontarRecebimentoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                CONSULTA_RECEBIMENTO pag = recApp.GetItemById(item.CORE_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O recebimento " + item.CORE_GU_GUID + " de " + pac.PACI_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensPaciente"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaRecebimento");
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


        public Int32 UploadFileQueueRecebimentoNovo(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdRecebimento"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONSULTA_RECEBIMENTO copa = recApp.GetItemById(idNot);

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return 1;
                }

                // Valida Nome
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                Session["CompAnexo"] = fileName;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + idNot.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                RECEBIMENTO_ANEXO foto = new RECEBIMENTO_ANEXO();
                foto.REAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.REAN_DT_ANEXO = DateTime.Today;
                foto.REAN_IN_ATIVO = 1;
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
                foto.REAN_IN_TIPO = tipo;
                foto.REAN_NM_TITULO = fileName;
                foto.ASSI_CD_ID = idAss;
                foto.CORE_CD_ID = copa.CORE_CD_ID;
                copa.RECEBIMENTO_ANEXO.Add(foto);
                objetoAntesREc = copa;
                Int32 volta = recApp.ValidateEdit(copa, objetoAntesREc);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recebimento: " + copa.CORE_NM_RECEBIMENTO.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);
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
                return 9;
            }
        }

        public ActionResult VoltarAnexoRecebimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarRecebimento", new { id = (Int32)Session["IdRecebimento"] });
        }

        [HttpPost]
        public void UploadFileToSessionRecebimento(IEnumerable<HttpPostedFileBase> files, String profile)
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
            Session["FileQueueRecebimento"] = queue;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileRecebimento(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdRecebimento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                CONSULTA_RECEBIMENTO item = recApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + item.CORE_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }


                // Gravar registro
                RECEBIMENTO_ANEXO foto = new RECEBIMENTO_ANEXO();
                foto.ASSI_CD_ID = idAss;
                foto.REAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.REAN_DT_ANEXO = DateTime.Today;
                foto.REAN_IN_ATIVO = 1;
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
                foto.REAN_IN_TIPO = tipo;
                foto.REAN_NM_TITULO = fileName;
                foto.CORE_CD_ID = item.CORE_CD_ID;
                item.RECEBIMENTO_ANEXO.Add(foto);
                Int32 volta = recApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "iarCOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recebimento: " + item.CORE_NM_RECEBIMENTO + " | Anexo: " + fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelRecebimento"] = 2;
                Session["RecebimentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoRecebimento");
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
        public ActionResult VerAnexoRecebimento(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_ANEXO item = recApp.GetAnexoById(id);
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_ANEXO", "Financeiro", "VerAnexoRecebimento");
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
        public ActionResult VerAnexoRecebimentoAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_ANEXO item = recApp.GetAnexoById(id);
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_ANEXO", "Financeiro", "VerAnexoRecebimentoAudio");
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

        public FileResult DownloadRecebimento(Int32 id)
        {
            try
            {
                RECEBIMENTO_ANEXO item = recApp.GetAnexoById(id);
                String arquivo = item.REAN_AQ_ARQUIVO;
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
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 2;
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
        public ActionResult ExcluirAnexoRecebimento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_ANEXO item = recApp.GetAnexoById(id);
                CONSULTA_RECEBIMENTO pac = recApp.GetItemById(item.CORE_CD_ID.Value);
                item.REAN_IN_ATIVO = 0;
                Int32 volta = recApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recebimento: " + pac.CORE_NM_RECEBIMENTO.ToUpper() + " | Anexo: " + item.REAN_NM_TITULO + " | Data: " + item.REAN_DT_ANEXO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 2;
                Session["RecebimentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoRecebimento");
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

        public ActionResult IncluirAnotacaoRecebimento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Alteração";
                        return RedirectToAction("VoltarAnexoRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 3;

                CONSULTA_RECEBIMENTO item = recApp.GetItemById((Int32)Session["IdRecebimento"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_ANOTACAO coment = new RECEBIMENTO_ANOTACAO();
                RecebimentoAnotacaoViewModel vm = Mapper.Map<RECEBIMENTO_ANOTACAO, RecebimentoAnotacaoViewModel>(coment);
                vm.REAT_DT_ANOTACAO = DateTime.Now;
                vm.REAT_IN_ATIVO = 1;
                vm.CORE_CD_ID = item.CORE_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_ANOTACAO_INCLUIR", "Financeiro", "IncluirAnotacaoRecebimento");
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
        public ActionResult IncluirAnotacaoRecebimento(RecebimentoAnotacaoViewModel vm)
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
                    vm.REAT_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.REAT_TX_ANOTACAO);

                    // Executa a operação
                    RECEBIMENTO_ANOTACAO item = Mapper.Map<RecebimentoAnotacaoViewModel, RECEBIMENTO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONSULTA_RECEBIMENTO not = recApp.GetItemById((Int32)Session["IdRecebimento"]);

                    item.USUARIO = null;
                    not.RECEBIMENTO_ANOTACAO.Add(item);
                    Int32 volta = recApp.ValidateEdit(not, not);

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Recebimento_Anotacao dto = MontarRecebimentoAnotacaoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Recebimento - Anotação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    Session["NivelPaciente"] = 1;
                    Session["NivelRecebimento"] = 3;
                    Session["VoltarPesquisa"] = 0;
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    return RedirectToAction("VoltarAnexoRecebimento");
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
        public ActionResult EditarAnotacaoRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Alteração";
                        return RedirectToAction("VoltarAnexoRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 3;
                RECEBIMENTO_ANOTACAO item = recApp.GetAnotacaoById(id);
                RecebimentoAnotacaoViewModel vm = Mapper.Map<RECEBIMENTO_ANOTACAO, RecebimentoAnotacaoViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "RECEBIMENTO_ANOTACAO_EDITAR", "Financeiro", "EditarAnotacaoRecebimento");
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
        public ActionResult EditarAnotacaoRecebimento(RecebimentoAnotacaoViewModel vm)
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
                    vm.REAT_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.REAT_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    RECEBIMENTO_ANOTACAO item = Mapper.Map<RecebimentoAnotacaoViewModel, RECEBIMENTO_ANOTACAO>(vm);
                    CONSULTA_RECEBIMENTO copa = recApp.GetItemById(item.CORE_CD_ID.Value);
                    Int32 volta = recApp.ValidateEditAnotacao(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Recebimento - Anotação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Recebimento: " + copa.CORE_NM_RECEBIMENTO.ToUpper() + " | Data: " + item.REAT_DT_ANOTACAO.ToString() + " | Anotação: " + item.REAT_TX_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["RecebimentoAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["NivelRecebimento"] = 3;
                    return RedirectToAction("VoltarAnexoRecebimento");
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
        public ActionResult ExcluirAnotacaoRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Recebimento - Alteração";
                        return RedirectToAction("VoltarAnexoRecebimento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_ANOTACAO item = recApp.GetAnotacaoById(id);
                item.REAT_IN_ATIVO = 0;
                Int32 volta = recApp.ValidateEditAnotacao(item);
                Session["RecebimentoAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 3;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Anotação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recebimento: " + item.CONSULTA_RECEBIMENTO.CORE_NM_RECEBIMENTO.ToUpper() + " | Data: " + item.REAT_DT_ANOTACAO.ToString() + " | Anotação: " + item.REAT_TX_ANOTACAO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoRecebimento");
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

        public JsonResult GetRecebimento(Int32 id)
        {
            var tp = tpApp.GetItemById(id);
            var hash = new Hashtable();
            String nome = "Pagamento de " + tp.TIPA_NM_PAGAMENTO + " em " + DateTime.Today.Date.ToLongDateString();            
            hash.Add("pagamento", nome);
            return Json(hash);
        }

        [HttpGet]
        public ActionResult MontarTelaPagamento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ACESSO == 0)
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
                Session["ModuloAtual"] = "Financeiro - Pagamentos";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0538", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0537", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 6)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 7)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 12)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0535", CultureInfo.CurrentCulture) + " - Arquivo: " + (String)Session["CompAnexo"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 temHoje = 0;
                List<CONSULTA_PAGAMENTO> listaHoje = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] == null)
                {
                    if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                    {
                        listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 0).ToList();
                    }
                    else
                    {
                        listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 0 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    listaHoje = listaMasterPag.Where(p => p.COPA_DT_VENCIMENTO.Value.Date == DateTime.Today.Date).ToList();
                    listaMasterPag = listaHoje;
                    if (listaHoje.Count > 0)
                    {
                        temHoje = 1;
                    }
                    listaMasterPag = listaMasterPag.OrderBy(p => p.COPA_DT_VENCIMENTO).ToList();
                    Session["ListaPagamento"] = listaMasterPag;
                    Session["TemHoje"] = temHoje;
                }

                // Monta demais listas
                ViewBag.Tipo = new SelectList(CarregaTipoPagamento(), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");
                ViewBag.Listas = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                var confere = new List<SelectListItem>();
                confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
                confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Confere = new SelectList(confere, "Value", "Text");
                var quitacao = new List<SelectListItem>();
                quitacao.Add(new SelectListItem() { Text = "Em Aberto", Value = "0" });
                quitacao.Add(new SelectListItem() { Text = "Quitados", Value = "1" });
                ViewBag.Quitacao = new SelectList(quitacao, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Relação de Pagamentos*", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Total por Data*", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Total por Mês", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Total por Ano", Value = "9" });
                relat.Add(new SelectListItem() { Text = "Total por Favorecido*", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Total por Tipo*", Value = "10" });
                relat.Add(new SelectListItem() { Text = "Vencendo Hoje", Value = "5" });
                relat.Add(new SelectListItem() { Text = "Vencendo no Mês", Value = "6" });
                relat.Add(new SelectListItem() { Text = "Quitados Hoje", Value = "7" });
                relat.Add(new SelectListItem() { Text = "Quitados no Mês", Value = "8" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

                // Verifica possibilidade de criação
                List<CONSULTA_PAGAMENTO> pagMes = new List<CONSULTA_PAGAMENTO>();
                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    pagMes = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1).ToList();
                }
                else
                {
                    pagMes = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.PagtoPode = 1;
                Int32 num = pagMes.Where(p => p.COPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                if ((Int32)Session["NumPagamentos"] <= num)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0587", CultureInfo.CurrentCulture);
                    ModelState.AddModelError("", frase);
                    ViewBag.PagtoPode = 0;
                }

                // Acerta estado    
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 6;
                Session["VoltarConsulta"] = 4;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_1.pdf";
                Session["ModoConsulta"] = 0;
                Session["NivelPagamento"] = 1;

                // Carrega view
                objetoPag = new CONSULTA_PAGAMENTO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO", "Financeiro", "MontarTelaPagamento");
                return View(objetoPag);
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
        public ActionResult FiltrarPagamento(CONSULTA_PAGAMENTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.COPA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(item.COPA_NM_NOME);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<CONSULTA_PAGAMENTO> listaObj = new List<CONSULTA_PAGAMENTO>();
                Tuple<Int32, List<CONSULTA_PAGAMENTO>, Boolean> volta = pagApp.ExecuteFilterTuple(item.TIPA_CD_ID, item.COPA_NM_NOME, item.COPA_NM_FAVORECIDO, item.COPA_DT_DUMMY, item.COPA_DT_VENCIMENTO, item.COPA_IN_PAGO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaPagamento");
                }

                // Sucesso
                listaMasterPag = volta.Item2.ToList();
                listaMasterPag = listaMasterPag.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                Session["ListaPagamento"] = listaMasterPag;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult RetirarFiltroPagamento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                Session["ListaPagamento"] = null;
                Session["TipoExibicaoPagamento"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult VerTodosPagamento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1).ToList();
                }
                else
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["TipoExibicaoPagamento"] = 5;
                Session["ListaPagamento"] = listaMasterPag;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult VerPagamentoMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 0 & p.COPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                }
                else
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 0 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                }
                Session["ListaPagamento"] = listaMasterPag;
                Session["TipoExibicaoPagamento"] = 2;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult VerPagamentosFeitosHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.COPA_DT_PAGAMENTO == DateTime.Today.Date).ToList();
                }
                else
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_PAGAMENTO == DateTime.Today.Date).ToList();
                }
                Session["ListaPagamento"] = listaMasterPag;
                Session["TipoExibicaoPagamento"] = 3;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult VerPagamentoFeitosMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.COPA_DT_PAGAMENTO != null).ToList();
                }
                else
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_PAGAMENTO != null).ToList();
                }
                listaMasterPag = listaMasterPag.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                Session["ListaPagamento"] = listaMasterPag;
                Session["TipoExibicaoPagamento"] = 4;
                return RedirectToAction("MontarTelaPagamento");
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
        public JsonResult GetFavorecidoNome(String term)
        {
            List<CONSULTA_PAGAMENTO> usu = CarregaPagamento();
            List<String> nomes = usu.Select(p => p.COPA_NM_FAVORECIDO).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerRecebimentoMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                }
                else
                {
                    listaMasterRec = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();
                }
                Session["ListaRecebimento"] = listaMasterRec;
                Session["TipoExibicaoRecebimento"] = 2;
                return RedirectToAction("MontarTelaRecebimento");
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
        public ActionResult IncluirPagamento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Inclusão";
                        return RedirectToAction("MontarTelaPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                var quita = new List<SelectListItem>();
                quita.Add(new SelectListItem() { Text = "Não", Value = "0" });
                quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Quitar = new SelectList(quita, "Value", "Text");
                var rec = new List<SelectListItem>();
                rec.Add(new SelectListItem() { Text = "Não", Value = "0" });
                rec.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Recursivo = new SelectList(rec, "Value", "Text");
                ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");
                var fixo = new List<SelectListItem>();
                fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
                fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Fixo = new SelectList(fixo, "Value", "Text");

                ViewBag.Tipo = new SelectList(CarregaTipoPagamento(), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");
                Session["ModuloAtual"] = "Financeiro - Pagamentos - Inclusão";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_2.pdf";
                Session["NivelPaciente"] = 1;

                // Prepara registro
                CONSULTA_PAGAMENTO item = new CONSULTA_PAGAMENTO();
                PagamentoViewModel vm = Mapper.Map<CONSULTA_PAGAMENTO, PagamentoViewModel>(item);
                vm.COPA_IN_ATIVO = 1;
                vm.COPA_GU_GUID = Xid.NewXid().ToString();
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.COPA_IN_CONFERIDO = 0;
                vm.COPA_VL_VALOR = 0;
                vm.COPA_VL_DESCONTO = 0;
                vm.COPA_VL_MULTA = 0;
                vm.COPA_VL_PAGO = 0;
                vm.COPA_IN_PAGO = 0;
                vm.QUITA_PAGAMENTO = 0;
                vm.NUMERO_VEZES = 1;
                vm.DATA_INICIO = DateTime.Today.Date;
                vm.RECURSIVO = 0;
                vm.COPA_DT_CADASTRO = DateTime.Today.Date;
                vm.DIA_FIXO = 0;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_INCLUIR", "Financeiro", "IncluirPagamento");
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
        public async Task<ActionResult> IncluirPagamento(PagamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Tipo = new SelectList(CarregaTipoPagamento(), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");
            var quita = new List<SelectListItem>();
            quita.Add(new SelectListItem() { Text = "Não", Value = "0" });
            quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Quitar = new SelectList(quita, "Value", "Text");
            var rec = new List<SelectListItem>();
            rec.Add(new SelectListItem() { Text = "Não", Value = "0" });
            rec.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Recursivo = new SelectList(rec, "Value", "Text");
            ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");
            var fixo = new List<SelectListItem>();
            fixo.Add(new SelectListItem() { Text = "Não", Value = "0" });
            fixo.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Fixo = new SelectList(fixo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.COPA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COPA_NM_NOME);
                    vm.COPA_NM_FAVORECIDO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COPA_NM_FAVORECIDO);
                    vm.COPA_IN_CONFERIDO = 0;
                    vm.COPA_DT_CADASTRO = DateTime.Today.Date;
                    Int32? tipo = vm.TIPA_CD_ID;

                    // Critica
                    if (vm.COPA_NM_NOME == null)
                    {
                        TIPO_PAGAMENTO tp = tpApp.GetItemById(vm.TIPA_CD_ID.Value);
                        String nome = "Pagamento de " + tp.TIPA_NM_PAGAMENTO + " em " + vm.COPA_DT_PAGAMENTO.Value.ToLongDateString();
                        vm.COPA_NM_NOME = nome;
                    }
                    if (vm.COPA_NM_FAVORECIDO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0718", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.RECURSIVO == null)
                    {
                        vm.RECURSIVO = 0;
                    }
                    if (vm.COPA_VL_VALOR == 0 || vm.COPA_VL_VALOR == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0715", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.COPA_DT_VENCIMENTO == null)
                    {
                        vm.COPA_DT_VENCIMENTO = DateTime.Today.Date;
                    }
                    if (vm.COPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0717", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Critica de quitacao
                    if (vm.QUITA_PAGAMENTO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0648", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    else if (vm.QUITA_PAGAMENTO == 1)
                    {
                        if (vm.COPA_DT_PAGAMENTO == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0649", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        vm.COPA_IN_PAGO = 1;
                    }
                    else
                    {
                        vm.COPA_DT_PAGAMENTO = null;
                        vm.COPA_IN_PAGO = 0;
                        vm.COPA_VL_DESCONTO = 0;
                        vm.COPA_VL_MULTA = 0;
                        vm.COPA_VL_PAGO = 0;
                    }

                    // Critica de recursividade
                    Int32 tipoRec = 1;
                    if (vm.RECURSIVO == 1)
                    {
                        if (vm.PETA_CD_ID == null || vm.PETA_CD_ID == 0)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0650", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.DATA_FINAL == null & (vm.NUMERO_VEZES == null || vm.NUMERO_VEZES == 0))
                        {
                            vm.NUMERO_VEZES = 1;
                            tipoRec = 2;
                        }
                        if (vm.DATA_FINAL != null & (vm.NUMERO_VEZES == null || vm.NUMERO_VEZES == 0))
                        {
                            vm.NUMERO_VEZES = 0;
                            tipoRec = 1;
                        }
                        if (vm.DATA_FINAL == null & (vm.NUMERO_VEZES != null & vm.NUMERO_VEZES > 0))
                        {
                            tipoRec = 2;
                        }
                        if (vm.DIA_FIXO == null)
                        {
                            vm.DIA_FIXO = 0;
                        }
                        PERIODICIDADE_TAREFA peta = perApp.GetItemById(vm.PETA_CD_ID.Value);
                        if (peta.PETA_NM_NOME == "Diária" || peta.PETA_NM_NOME == "Semanal")
                        {
                            vm.DIA_FIXO = 0;
                        }
                        if (vm.DIA_FIXO == 1)
                        {
                            if (vm.COPA_DT_VENCIMENTO.Value.Day == 31)
                            {
                                vm.COPA_DT_VENCIMENTO.Value.AddDays(-1);
                            }
                        }
                    }

                    // Processa criação
                    if (vm.RECURSIVO == 0)
                    {
                        // Processa sem recursividade
                        CONSULTA_PAGAMENTO item = Mapper.Map<PagamentoViewModel, CONSULTA_PAGAMENTO>(vm);
                        Int32 volta = pagApp.ValidateCreate(item, usuarioLogado);

                        // Cria pastas
                        String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/Anexos/";
                        String map = Server.MapPath(caminho);
                        Directory.CreateDirectory(Server.MapPath(caminho));
                        caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/NF/";
                        map = Server.MapPath(caminho);
                        Directory.CreateDirectory(Server.MapPath(caminho));

                        // Acerta pagamento
                        CONSULTA_PAGAMENTO pagX = pagApp.GetItemById(item.COPA_CD_ID);
                        pagX.TIPA_CD_ID = tipo;
                        Int32 voltaA = pagApp.ValidateEdit(pagX, pagX);

                        // Acerta estado
                        Session["IdPagamento"] = item.COPA_CD_ID;
                        Session["PagamentoAlterada"] = 1;
                        Session["NivelPaciente"] = 1;
                        Session["ListaPagamento"] = null;
                        Session["ListaPagtoMes"] = null;
                        Int32 volta3 = 0;

                        // Trata anexos
                        if (Session["FileQueuePagamento"] != null)
                        {
                            List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePagamento"];
                            foreach (var file in fq)
                            {
                                if (file.Profile == null)
                                {
                                    volta3 = await UploadFileQueuePagamentoNovoBlob(file);
                                }
                            }
                            Session["FileQueuePagamento"] = null;
                        }

                        // Configura serialização
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore
                        };

                        // Monta Log
                        DTO_Pagamento dto = MontarPagamentoDTOObj(item);
                        String json = JsonConvert.SerializeObject(dto, settings);
                        LOG log = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                            USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Pagamento - Inclusão",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json,
                            LOG_IN_SISTEMA = 6
                        };
                        Int32 volta1 = logApp.ValidateCreate(log);

                        // Mensagem do CRUD
                        Session["MsgCRUD"] = "O pagamento " + item.COPA_GU_GUID + " de " + pagX.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO.ToUpper() + " para " + pagX.COPA_NM_FAVORECIDO.ToUpper() + " foi cadastrado com sucesso.";
                        Session["MensPaciente"] = 61;

                    }
                    else
                    {

                        // Processa recursividade
                        PERIODICIDADE_TAREFA peta = perApp.GetItemById(vm.PETA_CD_ID.Value);
                        Int32 dias = peta.PETA_NR_DIAS;
                        Int32 num = 1;
                        Int32 dia = 0;
                        Int32 vezes = 1;
                        DateTime data = vm.COPA_DT_VENCIMENTO.Value;
                        String fav = vm.COPA_NM_FAVORECIDO;
                        while (num > 0)
                        {
                            CONSULTA_PAGAMENTO pag = new CONSULTA_PAGAMENTO();
                            pag.ASSI_CD_ID = vm.ASSI_CD_ID;
                            pag.COPA_DT_VENCIMENTO = data;
                            pag.COPA_GU_GUID = Xid.NewXid().ToString();
                            pag.COPA_IN_ATIVO = 1;
                            pag.COPA_IN_CONFERIDO = 0;
                            pag.COPA_DT_CADASTRO = DateTime.Today.Date;
                            if (num == 1)
                            {
                                if (vm.QUITA_PAGAMENTO == 1)
                                {
                                    pag.COPA_DT_PAGAMENTO = vm.COPA_DT_PAGAMENTO;
                                    pag.COPA_IN_PAGO = 1;
                                    pag.COPA_VL_DESCONTO = vm.COPA_VL_DESCONTO;
                                    pag.COPA_VL_MULTA = vm.COPA_VL_MULTA;
                                    pag.COPA_VL_PAGO = vm.COPA_VL_PAGO;
                                }
                                else
                                {
                                    pag.COPA_DT_PAGAMENTO = null;
                                    pag.COPA_IN_PAGO = 0;
                                    pag.COPA_VL_DESCONTO = 0;
                                    pag.COPA_VL_MULTA = 0;
                                    pag.COPA_VL_PAGO = 0;
                                }
                            }
                            else
                            {
                                pag.COPA_DT_PAGAMENTO = null;
                                pag.COPA_IN_PAGO = 0;
                                pag.COPA_VL_DESCONTO = 0;
                                pag.COPA_VL_MULTA = 0;
                                pag.COPA_VL_PAGO = 0;
                            }
                            pag.COPA_NM_FAVORECIDO = vm.COPA_NM_FAVORECIDO;
                            pag.COPA_NM_NOME = vm.COPA_NM_NOME;
                            pag.COPA_VL_VALOR = vm.COPA_VL_VALOR;
                            pag.TIPA_CD_ID = vm.TIPA_CD_ID;
                            pag.USUA_CD_ID = vm.USUA_CD_ID;
                            Int32 volta = pagApp.ValidateCreate(pag, usuarioLogado);
                            CONSULTA_PAGAMENTO pagto = pagApp.GetItemById(pag.COPA_CD_ID);
                            pagto.TIPA_CD_ID = tipo;
                            Int32 voltaA = pagApp.ValidateEdit(pagto, pagto);

                            // Cria pastas
                            String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + pagto.COPA_CD_ID.ToString() + "/Anexos/";
                            String map = Server.MapPath(caminho);
                            Directory.CreateDirectory(Server.MapPath(caminho));
                            caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + pagto.COPA_CD_ID.ToString() + "/NF/";
                            map = Server.MapPath(caminho);
                            Directory.CreateDirectory(Server.MapPath(caminho));

                            // Configura serialização
                            JsonSerializerSettings settings = new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore
                            };

                            // Monta Log
                            DTO_Pagamento dto = MontarPagamentoDTOObj(pag);
                            String json = JsonConvert.SerializeObject(dto, settings);
                            CONSULTA_PAGAMENTO paga = pagApp.GetItemById(pag.COPA_CD_ID);
                            LOG log = new LOG
                            {
                                LOG_DT_DATA = DateTime.Now,
                                ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                                USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                                LOG_NM_OPERACAO = "Pagamento Recursivo - Inclusão",
                                LOG_IN_ATIVO = 1,
                                LOG_TX_REGISTRO = json,
                                LOG_IN_SISTEMA = 6
                            };
                            Int32 volta1 = logApp.ValidateCreate(log);

                            // Calcula proximo vencimento
                            if (peta.PETA_NM_NOME == "Anual")
                            {
                                data = data.AddYears(1);
                            }
                            else if (peta.PETA_NM_NOME == "Diária" || peta.PETA_NM_NOME == "Semanal" || peta.PETA_NM_NOME == "Quinzenal")
                            {
                                data = data.AddDays(dias);
                            }
                            else
                            {
                                if (vm.DIA_FIXO == 1)
                                {
                                    Int32 mes = dias / 30;
                                    data = data.AddMonths(mes);
                                }
                                else
                                {
                                    data = data.AddDays(dias);
                                }
                            }

                            // Contorno
                            num++;
                            if (tipoRec == 1)
                            {
                                if (data > vm.DATA_FINAL)
                                {
                                    num = 0;
                                    break;
                                }
                            }
                            else
                            {
                                vezes++;
                                if (vezes > vm.NUMERO_VEZES)
                                {
                                    num = 0;
                                    break;
                                }
                            }
                        }

                        // Mensagem do CRUD
                        Session["MsgCRUD"] = "Foram cadastrados " + vm.NUMERO_VEZES.ToString() + " pagamentos recursivos para " + fav.ToUpper();
                        Session["MensPaciente"] = 61;

                    }

                    // Retorno
                    Session["Pagamentos"] = null;
                    Session["PagamentoAlterada"] = 1;
                    Session["NivelPagamento"] = 1;
                    return RedirectToAction("MontarTelaPagamento");
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

        public DTO_Pagamento MontarPagamentoDTOObj(CONSULTA_PAGAMENTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Pagamento()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    COPA_CD_ID = l.COPA_CD_ID,
                    TIPA_CD_ID = l.TIPA_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    COPA_DT_CADASTRO = l.COPA_DT_CADASTRO,
                    COPA_DT_PAGAMENTO = l.COPA_DT_PAGAMENTO,
                    COPA_DT_VENCIMENTO = l.COPA_DT_VENCIMENTO,
                    COPA_GU_GUID = l.COPA_GU_GUID,
                    COPA_IN_ATIVO = l.COPA_IN_ATIVO,
                    COPA_IN_CONFERIDO = l.COPA_IN_CONFERIDO,
                    COPA_IN_PAGO = l.COPA_IN_PAGO,
                    COPA_NM_FAVORECIDO = l.COPA_NM_FAVORECIDO,
                    COPA_NM_NOME = l.COPA_NM_NOME,
                    COPA_NR_ATRASO = l.COPA_NR_ATRASO,
                    COPA_VL_DESCONTO = l.COPA_VL_DESCONTO,
                    COPA_VL_MULTA = l.COPA_VL_MULTA,
                    COPA_VL_PAGO = l.COPA_VL_PAGO,
                    COPA_VL_VALOR = l.COPA_VL_VALOR,
                    COPA_XM_NOTA_FISCAL = l.COPA_XM_NOTA_FISCAL,
                };
                return mediDTO;
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

        public DTO_Pagamento_Anotacao MontarPagamentoAnotacaoDTOObj(PAGAMENTO_ANOTACAO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Pagamento_Anotacao()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    COPA_CD_ID = l.COPA_CD_ID,
                    PGAN_CD_ID = l.PGAN_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    PGAN_DT_ANOTACAO = l.PGAN_DT_ANOTACAO,
                    PGAN_IN_ATIVO = l.PGAN_IN_ATIVO,
                    PGAN_TX_ANOTACAO = l.PGAN_TX_ANOTACAO,
                };
                return mediDTO;
            }

        }

        public DTO_Recebimento_Anotacao MontarRecebimentoAnotacaoDTOObj(RECEBIMENTO_ANOTACAO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Recebimento_Anotacao()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    CORE_CD_ID = l.CORE_CD_ID,
                    REAT_CD_ID = l.REAT_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    REAT_DT_ANOTACAO = l.REAT_DT_ANOTACAO,
                    REAT_IN_ATIVO = l.REAT_IN_ATIVO,
                    REAT_TX_ANOTACAO = l.REAT_TX_ANOTACAO,
                };
                return mediDTO;
            }
        }

        [HttpGet]
        public ActionResult EditarPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Edição";
                        return RedirectToAction("MontarTelaPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                ViewBag.Tipo = new SelectList(CarregaTipoPagamento(), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");
                var confere = new List<SelectListItem>();
                confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Confere = new SelectList(confere, "Value", "Text");
                var quita = new List<SelectListItem>();
                quita.Add(new SelectListItem() { Text = "Não", Value = "0" });
                quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Quitar = new SelectList(quita, "Value", "Text");
                Session["NivelPaciente"] = 1;
                Session["ModuloAtual"] = "Financeiro - Pagamentos - Edição";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_3.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 5)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0601", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 55)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0602", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 56)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0603", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara registro
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(id);
                Session["IdPagamento"] = item.COPA_CD_ID;
                PagamentoViewModel vm = Mapper.Map<CONSULTA_PAGAMENTO, PagamentoViewModel>(item);
                Session["Pagamento"] = item;
                Session["TipoPagamentoId"] = item.TIPA_CD_ID;
                Session["PagamentoVer"] = 0;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_EDITAR", "Financeiro", "EditarPagamento");
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
        public ActionResult EditarPagamento(PagamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ViewBag.Tipo = new SelectList(CarregaTipoPagamento(), "TIPA_CD_ID", "TIPA_NM_NOME");
            var confere = new List<SelectListItem>();
            confere.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            confere.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Confere = new SelectList(confere, "Value", "Text");
            var quita = new List<SelectListItem>();
            quita.Add(new SelectListItem() { Text = "Não", Value = "0" });
            quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Quitar = new SelectList(quita, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.COPA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COPA_NM_NOME);
                    vm.COPA_NM_FAVORECIDO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.COPA_NM_FAVORECIDO);
                    vm.TIPA_CD_ID = (Int32)Session["TipoPagamentoId"];

                    // Critica
                    if (vm.COPA_NM_NOME == null)
                    {
                        String nome = "Pagamento de " + vm.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO.ToUpper() + " em " + vm.COPA_DT_PAGAMENTO.Value.ToLongDateString();
                        vm.COPA_NM_NOME = nome;
                    }
                    if (vm.COPA_IN_CONFERIDO == null)
                    {
                        vm.COPA_IN_CONFERIDO = 0;
                    }

                    // Processa pagamento
                    if (vm.QUITA_PAGAMENTO == null)
                    {
                        vm.QUITA_PAGAMENTO = 0;
                    }
                    if (vm.QUITA_PAGAMENTO == 1)
                    {
                        if (vm.COPA_DT_PAGAMENTO == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0649", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        vm.COPA_IN_PAGO = 1;
                    }
                    else
                    {
                        vm.COPA_DT_PAGAMENTO = null;
                        vm.COPA_IN_PAGO = 0;
                        vm.COPA_VL_DESCONTO = 0;
                        vm.COPA_VL_MULTA = 0;
                        vm.COPA_VL_PAGO = 0;
                    }

                    // Executa a operação
                    CONSULTA_PAGAMENTO item = Mapper.Map<PagamentoViewModel, CONSULTA_PAGAMENTO>(vm);
                    Int32 volta = pagApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno
                    Session["IdPagamento"] = item.COPA_CD_ID;
                    Session["PagamentoAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaPagamento"] = null;
                    Session["NivelPagamento"] = 1;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    CONSULTA_PAGAMENTO pag = pagApp.GetItemById(item.COPA_CD_ID);
                    DTO_Pagamento dto = MontarPagamentoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    DTO_Pagamento dtoAntes = MontarPagamentoDTOObj((CONSULTA_PAGAMENTO)Session["Pagamento"]);
                    String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Pagamento - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O pagamento " + item.COPA_GU_GUID + " de " + pag.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO.ToUpper() + " foi alterado com sucesso.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaPagamento");
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
        public ActionResult VerPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Edição";
                        return RedirectToAction("MontarTelaPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 1;
                Session["ModuloAtual"] = "Financeiro - Pagamentos - Ver";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20_3.pdf";
                Int32 s = (Int32)Session["VoltarPesquisa"];

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara registro
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(id);
                Session["IdPagamento"] = item.COPA_CD_ID;
                PagamentoViewModel vm = Mapper.Map<CONSULTA_PAGAMENTO, PagamentoViewModel>(item);
                Session["Pagamento"] = item;
                Session["TipoPagamentoId"] = item.TIPA_CD_ID;
                Session["PagamentoVer"] = 1;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_VER", "Financeiro", "VerPagamento");
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
        public ActionResult ExcluirPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Exclusão";
                        return RedirectToAction("MontarTelaPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(id);
                item.COPA_IN_ATIVO = 0;
                Int32 volta = pagApp.ValidateDelete(item, usuarioLogado);

                Session["PagamentoAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaPagamento"] = null;

                // Configura serialização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Pagamento dto = MontarPagamentoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                CONSULTA_PAGAMENTO pag = pagApp.GetItemById(item.COPA_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O pagamento " + item.COPA_GU_GUID + " de " + pag.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO.ToUpper() + " para " + pag.COPA_NM_FAVORECIDO.ToUpper() + " foi excluído com sucesso.";
                Session["MensPaciente"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaPagamento");
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
        public ActionResult UploadFileQueuePagamento(FileQueue file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONSULTA_PAGAMENTO copa = pagApp.GetItemById(idNot);

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("MontarTelaPagamento");
                }

                // Valida Nome
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("MontarTelaPagamento");
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("MontarTelaPagamento");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return RedirectToAction("MontarTelaPagamento");
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + idNot.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                PAGAMENTO_ANEXO foto = new PAGAMENTO_ANEXO();
                foto.PAAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAAN_DT_ANEXO = DateTime.Today;
                foto.PAAN_IN_ATIVO = 1;
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
                foto.PAAN_IN_TIPO = tipo;
                foto.PAAN_NM_TITULO = fileName;
                foto.COPA_CD_ID = copa.COPA_CD_ID;
                copa.PAGAMENTO_ANEXO.Add(foto);
                objetoAntesPag = copa;
                Int32 volta = pagApp.ValidateEdit(copa, objetoAntesPag);
                return RedirectToAction("MontarTelaPagamento");
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

        public Int32 UploadFileQueuePagamentoNovo(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONSULTA_PAGAMENTO copa = pagApp.GetItemById(idNot);

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return 1;
                }

                // Valida Nome
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                Session["CompAnexo"] = fileName;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + idNot.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                PAGAMENTO_ANEXO foto = new PAGAMENTO_ANEXO();
                foto.PAAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAAN_DT_ANEXO = DateTime.Today;
                foto.PAAN_IN_ATIVO = 1;
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
                foto.PAAN_IN_TIPO = tipo;
                foto.PAAN_NM_TITULO = fileName;
                foto.ASSI_CD_ID = idAss;
                foto.COPA_CD_ID = copa.COPA_CD_ID;
                copa.PAGAMENTO_ANEXO.Add(foto);
                objetoAntesPag = copa;
                Int32 volta = pagApp.ValidateEdit(copa, objetoAntesPag);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + copa.COPA_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);
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
                return 9;
            }
        }

        public async Task<Int32> UploadFileQueuePagamentoNovoBlob(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                CONSULTA_PAGAMENTO copa = pagApp.GetItemById(idNot);

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return 1;
                }

                // Valida Nome
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                Session["CompAnexo"] = fileName;
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return 4;
                }

                // 1. DEFINIÇÃO DE CAMINHOS
                String caminhoRelativo = "Imagens/" + idAss.ToString() + "/Pagamento/" + copa.COPA_CD_ID.ToString() + "/Anexos/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                // Garante que a pasta local existe
                if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                // 2. CÓPIA LOCAL (Escrita de Bytes)
                System.IO.File.WriteAllBytes(fullPathLocal, file.Contents);

                // 3. CÓPIA PARA O AZURE BLOB STORAGE
                try
                {
                    CONFIGURACAO conf = CarregaConfiguracaoGeral();
                    string connString = conf.CONF_NM_STORAGE_CONN;
                    string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                    var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    // O nome do blob no Azure
                    string blobName = caminhoRelativo + fileName;
                    var blobClient = containerClient.GetBlobClient(blobName);

                    // Como file.Contents é byte[], usamos MemoryStream para o upload
                    using (var ms = new MemoryStream(file.Contents))
                    {
                        await blobClient.UploadAsync(ms, overwrite: true);
                    }
                }
                catch (Exception exAzure)
                {
                    Session["MsgCRUD"] = "Erro na sincronização: " + exAzure.Message;
                    Session["MensPaciente"] = 61;
                    return 0;
                }

                // Gravar registro
                PAGAMENTO_ANEXO foto = new PAGAMENTO_ANEXO();
                foto.PAAN_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.PAAN_DT_ANEXO = DateTime.Today;
                foto.PAAN_IN_ATIVO = 1;
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
                foto.PAAN_IN_TIPO = tipo;
                foto.PAAN_NM_TITULO = fileName;
                foto.ASSI_CD_ID = idAss;
                foto.COPA_CD_ID = copa.COPA_CD_ID;
                copa.PAGAMENTO_ANEXO.Add(foto);
                objetoAntesPag = copa;
                Int32 volta = pagApp.ValidateEdit(copa, objetoAntesPag);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + copa.COPA_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);
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
                return 9;
            }
        }

        public ActionResult VoltarAnexoPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarPagamento", new { id = (Int32)Session["IdPagamento"] });
        }

        public ActionResult VoltarAnexoValorConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("EditarValorConsulta", new { id = (Int32)Session["IdValorConsulta"] });
        }

        public ActionResult VoltarAnexoPagamentoVer()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaTela"] = 1;
            return RedirectToAction("VerPagamento", new { id = (Int32)Session["IdPagamento"] });
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
            Session["FileQueuePagamento"] = queue;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFilePagamento(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                //file.SaveAs(path);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                // Gravar registro
                PAGAMENTO_ANEXO foto = new PAGAMENTO_ANEXO();
                foto.ASSI_CD_ID = idAss;
                foto.PAAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PAAN_DT_ANEXO = DateTime.Today.Date;
                foto.PAAN_IN_ATIVO = 1;
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
                foto.PAAN_IN_TIPO = tipo;
                foto.PAAN_IN_ATIVO = 1;
                foto.PAAN_NM_TITULO = fileName;
                foto.COPA_CD_ID = item.COPA_CD_ID;
                item.PAGAMENTO_ANEXO.Add(foto);
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.COPA_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPagamento"] = 2;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
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

        public async Task<ActionResult> UploadFilePagamentoBlob(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera exame
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Critica tamanho nome
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensPaciente"] = 6;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensPaciente"] = 7;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensPaciente"] = 12;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
                // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
                String caminhoRelativo = "Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/Anexos/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                // Garante que a pasta local existe
                if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                // 2. CÓPIA LOCAL
                using (var stream = new FileStream(fullPathLocal, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

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
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Gravar registro
                PAGAMENTO_ANEXO foto = new PAGAMENTO_ANEXO();
                foto.ASSI_CD_ID = idAss;
                foto.PAAN_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.PAAN_DT_ANEXO = DateTime.Today.Date;
                foto.PAAN_IN_ATIVO = 1;
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
                foto.PAAN_IN_TIPO = tipo;
                foto.PAAN_IN_ATIVO = 1;
                foto.PAAN_NM_TITULO = fileName;
                foto.COPA_CD_ID = item.COPA_CD_ID;
                item.PAGAMENTO_ANEXO.Add(foto);
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.COPA_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPagamento"] = 2;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
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
        public async Task<ActionResult> UploadFileNotaFiscal(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera pagamento
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null || file.ContentLength == 0)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Lê e processa o XML recém-salvo
                XDocument xmlDoc = XDocument.Load(file.InputStream);

                var ide = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "ide");
                var emit = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "emit");
                var total = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "total");

                String numero = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "nNF")?.Value;
                String serie = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "serie")?.Value;
                String dataEmissao = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "dhEmi")?.Value;
                String emitente = emit?.Elements().FirstOrDefault(e => e.Name.LocalName == "xNome")?.Value;
                String valorTotal = total?.Descendants().FirstOrDefault(e => e.Name.LocalName == "vNF")?.Value;

                DateTime? dataEmissaoParsed = null;
                if (DateTime.TryParse(dataEmissao, out DateTime dt))
                    dataEmissaoParsed = dt;

                decimal? valorTotalParsed = null;
                if (decimal.TryParse(valorTotal, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal vl))
                    valorTotalParsed = vl;

                // Recupera arquivos
                var fileName = Path.GetFileName(file.FileName);
                var fileSize = file.ContentLength;
                extensao = Path.GetExtension(fileName);

                // Criticas
                if (item.COPA_VL_VALOR != valorTotalParsed)
                {
                    Session["MensPaciente"] = 55;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Monta nome
                String nome = "Nota Fiscal Num: ";
                nome += numero + " - Série: " + serie + " - Data: " + dataEmissaoParsed.Value.ToLongDateString() + " - Emissor: " + emitente;

                // Checa duplicidade
                List<PAGAMENTO_NOTA_FISCAL> notas = item.PAGAMENTO_NOTA_FISCAL.ToList();
                Int32 temNota = notas.Where(p => p.PANF_NM_NOME == nome & p.PANF_VL_VALOR == valorTotalParsed).ToList().Count;
                if (temNota > 0)
                {
                    Session["MensPaciente"] = 56;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/NF/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                //file.SaveAs(path);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                // Gravar registro
                PAGAMENTO_NOTA_FISCAL foto = new PAGAMENTO_NOTA_FISCAL();
                foto.PANF_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PANF_DT_EMISSAO = dataEmissaoParsed;
                foto.PANF_IN_ATIVO = 1;
                foto.PANF_NM_EMITENTE = emitente;
                foto.PANF_NR_NUMERO = numero;
                foto.PANF_NM_NOME = nome;
                foto.PANF_NR_SERIE = serie;
                foto.PANF_VL_VALOR = valorTotalParsed;
                foto.COPA_CD_ID = item.COPA_CD_ID;
                item.PAGAMENTO_NOTA_FISCAL.Add(foto);
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Nota Fiscal - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.COPA_NM_NOME.ToUpper() + " | Nota: " + numero + " | Nome: " + nome.ToUpper() + " | Emitente: " + emitente.ToUpper() + " | Valor: " + valorTotalParsed.ToString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPagamento"] = 4;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;   
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileNotaFiscalBlob(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdPagamento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera pagamento
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null || file.ContentLength == 0)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Lê e processa o XML recém-salvo
                XDocument xmlDoc = XDocument.Load(file.InputStream);

                var ide = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "ide");
                var emit = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "emit");
                var total = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "total");

                String numero = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "nNF")?.Value;
                String serie = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "serie")?.Value;
                String dataEmissao = ide?.Elements().FirstOrDefault(e => e.Name.LocalName == "dhEmi")?.Value;
                String emitente = emit?.Elements().FirstOrDefault(e => e.Name.LocalName == "xNome")?.Value;
                String valorTotal = total?.Descendants().FirstOrDefault(e => e.Name.LocalName == "vNF")?.Value;

                DateTime? dataEmissaoParsed = null;
                if (DateTime.TryParse(dataEmissao, out DateTime dt))
                    dataEmissaoParsed = dt;

                decimal? valorTotalParsed = null;
                if (decimal.TryParse(valorTotal, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal vl))
                    valorTotalParsed = vl;

                // Recupera arquivos
                var fileName = Path.GetFileName(file.FileName);
                var fileSize = file.ContentLength;
                extensao = Path.GetExtension(fileName);

                // Criticas
                if (item.COPA_VL_VALOR != valorTotalParsed)
                {
                    Session["MensPaciente"] = 55;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // Monta nome
                String nome = "Nota Fiscal Num: ";
                nome += numero + " - Série: " + serie + " - Data: " + dataEmissaoParsed.Value.ToLongDateString() + " - Emissor: " + emitente;

                // Checa duplicidade
                List<PAGAMENTO_NOTA_FISCAL> notas = item.PAGAMENTO_NOTA_FISCAL.ToList();
                Int32 temNota = notas.Where(p => p.PANF_NM_NOME == nome & p.PANF_VL_VALOR == valorTotalParsed).ToList().Count;
                if (temNota > 0)
                {
                    Session["MensPaciente"] = 56;
                    return RedirectToAction("VoltarAnexoPagamento");
                }

                // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
                // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
                String caminhoRelativo = "Imagens/" + idAss.ToString() + "/Pagamento/" + item.COPA_CD_ID.ToString() + "/NF/";
                String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                // Garante que a pasta local existe
                if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                // 2. CÓPIA LOCAL
                using (var stream = new FileStream(fullPathLocal, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

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
                    return RedirectToAction("VoltarAnexoPaciente");
                }

                // Gravar registro
                PAGAMENTO_NOTA_FISCAL foto = new PAGAMENTO_NOTA_FISCAL();
                foto.PANF_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.PANF_DT_EMISSAO = dataEmissaoParsed;
                foto.PANF_IN_ATIVO = 1;
                foto.PANF_NM_EMITENTE = emitente;
                foto.PANF_NR_NUMERO = numero;
                foto.PANF_NM_NOME = nome;
                foto.PANF_NR_SERIE = serie;
                foto.PANF_VL_VALOR = valorTotalParsed;
                foto.COPA_CD_ID = item.COPA_CD_ID;
                item.PAGAMENTO_NOTA_FISCAL.Add(foto);
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Nota Fiscal - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.COPA_NM_NOME.ToUpper() + " | Nota: " + numero + " | Nome: " + nome.ToUpper() + " | Emitente: " + emitente.ToUpper() + " | Valor: " + valorTotalParsed.ToString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPagamento"] = 4;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;   
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileRecibo(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdRecebimento"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera recebimento
                CONSULTA_RECEBIMENTO item = recApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];

                // Criticas
                if (file == null || file.ContentLength == 0)
                {
                    Session["MensPaciente"] = 5;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Lê e processa o XML recém-salvo
                XDocument xmlDoc = XDocument.Load(file.InputStream);
                XNamespace ns = "https://www.gov.br/receita-federal/recibos/consulta-medica/v1";

                var identificacao = xmlDoc.Descendants(ns + "IdentificacaoRecibo").FirstOrDefault();
                var paciente = xmlDoc.Descendants(ns + "Paciente").FirstOrDefault();
                var profissional = xmlDoc.Descendants(ns + "Profissional").FirstOrDefault();

                String numeroRecibo = identificacao?.Element(ns + "NumeroRecibo")?.Value;
                String dataEmissaoStr = identificacao?.Element(ns + "DataEmissao")?.Value;
                String valorStr = identificacao?.Element(ns + "Valor")?.Value;

                String nomePaciente = paciente?.Element(ns + "Nome")?.Value;
                String cpfPaciente = paciente?.Element(ns + "CPF")?.Value;

                String nomeProfissional = profissional?.Element(ns + "Nome")?.Value;
                String cpfProfissional = profissional?.Element(ns + "CPF")?.Value;
                String crm = profissional?.Element(ns + "CRM")?.Element(ns + "Numero")?.Value;
                String ufCrm = profissional?.Element(ns + "CRM")?.Element(ns + "UF")?.Value;

                DateTime dataEmissao = DateTime.MinValue;
                DateTime.TryParse(dataEmissaoStr, out dataEmissao);

                Decimal valor = 0;
                Decimal.TryParse(valorStr, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);

                // Recupera arquivos
                var fileName = Path.GetFileName(file.FileName);
                var fileSize = file.ContentLength;
                extensao = Path.GetExtension(fileName);

                // Criticas
                if (item.CORE_VL_VALOR != valor)
                {
                    Session["MensPaciente"] = 55;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Monta nome
                String nome = "Recibo Num: ";
                nome += numeroRecibo + " - Data: " + dataEmissao.ToLongDateString() + " - Paciente: " + nomePaciente;

                // Checa duplicidade
                List<RECEBIMENTO_RECIBO> notas = item.RECEBIMENTO_RECIBO.ToList();
                Int32 temNota = notas.Where(p => p.RERC_NM_NOME == nome & p.RERC_VL_VALOR == valor).ToList().Count;
                if (temNota > 0)
                {
                    Session["MensPaciente"] = 56;
                    return RedirectToAction("VoltarAnexoRecebimento");
                }

                // Copia arquivo
                String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + item.CORE_CD_ID.ToString() + "/Recibo/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                // Gravar registro
                RECEBIMENTO_RECIBO foto = new RECEBIMENTO_RECIBO();
                foto.RERC_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.RERC_DT_EMISSAO = dataEmissao;
                foto.RERC_IN_ATIVO = 1;
                foto.RERC_NR_NUMERO = numeroRecibo;
                foto.RERC_VL_VALOR = valor;
                foto.RERC_NM_PACIENTE = nomePaciente;
                foto.RERC_NR_CPF = cpfPaciente;
                foto.RERC_NM_MEDICO = nomeProfissional;
                foto.RERC_NR_CPF_MEDICO = cpfProfissional;
                foto.RERC_NR_CLASSE = crm;
                foto.RERC_SG_UF = ufCrm;
                foto.CORE_CD_ID = item.CORE_CD_ID;
                item.RECEBIMENTO_RECIBO.Add(foto);
                Int32 volta = recApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Recibo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recebimento: " + item.CORE_NM_RECEBIMENTO.ToUpper() + " | Recibo: " + numeroRecibo + " | Nome: " + nome + " | Emitente: " + nomePaciente + " | Valor: " + valor.ToString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelRecebimento"] = 4;
                Session["RecebimentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoRecebimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;   
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoPagamento(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_ANEXO item = pagApp.GetAnexoById(id);
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 2;
                Session["ModuloAtual"] = "Financeiro - Pagamentos - Anexos";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_ANEXO", "Financeiro", "VerAnexoPagamento");
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
        public ActionResult VerAnexoPagamentoAudio(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_ANEXO item = pagApp.GetAnexoById(id);
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 2;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_ANEXO", "Financeiro", "VerAnexoPagamento");
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

        public FileResult DownloadPagamento(Int32 id)
        {
            try
            {
                PAGAMENTO_ANEXO item = pagApp.GetAnexoById(id);
                String arquivo = item.PAAN_AQ_ARQUIVO;
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
                else if (arquivo.Contains(".mpeg"))
                {
                    contentType = "audio/mpeg";
                }
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 2;
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

        public FileResult DownloadNotaFiscal(Int32 id)
        {
            try
            {
                PAGAMENTO_NOTA_FISCAL item = pagApp.GetNotaById(id);
                String arquivo = item.PANF_AQ_ARQUIVO;
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
                else if (arquivo.Contains(".xml"))
                {
                    contentType = "application/xml";
                }
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 4;
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public FileResult DownloadRecibo(Int32 id)
        {
            try
            {
                RECEBIMENTO_RECIBO item = recApp.GetReciboById(id);
                String arquivo = item.RERC_AQ_ARQUIVO;
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
                else if (arquivo.Contains(".xml"))
                {
                    contentType = "application/xml";
                }
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 4;
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexoPagamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_ANEXO item = pagApp.GetAnexoById(id);
                CONSULTA_PAGAMENTO pac = pagApp.GetItemById(item.COPA_CD_ID);

                item.PAAN_IN_ATIVO = 0;
                Int32 volta = pagApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.PAAN_NM_TITULO.ToUpper() + " | Anexo: " + item.PAAN_NM_TITULO.ToUpper() + " | Data: " + item.PAAN_DT_ANEXO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 2;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
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
        public ActionResult ExcluirNotaFiscalPagamento(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_NOTA_FISCAL item = pagApp.GetNotaById(id);
                CONSULTA_PAGAMENTO pac = pagApp.GetItemById(item.COPA_CD_ID);

                item.PANF_IN_ATIVO = 0;
                Int32 volta = pagApp.ValidateEditNota(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Nota Fiscal - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + pac.COPA_NM_NOME.ToUpper() + "Nota: " + item.PANF_NR_NUMERO + " | Emitente: " + item.PANF_NM_EMITENTE.ToUpper() + " | Data: " + item.PANF_DT_EMISSAO.Value.ToShortDateString() + " | Valor: " + item.PANF_VL_VALOR.ToString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 4;
                Session["PagamentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirRecibo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                RECEBIMENTO_RECIBO item = recApp.GetReciboById(id);
                CONSULTA_RECEBIMENTO pac = recApp.GetItemById(item.CORE_CD_ID);

                item.RERC_IN_ATIVO = 0;
                Int32 volta = recApp.ValidateEditRecibo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Recebimento - Recibo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Recibo: " + item.RERC_NR_NUMERO + " | Emitente: " + item.RERC_NM_PACIENTE + " | Data: " + item.RERC_DT_EMISSAO.Value.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["NivelPaciente"] = 1;
                Session["NivelRecebimento"] = 4;
                Session["RecebimentoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoRecebimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAnotacaoPagamento()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Alteração";
                        return RedirectToAction("VoltarAnexoPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 3;

                CONSULTA_PAGAMENTO item = pagApp.GetItemById((Int32)Session["IdPagamento"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_ANOTACAO coment = new PAGAMENTO_ANOTACAO();
                PagamentoAnotacaoViewModel vm = Mapper.Map<PAGAMENTO_ANOTACAO, PagamentoAnotacaoViewModel>(coment);
                vm.PGAN_DT_ANOTACAO = DateTime.Now;
                vm.PGAN_IN_ATIVO = 1;
                vm.COPA_CD_ID = item.COPA_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_ANOTACAO_INCLUIR", "Financeiro", "IncluirAnotacaoPagamento");
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
        public ActionResult IncluirAnotacaoPagamento(PagamentoAnotacaoViewModel vm)
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
                    vm.PGAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PGAN_TX_ANOTACAO);

                    // Executa a operação
                    PAGAMENTO_ANOTACAO item = Mapper.Map<PagamentoAnotacaoViewModel, PAGAMENTO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONSULTA_PAGAMENTO not = pagApp.GetItemById((Int32)Session["IdPagamento"]);

                    item.USUARIO = null;
                    not.PAGAMENTO_ANOTACAO.Add(item);
                    Int32 volta = pagApp.ValidateEdit(not, not, usuarioLogado);

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Pagamento_Anotacao dto = MontarPagamentoAnotacaoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Pagamento - Anotação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Acerta pagamento
                    CONSULTA_PAGAMENTO pag = pagApp.GetItemById((Int32)Session["IdPagamento"]);
                    pag.TIPA_CD_ID = (Int32)Session["TipoPagamentoId"];
                    Int32 voltaP = pagApp.ValidateEdit(pag, pag, usuarioLogado);

                    // Sucesso
                    Session["NivelPaciente"] = 1;
                    Session["NivelPagamento"] = 3;
                    Session["VoltarPesquisa"] = 0;
                    Int32 s = (Int32)Session["VoltarPesquisa"];
                    return RedirectToAction("VoltarAnexoPagamento");
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
        public ActionResult EditarAnotacaoPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Alteração";
                        return RedirectToAction("VoltarAnexoPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 3;
                PAGAMENTO_ANOTACAO item = pagApp.GetAnotacaoById(id);
                PagamentoAnotacaoViewModel vm = Mapper.Map<PAGAMENTO_ANOTACAO, PagamentoAnotacaoViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_ANOTACAO_EDITAR", "Financeiro", "EditarAnotacaoPagamento");
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
        public ActionResult EditarAnotacaoPagamento(PagamentoAnotacaoViewModel vm)
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
                    vm.PGAN_TX_ANOTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PGAN_TX_ANOTACAO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PAGAMENTO_ANOTACAO item = Mapper.Map<PagamentoAnotacaoViewModel, PAGAMENTO_ANOTACAO>(vm);
                    CONSULTA_PAGAMENTO copa = pagApp.GetItemById(item.COPA_CD_ID);
                    Int32 volta = pagApp.ValidateEditAnotacao(item);

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Pagamento_Anotacao dto = MontarPagamentoAnotacaoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Pagamento - Anotação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PagamentoAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["NivelPagamento"] = 3;
                    return RedirectToAction("VoltarAnexoPagamento");
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
        public ActionResult ExcluirAnotacaoPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Alteração";
                        return RedirectToAction("VoltarAnexoPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PAGAMENTO_ANOTACAO item = pagApp.GetAnotacaoById(id);
                item.PGAN_IN_ATIVO = 0;
                Int32 volta = pagApp.ValidateEditAnotacao(item);
                Session["PagamentoAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["NivelPagamento"] = 3;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Pagamento_Anotacao dto = MontarPagamentoAnotacaoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Anotação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoPagamento");
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

        public JsonResult GetPagamento(Int32 id)
        {
            var tp = tpApp.GetItemById(id);
            var hash = new Hashtable();
            String nome = "Pagamento de " + tp.TIPA_NM_PAGAMENTO + " em " + DateTime.Today.Date.ToLongDateString();            
            hash.Add("pagamento", nome);
            return Json(hash);
        }

        public JsonResult GetPaciente(Int32 id)
        {
            var pac = pacApp.GetItemById(id);
            var hash = new Hashtable();
            String nome = "Recebimento de " + pac.PACI_NM_NOME.ToUpper() + " em " + DateTime.Today.Date.ToLongDateString() + " referente a consulta";
            hash.Add("nome", nome);
            return Json(hash);
        }

        public JsonResult GetConsulta(Int32 id)
        {
            var pac = vcApp.GetItemById(id);
            var hash = new Hashtable();
            Decimal? valor = pac.VACO_NR_VALOR;
            hash.Add("valor", valor);
            return Json(hash);
        }

        [HttpGet]
        public ActionResult ConsolidarPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_PAG_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Pagamento - Consolidação";
                        return RedirectToAction("MontarTelaPagamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera dados e confirma
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONSULTA_PAGAMENTO item = pagApp.GetItemById(id);
                item.COPA_IN_CONFERIDO = 1;
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Acerta estado
                Session["PagamentoAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaPagamento"] = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "ccoCOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Pagamento: " + item.COPA_NM_NOME + " | Data: " + item.COPA_DT_PAGAMENTO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O pagamento " + item.COPA_NM_NOME.ToUpper() + " efetuado em " + item.COPA_DT_PAGAMENTO.Value.ToLongDateString() + " foi consolidado com sucesso";
                Session["MensPaciente"] = 888;

                // Retorno
                return RedirectToAction("MontarTelaPagamento");
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

        public List<PACIENTE> CarregaPaciente()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE> conf = new List<PACIENTE>();
                if (Session["Pacientes"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["PacienteAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PACIENTE_CONSULTA> CarregaConsultas()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PACIENTE_CONSULTA> conf = new List<PACIENTE_CONSULTA>();
                if (Session["Consultas"] == null)
                {
                    conf = baseApp.GetAllConsultas(idAss);
                }
                else
                {
                    if ((Int32)Session["ConsultasAlterada"] == 1)
                    {
                        conf = baseApp.GetAllConsultas(idAss);
                    }
                    else
                    {
                        conf = (List<PACIENTE_CONSULTA>)Session["Consultas"];
                    }
                }
                Session["Consultas"] = conf;
                Session["ConsultasAlterada"] = 0;
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

        public List<VALOR_CONSULTA> CarregaTipoConsulta()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<VALOR_CONSULTA> conf = new List<VALOR_CONSULTA>();
                if (Session["TipoValorConsultas"] == null)
                {
                    conf = recApp.GetAllValorConsulta(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoValorConsultaAlterada"] == 1)
                    {
                        conf = recApp.GetAllValorConsulta(idAss);
                    }
                    else
                    {
                        conf = (List<VALOR_CONSULTA>)Session["TipoValorConsultas"];
                    }
                }
                Session["TipoValorConsultas"] = conf;
                Session["TipoValorConsultaAlterada"] = 0;
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

        public ActionResult GerarListagemPagamento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoLista" + "_" + data + ".pdf";
                List<CONSULTA_PAGAMENTO> lista = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    lista = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    lista = CarregaPagamento().ToList();
                }
                lista = lista.OrderBy(p => p.COPA_DT_VENCIMENTO).ToList();

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
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Pagamentos", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 80f, 200f, 100f, 60f, 60f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Favorecido", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quitado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_PAGAMENTO item in lista)
                {
                    if (item.COPA_DT_VENCIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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

                    if (item.COPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_FAVORECIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_PAGO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.COPA_IN_PAGO == 1)
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

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoVenceHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoListaVencimentoHoje" + "_" + data + ".pdf";
                List<CONSULTA_PAGAMENTO> lista = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_VENCIMENTO.Value.Date == DateTime.Today.Date).ToList();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? total = 0;
                Decimal? totalPago = 0;

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Vencimento em " + DateTime.Today.Date.ToLongDateString(), meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Vencimento em " + DateTime.Today.Date.ToLongDateString(), meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Favorecido", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quitado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_PAGAMENTO item in lista)
                {
                    if (item.COPA_DT_VENCIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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

                    if (item.COPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_FAVORECIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_PAGO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.COPA_IN_PAGO == 1)
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

                    total += item.COPA_VL_VALOR;
                    totalPago += item.COPA_VL_PAGO;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 5;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPago.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoVenceMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }


                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoListaVenceMes" + "_" + data + ".pdf";
                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Month) + " de " + DateTime.Today.Year.ToString();
                List<CONSULTA_PAGAMENTO> lista = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Month & p.COPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Year).OrderBy(p => p.COPA_DT_VENCIMENTO).ToList();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? total = 0;
                Decimal? totalPago = 0;

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Vencimento em " + mes, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Vencimento em " + mes, meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Favorecido", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quitado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_PAGAMENTO item in lista)
                {
                    if (item.COPA_DT_VENCIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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

                    if (item.COPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_FAVORECIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_PAGO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.COPA_IN_PAGO == 1)
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

                    total += item.COPA_VL_VALOR;
                    totalPago += item.COPA_VL_PAGO;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 5;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPago.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemRecebimento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "RecebimentoLista" + "_" + data + ".pdf";
                List<CONSULTA_RECEBIMENTO> lista = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    lista = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    lista = CarregaRecebimento().ToList();
                }
                lista = lista.OrderBy(p => p.CORE_DT_RECEBIMENTO).ToList();

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);

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

                    cell1 = new PdfPCell(new Paragraph("Recebimentos", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Recebimentos", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 100f, 160f, 160f, 70f, 70f });
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
                cell = new PdfPCell(new Paragraph("Forma de Recebimento", meuFont))
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
                cell = new PdfPCell(new Paragraph("Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Recebimento", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_RECEBIMENTO item in lista)
                {
                    if (item.CORE_DT_RECEBIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.CORE_DT_RECEBIMENTO.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.FORMA_RECEBIMENTO.FORE_NM_FORMA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CORE_GU_GUID, meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.CORE_NM_RECEBIMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.VALOR_CONSULTA.VACO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.CORE_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
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

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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
        public ActionResult MontarTelaFinanceiro()
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
                List<CONSULTA_PAGAMENTO> pagtos = new List<CONSULTA_PAGAMENTO>();
                List<CONSULTA_RECEBIMENTO> rectos = new List<CONSULTA_RECEBIMENTO>();
                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    pagtos = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1).ToList();
                    rectos = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1).ToList();
                }
                else
                {
                    pagtos = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    rectos = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Date.Month);          
                ViewBag.MesCorrente = mes + " de " + DateTime.Today.Date.Year.ToString();
                DateTime limite = DateTime.Today.Date.AddMonths(-12);

                // Carrega widgets
                List<CONSULTA_PAGAMENTO> jaPagtos = pagtos.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<CONSULTA_PAGAMENTO> pagtosMes = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Date.Year & p.COPA_IN_PAGO == 1).ToList();
                List<CONSULTA_PAGAMENTO> pagtosAno = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Date.Year & p.COPA_IN_PAGO == 1).ToList();
                List<CONSULTA_PAGAMENTO> vencHoje = pagtos.Where(p => p.COPA_DT_VENCIMENTO == DateTime.Today.Date & p.COPA_IN_PAGO == 0).ToList();
                Decimal? pagMes = pagtosMes.Sum(p => p.COPA_VL_PAGO);
                Decimal? pagAno = pagtosAno.Sum(p => p.COPA_VL_PAGO);
                Decimal? vencDia = vencHoje.Sum(p => p.COPA_VL_VALOR);
                ViewBag.PagtosMes = pagMes;
                ViewBag.PagtosAno = pagAno;
                ViewBag.VencimentoHoje = vencDia;

                List<CONSULTA_RECEBIMENTO> jaRectos = rectos.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<CONSULTA_RECEBIMENTO> rectosMes = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                List<CONSULTA_RECEBIMENTO> rectosAno = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                Decimal? recMes = rectosMes.Sum(p => p.CORE_VL_VALOR);
                Decimal? recAno = rectosAno.Sum(p => p.CORE_VL_VALOR);
                ViewBag.RectosMes = recMes;
                ViewBag.RectosAno = recAno;

                // Lista - Pagamentos e recebimentos no mês
                ViewBag.PagamentosMes = pagtosMes;
                ViewBag.RecebimentosMes = rectosMes;

                // Médias diaria por mes - Pagamentos
                List<DateTime> datas = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Year).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["PagamentoAlterada"] == 1 || Session["ListaPagtoMes"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date & p.COPA_IN_PAGO == 1).Count();
                        Decimal? soma = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date  & p.COPA_IN_PAGO == 1).Sum(p => p.COPA_VL_PAGO);
                        Decimal? media = 0;
                        if (conta > 0)
                        {
                            media = soma / conta;
                        }
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = media.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaMediaPagtoMes = lista;
                    Session["ListaMediaPagtoMes"] = lista;
                }
                else
                {
                    ViewBag.ListaMediaPagtoMes = (List<ModeloViewModel>)Session["ListaMediaPagtoMes"];
                }

                // Médias diaria por mes - Recebimentos
                datas = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Year).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["RecebimentoAlterada"] == 1 || Session["ListaRectoMes"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Count();
                        Decimal? soma = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Sum(p => p.CORE_VL_VALOR);
                        Decimal? media = 0;
                        if (conta > 0)
                        {
                            media = soma / conta;
                        }
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = media.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaMediaRectoMes = lista;
                    Session["ListaMediaRectoMes"] = lista;
                }
                else
                {
                    ViewBag.ListaMediaRectoMes = (List<ModeloViewModel>)Session["ListaMediaRectoMes"];
                }

                // Pagamentos por dia - Mes corrente
                List<ModeloViewModel> listaPagRec = new List<ModeloViewModel>();
                List<ModeloViewModel> listaPagRecMes = new List<ModeloViewModel>();
                datas = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Year).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["PagamentoAlterada"] == 1 || Session["ListaPagtoMes"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date & p.COPA_IN_PAGO == 1).Count();
                        Decimal? soma = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date & p.COPA_IN_PAGO == 1).Sum(p => p.COPA_VL_PAGO);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = soma.Value;
                        lista.Add(mod);

                        ModeloViewModel modPag = new ModeloViewModel();
                        modPag.DataEmissao = item;
                        modPag.Valor = 1;
                        modPag.ValorDec = soma.Value;
                        listaPagRec.Add(modPag);
                    }
                    ViewBag.ListaPagtoMes = lista;
                    Session["ListaPagtoMes"] = lista;
                }
                else
                {
                    ViewBag.ListaPagtoMes = (List<ModeloViewModel>)Session["ListaPagtoMes"];
                }

                // Recebimentos por dia - Mes corrente
                datas = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Year).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["RecebimentoAlterada"] == 1 || Session["ListaRectoMes"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Count();
                        Decimal? soma = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Sum(p => p.CORE_VL_VALOR);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = soma.Value;
                        lista.Add(mod);

                        ModeloViewModel modPag = new ModeloViewModel();
                        modPag.DataEmissao = item;
                        modPag.Valor = 2;
                        modPag.ValorDec = soma.Value;
                        listaPagRec.Add(modPag);
                    }
                    ViewBag.ListaRectoMes = lista;
                    Session["ListaRectoMes"] = lista;
                }
                else
                {
                    ViewBag.ListaRectoMes = (List<ModeloViewModel>)Session["ListaRectoMes"];
                }

                // Resumo Mensal Pagamentos
                List<DateTime> datasPagto = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO != null).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                datasPagto.Sort((i, j) => i.Date.CompareTo(j.Date));
                if ((Int32)Session["PagamentoAlterada"] == 1 || Session["ListaPagtosMes"] == null)
                {
                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    String mes2 = null;
                    String mesFeito2 = null;
                    foreach (DateTime item in datasPagto)
                    {
                        if (item.Date > limite)
                        {
                            mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes2 != mesFeito2)
                            {
                                Decimal conta = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.COPA_DT_PAGAMENTO > limite & p.COPA_IN_PAGO == 1).Sum(p => p.COPA_VL_PAGO.Value);
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes2;
                                mod.ValorDec1 = conta;
                                listaMes.Add(mod);
                                mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();

                                ModeloViewModel modPag = new ModeloViewModel();
                                modPag.Nome = mes2;
                                modPag.Valor = 1;
                                modPag.ValorDec1 = conta;
                                listaPagRecMes.Add(modPag);
                            }
                        }
                    }

                    mes2 = null;
                    mesFeito2 = null;
                    ViewBag.ListaPagtosMes = listaMes;
                    Session["ListaDatasPagtosMes"] = datasPagto;
                    Session["ListaPagtosMes"] = listaMes;
                    Session["ListaRecebimento"] = null;
                    Session["ListaPagamento"] = null;
                }
                else
                {
                    ViewBag.ListaPagtosMes = (List<ModeloViewModel>)Session["ListaPagtosMes"];
                }

                // Resumo Mensal Recebimentos
                List<DateTime> datasRecto = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO != null).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
                datasRecto.Sort((i, j) => i.Date.CompareTo(j.Date));
                if ((Int32)Session["RecebimentoAlterada"] == 1 || Session["ListaRectosMes"] == null)
                {
                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    String mes2 = null;
                    String mesFeito2 = null;
                    foreach (DateTime item in datasRecto)
                    {
                        if (item.Date > limite)
                        {
                            mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes2 != mesFeito2)
                            {
                                Decimal conta = jaRectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Month == item.Month & p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year & p.CORE_DT_RECEBIMENTO > limite).Sum(p => p.CORE_VL_VALOR.Value);
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes2;
                                mod.ValorDec1 = conta;
                                listaMes.Add(mod);
                                mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();

                                ModeloViewModel modPag = new ModeloViewModel();
                                modPag.Nome = mes2;
                                modPag.Valor = 2;
                                modPag.ValorDec1 = conta;
                                listaPagRecMes.Add(modPag);
                            }
                        }
                    }

                    mes2 = null;
                    mesFeito2 = null;
                    ViewBag.ListaRectosMes = listaMes;
                    Session["ListaDatasRectosMes"] = datasRecto;
                    Session["ListaRectosMes"] = listaMes;
                }
                else
                {
                    ViewBag.ListaRectosMes = (List<ModeloViewModel>)Session["ListaRectosMes"];
                }

                // Resumo Pagamento x Tipo
                List<CONSULTA_PAGAMENTO> pagTipo = pagtos.Where(p => p.TIPA_CD_ID != null).ToList();
                List<Int32?> tipos = pagTipo.Where(p => p.COPA_IN_ATIVO == 1).Select(p => p.TIPA_CD_ID).Distinct().ToList();
                List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
                foreach (Int32 item in tipos)
                {
                    TIPO_PAGAMENTO tp = tpApp.GetItemById(item);
                    Int32 conta1 = pagTipo.Where(p => p.TIPA_CD_ID == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = tp.TIPA_NM_PAGAMENTO;
                    mod1.Valor = conta1;
                    lista2.Add(mod1);
                }
                ViewBag.ListaPagtoTipo= lista2;
                ViewBag.ListaPagtoTipoConta = lista2.Count;
                Session["ListaPagtoTipo"] = lista2;


                // Resumo Recebimento x Tipo  
                List<CONSULTA_RECEBIMENTO> recTipo = rectos.Where(p => p.VACO_CD_ID != null).ToList();
                tipos = recTipo.Where(p => p.CORE_IN_ATIVO == 1).Select(p => p.VACO_CD_ID).Distinct().ToList();
                List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
                foreach (Int32 item in tipos)
                {
                    VALOR_CONSULTA vc = vcApp.GetItemById(item);
                    Int32 conta1 = recTipo.Where(p => p.VACO_CD_ID == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = vc.VACO_NM_NOME;
                    mod1.Valor = conta1;
                    lista3.Add(mod1);
                }
                ViewBag.ListaRectoTipo = lista3;
                ViewBag.ListaRectoTipoConta = lista3.Count;
                Session["ListaRectoTipo"] = lista3;

                // Resumo Recebimento x Forma  
                List<CONSULTA_RECEBIMENTO> recs = rectos.Where(p => p.FORE_CD_ID != null).ToList();
                tipos = recs.Where(p => p.CORE_IN_ATIVO == 1).Select(p => p.FORE_CD_ID).Distinct().ToList();
                List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
                foreach (Int32 item in tipos)
                {
                    FORMA_RECEBIMENTO fr = recApp.GetFormaById(item);
                    Int32 conta1 = rectos.Where(p => p.FORE_CD_ID == item).ToList().Count;
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.Nome = fr.FORE_NM_FORMA;
                    mod1.Valor = conta1;
                    lista4.Add(mod1);
                }
                ViewBag.ListaRectoForma = lista4;
                ViewBag.ListaRectoFormaConta = lista4.Count;
                Session["ListaRectoForma"] = lista4;

                // Resumo pagamento/média x dia
                datas = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Year).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["PagamentoAlterada"] == 1 || Session["ListaPagtoMedia"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date & p.COPA_IN_PAGO == 1).Count();
                        Decimal? soma = jaPagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date & p.COPA_IN_PAGO == 1).Sum(p => p.COPA_VL_PAGO);
                        Decimal? media = 0;
                        if (conta > 0)
                        {
                            media = soma / conta;
                        }
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = soma.Value;
                        mod.ValorDec1 = media.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaPagtoMedia= lista;
                    Session["ListaPagtoMedia"] = lista;
                }
                else
                {
                    ViewBag.ListaPagtoMedia = (List<ModeloViewModel>)Session["ListaPagtoMedia"];
                }

                // Resumo recebimento/média x dia
                datas = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Year).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
                if ((Int32)Session["RecebimentoAlterada"] == 1 || Session["ListaRectoMedia"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Count();
                        Decimal? soma = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Sum(p => p.CORE_VL_VALOR);
                        Decimal? media = 0;
                        if (conta > 0)
                        {
                            media = soma / conta;
                        }
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        mod.ValorDec = soma.Value;
                        mod.ValorDec1 = media.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaRectoMedia = lista;
                    Session["ListaRectoMedia"] = lista;
                }
                else
                {
                    ViewBag.ListaRectoMedia = (List<ModeloViewModel>)Session["ListaRectoMedia"];
                }

                // Pagamento e Recebimento por data
                Session["ListaPagRec"] = listaPagRec;
                datas = listaPagRec.Where(p => p.DataEmissao.Month == DateTime.Today.Month & p.DataEmissao.Year == DateTime.Today.Year).Select(p => p.DataEmissao.Date).Distinct().ToList();
                if ((Int32)Session["RecebimentoAlterada"] == 1 || Session["ListaPagtoRecto"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Decimal? somaPag = listaPagRec.Where(p => p.DataEmissao.Date == item.Date & p.Valor == 1).Sum(p => p.ValorDec);
                        Decimal? somaRec = listaPagRec.Where(p => p.DataEmissao.Date == item.Date & p.Valor == 2).Sum(p => p.ValorDec);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.ValorDec = somaPag.Value;
                        mod.ValorDec1 = somaRec.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaPagtoRecto = lista;
                    Session["ListaPagtoRecto"] = lista;
                }
                else
                {
                    ViewBag.ListaPagtoRecto = (List<ModeloViewModel>)Session["ListaPagtoRecto"];
                }

                // Pagamento e Recebimento por mes
                Session["ListaPagRecMes"] = listaPagRecMes;
                List<String> meses = listaPagRecMes.Select(p => p.Nome).Distinct().ToList();
                if (Session["ListaPagtoRectoMes"] == null)
                {
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (String item in meses)
                    {
                        Decimal? somaPag = listaPagRecMes.Where(p => p.Nome == item & p.Valor == 1).Sum(p => p.ValorDec1);
                        Decimal? somaRec = listaPagRecMes.Where(p => p.Nome == item & p.Valor == 2).Sum(p => p.ValorDec1);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = item;
                        mod.ValorDec = somaPag.Value;
                        mod.ValorDec1 = somaRec.Value;
                        lista.Add(mod);
                    }
                    ViewBag.ListaPagtoRectoMes = lista;
                    Session["ListaPagtoRectoMes"] = lista;
                }
                else
                {
                    ViewBag.ListaPagtoRectoMes = (List<ModeloViewModel>)Session["ListaPagtoRectoMes"];
                }

                // Resumo Pagamento x Consolidado  
                List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
                Int32 conta8 = pagtos.Where(p => p.COPA_IN_CONFERIDO == 0).ToList().Count;
                ModeloViewModel mod5 = new ModeloViewModel();
                mod5.Nome = "Não consolidado";
                mod5.Valor = conta8;
                lista7.Add(mod5);
                conta8 = pagtos.Where(p => p.COPA_IN_CONFERIDO == 1).ToList().Count;
                mod5 = new ModeloViewModel();
                mod5.Nome = "Consolidado";
                mod5.Valor = conta8;
                lista7.Add(mod5);

                ViewBag.ListaPagtoConsolidado = lista7;
                ViewBag.ListaPagtoConsolidadoConta = lista7.Count;
                Session["ListaPagtoConsolidado"] = lista7;

                // Resumo Recebimento x Consolidado  
                lista7 = new List<ModeloViewModel>();
                conta8 = rectos.Where(p => p.CORE_IN_CONFERIDO == 0).ToList().Count;
                mod5 = new ModeloViewModel();
                mod5.Nome = "Não consolidado";
                mod5.Valor = conta8;
                lista7.Add(mod5);
                conta8 = rectos.Where(p => p.CORE_IN_CONFERIDO == 1).ToList().Count;
                mod5 = new ModeloViewModel();
                mod5.Nome = "Consolidado";
                mod5.Valor = conta8;
                lista7.Add(mod5);

                ViewBag.ListaRectoConsolidado = lista7;
                ViewBag.ListaRectoConsolidadoConta = lista7.Count;
                Session["ListaRectoConsolidado"] = lista7;

                // Relatorios
                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Lista de Pagamentos", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Pagamentos/Data", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Pagamentos/Mês", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Pagamentos/Ano", Value = "9" });
                relat.Add(new SelectListItem() { Text = "Pagamentos/Favorecido", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Pagamentos/Tipo", Value = "10" });
                relat.Add(new SelectListItem() { Text = "Pagtos. Vencendo Hoje", Value = "5" });
                relat.Add(new SelectListItem() { Text = "Pagtos. Vencendo no Mês", Value = "6" });
                relat.Add(new SelectListItem() { Text = "Pagtos. Quitados Hoje", Value = "7" });
                relat.Add(new SelectListItem() { Text = "Pagtos. Quitados no Mês", Value = "8" });
                relat.Add(new SelectListItem() { Text = "Lista de Recebimentos", Value = "11" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Data", Value = "12" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Mês", Value = "13" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Ano", Value = "14" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Paciente", Value = "15" });
                relat.Add(new SelectListItem() { Text = "Recebimentos/Profissional", Value = "16" });
                relat.Add(new SelectListItem() { Text = "Receita x Despesa/Data", Value = "17" });
                relat.Add(new SelectListItem() { Text = "Receita x Despesa/Mês", Value = "18" });
                relat.Add(new SelectListItem() { Text = "Receita x Despesa/Ano", Value = "19" });
                relat.Add(new SelectListItem() { Text = "Caixa - Detalhado", Value = "20" });

                int posicaoMeio = 10;
                relat.Insert(posicaoMeio, new SelectListItem()
                {
                    Text = "────────────────────",
                    Value = "0",
                    Disabled = true
                });
                posicaoMeio = 17;
                relat.Insert(posicaoMeio, new SelectListItem()
                {
                    Text = "────────────────────",
                    Value = "0",
                    Disabled = true
                });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

                // Acerta estado    
                Session["VoltaFinanceiro"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/20/Ajuda20.pdf";
                Session["NivelPagamento"] = 1;
                Session["NivelRecebimento"] = 1;
                Session["Pagamentos"] = null;
                Session["Recebimentos"] = null;
                Session["ListaPagamento"] = null;
                Session["ListaRecebimento"] = null;

                // Carrega view
                objetoPag = new CONSULTA_PAGAMENTO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "FINANCEIRO_DASHBOARD", "Financeiro", "MontarTelaDashboardFinanceiro");
                return View(objetoPag);
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

        public JsonResult GetDadosPagamentoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoMes"];
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

        public JsonResult GetDadosRecebimentoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRectoMes"];
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

        public JsonResult GetDadosPagtoTipo()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoTipo"];
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

        public JsonResult GetDadosPagtoConsolidado()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoConsolidado"];
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

        public JsonResult GetDadosRectoConsolidado()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRectoConsolidado"];
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

        public JsonResult GetDadosRectoTipo()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRectoTipo"];
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

        public JsonResult GetDadosRectoForma()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRectoForma"];
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

        public JsonResult GetDadosPagtoMedia()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoMedia"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec);
                    valor2.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("pagtos", valor1);
                result.Add("media", valor2);
                return Json(result);
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

        public JsonResult GetDadosPagtoRecto()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoRecto"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec);
                    valor2.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("pagtos", valor1);
                result.Add("rectos", valor2);
                return Json(result);
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

        public JsonResult GetDadosPagtoRectoMes()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagtoRectoMes"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.Nome);
                    valor1.Add(item.ValorDec);
                    valor2.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("pagtos", valor1);
                result.Add("rectos", valor2);
                return Json(result);
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

        public JsonResult GetDadosRectoMedia()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRectoMedia"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec);
                    valor2.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("rectos", valor1);
                result.Add("media", valor2);
                return Json(result);
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
        public ActionResult MontarTelaEncerrarConsulta()
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
                Session["ModuloAtual"] = "Consultas - Encerramento";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 62)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0586", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 63)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (Session["ListaConsultaAberta"] == null)
                {
                    List<PACIENTE_CONSULTA> cons = null;
                    if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                    {
                        cons = CarregaConsultas().Where(p => p.PACO_IN_ATIVO == 1).ToList();
                    }
                    else
                    {
                        cons = CarregaConsultas().Where(p => p.PACO_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }

                    cons = cons.Where(p => p.PACO_DT_CONSULTA.Date < DateTime.Today.Date & p.PACO_IN_CONFIRMADA == 1 & p.PACO_IN_ENCERRADA == 0).ToList();
                    cons = cons.OrderBy(p => p.PACIENTE.PACI_NM_NOME).ToList();
                    Session["ListaConsultaAberta"] = cons;
                    listaMasterPC = cons;
                }

                // Monta demais listas
                ViewBag.TipoConsulta = new SelectList(CarregaValorConsulta(), "VACO_CD_ID", "VACO_NM_NOME");
                ViewBag.Listas = (List<PACIENTE_CONSULTA>)Session["ListaConsultaAberta"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Acerta estado    
                Session["MensPaciente"] = null;
                Session["VoltaPaciente"] = 1;
                Session["NivelPaciente"] = 1;
                Session["VoltaMsg"] = 0;
                Session["TipoSolicitacao"] = 6;
                Session["VoltarConsulta"] = 4;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/5/Ajuda5_4.pdf";
                Session["ModoConsulta"] = 0;
                Session["NivelRecebimento"] = 1;
                Session["VoltaTelaEncerra"] = 1;
                Session["VoltaEncerramento"] = 5;

                // Carrega view
                ViewBag.SRF = conf.CONF_IN_RECIBO_SRF;
                objetoPC = new PACIENTE_CONSULTA();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PACIENTE_CONSULTA_ENCERRAR", "Financeiro", "MontarTelaEncerrarConsulta");
                return View(objetoPC);
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
        public ActionResult FiltrarConsultasEncerramento(PACIENTE_CONSULTA item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.PACO_TX_RESUMO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.PACO_TX_RESUMO);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<PACIENTE_CONSULTA> listaObj = new List<PACIENTE_CONSULTA>();
                Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> volta = baseApp.ExecuteFilterTupleConsulta(item.VACO_CD_ID, item.PACO_TX_RESUMO, item.PACO_DT_DUMMY, item.PACO_DT_PROXIMA, item.PACO_IN_CONFIRMADA, item.USUA_CD_ID, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensPaciente"] = 1;
                    return RedirectToAction("MontarTelaEncerrarConsulta");
                }

                // Sucesso
                listaMasterPC = volta.Item2.ToList();
                listaMasterPC = listaMasterPC.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                listaMasterPC = listaMasterPC.Where(p => p.PACO_DT_CONSULTA.Date < DateTime.Today.Date & p.PACO_IN_CONFIRMADA == 1 & p.PACO_IN_ENCERRADA == 0).ToList();
                Session["ListaConsultaAberta"] = listaMasterPC;
                return RedirectToAction("MontarTelaEncerrarConsulta");
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

        public ActionResult RetirarFiltroConsultasEncerramento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                listaMasterPC = null;
                Session["ListaConsultaAberta"] = null;
                return RedirectToAction("MontarTelaEncerrarConsulta");
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

        public ActionResult VoltarEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["NivelPaciente"] = 13;
            if ((Int32)Session["VoltaEncerramento"] == 1)
            {
                return RedirectToAction("MontarTelaConsultas", "Paciente");
            }
            if ((Int32)Session["VoltaEncerramento"] == 3)
            {
                return RedirectToAction("VoltarAnexoPaciente", "Paciente");
            }
            if ((Int32)Session["VoltaEncerramento"] == 4)
            {
                return RedirectToAction("VoltarProcederConsulta", "Paciente");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        [HttpGet]
        public ActionResult EncerrarConsulta(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                // Recuperar Consulta
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);

                // Verifica consumo de material
                Session["ConsultaEncerra"] = item;
                VALOR_CONSULTA tipo = vcApp.GetItemById(item.VACO_CD_ID.Value);
                Session["TipoConsultaEncerra"] = tipo;
                if (tipo.VALOR_CONSULTA_MATERIAL.Count() > 0)
                {
                    return RedirectToAction("MontarTelaEncerrarConsultaMaterial", "Paciente2");
                }

                // Recuperar paciente
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);

                // Recupera forma de recebimento
                List<FORMA_RECEBIMENTO> frs = recApp.GetAllForma(idAss);
                if (frs.Count == 0)
                {
                    Session["MensPaciente"] = 62;
                    return RedirectToAction("MontarTelaEncerrarConsulta");
                }
                FORMA_RECEBIMENTO fr = frs.Where(p => p.FORE_IN_PADRAO == 1).FirstOrDefault();
                if (fr == null)
                {
                    fr = frs.FirstOrDefault();
                }

                // Acertar consulta
                item.PACO_IN_CONFIRMADA = 3;
                item.PACO_IN_ENCERRADA = 1;
                Int32 volta = baseApp.ValidateEditConsultaConfirma(item);

                // Mensagem do CRUD
                String crud = "A consulta do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " em " + item.PACO_DT_CONSULTA.ToLongDateString() + " foi encerrada com sucesso";

                // Gerar recebimento
                if (item.PACO_IN_RECEBE == 1)
                {
                    PACIENTE_CONSULTA item1 = baseApp.GetConsultaById(id);
                    List<CONSULTA_RECEBIMENTO> pagMes = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Int32 num = pagMes.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                    if ((Int32)Session["NumRecebimentos"] >= num)
                    {
                        if (conf.CONF_IN_GERA_RECEBIMENTO == 1 & fr != null)
                        {
                            String nome = "Recebimento de consulta de " + pac.PACI_NM_NOME.ToUpper() + " em " + item.PACO_DT_CONSULTA.ToLongDateString();
                            CONSULTA_RECEBIMENTO rec = new CONSULTA_RECEBIMENTO();
                            rec.CORE_IN_ATIVO = 1;
                            rec.CORE_DT_RECEBIMENTO = DateTime.Today.Date;
                            rec.CORE_GU_GUID = Xid.NewXid().ToString();
                            rec.USUA_CD_ID = usuario.USUA_CD_ID;
                            rec.ASSI_CD_ID = idAss;
                            rec.CORE_IN_CONFERIDO = 0;
                            rec.CORE_NM_RECEBIMENTO = nome;
                            rec.PACI_CD_ID = pac.PACI__CD_ID;
                            rec.PACO_CD_ID = item1.PACO_CD_ID;
                            rec.VACO_CD_ID = item1.VACO_CD_ID;
                            rec.CORE_VL_VALOR = item1.VALOR_CONSULTA.VACO_NR_VALOR;
                            rec.FORE_CD_ID = fr.FORE_CD_ID;
                            recApp.ValidateCreate(rec, usuario);

                            crud += ". Um lançamento de recebimento foi gerado para esta consulta.";

                            // Cria pastas
                            String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + rec.CORE_CD_ID.ToString() + "/Anexos/";
                            String map = Server.MapPath(caminho);
                            Directory.CreateDirectory(Server.MapPath(caminho));
                            Session["ListaRecebimento"] = null;
                            Session["Recebimentos"] = null;
                            Session["RecebimentoAlterada"] = 1;
                        }
                    }
                    else
                    {
                        crud += ". O lançamento de recebimento não pode ser gerado pois o número de lançamentos do mês excedeu o limite contratado.";
                    }
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = crud;
                Session["MensPaciente"] = 63;
                Session["ListaConsultaAberta"] = null;
                Session["ConsultasAlterada"] = 1;
                Session["Consultas"] = null;
                Session["ListaConsultasGeral"] = null;

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
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Consulta - Encerramento",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
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
                hist.PAHI_NM_OPERACAO = "Paciente - Encerramento de Consulta";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME.ToUpper() + " - Consulta encerrada: " + item.PACO_DT_CONSULTA.ToShortDateString();
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                return RedirectToAction("MontarTelaEncerrarConsulta");
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
        public JsonResult EncerrarConsultaJson(Int32 id)
        {
            string urlRedirecionamento = "";
            if ((String)Session["Ativa"] == null)
            {
                urlRedirecionamento = Url.Action("Logout", "ControleAceso");
                return Json(new { success = true, redirectUrl = urlRedirecionamento });
            }

            try
            {
                // Recuperar Consulta
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                PACIENTE_CONSULTA item = baseApp.GetConsultaById(id);

                // Verifica consumo de material
                Session["ConsultaEncerra"] = item;
                VALOR_CONSULTA tipo = vcApp.GetItemById(item.VACO_CD_ID.Value);
                Session["TipoConsultaEncerra"] = tipo;
                if (tipo.VALOR_CONSULTA_MATERIAL.Count() > 0)
                {
                    urlRedirecionamento = Url.Action("MontarTelaEncerrarConsultaMaterial", "Paciente2");
                    return Json(new { success = true, redirectUrl = urlRedirecionamento });
                }

                // Recuperar paciente
                PACIENTE pac = baseApp.GetItemById(item.PACI_CD_ID);

                // Recupera forma de recebimento
                List<FORMA_RECEBIMENTO> frs = recApp.GetAllForma(idAss);
                if (frs.Count == 0)
                {
                    Session["MensPaciente"] = 62;
                    urlRedirecionamento = Url.Action("MontarTelaEncerrarConsulta", "Financeiro");
                    return Json(new { success = true, redirectUrl = urlRedirecionamento });
                }
                FORMA_RECEBIMENTO fr = frs.Where(p => p.FORE_IN_PADRAO == 1).FirstOrDefault();
                if (fr == null)
                {
                    fr = frs.FirstOrDefault();
                }

                // Acertar consulta
                item.PACO_IN_CONFIRMADA = 3;
                item.PACO_IN_ENCERRADA = 1;
                Int32 volta = baseApp.ValidateEditConsultaConfirma(item);

                // Mensagem do CRUD
                String crud = "A consulta do(a) paciente " + pac.PACI_NM_NOME.ToUpper() + " em " + item.PACO_DT_CONSULTA.ToLongDateString() + " foi encerrada com sucesso";

                // Gerar recebimento
                if (item.PACO_IN_RECEBE == 1)
                {
                    if ((Int32)Session["PermFinanceiro"] == 1)
                    {
                        PACIENTE_CONSULTA item1 = baseApp.GetConsultaById(id);
                        List<CONSULTA_RECEBIMENTO> pagMes = CarregaRecebimento().Where(p => p.CORE_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                        Int32 num = pagMes.Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                        if ((Int32)Session["NumRecebimentos"] >= num)
                        {
                            if (conf.CONF_IN_GERA_RECEBIMENTO == 1 & fr != null)
                            {
                                String nome = "Recebimento de consulta de " + pac.PACI_NM_NOME.ToUpper() + " em " + item.PACO_DT_CONSULTA.ToLongDateString();
                                CONSULTA_RECEBIMENTO rec = new CONSULTA_RECEBIMENTO();
                                rec.CORE_IN_ATIVO = 1;
                                rec.CORE_DT_RECEBIMENTO = DateTime.Today.Date;
                                rec.CORE_GU_GUID = Xid.NewXid().ToString();
                                rec.USUA_CD_ID = usuario.USUA_CD_ID;
                                rec.ASSI_CD_ID = idAss;
                                rec.CORE_IN_CONFERIDO = 0;
                                rec.CORE_NM_RECEBIMENTO = nome;
                                rec.PACI_CD_ID = pac.PACI__CD_ID;
                                rec.PACO_CD_ID = item1.PACO_CD_ID;
                                rec.VACO_CD_ID = item1.VACO_CD_ID;
                                rec.CORE_VL_VALOR = item1.VALOR_CONSULTA.VACO_NR_VALOR;
                                rec.FORE_CD_ID = fr.FORE_CD_ID;
                                recApp.ValidateCreate(rec, usuario);

                                crud += ". Um lançamento de recebimento foi gerado para esta consulta.";

                                // Cria pastas
                                String caminho = "/Imagens/" + idAss.ToString() + "/Recebimento/" + rec.CORE_CD_ID.ToString() + "/Anexos/";
                                String map = Server.MapPath(caminho);
                                Directory.CreateDirectory(Server.MapPath(caminho));
                                Session["ListaRecebimento"] = null;
                                Session["Recebimentos"] = null;
                                Session["RecebimentoAlterada"] = 1;
                            }
                        }
                        else
                        {
                            crud += ". O lançamento de recebimento não pode ser gerado pois o número de lançamentos do mês excedeu o limite contratado.";
                        }
                    }
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = crud;
                Session["MensPaciente"] = 63;
                Session["ListaConsultaAberta"] = null;
                Session["ConsultasAlterada"] = 1;
                Session["Consultas"] = null;
                Session["ListaConsultasGeral"] = null;

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
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Consulta - Encerramento",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
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
                hist.PAHI_NM_OPERACAO = "Paciente - Encerramento de Consulta";
                hist.PAHI_DS_DESCRICAO = "Paciente: " + pac.PACI_NM_NOME.ToUpper() + " - Consulta encerrada: " + item.PACO_DT_CONSULTA.ToShortDateString();
                Int32 voltaHist = baseApp.ValidateCreateHistorico(hist);

                // Retorno
                urlRedirecionamento = Url.Action("MontarTelaEncerrarConsulta", "Financeiro");
                return Json(new { success = true, redirectUrl = urlRedirecionamento });
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
                urlRedirecionamento = Url.Action("TrataExcecao", "BaseAdmin");
                return Json(new { success = true, redirectUrl = urlRedirecionamento });
            }
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

        public ActionResult GerarRelatorioConsultasEncerramento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ConsultaEncerrarLista" + "_" + data + ".pdf";
                List<PACIENTE_CONSULTA> lista = (List<PACIENTE_CONSULTA>)Session["ListaConsultaAberta"];

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
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Consultas Em Aberto", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Consultas Em Aberto", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 60f, 140f, 60f, 60f});
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
                cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
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
                    if (item.VALOR_CONSULTA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.VALOR_CONSULTA.VACO_NM_NOME, meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.VALOR_CONSULTA.VACO_NR_VALOR.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Não informado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Não informado", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
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

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaEncerrarConsulta");
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

        public ActionResult ProcessaRelatorioPagamento(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarListagemPagamento");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarListagemPagamentoTotalData");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarListagemPagamentoTotalMes");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarListagemPagamentoTotalFavorecido");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarListagemPagamentoVenceHoje");
            }
            if (tipoRel == 6)
            {
                return RedirectToAction("GerarListagemPagamentoVenceMes");
            }
            if (tipoRel == 7)
            {
                return RedirectToAction("GerarListagemPagamentoQuitadoHoje");
            }
            if (tipoRel == 8)
            {
                return RedirectToAction("GerarListagemPagamentoQuitadoMes");
            }
            if (tipoRel == 9)
            {
                return RedirectToAction("GerarListagemPagamentoTotalAno");
            }
            if (tipoRel == 10)
            {
                return RedirectToAction("GerarListagemPagamentoTotalTipo");
            }
            return RedirectToAction("MontarTelaPagamento");
        }

        public ActionResult GerarListagemPagamentoTotalData()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoTotalDataLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos1 = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos1 = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos1 = CarregaPagamento().ToList();
                }
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datas = pagtos.Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();

                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 conta = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date).Count();
                    Decimal? soma = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date).Sum(p => p.COPA_VL_PAGO);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Data", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Data", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Data do Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Pagamentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média Diária de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.DataEmissao.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(mediaTotal.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoTotalFavorecido()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoTotalFavorecidoLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos = CarregaPagamento().ToList();
                }
                pagtos = pagtos.Where(p => p.COPA_IN_PAGO == 1).ToList();
                List<String> favs = pagtos.Select(p => p.COPA_NM_FAVORECIDO).Distinct().ToList();
                favs.Sort((i, j) => i.CompareTo(j));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (String item in favs)
                {
                    Int32 conta = pagtos.Where(p => p.COPA_NM_FAVORECIDO == item).Count();
                    Decimal? soma = pagtos.Where(p => p.COPA_NM_FAVORECIDO == item).Sum(p => p.COPA_VL_PAGO);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }
                lista = lista.OrderBy(p => p.Nome).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Favorecido", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Favorecido", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Nome do Favorecido", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Pagamentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista.OrderBy(p => p.Nome))
                {
                    if (item.Nome != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoTotalMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                DateTime limite = DateTime.Today.Date.AddMonths(-12);

                String nomeRel = "PagamentoTotalMesLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? numX = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos1 = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos1 = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos1 = CarregaPagamento().ToList();
                }
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPagto = pagtos.Where(p => p.COPA_DT_PAGAMENTO != null).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
                datasPagto.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String mes2 = null;
                String mesFeito2 = null;
                foreach (DateTime item in datasPagto)
                {
                    if (item.Date > limite)
                    {
                        mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                        if (mes2 != mesFeito2)
                        {
                            Decimal conta = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.COPA_DT_PAGAMENTO > limite).Sum(p => p.COPA_VL_PAGO.Value);
                            Int32 num = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.COPA_DT_PAGAMENTO > limite).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.Nome = mes2;
                            mod.Valor = num;
                            mod.ValorDec1 = conta;
                            listaMes.Add(mod);
                            mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                        }
                    }
                }
                listaMes = listaMes.OrderBy(p => p.DataEmissao).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total por Mès", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total por Mês", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Mês de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Pagamentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    total += item.ValorDec1;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f});
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult ProcessaRelatorioRecebimento(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarListagemRecebimento");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalData");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalMes");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalPaciente");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalAno");
            }
            if (tipoRel == 6)
            {   
                return RedirectToAction("GerarListagemRecebimentoTotalProfissional");
            }
            return RedirectToAction("MontarTelaRecebimento");
        }

        public ActionResult GerarListagemRecebimentoTotalData()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "RecebimentoTotalDataLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_RECEBIMENTO> pagtos1 = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    pagtos1 = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    pagtos1 = CarregaRecebimento().ToList();
                }
                List<CONSULTA_RECEBIMENTO> pagtos = pagtos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datas = pagtos.Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();

                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 conta = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Count();
                    Decimal? soma = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Sum(p => p.CORE_VL_VALOR);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }

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

                    cell1 = new PdfPCell(new Paragraph("Recebimentos - Total e Média por Data", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Recebimentos - Total e Média por Data", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Data do Recebimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Recebimentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média Diária de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.DataEmissao.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(mediaTotal.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult GerarListagemRecebimentoTotalMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                DateTime limite = DateTime.Today.Date.AddMonths(-12);

                String nomeRel = "RecebimentoTotalMesLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? numX = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;


                // Carrega dados
                List<CONSULTA_RECEBIMENTO> pagtos1 = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    pagtos1 = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    pagtos1 = CarregaRecebimento().ToList();
                }
                List<CONSULTA_RECEBIMENTO> pagtos = pagtos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasPagto = pagtos.Where(p => p.CORE_DT_RECEBIMENTO != null).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
                datasPagto.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String mes2 = null;
                String mesFeito2 = null;
                foreach (DateTime item in datasPagto)
                {
                    if (item.Date > limite)
                    {
                        mes2 = item.Month.ToString() + "/" + item.Year.ToString();
                        if (mes2 != mesFeito2)
                        {
                            Decimal conta = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Month == item.Month & p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year & p.CORE_DT_RECEBIMENTO > limite).Sum(p => p.CORE_VL_VALOR.Value);
                            Int32 num = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Month == item.Month & p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year & p.CORE_DT_RECEBIMENTO > limite).Count();
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.Nome = mes2;
                            mod.Valor = num;
                            mod.ValorDec1 = conta;
                            listaMes.Add(mod);
                            mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                        }
                    }
                }
                listaMes = listaMes.OrderBy(p => p.DataEmissao).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Recebimentos - Total por Mès", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Recebimentos - Total por Mês", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Mês de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Recebimentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    total += item.ValorDec1;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult GerarListagemRecebimentoTotalPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "RecebimentoTotalPacienteLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_RECEBIMENTO> pagtos = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    pagtos = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    pagtos = CarregaRecebimento().ToList();
                }
                pagtos = pagtos.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<Int32> pacs = pagtos.Select(p => p.PACI_CD_ID.Value).Distinct().ToList();
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (Int32 item in pacs)
                {
                    PACIENTE pac = baseApp.GetItemById(item);
                    Int32 conta = pagtos.Where(p => p.PACI_CD_ID.Value == item).Count();
                    Decimal? soma = pagtos.Where(p => p.PACI_CD_ID.Value  == item).Sum(p => p.CORE_VL_VALOR);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = pac.PACI_NM_NOME;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }
                lista = lista.OrderBy(p => p.Nome).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Paciente", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Paciente", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Nome do Paciente", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Recebimentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média Diária de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.Nome != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult GerarRelatorioValorConsulta()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "ConsultaValor" + "_" + data + ".pdf";
                List<VALOR_CONSULTA> lista = (List<VALOR_CONSULTA>)Session["ListaValorConsulta"];

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
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                    image.ScaleAbsolute(50, 50);
                    cell1.AddElement(image);
                    cell1.Border = PdfPCell.BOTTOM_BORDER;
                    headerTable.AddCell(cell1);

                    cell1 = new PdfPCell(new Paragraph("Valores de Consulta", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Valores de Consulta", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 80f, 70f, 200f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Tipo de Consulta", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data de Referência", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Valor Desconto (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (VALOR_CONSULTA item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.TIPO_VALOR_CONSULTA.TIVL_NM_TIPO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (item.VACO_DT_REFERENCIA != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.VACO_DT_REFERENCIA.Value.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(item.VACO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.VACO_NR_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.VACO_NR_DESCONTO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
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

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaValorConsulta");
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
        public ActionResult VerPagamentoCalendario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            try
            {
                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                CONSULTA_PAGAMENTO item = new CONSULTA_PAGAMENTO();
                PagamentoViewModel vm = Mapper.Map<CONSULTA_PAGAMENTO, PagamentoViewModel>(item);
                Int32 id = (Int32)Session["IdMarcacao"];
                ViewBag.Nome = usuario.USUA_NM_NOME;

                Session["Pagamentos"] = null;
                Session["PagamentoAlterada"] = 1;
                if (usuario.PERFIL.PERF_IN_VISAO_GERAL == 1 || usuario.PERFIL.PERF_SG_SIGLA == "ADM")
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1).ToList();
                }
                else
                {
                    listaMasterPag = CarregaPagamento().Where(p => p.USUA_CD_ID == id & p.COPA_IN_ATIVO == 1).ToList();
                }
                Session["ListaPagto"] = listaMasterPag;
                Session["EnviaLink"] = 0;
                Session["EditaLink"] = 0;
                Session["NaoFezNada"] = 0;
                Session["TipoAgenda"] = 1;
                Session["VoltaCalendario"] = 1;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PAGAMENTO_CALENDARIO", "Financeiro", "VerPagamentoCalendario");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public JsonResult GetEventosCalendarioPagamento()
        {
            try
            {
                var usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<Hashtable> listaCalendario = new List<Hashtable>();

                Int32 id = usuario.USUA_CD_ID;
                Session["Pagamentos"] = null;
                Session["PagamentoAlterada"] = 1;
                listaMasterPag = CarregaPagamento().Where(p => p.USUA_CD_ID == id & p.COPA_IN_ATIVO == 1).ToList();
                Session["ListaPagto"] = listaMasterPag;

                foreach (var item in listaMasterPag)
                {
                    var hash = new Hashtable();

                    hash.Add("id", item.COPA_CD_ID);
                    hash.Add("title", item.COPA_NM_NOME);
                    hash.Add("date", (item.COPA_DT_VENCIMENTO).Value.ToString("yyyy-MM-dd"));
                    hash.Add("description", (new DateTime() + "10:00" + " " + new DateTime() + "11:00"));
                    hash.Add("confirm", item.COPA_IN_PAGO == 1 ? "Sim" : "Não");

                    listaCalendario.Add(hash);
                }
                return Json(listaCalendario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetDetalhesEventoPagamento(Int32 id)
        {
            try
            {
                var evento = pagApp.GetItemById(id);

                var hash = new Hashtable();
                hash.Add("data", evento.COPA_DT_VENCIMENTO.Value.ToShortDateString());
                hash.Add("hora", "10:00");
                hash.Add("final", "11:00");
                hash.Add("nome", evento.COPA_NM_NOME);
                hash.Add("fav", evento.COPA_NM_FAVORECIDO);
                if (evento.COPA_IN_PAGO == 0)
                {
                    hash.Add("valor", evento.COPA_VL_VALOR.ToString());
                }
                else
                {
                    hash.Add("valor", evento.COPA_VL_PAGO.ToString());
                }
                hash.Add("confirm", evento.COPA_IN_PAGO == 1 ? "Sim" : "Não");
                return Json(hash);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpPost]
        public JsonResult EditarPagamentoOnChange(Int32 id, DateTime data)
        {
            try
            {
                // Recupera dados   
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                CONSULTA_PAGAMENTO obj = pagApp.GetItemById(id);
                var hash = new Hashtable();
                Int32 vai = 0;

                // Monta novo registro
                CONSULTA_PAGAMENTO item = new CONSULTA_PAGAMENTO();
                item.COPA_CD_ID = id;
                item.USUA_CD_ID = obj.USUA_CD_ID;
                item.ASSI_CD_ID = obj.ASSI_CD_ID;
                item.COPA_DT_CADASTRO = obj.COPA_DT_CADASTRO;
                item.COPA_DT_DUMMY = obj.COPA_DT_DUMMY;
                item.COPA_DT_PAGAMENTO = obj.COPA_DT_PAGAMENTO;
                if (obj.COPA_IN_PAGO == 1)
                {
                    item.COPA_DT_VENCIMENTO = obj.COPA_DT_VENCIMENTO;
                }
                else
                {
                    vai = 1;
                    item.COPA_DT_VENCIMENTO = data.Date;
                }
                item.COPA_GU_GUID = obj.COPA_GU_GUID;
                item.COPA_IN_ATIVO = obj.COPA_IN_ATIVO;
                item.COPA_IN_CONFERIDO = obj.COPA_IN_CONFERIDO;
                item.COPA_IN_PAGO = obj.COPA_IN_PAGO;
                item.COPA_NM_FAVORECIDO = obj.COPA_NM_FAVORECIDO;
                item.COPA_NM_NOME = obj.COPA_NM_NOME;
                item.COPA_NR_ATRASO = obj.COPA_NR_ATRASO;
                item.COPA_VL_DESCONTO = obj.COPA_VL_DESCONTO;
                item.COPA_VL_MULTA = obj.COPA_VL_MULTA;
                item.COPA_VL_PAGO = obj.COPA_VL_PAGO;
                item.COPA_VL_VALOR = obj.COPA_VL_VALOR;
                item.COPA_XM_NOTA_FISCAL = obj.COPA_XM_NOTA_FISCAL;
                item.TIPA_CD_ID = obj.TIPA_CD_ID;
                Int32 volta = pagApp.ValidateEdit(item, item);

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                CONSULTA_PAGAMENTO pag = pagApp.GetItemById(item.COPA_CD_ID);
                DTO_Pagamento dto = MontarPagamentoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Pagamento - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                if (vai == 1)
                {
                    hash.Add("volta", 1);
                    hash.Add("desc", "Pagamento reagendado com sucesso");
                }
                else
                {
                    hash.Add("volta", 2);
                    hash.Add("desc", "Pagamento já quitado não pode ser reagendado");
                }
                Session["Pagamentos"] = null;
                Session["PagamentoAlterada"] = 1;
                Session["ListaPagamento"] = null;
                return Json(hash);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return Json(0);
            }
        }

        public ActionResult GerarListagemPagamentoQuitadoHoje()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoListaQuitadoHoje" + "_" + data + ".pdf";
                List<CONSULTA_PAGAMENTO> lista = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_PAGAMENTO != null).ToList();
                lista = lista.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == DateTime.Today.Date).ToList();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? total = 0;
                Decimal? totalPago = 0;

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Quitados em " + DateTime.Today.Date.ToLongDateString(), meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Quitados em " + DateTime.Today.Date.ToLongDateString(), meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Favorecido", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quitado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_PAGAMENTO item in lista)
                {
                    if (item.COPA_DT_VENCIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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

                    if (item.COPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_FAVORECIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_PAGO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.COPA_IN_PAGO == 1)
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

                    total += item.COPA_VL_VALOR;
                    totalPago += item.COPA_VL_PAGO;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 5;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPago.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoQuitadoMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }


                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoListaQuitadoMes" + "_" + data + ".pdf";
                String mes = CrossCutting.UtilitariosGeral.NomeMes(DateTime.Today.Month) + " de " + DateTime.Today.Year.ToString();
                List<CONSULTA_PAGAMENTO> lista = CarregaPagamento().Where(p => p.COPA_IN_ATIVO == 1 & p.COPA_IN_PAGO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID & p.COPA_DT_PAGAMENTO != null).ToList();
                lista = lista.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Year).OrderBy(p => p.COPA_DT_PAGAMENTO).ToList();
                lista = lista.OrderBy(p => p.COPA_DT_PAGAMENTO).ToList();             
                
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? total = 0;
                Decimal? totalPago = 0;

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Quitados em " + mes, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Quitados em " + mes, meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
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
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Favorecido", meuFont))
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
                cell = new PdfPCell(new Paragraph("Valor Pago (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quitado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (CONSULTA_PAGAMENTO item in lista)
                {
                    if (item.COPA_DT_VENCIMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
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

                    if (item.COPA_DT_PAGAMENTO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.COPA_DT_PAGAMENTO.Value.ToShortDateString(), meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.TIPO_PAGAMENTO.TIPA_NM_PAGAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.COPA_NM_FAVORECIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_VALOR.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.COPA_VL_PAGO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.COPA_IN_PAGO == 1)
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
                    total += item.COPA_VL_VALOR;
                    totalPago += item.COPA_VL_PAGO;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 60f, 80f, 100f, 200f, 100f, 60f, 60f, 50f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 5;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPago.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("  ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public DTO_ValorConsulta MontarValorConsultaDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.VALOR_CONSULTA
                    .Where(l => l.VACO_CD_ID == mediId)
                    .Select(l => new DTO_ValorConsulta
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        VACO_CD_ID = l.VACO_CD_ID,
                        VACO_DT_REFERENCIA = l.VACO_DT_REFERENCIA,
                        VACO_IN_ATIVO = l.VACO_IN_ATIVO,
                        VACO_IN_PADRAO = l.VACO_IN_PADRAO,
                        VACO_NM_EXIBE = l.VACO_NM_EXIBE,
                        VACO_NM_NOME = l.VACO_NM_NOME,
                        VACO_NR_DESCONTO = l.VACO_NR_DESCONTO,
                        VACO_NR_VALOR = l.VACO_NR_VALOR,
                        TIVL_CD_ID = l.TIVL_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
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
                conf = conf.Where(p => p.PROD_IN_SISTEMA == 6).ToList();
                Session["ProdutoAlterada"] = 0;
                Session["Produtos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult IncluirConsumoMaterial()
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_INCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Inclusão";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Consultas - Valores - Inclusão";

                // Prepara listas
                ViewBag.Produto = new SelectList(CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 1), "PROD_CD_ID", "PROD_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/17/Ajuda17_1.pdf";
                Session["NivelPaciente"] = 1;

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 55)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0584", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0596", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONSULTA_CONSUMO_MATERIAL_INCLUIR", "Financeiro", "IncluirConsumoMaterial");

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONSULTA vc = vcApp.GetItemById((Int32)Session["IdValorConsulta"]);
                VALOR_CONSULTA_MATERIAL item = new VALOR_CONSULTA_MATERIAL();
                ValorConsulta1MaterialViewModel vm = Mapper.Map<VALOR_CONSULTA_MATERIAL, ValorConsulta1MaterialViewModel>(item);
                vm.VCMA_IN_ATIVO = 1;
                vm.ASSI_CD_ID = idAss;
                vm.VACO_CD_ID = vc.VACO_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.VCMA_QN_QUANTIDADE = 0;
                vm.VALOR_CONSULTA = vc;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Financeiro";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Financeiro", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConsumoMaterial(ValorConsulta1MaterialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Produto = new SelectList(CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 1), "PROD_CD_ID", "PROD_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if (vm.VCMA_QN_QUANTIDADE == null || vm.VCMA_QN_QUANTIDADE == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0723", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.PROD_CD_ID == null || vm.PROD_CD_ID == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0724", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    vm.VCMA_QN_QUANTIDADE_REAL = vm.VCMA_QN_QUANTIDADE;
                    VALOR_CONSULTA_MATERIAL item = Mapper.Map<ValorConsulta1MaterialViewModel, VALOR_CONSULTA_MATERIAL>(vm);
                    Int32 volta = vcApp.ValidateCreateConsultaMaterial(item);
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0725", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Verifica retorno
                    Session["IdValorConsulta"] = item.VACO_CD_ID;
                    Session["ValorConsultaAlterada"] = 1;
                    Session["NivelPaciente"] = 1;
                    Session["ListaValorConsulta"] = null;

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                    PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                    String json = "Tipo: " + vc.VACO_NM_NOME + " Material: " + prod.PROD_NM_NOME + " Quantidade: " + item.VCMA_QN_QUANTIDADE.ToString();
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Valor de Consulta - Material - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O material " + prod.PROD_NM_NOME.ToUpper() + " foi incluído com sucesso no tipo de consulta.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("EditarValorConsulta", new { id = (Int32)Session["IdValorConsulta"] });
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
        public ActionResult EditarConsumoMaterial(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_ALTERAR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Edição";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Consultas - Valores - Edição";

                // Prepara Listas
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/17/Ajuda17_2.pdf";

                // Mensagem
                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0581", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "CONSULTA_CONSUMO_MATERIAL_EDITAR", "Financeiro", "EditarConsumoMaterial");

                // Prepara registro
                Session["MensPaciente"] = null;
                VALOR_CONSULTA_MATERIAL item = vcApp.GetConsultaMaterialById(id);
                Session["IdValorConsulta"] = item.VACO_CD_ID;
                ValorConsulta1MaterialViewModel vm = Mapper.Map<VALOR_CONSULTA_MATERIAL, ValorConsulta1MaterialViewModel>(item);
                Session["ValorMaterialAntes"] = item;
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
        public ActionResult EditarConsumoMaterial(ValorConsulta1MaterialViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Criticas
                    if (vm.VCMA_QN_QUANTIDADE == null || vm.VCMA_QN_QUANTIDADE == 0)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0723", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    VALOR_CONSULTA_MATERIAL item = Mapper.Map<ValorConsulta1MaterialViewModel, VALOR_CONSULTA_MATERIAL>(vm);
                    Int32 volta = vcApp.ValidateEditConsultaMaterial(item);

                    // Verifica retorno

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                    PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                    String json = "Tipo: " + vc.VACO_NM_NOME + " Material: " + prod.PROD_NM_NOME + " Quantidade: " + item.VCMA_QN_QUANTIDADE.ToString();
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Valor de Consulta - Material - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O quantidade do material " + prod.PROD_NM_NOME.ToUpper() + " foi alterada com sucesso no tipo de consulta.";
                    Session["MensPaciente"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaValorConsulta");
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
        public ActionResult ExcluirConsumoMaterial(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_FINANCEIRO_REC_EXCLUIR == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Financeiro - Valor de Consulta - Exclusão";
                        return RedirectToAction("MontarTelaValorConsulta");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                VALOR_CONSULTA_MATERIAL item = vcApp.GetConsultaMaterialById(id);
                item.VCMA_IN_ATIVO = 0;
                Int32 volta = vcApp.ValidateEditConsultaMaterial(item);

                Session["ValorConsultaAlterada"] = 1;
                Session["NivelPaciente"] = 1;
                Session["ListaValorConsulta"] = null;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                VALOR_CONSULTA vc = vcApp.GetItemById(item.VACO_CD_ID);
                PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                String json = "Tipo: " + vc.VACO_NM_NOME + " Material: " + prod.PROD_NM_NOME + " Quantidade: " + item.VCMA_QN_QUANTIDADE.ToString();
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Valor de Consulta - Material - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O material " + prod.PROD_NM_NOME.ToUpper() + " foi excluído com sucesso no tipo de consulta.";
                Session["MensPaciente"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaValorConsulta");
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

        public ActionResult GerarListagemPagamentoTotalAno()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoTotalAnoLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos1 = CarregaPagamento();
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPagto = pagtos.Where(p => p.COPA_DT_PAGAMENTO != null).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();
               
                datasPagto.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String ano2 = null;
                String anoFeito2 = null;
                foreach (DateTime item in datasPagto)
                {
                    ano2 = item.Year.ToString();
                    if (ano2 != anoFeito2)
                    {
                        Decimal conta = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year).Sum(p => p.COPA_VL_PAGO.Value);
                        Int32 num2 = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = ano2;
                        mod.Valor = num2;
                        mod.ValorDec1 = conta;
                        listaMes.Add(mod);
                        anoFeito2 = item.Year.ToString();
                    }
                }

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total por Ano", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total por Ano", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Ano de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Pagamentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec1;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoTotalTipo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoTotalTipoLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos = CarregaPagamento().ToList();
                }
                List<Int32?> favs = pagtos.Select(p => p.TIPA_CD_ID).Distinct().ToList();
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (Int32 item in favs)
                {
                    Int32 conta = pagtos.Where(p => p.TIPA_CD_ID == item).Count();
                    Decimal? soma = pagtos.Where(p => p.TIPA_CD_ID == item).Sum(p => p.COPA_VL_PAGO);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = tpApp.GetItemById(item).TIPA_NM_PAGAMENTO;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }
                lista = lista.OrderBy(p => p.Nome).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Tipo", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos - Total e Média por Tipo", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Tipo de Pagamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Pagamentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.Nome != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal =0;
                if (itens > 0)
                {
                    mediaTotal = total / itens;
                }

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemRecebimentoTotalAno()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "recebimentoTotalAnoLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_RECEBIMENTO> pagtos1 = CarregaRecebimento();
                List<CONSULTA_RECEBIMENTO> pagtos = pagtos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasPagto = pagtos.Where(p => p.CORE_DT_RECEBIMENTO != null).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();
               
                datasPagto.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String ano2 = null;
                String anoFeito2 = null;
                foreach (DateTime item in datasPagto)
                {
                    ano2 = item.Year.ToString();
                    if (ano2 != anoFeito2)
                    {
                        Decimal conta = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year).Sum(p => p.CORE_VL_VALOR.Value);
                        Int32 num2 = pagtos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year).Count();
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = ano2;
                        mod.Valor = num2;
                        mod.ValorDec1 = conta;
                        listaMes.Add(mod);
                        anoFeito2 = item.Year.ToString();
                    }
                }
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

                    cell1 = new PdfPCell(new Paragraph("Recebimentos - Total por Ano", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Recebimentos - Total por Ano", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Ano de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Recebimentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    num += item.Valor;
                    total += item.ValorDec1;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult GerarListagemRecebimentoTotalProfissional()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "RecebimentoTotalProfissionalLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Decimal? num = 0;
                Decimal? total = 0;
                Decimal? mediaFinal = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_RECEBIMENTO> pagtos = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    pagtos = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    pagtos = CarregaRecebimento().ToList();
                }
                pagtos = pagtos.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<Int32> pacs = pagtos.Select(p => p.USUA_CD_ID).Distinct().ToList();
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (Int32 item in pacs)
                {
                    USUARIO pac = usuApp.GetItemById(item);
                    Int32 conta = pagtos.Where(p => p.USUA_CD_ID == item).Count();
                    Decimal? soma = pagtos.Where(p => p.USUA_CD_ID == item).Sum(p => p.CORE_VL_VALOR);
                    Decimal? media = 0;
                    if (conta > 0)
                    {
                        media = soma / conta;
                    }
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = pac.USUA_NM_NOME;
                    mod.Valor = conta;
                    mod.ValorDec = soma.Value;
                    mod.ValorDec1 = media.Value;
                    lista.Add(mod);
                }
                lista = lista.OrderBy(p => p.Nome).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Recebimentos - Total e Média por Profissional", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Recebimentos - Total e Média por Profissional", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Nome do Profissional", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Número de Recebimentos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.Nome != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.Valor), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    num += item.Valor;
                    total += item.ValorDec;
                    itens++;
                }
                pdfDoc.Add(table);

                // Calcula media final
                Decimal? mediaTotal = total / itens;

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(total.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaRecebimento");
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

        public ActionResult ProcessaRelatorioFinanceiroGeral(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarListagemPagamento");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarListagemPagamentoTotalData");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarListagemPagamentoTotalMes");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarListagemPagamentoTotalFavorecido");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarListagemPagamentoVenceHoje");
            }
            if (tipoRel == 6)
            {
                return RedirectToAction("GerarListagemPagamentoVenceMes");
            }
            if (tipoRel == 7)
            {
                return RedirectToAction("GerarListagemPagamentoQuitadoHoje");
            }
            if (tipoRel == 8)
            {
                return RedirectToAction("GerarListagemPagamentoQuitadoMes");
            }
            if (tipoRel == 9)
            {
                return RedirectToAction("GerarListagemPagamentoTotalAno");
            }
            if (tipoRel == 10)
            {
                return RedirectToAction("GerarListagemPagamentoTotalTipo");
            }
            if (tipoRel == 11)
            {
                return RedirectToAction("GerarListagemRecebimento");
            }
            if (tipoRel == 12)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalData");
            }
            if (tipoRel == 13)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalMes");
            }
            if (tipoRel == 15)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalPaciente");
            }
            if (tipoRel == 14)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalAno");
            }
            if (tipoRel == 16)
            {
                return RedirectToAction("GerarListagemRecebimentoTotalProfissional");
            }
            if (tipoRel == 17)
            {
                return RedirectToAction("GerarListagemPagamentoRecebimentoTotalData");
            }
            if (tipoRel == 18)
            {
                return RedirectToAction("GerarListagemPagamentoRecebimentoTotalMes");
            }
            if (tipoRel == 19)
            {
                return RedirectToAction("GerarListagemPagamentoRecebimentoTotalAno");
            }
            if (tipoRel == 20)
            {
                return RedirectToAction("GerarListagemPagamentoRecebimentoTotalDetalhado");
            }
            return RedirectToAction("MontarTelaFinanceiro");
        }

        public ActionResult GerarListagemPagamentoRecebimentoTotalData()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoRecebimentoTotalDataLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                Font meuFont5 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

                Decimal? num = 0;
                Decimal? totalPag = 0;
                Decimal? totalRec = 0;
                Decimal? totalSaldo = 0;
                Decimal? itens = 0;

                // Carrega dados Pagto
                List<CONSULTA_PAGAMENTO> pagtos1 = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos1 = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos1 = CarregaPagamento().ToList();
                }
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPag = pagtos.Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();

                // Carrega dados Recto
                List<CONSULTA_RECEBIMENTO> rectos1 = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    rectos1 = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    rectos1 = CarregaRecebimento().ToList();
                }
                List<CONSULTA_RECEBIMENTO> rectos = rectos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasRec = rectos.Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();

                // Merge das datas
                List<DateTime> datas = datasPag.Union(datasRec).OrderBy(d => d).ToList();

                // Monta lista final
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 contaPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date).Count();
                    Decimal? somaPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date).Sum(p => p.COPA_VL_PAGO);

                    Int32 contaRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Count();
                    Decimal? somaRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).Sum(p => p.CORE_VL_VALOR);

                    Decimal? saldo = somaRec - somaPag;
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.ValorDec = somaPag.Value;
                    mod.ValorDec1 = somaRec.Value;
                    mod.ValorDec2 = saldo.Value;
                    lista.Add(mod);
                }

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos x Recebimentos - Total por Data", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos x Recebimentos - Total por Data", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Data de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Saldo (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.DataEmissao.ToShortDateString(), meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.ValorDec2 > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 == 0)
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 < 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont4))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    totalPag += item.ValorDec;
                    totalRec += item.ValorDec1;
                    totalSaldo += item.ValorDec2;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;


                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPag.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalRec.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                if (totalSaldo > 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo == 0)
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo < 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont5))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoRecebimentoTotalMes()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                DateTime limite = DateTime.Today.Date.AddMonths(-12);

                String nomeRel = "PagamentoRecebimentoTotalMesLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                Font meuFont5 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

                Decimal? totalPag = 0;
                Decimal? totalRec = 0;
                Decimal? totalSaldo = 0;
                Decimal? itens = 0;

                // Carrega dados Pagto
                List<CONSULTA_PAGAMENTO> pagtos1 = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos1 = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos1 = CarregaPagamento().ToList();
                }
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO != null).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();

                // Carrega dados Recto
                List<CONSULTA_RECEBIMENTO> rectos1 = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    rectos1 = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    rectos1 = CarregaRecebimento().ToList();
                }
                List<CONSULTA_RECEBIMENTO> rectos = rectos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO != null).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();

                // Merge das datas
                List<DateTime> datas = datasPag.Union(datasRec).OrderBy(d => d).ToList();

                // Processa lista
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
                            Decimal? somaPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.COPA_DT_PAGAMENTO > limite).Sum(p => p.COPA_VL_PAGO.Value);
                            Int32 numPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Month == item.Month & p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year & p.COPA_DT_PAGAMENTO > limite).Count();

                            Decimal? somaRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Month == item.Month & p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year & p.CORE_DT_RECEBIMENTO > limite).Sum(p => p.CORE_VL_VALOR.Value);
                            Int32 numRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Month == item.Month & p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year & p.CORE_DT_RECEBIMENTO > limite).Count();
                            
                            Decimal? saldo = somaRec - somaPag;
                            ModeloViewModel mod = new ModeloViewModel();
                            mod.Nome = mes2;
                            mod.ValorDec = somaPag.Value;
                            mod.ValorDec1 = somaRec.Value;
                            mod.ValorDec2 = saldo.Value;
                            listaMes.Add(mod);
                            mesFeito2 = item.Month.ToString() + "/" + item.Year.ToString();
                        }
                    }
                }
                listaMes = listaMes.OrderBy(p => p.DataEmissao).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos e Recebimentos - Total por Mès", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos e Recebimentos - Total por Mês", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Mês de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Saldo (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
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
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    if (item.ValorDec2 > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 == 0)
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 < 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont4))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    totalPag += item.ValorDec;
                    totalRec += item.ValorDec1;
                    totalSaldo += item.ValorDec2;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;


                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPag.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalRec.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                if (totalSaldo > 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo == 0)
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo < 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont5))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                pdfDoc.Add(table1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoRecebimentoTotalAno()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoRecebimentoTotalAnoLista" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                Font meuFont5 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

                Decimal? totalPag = 0;
                Decimal? totalRec = 0;
                Decimal? totalSaldo = 0;
                Decimal? itens = 0;

                // Carrega dados
                List<CONSULTA_PAGAMENTO> pagtos1 = CarregaPagamento();
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO != null).Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();

                List<CONSULTA_RECEBIMENTO> rectos1 = CarregaRecebimento();
                List<CONSULTA_RECEBIMENTO> rectos = rectos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO != null).Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();

                // Merge das datas
                List<DateTime> datas = datasPag.Union(datasRec).OrderBy(d => d).ToList();

                // Procesa lista
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                String ano2 = null;
                String anoFeito2 = null;
                foreach (DateTime item in datas)
                {
                    ano2 = item.Year.ToString();
                    if (ano2 != anoFeito2)
                    {
                        Decimal? somaPag = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year).Sum(p => p.COPA_VL_PAGO.Value);
                        Int32 num2 = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date.Year == item.Year).Count();
                        Decimal? somaRec = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year).Sum(p => p.CORE_VL_VALOR.Value);
                        Int32 num3 = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date.Year == item.Year).Count();
                        
                        Decimal? saldo = somaRec - somaPag;
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.Nome = ano2;
                        mod.ValorDec = somaPag.Value;
                        mod.ValorDec1 = somaRec.Value;
                        mod.ValorDec2 = saldo.Value;
                        listaMes.Add(mod);

                        anoFeito2 = item.Year.ToString();
                    }
                }

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos e Recebimentos - Total por Ano", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos e Recebimentos - Total por Ano", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 80f, 80f, 80f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Ano de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Pagamentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Total de Recebimentos (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Saldo (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in listaMes)
                {
                    cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.ValorDec), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.ValorDec2 > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 == 0)
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.ValorDec2 < 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec2), meuFont4))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    totalPag += item.ValorDec;
                    totalRec += item.ValorDec1;
                    totalSaldo += item.ValorDec2;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] { 60f, 80f, 80f, 80f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;


                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPag.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalRec.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);

                if (totalSaldo > 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo == 0)
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont3))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                else if (totalSaldo < 0)
                {
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalSaldo.Value), meuFont5))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 1;
                    table1.AddCell(cell);
                }
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

        public ActionResult GerarListagemPagamentoRecebimentoTotalDetalhado()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);

                String nomeRel = "PagamentoRecebimentoTotalDetalhe" + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont3 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
                Font meuFont4 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.RED);
                Font meuFont5 = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

                Decimal? num = 0;
                Decimal? totalPag = 0;
                Decimal? totalRec = 0;
                Decimal? totalSaldo = 0;
                Decimal? itens = 0;

                // Carrega dados Pagto
                List<CONSULTA_PAGAMENTO> pagtos1 = new List<CONSULTA_PAGAMENTO>();
                if (Session["ListaPagamento"] != null)
                {
                    pagtos1 = (List<CONSULTA_PAGAMENTO>)Session["ListaPagamento"];
                }
                else
                {
                    pagtos1 = CarregaPagamento().ToList();
                }
                List<CONSULTA_PAGAMENTO> pagtos = pagtos1.Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                List<DateTime> datasPag = pagtos.Select(p => p.COPA_DT_PAGAMENTO.Value.Date).Distinct().ToList();

                // Carrega dados Recto
                List<CONSULTA_RECEBIMENTO> rectos1 = new List<CONSULTA_RECEBIMENTO>();
                if (Session["ListaRecebimento"] != null)
                {
                    rectos1 = (List<CONSULTA_RECEBIMENTO>)Session["ListaRecebimento"];
                }
                else
                {
                    rectos1 = CarregaRecebimento().ToList();
                }
                List<CONSULTA_RECEBIMENTO> rectos = rectos1.Where(p => p.CORE_DT_RECEBIMENTO != null).ToList();
                List<DateTime> datasRec = rectos.Select(p => p.CORE_DT_RECEBIMENTO.Value.Date).Distinct().ToList();

                // Merge das datas
                List<DateTime> datas = datasPag.Union(datasRec).OrderBy(d => d).ToList();

                // Monta lista final
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    List<CONSULTA_PAGAMENTO> pags = pagtos.Where(p => p.COPA_DT_PAGAMENTO.Value.Date == item.Date).ToList();
                    foreach (CONSULTA_PAGAMENTO pag in pags)
                    {
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.ValorDec = pag.COPA_VL_PAGO.Value;
                        mod.ValorDec1 = 0;
                        mod.Nome = pag.COPA_NM_NOME;
                        mod.Nome1 = pag.COPA_GU_GUID;
                        mod.Valor = 1;
                        lista.Add(mod);
                    }
                    List<CONSULTA_RECEBIMENTO> recs = rectos.Where(p => p.CORE_DT_RECEBIMENTO.Value.Date == item.Date).ToList();
                    foreach (CONSULTA_RECEBIMENTO rec in recs)
                    {
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.ValorDec1 = rec.CORE_VL_VALOR.Value;
                        mod.ValorDec = 0;
                        mod.Nome = rec.CORE_NM_RECEBIMENTO;
                        mod.Nome1 = rec.CORE_GU_GUID;
                        mod.Valor = 2;
                        lista.Add(mod);
                    }
                }

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

                    cell1 = new PdfPCell(new Paragraph("Pagamentos x Recebimentos - Detalhado", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Pagamentos x Recebimentos - Detalhado", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 60f, 60f, 80f, 80f, 200f, 100f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Data de Referência", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Pagamento (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Recebimento (R$)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Histórico", meuFont))
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
                DateTime? antes = null;

                foreach (ModeloViewModel item in lista)
                {
                    if (antes != item.DataEmissao & antes != null)
                    {
                        cell = new PdfPCell(new Paragraph(" ", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            Colspan = 6
                        };
                        table.AddCell(cell);
                    }

                    if (item.DataEmissao != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.DataEmissao.ToShortDateString(), meuFont))
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

                    if (item.Valor == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Pagamento", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);

                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Recebimento", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec1), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome1, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    totalPag += item.ValorDec;
                    totalRec += item.ValorDec1;
                    antes = item.DataEmissao;
                }
                pdfDoc.Add(table);

                // Grid - TOTAIS
                PdfPTable table1 = new PdfPTable(new float[] {  60f, 60f, 80f, 80f, 200f, 100f });
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = 0;
                table1.SpacingBefore = 1f;
                table1.SpacingAfter = 1f;


                cell = new PdfPCell(new Paragraph("TOTAIS", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalPag.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(totalRec.Value), meuFont3))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 1;
                table1.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.Colspan = 2;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
                pdfDoc.Add(table1);


                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                Session["NivelPaciente"] = 1;
                return RedirectToAction("MontarTelaPagamento");
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

    }
}