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
    public class ValidacaoDocumentoController : Controller
    {
        private readonly IPacienteAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IAssinanteAppService assApp;
        private readonly IConfiguracaoCalendarioAppService calApp;
        private readonly IConfiguracaoAnamneseAppService anaApp;
        private readonly ILogAppService logApp;

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

        public ValidacaoDocumentoController(IPacienteAppService baseApps, IConfiguracaoAppService confApps, IUsuarioAppService usuApps, IAcessoMetodoAppService aceApps, IAssinanteAppService assApps, IConfiguracaoCalendarioAppService calApps, IConfiguracaoAnamneseAppService anaApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            usuApp = usuApps;
            aceApp = aceApps;
            assApp = assApps;
            calApp = calApps;
            anaApp = anaApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult MontarTelaValidacaoDocumento()
        {
            try
            {
                // Trata mensagens
                if (Session["MensPaciente"] != null)
                {
                    // Mensagem
                    if ((Int32)Session["MensValida"] == 1)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Montar Listas
                var tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Atestado", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Solicitação de Exame", Value = "2" });
                tipo.Add(new SelectListItem() { Text = "Prescrição de Medicamento", Value = "3" });
                ViewBag.TipoDocumento = new SelectList(tipo, "Value", "Text");
                ViewBag.MostraDenuncia = 0;

                // Acerta estado
                Session["MensValida"] = null;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/1/Ajuda1.pdf";

                // Carrega view
                ModeloViewModel mod = new ModeloViewModel();
                return View(mod);
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
        public ActionResult MontarTelaValidacaoDocumento(ModeloViewModel vm)
        {
            // Preparação
            var tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Atestado", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Solicitação de Exame", Value = "2" });
            tipo.Add(new SelectListItem() { Text = "Prescrição de Medicamento", Value = "3" });
            ViewBag.TipoDocumento = new SelectList(tipo, "Value", "Text");

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.Nome = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.Nome);

                    // Criticas
                    if (vm.Valor == 0)
                    {
                        Session["MensValida"] = 1;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0679", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.Nome == null)
                    {
                        Session["MensValida"] = 2;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0680", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Recupera atestado
                    if (vm.Valor == 1)
                    {
                        PACIENTE_ATESTADO atestado = baseApp.GetAllAtestadoGeral().Where(p => p.PAAT_TK_TOKEN == vm.Nome).FirstOrDefault();
                        if (atestado == null)
                        {
                            Session["MensValida"] = 3;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0681", CultureInfo.CurrentCulture));
                            ViewBag.MostraDenuncia = 1;
                            return View(vm);
                        }

                        Session["ValidaAtestado"] = atestado;
                        Session["AtestadoValidado"] = 0;
                        Session["AtestadoValido"] = 0;
                        Session["AtestadoDenuncia"] = 0;
                        Session["AtestadoDenunciado"] = 0;
                        Session["MensagemValida"] = String.Empty;
                        return RedirectToAction("MostraValidarAtestado", "ValidacaoDocumento");
                    }

                    // Recupera solicitacao
                    if (vm.Valor == 2)
                    {
                        PACIENTE_SOLICITACAO solicitacao = baseApp.GetAllSolicitacaoGeral().Where(p => p.PASO_TK_TOKEN == vm.Nome).FirstOrDefault();
                        if (solicitacao == null)
                        {
                            Session["MensValida"] = 4;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0682", CultureInfo.CurrentCulture));
                            ViewBag.MostraDenuncia = 1;
                            return View(vm);
                        }
                        Session["ValidaSolicitacao"] = solicitacao;
                        Session["SolicitacaoValidado"] = 0;
                        Session["SolicitacaoValido"] = 0;
                        Session["SolicitacaoDenuncia"] = 0;
                        Session["SolicitacaoDenunciado"] = 0;
                        Session["MensagemValida"] = String.Empty;
                        return RedirectToAction("MostraValidarSolicitacao", "ValidacaoDocumento");
                    }

                    // Recupera prescricao
                    if (vm.Valor == 3)
                    {
                        PACIENTE_PRESCRICAO prescricao = baseApp.GetAllPrescricaoGeral().Where(p => p.PAPR_TK_TOKEN == vm.Nome).FirstOrDefault();
                        if (prescricao == null)
                        {
                            Session["MensValida"] = 5;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0683", CultureInfo.CurrentCulture));
                            ViewBag.MostraDenuncia = 1;
                            return View(vm);
                        }
                        Session["ValidaPrescricao"] = prescricao;
                        return RedirectToAction("MostraValidarPrescricao", "ValidacaoDocumento");
                    }

                    // Retorno
                    return RedirectToAction("MontarTelaValidacaoDocumento");
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

        [HttpGet]
        public ActionResult MostraValidarAtestado()
        {
            try
            {
                // Recupera atestado
                PACIENTE_ATESTADO entra = (PACIENTE_ATESTADO)Session["ValidaAtestado"];
                PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById(entra.PAAT_CD_ID);

                // Assinatura
                String frase = String.Empty;
                if (atestado.PAAT_IN_ASSINADO_DIGITAL == 1)
                {
                    frase = "Atestado assinado digitalmente por " + atestado.USUARIO.USUA_NM_NOME + " em " + atestado.PAAT_DT_EMISSAO_COMPLETA.Value.ToShortDateString() + " " + atestado.PAAT_DT_EMISSAO_COMPLETA.Value.ToShortTimeString() + " conforme MP 2.200-2/2001.";
                }
                ViewBag.MensagemAssina = frase;

                // Mensagem
                if (Session["MensValida"] != null)
                {
                    if ((Int32)Session["MensValida"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MensagemValida"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Carrega view
                if (atestado.PAAT_IN_VALIDACAO == 1)
                {
                    ViewBag.MensagemValida = "O atestado abaixo foi validado em " + atestado.PAAT_DT_VALIDACAO.Value.ToLongDateString() + ". Veja detalhes abaixo.";
                }
                if (atestado.PAAT_IN_DENUNCIA == 1)
                {
                    ViewBag.MensagemDenuncia = "O atestado abaixo foi denunciado em " + atestado.PAAT_DT_DENUNCIA.Value.ToLongDateString() + ". Veja detalhes abaixo."; 
                }
                PacienteAtestadoViewModel vm = Mapper.Map<PACIENTE_ATESTADO, PacienteAtestadoViewModel>(atestado);
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

        [HttpGet]
        public ActionResult ConfirmarValidacaoAtestado()
        {
            try
            {
                // Checa se já foi denunciado
                PACIENTE_ATESTADO entra = (PACIENTE_ATESTADO)Session["ValidaAtestado"];
                PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById(entra.PAAT_CD_ID);
                if (atestado.PAAT_IN_DENUNCIA == 1)
                {
                    Session["MensagemDenunciado"] = "O atestado abaixo foi denunciado anteriormente em " + atestado.PAAT_DT_DENUNCIA.Value.ToLongDateString() + " e não pode ser validado";
                    Session["AtestadoDenunciado"] = 1;
                    return RedirectToAction("MostraValidarAtestado");
                }

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa validação
                atestado.PAAT_DT_VALIDACAO = DateTime.Now;
                atestado.PAAT_IP_VALIDACAO = ip;
                atestado.PAAT_IN_VALIDACAO = 1;
                atestado.PAAT_IN_DENUNCIA = 0;
                atestado.PAAT_DT_DENUNCIA = null;
                atestado.PAAT_IP_DENUNCIA = null;
                atestado.PAAT_NR_DENUNCIA = 0;
                atestado.PAAT_TX_DENUNCIA = null;
                if (atestado.PAAT_NR_VALIDACAO == null)
                {
                    atestado.PAAT_NR_VALIDACAO = 1;
                }
                else
                {
                    atestado.PAAT_NR_VALIDACAO = atestado.PAAT_NR_VALIDACAO + 1;
                }
                Int32 volta = baseApp.ValidateEditAtestado(atestado);

                // Acerta estado
                Session["MensagemValida"] = "O atestado abaixo foi validado em " + DateTime.Now.ToString();
                Session["MensValida"] = 61;
                Session["AtestadoValidado"] = 1;
                Session["AtestadoValido"] = 1;

                // Retorno
                return RedirectToAction("MostraValidarAtestado");
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
        public Int32 ProcessaDenunciaAtestado(String texto)
        {
            try
            {
                // Recupera motivo
                String motivo = (String)Session["MotivoDenuncia"];

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa denuncia
                PACIENTE_ATESTADO entra = (PACIENTE_ATESTADO)Session["ValidaAtestado"];
                PACIENTE_ATESTADO atestado = baseApp.GetAtestadoById(entra.PAAT_CD_ID);
                atestado.PAAT_DT_VALIDACAO = null;
                atestado.PAAT_IP_VALIDACAO = null;
                atestado.PAAT_IN_VALIDACAO = 0;
                atestado.PAAT_NR_VALIDACAO = 0;
                atestado.PAAT_DT_DENUNCIA = DateTime.Now;
                atestado.PAAT_IP_DENUNCIA = ip;
                atestado.PAAT_IN_DENUNCIA = 1;
                if (atestado.PAAT_NR_DENUNCIA == null)
                {
                    atestado.PAAT_NR_DENUNCIA = 1;
                }
                else
                {
                    atestado.PAAT_NR_DENUNCIA = atestado.PAAT_NR_DENUNCIA + 1;
                }
                atestado.PAAT_TX_DENUNCIA = motivo;
                Int32 volta = baseApp.ValidateEditAtestado(atestado);

                // Acerta estado
                Session["MensagemValida"] = null;
                Session["AtestadoValidado"] = 0;
                Session["AtestadoValido"] = 0;
                Session["MensagemDenuncia"] = "O atestado abaixo foi denunciado em " + DateTime.Now.ToString();
                Session["AtestadoDenunciado"] = 0;
                Session["AtestadoDenuncia"] = 1;
                Session["TemValidaAntes"] = 0;

                // Retorno
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost] 
        public JsonResult DenunciarAtestado(ModalTextoViewModel viewModel)
        {
            // Aqui você pode acessar o texto enviado pelo usuário
            String textoRecebido = viewModel.Texto;

            // Processar denuncia
            try
            {
                // Critica
                if (string.IsNullOrEmpty(textoRecebido))
                {
                    return Json(new { success = false, message = "O motivo da denuncia não pode ser vazio." });
                }
                Session["MotivoDenuncia"] = textoRecebido;

                // Processa
                Int32 volta = ProcessaDenunciaAtestado(textoRecebido);

                // retorno
                String mensagem = "Denuncia processada com sucesso em " + DateTime.Today.Date.ToLongDateString();
                return Json(new { success = true, mensagem});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocorreu um erro interno ao processar o texto." });
            }
        }

        [HttpGet]
        public ActionResult MostraValidarSolicitacao()
        {
            try
            {
                // Recupera atestado
                PACIENTE_SOLICITACAO entra = (PACIENTE_SOLICITACAO)Session["ValidaSolicitacao"];
                PACIENTE_SOLICITACAO atestado = baseApp.GetSolicitacaoById(entra.PASO_CD_ID);

                // Assinatura
                String frase = String.Empty;
                if (atestado.PASO_IN_ASSINADO_DIGITAL == 1)
                {
                    frase = "Solicitação de exame assinada digitalmente por " + atestado.USUARIO.USUA_NM_NOME + " em " + atestado.PASO_DT_EMISSAO_COMPLETA.Value.ToShortDateString() + " " + atestado.PASO_DT_EMISSAO_COMPLETA.Value.ToShortTimeString() + " conforme MP 2.200-2/2001.";
                }
                ViewBag.MensagemAssina = frase;

                // Mensagem
                if (Session["MensValida"] != null)
                {
                    if ((Int32)Session["MensValida"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MensagemValida"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Carrega view
                if (atestado.PASO_IN_VALIDACAO == 1)
                {
                    ViewBag.MensagemValida = "A solicitação de exame abaixo foi validada em " + atestado.PASO_DT_VALIDACAO.Value.ToLongDateString() + ". Veja detalhes abaixo.";
                }
                if (atestado.PASO_IN_DENUNCIA == 1)
                {
                    ViewBag.MensagemDenuncia = "A solicitação de exame abaixo foi denunciada em " + atestado.PASO_DT_DENUNCIA.Value.ToLongDateString() + ". Veja detalhes abaixo.";
                }
                PacienteSolicitacaoViewModel vm = Mapper.Map<PACIENTE_SOLICITACAO, PacienteSolicitacaoViewModel>(atestado);
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

        [HttpGet]
        public ActionResult ConfirmarValidacaoSolicitacao()
        {
            try
            {
                // Checa se já foi denunciado
                PACIENTE_SOLICITACAO entra = (PACIENTE_SOLICITACAO)Session["ValidaSolicitacao"];
                PACIENTE_SOLICITACAO atestado = baseApp.GetSolicitacaoById(entra.PASO_CD_ID);
                if (atestado.PASO_IN_DENUNCIA == 1)
                {
                    Session["MensagemDenunciado"] = "A solicitação de exame abaixo foi denunciado anteriormente em " + atestado.PASO_DT_DENUNCIA.Value.ToLongDateString() + " e não pode ser validado";
                    Session["SolicitacaoDenunciado"] = 1;
                    return RedirectToAction("MostraValidarSolicitacao");
                }

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa validação
                atestado.PASO_DT_VALIDACAO = DateTime.Now;
                atestado.PASO_IP_VALIDACAO = ip;
                atestado.PASO_IN_VALIDACAO = 1;
                atestado.PASO_IN_DENUNCIA = 0;
                atestado.PASO_DT_DENUNCIA = null;
                atestado.PASO_IP_DENUNCIA = null;
                atestado.PASO_NR_DENUNCIA = 0;
                atestado.PASO_TX_DENUNCIA = null;
                if (atestado.PASO_NR_VALIDACAO == null)
                {
                    atestado.PASO_NR_VALIDACAO = 1;
                }
                else
                {
                    atestado.PASO_NR_VALIDACAO = atestado.PASO_NR_VALIDACAO + 1;
                }
                Int32 volta = baseApp.ValidateEditSolicitacao(atestado);

                // Acerta estado
                Session["MensagemValida"] = "A solicitação de exame abaixo foi validada em " + DateTime.Now.ToString();
                Session["MensValida"] = 61;
                Session["SolicitacaoValidado"] = 1;
                Session["SolicitacaoValido"] = 1;

                // Retorno
                return RedirectToAction("MostraValidarSolicitacao");
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
        public Int32 ProcessaDenunciaSolicitacao(String texto)
        {
            try
            {
                // Recupera motivo
                String motivo = (String)Session["MotivoDenuncia"];

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa denuncia
                PACIENTE_SOLICITACAO entra = (PACIENTE_SOLICITACAO)Session["ValidaSolicitacao"];
                PACIENTE_SOLICITACAO atestado = baseApp.GetSolicitacaoById(entra.PASO_CD_ID);
                atestado.PASO_DT_VALIDACAO = null;
                atestado.PASO_IP_VALIDACAO = null;
                atestado.PASO_IN_VALIDACAO = 0;
                atestado.PASO_NR_VALIDACAO = 0;
                atestado.PASO_DT_DENUNCIA = DateTime.Now;
                atestado.PASO_IP_DENUNCIA = ip;
                atestado.PASO_IN_DENUNCIA = 1;
                if (atestado.PASO_NR_DENUNCIA == null)
                {
                    atestado.PASO_NR_DENUNCIA = 1;
                }
                else
                {
                    atestado.PASO_NR_DENUNCIA = atestado.PASO_NR_DENUNCIA + 1;
                }
                atestado.PASO_TX_DENUNCIA = motivo;
                Int32 volta = baseApp.ValidateEditSolicitacao(atestado);

                // Acerta estado
                Session["MensagemValida"] = null;
                Session["SolicitacaoValidado"] = 0;
                Session["SolicitacaoValido"] = 0;
                Session["MensagemDenuncia"] = "A solicitação de exame abaixo foi denunciada em " + DateTime.Now.ToString();
                Session["SolicitacaoDenunciado"] = 0;
                Session["SolicitacaoDenuncia"] = 1;
                Session["TemValidaAntes"] = 0;

                // Retorno
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost] 
        public JsonResult DenunciarSolicitacao(ModalTextoViewModel viewModel)
        {
            // Aqui você pode acessar o texto enviado pelo usuário
            String textoRecebido = viewModel.Texto;

            // Processar denuncia
            try
            {
                // Critica
                if (string.IsNullOrEmpty(textoRecebido))
                {
                    return Json(new { success = false, message = "O motivo da denuncia não pode ser vazio." });
                }
                Session["MotivoDenuncia"] = textoRecebido;

                // Processa
                Int32 volta = ProcessaDenunciaSolicitacao(textoRecebido);

                // retorno
                String mensagem = "Denuncia processada com sucesso em " + DateTime.Today.Date.ToLongDateString();
                return Json(new { success = true, mensagem});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocorreu um erro interno ao processar o texto." });
            }
        }

        [HttpGet]
        public ActionResult MostraValidarPrescricao()
        {
            try
            {
                // Recupera prescricao
                PACIENTE_PRESCRICAO entra = (PACIENTE_PRESCRICAO)Session["ValidaPrescricao"];
                PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(entra.PAPR_CD_ID);
                ViewBag.Itens = prescricao.PACIENTE_PRESCRICAO_ITEM.ToList();

                // Assinatura
                String frase = String.Empty;
                if (prescricao.PAPR_IN_ASSINADO_DIGITAL == 1)
                {
                    frase = "Prescrição assinada digitalmente por " + prescricao.USUARIO.USUA_NM_NOME + " em " + prescricao.PAPR_DT_EMISSAO_COMPLETA.Value.ToShortDateString() + " " + prescricao.PAPR_DT_EMISSAO_COMPLETA.Value.ToShortTimeString() + " conforme MP 2.200-2/2001.";
                }
                ViewBag.MensagemAssina = frase;

                // Mensagem
                if (Session["MensValida"] != null)
                {
                    if ((Int32)Session["MensValida"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MensagemValida"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Carrega view
                if (prescricao.PAPR_IN_VALIDACAO == 1)
                {
                    ViewBag.MensagemValida = "A prescrição abaixo foi validada em " + prescricao.PAPR_DT_VALIDACAO.Value.ToLongDateString() + ". Veja detalhes abaixo.";
                }
                if (prescricao.PAPR_IN_DENUNCIA == 1)
                {
                    ViewBag.MensagemDenuncia = "A prescrição abaixo foi denunciada em " + prescricao.PAPR_DT_DENUNCIA.Value.ToLongDateString() + ". Veja detalhes abaixo.";
                }
                PacientePrescricaoViewModel vm = Mapper.Map<PACIENTE_PRESCRICAO, PacientePrescricaoViewModel>(prescricao);
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

        [HttpGet]
        public ActionResult ConfirmarValidacaoPrescricao()
        {
            try
            {
                // Checa se já foi denunciado
                PACIENTE_PRESCRICAO entra = (PACIENTE_PRESCRICAO)Session["ValidaPrescricao"];
                PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(entra.PAPR_CD_ID);
                if (prescricao.PAPR_IN_DENUNCIA == 1)
                {
                    Session["MensagemDenunciado"] = "A prescrição abaixo foi denunciada anteriormente em " + prescricao.PAPR_DT_DENUNCIA.Value.ToLongDateString() + " e não pode ser validada";
                    Session["PrescricaoDenunciado"] = 1;
                    return RedirectToAction("MostraValidarPrescricao");
                }

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa validação
                prescricao.PAPR_DT_VALIDACAO = DateTime.Now;
                prescricao.PAPR_IP_VALIDACAO = ip;
                prescricao.PAPR_IN_VALIDACAO = 1;
                prescricao.PAPR_IN_DENUNCIA = 0;
                prescricao.PAPR_DT_DENUNCIA = null;
                prescricao.PAPR_IP_DENUNCIA = null;
                prescricao.PAPR_NR_DENUNCIA = 0;
                prescricao.PAPR_TX_DENUNCIA = null;
                if (prescricao.PAPR_NR_VALIDACAO == null)
                {
                    prescricao.PAPR_NR_VALIDACAO = 1;
                }
                else
                {
                    prescricao.PAPR_NR_VALIDACAO = prescricao.PAPR_NR_VALIDACAO + 1;
                }
                Int32 volta = baseApp.ValidateEditPrescricao(prescricao);

                // Acerta estado
                Session["MensagemValida"] = "A prescrição abaixo foi validada em " + DateTime.Now.ToString();
                Session["MensValida"] = 61;
                Session["PrescricaoValidado"] = 1;
                Session["PrescricaoValido"] = 1;

                // Retorno
                return RedirectToAction("MostraValidarPrescricao");
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
        public JsonResult DenunciarPrescricao(ModalTextoViewModel viewModel)
        {
            // Aqui você pode acessar o texto enviado pelo usuário
            String textoRecebido = viewModel.Texto;

            // Processar denuncia
            try
            {
                // Critica
                if (string.IsNullOrEmpty(textoRecebido))
                {
                    return Json(new { success = false, message = "O motivo da denuncia não pode ser vazio." });
                }
                Session["MotivoDenuncia"] = textoRecebido;

                // Processa
                Int32 volta = ProcessaDenunciaPrescricao(textoRecebido);

                // retorno
                String mensagem = "Denuncia processada com sucesso em " + DateTime.Today.Date.ToLongDateString();
                return Json(new { success = true, mensagem });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocorreu um erro interno ao processar o texto." });
            }
        }

        [HttpGet]
        public Int32 ProcessaDenunciaPrescricao(String texto)
        {
            try
            {
                // Recupera motivo
                String motivo = (String)Session["MotivoDenuncia"];

                // Recupera IP
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }

                // Processa denuncia
                PACIENTE_PRESCRICAO entra = (PACIENTE_PRESCRICAO)Session["ValidaPrescricao"];
                PACIENTE_PRESCRICAO prescricao = baseApp.GetPrescricaoById(entra.PAPR_CD_ID);
                prescricao.PAPR_DT_VALIDACAO = null;
                prescricao.PAPR_IP_VALIDACAO = null;
                prescricao.PAPR_IN_VALIDACAO = 0;
                prescricao.PAPR_NR_VALIDACAO = 0;
                prescricao.PAPR_DT_DENUNCIA = DateTime.Now;
                prescricao.PAPR_IP_DENUNCIA = ip;
                prescricao.PAPR_IN_DENUNCIA = 1;
                if (prescricao.PAPR_NR_DENUNCIA == null)
                {
                    prescricao.PAPR_NR_DENUNCIA = 1;
                }
                else
                {
                    prescricao.PAPR_NR_DENUNCIA = prescricao.PAPR_NR_DENUNCIA + 1;
                }
                prescricao.PAPR_TX_DENUNCIA = motivo;
                Int32 volta = baseApp.ValidateEditPrescricao(prescricao);

                // Acerta estado
                Session["MensagemValida"] = null;
                Session["PrescricaoValidado"] = 0;
                Session["PrescricaoValido"] = 0;
                Session["MensagemDenuncia"] = "A prescrição abaixo foi denunciada em " + DateTime.Now.ToString();
                Session["PrescricaoDenunciado"] = 0;
                Session["PrescricaoDenuncia"] = 1;
                Session["TemValidaAntes"] = 0;

                // Retorno
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    public class ModalTextoViewModel
    {
        public string Texto { get; set; }
    }
}