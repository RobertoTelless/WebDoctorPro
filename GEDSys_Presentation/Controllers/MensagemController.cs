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

using System.Collections;
using System.Text.RegularExpressions;
using System.Net;       
using System.Text;

using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using CrossCutting;
using System.Net.Mail;
using static iTextSharp.text.pdf.AcroFields;
using System.Reflection;
using CRMPresentation.Controllers;
using Azure.Communication.Email;
using System.Net.Mime;
using ERP_Condominios_Solution.Classes;
using XidNet;
using GEDSys_Presentation.App_Start;
using iText.Kernel.Colors;

namespace ERP_Condominios_Solution.Controllers
{
    public class MensagemController : Controller
    {
        private readonly IMensagemAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IPacienteAppService cliApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateSMSAppService temApp;
        private readonly IGrupoAppService gruApp;
        private readonly ITemplateEMailAppService temaApp;
        private readonly IEMailAgendaAppService emApp;
        private readonly IPeriodicidadeAppService periodicidadeApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAssinanteAppService assApp;
        private readonly IRecursividadeAppService recApp;
        private readonly IMensagemEnviadaSistemaAppService mesApp;
        private readonly IAcessoMetodoAppService aceApp;

#pragma warning disable CS0169 // O campo "MensagemController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "MensagemController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "MensagemController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "MensagemController.exception" nunca é usado

        MENSAGENS objeto = new MENSAGENS();
        MENSAGENS objetoAntes = new MENSAGENS();
        List<MENSAGENS> listaMaster = new List<MENSAGENS>();
        List<RECURSIVIDADE> listaMasterRec = new List<RECURSIVIDADE>();
        RECURSIVIDADE objetoRec = new RECURSIVIDADE();
        List<RESULTADO_ROBOT> listaMasterRobot = new List<RESULTADO_ROBOT>();
        RESULTADO_ROBOT objetoRobot = new RESULTADO_ROBOT();
        List<MENSAGENS_ENVIADAS_SISTEMA> listaEnviadas= new List<MENSAGENS_ENVIADAS_SISTEMA>();
        MENSAGENS_ENVIADAS_SISTEMA objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
        String extensao;

        public MensagemController(IMensagemAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IPacienteAppService cliApps, IConfiguracaoAppService confApps, IGrupoAppService gruApps, ITemplateEMailAppService temaApps, IEMailAgendaAppService emApps, IPeriodicidadeAppService periodicidadeApps, IEmpresaAppService empApps, IAssinanteAppService assApps, IRecursividadeAppService recApps, ITemplateSMSAppService temApps, IMensagemEnviadaSistemaAppService mesApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            cliApp = cliApps;
            confApp = confApps;
            gruApp = gruApps;
            temaApp = temaApps;
            emApp = emApps;
            periodicidadeApp = periodicidadeApps;
            empApp = empApps;
            assApp = assApps;
            recApp = recApps;
            temApp = temApps;
            mesApp = mesApps;
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
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            if (volta == 5)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaDashboardMensagens");
            }
            if (volta == 5)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            if (volta == 2)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        [HttpPost]
        public JsonResult BuscaNomeRazao(String nome)
        {
            Int32 isRazao = 0;
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<Hashtable> listResult = new List<Hashtable>();
            List<PACIENTE> clientes = cliApp.GetAllItens(idAss);
            if ((String)Session["PerfilUsuario"] != "ADM")
            {
                clientes = clientes.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            if (nome != null)
            {
                List<PACIENTE> lstCliente = clientes.Where(x => x.PACI_NM_NOME != null && x.PACI_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<PACIENTE>();

                if (lstCliente == null || lstCliente.Count == 0)
                {
                    isRazao = 1;
                    lstCliente = clientes.Where(x => x.PACI_NM_SOCIAL != null).ToList<PACIENTE>();
                    lstCliente = lstCliente.Where(x => x.PACI_NM_SOCIAL.ToLower().Contains(nome.ToLower())).ToList<PACIENTE>();
                }
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    lstCliente = lstCliente.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }

                if (lstCliente != null)
                {
                    foreach (var item in lstCliente)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.PACI__CD_ID);
                        if (isRazao == 0)
                        {
                            result.Add("text", item.PACI_NM_NOME);
                        }
                        else
                        {
                            result.Add("text", item.PACI_NM_NOME + " (" + item.PACI_NM_SOCIAL + ")");
                        }
                        listResult.Add(result);
                    }
                }
            }
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult     MontarTelaResumoEnvios()
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
                        Session["ModuloPermissao"] = "Mensageria";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }

                    if (usuario.PERFIL.PERF_IN_ACESSO_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Mensageria";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Mensagens - Envios - Robot";

                // Carrega listas
                if (Session["ListaEnvios"] == null)
                {
                    listaMasterRobot = CarregarEnvios().Where(p => p.RERO_DT_ENVIO.Date == DateTime.Today.Date).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();
                    Session["ListaEnvios"] = listaMasterRobot;
                }
                listaMasterRobot = (List<RESULTADO_ROBOT>)Session["ListaEnvios"];
                ViewBag.Listas = listaMasterRobot;

                List<RESULTADO_ROBOT> emails = listaMasterRobot.Where(p => p.RERO_IN_TIPO == 1 & p.RERO_IN_SISTEMA == 6).ToList();
                List<RESULTADO_ROBOT> emailsDia = emails.Where(p => p.RERO_DT_ENVIO.Date == DateTime.Today.Date).ToList();
                List<RESULTADO_ROBOT> emailsMes = emails.Where(p => p.RERO_DT_ENVIO.Month == DateTime.Today.Date.Month & p.RERO_DT_ENVIO.Year == DateTime.Today.Date.Year).ToList();

                ViewBag.EMailTotalEnvio = emails.Count;
                ViewBag.EMailTotalEnvioMes = emailsMes.Count;
                ViewBag.EMailTotalEnvioDia = emailsDia.Count;

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENSAGEM_EMAIL_ROBOT", "Mensagem", "MontarTelaResumoEnvios");

                // Abre view
                Session["VoltaMensagem"] = 1;
                Session["MensMensagem"] = null;
                objetoRobot = new RESULTADO_ROBOT();
                objetoRobot.RERO_DT_ENVIO = DateTime.Today.Date;
                objetoRobot.RERO_DT_DUMMY = DateTime.Today.Date;
                return View(objetoRobot);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroEnviosRobot()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaEnvios"] = null;
                return RedirectToAction("MontarTelaResumoEnvios");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarMesEnviosRobot()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterRobot = baseApp.GetAllEnviosRobot(idAss).Where(p => p.RERO_DT_ENVIO.Month == DateTime.Today.Date.Month & p.RERO_DT_ENVIO.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();
                Session["ListaEnvios"] = listaMasterRobot;
                return RedirectToAction("MontarTelaResumoEnvios");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarEnviosRobot()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterRobot = baseApp.GetAllEnviosRobot(idAss).Where(p => p.RERO_IN_SISTEMA == 6).OrderByDescending(m => m.RERO_DT_ENVIO).ToList();

                Session["ListaEnvios"] = listaMasterRobot;
                return RedirectToAction("MontarTelaResumoEnvios");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();
            List<System.Net.Mail.Attachment> att = new List<System.Net.Mail.Attachment>();
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
                att.Add(new System.Net.Mail.Attachment(file.InputStream, f.Name));
                queue.Add(f);
            }
            Session["FileQueueMensagem"] = queue;
            Session["Attachments"] = att;
        }

