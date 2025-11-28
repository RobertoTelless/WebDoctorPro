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


namespace ERP_Condominios_Solution.Controllers
{
    public class TabelaAuxiliarController : Controller
    {
        private readonly ITipoPacienteAppService tpApp;
        private readonly ILogAppService logApp;
        private readonly IConvenioAppService conApp;
        private readonly ITipoExameAppService teApp;
        private readonly ITipoAtestadoAppService taApp;
        private readonly IUsuarioAppService usuApp;

        TIPO_PACIENTE objetoTP = new TIPO_PACIENTE();
        TIPO_PACIENTE objetoAntesTP= new TIPO_PACIENTE();
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

        String extensao;

        public TabelaAuxiliarController(ILogAppService logApps, IUsuarioAppService usuApps, ITipoPacienteAppService tpApps, IConvenioAppService conApps, ITipoExameAppService teApps, ITipoAtestadoAppService taApps)
        {
            logApp = logApps;
            taApp = taApps;
            usuApp = usuApps;
            tpApp = tpApps;
            teApp = teApps;
            conApp = conApps;
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
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTP = tpApp.GetAllItensAdm(idAss);
            Session["ListaTipoPaciente"] = listaMasterTP;
            return RedirectToAction("MontarTelaTipoPaciente");
        }

        public ActionResult VoltarBaseCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCargo"] == 2)
            {
                return RedirectToAction("IncluirUsuario", "Usuario");
            }
            if ((Int32)Session["VoltaCargo"] == 3)
            {
                return RedirectToAction("VoltarAnexoUsuario", "Usuario");
            }
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult IncluirCargo()
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
            CARGO item = new CARGO();
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CARG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCargo(CargoViewModel vm)
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
                    vm.CARG_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CARG_NM_NOME);

