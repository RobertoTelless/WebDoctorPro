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
using ERP_Condominios_Solution.Classes;
using GEDSys_Presentation.App_Start;
using EntitiesServices.WorkClasses;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IEmpresaAppService baseApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ILogAppService logApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IAssinanteAppService assiApp;
        private readonly IPacienteAppService pacApp;
        private readonly IGrupoAppService gruApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IPagamentoAppService pagApp;
        private readonly IRecebimentoAppService recApp;

        private String msg;
        private Exception exception;
        EMPRESA objeto = new EMPRESA();
        EMPRESA objetoAntes = new EMPRESA();
        List<EMPRESA> listaMaster = new List<EMPRESA>();
        String extensao;

        public EmpresaController(IEmpresaAppService baseApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, ILogAppService logApps, IMensagemEnviadaSistemaAppService meApps, IAssinanteAppService assiApps, IPacienteAppService pacApps, IGrupoAppService gruApps, IAcessoMetodoAppService aceApps, IPagamentoAppService pagApps, IRecebimentoAppService recApps)
        {
            baseApp = baseApps;
            usuApp = usuApps;
            confApp = confApps;
            logApp = logApps;
            meApp = meApps;
            assiApp = assiApps;
            pacApp = pacApps;
            gruApp = gruApps;
            aceApp = aceApps;
            pagApp = pagApps;
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

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");

        }

        [HttpGet]
        public ActionResult MontarTelaEmpresa(Int32? id)
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_EMPRESA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Assinante";

                // Carrega Dados
                objeto = baseApp.GetItemByAssinante(usuario.ASSI_CD_ID);
                Session["Empresa"] = objeto;
                ViewBag.Empresa = objeto;
                Session["IdEmpresa"] = objeto.EMPR_CD_ID;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/2/Ajuda2.pdf";

                // Carrega dados do assinante
                ASSINANTE assi = assiApp.GetItemById(idAss);
                List<ASSINANTE_PLANO_ASSINATURA> listaAssi = assi.ASSINANTE_PLANO_ASSINATURA.Where(p => p.ASPA_IN_SISTEMA == 6 & p.ASPA_IN_ATIVO == 1).ToList();
                ViewBag.ListaPlanos = listaAssi;

                List<ASSINANTE_PAGAMENTO> listaPag = assi.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_IN_SISTEMA == 6 & p.ASPA_IN_ATIVO == 1).ToList();
                ViewBag.ListaPags = listaPag;

                // Recupera plano
                Int32 vencer = 0;
                ASSINANTE_PLANO_ASSINATURA plano = listaAssi.FirstOrDefault();
                if (plano.ASPA_DT_VALIDADE < DateTime.Today.Date)
                {
                    vencer = 1;
                }
                if (plano.ASPA_DT_VALIDADE < DateTime.Today.Date.AddDays(30))
                {
                    vencer = 2;
                }
                Session["Vencer"] = vencer;
                ViewBag.Vencer = vencer;
                ViewBag.Plano = plano.PLANO_ASSINATURA.PLAS_NM_NOME;

                // Recupera consumo
                List<USUARIO> listaUsu = CarregaUsuario();
                List<PACIENTE> listaPac = CarregaPaciente();
                List<GRUPO_PAC> listaGru = CarregarGrupo();
                ViewBag.NumUsu = listaUsu.Count;
                ViewBag.NumPac = listaPac.Count;
                ViewBag.NumGru = listaGru.Count;

                if ((Int32)Session["PermMensageria"] == 1)
                {
                    List<MENSAGENS_ENVIADAS_SISTEMA> listaMails = CarregaMensagemEnviada().Where(p => p.MEEN_IN_TIPO == 1 & p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year).ToList();
                    List<MENSAGENS_ENVIADAS_SISTEMA> listaSMS = CarregaMensagemEnviada().Where(p => p.MEEN_IN_TIPO == 2 & p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year).ToList();
                    ViewBag.NumMail = listaMails.Count;
                    ViewBag.NumSMS = listaSMS.Count;
                }

                if ((Int32)Session["PermFinanceiro"] == 1)
                {
                    List<CONSULTA_PAGAMENTO> pags = CarregaPagamento().Where(p => p.COPA_DT_PAGAMENTO != null).ToList();
                    pags = pags.Where(p => p.COPA_DT_PAGAMENTO.Value.Month == DateTime.Today.Date.Month & p.COPA_DT_PAGAMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                    List<CONSULTA_RECEBIMENTO> recs = CarregaRecebimento().Where(p => p.CORE_DT_RECEBIMENTO.Value.Month == DateTime.Today.Date.Month & p.CORE_DT_RECEBIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
                    ViewBag.NumPag = pags.Count;
                    ViewBag.NumRec = recs.Count;
                }

                // Recupera limites
                ViewBag.UsuPlano = plano.PLANO_ASSINATURA.PLAS_NR_USUARIOS > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_USUARIOS.ToString();
                ViewBag.PacPlano = plano.PLANO_ASSINATURA.PLAS_NR_CLIENTES > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_CLIENTES.ToString();
                ViewBag.MailPlano = plano.PLANO_ASSINATURA.PLAS_NR_EMAIL > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_EMAIL.ToString();
                ViewBag.SMSPlano = plano.PLANO_ASSINATURA.PLAS_NR_SMS > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_SMS.ToString();
                ViewBag.GrupoPlano = plano.PLANO_ASSINATURA.PLAS_NR_GRUPO > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_GRUPO.ToString();
                ViewBag.PagPlano = plano.PLANO_ASSINATURA.PLAS_NR_CP > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_CP.ToString();
                ViewBag.RecPlano = plano.PLANO_ASSINATURA.PLAS_NR_CR > 10000 ? "Ilimitado" : plano.PLANO_ASSINATURA.PLAS_NR_CR.ToString();

                // Mensagem
                if (Session["MensEmpresa"] != null)
                {
                    if ((Int32)Session["MensEmpresa"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0020", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0329", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 11)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0113", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 45)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 46)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0272", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 100)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0337", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 101)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0338", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 102)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0339", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensEmpresa"] == 99)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0415", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "EMPRESA", "Empresa", "MontarTelaEmpresa");

                // Abre view
                EmpresaViewModel vm = Mapper.Map<EMPRESA, EmpresaViewModel>(objeto);
                objetoAntes = objeto;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult MontarTelaEmpresa(EmpresaViewModel vm)
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
                    // Sanitização
                    vm.EMPR_NM_GUERRA = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.EMPR_NM_GUERRA);

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    EMPRESA item = Mapper.Map<EmpresaViewModel, EMPRESA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Acerta sessão
                    listaMaster = new List<EMPRESA>();
                    Session["Empresa"] = null;
                    Session["MensEmpresa"] = 0;
                    Session["NivelEmpresa"] = 1;

                    // Encerra
                    return RedirectToAction("MontarTelaEmpresa", "Empresa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Empresa";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult VerAnexoEmpresa(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["ModuloAtual"] = "Assinante - Anexo";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "EMPRESA_ANEXO", "Empresa", "VerAnexoEmpresa");

                // Prepara view
                Session["NivelEmpresa"] = 2;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/2/Ajuda2_6.pdf";
                EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEmpresaAudio(Int32 id)
        {
            try
            {
                // Prepara view
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["ModuloAtual"] = "Assinante - Anexo";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "EMPRESA_ANEXO_AUDIO", "Empresa", "VerAnexoEmpresaAudio");

                EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
                Session["NivelEmpresa"] = 2;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/2/Ajuda2_6.pdf";
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoEmpresa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEmpresa");
        }

        public FileResult DownloadEmpresa(Int32 id)
        {
            try
            {
                EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
                String arquivo = item.EMAN_AQ_ARQUIVO;
                Int32 pos = arquivo.LastIndexOf("/") + 1;
                String nomeDownload = arquivo.Substring(pos);
                String contentType = string.Empty;
                if (arquivo.Contains(".pdf"))
                {
                    contentType = "application/pdf";
                }
                else if (arquivo.Contains(".jpg"))
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
                return File(arquivo, contentType, nomeDownload);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }

        }

        [HttpPost]
        public ActionResult UploadFileEmpresa(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensEmpresa"] = 1;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                EMPRESA item = baseApp.GetItemByAssinante(usuario.ASSI_CD_ID);
                var fileName = Path.GetFileName(file.FileName);

                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensEmpresa"] = 3;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensEmpresa"] = 7;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                String caminho = "/Imagens/" + usuario.ASSI_CD_ID.ToString() + "/Empresa/" + item.EMPR_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                EMPRESA_ANEXO foto = new EMPRESA_ANEXO();
                foto.EMAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.EMAN_DT_ANEXO = DateTime.Today;
                foto.EMAN_IN_ATIVO = 1;
                Int32 tipo = 7;
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
                foto.EMAN_IN_TIPO = tipo;
                foto.EMAN_NM_TITULO = fileName;
                foto.EMPR_CD_ID = item.EMPR_CD_ID;

                item.EMPRESA_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                Session["NivelEmpresa"] = 2;
                return RedirectToAction("VoltarAnexoEmpresa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoEmpresa(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdEmpresa"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensEmpresa"] = 1;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                EMPRESA item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensEmpresa"] = 3;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensEmpresa"] = 7;
                    return RedirectToAction("VoltarAnexoEmpresa");
                }

                String caminho = "/Imagens/" + idAss.ToString() + "/Empresa/" + item.EMPR_CD_ID.ToString() + "/Logo/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // Salva arquivo
                    file.SaveAs(path);

                    // Gravar registro
                    item.EMPR_AQ_LOGO = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }
                Session["VoltaTela"] = 4;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                Session["NivelEmpresa"] = 3;
                return RedirectToAction("MontarTelaEmpresa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnexo(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                EMPRESA_ANEXO item = baseApp.GetAnexoById(id);
                item.EMAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);
                Session["NivelEmpresa"] = 2;
                Session["EmpresaAlterada"] = 1;
                return RedirectToAction("MontarTelaEmpresa");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Empresa";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Empresa", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<USUARIO> CarregaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> conf = new List<USUARIO>();
                if (Session["Usuarios"] == null)
                {
                    conf = usuApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = usuApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<USUARIO>)Session["Usuarios"];
                    }
                }
                conf = conf.Where(p => p.USUA_IN_SISTEMA == 6 || p.USUA_IN_SISTEMA == 0).ToList();
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
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
                Session["VoltaExcecao"] = "Paciente";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<GRUPO_PAC> CarregarGrupo()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<GRUPO_PAC> conf = new List<GRUPO_PAC>();
            if (Session["Grupos"] == null)
            {
                conf = gruApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["GrupoAlterada"] == 1)
                {
                    conf = gruApp.GetAllItens(idAss);
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

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregaMensagemEnviada()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
                if (Session["MensagensEnviadas"] == null)
                {
                    conf = meApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["MensagensEnviadaAlterada"] == 1)
                    {
                        conf = meApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["MensagensEnviadas"];
                    }
                }
                conf = conf.Where(p => p.MEEN_IN_SISTEMA == 6).ToList();
                Session["CatAgendas"] = conf;
                Session["MensagensEnviadaAlterada"] = 0;
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

        [HttpGet]
        public ActionResult IncluirPagamento(Int32 id)
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
                Session["ModuloAtual"] = "Assinante - Pagamento";

                // Prepara listas
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "PIX", Value = "2" });
                ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/2/Ajuda2_1.pdf";

                // Prepara objeto
                ASSINANTE_PAGAMENTO pag = assiApp.GetPagtoById(id);
                AssinantePagamentoViewModel vm = Mapper.Map<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>(pag);
                vm.ASPA_DT_INFORMA = DateTime.Today.Date;
                vm.ASPA_IN_TIPO = 1;
                vm.ASPA_VL_VALOR_PAGO = vm.ASPA_VL_VALOR;
                vm.ASPA_DT_PAGAMENTO = DateTime.Today.Date;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "ASSINANTE_PAGAMENTO", "Assinante", "IncluirPagamento");
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
        public ActionResult IncluirPagamento(AssinantePagamentoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "PIX", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.ASPA_DS_INFORMACOES = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.ASPA_DS_INFORMACOES);

                    // Critica
                    if (vm.ASPA_IN_TIPO == null || vm.ASPA_IN_TIPO == 0)
                    {
                        Session["MensAssinante"] = 1;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0597", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.ASPA_VL_VALOR_PAGO == 0 || vm.ASPA_VL_VALOR_PAGO == null)
                    {
                        vm.ASPA_VL_VALOR_PAGO = vm.ASPA_VL_VALOR;
                    }

                    // Monta pagamento
                    ASSINANTE_PAGAMENTO item = Mapper.Map<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>(vm);

                    // Executa
                    item.ASPA_IN_INFORMADO = 1;
                    Int32 volta = assiApp.ValidateEditPagto(item);

                    // Trata anexos
                    Session["IdPagto"] = item.ASPA_CD_ID;
                    if (Session["FileQueueAssinante"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueAssinante"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                Int32 volta2 = UploadFileQueuePagamento(file);
                            }
                        }
                        Session["FileQueueAssinante"] = null;
                    }

                    // Acerta estado

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "ipgEMPR",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<ASSINANTE_PAGAMENTO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Retorno
                    return RedirectToAction("MontarTelaEmpresa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Paciente";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Paciente", "epront", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
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
            Session["FileQueueAssinante"] = queue;
        }

        [HttpPost]
        public Int32 UploadFileQueuePagamento(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdPagto"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensAssinante"] = 5;
                    return 1;
                }

                // Recupera pagamento
                ASSINANTE_PAGAMENTO item = assiApp.GetPagtoById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensAssinante"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensAssinante"] = 7;
                    return 3;
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;
                if (!((String)Session["ExtensoesPossiveis"]).Contains(extensao.ToUpper()))
                {
                    Session["MensAssinante"] = 12;
                    return 4;
                }

                // Copia arquivo para pasta
                String caminho = "/Imagens/Assinante/" + idAss.ToString() + "/Pagamentos/" + item.ASPA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                ASSINANTE_PAGAMENTO_ANEXO foto = new ASSINANTE_PAGAMENTO_ANEXO();
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
                foto.ASPA_CD_ID = item.ASPA_CD_ID;
                item.ASSINANTE_PAGAMENTO_ANEXO.Add(foto);
                Int32 volta = assiApp.ValidateEditPagto(item);
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
                return 0;
            }
        }

    }
}