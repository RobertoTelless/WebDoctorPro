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

namespace GEDSys_Presentation.Controllers
{
    public class MedicamentoController : Controller
    {
        private readonly IMedicamentoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IPacienteAppService pacApp;

        private String msg;
        private Exception exception;
        MEDICAMENTO objeto = new MEDICAMENTO();
        MEDICAMENTO objetoAntes = new MEDICAMENTO();
        List<MEDICAMENTO> listaMaster = new List<MEDICAMENTO>();
        String extensao;

        public MedicamentoController(IMedicamentoAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IEmpresaAppService empApps, IAcessoMetodoAppService aceApps, IPacienteAppService pacApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
            empApp = empApps;
            aceApp = aceApps;
            pacApp = pacApps;
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
            return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaMedicamento()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medicamento";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medicamentos";

                // Carrega listas
                if ((List<MEDICAMENTO>)Session["ListaMedicamentoBase"] == null)
                {
                    listaMaster = CarregarMedicamento().OrderBy(p => p.MEDI_NM_MEDICAMENTO).ToList();
                    Session["ListaMedicamentoBase"] = listaMaster;
                }
                ViewBag.Listas = (List<MEDICAMENTO>)Session["ListaMedicamentoBase"];
                Session["Medicamento"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/12/Ajuda12.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                if (Session["MensMedicamento"] != null)
                {
                    if ((Int32)Session["MensMedicamento"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedicamento"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedicamento"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedicamento"] == 99)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0595", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMedicamento"] == 61)
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
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICAMENTO", "Medicamento", "MontarTelaMedicamento");

                // Abre view
                Session["MensMedicamento"] = null;
                Session["VoltaMedicamento"] = 1;
                Session["ListaLog"] = null;
                objeto = new MEDICAMENTO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMedicamento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaMedicamentoBase"] = null;
                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoMedicamento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss).ToList();
                Session["ListaMedicamentoBase"] = listaMaster;
                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseMedicamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMedicamento");
        }

        [HttpPost]
        public ActionResult FiltrarMedicamento(MEDICAMENTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executa a operação
                List<MEDICAMENTO> listaObj = new List<MEDICAMENTO>();
                Tuple<Int32, List<MEDICAMENTO>, Boolean> volta = baseApp.ExecuteFilter(item.MEDI_NM_GENERICO, item.MEDI_NM_MEDICAMENTO, item.MEDI_NM_LABORATORIO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMedicamento"] = 1;
                    return RedirectToAction("MontarTelaMedicamento");
                }

                // Sucesso
                Session["MensMedicamento"] = null;
                listaMaster = volta.Item2;
                Session["ListaMedicamentoBase"] = volta.Item2;
                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpGet]
        public ActionResult IncluirMedicamento()
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medicamento - Inclusão";
                        return RedirectToAction("MontarTelaMedicamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medicamentos - Inclusão";

                if (Session["MensMedicamento"] != null)
                {
                    if ((Int32)Session["MensMedicamento"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                    }
                }

                // Prepara listas
                ViewBag.TipoForma = new SelectList(CarregaTipoForma(), "TIFO_CD_ID", "TIFO_NM_NOME");
                ViewBag.Controle = new SelectList(CarregaTipoControle(), "TICO_CD_ID", "TICO_NM_NOME");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/12/Ajuda12_1.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICAMENTO_INCLUIR", "Medicamento", "IncluirMedicamento");

                // Prepara view
                Session["MensMedicamento"] = null;
                MEDICAMENTO item = new MEDICAMENTO();
                MedicamentoBaseViewModel vm = Mapper.Map<MEDICAMENTO, MedicamentoBaseViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.MEDI_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.TIFO_CD_ID = null;
                vm.TICO_CD_ID = 1;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMedicamento(MedicamentoBaseViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TipoForma = new SelectList(CarregaTipoForma(), "TIFO_CD_ID", "TIFO_NM_NOME");
            ViewBag.Controle = new SelectList(CarregaTipoControle(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEDI_NM_MEDICAMENTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDI_NM_MEDICAMENTO);
                    vm.MEDI_NM_GENERICO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDI_NM_GENERICO);
                    vm.MEDI_NM_LABORATORIO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDI_NM_LABORATORIO);
                    vm.MEDI_NM_APRESENTACAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDI_NM_APRESENTACAO);
                    vm.MEDI_DS_POSOLOGIA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.MEDI_DS_POSOLOGIA);

                    // Preparação
                    MEDICAMENTO item = Mapper.Map<MedicamentoBaseViewModel, MEDICAMENTO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];

                    // Processa
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMedicamento"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0541", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Sucesso
                    listaMaster = new List<MEDICAMENTO>();
                    Session["ListaMedicamentoBase"] = null;
                    Session["IdMedicamento"] = item.MEDI_CD_ID;
                    Session["MedicamentoAlterada"] = 1;
                    Session["Medicamentos"] = null;
                    Session["MedicamentoAlterada"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O medicamento " + item.MEDI_NM_MEDICAMENTO.ToUpper() + " foi incluído com sucesso";
                    Session["MensMedicamento"] = 61;

                    if ((Int32)Session["VoltaMedicamento"] == 2)
                    {
                        return RedirectToAction("IncluirPrescricaoItem", "Paciente");
                    }
                    return RedirectToAction("MontarTelaMedicamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medicamento";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMedicamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medicamento - Edição";
                        return RedirectToAction("MontarTelaMedicamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medicamentos - Edição";

                MEDICAMENTO item = baseApp.GetItemById(id);
                Session["Medicamento"] = item;
                ViewBag.TipoForma = new SelectList(CarregaTipoForma(), "TIFO_CD_ID", "TIFO_NM_NOME");
                ViewBag.Controle = new SelectList(CarregaTipoControle(), "TICO_CD_ID", "TICO_NM_NOME");

                if (Session["MensMedicamento"] != null)
                {
                    if ((Int32)Session["MensMedicamento"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICAMENTO_EDITAR", "Medicamento", "EditarMedicamento");

                Session["MensMedicamento"] = null;
                Session["VoltaMedicamento"] = 1;
                objetoAntes = item;
                Session["IdMedicamento"] = id;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/12/Ajuda12_2.pdf";
                MedicamentoBaseViewModel vm = Mapper.Map<MEDICAMENTO, MedicamentoBaseViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarMedicamento(MedicamentoBaseViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.TipoForma = new SelectList(CarregaTipoForma(), "TIFO_CD_ID", "TIFO_NM_NOME");
            ViewBag.Controle = new SelectList(CarregaTipoControle(), "TICO_CD_ID", "TICO_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.MEDI_NM_MEDICAMENTO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MEDI_NM_MEDICAMENTO);
                    vm.MEDI_NM_GENERICO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MEDI_NM_GENERICO);
                    vm.MEDI_NM_LABORATORIO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.MEDI_NM_LABORATORIO);
                    vm.MEDI_NM_APRESENTACAO = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MEDI_NM_APRESENTACAO);
                    vm.MEDI_DS_POSOLOGIA = CrossCutting.UtilitariosGeral.CleanStringTexto(vm.MEDI_DS_POSOLOGIA);

                    // Preparação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    MEDICAMENTO item = Mapper.Map<MedicamentoBaseViewModel, MEDICAMENTO>(vm);

                    // Processa
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Sucesso
                    listaMaster = new List<MEDICAMENTO>();
                    Session["ListaMedicamentoBase"] = null;
                    Session["MedicamentoAlterada"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O medicamento " + item.MEDI_NM_MEDICAMENTO.ToUpper() + " foi alterado com sucesso";
                    Session["MensMedicamento"] = 61;

                    return RedirectToAction("VoltarAnexoMedicamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Medicamento";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }


        public ActionResult VoltarAnexoMedicamento()
        {

            return RedirectToAction("EditarMedicamento", new { id = (Int32)Session["IdMedicamento"] });
        }

        [HttpGet]
        public ActionResult ExcluirMedicamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medicamento - Exclusão";
                        return RedirectToAction("MontarTelaMedicamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera medicamento
                MEDICAMENTO item = baseApp.GetItemById(id);

                // Verifica se foi usado
                List<PACIENTE_PRESCRICAO_ITEM> itens = pacApp.GetAllPrescricaoItem(idAss).Where(p => p.PAPI_IN_ATIVO == 1).ToList();
                itens = itens.Where(p => p.PAPI_NM_REMEDIO.ToUpper() == item.MEDI_NM_MEDICAMENTO.ToUpper()).ToList();
                if (itens.Count > 0)
                {
                    Session["MensMedicamento"] = 99;
                    return RedirectToAction("MontarTelaMedicamento");
                }

                // Processa
                objetoAntes = (MEDICAMENTO)Session["Medicamento"];
                item.MEDI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                Session["ListaMedicamentoBase"] = null;
                Session["MedicamentoAlterada"] = 1;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O medicamento " + item.MEDI_NM_MEDICAMENTO.ToUpper() + " foi excluído com sucesso";
                Session["MensMedicamento"] = 61;

                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarMedicamento(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_PRESCRICAO_ACESSO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Medicamento - Exclusão";
                        return RedirectToAction("MontarTelaMedicamento");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                MEDICAMENTO item = baseApp.GetItemById(id);
                objetoAntes = (MEDICAMENTO)Session["Medicamento"];
                item.MEDI_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O medicamento " + item.MEDI_NM_MEDICAMENTO.ToUpper() + " foi reativado com sucesso";
                Session["MensMedicamento"] = 61;

                Session["ListaMedicamentoBase"] = null;
                Session["MedicamentoAlterada"] = 1;
                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMedicamento(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Medicamentos - Consulta";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MEDICAMENTO_VER", "Medicamento", "VerMedicamento");

                Session["IdMedicamento"] = id;
                MEDICAMENTO item = baseApp.GetItemById(id);
                MedicamentoBaseViewModel vm = Mapper.Map<MEDICAMENTO, MedicamentoBaseViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<MEDICAMENTO> CarregarMedicamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MEDICAMENTO> conf = new List<MEDICAMENTO>();
                if (Session["Medicamentos"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["MedicamentoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<MEDICAMENTO>)Session["Medicamentos"];
                    }
                }
                Session["MedicamentoAlterada"] = 0;
                Session["Medicamentos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult GerarListagemMedicamento()
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

                String nomeRel = "MedicamentoLista" + "_" + data + ".pdf";
                List<MEDICAMENTO> lista = (List<MEDICAMENTO>)Session["ListaMedicamentoBase"];

                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(new float[] { 20f, 700f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; 
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                Image image = null;
                if (conf.CONF_IN_LOGO_EMPRESA == 1)
                {
                    EMPRESA empresa = empApp.GetItemByAssinante(idAss);
                    image = Image.GetInstance(Server.MapPath(empresa.EMPR_AQ_LOGO));
                }
                else
                {
                    image = Image.GetInstance(Server.MapPath("~/Images/Prontuario_Icone_1.png"));
                }
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Medicamentos", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 1;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 120f, 120f, 80f, 120f, 120f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Medicamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome Genérico", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Forma de Uso", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Apresentação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Laboratório", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (MEDICAMENTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_MEDICAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_GENERICO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.TIPO_FORMA.TIFO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_APRESENTACAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_LABORATORIO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
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

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaMedicamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Medicamento";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Medicamento", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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

        public List<TIPO_FORMA> CarregaTipoForma()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_FORMA> conf = new List<TIPO_FORMA>();
                conf = baseApp.GetAllFormas();
                Session["TipoFormas"] = conf;
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

        public List<TIPO_CONTROLE> CarregaTipoControle()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_CONTROLE> conf = new List<TIPO_CONTROLE>();
                conf = pacApp.GetAllTipoControle();
                Session["TipoControles"] = conf;
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

        public ActionResult GerarRelatorioMedicamentos()
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

                String nomeRel = "MedicamentoLista" + "_" + data + ".pdf";
                List<MEDICAMENTO> lista = (List<MEDICAMENTO>)Session["ListaMedicamentoBase"];

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

                    cell1 = new PdfPCell(new Paragraph("Medicamentos Cadastrados", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Medicamentos Cadastrados", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 80f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Medicamento", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome Genérico", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Forma de Uso", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Apresentação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Laboratório", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 1;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (MEDICAMENTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_MEDICAMENTO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_GENERICO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.TIPO_FORMA.TIFO_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_APRESENTACAO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MEDI_NM_LABORATORIO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
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

                Session["NivelPaciente"] = 9;
                return RedirectToAction("MontarTelaMedicamentos");
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