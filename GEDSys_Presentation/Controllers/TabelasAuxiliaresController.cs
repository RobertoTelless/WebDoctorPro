    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using ModelServices.Interfaces.Repositories;


namespace GEDSys_Presentation.Controllers
{
    public class TabelasAuxiliaresController : Controller
    {
        private readonly ITipoPacienteAppService tpApp;
        private readonly ILogAppService logApp;
        private readonly IConvenioAppService conApp;
        private readonly ITipoExameAppService teApp;
        private readonly ITipoAtestadoAppService taApp;
        private readonly IUsuarioAppService baseApp;
        private readonly ILaboratorioAppService labApp;
        private readonly IEspecialidadeAppService espApp;
        private readonly IFormaRecebimentoAppService foreApp;
        private readonly ITipoPagamentoAppService tipaApp;
        private readonly ITipoValorConsultaAppService ticoApp;
        private readonly ITipoValorServicoAppService tiseApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly ISubcategoriaProdutoAppService spApp;
        private readonly IUnidadeAppService uniApp;

        TIPO_PACIENTE objetoTP = new TIPO_PACIENTE();
        TIPO_PACIENTE objetoAntesTP = new TIPO_PACIENTE();
        List<TIPO_PACIENTE> listaMasterTP = new List<TIPO_PACIENTE>();
        CONVENIO objetoCon = new CONVENIO();
        CONVENIO objetoAntesCon = new CONVENIO();
        List<CONVENIO> listaMasterCon = new List<CONVENIO>();
        TIPO_EXAME objetoTE = new TIPO_EXAME();
        TIPO_EXAME objetoAntesTE = new TIPO_EXAME();
        List<TIPO_EXAME> listaMasterTE = new List<TIPO_EXAME>();
        TIPO_ATESTADO objetoTA = new TIPO_ATESTADO();
        TIPO_ATESTADO objetoAntesTA = new TIPO_ATESTADO();
        List<TIPO_ATESTADO> listaMasterTA = new List<TIPO_ATESTADO>();
        CONVENIO objetoCV = new CONVENIO();
        CONVENIO objetoAntesCV = new CONVENIO();
        List<CONVENIO> listaMasterCV = new List<CONVENIO>();
        LABORATORIO objetoLab = new LABORATORIO();
        LABORATORIO objetoAntesLab = new LABORATORIO();
        List<LABORATORIO> listaMasterLab = new List<LABORATORIO>();
        ESPECIALIDADE objetoEsp = new ESPECIALIDADE();
        ESPECIALIDADE objetoAntesEsp = new ESPECIALIDADE();
        List<ESPECIALIDADE> listaMasterEsp = new List<ESPECIALIDADE>();
        TIPO_CARTEIRA_CLASSE objetoTic = new TIPO_CARTEIRA_CLASSE();
        TIPO_CARTEIRA_CLASSE objetoAntesTic = new TIPO_CARTEIRA_CLASSE();
        List<TIPO_CARTEIRA_CLASSE> listaMasterTic = new List<TIPO_CARTEIRA_CLASSE>();
        FORMA_RECEBIMENTO objetoFore = new FORMA_RECEBIMENTO();
        FORMA_RECEBIMENTO objetoAntesFore = new FORMA_RECEBIMENTO();
        List<FORMA_RECEBIMENTO> listaMasterFore = new List<FORMA_RECEBIMENTO>();
        TIPO_PAGAMENTO objetoTipa = new TIPO_PAGAMENTO();
        TIPO_PAGAMENTO objetoAntesTipa = new TIPO_PAGAMENTO();
        List<TIPO_PAGAMENTO> listaMasterTipa = new List<TIPO_PAGAMENTO>();
        TIPO_VALOR_CONSULTA objetoTico = new TIPO_VALOR_CONSULTA();
        TIPO_VALOR_CONSULTA objetoAntesTico = new TIPO_VALOR_CONSULTA();
        List<TIPO_VALOR_CONSULTA> listaMasterTico = new List<TIPO_VALOR_CONSULTA>();
        TIPO_SERVICO_CONSULTA objetoTise = new TIPO_SERVICO_CONSULTA();
        TIPO_SERVICO_CONSULTA objetoAntesTise = new TIPO_SERVICO_CONSULTA();
        List<TIPO_SERVICO_CONSULTA> listaMasterTise = new List<TIPO_SERVICO_CONSULTA>();
        CATEGORIA_PRODUTO objetoCatProd = new CATEGORIA_PRODUTO();
        CATEGORIA_PRODUTO objetoAntesCatProd = new CATEGORIA_PRODUTO();
        List<CATEGORIA_PRODUTO> listaMasterCatProd = new List<CATEGORIA_PRODUTO>();
        SUBCATEGORIA_PRODUTO objetoSubProd = new SUBCATEGORIA_PRODUTO();
        SUBCATEGORIA_PRODUTO objetoAntesSubProd = new SUBCATEGORIA_PRODUTO();
        List<SUBCATEGORIA_PRODUTO> listaMasterSubProd = new List<SUBCATEGORIA_PRODUTO>();

#pragma warning disable CS0169 // O campo "TabelasAuxiliaresController.extensao" nunca é usado
        String extensao;
#pragma warning restore CS0169 // O campo "TabelasAuxiliaresController.extensao" nunca é usado

        public TabelasAuxiliaresController(ILogAppService logApps, IUsuarioAppService usuApps, ITipoPacienteAppService tpApps, IConvenioAppService conApps, ITipoExameAppService teApps, ITipoAtestadoAppService taApps, ILaboratorioAppService labApps, IEspecialidadeAppService espApps, IFormaRecebimentoAppService foreApps, ITipoPagamentoAppService tipaApps, ITipoValorConsultaAppService ticoApps, ITipoValorServicoAppService tiseApps, ICategoriaProdutoAppService cpApps, ISubcategoriaProdutoAppService spApps, IUnidadeAppService uniApps)
        {
            logApp = logApps;
            taApp = taApps;
            baseApp = usuApps;
            tpApp = tpApps;
            teApp = teApps;
            conApp = conApps;
            labApp = labApps;
            espApp = espApps;
            foreApp = foreApps;
            tipaApp = tipaApps;
            ticoApp = ticoApps;
            tiseApp = tiseApps;
            cpApp = cpApps;
            spApp = spApps;
            uniApp = uniApps;
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

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaTabelasAuxiliares", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoPaciente()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoPaciente"] == null)
                {
                    listaMasterTP = tpApp.GetAllItens(idAss);
                    Session["ListaTipoPaciente"] = listaMasterTP;
                }
                ViewBag.Listas = (List<TIPO_PACIENTE>)Session["ListaTipoPaciente"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0503", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0504", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTP = new TIPO_PACIENTE();
                return View(objetoTP);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoPaciente"] = null;
            return RedirectToAction("MontarTelaTipoPaciente");
        }