        [HttpPost]
        public ActionResult UploadFileQueueMensagem(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            try
            {
                Int32 idNot = (Int32)Session["IdMensagem"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    Session["MensMensagem"] = 10;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }

                MENSAGENS item = baseApp.GetItemById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    Session["MensMensagem"] = 11;
                    return RedirectToAction("VoltarBaseMensagemSMS");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                MENSAGEM_ANEXO foto = new MENSAGEM_ANEXO();
                foto.MEAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.MEAN_DT_ANEXO = DateTime.Today;
                foto.MEAN_IN_ATIVO = 1;
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
                foto.MEAN_IN_TIPO = tipo;
                if (fileName.Length > 245)
                {
                    foto.MEAN_NM_TITULO_NOVO = fileName.Substring(0, 245);
                }
                else
                {
                    foto.MEAN_NM_TITULO_NOVO = fileName;
                }
                foto.MEAN_NM_TITULO = fileName;
                foto.MENS_CD_ID = item.MENS_CD_ID;
                foto.MEAN_BN_BINARIO = System.IO.File.ReadAllBytes(path);
                item.MENSAGEM_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessarEnvioMensagemEMail(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera dados
            Int32 idAss = (Int32)Session["IdAssinante"];
            PACIENTE cliente = null;
            GRUPO_PAC grupo = null;
            List<PACIENTE> listaCli = new List<PACIENTE>();
            Int32 escopo = 0;
            String erro = null;
            Int32 volta = 0;
            Int32 totMens = 0;
            CRMSysDBEntities Db = new CRMSysDBEntities();
            MENSAGENS mens = baseApp.GetItemById(vm.MENS_CD_ID);
            Session["Erro"] = null;
            List<PACIENTE> nova = new List<PACIENTE>();
            ASSINANTE assi = assApp.GetItemById(idAss);
            USUARIO cont = (USUARIO)Session["UserCredentials"];

            // Monta lista de destinatarios
            if (vm.TIPO_ENVIO == 1)
            {
                if (vm.ID > 0)
                {
                    cliente = cliApp.GetItemById(vm.ID.Value);
                    escopo = 1;
                }
                else if (vm.GRPA_CD_ID > 0)
                {
                    listaCli = new List<PACIENTE>();
                    grupo = gruApp.GetItemById((int)vm.GRPA_CD_ID);
                    foreach (GRUPO_PACIENTE item in grupo.GRUPO_PACIENTE)
                    {
                        if (item.GRCL_IN_ATIVO == 1)
                        {
                            listaCli.Add(item.PACIENTE);
                        }
                    }
                    escopo = 2;
                }
                else if (vm.ANIVERSARIO > 0)
                {
                    listaCli = CarregaPaciente();
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        listaCli = listaCli.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }

                    // Recupera aniversarios
                    DateTime hoje = DateTime.Today.Date;
                    if (vm.ANIVERSARIO == 1)
                    {
                        listaCli = listaCli.Where(p => p.PACI_DT_NASCIMENTO.Value.Day == hoje.Day & p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
                    }
                    else
                    {
                        listaCli = listaCli.Where(p => p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
                    }
                    escopo = 2;
                }
            }
            else
            {
                listaCli = CarregaPaciente();
                escopo = 2;
            }
            Session["ClienteEMail"] = cliente;
            Session["ListaClienteEMail"] = listaCli;
            Session["Escopo"] = escopo;

            // Recupera configuração
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Prepara assinatura
            String classe = cont.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + cont.USUA_NR_CLASSE;
            String rod = "<b>" + cont.USUA_NM_NOME + "</b><br />";
            rod += cont.USUA_NM_ESPECIALIDADE + "<br />";
            rod += classe + "  CPF: " + cont.USUA_NR_CPF;

            // Prepara texto
            String cabecalho = String.Empty;
            String texto = String.Empty;
            String rodape = String.Empty;
            String link = vm.MENS_NM_LINK;
            Int32 arq = 0;
            if (vm.MENS_AQ_ARQUIVO == null)
            {
                if (vm.TEEM_CD_ID != null)
                {
                    //TEMPLATE_EMAIL temp = temaApp.GetItemById(vm.TEEM_CD_ID.Value);
                    //texto = temp.TEEM_TX_CORPO;
                    //cabecalho = temp.TEEM_TX_CABECALHO;
                    //rodape = temp.TEEM_TX_DADOS;

                    texto = vm.MENS_TX_TEXTO;
                    cabecalho = vm.MENS_NM_CABECALHO;
                    rodape = vm.MENS_NM_RODAPE;
                }
                else
                {
                    texto = vm.MENS_TX_TEXTO;
                    cabecalho = vm.MENS_NM_CABECALHO;
                    rodape = vm.MENS_NM_RODAPE;
                }
                texto = cabecalho + "<br />" + texto + "<br />" + rodape + "<br />" + rod + "<br />";
            }
            else
            {
                arq = 1;
                texto = System.IO.File.ReadAllText(vm.MENS_AQ_ARQUIVO);
            }

            // Susbtitui texto
            texto = texto.Replace("{Assinante}", assi.ASSI_NM_NOME);

            // Prepara  link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(link))
            {
                if (!link.Contains("www."))
                {
                    link = "www." + link;
                }
                if (!link.Contains("http://"))
                {
                    link = "http://" + link;
                }
                str.AppendLine("<a href='" + link + "'>Clique aqui acessar o link " + vm.MENS_NM_LINK + "</a>");
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "");
            body = body.Replace("<p>", "");
            body = body.Replace("</p>", "<br />");
            body = body.Replace("<br>", "<br />");
            String emailBody = body;
            Session["BodyEMail"] = emailBody;

            // Trata anexos
            List<MENSAGEM_ANEXO> anexos = mens.MENSAGEM_ANEXO.ToList();
            List<AttachmentModel> models = new List<AttachmentModel>();
            if (anexos.Count > 0)
            {
                String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + mens.MENS_CD_ID.ToString() + "/Anexos/";
                foreach (MENSAGEM_ANEXO anexo in anexos)
                {
                    String path = Path.Combine(Server.MapPath(caminho), anexo.MEAN_NM_TITULO_NOVO);

                    AttachmentModel model = new AttachmentModel();
                    model.PATH = path;
                    model.ATTACHMENT_NAME = anexo.MEAN_NM_TITULO_NOVO;
                    if (anexo.MEAN_IN_TIPO == 1)
                    {
                        model.CONTENT_TYPE = MediaTypeNames.Image.Jpeg;
                    }
                    if (anexo.MEAN_IN_TIPO == 3)
                    {
                        model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                    }
                    if (anexo.MEAN_IN_TIPO != 1 & anexo.MEAN_IN_TIPO != 3)
                    {
                        model.CONTENT_TYPE = MediaTypeNames.Application.Octet;
                    }
                    models.Add(model);
                }
            }
            else
            {
                models = null;
            }

            // inicia processo
            String resposta = String.Empty;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // ****************** Processa mensagens - ENVIO UNICO
            if (escopo == 1)
            {
                try
                {
                    // Susbtitui texto
                    emailBody = emailBody.Replace("{Nome}", cliente.PACI_NM_NOME);
                    Session["BodyEMail"] = emailBody;
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
                    Int32? statusBase = 1;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado

                    // Envio direto
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                        EmailAzure mensagem = new EmailAzure();
                        mensagem.ASSUNTO = mens.MENS_NM_NOME;
                        mensagem.CORPO = emailBody;
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_TO_DESTINO = cliente.PACI_NM_EMAIL;
                        mensagem.NOME_EMISSOR_AZURE = emissor;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
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
                            Tuple<EmailSendStatus, String, Boolean> voltaMail = CrossCutting.CommunicationAzurePackage.SendMail(mensagem, models);
                            status = voltaMail.Item1.ToString();
                            iD = voltaMail.Item2;
                            Session["IdMail"] = iD;
                            Session["StatusMail"] = status;
                        }
                        catch (Exception ex)
                        {
                            erro = ex.Message;
                            ViewBag.Message = ex.Message;
                            Session["TipoVolta"] = 2;
                            Session["VoltaExcecao"] = "Mensagens";
                            Session["Excecao"] = ex;
                            Session["ExcecaoTipo"] = ex.GetType().ToString();
                            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                            Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                            throw;
                        }

                        // Grava mensagem/destino e erros
                        MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                        dest.MEDE_IN_ATIVO = 1;
                        dest.MEDE_IN_POSICAO = 1;
                        dest.MEDE_IN_STATUS = 1;
                        dest.PACI_CD_ID = cliente.PACI__CD_ID;
                        dest.MEDE_DS_ERRO_ENVIO = status;
                        dest.MENS_CD_ID = mens.MENS_CD_ID;
                        dest.MEDE_SG_STATUS = status;
                        dest.MEDE_GU_ID_MENSAGEM = iD;
                        dest.MEDE_IN_CRM = 0;
                        dest.MEDE_DT_ENVIO = DateTime.Now;
                        dest.ASSI_CD_ID = idAss;
                        dest.MEDE_IN_SISTEMA = 6;
                        mens.MENSAGENS_DESTINOS.Add(dest);
                        mens.MENS_DT_ENVIO = DateTime.Now;
                        mens.MENS_IN_OCORRENCIAS = 1;
                        mens.MENS_IN_ENVIADAS = 1;
                        mens.MENS_IN_STATUS = 2;
                        mens.MENS_IN_DESTINOS = 1;
                        mens.PACI_CD_ID = cliente.PACI__CD_ID;
                        if (arq == 0)
                        {
                            mens.MENS_TX_TEXTO = body;
                        }
                        else
                        {
                            mens.MENS_TX_TEXTO = vm.MENS_AQ_ARQUIVO;
                        }
                        volta = baseApp.ValidateEdit(mens, mens);

                        Session["Erro"] = erro;
                        erro = null;
                        Session["TotMens"] = 1;
                    }
                    else
                    {
                        // Grava agendamento
                        if (mens.MENS_DT_AGENDAMENTO != null & mens.MENS_NR_REPETICOES == 0)
                        {
                            statusBase = 1;
                            mens.MENS_IN_STATUS = 1;
                            volta = baseApp.ValidateEdit(mens, mens);

                            // Monta recursividade
                            RECURSIVIDADE rec = new RECURSIVIDADE();
                            rec.ASSI_CD_ID = idAss;
                            rec.MENS_CD_ID = mens.MENS_CD_ID;
                            rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                            rec.RECU_IN_TIPO_MENSAGEM = 1;
                            rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                            rec.RECU_IN_TIPO_SMS = 0;
                            rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                            rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                            rec.RECU_IN_SISTEMA = 6;
                            rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                            rec.RECU_IN_TIPO_ENVIO = 1;
                            if (arq == 0)
                            {
                                rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                            }
                            else
                            {
                                rec.RECU_TX_TEXTO = vm.MENS_AQ_ARQUIVO;

                            }
                            rec.RECU_IN_ATIVO = 1;

                            // Monta destinos
                            PACIENTE cli = (PACIENTE)Session["ClienteEMail"];
                            RECURSIVIDADE_DESTINO dest1 = new RECURSIVIDADE_DESTINO();
                            dest1.ASSI_CD_ID = idAss;
                            dest1.PACI_CD_ID = cli.PACI__CD_ID;
                            dest1.REDE_EM_EMAIL = cli.PACI_NM_EMAIL;
                            dest1.REDE_NM_NOME = cli.PACI_NM_NOME;
                            dest1.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.PACI_NM_NOME);
                            dest1.REDE_IN_ATIVO = 1;
                            dest1.USUA_CD_ID = usuario.USUA_CD_ID;
                            rec.RECURSIVIDADE_DESTINO.Add(dest1);

                            // Monta Datas
                            RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                            data1.REDA_DT_PROGRAMADA = mens.MENS_DT_AGENDAMENTO.Value;
                            data1.ASSI_CD_ID = idAss;
                            data1.REDA_IN_PROCESSADA = 0;
                            data1.REDA_IN_ATIVO = 1;
                            data1.USUA_CD_ID = usuario.USUA_CD_ID;
                            data1.REDA_IN_SISTEMA = 6;
                            rec.RECURSIVIDADE_DATA.Add(data1);

                            // Grava recursividade
                            Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                        }

                        // Monta registro de recursividade
                        if (mens.MENS_DT_AGENDAMENTO != null & mens.MENS_NR_REPETICOES != 0)
                        {
                            // Grava status
                            mens.MENS_IN_STATUS = 4;
                            volta = baseApp.ValidateEdit(mens, mens);

                            // Monta recursividade
                            RECURSIVIDADE rec = new RECURSIVIDADE();
                            rec.ASSI_CD_ID = idAss;
                            rec.MENS_CD_ID = mens.MENS_CD_ID;
                            rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                            rec.RECU_IN_TIPO_MENSAGEM = 1;
                            rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                            rec.RECU_IN_SISTEMA = 6;
                            rec.RECU_IN_TIPO_ENVIO = 1;
                            if (mens.MENS_DT_AGENDAMENTO == null)
                            {
                                rec.RECU_DT_CRIACAO = DateTime.Now;
                            }
                            else
                            {
                                rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                            }
                            rec.RECU_IN_TIPO_SMS = 0;
                            rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                            rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                            if (arq == 0)
                            {
                                rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                            }
                            else
                            {
                                rec.RECU_TX_TEXTO = vm.MENS_AQ_ARQUIVO;

                            }
                            rec.RECU_IN_ATIVO = 1;

                            // Monta destinos
                            PACIENTE cli = (PACIENTE)Session["ClienteEMail"];
                            RECURSIVIDADE_DESTINO dest2 = new RECURSIVIDADE_DESTINO();
                            dest2.ASSI_CD_ID = idAss;
                            dest2.PACI_CD_ID = cli.PACI__CD_ID;
                            dest2.REDE_EM_EMAIL = cli.PACI_NM_EMAIL;
                            dest2.REDE_NM_NOME = cli.PACI_NM_NOME;
                            dest2.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.PACI_NM_NOME);
                            dest2.REDE_IN_ATIVO = 1;
                            rec.RECURSIVIDADE_DESTINO.Add(dest2);

                            if (vm.FORMA_REPETICAO == 1)
                            {
                                // Monta Datas
                                Int32 dias = 0;
                                Int32 numRep = 1;
                                if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    dias = 0;
                                    numRep = 1;
                                }
                                else
                                {
                                    if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                                    {
                                        dias = 30;
                                    }
                                    else
                                    {
                                        PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                                        if (peri != null)
                                        {
                                            dias = peri.PETA_NR_DIAS;
                                        }

                                    }

                                    if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                    {
                                        numRep = 1;
                                    }
                                    else
                                    {
                                        numRep = mens.MENS_NR_REPETICOES.Value;
                                    }
                                }

                                DateTime datax = DateTime.Now.AddMinutes(30);
                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    datax = mens.MENS_DT_AGENDAMENTO.Value;
                                }
                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = datax;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.USUA_CD_ID = usuario.USUA_CD_ID;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    datax = datax.AddDays(dias);
                                }
                            }
                            else if (vm.FORMA_REPETICAO == 2)
                            {
                                // Monta Datas
                                Int32 dias = 0;
                                Int32 numRep = 0;

                                if ((vm.DIA_MES == null || vm.DIA_MES == 0))
                                {
                                    dias = 1;
                                }
                                else
                                {
                                    dias = vm.DIA_MES.Value;
                                }

                                if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    numRep = 1;
                                }
                                else
                                {
                                    numRep = mens.MENS_NR_REPETICOES.Value;
                                }

                                DateTime hoje = DateTime.Now;
                                String dataS = dias.ToString() + "/" + hoje.Month.ToString() + "/" + hoje.Year.ToString();
                                DateTime inicio = Convert.ToDateTime(dataS);

                                if (dias < hoje.Day )
                                {
                                    inicio = inicio.AddMonths(1);
                                }
                                
                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    if (mens.MENS_DT_AGENDAMENTO.Value.Date > inicio.Date)
                                    {
                                        if (mens.MENS_DT_AGENDAMENTO.Value.Month == inicio.Month)
                                        {
                                            inicio = inicio.AddMonths(1);
                                        }
                                        else
                                        {
                                            String dataC = dias.ToString() + "/" + mens.MENS_DT_AGENDAMENTO.Value.Month.ToString() + "/" + mens.MENS_DT_AGENDAMENTO.Value.Year.ToString();
                                            inicio = Convert.ToDateTime(dataC);
                                        }

                                    }
                                }
                                inicio = inicio.AddMinutes(30);

                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = inicio;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.USUA_CD_ID = usuario.USUA_CD_ID;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    inicio = inicio.AddMonths(1);
                                }
                            }
                            else if (vm.FORMA_REPETICAO == 3)
                            {
                                // Monta Datas
                                Int32 dias = 0;
                                Int32 numRep = 0;

                                if ((vm.DIA_SEMANA == null || vm.DIA_SEMANA == 0))
                                {
                                    dias = 1;
                                }
                                else
                                {
                                    dias = vm.DIA_SEMANA.Value;
                                }

                                if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    numRep = 1;
                                }
                                else
                                {
                                    numRep = mens.MENS_NR_REPETICOES.Value;
                                }

                                String dataS = String.Empty;
                                DateTime inicio = new DateTime();
                                DateTime hoje = DateTime.Now;
                                DayOfWeek semana = hoje.DayOfWeek;
                                Int32 diaSemana = (Int32)semana;
                                if (diaSemana == dias)
                                {
                                    inicio = hoje;
                                }
                                else
                                {
                                    Int32 daysUntilNext = (dias - diaSemana + 7) % 7;
                                    inicio = hoje.AddDays(daysUntilNext);
                                }

                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    if (mens.MENS_DT_AGENDAMENTO.Value.Date > inicio.Date)
                                    {
                                        Int32 daysUntilNext1 = (dias - (Int32)mens.MENS_DT_AGENDAMENTO.Value.DayOfWeek + 7) % 7;
                                        inicio = mens.MENS_DT_AGENDAMENTO.Value.AddDays(daysUntilNext1);
                                    }
                                }
                                inicio = inicio.AddMinutes(30);

                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = inicio;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.USUA_CD_ID = usuario.USUA_CD_ID;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    inicio = inicio.AddDays(7);
                                }
                            }

                            // Grava recursividade
                            Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                        }

                        // Grava mensagem/destino e erros
                        iD = Xid.NewXid().ToString();
                        MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                        dest.MEDE_IN_ATIVO = 1;
                        dest.MEDE_IN_POSICAO = 1;
                        dest.MEDE_IN_STATUS = 1;
                        dest.PACI_CD_ID = cliente.PACI__CD_ID;
                        dest.MEDE_DS_ERRO_ENVIO = status;
                        dest.MENS_CD_ID = mens.MENS_CD_ID;
                        dest.MEDE_SG_STATUS = status;
                        dest.MEDE_GU_ID_MENSAGEM = iD;
                        dest.MEDE_IN_CRM = 0;
                        dest.MEDE_DT_ENVIO = DateTime.Now;
                        dest.ASSI_CD_ID = idAss;
                        dest.MEDE_IN_SISTEMA = 6;
                        mens.MENSAGENS_DESTINOS.Add(dest);
                        mens.MENS_DT_ENVIO = null;
                        mens.MENS_IN_DESTINOS = 1;
                        if (mens.MENS_NR_REPETICOES > 0)
                        {
                            mens.MENS_IN_OCORRENCIAS = mens.MENS_NR_REPETICOES;
                        }
                        else
                        {
                            mens.MENS_IN_OCORRENCIAS = 1;
                        }
                        mens.MENS_IN_ENVIADAS = 0;
                        if (arq == 0)
                        {
                            mens.MENS_TX_TEXTO = body;
                        }
                        else
                        {
                            mens.MENS_TX_TEXTO = vm.MENS_AQ_ARQUIVO;
                        }
                        volta = baseApp.ValidateEdit(mens, mens);
                    }

                    Session["ListaRecursividade"] = null;
                    Session["TotMens"] = 1;
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }
                return 0;
            }
            else
            {
                try
                {
                    // ***************** Envio para grupo
                    Int32? ocorrencias = 1;
                    Int32? destinos = 0;
                    List<EmailAddress> emails = new List<EmailAddress>();
                    String data = String.Empty;
                    String json = String.Empty;

                    // Checa se todos tem e-mail e monta lista
                    foreach (PACIENTE cli in listaCli)
                    {
                        if (cli.PACI_NM_EMAIL != null)
                        {
                            //emailBody = emailBody.Replace("{Nome}", cli.PACI_NM_NOME);
                            totMens++;
                            nova.Add(cli);

                            EmailAddress add = new EmailAddress(
                                    address: cli.PACI_NM_EMAIL,
                                    displayName: cli.PACI_NM_NOME);
                            emails.Add(add);
                            destinos++;
                        }
                    }

                    // Envio imediato
                    if (mens.MENS_DT_AGENDAMENTO == null)
                    {
                        mens.MENS_IN_STATUS = 1;
                        volta = baseApp.ValidateEdit(mens, mens);

                        // Monta registro de recursividade
                        RECURSIVIDADE rec = new RECURSIVIDADE();
                        rec.ASSI_CD_ID = idAss;
                        rec.MENS_CD_ID = mens.MENS_CD_ID;
                        rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                        rec.RECU_IN_TIPO_MENSAGEM = 1;
                        rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                        if (mens.MENS_DT_AGENDAMENTO == null)
                        {
                            rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                        }
                        else
                        {
                            rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                        }
                        rec.RECU_IN_TIPO_SMS = 0;
                        rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                        rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                        rec.RECU_IN_TIPO_ENVIO = 1;
                        if (arq == 0)
                        {
                            rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                        }
                        else
                        {
                            rec.RECU_TX_TEXTO = vm.MENS_AQ_ARQUIVO;

                        }
                        rec.RECU_IN_ATIVO = 1;
                        rec.RECU_IN_SISTEMA = 6;

                        // Monta destinos
                        List<PACIENTE> lista = nova;
                        foreach (PACIENTE cli in lista)
                        {
                            RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                            dest.ASSI_CD_ID = idAss;
                            dest.PACI_CD_ID = cli.PACI__CD_ID;
                            dest.REDE_EM_EMAIL = cli.PACI_NM_EMAIL;
                            dest.REDE_NM_NOME = cli.PACI_NM_NOME;
                            dest.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.PACI_NM_NOME);
                            dest.REDE_IN_ATIVO = 1;
                            dest.USUA_CD_ID = usuario.USUA_CD_ID;
                            rec.RECURSIVIDADE_DESTINO.Add(dest);
                        }

                        RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                        data1.ASSI_CD_ID = idAss;
                        data1.REDA_DT_PROGRAMADA = mens.MENS_DT_CRIACAO.Value;
                        data1.REDA_IN_PROCESSADA = 0;
                        data1.REDA_IN_ATIVO = 1;
                        data1.REDA_IN_SISTEMA = 6;
                        rec.RECURSIVIDADE_DATA.Add(data1);

                        // Grava recursividade
                        Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                        Session["TotMens"] = totMens;
                        Session["ListaRecursividade"] = null;

                        // Grava mensagem/destino e erros
                        Session["Erro"] = erro;
                        foreach (PACIENTE cli in nova)
                        {
                            iD = Xid.NewXid().ToString();
                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.PACI_CD_ID = cli.PACI__CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = resposta;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            dest.MEDE_SG_STATUS = status;
                            dest.MEDE_GU_ID_MENSAGEM = iD;
                            dest.MEDE_IN_CRM = 0;
                            dest.MEDE_DT_ENVIO = DateTime.Now;
                            mens.MENS_IN_OCORRENCIAS = nova.Count;
                            mens.MENS_IN_ENVIADAS = nova.Count;
                            dest.ASSI_CD_ID = idAss;
                            dest.MEDE_IN_SISTEMA = 6;
                            mens.MENS_IN_DESTINOS = destinos;
                            if (arq == 0)
                            {
                                mens.MENS_TX_TEXTO = body;
                            }
                            else
                            {
                                mens.MENS_TX_TEXTO = vm.MENS_AQ_ARQUIVO;
                            }
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = DateTime.Now;
                            mens.MENS_IN_STATUS = 2;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                        erro = null;
                    }
                    else
                    {
                        // Grava agendamento
                        if (mens.MENS_DT_AGENDAMENTO != null & mens.MENS_NR_REPETICOES == 0)
                        {
                            mens.MENS_IN_STATUS = 1;
                            volta = baseApp.ValidateEdit(mens, mens);

                            // Monta registro de recursividade
                            RECURSIVIDADE rec = new RECURSIVIDADE();
                            rec.ASSI_CD_ID = idAss;
                            rec.MENS_CD_ID = mens.MENS_CD_ID;
                            rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                            rec.RECU_IN_TIPO_MENSAGEM = 1;
                            rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                            if (mens.MENS_DT_AGENDAMENTO == null)
                            {
                                rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                            }
                            else
                            {
                                rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                            }
                            rec.RECU_IN_TIPO_SMS = 0;
                            rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                            rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                            rec.RECU_IN_TIPO_ENVIO = 1;
                            if (arq == 0)
                            {
                                rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                            }
                            else
                            {
                                rec.RECU_TX_TEXTO = vm.MENS_AQ_ARQUIVO;

                            }
                            rec.RECU_IN_ATIVO = 1;
                            rec.RECU_IN_SISTEMA = 6;

                            // Monta destinos
                            List<PACIENTE> lista = (List<PACIENTE>)Session["ListaClienteEMail"];
                            foreach (PACIENTE cli in lista)
                            {
                                RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                                dest.ASSI_CD_ID = idAss;
                                dest.PACI_CD_ID = cli.PACI__CD_ID;
                                dest.REDE_EM_EMAIL = cli.PACI_NM_EMAIL;
                                dest.REDE_NM_NOME = cli.PACI_NM_NOME;
                                dest.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.PACI_NM_NOME);
                                dest.REDE_IN_ATIVO = 1;
                                dest.USUA_CD_ID = usuario.USUA_CD_ID;
                                rec.RECURSIVIDADE_DESTINO.Add(dest);
                            }

                            RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                            data1.ASSI_CD_ID = idAss;
                            data1.REDA_DT_PROGRAMADA = mens.MENS_DT_AGENDAMENTO.Value;
                            data1.REDA_IN_PROCESSADA = 0;
                            data1.REDA_IN_ATIVO = 1;
                            data1.REDA_IN_SISTEMA = 6;
                            rec.RECURSIVIDADE_DATA.Add(data1);

                            // Grava recursividade
                            Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                            Session["TotMens"] = totMens;
                            Session["ListaRecursividade"] = null;
                        }

                        if (mens.MENS_DT_AGENDAMENTO != null & mens.MENS_NR_REPETICOES != 0)
                        {
                            mens.MENS_IN_STATUS = 4;
                            volta = baseApp.ValidateEdit(mens, mens);

                            // Monta registro de recursividade
                            ocorrencias = mens.MENS_NR_REPETICOES;
                            RECURSIVIDADE rec = new RECURSIVIDADE();
                            rec.ASSI_CD_ID = idAss;
                            rec.MENS_CD_ID = mens.MENS_CD_ID;
                            rec.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                            rec.RECU_IN_TIPO_MENSAGEM = 1;
                            rec.EMFI_CD_ID = usuario.EMFI_CD_ID;
                            if (mens.MENS_DT_AGENDAMENTO == null)
                            {
                                rec.RECU_DT_CRIACAO = DateTime.Today.Date;
                            }
                            else
                            {
                                rec.RECU_DT_CRIACAO = mens.MENS_DT_AGENDAMENTO.Value;
                            }
                            rec.RECU_IN_TIPO_SMS = 0;
                            rec.RECU_NM_NOME = mens.MENS_NM_NOME;
                            rec.RECU_LK_LINK = mens.MENS_NM_LINK;
                            rec.RECU_IN_TIPO_ENVIO = 1;
                            if (arq == 0)
                            {
                                rec.RECU_TX_TEXTO = (String)Session["BodyEMail"];
                            }
                            else
                            {
                                rec.RECU_TX_TEXTO = vm.MENS_AQ_ARQUIVO;

                            }
                            rec.RECU_IN_ATIVO = 1;
                            rec.RECU_IN_SISTEMA = 6;

                            // Monta destinos
                            List<PACIENTE> lista = (List<PACIENTE>)Session["ListaClienteEMail"];
                            foreach (PACIENTE cli in lista)
                            {
                                RECURSIVIDADE_DESTINO dest = new RECURSIVIDADE_DESTINO();
                                dest.ASSI_CD_ID = idAss;
                                dest.PACI_CD_ID = cli.PACI__CD_ID;
                                dest.REDE_EM_EMAIL = cli.PACI_NM_EMAIL;
                                dest.REDE_NM_NOME = cli.PACI_NM_NOME;
                                dest.REDE_TX_CORPO = ((String)Session["BodyEMail"]).Replace("{Nome}", cli.PACI_NM_NOME);
                                dest.REDE_IN_ATIVO = 1;
                                dest.USUA_CD_ID = usuario.USUA_CD_ID;
                                rec.RECURSIVIDADE_DESTINO.Add(dest);
                            }

                            if (vm.FORMA_REPETICAO == 1)
                            {
                                Int32 dias = 0;
                                Int32 numRep = 1;
                                if ((mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0) & mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    dias = 0;
                                    numRep = 1;
                                }
                                else
                                {
                                    if (mens.PETA_CD_ID == null || mens.PETA_CD_ID == 0)
                                    {
                                        dias = 30;
                                    }
                                    PERIODICIDADE_TAREFA peri = periodicidadeApp.GetItemById(mens.PETA_CD_ID.Value);
                                    if (peri != null)
                                    {
                                        dias = peri.PETA_NR_DIAS;
                                    }

                                    if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                    {
                                        numRep = 1;
                                    }
                                    else
                                    {
                                        numRep = mens.MENS_NR_REPETICOES.Value;
                                    }
                                }

                                DateTime datax = DateTime.Now;
                                datax = datax.AddDays(dias);
                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    datax = mens.MENS_DT_AGENDAMENTO.Value;
                                }
                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = datax;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    datax = datax.AddDays(dias);
                                }
                            }
                            else if (vm.FORMA_REPETICAO == 2)
                            {
                                // Monta Datas
                                Int32 dias = 0;
                                Int32 numRep = 0;

                                if ((vm.DIA_MES == null || vm.DIA_MES == 0))
                                {
                                    dias = 1;
                                }
                                else
                                {
                                    dias = vm.DIA_MES.Value;
                                }

                                if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    numRep = 1;
                                }
                                else
                                {
                                    numRep = mens.MENS_NR_REPETICOES.Value;
                                }

                                DateTime hoje = DateTime.Now;
                                String dataS = dias.ToString() + "/" + hoje.Month.ToString() + "/" + hoje.Year.ToString();
                                DateTime inicio = Convert.ToDateTime(dataS);

                                if (dias < hoje.Day)
                                {
                                    inicio = inicio.AddMonths(1);
                                }

                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    if (mens.MENS_DT_AGENDAMENTO.Value.Date > inicio.Date)
                                    {
                                        if (mens.MENS_DT_AGENDAMENTO.Value.Month == inicio.Month)
                                        {
                                            inicio = inicio.AddMonths(1);
                                        }
                                        else
                                        {
                                            String dataC = dias.ToString() + "/" + mens.MENS_DT_AGENDAMENTO.Value.Month.ToString() + "/" + mens.MENS_DT_AGENDAMENTO.Value.Year.ToString();
                                            inicio = Convert.ToDateTime(dataC);
                                        }

                                    }
                                }
                                inicio = inicio.AddMinutes(30);

                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = inicio;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    inicio = inicio.AddMonths(1);
                                }
                            }
                            else if (vm.FORMA_REPETICAO == 3)
                            {
                                // Monta Datas
                                Int32 dias = 0;
                                Int32 numRep = 0;

                                if ((vm.DIA_SEMANA == null || vm.DIA_SEMANA == 0))
                                {
                                    dias = 1;
                                }
                                else
                                {
                                    dias = vm.DIA_SEMANA.Value;
                                }

                                if (mens.MENS_NR_REPETICOES == null || mens.MENS_NR_REPETICOES == 0)
                                {
                                    numRep = 1;
                                }
                                else
                                {
                                    numRep = mens.MENS_NR_REPETICOES.Value;
                                }

                                String dataS = String.Empty;
                                DateTime inicio = new DateTime();
                                DateTime hoje = DateTime.Now;
                                DayOfWeek semana = hoje.DayOfWeek;
                                Int32 diaSemana = (Int32)semana;
                                if (diaSemana == dias)
                                {
                                    inicio = hoje;
                                }
                                else
                                {
                                    Int32 daysUntilNext = (dias - diaSemana + 7) % 7;
                                    inicio = hoje.AddDays(daysUntilNext);
                                }

                                if (mens.MENS_DT_AGENDAMENTO != null)
                                {
                                    if (mens.MENS_DT_AGENDAMENTO.Value.Date > inicio.Date)
                                    {
                                        Int32 daysUntilNext1 = (dias - (Int32)mens.MENS_DT_AGENDAMENTO.Value.DayOfWeek + 7) % 7;
                                        inicio = mens.MENS_DT_AGENDAMENTO.Value.AddDays(daysUntilNext1);
                                    }
                                }
                                inicio = inicio.AddMinutes(30);

                                for (Int32 i = 1; i <= numRep; i++)
                                {
                                    RECURSIVIDADE_DATA data1 = new RECURSIVIDADE_DATA();
                                    data1.ASSI_CD_ID = idAss;
                                    data1.REDA_DT_PROGRAMADA = inicio;
                                    data1.REDA_IN_PROCESSADA = 0;
                                    data1.REDA_IN_ATIVO = 1;
                                    data1.REDA_IN_SISTEMA = 6;
                                    rec.RECURSIVIDADE_DATA.Add(data1);
                                    inicio = inicio.AddDays(7);
                                }
                            }

                            // Grava recursividade
                            Int32 voltaRec = recApp.ValidateCreate(rec, usuario);
                            Session["TotMens"] = totMens;
                            Session["ListaRecursividade"] = null;
                        }

                        // Grava mensagem/destino e erros
                        Session["Erro"] = erro;
                        foreach (PACIENTE cli in nova)
                        {
                            iD = Xid.NewXid().ToString();
                            MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                            dest.MEDE_IN_ATIVO = 1;
                            dest.MEDE_IN_POSICAO = 1;
                            dest.MEDE_IN_STATUS = 1;
                            dest.PACI_CD_ID = cli.PACI__CD_ID;
                            dest.MEDE_DS_ERRO_ENVIO = resposta;
                            dest.MENS_CD_ID = mens.MENS_CD_ID;
                            dest.MEDE_SG_STATUS = status;
                            dest.MEDE_GU_ID_MENSAGEM = iD;
                            dest.MEDE_IN_CRM = 0;
                            dest.MEDE_DT_ENVIO = null;
                            dest.ASSI_CD_ID = idAss;
                            dest.MEDE_IN_SISTEMA = 6;
                            if (mens.MENS_NR_REPETICOES > 0)
                            {
                                mens.MENS_IN_OCORRENCIAS = nova.Count * mens.MENS_NR_REPETICOES;
                            }
                            else
                            {
                                mens.MENS_IN_OCORRENCIAS = nova.Count;
                            }
                            mens.MENS_IN_ENVIADAS = 0;
                            if (arq == 0)
                            {
                                mens.MENS_TX_TEXTO = body;
                            }
                            else
                            {
                                mens.MENS_TX_TEXTO = vm.MENS_AQ_ARQUIVO;
                            }
                            mens.MENSAGENS_DESTINOS.Add(dest);
                            mens.MENS_DT_ENVIO = null;
                            mens.MENS_IN_DESTINOS = destinos;
                            volta = baseApp.ValidateEdit(mens, mens);
                        }
                    }
                    return 0;   
                }
                catch (Exception ex)
                {
                    erro = ex.Message;
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    throw;
                }
            }
        }

        [HttpGet]
        public ActionResult MontarTelaMensagemEMail()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Mensageria";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Mensagens - E-Mail";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/23/Ajuda23.pdf";

                // Carrega estatisticas de mensagens
                Int32 volta = CarregaNumeroMensagem();

                // Carrega listas
                if (Session["ListaMensagemEMail"] == null)
                {
                    listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    Session["ListaMensagemEMail"] = listaMaster;
                }
                ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagemEMail"];
                Session["Mensagem"] = null;
                Session["IncluirMensagem"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/23/Ajuda23.pdf";

                ViewBag.EMailTotalEnvio = (Int32)Session["EMailTotalEnvio"];
                ViewBag.EMailTotalEnvioMes = (Int32)Session["EMailTotalEnvioMes"];
                ViewBag.EMailTotalEnvioDia = (Int32)Session["EMailTotalEnvioDia"];


                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 40)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture);
                        frase += " - " + (String)Session["CliCRM"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensMensagem"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0260", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0252", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 60)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 61)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0258", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 80)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0269", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 99)
                    {
                        ModelState.AddModelError("", (String)Session["MsgCRUD"]);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENSAGEM_EMAIL", "Mensagem", "MontarTelaMensagemEMail");

                // Abre view
                Session["VoltaMensagem"] = 1;
                Session["MensMensagem"] = null;
                Session["VoltaRec"] = 1;
                Session["FlagMensagensEnviadas"] = 50;
                Session["VoltaPaciente"] = 99;
                Session["VoltaGrupo"] = 99;
                Session["ListaLog"] = null;
                objeto = new MENSAGENS();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMensagemEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaMensagemEMail"] = null;
                Session["FiltroMensagem"] = null;
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarMesAtualMensagemEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagemEMail"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoMensagemEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_SISTEMA == 6 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagem"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarMesesMensagemEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagemEMail"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMensagemEMail(MENSAGENS item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                // Executa a operação
                List<MENSAGENS> listaObj = new List<MENSAGENS>();
                Session["FiltroMensagem"] = item;
                Tuple<Int32, List<MENSAGENS>, Boolean> volta = baseApp.ExecuteFilterEMail(item.MENS_DT_ENVIO, item.MENS_DT_AGENDAMENTO, 0, item.MENS_NM_NOME, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaMensagemEmail");
                }

                // Sucesso
                listaMaster = volta.Item2;
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                Session["ListaMensagemEMail"] = listaMaster;
                return RedirectToAction("MontarTelaMensagemEmail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarEnviosRobot(RESULTADO_ROBOT item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                // Executa a operação
                List<RESULTADO_ROBOT> listaObj = new List<RESULTADO_ROBOT>();
                Tuple<Int32, List<RESULTADO_ROBOT>, Boolean> volta = baseApp.ExecuteFilterRobot(item.RERO_IN_TIPO, item.RERO_DT_ENVIO, item.RERO_DT_DUMMY, item.RERO_NM_BUSCA, item.RERO_NM_EMAIL, item.RERO_NR_CELULAR, item.RERO_IN_STATUS, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaResumoEnvios");
                }

                // Sucesso
                listaMasterRobot = volta.Item2;
                listaMasterRobot = listaMasterRobot.Where(p => p.RERO_IN_SISTEMA == 2).ToList();
                Session["ListaEnvios"] = volta.Item2;
                return RedirectToAction("MontarTelaResumoEnvios");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseMensagemEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult VoltarBaseMensagem()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        public ActionResult VoltarRecursivo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        public ActionResult VoltarAnexoMensagemEMail()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 volta = (Int32)Session["VoltaMensagem"];
            if (volta == 1)
            {
                return RedirectToAction("MontarTelaMensagemEMail");
            }
            else if (volta == 2)
            {
                return RedirectToAction("VoltarAnexoPaciente", "Paciente");
            }
            else if (volta == 3)
            {
                return RedirectToAction("MontarTelaPaciente", "Paciente");
            }
            else if (volta == 11)
            {
                return RedirectToAction("VerEMailAgendados", "Mensagem");
            }
            return RedirectToAction("MontarTelaMensagemEMail");
        }

        [HttpGet]
        public ActionResult ExcluirMensagemEMail(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((Int32)Session["PermMens"] == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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

                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                Session["FlagAlteraEstado"] = 1;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarMensagemEMail(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((Int32)Session["PermMens"] == 0)
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
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

                MENSAGENS item = baseApp.GetItemById(id);
                item.MENS_IN_ATIVO = 1;
                Int32 volta = baseApp.ValidateReativar(item, usuario);
                Session["ListaMensagem"] = null;
                Session["MensagemAlterada"] = 1;
                Session["FlagAlteraEstado"] = 1;
                return RedirectToAction("VoltarBaseMensagemEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagens";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public JsonResult PesquisaTemplateEMail(String temp)
        {
            var hash = new Hashtable();
            if (!string.IsNullOrEmpty(temp))
            {
                // Recupera Template
                TEMPLATE_EMAIL tmp = temaApp.GetItemById(Convert.ToInt32(temp));

                // Atualiza
                hash.Add("TEEM_TX_CORPO", tmp.TEEM_TX_CORPO);
                hash.Add("TEEM_TX_CABECALHO", tmp.TEEM_TX_CABECALHO);
                hash.Add("TEEM_TX_DADOS", tmp.TEEM_TX_DADOS);
            }

            // Retorna
            return Json(hash);
        }

        public ActionResult IncluirPaciente()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["VoltaPaciente"] = 2;
                Session["VoltaTela"] = 0;
                Session["VoltaMsg"] = 6666;
                ViewBag.Incluir = (Int32)Session["VoltaTela"];
                return RedirectToAction("IncluirPaciente", "Paciente");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirGrupo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["VoltaGrupo"] = 7;
                return RedirectToAction("IncluirGrupo", "Grupo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail()
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
                        Session["ModuloPermissao"] = "Mensageria";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "E-Mail - Criação";
                        return RedirectToAction("MontarTelaMensagemEMail");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Mensagens - E-Mail - Criação";
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/23/Ajuda23_1.pdf";

                // Verifica carga de mensagens
                if ((Int32)Session["MensagemCarregada"] == 0)
                {
                    Int32 volta = CarregaNumeroMensagem();
                }
                List<MENSAGENS> listaMens = (List<MENSAGENS>)Session["ListaEMailTudo"];

                // Verifica possibilidade
                Int32 num = listaMens.Where(p => p.MENS_IN_TIPO.Value == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensMensagem"] = 51;
                    return RedirectToAction("VoltarBaseMensagemEMail");
                }

                // Prepara listas   
                List<PACIENTE> listaTotal = CarregaPaciente();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaTotal = listaTotal.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");

                List<GRUPO_PAC> listaTotal1 = CarregaGrupo();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    listaTotal1 = listaTotal1.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                listaTotal1 = listaTotal1.Where(p => p.GRUPO_PACIENTE.Count > 0).ToList();
                ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");

                Session["Mensagem"] = null;
                ViewBag.Temp = new SelectList(CarregarModeloEMail().Where(p => p.TEEM_IN_HTML == 1 & p.TEEM_IN_FIXO == 0).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
                ViewBag.Usuario = usuario;

                ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
                List<SelectListItem> tipoEnvio = new List<SelectListItem>();
                tipoEnvio.Add(new SelectListItem() { Text = "Selecionar Pacientes", Value = "1" });
                tipoEnvio.Add(new SelectListItem() { Text = "Todos os Pacientes", Value = "2" });
                ViewBag.TiposEnvio = new SelectList(tipoEnvio, "Value", "Text");
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> forma = new List<SelectListItem>();
                forma.Add(new SelectListItem() { Text = "Periodicidade", Value = "1" });
                forma.Add(new SelectListItem() { Text = "Dia do Mês", Value = "2" });
                forma.Add(new SelectListItem() { Text = "Dia da Semana", Value = "3" });
                ViewBag.Forma = new SelectList(forma, "Value", "Text");
                List<SelectListItem> semana = new List<SelectListItem>();
                semana.Add(new SelectListItem() { Text = "Domingo", Value = "0" });
                semana.Add(new SelectListItem() { Text = "2a Feira", Value = "1" });
                semana.Add(new SelectListItem() { Text = "3a feira", Value = "2" });
                semana.Add(new SelectListItem() { Text = "4a feira", Value = "3" });
                semana.Add(new SelectListItem() { Text = "5a feira", Value = "4" });
                semana.Add(new SelectListItem() { Text = "6a feira", Value = "5" });
                semana.Add(new SelectListItem() { Text = "Sábado", Value = "6" });
                ViewBag.Semana = new SelectList(semana, "Value", "Text");
                List<SelectListItem> aniv = new List<SelectListItem>();
                aniv.Add(new SelectListItem() { Text = "Aniversariantes do Dia", Value = "1" });
                aniv.Add(new SelectListItem() { Text = "Aniversariantes do Mês", Value = "2" });
                ViewBag.Aniversariantes = new SelectList(aniv, "Value", "Text");
                ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 52)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensMensagem"] == 72)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENSAGEM_EMAIL_INCLUIR", "Mensagem", "IncluirMensagemEMail");

                // Prepara view
                Session["EMailAgendaNum"] = 0;
                Session["MensagemNovo"] = 0;
                Session["VoltaPaciente"] = 99;
                Session["VoltaGrupo"] = 99;
                MENSAGENS item = new MENSAGENS();
                MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.MENS_DT_CRIACAO = DateTime.Now;
                vm.MENS_IN_ATIVO = 1;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.MENS_IN_TIPO = 1;
                vm.MENS_TX_TEXTO = null;
                vm.ID = 0;
                vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
                vm.MENS_IN_REPETICAO = null;
                vm.MENS_NR_REPETICOES = 0;
                vm.MENS_IN_STATUS = 1;
                vm.MENS_IN_OCORRENCIAS = 1;
                vm.MENS_IN_ENVIADAS = 1;
                vm.MENS_IN_SISTEMA = 6;
                vm.EMFI_CD_ID = usuario.EMFI_CD_ID;
                vm.TIPO_ENVIO = 1;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirMensagemEMail(MensagemViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            List<PACIENTE> listaTotal = CarregaPaciente();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaTotal = listaTotal.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");

            List<GRUPO_PAC> listaTotal1 = CarregaGrupo();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                listaTotal1 = listaTotal1.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }
            listaTotal1 = listaTotal1.Where(p => p.GRUPO_PACIENTE.Count > 0).ToList();
            ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Mensagem"] = null;
            ViewBag.Temp = new SelectList(CarregarModeloEMail().Where(p => p.TEEM_IN_HTML == 1 & p.TEEM_IN_FIXO == 0).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
            ViewBag.Usuario = usuario;
            ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> tipoEnvio = new List<SelectListItem>();
            tipoEnvio.Add(new SelectListItem() { Text = "Selecionar Pacientes", Value = "1" });
            tipoEnvio.Add(new SelectListItem() { Text = "Todos os Pacientes", Value = "2" });
            ViewBag.TiposEnvio = new SelectList(tipoEnvio, "Value", "Text");
            List<SelectListItem> forma = new List<SelectListItem>();
            forma.Add(new SelectListItem() { Text = "Periodicidade", Value = "1" });
            forma.Add(new SelectListItem() { Text = "Dia do Mês", Value = "2" });
            forma.Add(new SelectListItem() { Text = "Dia da Semana", Value = "3" });
            ViewBag.Forma = new SelectList(forma, "Value", "Text");
            List<SelectListItem> semana = new List<SelectListItem>();
            semana.Add(new SelectListItem() { Text = "Domingo", Value = "0" });
            semana.Add(new SelectListItem() { Text = "2a Feira", Value = "1" });
            semana.Add(new SelectListItem() { Text = "3a feira", Value = "2" });
            semana.Add(new SelectListItem() { Text = "4a feira", Value = "3" });
            semana.Add(new SelectListItem() { Text = "5a feira", Value = "4" });
            semana.Add(new SelectListItem() { Text = "6a feira", Value = "5" });
            semana.Add(new SelectListItem() { Text = "Sábado", Value = "6" });
            ViewBag.Semana = new SelectList(semana, "Value", "Text");
            List<SelectListItem> aniv = new List<SelectListItem>();
            aniv.Add(new SelectListItem() { Text = "Aniversariantes do Dia", Value = "1" });
            aniv.Add(new SelectListItem() { Text = "Aniversariantes do Mês", Value = "2" });
            ViewBag.Aniversariantes = new SelectList(aniv, "Value", "Text");
            ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

            if (ModelState.IsValid)
            {
                try
                {
                    // Checa preenchimento
                    if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO) & vm.TEEM_CD_ID == null & vm.MENS_AQ_ARQUIVO == null)
                    {
                        Session["MensMensagem"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (vm.TIPO_ENVIO == null || vm.TIPO_ENVIO == 0)
                    {
                        vm.TIPO_ENVIO = 1;
                    }
                    if (vm.TIPO_ENVIO == 1)
                    {
                        if (vm.ID == null & vm.GRPA_CD_ID == null & vm.ANIVERSARIO == null)
                        {
                            Session["MensMensagem"] = 52;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                    }
                    if (vm.MENS_IN_TIPO_EMAIL == null || vm.MENS_IN_TIPO_EMAIL == 0)
                    {
                        vm.MENS_IN_TIPO_EMAIL = 1;
                    }
                    if (vm.ID > 0)
                    {
                        vm.GRPA_CD_ID = null;
                        vm.ANIVERSARIO = 0;
                    }
                    if (vm.GRPA_CD_ID > 0)
                    {
                        vm.ANIVERSARIO = 0;
                    }
                    if (vm.ANIVERSARIO > 0)
                    {
                        vm.GRPA_CD_ID = null;
                    }

                    // Checa agendamento
                    if (vm.MENS_DT_AGENDAMENTO != null)
                    {
                        if (vm.MENS_DT_AGENDAMENTO < DateTime.Today.Date.AddMinutes(30))
                        {
                            Session["MensMensagem"] = 72;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        vm.MENS_IN_AGENDAMENTO = 1;
                    }
                    if (vm.MENS_DT_AGENDAMENTO == null & (vm.MENS_NR_REPETICOES != null & vm.MENS_NR_REPETICOES > 0))
                    {
                        vm.MENS_DT_AGENDAMENTO = DateTime.Now;
                        vm.MENS_IN_AGENDAMENTO = 1;
                    }

                    // Verifica possibilidade
                    Int32 numBase = 0;
                    Int32 num = CarregaEMailDia().Count;
                    Int32 destinos = 0;

                    // Monta destinos
                    if (vm.TIPO_ENVIO == 1)
                    {
                        if (vm.ID > 0)
                        {
                            numBase = num + 1;
                            if ((Int32)Session["NumEMail"] <= numBase)
                            {
                                Session["MensEMail"] = 50;
                                return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                            }
                            destinos = 1;
                        }
                        else if (vm.GRPA_CD_ID > 0)
                        {
                            GRUPO_PAC grupo = gruApp.GetItemById((int)vm.GRPA_CD_ID);
                            Int32 numGrupo = grupo.GRUPO_PACIENTE.Count;
                            numBase = num + numGrupo;
                            if ((Int32)Session["NumEMail"] <= numBase)
                            {
                                Session["MensEMail"] = 50;
                                return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                            }
                            destinos = numGrupo;
                        }
                        else if (vm.ANIVERSARIO > 0)
                        {
                            List<PACIENTE> lista = CarregaPaciente();
                            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                            {
                                lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                            }

                            // Recupera aniversarios
                            DateTime hoje = DateTime.Today.Date;
                            if (vm.ANIVERSARIO == 1)
                            {
                                lista = lista.Where(p => p.PACI_DT_NASCIMENTO.Value.Day == hoje.Day & p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
                            }
                            else
                            {
                                lista = lista.Where(p => p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
                            }
                            Int32 quant = lista.Count;

                            numBase = num + quant;
                            if ((Int32)Session["NumEMail"] <= numBase)
                            {
                                Session["MensEMail"] = 50;
                                return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                            }
                            destinos = quant;
                        }
                    }
                    else
                    {
                        Int32 numPacientes = CarregaPaciente().Count();
                        numBase = num + numPacientes;
                        if ((Int32)Session["NumEMail"] <= numBase)
                        {
                            Session["MensEMail"] = 50;
                            return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
                        }
                        destinos = numPacientes;
                    }

                    // Prepara texto
                    String cabecalho = String.Empty;
                    String texto = String.Empty;
                    String rodape = String.Empty;
                    if (vm.MENS_AQ_ARQUIVO == null)
                    {
                        if (vm.TEEM_CD_ID == null)
                        {
                            vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("<p>", "");
                            vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("</p>", "<br />");
                            vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("<br>", "<br />");
                            vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("<p>", "");
                            vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("</p>", "<br />");
                            vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("<br>", "<br />");
                            vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("<p>", "");
                            vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("</p>", "<br />");
                            vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("<br>", "<br />");
                        }
                    }

                    // Prepara a operação
                    vm.MENS_GU_GUID = Xid.NewXid().ToString();
                    vm.MENS_ID_IDENTIFICADOR = Xid.NewXid().ToString();
                    MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
                    item.MENS_IN_DESTINOS = destinos;
                    item.MENS_IN_STATUS = 2;
                    item.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
                    item.EMFI_CD_ID = usuario.EMFI_CD_ID;
                    item.MENS_IN_SISTEMA = 6;
                    item.PACI_CD_ID = vm.ID;
                    Int32 volta = baseApp.ValidateCreate(item, usuario);
                    Session["IdMensagem"] = item.MENS_CD_ID;

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
                    String map = Server.MapPath(caminho);
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    if (Session["FileQueueMensagem"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMensagem"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueMensagem(file);
                            }
                        }
                        Session["FileQueueMensagem"] = null;
                    }

                    // Processa
                    MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
                    Session["IdMensagem"] = mens.MENS_CD_ID;
                    //MensagemViewModel vm1 = Mapper.Map<MENSAGENS, MensagemViewModel>(mens);
                    vm.MENS_CD_ID = mens.MENS_CD_ID;
                    vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
                    Int32 retGrava = ProcessarEnvioMensagemEMail(vm, usuario);

                    // Retornos de erros
                    if (retGrava == 1)
                    {
                        Session["MensMensagem"] = 51;
                        return RedirectToAction("MontarTelaMensagemEMail");
                    }

                    // Sucesso
                    Session["EMailPadraoAlterada"] = 1;
                    Session["MensagemAlterada"] = 1;
                    Session["ListaMensagemEMail"] = null;

                    // Acerta sessions
                    Int32 totMens = (Int32)Session["TotMens"];
                    Int32 enviado    = ((Int32)Session["EMailTotalEnvio"]) + totMens;
                    Int32 enviadoMes = ((Int32)Session["EMailTotalEnvioMes"]) + totMens;
                    Int32 enviadoDia = ((Int32)Session["EMailTotalEnvioDia"]) + totMens;
                    if (mens.MENS_IN_AGENDAMENTO == 1)
                    {
                        Int32 agenda = ((Int32)Session["EMailAgendaNum"]) + totMens;
                        Session["EMailAgendaNum"] = agenda;
                    }
                    Session["EMailTotalEnvio"] = enviado;
                    Session["EMailTotalEnvioMes"] = enviadoMes;
                    Session["EMailTotalEnvioDia"] = enviadoDia;

                    listaMaster = new List<MENSAGENS>();
                    Session["ListaMensagemEMail"] = null;
                    Session["MensagemNovo"] = item.MENS_CD_ID;

                    Session["ListaMensagemEMail"] = null;
                    Session["MensagemAlterada"] = 1;
                    Session["FlagAlteraEstado"] = 1;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "Foram enviadas/agendadas mensagens para " + totMens.ToString() + " destinatários";
                    Session["MensMensagem"] = 99;

                    return RedirectToAction("MontarTelaMensagemEMail");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Mensagens";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerEMailAgendados()
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

                // Carrega mensagens
                List<MENSAGENS> agSMS = CarregarMensagem().Where(p => p.MENS_NR_REPETICOES > 0 & p.MENS_IN_TIPO == 1).ToList();
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    agSMS = agSMS.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }

                // Verifica se já foi enviada
                List<MENSAGENS> final =  new List<MENSAGENS>();
                foreach (MENSAGENS item in agSMS)
                {
                    if (item.RECURSIVIDADE != null)
                    {
                        RECURSIVIDADE rec = item.RECURSIVIDADE.First();
                        Int32 destinos = rec.RECURSIVIDADE_DESTINO.Count;
                        List<RECURSIVIDADE_DATA> datas = rec.RECURSIVIDADE_DATA.Where(p => p.REDA_IN_PROCESSADA == 0 & p.REDA_IN_SISTEMA == 6).ToList();
                        if (datas.Count > 0)
                        {
                            foreach (RECURSIVIDADE_DATA data in datas)
                            {
                                MENSAGENS mensNova = new MENSAGENS();
                                mensNova.ASSI_CD_ID = item.ASSI_CD_ID;
                                mensNova.USUA_CD_ID = item.USUA_CD_ID;
                                mensNova.MENS_IN_TIPO_SMS = item.MENS_CD_ID;
                                mensNova.MENS_DT_CRIACAO = item.MENS_DT_CRIACAO;
                                mensNova.MENS_NM_NOME = item.MENS_NM_NOME;
                                mensNova.MENS_IN_AGENDAMENTO = destinos;
                                mensNova.MENS_DT_AGENDAMENTO = data.REDA_DT_PROGRAMADA;
                                mensNova.MENS_GU_GUID = item.MENS_GU_GUID;
                                final.Add(mensNova);
                            }
                        }
                    }
                }
                Session["EMailAgenda"] = final;

                MENSAGENS mens = new MENSAGENS();
                ViewBag.Listas = final;
                Session["VoltaMensagem"] = 11;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoMensagem(Int32 id)
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

                // Prepara view
                MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoMensagem()
        {

            return RedirectToAction("VerMensagem", new { id = (Int32)Session["IdMensagem"] });
        }

        public FileResult DownloadMensagem(Int32 id)
        {
            MENSAGEM_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.MEAN_AQ_ARQUIVO;
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

        [HttpGet]
        public ActionResult VerMensagemEMail(Int32 id)
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

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 40)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
                    }
                }

                // Recupera mensagem
                Session["IdMensagem"] = id;
                Session["VoltaMensagem"] = 1;
                MENSAGENS item = baseApp.GetItemById(id);
                MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
                vm.MENS_NM_CABECALHO = CrossCutting.HtmlToText.ExtractTextFromHtml(vm.MENS_NM_CABECALHO);
                vm.MENS_TX_TEXTO = CrossCutting.HtmlToText.ExtractTextFromHtml(vm.MENS_TX_TEXTO);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardMensagens()
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

                // Carrega numeros
                Int32 volta = CarregaNumeroMensagem();
                List<MENSAGENS> listaMens = (List<MENSAGENS>)Session["ListaEMailTudo"];

                // Resumo criação Data E-Mail
                List<MENSAGENS> listaMesEMail = (List<MENSAGENS>)Session["ListaMesEMail"];
                List<DateTime> datas = listaMesEMail.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
                if ((Int32)Session["FlagDataMens"] == 1 || Session["ListaEMailDataSaida"] == null)
                {
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                    List<ModeloViewModel> lista = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        Int32 conta = listaMesEMail.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList().Count;
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = item;
                        mod.Valor = conta;
                        lista.Add(mod);
                    }
                    ViewBag.ListaEMailData = lista;
                    ViewBag.ContaEMailData = lista.Count;
                    Session["ListaDatasEMail"] = datas;
                    Session["ListaEMailDataSaida"] = lista;
                }
                else
                {
                    ViewBag.ListaEMailData = (List<ModeloViewModel>)Session["ListaEMailDataSaida"];
                    ViewBag.ContaEMailData = ((List<ModeloViewModel>)Session["ListaEMailDataSaida"]).Count;
                }

                // Resumo Mes Criação
                DateTime limite = DateTime.Today.Date.AddMonths(-12);
                String mes = null;
                String mesFeito = null;
                if ((Int32)Session["FlagDataMens"] == 1 || Session["ListaEMailMesSaida"] == null)
                {
                    datas = listaMens.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
                    datas.Sort((i, j) => i.Date.CompareTo(j.Date));

                    List<ModeloViewModel> listaMes = new List<ModeloViewModel>();
                    foreach (DateTime item in datas)
                    {
                        if (item.Date > limite)
                        {
                            mes = item.Month.ToString() + "/" + item.Year.ToString();
                            if (mes != mesFeito)
                            {
                                Int32 conta = listaMens.Where(p => p.MENS_DT_CRIACAO.Value.Date.Month == item.Month & p.MENS_DT_CRIACAO.Value.Date.Year == item.Year & p.MENS_DT_CRIACAO > limite).Count();
                                ModeloViewModel mod = new ModeloViewModel();
                                mod.Nome = mes;
                                mod.Valor = conta;
                                listaMes.Add(mod);
                                mesFeito = item.Month.ToString() + "/" + item.Year.ToString();
                            }
                        }
                    }
                    ViewBag.ListaEMailMes = listaMes;
                    ViewBag.ContaEMailMes = listaMes.Count;
                    Session["ListaEMailMesSaida"] = listaMes;
                }
                else
                {
                    ViewBag.ListaEMailMes = (List<ModeloViewModel>)Session["ListaEMailMesSaida"];
                    ViewBag.ContaEMailMes = ((List<ModeloViewModel>)Session["ListaEMailMesSaida"]).Count;
                }

                // Resumo envios por data
                List<RECURSIVIDADE> listaRecursividades = (List<RECURSIVIDADE>)Session["Recursividades"];
                List<RECURSIVIDADE_DATA> listaRecursividadesDatas = recApp.GetAllDatas(idAss);
                List<RECURSIVIDADE_DATA> listaDatasVale = new List<RECURSIVIDADE_DATA>();
                Int32 hoje = 0;
                datas = listaMesEMail.Select(p => p.MENS_DT_CRIACAO.Value.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> listaEnvioData = new List<ModeloViewModel>();
                foreach (DateTime item in datas)
                {
                    Int32 conta = listaMesEMail.Where(p => p.MENS_DT_CRIACAO.Value.Date == item & p.MENS_DT_AGENDAMENTO == null).Sum(x => x.MENS_IN_DESTINOS.Value);
                    List<MENSAGENS> mens1 = listaMesEMail.Where(p => p.MENS_DT_AGENDAMENTO != null).ToList();                   
                    Int32 conta1 = mens1.Where(p => p.MENS_DT_AGENDAMENTO.Value.Date == item & p.MENS_IN_OCORRENCIAS == 1).Sum(x => x.MENS_IN_DESTINOS.Value);
                    Int32 conta2 = 0;

                    listaDatasVale = listaRecursividadesDatas.Where(p => p.REDA_DT_PROGRAMADA == item & p.REDA_IN_PROCESSADA == 1 & p.REDA_IN_SISTEMA == 6).ToList();
                    foreach (RECURSIVIDADE_DATA rec in listaDatasVale)
                    {
                        RECURSIVIDADE recx = recApp.GetItemById(rec.RECU_CD_ID);
                        conta2 += recx.RECURSIVIDADE_DESTINO.Count;
                    }

                    ModeloViewModel mod = new ModeloViewModel();
                    mod.DataEmissao = item;
                    mod.Valor = conta + conta1 + conta2;
                    listaEnvioData.Add(mod);

                    if (item.Date == DateTime.Today.Date)
                    {
                        hoje = conta + conta1 + conta2;
                    }
                }
                ViewBag.ListaEMailEnvioData = listaEnvioData;
                ViewBag.ContaEMailEnvioData = listaEnvioData.Count;
                ViewBag.EnvioHoje = hoje;
                Session["ListaEMailEnvioData"] = listaEnvioData;

                // Recupera Mensagens por situação
                List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = "Enviadas";
                mod1.Valor = (Int32)Session["EMailEnviado"];
                lista2.Add(mod1);
                mod1 = new ModeloViewModel();
                mod1.Nome = "Pendentes";
                mod1.Valor = (Int32)Session["EMailAguarda"];
                lista2.Add(mod1);
                ViewBag.ListaMensagemSituacao = lista2;
                Session["ListaMensagemSituacao"] = lista2;

                // Recupera Mensagens por tipo
                List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
                ModeloViewModel mod2 = new ModeloViewModel();
                mod2.Nome = "Normal";
                mod2.Valor = (Int32)Session["EMailNormal"];
                lista3.Add(mod2);
                mod2 = new ModeloViewModel();
                mod2.Nome = "Agendadas";
                mod2.Valor = (Int32)Session["EMailAgenda"];
                lista3.Add(mod2);
                mod2 = new ModeloViewModel();
                mod2.Nome = "Recursivas";
                mod2.Valor = (Int32)Session["EMailRecursiva"];
                lista3.Add(mod2);
                ViewBag.ListaMensagemTipo = lista3;
                Session["ListaMensagemTipo"] = lista3;

                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        public Int32 CarregaNumeroMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Recupera listas Mensagens
            List<MENSAGENS> lt = CarregarMensagem();

            // Recupera listas Mensagens - Email
            List<MENSAGENS> emails = lt.Where(p => p.MENS_IN_TIPO == 1).ToList();
            List<MENSAGENS> listaMesEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            List<MENSAGENS> listaDiaEMail = emails.Where(p => p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).ToList();

            List<MENSAGENS> emailsNormal= emails.Where(p => p.MENS_DT_AGENDAMENTO == null & p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsAguarda = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_ENVIADAS != p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsAgenda = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_STATUS != 4).ToList();
            List<MENSAGENS> emailsRecursiva = emails.Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_STATUS == 4).ToList();
            List<MENSAGENS> emailsEnviado = emails.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsFalha = emails.Where(p => p.MENS_IN_STATUS == 3).ToList();

            List<MENSAGENS> emailsEnviadoDia = listaDiaEMail.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();
            List<MENSAGENS> emailsEnviadoMes = listaMesEMail.Where(p => p.MENS_IN_ENVIADAS == p.MENS_IN_OCORRENCIAS).ToList();

            Int32 emailTot = emails.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 emailTotMes = listaMesEMail.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 emailTotDia = listaDiaEMail.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;

            Int32 aguarda = emailsAguarda.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 agenda = emailsAgenda.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 recursiva = emailsRecursiva.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviado = emailsEnviado.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 normal = emailsFalha.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 falha = emailsFalha.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviadoDia = emailsEnviadoDia.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;
            Int32 enviadoMes = emailsEnviadoMes.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;

            Int32? destinos = emails.Where(p => p.MENSAGENS_DESTINOS.Any(m => m.MEDE_IN_ATIVO == 1 & p.MENS_IN_SISTEMA == 2)).ToList().Count;


            // Viewbags
            ViewBag.Total = lt.Count;
            ViewBag.TotalEMails = emails.Count;

            ViewBag.EMailsAguarda = aguarda;
            ViewBag.EMailsEnviado = enviado;
            ViewBag.EMailsFalha = falha;
            ViewBag.EMailsMes= emailTotMes;
            ViewBag.EMailsDia = emailTotDia;
            ViewBag.EMailsAgenda = agenda;

            ViewBag.EMailsTotalEnvio = emailTot;
            ViewBag.EMailsTotalEnvioMes = enviadoMes;
            ViewBag.EMailsTotalEnvioDia = enviadoDia;

            ViewBag.EMailsTotalCriado = emails.Count;
            ViewBag.EMailsTotalCriadoMes = listaMesEMail.Count;
            ViewBag.EMailsTotalCriadoDia = listaDiaEMail.Count;

            ViewBag.Destinos = destinos;

            Session["EMailTotalEnvio"] = emailTot;
            Session["EMailTotalEnvioMes"] = emailTotMes;
            Session["EMailTotalEnvioDia"] = emailTotDia;
            Session["EMailAguarda"] = aguarda;
            Session["EMailAgenda"] = agenda;
            Session["EMailRecursiva"] = recursiva;
            Session["EMailFalha"] = falha;
            Session["EMailEnviado"] = enviado;
            Session["EMailnormal"] = enviado;
            Session["Destinos"] = destinos;

            Session["ListaMesEMail"] = listaMesEMail;
            Session["ListaEMailTudo"] = emails;
            Session["ListaDiaMail"] = listaDiaEMail;

            Session["ListaEMailAgenda"] = emailsAgenda;
            Session["ListaEMailAguarda"] = emailsAguarda;
            Session["ListaEMailRecursiva"] = emailsRecursiva;
            Session["ListaEMailEnviado"] = emailsEnviado;
            Session["ListaEMailFalha"] = emailsFalha;
            Session["MensagemCarregada"] = 1;
            return 0;
        }

        public JsonResult GetDadosGraficoTotal()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotalTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotalTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoTotalTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaTotal"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasTotal"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetAnivesario(Int32 id)
        {
            // Recupera pacientes
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<PACIENTE> lista = CarregaPaciente();
            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            {
                lista = lista.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
            }

            // Recupera aniversarios
            DateTime hoje = DateTime.Today.Date;
            if (id == 1)
            {
                lista = lista.Where(p => p.PACI_DT_NASCIMENTO.Value.Day == hoje.Day & p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
            }
            else
            {
                lista = lista.Where(p => p.PACI_DT_NASCIMENTO.Value.Month == hoje.Month).ToList();
            }
            Int32 quant = lista.Count;

            // Retorno
            var hash = new Hashtable();
            hash.Add("quant", quant);
            return Json(hash);
        }

        public JsonResult GetDadosGraficoEmail()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaEMailDataSaida"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosGraficoEmailMes()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaEMailMesSaida"];
            List<String> meses = new List<String>();
            List<Int32> valor = new List<Int32>();
            meses.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                meses.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("meses", meses);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoEmailEnvio()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaEMailEnvioData"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
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

        public JsonResult GetDadosGraficoFalhas()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalha"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalha"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoFalhasTodos()
        {
            List<MENSAGENS> listaCP1 = (List<MENSAGENS>)Session["ListaFalhaTodas"];
            List<DateTime> datas = (List<DateTime>)Session["ListaDatasFalhaTodas"];
            List<MENSAGENS> listaDia = new List<MENSAGENS>();
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (DateTime item in datas)
            {
                listaDia = listaCP1.Where(p => p.MENS_DT_CRIACAO.Value.Date == item).ToList();
                Int32 contaDia = listaDia.Count();
                dias.Add(item.ToShortDateString());
                valor.Add(contaDia);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public ActionResult MostrarPaciente()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 30;
            return RedirectToAction("MontarTelaCentralPaciente", "Paciente");
        }

        public ActionResult MostrarDashboard()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 5;
            return RedirectToAction("MontarTelaDashboardMensagens", "Mensagem");
        }

        public ActionResult MostrarMensagens()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 1;
            return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
        }

        public ActionResult MostrarGrupos()
        {
            // Prepara grid
            Session["VoltaMensagem"] = 40;
            return RedirectToAction("MontarTelaGrupo", "Grupo");
        }

        public List<MENSAGENS> CarregaEMailDia()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                List<MENSAGENS> conf = new List<MENSAGENS>();
                conf = baseApp.GetAllItens(idAss);
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    conf = conf.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                }
                conf = conf.Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO == DateTime.Today.Date).ToList();
                conf = conf.Where(p => p.MENS_IN_SISTEMA == 6).ToList();
                Session["EMailPadrao"] = conf;
                Session["EMailPadraoAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PERIODICIDADE_TAREFA> CarregaPeriodicidade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PERIODICIDADE_TAREFA> conf = new List<PERIODICIDADE_TAREFA>();
            if (Session["Periodicidades"] == null)
            {
                conf = periodicidadeApp.GetAllItens();
            }
            else
            {
                if ((Int32)Session["PeriodicidadeAlterada"] == 1)
                {
                    conf = periodicidadeApp.GetAllItens();
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

        [HttpGet]
        public ActionResult MontarTelaRecursivaEMail()
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
                Session["ModuloAtual"] = "Mensagens - Envios - Recursivos";

                // Carrega listas
                if (Session["ListaRecursividadeEMail"] == null)
                {
                    listaMasterRec = CarregaRecursividade().Where(p => p.RECU_IN_TIPO_MENSAGEM == 1).OrderByDescending(m => m.RECU_DT_CRIACAO).ToList();
                    Session["ListaRecursividadeEMail"] = listaMasterRec;
                }
                ViewBag.Listas = (List<RECURSIVIDADE>)Session["ListaRecursividadeEMail"];

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "MENSAGEM_EMAIL_RECURSIVA", "Mensagem", "MontarTelaRecursivaEMail");

                // Abre view
                Session["VoltaMensagem"] = 1;
                Session["MensMensagem"] = null;
                objetoRec = new RECURSIVIDADE();
                objetoRec.RECU_DT_CRIACAO = DateTime.Today.Date;
                return View(objetoRec);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarRecursividadeEMail(RECURSIVIDADE item)
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            try
            {
                // Executa a operação
                Tuple<Int32, List<RECURSIVIDADE>, Boolean> volta = recApp.ExecuteFilter(1, item.RECU_NM_NOME, item.RECU_DT_CRIACAO, item.RECU_DT_DUMMY, item.RECU_TX_TEXTO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensMensagem"] = 1;
                    return RedirectToAction("MontarTelaRecursivaEMail");
                }

                // Sucesso
                listaMasterRec = volta.Item2;
                Session["ListaRecursividadeEMail"] = listaMasterRec;
                return RedirectToAction("MontarTelaRecursivaEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroRecursividadeEMail()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaRecursividadeEMail"] = null;
                return RedirectToAction("MontarTelaRecursivaEMail");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerRecursividades(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                MENSAGENS mens = baseApp.GetItemById(id);
                List<RECURSIVIDADE> recs = mens.RECURSIVIDADE.Where(p => p.RECU_IN_TIPO_ENVIO == 1 & p.RECU_IN_SISTEMA == 6).ToList();
                RECURSIVIDADE item = recs.Where(p => p.MENS_CD_ID == id).FirstOrDefault();

                Session["IdRecursividade"] = item.RECU_CD_ID;
                ViewBag.Destinos = item.RECURSIVIDADE_DESTINO.ToList();
                ViewBag.Datas = item.RECURSIVIDADE_DATA.ToList();
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public List<RECURSIVIDADE> CarregaRecursividade()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<RECURSIVIDADE> conf = new List<RECURSIVIDADE>();
            if (Session["Recursividades"] == null)
            {
                conf = recApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["RecursividadeAlterada"] == 1)
                {
                    conf = recApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<RECURSIVIDADE>)Session["Recursividades"];
                }
            }
            conf = conf.Where(p => p.RECU_IN_SISTEMA == 6).ToList();
            Session["RecursividadeAlterada"] = 0;
            Session["Recursividades"] = conf;
            return conf;
        }

        public List<MENSAGENS_ENVIADAS_SISTEMA> CarregarMensagemEnviada()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS_ENVIADAS_SISTEMA> conf = new List<MENSAGENS_ENVIADAS_SISTEMA>();
            if (Session["Enviadas"] == null)
            {
                conf = mesApp.GetAllItens(idAss);
            }
            else
            {
                conf = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["Enviadas"];
            }
            conf = conf.Where(p => p.MEEN_IN_SISTEMA == 6).ToList();
            Session["Enviadas"] = conf;
            return conf;
        }

        public List<MENSAGENS> CarregarMensagem()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<MENSAGENS> conf = new List<MENSAGENS>();
            if (Session["Mensagens"] == null)
            {
                conf = baseApp.GetAllItens(idAss);
            }
            else
            {
                if ((Int32)Session["MensagemAlterada"] == 1)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<MENSAGENS>)Session["Mensagens"];
                }
            }
            conf = conf.Where(p => p.MENS_IN_SISTEMA == 6).ToList();
            Session["MensagemAlterada"] = 0;
            Session["Mensagens"] = conf;
            return conf;
        }

        public JsonResult GetDadosGraficoMensagemTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaMensagemTipo"];
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
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


        public JsonResult GetDadosGraficoMensagemSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaMensagemSituacao"];
            String[] cores = CrossCutting.UtilitariosGeral.GetListaCores();
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
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

        public List<RESULTADO_ROBOT> CarregarEnvios()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<RESULTADO_ROBOT> conf = new List<RESULTADO_ROBOT>();
            conf = baseApp.GetAllEnviosRobot(idAss);
            return conf;
        }

        [NonAction]
        public List<SelectListItem> CarregarModelosHtml()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            String caminho = "/TemplateEMail/Modelos/" + idAss.ToString() + "/";
            String path = Path.Combine(Server.MapPath(caminho));
            String[] files = Directory.GetFiles(path, "*.html");
            List<SelectListItem> mod = new List<SelectListItem>();
            foreach (String file in files)
            {
                mod.Add(new SelectListItem() { Text = System.IO.Path.GetFileNameWithoutExtension(file), Value = file });
            }

            return mod;
        }

        public ActionResult MontarTelaMensagem()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_MENSAGEM == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Mensageria";
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega Mensagens
                if (Session["ListaMensagemEnviada"] == null)
                {
                    listaEnviadas = CarregarMensagemEnviada().ToList();
                    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                    {
                        listaEnviadas = listaEnviadas.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    }
                    listaEnviadas = listaEnviadas.Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Year).ToList();
                    Session["ListaMensagemEnviada"] = listaEnviadas;
                }

                // Emitente
                String classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                String nomeMedico = usuario.USUA_NM_NOME;
                if (usuario.USUA_NM_PREFIXO != null)
                {
                    nomeMedico = usuario.USUA_NM_PREFIXO + " " + nomeMedico;
                }
                if (usuario.USUA_NM_SUFIXO != null)
                {
                    nomeMedico = nomeMedico + " " + usuario.USUA_NM_SUFIXO;
                }
                ViewBag.Nome = nomeMedico;
                ViewBag.Classe = classe;
                ViewBag.Mail = usuario.USUA_NM_EMAIL;
                ViewBag.Celular = usuario.USUA_NR_CELULAR;
                ViewBag.Whats = usuario.USUA_NR_WHATSAPP;

                // Separa listas
                listaEnviadas = (List<MENSAGENS_ENVIADAS_SISTEMA>)Session["ListaMensagemEnviada"];
                ViewBag.Listas = listaEnviadas;
                List<MENSAGENS_ENVIADAS_SISTEMA> mails = listaEnviadas.Where(p => p.MEEN_IN_TIPO == 1).OrderByDescending(p => p.MEEN_DT_DATA_ENVIO).ToList();
                List<MENSAGENS_ENVIADAS_SISTEMA> sms = listaEnviadas.Where(p => p.MEEN_IN_TIPO == 2).OrderByDescending(p => p.MEEN_DT_DATA_ENVIO).ToList();
                List<MENSAGENS_ENVIADAS_SISTEMA> whas = listaEnviadas.Where(p => p.MEEN_IN_TIPO == 3).OrderByDescending(p => p.MEEN_DT_DATA_ENVIO).ToList();
                ViewBag.Mails = mails;
                ViewBag.SMS = sms;
                ViewBag.WhatsApp = whas;
                ViewBag.TotalMails = mails.Count;
                ViewBag.TotalSMS = sms.Count;
                ViewBag.TotalWhatsApp = whas.Count;

                // Carrega listas de seleção
                ViewBag.Pacientes = new SelectList(CarregaPaciente().OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");
                List<SelectListItem> envio = new List<SelectListItem>();
                envio.Add(new SelectListItem() { Text = "Envio", Value = "2" });
                envio.Add(new SelectListItem() { Text = "Recebimento", Value = "1" });
                ViewBag.Envio = new SelectList(envio, "Value", "Text");

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensMensagem"] != null)
                {
                    if ((Int32)Session["MensMensagem"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                Session["MensMensagem"] = null;
                objetoEnviada = new MENSAGENS_ENVIADAS_SISTEMA();
                objetoEnviada.MEEN_DT_DATA_ENVIO = DateTime.Today.Date;
                objetoEnviada.MEEN_DT_DUMMY = DateTime.Today.Date;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensageria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Pacientes";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Pacientes", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<GRUPO_PAC> CarregaGrupo()
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

        public List<TEMPLATE_EMAIL> CarregarModeloEMail()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TEMPLATE_EMAIL> conf = new List<TEMPLATE_EMAIL>();
                if (Session["ModeloEMails"] == null)
                {
                    conf = temaApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ModeloEMailAlterada"] == 1)
                    {
                        conf = temaApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TEMPLATE_EMAIL>)Session["ModeloEMails"];
                    }
                }
                conf = conf.Where(p => p.TEEM_IN_SISTEMA == 6 ).ToList();
                conf = conf.Where(p => p.TEEM_IN_ROBOT == 0).ToList();
                Session["ModeloEMailAlterada"] = 0;
                Session["ModeloEMails"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Modelos E-Mails";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Modelos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult IncluirPacienteForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["VoltaCliente"] = 66;
            return RedirectToAction("IncluirPaciente", "Paciente");
        }

        public ActionResult VerTodosMensagemDia()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaEMailDataSaida"];
                Session["AbaDash"] = 4;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagem";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagem", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerTodosMensagemEnvioDia()
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaEMailEnvioData"];
                Session["AbaDash"] = 4;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Mensagem";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagem", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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

        //[HttpGet]
        //public ActionResult MontarTelaMensagemSMS()
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //            if (usuario.PERFIL.PERF_IN_ACESSO_MENSAGEM == 0)
        //            {
        //                Session["MensPermissao"] = 2;
        //                Session["ModuloPermissao"] = "Mensageria";
        //                return RedirectToAction("MontarTelaPaciente", "Paciente");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Carrega estatisticas de mensagens
        //        Int32 volta = CarregaNumeroMensagem();

        //        // Carrega listas
        //        if (Session["ListaMensagemSMS"] == null)
        //        {
        //            listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 2 & p.MENS_DT_CRIACAO.Value.Date == DateTime.Today.Date).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
        //            if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //            {
        //                listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //            }
        //            Session["ListaMensagemSMS"] = listaMaster;
        //        }
        //        ViewBag.Listas = (List<MENSAGENS>)Session["ListaMensagemSMS"];
        //        Session["Mensagem"] = null;
        //        Session["IncluirMensagem"] = 0;

        //        ViewBag.SMSTotalEnvio = (Int32)Session["SMSTotalEnvio"];
        //        ViewBag.SMSTotalEnvioMes = (Int32)Session["SMSTotalEnvioMes"];
        //        ViewBag.SMSTotalEnvioDia = (Int32)Session["SMSTotalEnvioDia"];

        //        // Indicadores
        //        ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

        //        // Mensagens
        //        if (Session["MensMensagem"] != null)
        //        {
        //            if ((Int32)Session["MensMensagem"] == 1)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 2)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 51)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0054", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 40)
        //            {
        //                String frase = CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture);
        //                frase += " - " + (String)Session["CliCRM"];
        //                ModelState.AddModelError("", frase);
        //            }
        //            if ((Int32)Session["MensMensagem"] == 50)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0260", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 51)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0252", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 60)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 61)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0065", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 51)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0258", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 80)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0269", CultureInfo.CurrentCulture));
        //            }
        //        }

        //        // Abre view
        //        Session["VoltaMensagem"] = 1;
        //        Session["MensMensagem"] = null;
        //        Session["VoltaRec"] = 1;
        //        objeto = new MENSAGENS();
        //        objeto.MENS_DT_ENVIO = DateTime.Today.Date.AddDays(-5);
        //        objeto.MENS_DT_AGENDAMENTO = DateTime.Today.Date;
        //        return View(objeto);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public ActionResult RetirarFiltroMensagemEMail()
        //{
        //    try
        //    {
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        Session["ListaMensagemEMail"] = null;
        //        Session["FiltroMensagem"] = null;
        //        return RedirectToAction("MontarTelaMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public ActionResult MostrarMesAtualMensagemEMail()
        //{
        //    try
        //    {
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        USUARIO usuario = (USUARIO)Session["UserCredentials"];

        //        listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        Session["ListaMensagemEMail"] = listaMaster;
        //        return RedirectToAction("MontarTelaMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public ActionResult MostrarTudoMensagemEMail()
        //{
        //    try
        //    {
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        USUARIO usuario = (USUARIO)Session["UserCredentials"];

        //        listaMaster = baseApp.GetAllItensAdm(idAss).Where(p => p.MENS_IN_TIPO == 1 & p.MENS_IN_SISTEMA == 6 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        Session["ListaMensagem"] = listaMaster;
        //        return RedirectToAction("MontarTelaMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public ActionResult MostrarMesesMensagemEMail()
        //{
        //    try
        //    {
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];
        //        USUARIO usuario = (USUARIO)Session["UserCredentials"];

        //        listaMaster = CarregarMensagem().Where(p => p.MENS_IN_TIPO == 1).OrderByDescending(m => m.MENS_DT_CRIACAO).ToList();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        Session["ListaMensagemEMail"] = listaMaster;
        //        return RedirectToAction("MontarTelaMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //[HttpPost]
        //public ActionResult FiltrarMensagemEMail(MENSAGENS item)
        //{

        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //    try
        //    {
        //        // Executa a operação
        //        List<MENSAGENS> listaObj = new List<MENSAGENS>();
        //        Session["FiltroMensagem"] = item;
        //        Tuple<Int32, List<MENSAGENS>, Boolean> volta = baseApp.ExecuteFilterEMail(item.MENS_DT_ENVIO.Value, item.MENS_DT_AGENDAMENTO.Value, 0, item.MENS_TX_TEXTO, idAss);

        //        // Verifica retorno
        //        if (volta.Item1 == 1)
        //        {
        //            Session["MensMensagem"] = 1;
        //            return RedirectToAction("MontarTelaMensagemEmail");
        //        }

        //        // Sucesso
        //        listaMaster = volta.Item2;
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaMaster = listaMaster.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        Session["ListaMensagemEMail"] = listaMaster;
        //        return RedirectToAction("MontarTelaMensagemEmail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public ActionResult VoltarBaseMensagemEMail()
        //{
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    return RedirectToAction("MontarTelaMensagemEMail");
        //}

        //        public ActionResult VoltarAnexoMensagemEMail()
        //{
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    Int32 volta = (Int32)Session["VoltaMensagem"];
        //    if (volta == 1)
        //    {
        //        return RedirectToAction("MontarTelaMensagemEMail");
        //    }
        //    else if (volta == 2)
        //    {
        //        return RedirectToAction("VoltarAnexoPaciente", "Paciente");
        //    }
        //    else if (volta == 3)
        //    {
        //        return RedirectToAction("MontarTelaPaciente", "Paciente");
        //    }
        //    else if (volta == 11)
        //    {
        //        return RedirectToAction("VerEMailAgendados", "Mensagem");
        //    }
        //    return RedirectToAction("MontarTelaMensagemEMail");
        //}

        //[HttpGet]
        //public ActionResult ExcluirMensagemEMail(Int32 id)
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((Int32)Session["PermMens"] == 0)
        //        {
        //            Session["MensPermissao"] = 2;
        //            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        MENSAGENS item = baseApp.GetItemById(id);
        //        item.MENS_IN_ATIVO = 0;
        //        Int32 volta = baseApp.ValidateDelete(item, usuario);
        //        Session["ListaMensagem"] = null;
        //        Session["MensagemAlterada"] = 1;
        //        Session["FlagAlteraEstado"] = 1;
        //        return RedirectToAction("VoltarBaseMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensagens";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //[HttpGet]
        //public ActionResult ReativarMensagemEMail(Int32 id)
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((Int32)Session["PermMens"] == 0)
        //        {
        //            Session["MensPermissao"] = 2;
        //            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        MENSAGENS item = baseApp.GetItemById(id);
        //        item.MENS_IN_ATIVO = 1;
        //        Int32 volta = baseApp.ValidateReativar(item, usuario);
        //        Session["ListaMensagem"] = null;
        //        Session["MensagemAlterada"] = 1;
        //        Session["FlagAlteraEstado"] = 1;
        //        return RedirectToAction("VoltarBaseMensagemEMail");
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensagens";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //public JsonResult PesquisaTemplateEMail(String temp)
        //{
        //    var hash = new Hashtable();
        //    if (!string.IsNullOrEmpty(temp))
        //    {
        //        // Recupera Template
        //        TEMPLATE_EMAIL tmp = temaApp.GetItemById(Convert.ToInt32(temp));

        //        // Atualiza
        //        hash.Add("TEEM_TX_CORPO", tmp.TEEM_TX_CORPO);
        //        hash.Add("TEEM_TX_CABECALHO", tmp.TEEM_TX_CABECALHO);
        //        hash.Add("TEEM_TX_DADOS", tmp.TEEM_TX_DADOS);
        //    }

        //    // Retorna
        //    return Json(hash);
        //}

        //        [HttpGet]
        //[ValidateInput(false)]
        //public ActionResult IncluirMensagemEMail()
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //            if ((Int32)Session["PermMensageria"] == 0)
        //            {
        //                Session["MensPermissao"] = 2;
        //                Session["ModuloPermissao"] = "Mensageria";
        //                return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        //            }
        //            if (usuario.PERFIL.PERF_IN_INCLUSAO_MENSAGEM == 0)
        //            {
        //                Session["MensPermissao"] = 2;
        //                Session["ModuloPermissao"] = "E-Mail - Criação";
        //                return RedirectToAction("MontarTelaMensagemEMail");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Verifica possibilidade
        //        Int32 num = CarregarMensagem().Where(p => p.MENS_IN_TIPO.Value == 1 & p.MENS_DT_CRIACAO.Value.Month == DateTime.Today.Date.Month & p.MENS_DT_CRIACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
        //        if ((Int32)Session["NumEMail"] <= num)
        //        {
        //            Session["MensMensagem"] = 51;
        //            return RedirectToAction("VoltarBaseMensagemEMail");
        //        }

        //        // Prepara listas   
        //        List<PACIENTE> listaTotal = CarregaPaciente();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaTotal = listaTotal.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");

        //        List<GRUPO_PAC> listaTotal1 = CarregaGrupo();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            listaTotal1 = listaTotal1.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }
        //        listaTotal1 = listaTotal1.Where(p => p.GRUPO_PACIENTE.Count > 0).ToList();
        //        ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");

        //        Session["Mensagem"] = null;
        //        ViewBag.Temp = new SelectList(CarregarModeloEMail().Where(p => p.TEEM_IN_HTML == 1).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
        //        ViewBag.Usuario = usuario;

        //        ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
        //        List<SelectListItem> tipo = new List<SelectListItem>();
        //        tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
        //        tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
        //        ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

        //        ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

        //        // Mensagens
        //        if (Session["MensMensagem"] != null)
        //        {
        //            if ((Int32)Session["MensMensagem"] == 2)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0026", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 3)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 52)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
        //            }
        //            if ((Int32)Session["MensMensagem"] == 72)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
        //            }
        //        }

        //        // Prepara view
        //        Session["EMailAgendaNum"] = 0;
        //        Session["MensagemNovo"] = 0;
        //        MENSAGENS item = new MENSAGENS();
        //        MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
        //        vm.ASSI_CD_ID = idAss;
        //        vm.MENS_DT_CRIACAO = DateTime.Now;
        //        vm.MENS_IN_ATIVO = 1;
        //        vm.USUA_CD_ID = usuario.USUA_CD_ID;
        //        vm.MENS_IN_TIPO = 1;
        //        vm.MENS_TX_TEXTO = null;
        //        vm.ID = 0;
        //        vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
        //        vm.MENS_IN_REPETICAO = null;
        //        vm.MENS_NR_REPETICOES = 0;
        //        vm.MENS_IN_STATUS = 1;
        //        vm.MENS_IN_OCORRENCIAS = 1;
        //        vm.MENS_IN_ENVIADAS = 1;
        //        vm.MENS_IN_SISTEMA = 6;
        //        vm.EMFI_CD_ID = usuario.EMFI_CD_ID;
        //        return View(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult IncluirMensagemEMail(MensagemViewModel vm)
        //{
        //    if ((String)Session["Ativa"] == null)
        //    {
        //        return RedirectToAction("Logout", "ControleAcesso");
        //    }
        //    Int32 idAss = (Int32)Session["IdAssinante"];
        //    USUARIO usuario = (USUARIO)Session["UserCredentials"];

        //    List<PACIENTE> listaTotal = CarregaPaciente();
        //    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //    {
        //        listaTotal = listaTotal.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //    }
        //    ViewBag.Clientes = new SelectList(listaTotal.OrderBy(p => p.PACI_NM_NOME), "PACI__CD_ID", "PACI_NM_NOME");

        //    List<GRUPO_PAC> listaTotal1 = CarregaGrupo();
        //    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //    {
        //        listaTotal1 = listaTotal1.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //    }
        //    listaTotal1 = listaTotal1.Where(p => p.GRUPO_PACIENTE.Count > 0).ToList();
        //    ViewBag.Grupos = new SelectList(listaTotal1.OrderBy(p => p.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
        //    Session["Mensagem"] = null;
        //    ViewBag.Temp = new SelectList(CarregarModeloEMail().Where(p => p.TEEM_IN_HTML == 1).OrderBy(p => p.TEEM_NM_NOME), "TEEM_CD_ID", "TEEM_NM_NOME");
        //    ViewBag.Usuario = usuario;
        //    ViewBag.Modelos = new SelectList(CarregarModelosHtml(), "Value", "Text");
        //    List<SelectListItem> tipo = new List<SelectListItem>();
        //    tipo.Add(new SelectListItem() { Text = "Normal", Value = "1" });
        //    tipo.Add(new SelectListItem() { Text = "Arquivo HTML", Value = "2" });
        //    ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
        //    ViewBag.Periodicidade = new SelectList(CarregaPeriodicidade().OrderBy(p => p.PETA_NR_DIAS), "PETA_CD_ID", "PETA_NM_NOME");

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Checa preenchimento
        //            if (String.IsNullOrEmpty(vm.MENS_TX_TEXTO) & vm.TEEM_CD_ID == null & vm.MENS_AQ_ARQUIVO == null)
        //            {
        //                Session["MensMensagem"] = 3;
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0250", CultureInfo.CurrentCulture));
        //                return View(vm);
        //            }
        //            if (vm.ID == null & vm.GRUP_CD_ID == null)
        //            {
        //                Session["MensMensagem"] = 52;
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0259", CultureInfo.CurrentCulture));
        //                return View(vm);
        //            }

        //            // Checa agendamento
        //            if (vm.MENS_DT_AGENDAMENTO != null)
        //            {
        //                if (vm.MENS_DT_AGENDAMENTO < DateTime.Today.Date.AddMinutes(30))
        //                {
        //                    Session["MensMensagem"] = 72;
        //                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0263", CultureInfo.CurrentCulture));
        //                    return View(vm);
        //                }
        //                vm.MENS_IN_AGENDAMENTO = 1;
        //            }
        //            else
        //            {
        //                vm.MENS_DT_AGENDAMENTO = DateTime.Now;
        //                vm.MENS_IN_AGENDAMENTO = 0;
        //                vm.MENS_NR_REPETICOES = 0;
        //            }

        //            // Verifica possibilidade
        //            Int32 numBase = 0;
        //            Int32 num = CarregaEMailDia().Count;
        //            Int32 destinos = 0;

        //            // Monta destinos
        //            if (vm.ID > 0)
        //            {
        //                numBase = num + 1;
        //                if ((Int32)Session["NumEMail"] <= numBase)
        //                {
        //                    Session["MensEMail"] = 50;
        //                    return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
        //                }
        //                destinos = 1;
        //            }
        //            else if (vm.GRUP_CD_ID > 0)
        //            {
        //                GRUPO_PAC grupo = gruApp.GetItemById((int)vm.GRUP_CD_ID);
        //                Int32 numGrupo = grupo.GRUPO_PACIENTE.Count;
        //                numBase = num + numGrupo;
        //                if ((Int32)Session["NumEMail"] <= numBase)
        //                {
        //                    Session["MensEMail"] = 50;
        //                    return RedirectToAction("MontarTelaMensagemEMail", "Mensagem");
        //                }
        //                destinos = numGrupo;
        //            }

        //            // Prepara texto
        //            String cabecalho = String.Empty;
        //            String texto = String.Empty;
        //            String rodape = String.Empty;
        //            if (vm.MENS_AQ_ARQUIVO == null)
        //            {
        //                if (vm.TEEM_CD_ID == null)
        //                {
        //                    vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("<p>", "");
        //                    vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("</p>", "<br />");
        //                    vm.MENS_NM_CABECALHO = vm.MENS_NM_CABECALHO.Replace("<br>", "<br />");
        //                    vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("<p>", "");
        //                    vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("</p>", "<br />");
        //                    vm.MENS_TX_TEXTO = vm.MENS_TX_TEXTO.Replace("<br>", "<br />");
        //                    vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("<p>", "");
        //                    vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("</p>", "<br />");
        //                    vm.MENS_NM_RODAPE = vm.MENS_NM_RODAPE.Replace("<br>", "<br />");
        //                }
        //            }

        //            // Prepara a operação
        //            vm.MENS_GU_GUID = Xid.NewXid().ToString();
        //            vm.MENS_ID_IDENTIFICADOR = Xid.NewXid().ToString();
        //            MENSAGENS item = Mapper.Map<MensagemViewModel, MENSAGENS>(vm);
        //            item.MENS_IN_DESTINOS = destinos;
        //            item.MENS_IN_STATUS = 2;
        //            item.EMPR_CD_ID = (Int32)Session["IdEmpresa"];
        //            item.EMFI_CD_ID = usuario.EMFI_CD_ID;
        //            item.MENS_IN_SISTEMA = 6;
        //            Int32 volta = baseApp.ValidateCreate(item, usuario);
        //            Session["IdMensagem"] = item.MENS_CD_ID;

        //            // Cria pastas
        //            String caminho = "/Imagens/" + idAss.ToString() + "/Mensagem/" + item.MENS_CD_ID.ToString() + "/Anexos/";
        //            String map = Server.MapPath(caminho);
        //            Directory.CreateDirectory(Server.MapPath(caminho));

        //            // Trata anexos
        //            if (Session["FileQueueMensagem"] != null)
        //            {
        //                List<FileQueue> fq = (List<FileQueue>)Session["FileQueueMensagem"];
        //                foreach (var file in fq)
        //                {
        //                    if (file.Profile == null)
        //                    {
        //                        UploadFileQueueMensagem(file);
        //                    }
        //                }
        //                Session["FileQueueMensagem"] = null;
        //            }

        //            // Processa
        //            MENSAGENS mens = baseApp.GetItemById(item.MENS_CD_ID);
        //            Session["IdMensagem"] = mens.MENS_CD_ID;
        //            vm.MENS_CD_ID = mens.MENS_CD_ID;
        //            vm.MENSAGEM_ANEXO = mens.MENSAGEM_ANEXO;
        //            Int32 retGrava = ProcessarEnvioMensagemEMail(vm, usuario);

        //            // Retornos de erros
        //            if (retGrava == 1)
        //            {
        //                Session["MensMensagem"] = 51;
        //                return RedirectToAction("MontarTelaMensagemEMail");
        //            }

        //            // Sucesso
        //            Session["EMailPadraoAlterada"] = 1;

        //            // Acerta sessions
        //            Int32 totMens = (Int32)Session["TotMens"];
        //            Int32 enviado = ((Int32)Session["EMailTotalEnvio"]) + totMens;
        //            Int32 enviadoMes = ((Int32)Session["EMailTotalEnvioMes"]) + totMens;
        //            Int32 enviadoDia = ((Int32)Session["EMailTotalEnvioDia"]) + totMens;
        //            if (mens.MENS_IN_AGENDAMENTO == 1)
        //            {
        //                Int32 agenda = ((Int32)Session["EMailAgendaNum"]) + totMens;
        //                Session["EMailAgendaNum"] = agenda;
        //            }
        //            Session["EMailTotalEnvio"] = enviado;
        //            Session["EMailTotalEnvioMes"] = enviadoMes;
        //            Session["EMailTotalEnvioDia"] = enviadoDia;

        //            listaMaster = new List<MENSAGENS>();
        //            Session["ListaMensagemEMail"] = null;
        //            Session["MensagemNovo"] = item.MENS_CD_ID;

        //            Session["ListaMensagemEMail"] = null;
        //            Session["MensagemAlterada"] = 1;
        //            Session["FlagAlteraEstado"] = 1;
        //            return RedirectToAction("MontarTelaMensagemEMail");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = ex.Message;
        //            Session["TipoVolta"] = 2;
        //            Session["VoltaExcecao"] = "Mensagens";
        //            Session["Excecao"] = ex;
        //            Session["ExcecaoTipo"] = ex.GetType().ToString();
        //            GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //            Int32 voltaX = grava.GravarLogExcecao(ex, "Mensagens", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
        //            return RedirectToAction("TrataExcecao", "BaseAdmin");
        //        }
        //    }
        //    else
        //    {
        //        return View(vm);
        //    }
        //}

        //[HttpGet]
        //public ActionResult VerEMailAgendados()
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Carrega mensagens
        //        List<MENSAGENS> agSMS = CarregarMensagem().Where(p => p.MENS_DT_AGENDAMENTO != null & p.MENS_IN_TIPO == 1).ToList();
        //        if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
        //        {
        //            agSMS = agSMS.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
        //        }

        //        // Verifica se já foi enviada
        //        List<MENSAGENS> final =  new List<MENSAGENS>();
        //        foreach (MENSAGENS item in agSMS)
        //        {
        //            RECURSIVIDADE rec = item.RECURSIVIDADE.First();
        //            Int32 destinos = rec.RECURSIVIDADE_DESTINO.Count;
        //            List<RECURSIVIDADE_DATA> datas = rec.RECURSIVIDADE_DATA.Where(p => p.REDA_IN_PROCESSADA == 0).ToList();
        //            if (datas.Count > 0)
        //            {
        //                foreach (RECURSIVIDADE_DATA data in datas)
        //                {
        //                    MENSAGENS mensNova = new MENSAGENS();
        //                    mensNova.ASSI_CD_ID = item.ASSI_CD_ID;
        //                    mensNova.USUA_CD_ID = item.USUA_CD_ID;
        //                    mensNova.MENS_IN_TIPO_SMS = item.MENS_CD_ID;
        //                    mensNova.MENS_DT_CRIACAO = item.MENS_DT_CRIACAO;
        //                    mensNova.MENS_NM_NOME = item.MENS_NM_NOME;
        //                    mensNova.MENS_IN_AGENDAMENTO = destinos;
        //                    mensNova.MENS_DT_AGENDAMENTO = data.REDA_DT_PROGRAMADA;
        //                    mensNova.MENS_GU_GUID = item.MENS_GU_GUID;
        //                    final.Add(mensNova);
        //                }
        //            }
        //        }
        //        Session["EMailAgenda"] = final;

        //        MENSAGENS mens = new MENSAGENS();
        //        ViewBag.Listas = final;
        //        Session["VoltaMensagem"] = 11;
        //        return View(mens);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        //[HttpGet]
        //public ActionResult VerMensagemEMail(Int32 id)
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        USUARIO usuario = new USUARIO();
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        if ((USUARIO)Session["UserCredentials"] != null)
        //        {
        //            usuario = (USUARIO)Session["UserCredentials"];

        //            // Verfifica permissão
        //        }
        //        else
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Mensagens
        //        if (Session["MensMensagem"] != null)
        //        {
        //            if ((Int32)Session["MensMensagem"] == 40)
        //            {
        //                ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0034", CultureInfo.CurrentCulture));
        //            }
        //        }

        //        // Recupera mensagem
        //        Session["IdMensagem"] = id;
        //        Session["VoltaMensagem"] = 1;
        //        MENSAGENS item = baseApp.GetItemById(id);
        //        MensagemViewModel vm = Mapper.Map<MENSAGENS, MensagemViewModel>(item);
        //        vm.MENS_NM_CABECALHO = CrossCutting.HtmlToText.ExtractTextFromHtml(vm.MENS_NM_CABECALHO);
        //        vm.MENS_TX_TEXTO = CrossCutting.HtmlToText.ExtractTextFromHtml(vm.MENS_TX_TEXTO);
        //        return View(vm);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Mensageria";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Mensageria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

    }
}