using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using ERP_Condominios_Solution.Classes;
using ApplicationServices.Services;
using GEDSys_Presentation.App_Start;


namespace CRMPresentation.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IPerfilAppService baseApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ILogAppService logApp;
        private readonly IAcessoMetodoAppService aceApp;

        private String msg;
        private Exception exception;
        PERFIL objeto = new PERFIL();
        PERFIL objetoAntes = new PERFIL();
        List<PERFIL> listaMaster = new List<PERFIL>();
        String extensao;

        public PerfilController(IPerfilAppService baseApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps, LogAppService logApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            usuApp = usuApps;
            confApp = confApps;
            logApp = logApps;
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
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaPerfil()
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
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Perfil";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Perfil";

                // Carrega listas
                if ((List<PERFIL>)Session["ListaPerfilBase"] == null)
                {
                    listaMaster = CarregaPerfil();
                    Session["ListaPerfilBase"] = listaMaster;
                }
                ViewBag.Listas = (List<PERFIL>)Session["ListaPerfilBase"];

                // Mensagens
                if (Session["MensPerfil"] != null)
                {
                    if ((Int32)Session["MensPerfil"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPerfil"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPerfil"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0422", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPerfil"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0423", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPerfil"] == 61)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                    if ((Int32)Session["MensPerfil"] == 999)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PERFIL", "Perfil", "MontarTelaPerfil");

                // Abre view
                Session["MensPerfil"] = null;
                Session["VoltaPerfil"] = 1;
                Session["TabPerfil"] = 1;
                Session["ListaLog"] = null;
                objeto = new PERFIL();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroPerfil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaPerfilBase"] = null;
            return RedirectToAction("MontarTelaPerfil");
        }

        public ActionResult VoltarBasePerfil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaPerfilBase"] = null;
            return RedirectToAction("MontarTelaPerfil");
        }

       [HttpGet]
        public ActionResult IncluirPerfil()
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
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Perfil";
                        return RedirectToAction("MontarTelaPerfil", "Perfil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Perfil - Inclusão";

                // Monta listas
                var tudo = new List<SelectListItem>();
                tudo.Add(new SelectListItem() { Text = "Permitir Todos os Acessos", Value = "1" });
                tudo.Add(new SelectListItem() { Text = "Permitir Acessos Individualmente", Value = "2" });
                ViewBag.Tudo = new SelectList(tudo, "Value", "Text");

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0571", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 568)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0568", CultureInfo.CurrentCulture));
                    }   
                    if ((Int32)Session["MensPaciente"] == 569)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0569", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensPaciente"] == 570)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0570", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PERFIL_INCLUSAO", "Perfil", "IncluirPerfil");

                // Prepara view
                Session["MensPaciente"] = null;
                PERFIL item = new PERFIL();
                PerfilViewModel vm = Mapper.Map<PERFIL, PerfilViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.PERF_IN_ATIVO = 1;
                vm.PERF_IN_FIXO = 0;
                vm.SELECIONAR_TUDO = 0;
                PerfilViewModel vm1 = PreencherTudo(vm);
                return View(vm1);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirPerfil(PerfilViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            var tudo = new List<SelectListItem>();
            tudo.Add(new SelectListItem() { Text = "Permitir Todos os Acessos", Value = "1" });
            tudo.Add(new SelectListItem() { Text = "Permitir Acessos Individualmente", Value = "2" });
            ViewBag.Tudo = new SelectList(tudo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PERF_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PERF_NM_NOME);
                    vm.PERF_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PERF_SG_SIGLA);

                    // Critica de seleção
                    PerfilViewModel vm1 = new PerfilViewModel();
                    Int32 volta1 = 0;
                    if (vm.SELECIONAR_TUDO == 0 || vm.SELECIONAR_TUDO == null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0570", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.SELECIONAR_TUDO == 1)
                    {
                        vm1 = SelecionarTudo(vm);
                    }
                    else
                    {
                        volta1 = VerificarTudo(vm);
                        vm1 = vm;
                    }
                    if (volta1 == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0568", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação   
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    PERFIL item = Mapper.Map<PerfilViewModel, PERFIL>(vm1);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPaciente"] = 3;
                        return View(vm);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O perfil " + item.PERF_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensPaciente"] = 999;

                    // Sucesso
                    listaMaster = new List<PERFIL>();
                    Session["ListaPerfilBase"] = null;
                    Session["IdPerfil"] = item.PERF_CD_ID;
                    Session["PerfilAlterada"] = 1;
                    Session["TabPerfil"] = 1;
                    Session["PerfisBase"] = null;
                    return RedirectToAction("MontarTelaPerfil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Perfil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarPerfil(Int32 id)
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
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Perfil";
                        return RedirectToAction("MontarTelaPerfil", "Perfil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Perfil - Edição";

                // Prepara view
                PERFIL item = baseApp.GetItemById(id);
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                Session["PerfilAntes"] = item;

                if (Session["MensPaciente"] != null)
                {
                    if ((Int32)Session["MensPaciente"] == 999)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PERFIL_EDICAO", "Perfil", "EditarPerfil");

                // Indicadores
                Session["VoltaPerfil"] = 1;
                objetoAntes = item;
                Session["PerfilBase"] = item;
                Session["IdPerfil"] = id;
                PerfilViewModel vm = Mapper.Map<PERFIL, PerfilViewModel>(item);
                return View(vm);

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EditarPerfil(PerfilViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PERF_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PERF_NM_NOME);
                    vm.PERF_SG_SIGLA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.PERF_SG_SIGLA);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PERFIL item = Mapper.Map<PerfilViewModel, PERFIL>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (PERFIL)Session["PerfilAntes"], usuarioLogado);

                    // Verifica retorno

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O perfil " + item.PERF_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensPaciente"] = 999;

                    // Sucesso
                    listaMaster = new List<PERFIL>();
                    Session["ListaPerfilBase"] = null;
                    Session["PerfilAlterada"] = 1;
                    Session["PerfisBase"] = null;
                    return RedirectToAction("VoltarAnexoPerfil");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Perfil";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirPerfil(Int32 id)
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
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Perfil";
                        return RedirectToAction("MontarTelaPerfil", "Perfil");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Perfil - Exclusão";

                // Executar
                PERFIL item = baseApp.GetItemById(id);
                objetoAntes = (PERFIL)Session["PerfilBase"];
                item.PERF_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta == 1)
                {
                    Session["MensPerfil"] = 4;
                    return RedirectToAction("MontarTelaPerfil", "Perfil");
                }
                listaMaster = new List<PERFIL>();
                Session["ListaPerfilBase"] = null;
                Session["PerfilAlterada"] = 1;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "PERFIL_EXCLUIR", "Perfil", "ExcluirPerfil");

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O perfil " + item.PERF_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensPerfil"] = 61;

                return RedirectToAction("MontarTelaPerfil");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoPerfil()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarPerfil", new { id = (Int32)Session["IdPerfil"] });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public List<PERFIL> CarregaPerfil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PERFIL> conf = new List<PERFIL>();
                if (Session["PerfisBase"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["PerfilAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<PERFIL>)Session["PerfisBase"];
                    }
                }
                Session["PerfisBase"] = conf;
                Session["PerfilAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public PerfilViewModel SelecionarTudo(PerfilViewModel vm)
        {
            try
            {
                PerfilViewModel m = new PerfilViewModel();
                m.PAERF_IN_PACIENTE_MENSAGEM = 1;
                m.PERF_IN_ACESSO_ADMIN = 1;
                m.PERF_IN_ACESSO_AUX = 1;
                m.PERF_IN_ACESSO_EMPRESA = 1;
                m.PERF_IN_ACESSO_GRUPO = 1;
                m.PERF_IN_ACESSO_MENSAGEM = 1;
                m.PERF_IN_ACESSO_PACIENTE = 1;
                m.PERF_IN_ACESSO_TEMPLATE = 1;
                m.PERF_IN_ACESSO_USUARIO = 1;
                m.PERF_IN_ALTERAR_PACIENTE = 1;
                m.PERF_IN_ANAMNESE_ACESSO = 1;
                m.PERF_IN_ANAMNESE_ALTERAR = 1;
                m.PERF_IN_ANAMNESE_EXCLUIR = 1;
                m.PERF_IN_ANAMNESE_INCLUIR = 1;
                m.PERF_IN_ATESTADO_ACESSO = 1;
                m.PERF_IN_ATESTADO_ALTERAR = 1;
                m.PERF_IN_ATESTADO_ENVIAR = 1;
                m.PERF_IN_ATESTADO_EXCLUIR = 1;
                m.PERF_IN_ATESTADO_INCLUIR = 1;
                m.PERF_IN_BLOQUEIO = 1;
                m.PERF_IN_BLOQUEIO_USUARIO = 1;
                m.PERF_IN_CONF_CANC_CONSULTA = 1;
                m.PERF_IN_EDICAO_AUX = 1;
                m.PERF_IN_EDICAO_EMPRESA = 1;
                m.PERF_IN_EDICAO_GRUPO = 1;
                m.PERF_IN_EDICAO_USUARIO = 1;
                m.PERF_IN_EDITAR_TEMPLATE = 1;
                m.PERF_IN_EXAME_ACESSO = 1;
                m.PERF_IN_EXAME_ALTERAR = 1;
                m.PERF_IN_EXAME_EXCLUIR = 1;
                m.PERF_IN_EXAME_INCLUIR = 1;
                m.PERF_IN_EXCLUIR_PACIENTE = 1;
                m.PERF_IN_EXCLUSAO_AUX = 1;
                m.PERF_IN_EXCLUSAO_GRUPO = 1;
                m.PERF_IN_EXCLUSAO_TEMPLATE = 1;
                m.PERF_IN_EXCLUSAO_USUARIO = 1;
                m.PERF_IN_FISICO_ACESSO = 1;
                m.PERF_IN_FISICO_ALTERAR = 1;
                m.PERF_IN_FISICO_EXCLUIR = 1;
                m.PERF_IN_FISICO_INCLUIR = 1;
                m.PERF_IN_INCLUIR_PACIENTE = 1;
                m.PERF_IN_INCLUSAO_AUX = 1;
                m.PERF_IN_INCLUSAO_GRUPO = 1;
                m.PERF_IN_INCLUSAO_MENSAGEM = 1;
                m.PERF_IN_INCLUSAO_TEMPLATE = 1;
                m.PERF_IN_INCLUSAO_USUARIO = 1;
                m.PERF_IN_PACIENTE_ATESTADO = 1;
                m.PERF_IN_PACIENTE_CONSULTA_ACESSO = 1;
                m.PERF_IN_PACIENTE_CONSULTA_ALTERAR = 1;
                m.PERF_IN_PACIENTE_CONSULTA_EXCLUIR = 1;
                m.PERF_IN_PACIENTE_CONSULTA_INCLUIR = 1;
                m.PERF_IN_PACIENTE_SOLICITACAO = 1;
                m.PERF_IN_PRESCRICAO_ACESSO = 1;
                m.PERF_IN_PRESCRICAO_ALTERAR = 1;
                m.PERF_IN_PRESCRICAO_ENVIAR = 1;
                m.PERF_IN_PRESCRICAO_EXCLUIR = 1;
                m.PERF_IN_PRESCRICAO_INCLUIR = 1;
                m.PERF_IN_REATIVACAO_AUX = 1;
                m.PERF_IN_REATIVACAO_USUARIO = 1;
                m.PERF_IN_REATIVAR_GRUPO = 1;
                m.PERF_IN_REATIVAR_PACIENTE = 1;
                m.PERF_IN_REATIVA_TEMPLATE = 1;
                m.PERF_IN_SOLICITACAO_ACESSO = 1;
                m.PERF_IN_SOLICITACAO_ALTERAR = 1;
                m.PERF_IN_SOLICITACAO_ENVIAR = 1;
                m.PERF_IN_SOLICITACAO_EXCLUIR = 1;
                m.PERF_IN_SOLICITACAO_INCLUIR = 1;
                m.PERF_IN_VISAO_GERAL = 1;
                m.PERF_NM_NOME = vm.PERF_NM_NOME;
                m.PERF_SG_SIGLA = vm.PERF_SG_SIGLA;
                m.ASSI_CD_ID = vm.ASSI_CD_ID;
                m.PERF_IN_ATIVO = 1;
                m.PERF_IN_FIXO = 0;
                m.PERF_IN_CONF_CANC_CONSULTA = 1;
                m.PERF_IN_FINANCEIRO_ACESSO = 1;
                m.PERF_IN_FINANCEIRO_PAG_ACESSO = 1;
                m.PERF_IN_FINANCEIRO_PAG_ALTERAR = 1;
                m.PERF_IN_FINANCEIRO_PAG_EXCLUIR = 1;
                m.PERF_IN_FINANCEIRO_PAG_INCLUIR = 1;
                m.PERF_IN_FINANCEIRO_REC_ACESSO = 1;
                m.PERF_IN_FINANCEIRO_REC_ALTERAR = 1;
                m.PERF_IN_FINANCEIRO_REC_EXCLUIR = 1;
                m.PERF_IN_FINANCEIRO_REC_INCLUIR = 1;
                m.PERF_IN_FINANCEIRO_RELATORIO = 1;
                m.PERF_IN_FINANCEIRO_CONTADOR = 1;
                return m;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public Int32 VerificarTudo(PerfilViewModel vm)
        {
            try
            {
                Int32 temPermissao = 1;
                if (vm.PERF_IN_ACESSO_PACIENTE == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_ATESTADO_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_PACIENTE_CONSULTA_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_SOLICITACAO_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_EXAME_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_PRESCRICAO_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_ACESSO_ADMIN == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_ACESSO_MENSAGEM == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_FINANCEIRO_PAG_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                if (vm.PERF_IN_FINANCEIRO_REC_ACESSO == 1)
                {
                    temPermissao = 0;
                }
                return temPermissao;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 2;
            }
        }

        public PerfilViewModel PreencherTudo(PerfilViewModel vm)
        {
            try
            {
                PerfilViewModel m = new PerfilViewModel();
                m.PAERF_IN_PACIENTE_MENSAGEM = 0;
                m.PERF_IN_ACESSO_ADMIN = 0;
                m.PERF_IN_ACESSO_AUX = 0;
                m.PERF_IN_ACESSO_EMPRESA = 0;
                m.PERF_IN_ACESSO_GRUPO = 0;
                m.PERF_IN_ACESSO_MENSAGEM = 0;
                m.PERF_IN_ACESSO_PACIENTE = 0;
                m.PERF_IN_ACESSO_TEMPLATE = 0;
                m.PERF_IN_ACESSO_USUARIO = 0;
                m.PERF_IN_ALTERAR_PACIENTE = 0;
                m.PERF_IN_ANAMNESE_ACESSO = 0;
                m.PERF_IN_ANAMNESE_ALTERAR = 0;
                m.PERF_IN_ANAMNESE_EXCLUIR = 0;
                m.PERF_IN_ANAMNESE_INCLUIR = 0;
                m.PERF_IN_ATESTADO_ACESSO = 0;
                m.PERF_IN_ATESTADO_ALTERAR = 0;
                m.PERF_IN_ATESTADO_ENVIAR = 0;
                m.PERF_IN_ATESTADO_EXCLUIR = 0;
                m.PERF_IN_ATESTADO_INCLUIR = 0;
                m.PERF_IN_BLOQUEIO = 0;
                m.PERF_IN_BLOQUEIO_USUARIO = 0;
                m.PERF_IN_CONF_CANC_CONSULTA = 0;
                m.PERF_IN_EDICAO_AUX = 0;
                m.PERF_IN_EDICAO_EMPRESA = 0;
                m.PERF_IN_EDICAO_GRUPO = 0;
                m.PERF_IN_EDICAO_USUARIO = 0;
                m.PERF_IN_EDITAR_TEMPLATE = 0;
                m.PERF_IN_EXAME_ACESSO = 0;
                m.PERF_IN_EXAME_ALTERAR = 0;
                m.PERF_IN_EXAME_EXCLUIR = 0;
                m.PERF_IN_EXAME_INCLUIR = 0;
                m.PERF_IN_EXCLUIR_PACIENTE = 0;
                m.PERF_IN_EXCLUSAO_AUX = 0;
                m.PERF_IN_EXCLUSAO_GRUPO = 0;
                m.PERF_IN_EXCLUSAO_TEMPLATE = 0;
                m.PERF_IN_EXCLUSAO_USUARIO = 0;
                m.PERF_IN_FISICO_ACESSO = 0;
                m.PERF_IN_FISICO_ALTERAR = 0;
                m.PERF_IN_FISICO_EXCLUIR = 0;
                m.PERF_IN_FISICO_INCLUIR = 0;
                m.PERF_IN_INCLUIR_PACIENTE = 0;
                m.PERF_IN_INCLUSAO_AUX = 0;
                m.PERF_IN_INCLUSAO_GRUPO = 0;
                m.PERF_IN_INCLUSAO_MENSAGEM = 0;
                m.PERF_IN_INCLUSAO_TEMPLATE = 0;
                m.PERF_IN_INCLUSAO_USUARIO = 0;
                m.PERF_IN_PACIENTE_ATESTADO = 0;
                m.PERF_IN_PACIENTE_CONSULTA_ACESSO = 0;
                m.PERF_IN_PACIENTE_CONSULTA_ALTERAR = 0;
                m.PERF_IN_PACIENTE_CONSULTA_EXCLUIR = 0;
                m.PERF_IN_PACIENTE_CONSULTA_INCLUIR = 0;
                m.PERF_IN_PACIENTE_SOLICITACAO = 0;
                m.PERF_IN_PRESCRICAO_ACESSO = 0;
                m.PERF_IN_PRESCRICAO_ALTERAR = 0;
                m.PERF_IN_PRESCRICAO_ENVIAR = 0;
                m.PERF_IN_PRESCRICAO_EXCLUIR = 0;
                m.PERF_IN_PRESCRICAO_INCLUIR = 0;
                m.PERF_IN_REATIVACAO_AUX = 0;
                m.PERF_IN_REATIVACAO_USUARIO = 0;
                m.PERF_IN_REATIVAR_GRUPO = 0;
                m.PERF_IN_REATIVAR_PACIENTE = 0;
                m.PERF_IN_REATIVA_TEMPLATE = 0;
                m.PERF_IN_SOLICITACAO_ACESSO = 0;
                m.PERF_IN_SOLICITACAO_ALTERAR = 0;
                m.PERF_IN_SOLICITACAO_ENVIAR = 0;
                m.PERF_IN_SOLICITACAO_EXCLUIR = 0;
                m.PERF_IN_SOLICITACAO_INCLUIR = 0;
                m.PERF_IN_VISAO_GERAL = 0;
                m.PERF_IN_ATIVO = 1;
                m.PERF_IN_FIXO = 0;
                m.PERF_IN_CONF_CANC_CONSULTA = 0;
                m.PERF_IN_FINANCEIRO_ACESSO = 0;
                m.PERF_IN_FINANCEIRO_PAG_ACESSO = 0;
                m.PERF_IN_FINANCEIRO_PAG_ALTERAR = 0;
                m.PERF_IN_FINANCEIRO_PAG_EXCLUIR = 0;
                m.PERF_IN_FINANCEIRO_PAG_INCLUIR = 0;
                m.PERF_IN_FINANCEIRO_REC_ACESSO = 0;
                m.PERF_IN_FINANCEIRO_REC_ALTERAR = 0;
                m.PERF_IN_FINANCEIRO_REC_EXCLUIR = 0;
                m.PERF_IN_FINANCEIRO_REC_INCLUIR = 0;
                m.PERF_IN_FINANCEIRO_RELATORIO = 0;
                m.PERF_IN_FINANCEIRO_CONTADOR = 0;
                m.ASSI_CD_ID = vm.ASSI_CD_ID;
                return m;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Perfil";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Perfil", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}