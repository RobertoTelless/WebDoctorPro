using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Reflection;
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.Controllers
{
    public class GrupoController : Controller
    {
        private readonly IGrupoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IPacienteAppService cliApp;
        private readonly IMensagemAppService menApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;

#pragma warning disable CS0169 // O campo "GrupoController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "GrupoController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "GrupoController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "GrupoController.exception" nunca é usado
        GRUPO_PAC objeto = new GRUPO_PAC();
        GRUPO_PAC objetoAntes = new GRUPO_PAC();
        List<GRUPO_PAC> listaMaster = new List<GRUPO_PAC>();
#pragma warning disable CS0169 // O campo "GrupoController.extensao" nunca é usado
        String extensao;
#pragma warning restore CS0169 // O campo "GrupoController.extensao" nunca é usado

        public GrupoController(IGrupoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IPacienteAppService cliApps, IMensagemAppService menApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            cliApp = cliApps;
            menApp = menApps;
            empApp = empApps;
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

        private int div(int x)
        {
            return x / 0;
        }

        public ActionResult Voltar()
        {

            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupo()
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
                    if ((Int32)Session["PermMensageria"] == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                    if (usuario.PERFIL.PERF_IN_ACESSO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Grupos";

                // Carrega listas
                listaMaster = CarregaGrupo();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaGrupo"] = listaMaster;
                ViewBag.Listas = (List<GRUPO_PAC>)Session["ListaGrupo"];
                Session["Grupo"] = null;
                Session["IncluirGrupo"] = 0;
                Session["ListaClienteGrupo"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/16/Ajuda16.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Trata mensagens
                if (Session["MensGrupo"] != null)
                {
                    if ((Int32)Session["MensGrupo"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0062", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 11)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 12)
                    {
                        String frase = "Foram incluídos um total de " + (String)Session["TotalGrupo"] + " pacientes após o processamento do grupo";
                        TempData["MensagemAcerto"] = frase;
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensGrupo"] == 22)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0266", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0433", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0434", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 112)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0435", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "GRUPO", "Grupo", "MontarTelaGrupo");

                // Abre view
                Session["VoltaCliGrupo"] = 0;
                Session["MensGrupo"] = null;
                Session["LinhaAlterada"] = 0;
                Session["VoltaGrupo"] = 1;
                objeto = new GRUPO_PAC();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroGrupo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaGrupo"] = null;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoGrupo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMaster = baseApp.GetAllItensAdm(idAss);
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaGrupo"] = listaMaster;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaGrupo"] == 99)
            {
                return RedirectToAction("IncluirMensagemEMail", "Mensagem");
            }
            if ((Int32)Session["VoltaGrupo"] == 11)
            {
                return RedirectToAction("VoltarAnexoPaciente", "Paciente");
            }
            if ((Int32)Session["VoltaGrupo"] == 7)
            {
                return RedirectToAction("IncluirMensagemEMail", "Mensagem");
            }
            if ((Int32)Session["VoltaGrupo"] == 22)
            {
                return RedirectToAction("VoltarAnexoPaciente", "Paciente");
            }
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Inclusão";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["ModuloAtual"] = "Grupos - Inclusão";

                // Verifica possibilidade
                Int32 num = CarregaGrupo().Count;
                if ((Int32)Session["NumGrupos"] <= num)
                {
                    Session["MensGrupo"] = 50;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }

                // Prepara listas
                List<PACIENTE> lista = CarregaPaciente();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                List<String> cids = lista.Where(m => m.PACI_NM_CIDADE != null).Select(p => p.PACI_NM_CIDADE).Distinct().ToList();
                ViewBag.Clientes = new SelectList(lista.OrderBy(p => p.PACI_NM_NOME), "PACI_CD_ID", "PACI_NM_NOME");
                ViewBag.Cats = new SelectList(CarregaTipoPaciente().OrderBy(p => p.TIPA_NM_NOME), "TIPA_CD_ID", "TIPA_NM_NOME");
                ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
                ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
                ViewBag.Cidades = new SelectList(cids);

                List<SelectListItem> dias = new List<SelectListItem>();
                for (int i = 1; i < 32; i++)
                {
                    dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }
                ViewBag.Dias = new SelectList(dias, "Value", "Text");
                List<SelectListItem> meses = new List<SelectListItem>();
                meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
                meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
                meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
                meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
                meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
                meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
                meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
                meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
                meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
                meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
                meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
                meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
                ViewBag.Meses = new SelectList(meses, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/16/Ajuda16_1.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "GRUPO_INCLUIR", "Grupo", "IncluirGrupo");

                // Prepara view
                Session["GrupoNovo"] = 0;
                GRUPO_PAC item = new GRUPO_PAC();
                GrupoViewModel vm = Mapper.Map<GRUPO_PAC, GrupoViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.GRUP_DT_CADASTRO = DateTime.Today.Date;
                vm.GRUP_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<PACIENTE> lista = CarregaPaciente();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            List<String> cids = lista.Where(m => m.PACI_NM_CIDADE != null).Select(p => p.PACI_NM_CIDADE).Distinct().ToList();
            ViewBag.Clientes = new SelectList(lista.OrderBy(p => p.PACI_NM_NOME), "PACI_CD_ID", "PACI_NM_NOME");
            ViewBag.Cats = new SelectList(CarregaTipoPaciente().OrderBy(p => p.TIPA_NM_NOME), "TIPA_CD_ID", "TIPA_NM_NOME");
            ViewBag.UF = new SelectList(CarregaUF().OrderBy(p => p.UF_SG_SIGLA), "UF_CD_ID", "UF_NM_NOME");
            ViewBag.Sexo = new SelectList(CarregaSexo().OrderBy(p => p.SEXO_NM_NOME), "SEXO_CD_ID", "SEXO_NM_NOME");
            ViewBag.Cidades = new SelectList(cids);

            List<SelectListItem> dias = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                dias.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.Dias = new SelectList(dias, "Value", "Text");
            List<SelectListItem> meses = new List<SelectListItem>();
            meses.Add(new SelectListItem() { Text = "Janeiro", Value = "1" });
            meses.Add(new SelectListItem() { Text = "Fevereiro", Value = "2" });
            meses.Add(new SelectListItem() { Text = "Março", Value = "3" });
            meses.Add(new SelectListItem() { Text = "Abril", Value = "4" });
            meses.Add(new SelectListItem() { Text = "Maio", Value = "5" });
            meses.Add(new SelectListItem() { Text = "Junho", Value = "6" });
            meses.Add(new SelectListItem() { Text = "Julho", Value = "7" });
            meses.Add(new SelectListItem() { Text = "Agosto", Value = "8" });
            meses.Add(new SelectListItem() { Text = "Setembro", Value = "9" });
            meses.Add(new SelectListItem() { Text = "Outubro", Value = "10" });
            meses.Add(new SelectListItem() { Text = "Novembro", Value = "11" });
            meses.Add(new SelectListItem() { Text = "Dezembro", Value = "12" });
            ViewBag.Meses = new SelectList(meses, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.GRUP_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.GRUP_NM_CIDADE);
                    vm.GRUP_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.GRUP_NM_NOME);

                    // Crítica
                    if (vm.SEXO_CD_ID == null & vm.TIPA_CD_ID == null & vm.GRUP_NM_CIDADE == null & vm.UF_CD_ID == null & vm.GRUP_DT_NASCIMENTO == null & vm.GRUP_NR_DIA == null & vm.GRUP_NR_MES == null & vm.GRUP_NR_ANO == null)
                    {
                        Session["MensGrupo"] = 10;
                        return RedirectToAction("MontarTelaGrupo");
                    }
                    if (vm.USUA_CD_ID == null)
                    {
                        vm.USUA_CD_ID = usuario.USUA_CD_ID;
                    }

                    // Executa a operação
                    GRUPO_PAC item = Mapper.Map<GrupoViewModel, GRUPO_PAC>(vm);
                    MontagemGrupoPaciente grupo = new MontagemGrupoPaciente();
                    Int32 volta = baseApp.ValidateCreate(item, grupo, usuario);

                    // Validações
                    Session["MensGrupo"] = null;
                    if (volta == 3456)
                    {
                        Session["MensGrupo"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0025", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Atualiza cache
                    listaMaster = new List<GRUPO_PAC>();
                    Session["ListaGrupo"] = null;
                    Session["IncluirGrupo"] = 1;
                    Session["GrupoNovo"] = item.GRUP_CD_ID;
                    Session["IdGrupo"] = item.GRUP_CD_ID;
                    Session["GrupoAlterada"] = 1;
                    Session["TotalGrupo"] = volta.ToString();
                    Session["LinhaAlterada"] = item.GRUP_CD_ID;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi criado com sucesso. Foram incluídos " + volta.ToString() + " pacientes.";
                    Session["MensGrupo"] = 61;

                    // Grupo vazio
                    if (volta == 0)
                    {
                        Session["MensGrupo"] = 112;
                        return RedirectToAction("VoltarBaseGrupo", "Grupo");
                    }

                    // Trata retorno
                    if ((Int32)Session["VoltaGrupo"] == 11)
                    {
                        return RedirectToAction("VoltarAnexoPaciente", "Paciente");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 99)
                    {
                        return RedirectToAction("IncluirMensagemEMail", "Mensagem");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarBaseGrupo");
                    }
                    if ((Int32)Session["VoltaGrupo"] == 7)
                    {
                        return RedirectToAction("IncluirMensagemEMail", "Mensagem");
                    }
                    return RedirectToAction("VoltarBaseGrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupos";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult RemontarGrupo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                GRUPO_PAC item = baseApp.GetItemById(id);
                Int32 itens = item.GRUPO_PACIENTE.Count();

                MontagemGrupoPaciente grupo = new MontagemGrupoPaciente();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = baseApp.ValidateRemontar(item, grupo, usuario);

                // Validações
                if (volta == 2)
                {
                    Session["MensGrupo"] = 11;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0063", CultureInfo.CurrentCulture));
                    return RedirectToAction("VoltarBaseGrupo");
                }

                // Atualiza cache
                listaMaster = new List<GRUPO_PAC>();
                Session["ListaGrupo"] = null;
                Session["IncluirGrupo"] = 1;
                Session["GrupoNovo"] = item.GRUP_CD_ID;
                Session["IdGrupo"] = item.GRUP_CD_ID;
                Session["GrupoAlterada"] = 1;

                // Mensagem do CRUD
                if (volta == itens)
                {
                    Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi remontado com sucesso. Foram incluídos " + volta.ToString() + " pacientes.";
                    Session["MensGrupo"] = 61;
                }
                else
                {
                    Int32 dif = itens - volta;
                    Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi remontado com sucesso. Foram incluídos " + volta.ToString() + " pacientes. Foram abandonados " + dif.ToString() + " pacientes.";
                    Session["MensGrupo"] = 61;
                }

                // Trata retorno
                if ((Int32)Session["VoltaGrupo"] == 2)
                {
                    return RedirectToAction("VoltarAnexoGrupo");
                }
                return RedirectToAction("VoltarBaseGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarGrupo(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Edição";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Grupos - Edição";

                // Mensagens
                if (Session["MensGrupo"] != null)
                {
                    if ((Int32)Session["MensGrupo"] == 112)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0435", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Recupera grupo
                GRUPO_PAC item = baseApp.GetItemById(id);
                Session["Grupo"] = item;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/16/Ajuda16_2.pdf";

                // Indicadores
                ViewBag.Incluir = (Int32)Session["IncluirGrupo"];

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "GRUPO_EDITAR", "Grupo", "EditarGrupo");

                // Monta view
                Session["VoltaGrupo"] = 2;
                Session["GrupoAntes"] = item;
                Session["IdGrupo"] = id;
                GrupoViewModel vm = Mapper.Map<GRUPO_PAC, GrupoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitizar
                    vm.GRUP_NM_CIDADE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.GRUP_NM_CIDADE);
                    vm.GRUP_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.GRUP_NM_NOME);

                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    GRUPO_PAC item = Mapper.Map<GrupoViewModel, GRUPO_PAC>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, (GRUPO_PAC)Session["GrupoAntes"], usuario);

                    // Verifica retorno

                    // Atualiza cache
                    listaMaster = new List<GRUPO_PAC>();
                    Session["ListaGrupo"] = null;
                    Session["GrupoAlterada"] = 1;
                    Session["LinhaAlterada"] = item.GRUP_CD_ID;
                    Session["FlagAlteraEstado"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensGrupo"] = 61;

                    // Trata retorno
                    if ((Int32)Session["VoltaGrupo"] == 10)
                    {
                        return RedirectToAction("VoltarAnexoPaciente", "Paciente");
                    }
                    if ((Int32)Session["VoltaCliGrupo"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoPaciente", "Paciente");
                    }
                    return RedirectToAction("MontarTelaGrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupos";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerGrupo(Int32 id)
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

                // Recupera grupo
                GRUPO_PAC item = baseApp.GetItemById(id);
                Session["Grupo"] = item;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "GRUPO_VER", "Grupo", "VerGrupo");

                // Indicadores
                objetoAntes = item;
                Session["IdGrupo"] = id;
                GrupoViewModel vm = Mapper.Map<GRUPO_PAC, GrupoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EditarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        public ActionResult RemontarGrupoForn()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("RemontarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirGrupo(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Exclusão";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                GRUPO_PAC item = baseApp.GetItemById(id);
                objetoAntes = (GRUPO_PAC)Session["Grupo"];
                item.GRUP_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                if (volta > 0)
                {
                    Session["MensGrupo"] = 22;
                    return RedirectToAction("MontarTelaGrupo");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensGrupo"] = 61;


                Session["ListaGrupo"] = null;
                Session["GrupoAlterada"] = 1;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVAR_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Reativação";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica possibilidade
                Int32 num = CarregaGrupo().Count;
                if ((Int32)Session["NumGrupos"] <= num)
                {
                    Session["MensGrupo"] = 50;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }

                GRUPO_PAC item = baseApp.GetItemById(id);
                objetoAntes = (GRUPO_PAC)Session["Grupo"];
                item.GRUP_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O grupo " + item.GRUP_NM_NOME.ToUpper() + " foi reativado com sucesso.";
                Session["MensGrupo"] = 61;

                Session["ListaGrupo"] = null;
                Session["GrupoAlterada"] = 1;
                return RedirectToAction("MontarTelaGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult IncluirContatoGrupo()
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Edição";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Grupos - Inclusão - Contato";

                // Verifica possibilidade
                GRUPO_PAC grupo = baseApp.GetItemById((Int32)Session["IdGrupo"]);
                Int32 num = grupo.GRUPO_PACIENTE.Count;
                if ((Int32)Session["NumItemGrupos"] <= num)
                {
                    Session["MensGrupo"] = 51;
                    return RedirectToAction("MontarTelaGrupo", "Grupo");
                }

                // Mensagens
                if (Session["MensGrupo"] != null)
                {
                    if ((Int32)Session["MensGrupo"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0027", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 14)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0268", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensGrupo"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }
                Session["MensGrupo"] = 0;

                // Prepara view
                List<PACIENTE> lista = null;
                List<PACIENTE> listaNova = new List<PACIENTE>();
                lista = CarregaPaciente();
                lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                foreach (PACIENTE cli in lista)
                    {
                    if (cli.GRUPO_PACIENTE.Count > 0)
                    {
                        Int32 flag = 0;
                        foreach (GRUPO_PACIENTE gru in cli.GRUPO_PACIENTE)
                        {
                            if (gru.GRUP_CD_ID == (Int32)Session["IdGrupo"])
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 0)
                        {
                            listaNova.Add(cli);
                        }
                    }
                    else
                    {
                        listaNova.Add(cli);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "GRUPO_INCLUIR_CONTATO", "Grupo", "IncluirContatoGrupo");

                ViewBag.Lista = new SelectList(listaNova.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");
                GRUPO_PACIENTE item = new GRUPO_PACIENTE();
                GrupoContatoViewModel vm = Mapper.Map<GRUPO_PACIENTE, GrupoContatoViewModel>(item);
                vm.GRCL_IN_ATIVO = 1;
                vm.GRUP_CD_ID = (Int32)Session["IdGrupo"];
                listaMaster = new List<GRUPO_PAC>();
                Session["ListaGrupo"] = null;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirContatoGrupo(GrupoContatoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<PACIENTE> lista = null;
            List<PACIENTE> listaNova = new List<PACIENTE>();
            lista = CarregaPaciente();
            lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            foreach (PACIENTE cli in lista)
            {
                if (cli.GRUPO_PACIENTE.Count > 0)
                {
                    Int32 flag = 0;
                    foreach (GRUPO_PACIENTE gru in cli.GRUPO_PACIENTE)
                    {
                        if (gru.GRUP_CD_ID == (Int32)Session["IdGrupo"])
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        listaNova.Add(cli);
                    }
                }
                else
                {
                    listaNova.Add(cli);
                }
            }
            ViewBag.Lista = new SelectList(listaNova.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Critica
                    if (vm.PACI_CD_ID == null || vm.PACI_CD_ID == 0)
                    {
                        Session["MensGrupo"] = 14;
                        return RedirectToAction("IncluirContatoGrupo");
                    }

                    // Executa a operação
                    PACIENTE pac = cliApp.GetItemById(vm.PACI_CD_ID.Value);
                    GRUPO_PAC gru = baseApp.GetItemById(vm.GRUP_CD_ID.Value);
                    GRUPO_PACIENTE item = Mapper.Map<GrupoContatoViewModel, GRUPO_PACIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreateContato(item);
                    
                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 4;
                        return RedirectToAction("IncluirContatoGrupo");
                    }

                    // Configura serilização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_GrupoPaciente dto = MontarGrupoPacienteDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "Grupo de Pacientes - Inclusão de Paciente",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O paciente " + pac.PACI_NM_NOME.ToUpper() + " foi Incluído com sucesso no grupo " + gru.GRUP_NM_NOME.ToUpper();
                    Session["MensGrupo"] = 61;

                    // Verifica retorno
                    Session["GrupoAlterada"] = 1;
                    Session["PacienteAlterada"] = 1;
                    return RedirectToAction("IncluirContatoGrupo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Grupos";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public DTO_GrupoPaciente MontarGrupoPacienteDTOObj(GRUPO_PACIENTE antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_GrupoPaciente()
                {
                    GRCL_IN_ATIVO = antes.GRCL_IN_ATIVO,
                    PACI_CD_ID = antes.PACI_CD_ID,
                    GRUP_CD_ID = antes.GRUP_CD_ID,
                    GRCL_CD_ID = antes.GRCL_CD_ID,
                };
                return mediDTO;
            }
        }

        [HttpGet]
        public ActionResult ExcluirContatoGrupo(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Edição";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                GRUPO_PACIENTE item = baseApp.GetContatoById(id);
                PACIENTE pac = cliApp.GetItemById(item.PACI_CD_ID.Value);
                GRUPO_PAC gru = baseApp.GetItemById(item.GRUP_CD_ID.Value);
                item.GRCL_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditContato(item);

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_GrupoPaciente dto = MontarGrupoPacienteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Grupo de Pacientes - Exclusão de Paciente",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O paciente " + pac.PACI_NM_NOME.ToUpper() + " foi excluído com sucesso no grupo " + gru.GRUP_NM_NOME.ToUpper();
                Session["MensGrupo"] = 61;

                return RedirectToAction("VoltarAnexoGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarContatoGrupo(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_GRUPO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Grupos - Edição";
                        return RedirectToAction("MontarTelaGrupo");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                GRUPO_PACIENTE item = baseApp.GetContatoById(id);
                PACIENTE pac = cliApp.GetItemById(item.PACI_CD_ID.Value);
                GRUPO_PAC gru = baseApp.GetItemById(item.GRUP_CD_ID.Value);
                objetoAntes = (GRUPO_PAC)Session["Grupo"];
                item.GRCL_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateEditContato(item);

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_GrupoPaciente dto = MontarGrupoPacienteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Grupo de Pacientes - Reativação de Paciente",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O paciente " + pac.PACI_NM_NOME.ToUpper() + " foi reativado com sucesso no grupo " + gru.GRUP_NM_NOME.ToUpper();
                Session["MensGrupo"] = 61;

                return RedirectToAction("VoltarAnexoGrupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Grupos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Grupos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<GRUPO_PAC> CarregaGrupo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO_PAC> conf = new List<GRUPO_PAC>();
            if (Session["Grupos"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<GRUPO_PAC>)Session["Grupos"];
                }
            }
            Session["Grupos"] = conf;
            Session["GrupoAlterada"] = 0;
            return conf;
        }

        public List<PACIENTE> CarregaPaciente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PACIENTE> conf = new List<PACIENTE>();
            if (Session["Pacientes"] == null)
            {
                conf = cliApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["PacienteAlterada"] == 1)
                {
                    conf = cliApp.GetAllItens(idAss);
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

        public List<UF> CarregaUF()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<UF> conf = new List<UF>();
            if (Session["UF"] == null)
            {
                conf = cliApp.GetAllUF();
            }
            else
            {
                conf = (List<UF>)Session["UF"];
            }
            Session["UF"] = conf;
            return conf;
        }

        public List<SEXO> CarregaSexo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SEXO> conf = new List<SEXO>();
            if (Session["Sexos"] == null)
            {
                conf = cliApp.GetAllSexo();
            }
            else
            {
                conf = (List<SEXO>)Session["Sexos"];
            }
            Session["Sexos"] = conf;
            return conf;
        }

        public List<TIPO_PACIENTE> CarregaTipoPaciente()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<TIPO_PACIENTE> conf = new List<TIPO_PACIENTE>();
            if (Session["TipoPacientes"] == null)
            {
                conf = cliApp.GetAllTipos(idAss);
            }
            else
            {
                if ((Int32)Session["TipoPacienteAlterada"] == 1)
                {
                    conf = cliApp.GetAllTipos(idAss);
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

        public List<EMPRESA> CarregaEmpresa()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<EMPRESA> conf = new List<EMPRESA>();
            if (Session["Empresas"] == null)
            {
                conf = empApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["EmpresaAlterada"] == 1)
                {
                    conf = empApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<EMPRESA>)Session["Empresas"];
                }
            }
            Session["Empresas"] = conf;
            Session["EmpresaAlterada"] = 0;
            return conf;
        }

        public JsonResult GetGrupo(Int32 id)
        {
            var grupo = baseApp.GetItemById(id);
            var quant = grupo.GRUPO_PACIENTE.Where(p => p.GRCL_IN_ATIVO == 1).ToList().Count;
            var hash = new Hashtable();
            hash.Add("quant", quant);
            return Json(hash);
        }

    }
}