                    // Executa a operação
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = carApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCargo"] = 3;
                        return RedirectToAction("MontarTelaCargo");
                    }
                    Session["IdVolta"] = item.CARG_CD_ID;

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    Session["ListaCargo"] = null;
                    Session["CargoAlterada"] = 1;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerCargo(Int32 id)
        {
            
            // Prepara view
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCargo(Int32 id)
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
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCargo(CargoViewModel vm)
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
                    vm.CARG_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CARG_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    Int32 volta = carApp.ValidateEdit(item, objetoAntesCargo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    Session["ListaCargo"] = null;
                    Session["CargoAlterada"] = 1;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCargo(Int32 id)
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
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            item.CARG_IN_ATIVO = 0;
            Int32 volta = carApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCargo"] = 4;
                return RedirectToAction("MontarTelaCargo");
            }
            listaMasterCargo = new List<CARGO>();
            Session["ListaCargo"] = null;
            Session["CargoAlterada"] = 1;
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult ReativarCargo(Int32 id)
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
            CARGO item = carApp.GetItemById(id);
            item.CARG_IN_ATIVO = 1;
            objetoAntesCargo = item;
            Int32 volta = carApp.ValidateReativar(item, usuario);
            listaMasterCargo = new List<CARGO>();
            Session["ListaCargo"] = null;
            Session["CargoAlterada"] = 1;
            return RedirectToAction("MontarTelaCargo");
        }
    
        [HttpGet]
        public ActionResult MontarTelaCatCliente()
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
            if (Session["ListaCatCliente"] == null)
            {
                listaMasterCatCliente= ccApp.GetAllItens(idAss);
                Session["ListaCatCliente"] = listaMasterCatCliente;
            }
            ViewBag.Listas = (List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"];
            ViewBag.Title = "CatCliente";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"]).Count;

            if (Session["MensCatCliente"] != null)
            {
                if ((Int32)Session["MensCatCliente"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0176", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatCliente"] = 1;
            Session["MensCatCliente"] = 0;
            objetoCatCliente = new CATEGORIA_CLIENTE();
            return View(objetoCatCliente);
        }

        public ActionResult RetirarFiltroCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatCliente"] = null;
            Session["FiltroCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult MostrarTudoCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatCliente= ccApp.GetAllItensAdm(idAss);
            Session["FiltroCatCliente"] = null;
            Session["ListaCatCliente"] = listaMasterCatCliente;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult VoltarBaseCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 x = (Int32)Session["VoltaCatCliente"];
            if ((Int32)Session["VoltaCatCliente"] == 2)
            {
                return RedirectToAction("IncluirCliente", "Cliente");
            }
            if ((Int32)Session["VoltaCatCliente"] == 3)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult IncluirCatCliente()
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
            CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CACL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatCliente(CategoriaClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                // Sanitizar
                vm.CACL_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CACL_NM_NOME);

                try
                {
                    // Executa a operação
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatCliente"] = 3;
                        return RedirectToAction("MontarTelaCatCliente");
                    }
                    Session["IdVolta"] = item.CACL_CD_ID;

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    Session["CatClienteAlterada"] = 1;
                    return RedirectToAction("VoltarBaseCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerCatCliente(Int32 id)
        {
            
            // Prepara view
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCatCliente(Int32 id)
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
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            objetoAntesCatCliente= item;
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatCliente(CategoriaClienteViewModel vm)
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
                    vm.CACL_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CACL_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objetoAntesCatCliente, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    Session["CatClienteAlterada"] = 1;
                    return RedirectToAction("MontarTelaCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatCliente(Int32 id)
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
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 0;
            Int32 volta = ccApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatCliente"] = 4;
                return RedirectToAction("MontarTelaCatCliente");
            }
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            Session["CatClienteAlterada"] = 1;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult ReativarCatCliente(Int32 id)
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
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 1;
            Int32 volta = ccApp.ValidateReativar(item, usuario);
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            Session["CatClienteAlterada"] = 1;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoAcao()
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
            if (Session["ListaTipoAcao"] == null)
            {
                listaMasterTipoAcao = taApp.GetAllItens(idAss);
                Session["ListaTipoAcao"] = listaMasterTipoAcao;
            }
            ViewBag.Listas = (List<TIPO_ACAO>)Session["ListaTipoAcao"];
            ViewBag.Title = "TipoAcao";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoAcao = ((List<TIPO_ACAO>)Session["ListaTipoAcao"]).Count;

            if (Session["MensTipoAcao"] != null)
            {
                if ((Int32)Session["MensTipoAcao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0183", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0184", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoAcao"] = 1;
            Session["MensTipoAcao"] = 0;
            objetoTipoAcao = new TIPO_ACAO();
            return View(objetoTipoAcao);
        }

        public ActionResult RetirarFiltroTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoAcao"] = null;
            Session["FiltroTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult MostrarTudoTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTipoAcao= taApp.GetAllItensAdm(idAss);
            Session["FiltroTipoAcao"] = null;
            Session["ListaTipoAcao"] = listaMasterTipoAcao;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult VoltarBaseTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaTipoAcao"] == 2)
            {
                return RedirectToAction("IncluirAcao", "CRM");
            }
            if ((Int32)Session["VoltaTipoAcao"] == 3)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult IncluirTipoAcao()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            TIPO_ACAO item = new TIPO_ACAO();
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIAC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoAcao(TipoAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.TIAC_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIAC_NM_NOME);

                    // Executa a operação
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = taApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoAcao"] = 3;
                        return RedirectToAction("MontarTelaTipoAcao");
                    }
                    Session["IdVolta"] = item.TIAC_CD_ID;

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    Session["TipoAcaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoAcao(Int32 id)
        {
            
            // Prepara view
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoAcao(Int32 id)
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
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoAcao(TipoAcaoViewModel vm)
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
                    vm.TIAC_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIAC_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoAntesTipoAcao, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    Session["TipoAcaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoAcao(Int32 id)
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
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            item.TIAC_IN_ATIVO = 0;
            Int32 volta = taApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoAcao"] = 4;
                return RedirectToAction("MontarTelaTipoAcao");
            }
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            Session["TipoAcaoAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult ReativarTipoAcao(Int32 id)
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
            TIPO_ACAO item = taApp.GetItemById(id);
            item.TIAC_IN_ATIVO = 1;
            objetoAntesTipoAcao = item;
            Int32 volta = taApp.ValidateReativar(item, usuario);
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            Session["TipoAcaoAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult MontarTelaMotCancelamento()
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
            if (Session["ListaMotCancelamento"] == null)
            {
                listaMasterMotCancelamento = mcApp.GetAllItens(idAss);
                Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            }
            ViewBag.Listas = (List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"];
            ViewBag.Title = "MotCancelamento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"]).Count;

            if (Session["MensMotCancelamento"] != null)
            {
                if ((Int32)Session["MensMotCancelamento"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0185", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0186", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotCancelamento"] = 1;
            Session["MensMotCancelamento"] = 0;
            objetoMotCancelamento = new MOTIVO_CANCELAMENTO();
            return View(objetoMotCancelamento);
        }

        public ActionResult RetirarFiltroMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult MostrarTudoMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotCancelamento = mcApp.GetAllItensAdm(idAss);
            Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult VoltarBaseMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotCancelamento"] == 2)
            {
                return RedirectToAction("VoltarCancelarPedido", "CRM");
            }
            if ((Int32)Session["VoltaMotCancelamento"] == 3)
            {
                return RedirectToAction("VoltarCancelarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult IncluirMotCancelamento()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_CANCELAMENTO item = new MOTIVO_CANCELAMENTO();
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOCA_IN_ATIVO = 1;
            vm.MOCA_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotCancelamento(MotivoCancelamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.MOCA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MOCA_NM_NOME);

                    // Executa a operação
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = mcApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotCancelamento"] = 3;
                        return RedirectToAction("MontarTelaMotCancelamento");
                    }
                    Session["IdVolta"] = item.MOCA_CD_ID;

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    Session["MotCancelamentoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerMotCancelamento(Int32 id)
        {
            
            // Prepara view
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarMotCancelamento(Int32 id)
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
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotCancelamento(MotivoCancelamentoViewModel vm)
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
                    vm.MOCA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MOCA_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    Int32 volta = mcApp.ValidateEdit(item, objetoAntesMotCancelamento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    Session["MotCancelamentoAlterada"] = 1;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirMotCancelamento(Int32 id)
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
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            item.MOCA_IN_ATIVO = 0;
            Int32 volta = mcApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotCancelamento"] = 4;
                return RedirectToAction("MontarTelaMotCancelamento");
            }
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            Session["MotCancelamentoAlterada"] = 1;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult ReativarMotCancelamento(Int32 id)
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
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            item.MOCA_IN_ATIVO = 1;
            objetoAntesMotCancelamento = item;
            Int32 volta = mcApp.ValidateReativar(item, usuario);
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            Session["MotCancelamentoAlterada"] = 1;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult MontarTelaMotEncerramento()
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
            if (Session["ListaMotEncerramento"] == null)
            {
                listaMasterMotEncerramento = meApp.GetAllItens(idAss);
                Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            }
            ViewBag.Listas = (List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"];
            ViewBag.Title = "MotEncerramento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"]).Count;

            if (Session["MensMotEncerramento"] != null)
            {
                if ((Int32)Session["MensMotEncerramento"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0185", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0186", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotEncerramento"] = 1;
            Session["MensMotEncerramento"] = 0;
            objetoMotEncerramento = new MOTIVO_ENCERRAMENTO();
            return View(objetoMotEncerramento);
        }

        public ActionResult RetirarFiltroMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult MostrarTudoMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotEncerramento = meApp.GetAllItensAdm(idAss);
            Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult VoltarBaseMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotEncerramento"] == 2)
            {
                return RedirectToAction("VoltarEncerrarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult IncluirMotEncerramento()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_ENCERRAMENTO item = new MOTIVO_ENCERRAMENTO();
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOEN_IN_ATIVO = 1;
            vm.MOEN_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotEncerramento(MotivoEncerramentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.MOEN_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MOEN_NM_NOME);

                    // Executa a operação
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = meApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotEncerramento"] = 3;
                        return RedirectToAction("MontarTelaMotEncerramento");
                    }
                    Session["IdVolta"] = item.MOEN_CD_ID;

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    Session["MotEncerramentoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMotEncerramento(Int32 id)
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
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            Session["MotEncerramento"] = item;
            Session["IdMotEncerramento"] = id;
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotEncerramento(MotivoEncerramentoViewModel vm)
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
                    vm.MOEN_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MOEN_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    Int32 volta = meApp.ValidateEdit(item, objetoAntesMotEncerramento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    Session["MotEncerramentoAlterada"] = 1;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirMotEncerramento(Int32 id)
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
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            item.MOEN_IN_ATIVO = 0;
            Int32 volta = meApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotEncerramento"] = 4;
                return RedirectToAction("MontarTelaMotEncerramento");
            }
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            Session["MotEncerramentoAlterada"] = 1;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult ReativarMotEncerramento(Int32 id)
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
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            item.MOEN_CD_ID = 1;
            objetoAntesMotEncerramento = item;
            Int32 volta = meApp.ValidateReativar(item, usuario);
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            Session["MotEncerramentoAlterada"] = 1;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoTarefa()
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
            if (Session["ListaTipoTarefa"] == null)
            {
                listaMasterTarefa = ttApp.GetAllItens(idAss);
                Session["ListaTipoTarefa"] = listaMasterTarefa;
            }
            ViewBag.Listas = (List<TIPO_TAREFA>)Session["ListaTipoTarefa"];
            ViewBag.Title = "TipoTarefa";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoTarefa = ((List<TIPO_TAREFA>)Session["ListaTipoTarefa"]).Count;

            if (Session["MensTipoTarefa"] != null)
            {
                if ((Int32)Session["MensTipoTarefa"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0212", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTarefa"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTarefa"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0213", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoTarefa"] = 1;
            Session["MensTipoTarefa"] = 0;
            objetoTarefa= new TIPO_TAREFA();
            return View(objetoTarefa);
        }

        public ActionResult RetirarFiltroTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoTarefa"] = null;
            Session["FiltroTipoTarefa"] = null;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        public ActionResult MostrarTudoTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTarefa = ttApp.GetAllItensAdm(idAss);
            Session["FiltroTipoTarefa"] = null;
            Session["ListaTipoTarefa"] = listaMasterTarefa;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        public ActionResult VoltarBaseTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult IncluirTipoTarefa()
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
            TIPO_TAREFA item = new TIPO_TAREFA();
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TITR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoTarefa(TipoTarefaViewModel vm)
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
                    vm.TITR_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TITR_NM_NOME);

                    // Executa a operação
                    TIPO_TAREFA item = Mapper.Map<TipoTarefaViewModel, TIPO_TAREFA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ttApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoTarefa"] = 3;
                        return RedirectToAction("MontarTelaTipoTarefa");
                    }
                    Session["IdVolta"] = item.TITR_CD_ID;

                    // Sucesso
                    listaMasterTarefa = new List<TIPO_TAREFA>();
                    Session["ListaTipoTarefa"] = null;
                    Session["TipoTarefaAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoTarefa(Int32 id)
        {
            
            // Prepara view
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            Session["TipoTarefa"] = item;
            Session["IdTipoTarefa"] = id;
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoTarefa(Int32 id)
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
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            Session["TipoTarefa"] = item;
            Session["IdTipoTarefa"] = id;
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoTarefa(TipoTarefaViewModel vm)
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
                    vm.TITR_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TITR_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_TAREFA item = Mapper.Map<TipoTarefaViewModel, TIPO_TAREFA>(vm);
                    Int32 volta = ttApp.ValidateEdit(item, objetoAntesTarefa, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTarefa = new List<TIPO_TAREFA>();
                    Session["ListaTipoTarefa"] = null;
                    Session["TipoTarefaAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoTarefa(Int32 id)
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
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            item.TITR_IN_ATIVO = 0;
            Int32 volta = ttApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoTarefa"] = 4;
                return RedirectToAction("MontarTelaTipoTarefa");
            }
            listaMasterTarefa = new List<TIPO_TAREFA>();
            Session["ListaTipoTarefa"] = null;
            Session["TipoTarefaAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult ReativarTipoTarefa(Int32 id)
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
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            item.TITR_IN_ATIVO = 1;
            Int32 volta = ttApp.ValidateReativar(item, usuario);
            listaMasterTarefa = new List<TIPO_TAREFA>();
            Session["ListaTipoTarefa"] = null;
            Session["TipoTarefaAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult MontarTelaCatAgenda()
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
            if (Session["ListaCatAgenda"] == null)
            {
                listaMasterCatAgenda= caApp.GetAllItens(idAss);
                Session["ListaCatAgenda"] = listaMasterCatAgenda;
            }
            ViewBag.Listas = (List<CATEGORIA_AGENDA>)Session["ListaCatAgenda"];
            ViewBag.Title = "CatAgenda";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_AGENDA>)Session["ListaCatAgenda"]).Count;

            if (Session["MensCatAgenda"] != null)
            {
                if ((Int32)Session["MensCatAgenda"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0255", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAgenda"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAgenda"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatAgenda"] = 1;
            Session["MensCatAgenda"] = 0;
            objetoCatAgenda= new CATEGORIA_AGENDA();
            return View(objetoCatAgenda);
        }

        public ActionResult RetirarFiltroCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatAgenda"] = null;
            Session["FiltroCatAgenda"] = null;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        public ActionResult MostrarTudoCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatAgenda = caApp.GetAllItensAdm(idAss);
            Session["FiltroCatAgenda"] = null;
            Session["ListaCatAgenda"] = listaMasterCatAgenda;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        public ActionResult VoltarBaseCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult IncluirCatAgenda()
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
            CATEGORIA_AGENDA item = new CATEGORIA_AGENDA();
            CategoriaAgendaViewModel vm = Mapper.Map<CATEGORIA_AGENDA, CategoriaAgendaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAAG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatAgenda(CategoriaAgendaViewModel vm)
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
                    vm.CAAG_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CAAG_NM_NOME);

                    // Executa a operação
                    CATEGORIA_AGENDA item = Mapper.Map<CategoriaAgendaViewModel, CATEGORIA_AGENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = caApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatAgenda"] = 3;
                        return RedirectToAction("MontarTelaCatAgenda");
                    }
                    Session["IdVolta"] = item.CAAG_CD_ID;

                    // Sucesso
                    listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
                    Session["ListaCatAgenda"] = null;
                    Session["CatAgendaAlterada"] = 1;
                    return RedirectToAction("VoltarBaseCatAgenda");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatAgenda(Int32 id)
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
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            objetoAntesCatAgenda = item;
            Session["CatAgenda"] = item;
            Session["IdCatAgenda"] = id;
            CategoriaAgendaViewModel vm = Mapper.Map<CATEGORIA_AGENDA, CategoriaAgendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatAgenda(CategoriaAgendaViewModel vm)
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
                    vm.CAAG_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CAAG_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_AGENDA item = Mapper.Map<CategoriaAgendaViewModel, CATEGORIA_AGENDA>(vm);
                    Int32 volta = caApp.ValidateEdit(item, objetoAntesCatAgenda, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
                    Session["ListaCatAgenda"] = null;
                    Session["CatAgendaAlterada"] = 1;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatAgenda(Int32 id)
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
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            item.CAAG_IN_ATIVO = 0;
            Int32 volta = caApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatAgenda"] = 4;
                return RedirectToAction("MontarTelaCatAgenda");
            }
            listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
            Session["ListaCatAgenda"] = null;
            Session["CatAgendaAlterada"] = 1;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult ReativarCatAgenda(Int32 id)
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
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            item.CAAG_IN_ATIVO = 1;
            Int32 volta = caApp.ValidateReativar(item, usuario);
            listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
            Session["ListaCatAgenda"] = null;
            Session["CatAgendaAlterada"] = 1;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoFollow()
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
            if (Session["ListaTipoFollow"] == null)
            {
                listaMasterFollow= foApp.GetAllItens(idAss);
                Session["ListaTipoFollow"] = listaMasterFollow;
            }
            ViewBag.Listas = (List<TIPO_FOLLOW>)Session["ListaTipoFollow"];
            ViewBag.Title = "TipoTarefa";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoFollow = ((List<TIPO_FOLLOW>)Session["ListaTipoFollow"]).Count;

            if (Session["MensTipoFollow"] != null)
            {
                if ((Int32)Session["MensTipoFollow"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0212", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoFollow"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoFollow"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0213", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoFollow"] = 1;
            Session["MensTipoFollow"] = 0;
            objetoFollow = new TIPO_FOLLOW();
            return View(objetoFollow);
        }

        public ActionResult RetirarFiltroTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoFollow"] = null;
            Session["FiltroTipoFollow"] = null;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        public ActionResult MostrarTudoTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFollow= foApp.GetAllItensAdm(idAss);
            Session["FiltroTipoFollow"] = null;
            Session["ListaTipoFollow"] = listaMasterFollow;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        public ActionResult VoltarBaseTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoFollow");
        }

        [HttpGet]
        public ActionResult IncluirTipoFollow()
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
            TIPO_FOLLOW item = new TIPO_FOLLOW();
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIFL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoFollow(TipoFollowViewModel vm)
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
                    vm.TIFL_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIFL_NM_NOME);

                    // Executa a operação
                    TIPO_FOLLOW item = Mapper.Map<TipoFollowViewModel, TIPO_FOLLOW>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = foApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoFollow"] = 3;
                        return RedirectToAction("MontarTelaTipoFollow");
                    }
                    Session["IdVolta"] = item.TIFL_CD_ID;

                    // Sucesso
                    listaMasterFollow = new List<TIPO_FOLLOW>();
                    Session["ListaTipoFollow"] = null;
                    Session["TipoFollowAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoFollow(Int32 id)
        {
            
            // Prepara view
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            Session["TipoFollow"] = item;
            Session["IdTipoFollow"] = id;
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoFollow(Int32 id)
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
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            Session["TipoFollow"] = item;
            Session["IdTipoFollow"] = id;
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoFollow(TipoFollowViewModel vm)
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
                    vm.TIFL_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIFL_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_FOLLOW item = Mapper.Map<TipoFollowViewModel, TIPO_FOLLOW>(vm);
                    Int32 volta = foApp.ValidateEdit(item, objetoAntesFollow, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFollow = new List<TIPO_FOLLOW>();
                    Session["ListaTipoFollow"] = null;
                    Session["TipoFollowAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoFollow(Int32 id)
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
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow= item;
            item.TIFL_IN_ATIVO = 0;
            Int32 volta = foApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoFollow"] = 4;
                return RedirectToAction("MontarTelaTipoFollow");
            }
            listaMasterFollow = new List<TIPO_FOLLOW>();
            Session["ListaTipoFollow"] = null;
            Session["TipoFollowAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        [HttpGet]
        public ActionResult ReativarTipoFollow(Int32 id)
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
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            item.TIFL_IN_ATIVO = 1;
            Int32 volta = foApp.ValidateReativar(item, usuario);
            listaMasterFollow = new List<TIPO_FOLLOW>();
            Session["ListaTipoFollow"] = null;
            Session["TipoFollowAlterada"] = 1;
            return RedirectToAction("MontarTelaTipoFollow");
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                listaMasterCatProd= CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).ToList();
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
            listaMasterCatProd = cpApp.GetAllItensAdm(idAss).Where(p => p.CAPR_IN_TIPO == 1).ToList();
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
            if ((Int32)Session["VoltarTabs"] == 2)
            {
                return RedirectToAction("MontarTelaRoteiro_0", "Precificacao");
            }
            if ((Int32)Session["VoltaCatProduto"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaCatProduto"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
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
            CATEGORIA_PRODUTO item = new CATEGORIA_PRODUTO();
            CategoriaProdutoViewModel vm = Mapper.Map<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAPR_IN_ATIVO = 1;
            vm.CAPR_IN_TIPO = 1;
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
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_PRODUTO item = Mapper.Map<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatProduto"] = 3;
                        return RedirectToAction("MontarTelaCatProduto");
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
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
            if (ModelState.IsValid)
            {
                try
            {
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
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                listaMasterSubProd = CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).ToList();
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
            listaMasterSubProd = spApp.GetAllItensAdm(idAss).Where(p => p.SCPR_IN_TIPO == 1).ToList();
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
            if ((Int32)Session["VoltarTabs"] == 2)
            {
                return RedirectToAction("MontarTelaRoteiro_0", "Precificacao");
            }
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");
            //ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            SUBCATEGORIA_PRODUTO item = new SUBCATEGORIA_PRODUTO();
            SubCategoriaProdutoViewModel vm = Mapper.Map<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SCPR_IN_ATIVO = 1;
            vm.SCPR_IN_TIPO = 1;
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
            //ViewBag.Unidades = new SelectList(CarregarUnidade().Where(p => p.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME), "UNID_CD_ID", "UNID_NM_NOME");
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
                        return RedirectToAction("MontarTelaSubCatProduto");
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
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Cats = new SelectList(CarregarCatProduto().OrderBy(p => p.CAPR_NM_NOME), "CAPR_CD_ID", "CAPR_NM_NOME");

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
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
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
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaUnidade()
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
            listaMasterUnid = CarregarUnidade();
            Session["ListaUnidade"] = listaMasterUnid;
            ViewBag.Listas = (List<UNIDADE>)Session["ListaUnidade"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;


            if (Session["MensUnidade"] != null)
            {
                if ((Int32)Session["MensUnidade"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUnidade"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensUnidade"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0411", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaUnidade"] = 1;
            Session["MensUnidade"] = 0;
            objetoUnid = new UNIDADE();
            return View(objetoUnid);
        }

        public ActionResult RetirarFiltroUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaUnidade"] = null;
            Session["FiltroUnidade"] = null;
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult MostrarTudoUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterUnid = uniApp.GetAllItensAdm(idAss);
            Session["FiltroUnidade"] = null;
            Session["ListaUnidade"] = listaMasterUnid;
            return RedirectToAction("MontarTelaUnidade");
        }

        public ActionResult VoltarBaseUnidade()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltarTabs"] == 2)
            {
                return RedirectToAction("MontarTelaRoteiro_0", "Precificacao");
            }
            if ((Int32)Session["VoltaUnidade"] == 2)
            {
                return RedirectToAction("IncluirProduto", "Produto");
            }
            if ((Int32)Session["VoltaUnidade"] == 3)
            {
                return RedirectToAction("VoltarAnexoProduto", "Produto");
            }
            return RedirectToAction("MontarTelaUnidade");
        }

        [HttpGet]
        public ActionResult IncluirUnidade()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> frac = new List<SelectListItem>();
            frac.Add(new SelectListItem() { Text = "Não", Value = "0" });
            frac.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Fracionado = new SelectList(frac, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            UNIDADE item = new UNIDADE();
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.UNID_IN_ATIVO = 1;
            vm.UNID_IN_TIPO_UNIDADE = 1;
            vm.UNID_IN_FRACIONADA = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirUnidade(UnidadeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> frac = new List<SelectListItem>();
            frac.Add(new SelectListItem() { Text = "Não", Value = "0" });
            frac.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Fracionado = new SelectList(frac, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = uniApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUnidade"] = 3;
                        return RedirectToAction("MontarTelaUnidade");
                    }
                    Session["IdVolta"] = item.UNID_CD_ID;

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    Session["ListaUnidade"] = null;
                    Session["UnidadeAlterada"] = 1;
                    return RedirectToAction("MontarTelaUnidade");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarUnidade(Int32 id)
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> frac = new List<SelectListItem>();
            frac.Add(new SelectListItem() { Text = "Não", Value = "0" });
            frac.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Fracionado = new SelectList(frac, "Value", "Text");

            UNIDADE item = uniApp.GetItemById(id);
            objetoAntesUnid = item;
            Session["Unidade"] = item;
            Session["IdUnidade"] = id;
            UnidadeViewModel vm = Mapper.Map<UNIDADE, UnidadeViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarUnidade(UnidadeViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Serviço", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> frac = new List<SelectListItem>();
            frac.Add(new SelectListItem() { Text = "Não", Value = "0" });
            frac.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Fracionado = new SelectList(frac, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    UNIDADE item = Mapper.Map<UnidadeViewModel, UNIDADE>(vm);
                    Int32 volta = uniApp.ValidateEdit(item, objetoAntesUnid, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterUnid = new List<UNIDADE>();
                    Session["ListaUnidade"] = null;
                    Session["UnidadeAlterada"] = 1;
                    return RedirectToAction("MontarTelaUnidade");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirUnidade(Int32 id)
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
            try
            {
                UNIDADE item = uniApp.GetItemById(id);
                objetoAntesUnid = item;
                item.UNID_IN_ATIVO = 0;
                Int32 volta = uniApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensUnidade"] = 4;
                    return RedirectToAction("MontarTelaUnidade");
                }
                listaMasterUnid = new List<UNIDADE>();
                Session["ListaUnidade"] = null;
                return RedirectToAction("MontarTelaUnidade");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarUnidade(Int32 id)
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
            try
            {
                UNIDADE item = uniApp.GetItemById(id);
                objetoAntesUnid = item;
                item.UNID_IN_ATIVO = 1;
                Int32 volta = uniApp.ValidateReativar(item, usuario);
                listaMasterUnid = new List<UNIDADE>();
                Session["ListaUnidade"] = null;
                return RedirectToAction("MontarTelaUnidade");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<CATEGORIA_PRODUTO> CarregarCatProduto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CATEGORIA_PRODUTO> conf = new List<CATEGORIA_PRODUTO>();
            if (Session["CatProdutos"] == null)
            {
                conf = cpApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["CatProdutoAlterada"] == 1)
                {
                    conf = cpApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<CATEGORIA_PRODUTO>)Session["CatProdutos"];
                }
            }
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
                conf = spApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["SubCatProdutoAlterada"] == 1)
                {
                    conf = spApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<SUBCATEGORIA_PRODUTO>)Session["SubCatProdutos"];
                }
            }
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
            Session["UnidadeAlterada"] = 0;
            Session["Unidades"] = conf;
            return conf;
        }

        [HttpGet]
        public ActionResult MontarTelaCatServico()
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
            if (Session["ListaCatServico"] == null)
            {
                listaMasterCatServico = csApp.GetAllItens(idAss);
                Session["ListaCatServico"] = listaMasterCatServico;
            }
            ViewBag.Listas = (List<CATEGORIA_SERVICO>)Session["ListaCatServico"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_SERVICO>)Session["ListaCatServico"]).Count;

            if (Session["MensCatServico"] != null)
            {
                if ((Int32)Session["MensCatServico"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0176", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatServico"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatServico"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatServico"] = 1;
            Session["MensCatServico"] = 0;
            objetoCatServico = new CATEGORIA_SERVICO();
            return View(objetoCatServico);
        }

        public ActionResult RetirarFiltroCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatServico"] = null;
            Session["FiltroCatServico"] = null;
            return RedirectToAction("MontarTelaCatServico");
        }

        public ActionResult MostrarTudoCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatServico = csApp.GetAllItensAdm(idAss);
            Session["FiltroCatServico"] = null;
            Session["ListaCatServico"] = listaMasterCatServico;
            return RedirectToAction("MontarTelaCatServico");
        }

        public ActionResult VoltarBaseCatServico()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatServico"] == 2)
            {
                return RedirectToAction("IncluirServico", "Servico");
            }
            if ((Int32)Session["VoltaCatServico"] == 3)
            {
                return RedirectToAction("VoltarAnexoServico", "Servico");
            }
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult IncluirCatServico()
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
            CATEGORIA_SERVICO item = new CATEGORIA_SERVICO();
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CASE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatServico(CategoriaServicoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                // Sanitizar
                vm.CASE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CASE_NM_NOME);

                try
                {
                    // Executa a operação
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = csApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatServico"] = 3;
                        return RedirectToAction("MontarTelaCatServico");
                    }
                    Session["IdVolta"] = item.CASE_CD_ID;

                    // Sucesso
                    listaMasterCatServico = new List<CATEGORIA_SERVICO>();
                    Session["ListaCatServico"] = null;
                    Session["CatServicoAlterada"] = 1;
                    return RedirectToAction("VoltarBaseCatServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatServico(Int32 id)
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
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            objetoAntesCatServico = item;
            Session["CatServico"] = item;
            Session["IdCatServico"] = id;
            CategoriaServicoViewModel vm = Mapper.Map<CATEGORIA_SERVICO, CategoriaServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatServico(CategoriaServicoViewModel vm)
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
                    vm.CASE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.CASE_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_SERVICO item = Mapper.Map<CategoriaServicoViewModel, CATEGORIA_SERVICO>(vm);
                    Int32 volta = csApp.ValidateEdit(item, objetoAntesCatServico, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatServico = new List<CATEGORIA_SERVICO>();
                    Session["ListaCatServico"] = null;
                    Session["CatServicoAlterada"] = 1;
                    return RedirectToAction("MontarTelaCatServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatServico(Int32 id)
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
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            item.CASE_IN_ATIVO = 0;
            Int32 volta = csApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatServico"] = 4;
                return RedirectToAction("MontarTelaCatServico");
            }
            listaMasterCatServico = new List<CATEGORIA_SERVICO>();
            Session["ListaCatServico"] = null;
            Session["CatServicoAlterada"] = 1;
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult ReativarCatServico(Int32 id)
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
            CATEGORIA_SERVICO item = csApp.GetItemById(id);
            item.CASE_IN_ATIVO = 1;
            Int32 volta = csApp.ValidateReativar(item, usuario);
            listaMasterCatServico = new List<CATEGORIA_SERVICO>();
            Session["ListaCatServico"] = null;
            Session["CatServicoAlterada"] = 1;
            return RedirectToAction("MontarTelaCatServico");
        }

        [HttpGet]
        public ActionResult MontarTelaCF()
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
            if (Session["ListaCF"] == null)
            {
                listaMasterCF = cfApp.GetAllItens(idAss);
                Session["ListaCF"] = listaMasterCF;
            }
            ViewBag.Listas = (List<CATEGORIA_FORNECEDOR>)Session["ListaCF"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.CF = ((List<CATEGORIA_FORNECEDOR>)Session["ListaCF"]).Count;

            if (Session["MensCF"] != null)
            {
                if ((Int32)Session["MensCF"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0409", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCF"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCF"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0410", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCF"] = 1;
            Session["MensCF"] = null;
            objetoCF = new CATEGORIA_FORNECEDOR();
            return View(objetoCF);
        }

        public ActionResult RetirarFiltroCF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCF"] = null;
            return RedirectToAction("MontarTelaCF");
        }

        public ActionResult MostrarTudoCF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCF = cfApp.GetAllItensAdm(idAss);
            Session["ListaCF"] = listaMasterCF;
            return RedirectToAction("MontarTelaCF");
        }

        public ActionResult VoltarBaseCF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltarTabs"] == 2)
            {
                return RedirectToAction("MontarTelaRoteiro_0", "Precificacao");
            }
            return RedirectToAction("MontarTelaCF");
        }

        [HttpGet]
        public ActionResult IncluirCF()
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
            CATEGORIA_FORNECEDOR item = new CATEGORIA_FORNECEDOR();
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAFO_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCF(CategoriaFornecedorViewModel vm)
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
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cfApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCF"] = 3;
                        return RedirectToAction("MontarTelaCF");
                    }
                    Session["IdVolta"] = item.CAFO_CD_ID;

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCF"] = null;
                    Session["CatFornecedorAlterada"] = 1;
                    return RedirectToAction("MontarTelaCF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCF(Int32 id)
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
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            objetoAntesCF = item;
            Session["CF"] = item;
            Session["IdCF"] = id;
            CategoriaFornecedorViewModel vm = Mapper.Map<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCF(CategoriaFornecedorViewModel vm)
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
                    CATEGORIA_FORNECEDOR item = Mapper.Map<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>(vm);
                    Int32 volta = cfApp.ValidateEdit(item, objetoAntesCF, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
                    Session["ListaCF"] = null;
                    Session["CatFornecedorAlterada"] = 1;
                    return RedirectToAction("MontarTelaCF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCF(Int32 id)
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
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            objetoAntesCF = item;
            item.CAFO_IN_ATIVO = 0;
            Int32 volta = cfApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCF"] = 4;
                return RedirectToAction("MontarTelaCF");
            }
            listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
            Session["ListaCF"] = null;
            Session["CatFornecedorAlterada"] = 1;
            return RedirectToAction("MontarTelaCF");
        }

        [HttpGet]
        public ActionResult ReativarCF(Int32 id)
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
            CATEGORIA_FORNECEDOR item = cfApp.GetItemById(id);
            item.CAFO_IN_ATIVO = 1;
            objetoAntesCF = item;
            Int32 volta = cfApp.ValidateReativar(item, usuario);
            listaMasterCF = new List<CATEGORIA_FORNECEDOR>();
            Session["ListaCF"] = null;
            Session["CatFornecedorAlterada"] = 1;
            return RedirectToAction("MontarTelaCF");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupo()
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
            if (Session["ListaGrupo"] == null)
            {
                listaMasterGrupo = grApp.GetAllItens(idAss);
                Session["ListaGrupo"] = listaMasterGrupo;
            }
            ViewBag.Listas = (List<GRUPO_CC>)Session["ListaGrupo"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Grupo = ((List<GRUPO_CC>)Session["ListaGrupo"]).Count;

            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupo"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0411", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0412", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaGrupo"] = 1;
            Session["MensGrupo"] = null;
            objetoGrupo= new GRUPO_CC();
            return View(objetoGrupo);
        }

        public ActionResult RetirarFiltroGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult MostrarTudoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterGrupo = grApp.GetAllItensAdm(idAss);
            Session["ListaGrupo"] = listaMasterGrupo;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
            GRUPO_CC item = new GRUPO_CC();
            GrupoCCViewModel vm = Mapper.Map<GRUPO_CC, GrupoCCViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.GRCC_IN_ATIVO = 1;
            vm.GRCC_IN_FIXO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirGrupo(GrupoCCViewModel vm)
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
                    GRUPO_CC item = Mapper.Map<GrupoCCViewModel, GRUPO_CC>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = grApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 3;
                        return RedirectToAction("MontarTelaGrupo");
                    }
                    Session["IdVolta"] = item.GRCC_CD_ID;

                    // Sucesso
                    listaMasterGrupo = new List<GRUPO_CC>();
                    Session["ListaGrupo"] = null;
                    Session["GrupoCCAlterada"] = 1;
                    return RedirectToAction("MontarTelaGrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarGrupo(Int32 id)
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
            GRUPO_CC item = grApp.GetItemById(id);
            objetoAntesGrupo = item;
            Session["Grupo"] = item;
            Session["IdGrupoF"] = id;
            GrupoCCViewModel vm = Mapper.Map<GRUPO_CC, GrupoCCViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoCCViewModel vm)
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
                    GRUPO_CC item = Mapper.Map<GrupoCCViewModel, GRUPO_CC>(vm);
                    Int32 volta = grApp.ValidateEdit(item, objetoAntesGrupo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterGrupo = new List<GRUPO_CC>();
                    Session["ListaGrupo"] = null;
                    Session["GrupoCCAlterada"] = 1;
                    return RedirectToAction("MontarTelaGrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirGrupo(Int32 id)
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
            GRUPO_CC item = grApp.GetItemById(id);
            objetoAntesGrupo = item;
            item.GRCC_IN_ATIVO = 0;
            Int32 volta = grApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupo"] = 4;
                return RedirectToAction("MontarTelaGrupo");
            }
            listaMasterGrupo = new List<GRUPO_CC>();
            Session["ListaGrupo"] = null;
            Session["GrupoCCAlterada"] = 1;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
            GRUPO_CC item = grApp.GetItemById(id);
            item.GRCC_IN_ATIVO = 1;
            objetoAntesGrupo = item;
            Int32 volta = grApp.ValidateReativar(item, usuario);
            listaMasterGrupo = new List<GRUPO_CC>();
            Session["ListaGrupo"] = null;
            Session["GrupoCCAlterada"] = 1;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult MontarTelaSubgrupo()
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
            if (Session["ListaSubgrupo"] == null)
            {
                listaMasterSubgrupo = sgApp.GetAllItens(idAss);
                Session["ListaSubgrupo"] = listaMasterSubgrupo;
            }
            ViewBag.Listas = (List<SUBGRUPO>)Session["ListaSubgrupo"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Subgrupo = ((List<SUBGRUPO>)Session["ListaSubgrupo"]).Count;

            if (Session["MensSubgrupo"] != null)
            {
                if ((Int32)Session["MensSubgrupo"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0413", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0414", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaSubgrupo"] = 1;
            Session["MensSubgrupo"] = null;
            objetoSubgrupo = new SUBGRUPO();
            return View(objetoSubgrupo);
        }

        public ActionResult RetirarFiltroSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult MostrarTudoSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSubgrupo= sgApp.GetAllItensAdm(idAss);
            Session["ListaSubgrupo"] = listaMasterSubgrupo;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult VoltarBaseSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult IncluirSubgrupo()
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
            ViewBag.Grupos = new SelectList(CarregaGrupo(), "GRCC_CD_ID", "GRCC_NM_NOME");

            // Prepara view
            SUBGRUPO item = new SUBGRUPO();
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.SUBG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirSubgrupo(SubgrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Grupos = new SelectList(CarregaGrupo(), "GRCC_CD_ID", "GRCC_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = sgApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubgrupo"] = 3;
                        return RedirectToAction("MontarTelaSubgrupo");
                    }
                    Session["IdVolta"] = item.SUBG_CD_ID;

                    // Sucesso
                    listaMasterSubgrupo = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    Session["SubGrupoCCAlterada"] = 1;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarSubgrupo(Int32 id)
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
            SUBGRUPO item = sgApp.GetItemById(id);
            objetoAntesSubgrupo = item;
            Session["Subgrupo"] = item;
            Session["IdSubgrupo"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubgrupo(SubgrupoViewModel vm)
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
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    Int32 volta = sgApp.ValidateEdit(item, objetoAntesSubgrupo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSubgrupo = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    Session["SubGrupoCCAlterada"] = 1;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirSubgrupo(Int32 id)
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
            SUBGRUPO item = sgApp.GetItemById(id);
            objetoAntesSubgrupo = item;
            item.SUBG_IN_ATIVO = 0;
            Int32 volta = sgApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubgrupo"] = 4;
                return RedirectToAction("MontarTelaSubgrupo");
            }
            listaMasterSubgrupo = new List<SUBGRUPO>();
            Session["ListaSubgrupo"] = null;
            Session["SubGrupoCCAlterada"] = 1;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult ReativarSubgrupo(Int32 id)
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
            SUBGRUPO item = sgApp.GetItemById(id);
            item.SUBG_IN_ATIVO = 1;
            objetoAntesSubgrupo = item;
            Int32 volta = sgApp.ValidateReativar(item, usuario);
            listaMasterSubgrupo = new List<SUBGRUPO>();
            Session["ListaSubgrupo"] = null;
            Session["SubGrupoCCAlterada"] = 1;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public List<GRUPO_CC> CarregaGrupo()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<GRUPO_CC> conf = new List<GRUPO_CC>();
                if (Session["GruposCC"] == null)
                {
                    conf = grApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["GrupoCCAlterada"] == 1)
                    {
                        conf = grApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<GRUPO_CC>)Session["GruposCC"];
                    }
                }
                Session["GruposCC"] = conf;
                Session["GrupoCCAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "SysFin", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaFE()
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
            if (Session["ListaFE"] == null)
            {
                listaMasterFE = feApp.GetAllItens(idAss);
                Session["ListaFE"] = listaMasterFE;
            }
            ViewBag.Listas = (List<FORMA_ENVIO>)Session["ListaFE"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.FE = ((List<CATEGORIA_FORNECEDOR>)Session["ListaFE"]).Count;

            if (Session["MensFE"] != null)
            {
                if ((Int32)Session["MensFE"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0437", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFE"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFE"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0438", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaFE"] = 1;
            Session["MensFE"] = null;
            objetoFE = new FORMA_ENVIO();
            return View(objetoFE);
        }

        public ActionResult RetirarFiltroFE()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFE"] = null;
            return RedirectToAction("MontarTelaFE");
        }

        public ActionResult MostrarTudoFE()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFE = feApp.GetAllItensAdm(idAss);
            Session["ListaFE"] = listaMasterFE;
            return RedirectToAction("MontarTelaFE");
        }

        public ActionResult VoltarBaseFE()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaFE");
        }

        [HttpGet]
        public ActionResult IncluirFE()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            FORMA_ENVIO item = new FORMA_ENVIO();
            FormaEnvioViewModel vm = Mapper.Map<FORMA_ENVIO, FormaEnvioViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.FOEN_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirFE(FormaEnvioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMA_ENVIO item = Mapper.Map<FormaEnvioViewModel, FORMA_ENVIO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = feApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFE"] = 3;
                        return RedirectToAction("MontarTelaFE");
                    }
                    Session["IdVolta"] = item.FOEN_CD_ID;

                    // Sucesso
                    listaMasterFE = new List<FORMA_ENVIO>();
                    Session["ListaFE"] = null;
                    Session["FEAlterada"] = 1;
                    return RedirectToAction("MontarTelaFE");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarFE(Int32 id)
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

            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            FORMA_ENVIO item = feApp.GetItemById(id);
            objetoAntesFE = item;
            Session["FE"] = item;
            Session["IdFE"] = id;
            FormaEnvioViewModel vm = Mapper.Map<FORMA_ENVIO, FormaEnvioViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarFE(FormaEnvioViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMA_ENVIO item = Mapper.Map<FormaEnvioViewModel, FORMA_ENVIO>(vm);
                    Int32 volta = feApp.ValidateEdit(item, objetoAntesFE, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFE = new List<FORMA_ENVIO>();
                    Session["ListaFE"] = null;
                    Session["FEAlterada"] = 1;
                    return RedirectToAction("MontarTelaFE");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirFE(Int32 id)
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
            FORMA_ENVIO item = feApp.GetItemById(id);
            objetoAntesFE = item;
            item.FOEN_IN_ATIVO = 0;
            Int32 volta = feApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensFE"] = 4;
                return RedirectToAction("MontarTelaFE");
            }
            listaMasterFE = new List<FORMA_ENVIO>();
            Session["ListaFE"] = null;
            Session["FEAlterada"] = 1;
            return RedirectToAction("MontarTelaFE");
        }

        [HttpGet]
        public ActionResult ReativarFE(Int32 id)
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
            FORMA_ENVIO item = feApp.GetItemById(id);
            item.FOEN_IN_ATIVO = 1;
            objetoAntesFE = item;
            Int32 volta = feApp.ValidateReativar(item, usuario);
            listaMasterFE = new List<FORMA_ENVIO>();
            Session["ListaFE"] = null;
            Session["FEAlterada"] = 1;
            return RedirectToAction("MontarTelaFE");
        }

        [HttpGet]
        public ActionResult MontarTelaFF()
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
            if (Session["ListaFF"] == null)
            {
                listaMasterFF = ffApp.GetAllItens(idAss);
                Session["ListaFF"] = listaMasterFF;
            }
            ViewBag.Listas = (List<FORMA_FRETE>)Session["ListaFF"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.FF = ((List<FORMA_FRETE>)Session["ListaFF"]).Count;

            if (Session["MensFF"] != null)
            {
                if ((Int32)Session["MensFF"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0439", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFF"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensFF"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0438", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaFF"] = 1;
            Session["MensFF"] = null;
            objetoFF = new FORMA_FRETE();
            return View(objetoFF);
        }

        public ActionResult RetirarFiltroFF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaFF"] = null;
            return RedirectToAction("MontarTelaFF");
        }

        public ActionResult MostrarTudoFF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFF= ffApp.GetAllItensAdm(idAss);
            Session["ListaFF"] = listaMasterFF;
            return RedirectToAction("MontarTelaFF");
        }

        public ActionResult VoltarBaseFF()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaFF");
        }

        [HttpGet]
        public ActionResult IncluirFF()
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            FORMA_FRETE item = new FORMA_FRETE();
            FormaFreteViewModel vm = Mapper.Map<FORMA_FRETE, FormaFreteViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.FOFR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirFF(FormaFreteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    FORMA_FRETE item = Mapper.Map<FormaFreteViewModel, FORMA_FRETE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ffApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensFF"] = 3;
                        return RedirectToAction("MontarTelaFF");
                    }
                    Session["IdVolta"] = item.FOFR_CD_ID;

                    // Sucesso
                    listaMasterFF = new List<FORMA_FRETE>();
                    Session["ListaFF"] = null;
                    Session["FFAlterada"] = 1;
                    return RedirectToAction("MontarTelaFF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarFF(Int32 id)
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
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            FORMA_FRETE item = ffApp.GetItemById(id);
            objetoAntesFF = item;
            Session["FF"] = item;
            Session["IdFF"] = id;
            FormaFreteViewModel vm = Mapper.Map<FORMA_FRETE, FormaFreteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarFF(FormaFreteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Entrega", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Recebimento", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    FORMA_FRETE item = Mapper.Map<FormaFreteViewModel, FORMA_FRETE>(vm);
                    Int32 volta = ffApp.ValidateEdit(item, objetoAntesFF, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFF = new List<FORMA_FRETE>();
                    Session["ListaFF"] = null;
                    Session["FFAlterada"] = 1;
                    return RedirectToAction("MontarTelaFF");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirFF(Int32 id)
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
            FORMA_FRETE item = ffApp.GetItemById(id);
            objetoAntesFF = item;
            item.FOFR_IN_ATIVO = 0;
            Int32 volta = ffApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensFF"] = 4;
                return RedirectToAction("MontarTelaFF");
            }
            listaMasterFF = new List<FORMA_FRETE>();
            Session["ListaFF"] = null;
            Session["FFAlterada"] = 1;
            return RedirectToAction("MontarTelaFF");
        }

        [HttpGet]
        public ActionResult ReativarFF(Int32 id)
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
            FORMA_FRETE item = ffApp.GetItemById(id);
            item.FOFR_IN_ATIVO = 1;
            objetoAntesFF = item;
            Int32 volta = ffApp.ValidateReativar(item, usuario);
            listaMasterFF = new List<FORMA_FRETE>();
            Session["ListaFF"] = null;
            Session["FFAlterada"] = 1;
            return RedirectToAction("MontarTelaFF");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoVeiculo()
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
            if (Session["ListaTipoVeiculo"] == null)
            {
                listaMasterTV = tvApp.GetAllItens(idAss);
                Session["ListaTipoVeiculo"] = listaMasterTV;
            }
            ViewBag.Listas = (List<TIPO_VEICULO>)Session["ListaTipoVeiculo"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoVeiculo = ((List<TIPO_VEICULO>)Session["ListaTipoVeiculo"]).Count;

            if (Session["MensTipoVeiculo"] != null)
            {
                if ((Int32)Session["MensTipoVeiculo"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0480", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoVeiculo"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoVeiculo"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0481", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoVeiculo"] = 1;
            Session["MensTipoVeiculo"] = 0;
            objetoTV = new TIPO_VEICULO();
            return View(objetoTV);
        }

        public ActionResult RetirarFiltroTipoVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoVeiculo"] = null;
            Session["FiltroTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoVeiculo");
        }

        public ActionResult MostrarTudoTipoVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTV = tvApp.GetAllItensAdm(idAss);
            Session["ListaTipoVeiculo"] = listaMasterTV;
            return RedirectToAction("MontarTelaTipoVeiculo");
        }

        public ActionResult VoltarBaseTipoVeiculo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoVeiculo");
        }

        [HttpGet]
        public ActionResult IncluirTipoVeiculo()
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
            TIPO_VEICULO item = new TIPO_VEICULO();
            TipoVeiculoViewModel vm = Mapper.Map<TIPO_VEICULO, TipoVeiculoViewModel>(item);
            vm.ASS_CD_ID = usuario.ASSI_CD_ID;
            vm.TIVE_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoVeiculo(TipoVeiculoViewModel vm)
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
                    vm.TIVE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIVE_NM_NOME);

                    // Executa a operação
                    TIPO_VEICULO item = Mapper.Map<TipoVeiculoViewModel, TIPO_VEICULO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = tvApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoVeiculo"] = 3;
                        return RedirectToAction("MontarTelaTipoVeiculo");
                    }

                    // Sucesso
                    listaMasterTV = new List<TIPO_VEICULO>();
                    Session["ListaTipoVeiculo"] = null;
                    Session["TipoVeiculoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoVeiculo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoVeiculo(Int32 id)
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
            TIPO_VEICULO item = tvApp.GetItemById(id);
            objetoAntesTV = item;
            Session["TipoVeiculo"] = item;
            Session["IdTipoVeiculo"] = id;
            TipoVeiculoViewModel vm = Mapper.Map<TIPO_VEICULO, TipoVeiculoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoVeiculo(TipoVeiculoViewModel vm)
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
                    vm.TIVE_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TIVE_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_VEICULO item = Mapper.Map<TipoVeiculoViewModel, TIPO_VEICULO>(vm);
                    Int32 volta = tvApp.ValidateEdit(item, objetoAntesTV, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTV = new List<TIPO_VEICULO>();
                    Session["ListaTipoVeiculo"] = null;
                    Session["TipoVeiculoAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoVeiculo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoVeiculo(Int32 id)
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
                TIPO_VEICULO item = tvApp.GetItemById(id);
                objetoAntesTV = item;
                item.TIVE_IN_ATIVO = 0;
                Int32 volta = tvApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTipoVeiculo"] = 4;
                    return RedirectToAction("MontarTelaTipoVeiculo");
                }
                listaMasterTV = new List<TIPO_VEICULO>();
                Session["ListaTipoVeiculo"] = null;
                Session["TipoVeiculoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoVeiculo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoVeiculo(Int32 id)
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
                TIPO_VEICULO item = tvApp.GetItemById(id);
                item.TIVE_IN_ATIVO = 1;
                objetoAntesTV = item;
                Int32 volta = tvApp.ValidateReativar(item, usuario);
                listaMasterTV = new List<TIPO_VEICULO>();
                Session["ListaTipoVeiculo"] = null;
                Session["TipoVeiculoAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoVeiculo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTipoTransporte()
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
            if (Session["ListaTipoTransporte"] == null)
            {
                listaMasterTR = trApp.GetAllItens(idAss);
                Session["ListaTipoTransporte"] = listaMasterTR;
            }
            ViewBag.Listas = (List<TIPO_TRANSPORTE>)Session["ListaTipoTransporte"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoTransporte = ((List<TIPO_TRANSPORTE>)Session["ListaTipoTransporte"]).Count;

            if (Session["MensTipoTransporte"] != null)
            {
                if ((Int32)Session["MensTipoTransporte"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0482", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTransporte"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTransporte"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0483", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoTransporte"] = 1;
            Session["MensTipoTransporte"] = 0;
            Session["VoltaTransp"] = 0;
            objetoTR = new TIPO_TRANSPORTE();
            return View(objetoTR);
        }

        public ActionResult RetirarFiltroTipoTransporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoTransporte"] = null;
            return RedirectToAction("MontarTelaTipoTransporte");
        }

        public ActionResult MostrarTudoTipoTransporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTR  = trApp.GetAllItensAdm(idAss);
            Session["ListaTipoTransporte"] = listaMasterTR;
            return RedirectToAction("MontarTelaTipoTransporte");
        }

        public ActionResult VoltarBaseTipoTransporte()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoTransporte");
        }

        [HttpGet]
        public ActionResult IncluirTipoTransporte()
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
            TIPO_TRANSPORTE item = new TIPO_TRANSPORTE();
            TipoTransporteViewModel vm = Mapper.Map<TIPO_TRANSPORTE, TipoTransporteViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TITR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoTransporte(TipoTransporteViewModel vm)
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
                    vm.TITR_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TITR_NM_NOME);

                    // Executa a operação
                    TIPO_TRANSPORTE item = Mapper.Map<TipoTransporteViewModel, TIPO_TRANSPORTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = trApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoTransporte"] = 3;
                        return RedirectToAction("MontarTelaTipoTransporte");
                    }

                    // Sucesso
                    listaMasterTR  = new List<TIPO_TRANSPORTE>();
                    Session["ListaTipoTransporte"] = null;
                    Session["TipoTransporteAlterada"] = 1;
                    if ((Int32)Session["VoltaTransp"] > 0)
                    {
                        return RedirectToAction("VoltarBaseTransportadora", "Transportadora");
                    }
                    return RedirectToAction("MontarTelaTipoTransporte");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarTipoTransporte(Int32 id)
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
            TIPO_TRANSPORTE item = trApp.GetItemById(id);
            objetoAntesTR = item;
            Session["TipoTransporte"] = item;
            Session["IdTipoTransporte"] = id;
            TipoTransporteViewModel vm = Mapper.Map<TIPO_TRANSPORTE, TipoTransporteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarTipoTransporte(TipoTransporteViewModel vm)
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
                    vm.TITR_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.TITR_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_TRANSPORTE item = Mapper.Map<TipoTransporteViewModel, TIPO_TRANSPORTE>(vm);
                    Int32 volta = trApp.ValidateEdit(item, objetoAntesTR, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTR  = new List<TIPO_TRANSPORTE>();
                    Session["ListaTipoTransporte"] = null;
                    Session["TipoTransporteAlterada"] = 1;
                    return RedirectToAction("MontarTelaTipoTransporte");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoTransporte(Int32 id)
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
                TIPO_TRANSPORTE item = trApp.GetItemById(id);
                objetoAntesTR = item;
                item.TITR_IN_ATIVO = 0;
                Int32 volta = trApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensTipoTransporte"] = 4;
                    return RedirectToAction("MontarTelaTipoTransporte");
                }
                listaMasterTR = new List<TIPO_TRANSPORTE>();
                Session["ListaTipoTransporte"] = null;
                Session["TipoTransporteAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoTransporte");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarTipoTransporte(Int32 id)
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
                TIPO_TRANSPORTE item = trApp.GetItemById(id);
                item.TITR_IN_ATIVO = 1;
                objetoAntesTR = item;
                Int32 volta = trApp.ValidateReativar(item, usuario);
                listaMasterTR = new List<TIPO_TRANSPORTE>();
                Session["ListaTipoTransporte"] = null;
                Session["TipoTransporteAlterada"] = 1;
                return RedirectToAction("MontarTelaTipoTransporte");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaVeiculoFuncao()
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
            if (Session["ListaVeiculoFuncao"] == null)
            {
                listaMasterVF = vfApp.GetAllItens(idAss);
                Session["ListaVeiculoFuncao"] = listaMasterVF;
            }
            ViewBag.Listas = (List<VEICULO_FUNCAO>)Session["ListaVeiculoFuncao"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.VeiculoFuncao = ((List<VEICULO_FUNCAO>)Session["ListaVeiculoFuncao"]).Count;

            if (Session["MensVeiculoFuncao"] != null)
            {
                if ((Int32)Session["MensVeiculoFuncao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0484", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensVeiculoFuncao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensVeiculoFuncao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0485", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaVeiculoFuncao"] = 1;
            Session["MensVeiculoFuncao"] = 0;
            objetoVF  = new VEICULO_FUNCAO();
            return View(objetoVF);
        }

        public ActionResult RetirarFiltroVeiculoFuncao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaVeiculoFuncao"] = null;
            return RedirectToAction("MontarTelaVeiculoFuncao");
        }

        public ActionResult MostrarTudoVeiculoFuncao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterVF  = vfApp.GetAllItensAdm(idAss);
            Session["ListaVeiculoFuncao"] = listaMasterVF;
            return RedirectToAction("MontarTelaVeiculoFuncao");
        }

        public ActionResult VoltarBaseVeiculoFuncao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaVeiculoFuncao");
        }

        [HttpGet]
        public ActionResult IncluirVeiculoFuncao()
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
            VEICULO_FUNCAO item = new VEICULO_FUNCAO();
            VeiculoFuncaoViewModel vm = Mapper.Map<VEICULO_FUNCAO, VeiculoFuncaoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.VEFU_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirVeiculoFuncao(VeiculoFuncaoViewModel vm)
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
                    vm.VEFU_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.VEFU_NM_NOME);

                    // Executa a operação
                    VEICULO_FUNCAO item = Mapper.Map<VeiculoFuncaoViewModel, VEICULO_FUNCAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = vfApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensVeiculoFuncao"] = 3;
                        return RedirectToAction("MontarTelaVeiculoFuncao");
                    }

                    // Sucesso
                    listaMasterVF  = new List<VEICULO_FUNCAO>();
                    Session["ListaVeiculoFuncao"] = null;
                    Session["VeiculoFuncaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaVeiculoFuncao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarVeiculoFuncao(Int32 id)
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
            VEICULO_FUNCAO item = vfApp.GetItemById(id);
            objetoAntesVF = item;
            Session["VeiculoFuncao"] = item;
            Session["IdVeiculoFuncao"] = id;
            VeiculoFuncaoViewModel vm = Mapper.Map<VEICULO_FUNCAO, VeiculoFuncaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarVeiculoFuncao(VeiculoFuncaoViewModel vm)
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
                    vm.VEFU_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.VEFU_NM_NOME);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    VEICULO_FUNCAO item = Mapper.Map<VeiculoFuncaoViewModel, VEICULO_FUNCAO>(vm);
                    Int32 volta = vfApp.ValidateEdit(item, objetoAntesVF, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterVF  = new List<VEICULO_FUNCAO>();
                    Session["ListaVeiculoFuncao"] = null;
                    Session["VeiculoFuncaoAlterada"] = 1;
                    return RedirectToAction("MontarTelaVeiculoFuncao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Tabelas";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirVeiculoFuncao(Int32 id)
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
                VEICULO_FUNCAO item = vfApp.GetItemById(id);
                objetoAntesVF = item;
                item.VEFU_IN_ATIVO = 0;
                Int32 volta = vfApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensVeiculoFuncao"] = 4;
                    return RedirectToAction("MontarTelaVeiculoFuncao");
                }
                listaMasterVF = new List<VEICULO_FUNCAO>();
                Session["ListaVeiculoFuncao"] = null;
                Session["VeiculoFuncaoAlterada"] = 1;
                return RedirectToAction("MontarTelaVeiculoFuncao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarVeiculoFuncao(Int32 id)
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
                VEICULO_FUNCAO item = vfApp.GetItemById(id);
                item.VEFU_IN_ATIVO = 1;
                objetoAntesVF = item;
                Int32 volta = vfApp.ValidateReativar(item, usuario);
                listaMasterVF = new List<VEICULO_FUNCAO>();
                Session["ListaVeiculoFuncao"] = null;
                Session["VeiculoFuncaoAlterada"] = 1;
                return RedirectToAction("MontarTelaVeiculoFuncao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Tabelas";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Tabelas", "ERPSys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}