        public ActionResult MostrarTudoTipoPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTP = tpApp.GetAllItensAdm(idAss);
                Session["ListaTipoPaciente"] = listaMasterTP;
                return RedirectToAction("MontarTelaTipoPaciente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoPaciente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoPaciente");
        }

        [HttpGet]
        public ActionResult IncluirTipoPaciente()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_PACIENTE item = new TIPO_PACIENTE();
            TipoPacienteViewModel vm = Mapper.Map<TIPO_PACIENTE, TipoPacienteViewModel>(item);
            vm.ASSI_C_DID = usuario.ASSI_CD_ID;
            vm.TIPA_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoPaciente(TipoPacienteViewModel vm)
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
                    // Sanitizar
                    vm.TIPA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIPA_NM_NOME);

                    // Executa a operação
                    TIPO_PACIENTE item = Mapper.Map<TipoPacienteViewModel, TIPO_PACIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaTipoPaciente");
                    }
                    Session["IdVolta"] = item.TIPA_CD_ID;

                    // Sucesso
                    listaMasterTP = new List<TIPO_PACIENTE>();
                    Session["ListaTipoPaciente"] = null;
                    Session["TipoPacienteAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoPaciente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_PACIENTE item = tpApp.GetItemById(id);
                objetoAntesTP = item;
                Session["TipoPaciente"] = item;
                Session["IdTipoPaciente"] = id;
                TipoPacienteViewModel vm = Mapper.Map<TIPO_PACIENTE, TipoPacienteViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoPaciente(TipoPacienteViewModel vm)
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
                    // Sanitizar
                    vm.TIPA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIPA_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_PACIENTE item = Mapper.Map<TipoPacienteViewModel, TIPO_PACIENTE>(vm);
                    Int32 volta = tpApp.ValidateEdit(item, objetoAntesTP, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTP = new List<TIPO_PACIENTE>();
                    Session["ListaTipoPaciente"] = null;
                    Session["TipoPacienteAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoPaciente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_PACIENTE item = tpApp.GetItemById(id);
                objetoAntesTP = item;
                item.TIPA_IN_ATIVO = 0;
                Int32 volta = tpApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoPaciente");
                }
                listaMasterTP = new List<TIPO_PACIENTE>();
                Session["ListaTipoPaciente"] = null;
                Session["TipoPacienteAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoPaciente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoPaciente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_PACIENTE item = tpApp.GetItemById(id);
                item.TIPA_IN_ATIVO = 1;
                objetoAntesTP = item;
                Int32 volta = tpApp.ValidateReativar(item, usuario);
                listaMasterTP = new List<TIPO_PACIENTE>();
                Session["ListaTipoPaciente"] = null;
                Session["TipoPacienteAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoPaciente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoExame()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoExame"] == null)
                {
                    listaMasterTE = teApp.GetAllItens(idAss);
                    Session["ListaTipoExame"] = listaMasterTE;
                }
                ViewBag.Listas = (List<TIPO_EXAME>)Session["ListaTipoExame"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0505", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0506", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTE = new TIPO_EXAME();
                return View(objetoTE);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoExame()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoPaciente"] = null;
            return RedirectToAction("MontarTelaTipoExame");
        }

        public ActionResult MostrarTudoTipoExame()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTE = teApp.GetAllItensAdm(idAss);
                Session["ListaTipoExame"] = listaMasterTE;
                return RedirectToAction("MontarTelaTipoExame");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoExame()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoExame");
        }

        [HttpGet]
        public ActionResult IncluirTipoExame()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_EXAME item = new TIPO_EXAME();
            TipoExameViewModel vm = Mapper.Map<TIPO_EXAME, TipoExameViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIEX_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoExame(TipoExameViewModel vm)
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
                    // Sanitizar
                    vm.TIEX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIEX_NM_NOME);

                    // Executa a operação
                    TIPO_EXAME item = Mapper.Map<TipoExameViewModel, TIPO_EXAME>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = teApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaTipoExame");
                    }
                    Session["IdVolta"] = item.TIEX_CD_ID;

                    // Sucesso
                    listaMasterTE = new List<TIPO_EXAME>();
                    Session["ListaTipoExame"] = null;
                    Session["TipoExameAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoExame");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_EXAME item = teApp.GetItemById(id);
                objetoAntesTE = item;
                Session["TipoExame"] = item;
                Session["IdTipoExame"] = id;
                TipoExameViewModel vm = Mapper.Map<TIPO_EXAME, TipoExameViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarTipoExame(TipoExameViewModel vm)
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
                    // Sanitizar
                    vm.TIEX_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIEX_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_EXAME item = Mapper.Map<TipoExameViewModel, TIPO_EXAME>(vm);
                    Int32 volta = teApp.ValidateEdit(item, objetoAntesTE, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTE = new List<TIPO_EXAME>();
                    Session["ListaTipoExame"] = null;
                    Session["TipoExameAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoExame");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_EXAME item = teApp.GetItemById(id);
                objetoAntesTE = item;
                item.TIEX_IN_ATIVO = 0;
                Int32 volta = teApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoExame");
                }
                listaMasterTE = new List<TIPO_EXAME>();
                Session["ListaTipoExame"] = null;
                Session["TipoExameAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoExame");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoExame(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_EXAME item = teApp.GetItemById(id);
                item.TIEX_IN_ATIVO = 1;
                objetoAntesTE = item;
                Int32 volta = teApp.ValidateReativar(item, usuario);
                listaMasterTE = new List<TIPO_EXAME>();
                Session["ListaTipoExame"] = null;
                Session["TipoExameAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoExame");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoAtestado()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoAtestado"] == null)
                {
                    listaMasterTA = taApp.GetAllItens(idAss);
                    Session["ListaTipoAtestado"] = listaMasterTA;
                }
                ViewBag.Listas = (List<TIPO_ATESTADO>)Session["ListaTipoAtestado"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0507", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0508", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTA = new TIPO_ATESTADO();
                return View(objetoTA);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoAtestado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoPaciente"] = null;
            return RedirectToAction("MontarTelaTipoAtestado");
        }

        public ActionResult MostrarTudoTipoAtestado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTA = taApp.GetAllItensAdm(idAss);
                Session["ListaTipoAtestado"] = listaMasterTA;
                return RedirectToAction("MontarTelaTipoAtestado");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoAtestado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoAtestado");
        }

        [HttpGet]
        public ActionResult IncluirTipoAtestado()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_ATESTADO item = new TIPO_ATESTADO();
            TipoAtestadoViewModel vm = Mapper.Map<TIPO_ATESTADO, TipoAtestadoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIAT_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoAtestado(TipoAtestadoViewModel vm)
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
                    // Sanitizar
                    vm.TIAT_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.TIAT_NM_NOME);
                    vm.TIAT_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.TIAT_TX_TEXTO);

                    // Executa a operação
                    TIPO_ATESTADO item = Mapper.Map<TipoAtestadoViewModel, TIPO_ATESTADO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = taApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaTipoAtestado");
                    }
                    Session["IdVolta"] = item.TIAT_CD_ID;

                    // Sucesso
                    listaMasterTA = new List<TIPO_ATESTADO>();
                    Session["ListaTipoAtestado"] = null;
                    Session["TipoAtestadoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoAtestado");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_ATESTADO item = taApp.GetItemById(id);
                objetoAntesTA = item;
                Session["TipoAtestado"] = item;
                Session["IdTipoAtestado"] = id;
                TipoAtestadoViewModel vm = Mapper.Map<TIPO_ATESTADO, TipoAtestadoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarTipoAtestado(TipoAtestadoViewModel vm)
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
                    // Sanitizar
                    vm.TIAT_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.TIAT_NM_NOME);
                    vm.TIAT_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.TIAT_TX_TEXTO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_ATESTADO item = Mapper.Map<TipoAtestadoViewModel, TIPO_ATESTADO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoAntesTA, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTA = new List<TIPO_ATESTADO>();
                    Session["ListaTipoAtestado"] = null;
                    Session["TipoAtestadoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoAtestado");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_ATESTADO item = taApp.GetItemById(id);
                objetoAntesTA = item;
                item.TIAT_IN_ATIVO = 0;
                Int32 volta = taApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoAtestado");
                }
                listaMasterTA = new List<TIPO_ATESTADO>();
                Session["ListaTipoAtestado"] = null;
                Session["TipoAtestadoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoAtestado");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoAtestado(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_ATESTADO item = taApp.GetItemById(id);
                item.TIAT_IN_ATIVO = 1;
                objetoAntesTA = item;
                Int32 volta = taApp.ValidateReativar(item, usuario);
                listaMasterTA = new List<TIPO_ATESTADO>();
                Session["ListaTipoAtestado"] = null;
                Session["TipoAtestadoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoAtestado");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult  MontarTelaConvenio()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaConvenio"] == null)
                {
                    listaMasterCV = conApp.GetAllItens(idAss);
                    Session["ListaConvenio"] = listaMasterCV;
                }
                ViewBag.Listas = (List<CONVENIO>)Session["ListaConvenio"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0509", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0510", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoCV = new CONVENIO();
                return View(objetoCV);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroConvenio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaConvenio"] = null;
            return RedirectToAction("MontarTelaConvenio");
        }

        public ActionResult MostrarTudoConvenio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterCV = conApp.GetAllItensAdm(idAss);
                Session["ListaConvenio"] = listaMasterCV;
                return RedirectToAction("MontarTelaConvenio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseConvenio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaConvenio");
        }

        [HttpGet]
        public ActionResult IncluirConvenio()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CONVENIO item = new CONVENIO();
            ConvenioViewModel vm = Mapper.Map<CONVENIO, ConvenioViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CONV_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirConvenio(ConvenioViewModel vm)
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
                    // Sanitizar
                    vm.CONV_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CONV_NM_NOME);

                    // Executa a operação
                    CONVENIO item = Mapper.Map<ConvenioViewModel, CONVENIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = conApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaConvenio");
                    }
                    Session["IdVolta"] = item.CONV_CD_ID;

                    // Sucesso
                    listaMasterCV = new List<CONVENIO>();
                    Session["ListaConvenio"] = null;
                    Session["ConvenioAlterada"] = 1;
                    return RedirectToAction("VoltarBaseConvenio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarConvenio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                CONVENIO item = conApp.GetItemById(id);
                objetoAntesCV = item;
                Session["Convenio"] = item;
                Session["IdConvenio"] = id;
                ConvenioViewModel vm = Mapper.Map<CONVENIO, ConvenioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarConvenio(ConvenioViewModel vm)
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
                    // Sanitizar
                    vm.CONV_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CONV_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONVENIO item = Mapper.Map<ConvenioViewModel, CONVENIO>(vm);
                    Int32 volta = conApp.ValidateEdit(item, objetoAntesCV, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCV = new List<CONVENIO>();
                    Session["ListaConvenio"] = null;
                    Session["ConvenioAlterada"] = 1;
                    return RedirectToAction("MontarTelaConvenio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirConvenio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                CONVENIO item = conApp.GetItemById(id);
                objetoAntesCV = item;
                item.CONV_IN_ATIVO = 0;
                Int32 volta = conApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaConvenio");
                }
                listaMasterCV = new List<CONVENIO>();
                Session["ListaConvenio"] = null;
                Session["ConvenioAlterada"] = 1;
                return RedirectToAction("MontarTelaConvenio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarConvenio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                CONVENIO item = conApp.GetItemById(id);
                item.CONV_IN_ATIVO = 1;
                objetoAntesCV = item;
                Int32 volta = conApp.ValidateReativar(item, usuario);
                listaMasterCV = new List<CONVENIO>();
                Session["ListaConvenio"] = null;
                Session["ConvenioAlterada"] = 1;
                return RedirectToAction("MontarTelaConvenio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaLaboratorio()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaLaboratorio"] == null)
                {
                    listaMasterLab = labApp.GetAllItens(idAss);
                    Session["ListaLaboratorio"] = listaMasterLab;
                }
                ViewBag.Listas = (List<LABORATORIO>)Session["ListaLaboratorio"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0536", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0599", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoLab = new LABORATORIO();
                return View(objetoLab);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroLaboratorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaLaboratorio"] = null;
            return RedirectToAction("MontarTelaLaboratorio");
        }

        public ActionResult MostrarTudoLaboratorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterLab = labApp.GetAllItensAdm(idAss);
                Session["ListaLaboratorio"] = listaMasterLab;
                return RedirectToAction("MontarTelaLaboratorio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseLaboratorio()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaLaboratorio");
        }

        [HttpGet]
        public ActionResult IncluirLaboratorio()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            LABORATORIO item = new LABORATORIO();
            LaboratorioViewModel vm = Mapper.Map<LABORATORIO, LaboratorioViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.LABS_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirLaboratorio(LaboratorioViewModel vm)
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
                    // Sanitizar
                    vm.LABS_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.LABS_NM_NOME);
                    vm.LABS_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.LABS_NM_CIDADE);
                    vm.LABS_LK_LINK_PESSOAL = CrossCutting.UtilitariosGeral.CleanStringLink(vm.LABS_LK_LINK_PESSOAL);
                    vm.LABS_LK_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.LABS_LK_LINK);

                    // Executa a operação
                    LABORATORIO item = Mapper.Map<LaboratorioViewModel, LABORATORIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = labApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaLaboratorio");
                    }
                    Session["IdVolta"] = item.LABS_CD_ID;

                    // Sucesso
                    listaMasterLab = new List<LABORATORIO>();
                    Session["ListaLaboratorio"] = null;
                    Session["LaboratorioAlterada"] = 1;
                    return RedirectToAction("VoltarBaseLaboratorio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarLaboratorio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                ViewBag.UF = new SelectList(CarregaUF(), "UF_CD_ID", "UF_SG_SIGLA");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                LABORATORIO item = labApp.GetItemById(id);
                objetoAntesLab = item;
                Session["Laboratorio"] = item;
                Session["IdLaboratorio"] = id;
                LaboratorioViewModel vm = Mapper.Map<LABORATORIO, LaboratorioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarLaboratorio(LaboratorioViewModel vm)
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
                    // Sanitizar
                    vm.LABS_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.LABS_NM_NOME);
                    vm.LABS_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.LABS_NM_CIDADE);
                    vm.LABS_LK_LINK_PESSOAL = CrossCutting.UtilitariosGeral.CleanStringLink(vm.LABS_LK_LINK_PESSOAL);
                    vm.LABS_LK_LINK = CrossCutting.UtilitariosGeral.CleanStringLink(vm.LABS_LK_LINK);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    LABORATORIO item = Mapper.Map<LaboratorioViewModel, LABORATORIO>(vm);
                    Int32 volta = labApp.ValidateEdit(item, objetoAntesLab, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterLab = new List<LABORATORIO>();
                    Session["ListaLaboratorio"] = null;
                    Session["LaboratorioAlterada"] = 1;
                    return RedirectToAction("MontarTelaLaboratorio");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirLaboratorio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                LABORATORIO item = labApp.GetItemById(id);
                objetoAntesLab = item;
                item.LABS_IN_ATIVO = 0;
                Int32 volta = labApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaLaboratorio");
                }
                listaMasterTA = new List<TIPO_ATESTADO>();
                Session["ListaLaboratorio"] = null;
                Session["LaboratorioAlterada"] = 1;
                return RedirectToAction("MontarTelaLaboratorio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarLaboratorio(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                LABORATORIO item = labApp.GetItemById(id);
                item.LABS_IN_ATIVO = 1;
                objetoAntesLab = item;
                Int32 volta = labApp.ValidateReativar(item, usuario);
                listaMasterLab = new List<LABORATORIO>();
                Session["ListaLaboratorio"] = null;
                Session["LaboratorioAlterada"] = 1;
                return RedirectToAction("MontarTelaLaboratorio");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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
                    conf = labApp.GetAllUF();
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaEspecialidade()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaEspecialidade"] == null)
                {
                    listaMasterEsp = espApp.GetAllItens(idAss);
                    Session["ListaEspecialidade"] = listaMasterEsp;
                }
                ViewBag.Listas = (List<ESPECIALIDADE>)Session["ListaEspecialidade"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0566", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0567", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoEsp = new ESPECIALIDADE();
                return View(objetoEsp);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroEspecialidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaEspecialidade"] = null;
            return RedirectToAction("MontarTelaEspecialidade");
        }

        public ActionResult MostrarTudoEspecialidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterEsp = espApp.GetAllItensAdm(idAss);
                Session["ListaEspecialidade"] = listaMasterEsp;
                return RedirectToAction("MontarTelaEspecialidade");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseEspecialidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaEspecialidade");
        }

        [HttpGet]
        public ActionResult IncluirEspecialidade()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            ESPECIALIDADE item = new ESPECIALIDADE();
            EspecialidadeViewModel vm = Mapper.Map<ESPECIALIDADE, EspecialidadeViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.ESPE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEspecialidade(EspecialidadeViewModel vm)
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
                    // Sanitizar
                    vm.ESPE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.ESPE_NM_NOME);

                    // Executa a operação
                    ESPECIALIDADE item = Mapper.Map<EspecialidadeViewModel, ESPECIALIDADE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = espApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaEspecialidade");
                    }
                    Session["IdVolta"] = item.ESPE_CD_ID;

                    // Sucesso
                    listaMasterEsp = new List<ESPECIALIDADE>();
                    Session["ListaEspecialidade"] = null;
                    Session["EspecialidadeAlterada"] = 1;
                    return RedirectToAction("VoltarBaseEspecialidade");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarEspecialidade(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                ESPECIALIDADE item = espApp.GetItemById(id);
                objetoAntesEsp = item;
                Session["Especialidade"] = item;
                Session["IdEspecialidade"] = id;
                EspecialidadeViewModel vm = Mapper.Map<ESPECIALIDADE, EspecialidadeViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarEspecialidade(EspecialidadeViewModel vm)
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
                    // Sanitizar
                    vm.ESPE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.ESPE_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ESPECIALIDADE item = Mapper.Map<EspecialidadeViewModel, ESPECIALIDADE>(vm);
                    Int32 volta = espApp.ValidateEdit(item, objetoAntesEsp, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterEsp = new List<ESPECIALIDADE>();
                    Session["ListaEspecialidade"] = null;
                    Session["EspecialidadeAlterada"] = 1;
                    return RedirectToAction("MontarTelaEspecialidade");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirEspecialidade(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                ESPECIALIDADE item = espApp.GetItemById(id);
                objetoAntesEsp = item;
                item.ESPE_IN_ATIVO = 0;
                Int32 volta = espApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaEspecialidade");
                }
                listaMasterEsp = new List<ESPECIALIDADE>();
                Session["ListaEspecialidade"] = null;
                Session["EspecialidadeAlterada"] = 1;
                return RedirectToAction("MontarTelaEspecialidade");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarEspecialidade(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                ESPECIALIDADE item = espApp.GetItemById(id);
                item.ESPE_IN_ATIVO = 1;
                objetoAntesEsp = item;
                Int32 volta = espApp.ValidateReativar(item, usuario);
                listaMasterEsp = new List<ESPECIALIDADE>();
                Session["ListaEspecialidade"] = null;
                Session["EspecialidadeAlterada"] = 1;
                return RedirectToAction("MontarTelaEspecialidade");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaFormaRecebimento()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaFormaRecebimento"] == null)
                {
                    listaMasterFore = foreApp.GetAllItens(idAss);
                    Session["ListaFormaRecebimento"] = listaMasterFore;
                }
                ViewBag.Listas = (List<FORMA_RECEBIMENTO>)Session["ListaFormaRecebimento"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0573", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0574", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoFore = new FORMA_RECEBIMENTO();
                return View(objetoFore);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroFormaRecebimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFormaRecebimento"] = null;
            return RedirectToAction("MontarTelaFormaRecebimento");
        }

        public ActionResult MostrarTudoFormaRecebimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterFore = foreApp.GetAllItensAdm(idAss);
                Session["ListaFormaRecebimento"] = listaMasterTA;
                return RedirectToAction("MontarTelaFormaRecebimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseFormaRecebimento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaFormaRecebimento");
        }

        [HttpGet]
        public ActionResult IncluirFormaRecebimento()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            var padrao = new List<SelectListItem>();
            padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            FORMA_RECEBIMENTO item = new FORMA_RECEBIMENTO();
            FormaRecebimentoViewModel vm = Mapper.Map<FORMA_RECEBIMENTO, FormaRecebimentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.FORE_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            vm.FORE_IN_PADRAO = 0;
            vm.FORE_IN_FIXO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirFormaRecebimento(FormaRecebimentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var padrao = new List<SelectListItem>();
            padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.FORE_NM_FORMA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FORE_NM_FORMA);

                    // Executa a operação
                    FORMA_RECEBIMENTO item = Mapper.Map<FormaRecebimentoViewModel, FORMA_RECEBIMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = foreApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaFormaRecebimento");
                    }
                    Session["IdVolta"] = item.FORE_CD_ID;

                    // Sucesso
                    listaMasterFore = new List<FORMA_RECEBIMENTO>();
                    Session["ListaFormaRecebimento"] = null;
                    Session["FormaRecebimentoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseFormaRecebimento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarFormaRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                var padrao = new List<SelectListItem>();
                padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Padrao = new SelectList(padrao, "Value", "Text");

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0573", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara view
                FORMA_RECEBIMENTO item = foreApp.GetItemById(id);
                objetoAntesFore = item;
                Session["FormaRecebimento"] = item;
                Session["IdFormaRecebimento"] = id;
                FormaRecebimentoViewModel vm = Mapper.Map<FORMA_RECEBIMENTO, FormaRecebimentoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarFormaRecebimento(FormaRecebimentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            var padrao = new List<SelectListItem>();
            padrao.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            padrao.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Padrao = new SelectList(padrao, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Sanitizar
                    vm.FORE_NM_FORMA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.FORE_NM_FORMA);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMA_RECEBIMENTO item = Mapper.Map<FormaRecebimentoViewModel, FORMA_RECEBIMENTO>(vm);
                    Int32 volta = foreApp.ValidateEdit(item, objetoAntesFore, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0573", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterFore = new List<FORMA_RECEBIMENTO>();
                    Session["ListaFormaRecebimento"] = null;
                    Session["FormaRecebimentoAlterada"] = 1;
                    return RedirectToAction("MontarTelaFormaRecebimento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirFormaRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                FORMA_RECEBIMENTO item = foreApp.GetItemById(id);
                objetoAntesFore = item;
                item.FORE_IN_ATIVO = 0;
                Int32 volta = foreApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaFormaRecebimento");
                }
                listaMasterFore = new List<FORMA_RECEBIMENTO>();
                Session["ListaFormaRecebimento"] = null;
                Session["FormaRecebimentoAlterada"] = 1;
                return RedirectToAction("MontarTelaFormaRecebimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarFormaRecebimento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                FORMA_RECEBIMENTO item = foreApp.GetItemById(id);
                item.FORE_IN_ATIVO = 1;
                objetoAntesFore = item;
                Int32 volta = foreApp.ValidateReativar(item, usuario);
                listaMasterFore = new List<FORMA_RECEBIMENTO>();
                Session["ListaFormaRecebimento"] = null;
                Session["FormaRecebimentoAlterada"] = 1;
                return RedirectToAction("MontarTelaFormaRecebimento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoPagamento()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoPagamento"] == null)
                {
                    listaMasterTipa = tipaApp.GetAllItens(idAss);
                    Session["ListaTipoPagamento"] = listaMasterTipa;
                }
                ViewBag.Listas = (List<TIPO_PAGAMENTO>)Session["ListaTipoPagamento"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0575", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0576", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTipa = new TIPO_PAGAMENTO();
                return View(objetoTipa);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoPagamento"] = null;
            return RedirectToAction("MontarTelaTipoPagamento");
        }

        public ActionResult MostrarTudoTipoPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTipa = tipaApp.GetAllItensAdm(idAss);
                Session["ListaTipoPagamento"] = listaMasterTipa;
                return RedirectToAction("MontarTelaTipoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoPagamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoPagamento");
        }

        [HttpGet]
        public ActionResult IncluirTipoPagamento()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_PAGAMENTO item = new TIPO_PAGAMENTO();
            TipoPagamentoViewModel vm = Mapper.Map<TIPO_PAGAMENTO, TipoPagamentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIPA_IN_ATIVO = 1;
            vm.USUA_CD_ID = usuario.USUA_CD_ID;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoPagamento(TipoPagamentoViewModel vm)
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
                    // Sanitizar
                    vm.TIPA_NM_PAGAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIPA_NM_PAGAMENTO);

                    // Executa a operação
                    TIPO_PAGAMENTO item = Mapper.Map<TipoPagamentoViewModel, TIPO_PAGAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tipaApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaTipoPagamento");
                    }
                    Session["IdVolta"] = item.TIPA_CD_ID;

                    // Sucesso
                    listaMasterTipa = new List<TIPO_PAGAMENTO>();
                    Session["ListaTipoPagamento"] = null;
                    Session["TipoPagamentoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoPagamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_PAGAMENTO item = tipaApp.GetItemById(id);
                objetoAntesTipa = item;
                Session["TipoPagamento"] = item;
                Session["IdTipoPagamento"] = id;
                TipoPagamentoViewModel vm = Mapper.Map<TIPO_PAGAMENTO, TipoPagamentoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarTipoPagamento(TipoPagamentoViewModel vm)
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
                    // Sanitizar
                    vm.TIPA_NM_PAGAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIPA_NM_PAGAMENTO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_PAGAMENTO item = Mapper.Map<TipoPagamentoViewModel, TIPO_PAGAMENTO>(vm);
                    Int32 volta = tipaApp.ValidateEdit(item, objetoAntesTipa, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTipa = new List<TIPO_PAGAMENTO>();
                    Session["ListaTipoPagamento"] = null;
                    Session["TipoPagamentoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoPagamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_PAGAMENTO item = tipaApp.GetItemById(id);
                objetoAntesTipa = item;
                item.TIPA_IN_ATIVO = 0;
                Int32 volta = tipaApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoPagamento");
                }
                listaMasterTipa = new List<TIPO_PAGAMENTO>();
                Session["ListaTipoPagamento"] = null;
                Session["TipoPagamentoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoPagamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_PAGAMENTO item = tipaApp.GetItemById(id);
                item.TIPA_IN_ATIVO = 1;
                objetoAntesTipa = item;
                Int32 volta = tipaApp.ValidateReativar(item, usuario);
                listaMasterTipa = new List<TIPO_PAGAMENTO>();
                Session["ListaTipoPagamento"] = null;
                Session["TipoPagamentoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoPagamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoValorConsulta()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoValorConsulta"] == null)
                {
                    listaMasterTico = ticoApp.GetAllItens(idAss);
                    Session["ListaTipoValorConsulta"] = listaMasterTico;
                }
                ViewBag.Listas = (List<TIPO_VALOR_CONSULTA>)Session["ListaTipoValorConsulta"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0577", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0578", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTico = new TIPO_VALOR_CONSULTA();
                return View(objetoTico);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoValorConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoValorConsulta"] = null;
            return RedirectToAction("MontarTelaTipoValorConsulta");
        }

        public ActionResult MostrarTudoTipoValorConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTico = ticoApp.GetAllItensAdm(idAss);
                Session["ListaTipoValorConsulta"] = listaMasterTico;
                return RedirectToAction("MontarTelaTipoValorConsulta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoValorConsulta()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoValorConsulta");
        }

        [HttpGet]
        public ActionResult IncluirTipoValorConsulta()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_VALOR_CONSULTA item = new TIPO_VALOR_CONSULTA();
            TipoValorConsultaViewModel vm = Mapper.Map<TIPO_VALOR_CONSULTA, TipoValorConsultaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIVL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoValorConsulta(TipoValorConsultaViewModel vm)
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
                    // Sanitizar
                    vm.TIVL_NM_TIPO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIVL_NM_TIPO);

                    // Executa a operação
                    TIPO_VALOR_CONSULTA item = Mapper.Map<TipoValorConsultaViewModel, TIPO_VALOR_CONSULTA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ticoApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0577", CultureInfo.CurrentCulture));
                        Session["MensAuxiliar"] = 3;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0672", CultureInfo.CurrentCulture));
                        Session["MensAuxiliar"] = 3;
                        return View(vm);
                    }
                    Session["IdVolta"] = item.TIVL_CD_ID;

                    // Sucesso
                    listaMasterTico = new List<TIPO_VALOR_CONSULTA>();
                    Session["ListaTipoValorConsulta"] = null;
                    Session["TipoValorConsultaAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoValorConsulta");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoValorConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_VALOR_CONSULTA item = ticoApp.GetItemById(id);
                objetoAntesTico = item;
                Session["TipoValorConsulta"] = item;
                Session["IdTipoValorConsulta"] = id;
                TipoValorConsultaViewModel vm = Mapper.Map<TIPO_VALOR_CONSULTA, TipoValorConsultaViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarTipoValorConsulta(TipoValorConsultaViewModel vm)
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
                    // Sanitizar
                    vm.TIVL_NM_TIPO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIVL_NM_TIPO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_VALOR_CONSULTA item = Mapper.Map<TipoValorConsultaViewModel, TIPO_VALOR_CONSULTA>(vm);
                    Int32 volta = ticoApp.ValidateEdit(item, objetoAntesTico, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0577", CultureInfo.CurrentCulture));
                        Session["MensAuxiliar"] = 3;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0672", CultureInfo.CurrentCulture));
                        Session["MensAuxiliar"] = 3;
                        return View(vm);
                    }

                    // Sucesso
                    listaMasterTico = new List<TIPO_VALOR_CONSULTA>();
                    Session["ListaTipoValorConsulta"] = null;
                    Session["TipoValorConsultaAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoValorConsulta");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoValorConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_VALOR_CONSULTA item = ticoApp.GetItemById(id);
                objetoAntesTico= item;
                item.TIVL_IN_ATIVO = 0;
                Int32 volta = ticoApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoValorConsulta");
                }
                listaMasterTico = new List<TIPO_VALOR_CONSULTA>();
                Session["ListaTipoValorConsulta"] = null;
                Session["TipoValorConsultaAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoValorConsulta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoValorConsulta(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_VALOR_CONSULTA item = ticoApp.GetItemById(id);
                item.TIVL_IN_ATIVO = 1;
                objetoAntesTico = item;
                Int32 volta = ticoApp.ValidateReativar(item, usuario);
                listaMasterTico = new List<TIPO_VALOR_CONSULTA>();
                Session["ListaTipoValorConsulta"] = null;
                Session["TipoValorConsultaAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoValorConsulta");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoValorServico()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaTipoValorServico"] == null)
                {
                    listaMasterTise = tiseApp.GetAllItens(idAss);
                    Session["ListaTipoValorServico"] = listaMasterTise;
                }
                ViewBag.Listas = (List<TIPO_SERVICO_CONSULTA>)Session["ListaTipoValorServico"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensAuxiliar"] != null)
                {
                    if ((Int32)Session["MensAuxiliar"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0579", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensAuxiliar"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0580", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensAuxiliar"] = 0;
                objetoTise = new TIPO_SERVICO_CONSULTA();
                return View(objetoTise);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroTipoValorServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoValorServico"] = null;
            return RedirectToAction("MontarTelaTipoValorServico");
        }

        public ActionResult MostrarTudoTipoValorServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterTise = tiseApp.GetAllItensAdm(idAss);
                Session["ListaTipoValorServico"] = listaMasterTise;
                return RedirectToAction("MontarTelaTipoValorServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tab.Auxiliar";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tab.Auxiliar", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseTipoValorServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoValorServico");
        }

        [HttpGet]
        public ActionResult IncluirTipoValorServico()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_SERVICO_CONSULTA item = new TIPO_SERVICO_CONSULTA();
            TipoValorServicoViewModel vm = Mapper.Map<TIPO_SERVICO_CONSULTA, TipoValorServicoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SERV_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoValorServico(TipoValorServicoViewModel vm)
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
                    // Sanitizar
                    vm.SERV_NM_SERVICO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.SERV_NM_SERVICO);
                    vm.SERV_DS_SERVICO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.SERV_DS_SERVICO);

                    // Executa a operação
                    TIPO_SERVICO_CONSULTA item = Mapper.Map<TipoValorServicoViewModel, TIPO_SERVICO_CONSULTA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tiseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensAuxiliar"] = 3;
                        return RedirectToAction("MontarTelaTipoValorServico");
                    }
                    Session["IdVolta"] = item.SERV_CD_ID;

                    // Sucesso
                    listaMasterTise = new List<TIPO_SERVICO_CONSULTA>();
                    Session["ListaTipoValorServico"] = null;
                    Session["TipoValorServicoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseTipoValorServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoValorServico(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Prepara view
                TIPO_SERVICO_CONSULTA item = tiseApp.GetItemById(id);
                objetoAntesTise = item;
                Session["TipoValorServico"] = item;
                Session["IdTipoValorServico"] = id;
                TipoValorServicoViewModel vm = Mapper.Map<TIPO_SERVICO_CONSULTA, TipoValorServicoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarTipoValorServico(TipoValorServicoViewModel vm)
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
                    // Sanitizar
                    vm.SERV_NM_SERVICO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.SERV_NM_SERVICO);
                    vm.SERV_DS_SERVICO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.SERV_DS_SERVICO);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_SERVICO_CONSULTA item = Mapper.Map<TipoValorServicoViewModel, TIPO_SERVICO_CONSULTA>(vm);
                    Int32 volta = tiseApp.ValidateEdit(item, objetoAntesTise, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTise = new List<TIPO_SERVICO_CONSULTA>();
                    Session["ListaTipoValorServico"] = null;
                    Session["TipoValorServicoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoValorServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoValorServico(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_SERVICO_CONSULTA item = tiseApp.GetItemById(id);
                objetoAntesTise = item;
                item.SERV_IN_ATIVO = 0;
                Int32 volta = tiseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensAuxiliar"] = 4;
                    return RedirectToAction("MontarTelaTipoValorServico");
                }
                listaMasterTise = new List<TIPO_SERVICO_CONSULTA>();
                Session["ListaTipoValorServico"] = null;
                Session["TipoValorServicoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoValorServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoValorServico(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Tabelas";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                TIPO_SERVICO_CONSULTA item = tiseApp.GetItemById(id);
                item.SERV_IN_ATIVO = 1;
                objetoAntesTise = item;
                Int32 volta = tiseApp.ValidateReativar(item, usuario);
                listaMasterTise = new List<TIPO_SERVICO_CONSULTA>();
                Session["ListaTipoValorServico"] = null;
                Session["TipoValorServicoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoValorServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaCatProduto()
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
                if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCatProduto"] == null)
            {
                listaMasterCatProd= CarregarCatProduto().Where(p => p.CAPR_IN_SISTEMA == 6).ToList();
                Session["ListaCatProduto"] = listaMasterCatProd;
            }
            ViewBag.Listas = (List<CATEGORIA_PRODUTO>)Session["ListaCatProduto"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensCatProduto"] != null)
            {
                if ((Int32)Session["MensCatProduto"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0409", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatProduto"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0410", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatProduto"] = 1;
            Session["VoltaCatProd"] = 2;
            Session["MensCatProduto"] = 0;
            objetoCatProd = new CATEGORIA_PRODUTO();
            return View(objetoCatProd);
        }

        public ActionResult RetirarFiltroCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatProduto"] = null;
            Session["FiltroCatProduto"] = null;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult MostrarTudoCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatProd = cpApp.GetAllItensAdm(idAss).Where(p => p.CAPR_IN_TIPO == 1 & p.CAPR_IN_SISTEMA == 6).ToList();
            Session["FiltroCatProduto"] = null;
            Session["ListaCatProduto"] = listaMasterCatProd;
            return RedirectToAction("MontarTelaCatProduto");
        }

        public ActionResult VoltarBaseCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatProduto");
        }

        [HttpGet]
        public ActionResult IncluirCatProduto()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAPR_IN_ATIVO = 1;
            vm.CAPR_IN_TIPO = 0;
            vm.CAPR_IN_SISTEMA = 6;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatProduto(CategoriaProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica
                    if (vm.CAPR_IN_TIPO == 0 || vm.CAPR_IN_TIPO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0689", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0409", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    Session["IdVolta"] = item.CAPR_CD_ID;

                    // Sucesso
                    listaMasterCatProd = new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    Session["CatProdutoAlterada"] = 1;
                    if ((Int32)Session["VoltaCatProduto"] == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    return RedirectToAction("MontarTelaCatProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

            // Prepara view
            CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
            Session["CatProduto"] = item;
            Session["IdCatProduto"] = id;
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatProduto(CategoriaProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica
                    if (vm.CAPR_IN_TIPO == 0 || vm.CAPR_IN_TIPO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0689", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    Int32 volta = cpApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatProd = new List<CATEGORIA_PRODUTO>();
                    Session["ListaCatProduto"] = null;
                    Session["CatProdutoAlterada"] = 1;
                    return RedirectToAction("MontarTelaCatProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
                objetoAntesCatProd = item;
                item.CAPR_IN_ATIVO = 0;
                Int32 volta = cpApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensCatProduto"] = 4;
                    return RedirectToAction("MontarTelaCatProduto");
                }
                listaMasterCatProd = new List<CATEGORIA_PRODUTO>();
                Session["ListaCatProduto"] = null;
                return RedirectToAction("MontarTelaCatProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                CATEGORIA_PRODUTO item = cpApp.GetItemById(id);
                objetoAntesCatProd = item;
                item.CAPR_IN_ATIVO = 1;
                Int32 volta = cpApp.ValidateReativar(item, usuario);
                listaMasterCatProd = new List<CATEGORIA_PRODUTO>();
                Session["ListaCatProduto"] = null;
                return RedirectToAction("MontarTelaCatProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaSubCatProduto()
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
                if (usuario.PERFIL.PERF_IN_ACESSO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaSubCatProduto"] == null)
            {
                listaMasterSubProd = CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1 & p.SCPR_IN_SISTEMA == 6).ToList();
                Session["ListaSubCatProduto"] = listaMasterSubProd;
            }
            ViewBag.Listas = (List<SUBCATEGORIA_PRODUTO>)Session["ListaSubCatProduto"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensSubCatProduto"] != null)
            {
                if ((Int32)Session["MensSubCatProduto"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0409", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubCatProduto"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0410", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaSubCatProduto"] = 1;
            Session["MensSubCatProduto"] = 0;
            objetoSubProd = new SUBCATEGORIA_PRODUTO();
            return View(objetoSubProd);
        }

        public ActionResult RetirarFiltroSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaSubCatProduto"] = null;
            Session["FiltroSubCatProduto"] = null;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult MostrarTudoSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSubProd = spApp.GetAllItensAdm(idAss).Where(p => p.SCPR_IN_TIPO == 1 & p.SCPR_IN_SISTEMA == 6).ToList();
            Session["FiltroSubCatProduto"] = null;
            Session["ListaSubCatProduto"] = listaMasterSubProd;
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        public ActionResult VoltarBaseSubCatProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaSubCatProduto"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaSubCatProduto"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaSubCatProduto");
        }

        [HttpGet]
        public ActionResult IncluirSubCatProduto()
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
                if (usuario.PERFIL.PERF_IN_INCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBCATEGORIA_PRODUTO item = new SUBCATEGORIA_PRODUTO();
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SCPR_IN_ATIVO = 1;
            vm.SCPR_IN_TIPO = 1;
            vm.SCPR_IN_SISTEMA = 6;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirSubCatProduto(SubCategoriaProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = spApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubCatProduto"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0409", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    Session["IdVolta"] = item.CAPR_CD_ID;

                    // Sucesso
                    listaMasterSubProd = new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    Session["SubCatProdutoAlterada"] = 1;
                    if ((Int32)Session["VoltaSubCatProduto"] == 2)
                    {
                        return RedirectToAction("IncluirProduto", "Produto");
                    }
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarSubCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_EDICAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");

            // Prepara view
            SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
            Session["SubCatProduto"] = item;
            Session["IdSubCatProduto"] = id;
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubCatProduto(SubCategoriaProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    SUBCATEGORIA_PRODUTO item = Mapper.Map<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>(vm);
                    Int32 volta = spApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSubProd = new List<SUBCATEGORIA_PRODUTO>();
                    Session["ListaSubCatProduto"] = null;
                    Session["SubCatProdutoAlterada"] = 1;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirSubCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_EXCLUSAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
                objetoAntesSubProd = item;
                item.SCPR_IN_ATIVO = 0;
                Int32 volta = spApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensSubCatProduto"] = 4;
                    return RedirectToAction("MontarTelaSubCatProduto");
                }
                listaMasterSubProd = new List<SUBCATEGORIA_PRODUTO>();
                Session["ListaSubCatProduto"] = null;
                return RedirectToAction("MontarTelaSubCatProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarSubCatProduto(Int32 id)
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
                if (usuario.PERFIL.PERF_IN_REATIVACAO_AUX == 0)
                {
                    Session["MensPermissao"] = 2;
                    Session["ModuloPermissao"] = "Tabelas";
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            try
            {
                SUBCATEGORIA_PRODUTO item = spApp.GetItemById(id);
                objetoAntesSubProd = item;
                item.SCPR_IN_ATIVO = 1;
                Int32 volta = spApp.ValidateReativar(item, usuario);
                listaMasterSubProd = new List<SUBCATEGORIA_PRODUTO>();
                Session["ListaSubCatProduto"] = null;
                return RedirectToAction("MontarTelaSubCatProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<CATEGORIA_PRODUTO> CarregarCatProduto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CATEGORIA_PRODUTO> conf = new List<CATEGORIA_PRODUTO>();
            if (Session["CatProdutos"] == null)
            {
                conf = cpApp.GetAllItensAdm(idAss);
            }
            else
            {
                if ((Int32)Session["CatProdutoAlterada"] == 1)
                {
                    conf = cpApp.GetAllItensAdm(idAss);
                }
                else
                {
                    conf = (List<CATEGORIA_PRODUTO>)Session["CatProdutos"];
                }
            }
            conf = conf.Where(p => p.CAPR_IN_SISTEMA == 6).ToList();
            Session["CatProdutoAlterada"] = 0;
            Session["CatProdutos"] = conf;
            return conf;
        }

        public List<SUBCATEGORIA_PRODUTO> CarregarSubCatProduto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SUBCATEGORIA_PRODUTO> conf = new List<SUBCATEGORIA_PRODUTO>();
            if (Session["SubCatProdutos"] == null)
            {
                conf = spApp.GetAllItensAdm(idAss);
            }
            else
            {
                if ((Int32)Session["SubCatProdutoAlterada"] == 1)
                {
                    conf = spApp.GetAllItensAdm(idAss);
                }
                else
                {
                    conf = (List<SUBCATEGORIA_PRODUTO>)Session["SubCatProdutos"];
                }
            }
            conf = conf.Where(p => p.SCPR_IN_SISTEMA == 6).ToList();
            Session["SubCatProdutoAlterada"] = 0;
            Session["SubCatProdutos"] = conf;
            return conf;
        }

        public List<UNIDADE> CarregarUnidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<UNIDADE> conf = new List<UNIDADE>();
            if (Session["Unidades"] == null)
            {
                conf = uniApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["UnidadeAlterada"] == 1)
                {
                    conf = uniApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<UNIDADE>)Session["Unidades"];
                }
            }
            conf = conf.Where(p => p.UNID_IN_TIPO_UNIDADE == 6).ToList();
            Session["UnidadeAlterada"] = 0;
            Session["Unidades"] = conf;
            return conf;
        }

    }
}