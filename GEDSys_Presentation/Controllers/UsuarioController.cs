using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using System.Text.RegularExpressions;
using System.Reflection;
using ERP_Condominios_Solution.Classes;
using System.Threading.Tasks;
using XidNet;
using GEDSys_Presentation.App_Start;
using System.Net.Mime;


namespace ERP_Condominios_Solution.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IEmpresaAppService empApp;
        private readonly IPerfilAppService perfApp;
        private readonly ICategoriaUsuarioAppService cusApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly ITemplateEMailAppService temApp;

#pragma warning disable CS0169 // O campo "UsuarioController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "UsuarioController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "UsuarioController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "UsuarioController.exception" nunca é usado
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        LOG_EXCECAO_NOVO objLogExc = new LOG_EXCECAO_NOVO();
        LOG_EXCECAO_NOVO objLogExcAntes = new LOG_EXCECAO_NOVO();
        List<LOG_EXCECAO_NOVO> listaMasterLogExc = new List<LOG_EXCECAO_NOVO>();
        INDICACAO objetoInd = new INDICACAO();
        INDICACAO objetoIndAntes = new INDICACAO();
        List<INDICACAO> listaMasterInd = new List<INDICACAO>();
        String extensao;

        public UsuarioController(IUsuarioAppService baseApps, ILogAppService logApps, IConfiguracaoAppService confApps, IMensagemEnviadaSistemaAppService meApps, IEmpresaAppService empApps, IPerfilAppService perfApps, ICategoriaUsuarioAppService cusApps, IAcessoMetodoAppService aceApps, IRecursividadeAppService recuApps, ITemplateEMailAppService temApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            confApp = confApps;
            meApp = meApps;
            empApp = empApps;
            perfApp = perfApps;
            cusApp = cusApps;
            aceApp = aceApps;
            recuApp = recuApps;
            temApp = temApps;
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
        public ActionResult MontarTelaUsuario()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_USUARIO == 0)
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
                Session["ModuloAtual"] = "Usuários";

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Carrega listas
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Cats = new SelectList(CarregaCategoriaUsuario(), "CAUS_CD_ID", "CAUS_NM_NOME");

                // Carrega listas
                if ((List<USUARIO>)Session["ListaUsuario"] == null)
                {
                    listaMaster = CarregaUsuario();
                    Session["ListaUsuario"] = listaMaster;
                    Session["FiltroUsuario"] = null;
                }
                List<USUARIO> listaUsu = (List<USUARIO>)Session["ListaUsuario"];
                ViewBag.Listas = listaUsu;
                ViewBag.Usuarios = listaUsu.Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                ViewBag.UsuariosBloqueados = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList().Count;
                ViewBag.UsuariosHoje = listaUsu.Where(p => p.USUA_IN_BLOQUEADO == 0 && p.USUA_DT_ACESSO.Value.Date == DateTime.Today.Date).ToList().Count;
                ViewBag.Title = "Usuários";

                // Recupera numero de usuarios do assinante
                Session["NumeroUsuarios"] = listaUsu.Count;
                Int32 usuariosPossiveis = (Int32)Session["NumUsuarios"];
                ViewBag.UsuariosPossiveis = usuariosPossiveis;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/19/Ajuda19.pdf";

                // Mensagens
                if (Session["MensUsuario"] != null)
                {
                    if ((Int32)Session["MensUsuario"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0158", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0365", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        TempData["MensagemAcerto"] = frase;
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        TempData["MensagemAcerto"] = frase;
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 109)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0532", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdSMS"];
                        TempData["MensagemAcerto"] = frase;
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0351", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 201)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0352", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 280)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0370", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 99)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0705", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 98)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0706", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 993)
                    {
                        String frase = (String)Session["MsgCRUD"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 992)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 994)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 995)
                    {
                        String frase = (String)Session["MsgCRUD"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO", "Usuario", "MontarTelaUsuario");

                Session["FlagMensagensEnviadas"] = 4;
                Session["VoltaAnexos"] = 0;
                Session["MensUsuario"] = null;
                Session["VoltaUsuario"] = 1;
                Session["VoltarConsulta"] = 1;
                Session["VoltaUsu"] = 1;
                Session["ListaLog"] = null;
                objeto = new USUARIO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaUsuario"] == 2)
            {
                return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpPost]
        public ActionResult FiltrarUsuario(USUARIO item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeral(item.USUA_NM_NOME);
                item.USUA_NM_APELIDO = CrossCutting.UtilitariosGeral.CleanStringGeral(item.USUA_NM_APELIDO);
                item.USUA_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(item.USUA_NR_CPF);

                Int32 idAss = (Int32)Session["IdAssinante"];
                List<USUARIO> listaObj = new List<USUARIO>();
                Tuple<Int32, List<USUARIO>, Boolean> volta = baseApp.ExecuteFilter(item.PERF_CD_ID, item.CAUS_CD_ID, item.USUA_NM_NOME, item.USUA_NM_APELIDO, item.USUA_NR_CPF, idAss);
                Session["FiltroUsuario"] = item;

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensUsuario"] = 1;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Sucesso
                Session["MensUsuario"] = 0;
                listaMaster = volta.Item2;
                Session["ListaUsuario"] = listaMaster;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroUsuario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaUsuario"] = null;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoUsuario()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                listaMaster = baseApp.GetAllUsuarios(idAss);
                Session["ListaUsuario"] = listaMaster;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoUsuario(Int32 id)
        {
            try
            {
                // Prepara view
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Usuários - Anexo";
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO_ANEXO", "Usuario", "VerAnexoUsuario");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerAnexoUsuarioAudio(Int32 id)
        {
            try
            {
                // Prepara view
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Usuários - Anexo";
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO_ANEXO", "Usuario", "VerAnexoUsuario");
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult DownloadUsuario(Int32 id)
        {
            // Força o uso de TLS 1.2 (Obrigatório para Azure Storage no .NET 4.8)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                // 1. Carrega as configurações de Storage da sua tabela CONFIGURACAO
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (conf == null) return Content("Erro: Configurações de Storage não encontradas.");

                string connString = conf.CONF_NM_STORAGE_CONN;
                string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                if (string.IsNullOrEmpty(connString)) return Content("Erro: String de conexão do Azure está vazia.");

                // 2. Busca o registro do anexo no banco
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                if (item == null || string.IsNullOrEmpty(item.USAN_AQ_ARQUIVO))
                {
                    return Content("Erro: Registro do anexo não encontrado no banco de dados.");
                }

                // 3. LIMPEZA DO CAMINHO (Tratamento para o Azure)
                // Remove o '~', remove barras do início e padroniza as barras invertidas
                string caminhoFormatado = item.USAN_AQ_ARQUIVO.Replace("~", "");
                caminhoFormatado = caminhoFormatado.TrimStart('/');
                caminhoFormatado = caminhoFormatado.Replace("\\", "/");

                // 4. Conexão com o Azure Blob Storage
                var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(caminhoFormatado);

                // 5. Verifica se o arquivo realmente existe no container
                if (!blobClient.Exists())
                {
                    return Content("Erro: Arquivo não localizado no Azure. Caminho tentado: [" + caminhoFormatado + "]");
                }

                // 6. Download do conteúdo para a memória do servidor
                var download = blobClient.DownloadContent();
                byte[] dados = download.Value.Content.ToArray();

                // 7. Define nome e tipo do arquivo
                string nomeDownload = Path.GetFileName(caminhoFormatado);
                string contentType = MimeMapping.GetMimeMapping(nomeDownload);

                // 8. Entrega o arquivo forçando o download no navegador
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;

                Response.ContentType = contentType;
                // Aspas duplas no nome do arquivo tratam nomes com espaços
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nomeDownload + "\"");

                Response.BinaryWrite(dados);
                Response.Flush();
                Response.End();

                return null;
            }
            catch (Exception ex)
            {
                // Gravação de Log de Exceção padrão WebDoctor/RTI
                try
                {
                    var user = Session["UserCredentials"] as USUARIO;
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    grava.GravarLogExcecao(ex, "Paciente", "WebDoctor", 1, user);
                }
                catch { /* Evita erro no catch se a sessão estiver expirada */ }

                return Content("Erro técnico ao realizar download: " + ex.Message);
            }
        }

        public ActionResult VoltarAnexoUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdUsuario"];
            if ((Int32)Session["VoltaAnexos"] == 0)
            {
                return RedirectToAction("MontarTelaUsuario", "Usuario");
            }
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        public ActionResult EditarPerfilUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["VoltaUsu"] = 2;
            Int32 idUsu = (Int32)Session["IdUsuario"];
            return RedirectToAction("EditarUsuario", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult VerUsuario(Int32 id)
        {
            try
            {
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["IdUsuario"] = id;
                Session["ModuloAtual"] = "Usuários - Visualização";
                USUARIO item = baseApp.GetItemById(id);
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO_VER", "Usuario", "VerUsuario");

                // Monta Log
                item.ASSINANTE = null;
                item.PERFIL = null;
                item.EMPRESA = null;
                item.CATEGORIA_USUARIO = null;
                item.CARGO = null;
                item.DEPARTAMENTO = null;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaPerfilUsuario()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idUsu = ((USUARIO)Session["UserCredentials"]).USUA_CD_ID;
            Session["VoltaUsuario"] = 2;
            return RedirectToAction("VerUsuario", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult IncluirUsuario()
        {
            try
            {
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];

                    if (usuario.PERFIL.PERF_IN_INCLUSAO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Usuários - Inclusão";

                // Verifica possibilidade
                Int32 num = CarregaUsuario().Count;
                Int32 cc = (Int32)Session["NumUsuarios"];
                if ((Int32)Session["NumUsuarios"] <= num)
                {
                    Session["MensUsuario"] = 50;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }

                // Prepara listas
                List<SelectListItem> marca = new List<SelectListItem>();
                marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Marca = new SelectList(marca, "Value", "Text");
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Classe = new SelectList(CarregaClasse(), "TICL_CD_ID", "TICL_NM_NOME");
                ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
                ViewBag.Cats = new SelectList(CarregaCategoriaUsuario(), "CAUS_CD_ID", "CAUS_NM_NOME");
                List<SelectListItem> indica = new List<SelectListItem>();
                indica.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                indica.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Indica = new SelectList(indica, "Value", "Text");
                EMPRESA emp = empApp.GetItemById(usuario.EMPR_CD_ID.Value);
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/19/Ajuda19_1.pdf";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO_INCLUSAO", "Usuario", "IncluirUsuario");

                // Prepara view
                USUARIO item = new USUARIO();
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
                vm.USUA_DT_CADASTRO = DateTime.Today.Date;
                vm.USUA_IN_ATIVO = 1;
                vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                vm.USUA_IN_BLOQUEADO = 0;
                vm.USUA_IN_LOGADO = 0;
                vm.USUA_IN_SISTEMA = 6;
                vm.USUA_IN_LOGIN_PROVISORIO = 0;
                vm.USUA_IN_PROVISORIO = 0;
                vm.USUA_NR_ACESSOS = 0;
                vm.USUA_NR_FALHAS = 0;
                vm.CAUS_CD_ID = 1;
                vm.USUA_IN_ESPECIAL = 0;
                vm.USUA_IN_PENDENTE_CODIGO = 0;
                vm.EMPR_CD_ID = empApp.GetItemByAssinante(idAss).EMPR_CD_ID;
                vm.USUA_IN_FILIAIS = 0;
                vm.USUA_IN_VENDEDOR = 0;
                vm.USUA_IN_COMISSAO = 0;
                vm.USUA_NM_PREFIXO = "Dr.";
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IncluirUsuario(UsuarioViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Classe = new SelectList(CarregaClasse(), "TICL_CD_ID", "tICL_NM_NOME");
            ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
            ViewBag.Cats = new SelectList(CarregaCategoriaUsuario(), "CAUS_CD_ID", "CAUS_NM_NOME");
            EMPRESA emp = empApp.GetItemById(usuarioLogado.EMPR_CD_ID.Value);
            List<SelectListItem> marca = new List<SelectListItem>();
            marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Marca = new SelectList(marca, "Value", "Text");
            List<SelectListItem> indica = new List<SelectListItem>();
            indica.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            indica.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Indica = new SelectList(indica, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NM_NOME);
                    vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                    vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);
                    vm.USUA_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.USUA_TX_OBSERVACOES);
                    vm.USUA_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_CPF);
                    vm.USUA_NR_RG = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_RG);
                    vm.USUA_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.USUA_NR_TELEFONE);
                    vm.USUA_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.USUA_NR_CELULAR);
                    vm.USUA_NR_CLASSE = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_CLASSE);

                    // Checa prefixo
                    if (vm.USUA_NM_PREFIXO == null)
                    {
                        vm.USUA_NM_PREFIXO = "Sr(a).";
                    }
                    if (vm.USUA_IN_CONSULTA == null)
                    {
                        vm.USUA_IN_CONSULTA = 1;
                    }

                    // Carrega foto e processa alteracao
                    if (vm.USUA_AQ_FOTO == null)
                    {
                        vm.USUA_AQ_FOTO = "~/Imagens/Base/icone_morador.png";
                    }

                    // Executa a operação
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 3;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0009", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 4;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensUsuario"] = 5;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensUsuario"] = 6;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        Session["MensUsuario"] = 7;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        Session["MensUsuario"] = 10;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0158", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        Session["MensUsuario"] = 11;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0223", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        Session["MensUsuario"] = 12;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0224", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 11)
                    {
                        Session["MensUsuario"] = 13;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0225", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 12)
                    {
                        Session["MensUsuario"] = 14;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0226", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 13)
                    {
                        Session["MensUsuario"] = 15;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0227", CultureInfo.CurrentCulture));
                        return View(vm);
                    }
                    if (volta == 20)
                    {
                        Session["MensUsuario"] = 20;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0652", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["IdUsuario"] = item.USUA_CD_ID;
                    Session["UsuarioAlterada"] = 1;

                    if (Session["FileQueueUsuario"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueUsuario"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueUsuarioBlob(file);
                            }
                            else
                            {
                                await UploadFotoQueueUsuarioBlob(file);
                            }
                        }

                        Session["FileQueueUsuario"] = null;
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensUsuario"] = 61;    
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarUsuario(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Usuários - Edição";

                // Verifica possibilidade E-Mail
                ViewBag.EMail = 1;
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    ViewBag.EMail = 0;
                }

                // Verifica possibilidade SMS
                ViewBag.SMS = 1;
                num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    ViewBag.SMS = 0;
                }

                // Mensagem
                if (Session["MensUsuario"] != null)
                {
                    if ((Int32)Session["MensUsuario"] == 100)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0305", CultureInfo.CurrentCulture) + " ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 101)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0257", CultureInfo.CurrentCulture) + " Status: " + (String)Session["StatusMail"] + ". ID do envio: " + (String)Session["IdMail"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 11)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0598", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 993)
                    {
                        String frase = (String)Session["MsgCRUD"];
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensUsuario"] == 992)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 994)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                    if ((Int32)Session["MensUsuario"] == 995)
                    {
                        String frase = (String)Session["MsgCRUD"];
                        ModelState.AddModelError("", frase);
                    }
                }

                // Prepara view
                ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
                ViewBag.Classe = new SelectList(CarregaClasse(), "TICL_CD_ID", "tICL_NM_NOME");
                ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
                ViewBag.Cats = new SelectList(CarregaCategoriaUsuario(), "CAUS_CD_ID", "CAUS_NM_NOME");
                List<SelectListItem> marca = new List<SelectListItem>();
                marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Marca = new SelectList(marca, "Value", "Text");
                List<SelectListItem> indica = new List<SelectListItem>();
                indica.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                indica.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Indica = new SelectList(indica, "Value", "Text");

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "USUARIO_EDICAO", "Usuario", "EditarUsuario");

                ViewBag.UsuarioLogado = usuario;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = item;
                Session["VoltaAnexos"] = 1;
                Session["Usuario"] = item;
                Session["IdUsuario"] = id;
                Session["MensUsuario"] = 0;
                Session["FlagMensagensEnviadas"] = 4;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/19/Ajuda19_2.pdf";
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario(UsuarioViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfis = new SelectList(CarregaPerfil(), "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Classe = new SelectList(CarregaClasse(), "TICL_CD_ID", "tICL_NM_NOME");
            ViewBag.Espec = new SelectList(CarregaEspecialidade(), "ESPE_CD_ID", "ESPE_NM_NOME");
            ViewBag.Cats = new SelectList(CarregaCategoriaUsuario(), "CAUS_CD_ID", "CAUS_NM_NOME");
            List<SelectListItem> marca = new List<SelectListItem>();
            marca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            marca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Marca = new SelectList(marca, "Value", "Text");
            List<SelectListItem> indica = new List<SelectListItem>();
            indica.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            indica.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Indica = new SelectList(indica, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.USUA_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NM_NOME);
                    vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                    vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);
                    vm.USUA_TX_OBSERVACOES = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_TX_OBSERVACOES);
                    vm.USUA_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_CPF);
                    vm.USUA_NR_RG = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_RG);
                    vm.USUA_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.USUA_NR_TELEFONE);
                    vm.USUA_NR_CELULAR = CrossCutting.UtilitariosGeral.CleanStringPhone(vm.USUA_NR_CELULAR);
                    vm.USUA_NR_CLASSE = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_CLASSE);

                    // Checa prefixo
                    if (vm.USUA_NM_PREFIXO == null)
                    {
                        vm.USUA_NM_PREFIXO = "Sr(a).";
                    }
                    if (vm.USUA_IN_CONSULTA == null)
                    {
                        vm.USUA_IN_CONSULTA = 1;
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensUsuario"] = 4;
                        return RedirectToAction("MontarTelaUsuario");
                    }
                    if (volta == 2)
                    {
                        Session["MensUsuario"] = 5;
                        return RedirectToAction("MontarTelaUsuario");
                    }

                    // Sucesso
                    listaMaster = new List<USUARIO>();
                    Session["ListaUsuario"] = null;
                    Session["UsuarioAlterada"] = 1;
                    Session["Usuarios"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensUsuario"] = 61;

                    if ((Int32)Session["VoltaUsu"] == 1)
                    {
                        return RedirectToAction("VoltarAnexoUsuario");
                    }
                    else if ((Int32)Session["VoltaUsu"] == 4)
                    {
                        return RedirectToAction("MontarTelaMarcacaoConsulta", "Paciente");
                    }
                    else
                    {
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

       [HttpGet]
        public ActionResult BloquearUsuario(Int32 id)
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

                    if (usuario.PERFIL.PERF_IN_BLOQUEIO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                // Verifica possibilidade
                List<USUARIO> usus = CarregaUsuario().Where(p => p.PERFIL.PERF_NM_NOME.ToUpper().Contains("DMIN")).ToList();
                USUARIO usu = usus.Where(p => p.USUA_CD_ID == item.USUA_CD_ID).FirstOrDefault();
                if (usu != null & usus.Count() == 1)
                {
                    Session["MensUsuario"] = 99;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Verifica se é o próprio
                if (item.USUA_CD_ID == usuario.USUA_CD_ID)
                {
                    Session["MensUsuario"] = 98;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Processa
                USUARIO block = new USUARIO();
                block.USUA_CD_ID = item.USUA_CD_ID;
                block.ASSI_CD_ID = item.ASSI_CD_ID;
                block.PERF_CD_ID = item.PERF_CD_ID;
                block.CARG_CD_ID = item.CARG_CD_ID;
                block.ESPE_CD_ID = item.ESPE_CD_ID;
                block.USUA_NM_NOME = item.USUA_NM_NOME;
                block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                block.USUA_NM_SENHA = item.USUA_NM_SENHA;
                block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                block.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                block.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                block.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                block.USUA_NR_CPF = item.USUA_NR_CPF;
                block.USUA_NR_RG = item.USUA_NR_RG;
                block.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                block.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                block.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
                block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
                block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                block.USUA_IN_BLOQUEADO = 1;
                block.USUA_DT_BLOQUEADO = DateTime.Today;
                block.EMPR_CD_ID = item.EMPR_CD_ID;
                block.USUA_NM_APELIDO = item.USUA_NM_APELIDO;
                block.USUA_NM_PREFIXO = item.USUA_NM_PREFIXO;
                block.USUA_NM_SUFIXO = item.USUA_NM_SUFIXO;
                block.TICL_CD_ID = item.ESPE_CD_ID;
                block.USUA_NR_CLASSE = item.USUA_NR_CLASSE;

                Int32 volta = baseApp.ValidateBloqueio(block, usuario);
                listaMaster = new List<USUARIO>();
                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                if ((Int32)Session["VoltaUsuario"] == 55)
                {
                    return RedirectToAction("MontarTelaControleAcesso", "BaseAdmin");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi bloqueado com sucesso.";
                Session["MensUsuario"] = 61;

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult DesbloquearUsuario(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_BLOQUEIO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO unblock = new USUARIO();
                unblock.USUA_CD_ID = item.USUA_CD_ID;
                unblock.ASSI_CD_ID = item.ASSI_CD_ID;
                unblock.PERF_CD_ID = item.PERF_CD_ID;
                unblock.CARG_CD_ID = item.CARG_CD_ID;
                unblock.ESPE_CD_ID = item.ESPE_CD_ID;
                unblock.USUA_NM_NOME = item.USUA_NM_NOME;
                unblock.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                unblock.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                unblock.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                unblock.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                unblock.USUA_NM_SENHA = item.USUA_NM_SENHA;
                unblock.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                unblock.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                unblock.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                unblock.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                unblock.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                unblock.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                unblock.USUA_NR_CPF = item.USUA_NR_CPF;
                unblock.USUA_NR_RG = item.USUA_NR_RG;
                unblock.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                unblock.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                unblock.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                unblock.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                unblock.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
                unblock.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
                unblock.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                unblock.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                unblock.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                unblock.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                unblock.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                unblock.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                unblock.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                unblock.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                unblock.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                unblock.USUA_IN_BLOQUEADO = 0;
                unblock.USUA_DT_BLOQUEADO = null;
                unblock.EMPR_CD_ID = item.EMPR_CD_ID;
                unblock.USUA_NM_APELIDO = item.USUA_NM_APELIDO;
                unblock.USUA_NM_PREFIXO = item.USUA_NM_PREFIXO;
                unblock.USUA_NM_SUFIXO = item.USUA_NM_SUFIXO;
                unblock.TICL_CD_ID = item.ESPE_CD_ID;
                unblock.USUA_NR_CLASSE = item.USUA_NR_CLASSE;

                Int32 volta = baseApp.ValidateDesbloqueio(unblock, usuario);
                listaMaster = new List<USUARIO>();

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }
                if ((Int32)Session["VoltaUsuario"] == 55)
                {
                    return RedirectToAction("MontarTelaControleAcesso", "BaseAdmin");
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi desbloqueado com sucesso.";
                Session["MensUsuario"] = 61;

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult DesativarUsuario(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recuperar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                // Verifica possibilidade
                List<USUARIO> usus = CarregaUsuario().Where(p => p.PERFIL.PERF_NM_NOME.ToUpper().Contains("DMIN")).ToList();
                USUARIO usu = usus.Where(p => p.USUA_CD_ID == item.USUA_CD_ID).FirstOrDefault();
                if (usu != null & usus.Count() == 1)
                {
                    Session["MensUsuario"] = 99;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Verifica se é o próprio
                if (item.USUA_CD_ID == usuario.USUA_CD_ID)
                {
                    Session["MensUsuario"] = 98;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Processar
                USUARIO dis = new USUARIO();
                dis.USUA_CD_ID = item.USUA_CD_ID;
                dis.ASSI_CD_ID = item.ASSI_CD_ID;
                dis.PERF_CD_ID = item.PERF_CD_ID;
                dis.CARG_CD_ID = item.CARG_CD_ID;
                dis.ESPE_CD_ID = item.ESPE_CD_ID;
                dis.USUA_NM_NOME = item.USUA_NM_NOME;
                dis.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                dis.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                dis.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                dis.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                dis.USUA_NM_SENHA = item.USUA_NM_SENHA;
                dis.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                dis.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                dis.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                dis.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                dis.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                dis.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                dis.USUA_NR_CPF = item.USUA_NR_CPF;
                dis.USUA_NR_RG = item.USUA_NR_RG;
                dis.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                dis.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                dis.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                dis.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                dis.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                dis.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                dis.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                dis.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                dis.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                dis.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                dis.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                dis.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                dis.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                dis.USUA_IN_ATIVO = 0;
                dis.USUA_DT_ALTERACAO = DateTime.Today;
                dis.EMPR_CD_ID = item.EMPR_CD_ID;
                dis.USUA_NM_APELIDO = item.USUA_NM_APELIDO;
                dis.USUA_NM_PREFIXO = item.USUA_NM_PREFIXO;
                dis.USUA_NM_SUFIXO = item.USUA_NM_SUFIXO;
                dis.TICL_CD_ID = item.ESPE_CD_ID;
                dis.USUA_NR_CLASSE = item.USUA_NR_CLASSE;

                Int32 volta = baseApp.ValidateDelete(dis, usuario);
                listaMaster = new List<USUARIO>();
                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi desativado com sucesso.";
                Session["MensUsuario"] = 61;

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ReativarUsuario(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_REATIVACAO_USUARIO == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                //Verifica possibilidade
                Int32 num = CarregaUsuario().Count;
                if ((Int32)Session["NumUsuarios"] <= num)
                {
                    Session["MensUsuario"] = 50;
                    return RedirectToAction("MontarTelaUsuario", "Usuario");
                }

                // Executar
                USUARIO item = baseApp.GetItemById(id);
                objetoAntes = (USUARIO)Session["Usuario"];

                USUARIO en = new USUARIO();
                en.USUA_CD_ID = item.USUA_CD_ID;
                en.ASSI_CD_ID = item.ASSI_CD_ID;
                en.PERF_CD_ID = item.PERF_CD_ID;
                en.CARG_CD_ID = item.CARG_CD_ID;
                en.ESPE_CD_ID = item.ESPE_CD_ID;
                en.USUA_NM_NOME = item.USUA_NM_NOME;
                en.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
                en.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
                en.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
                en.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
                en.USUA_NM_SENHA = item.USUA_NM_SENHA;
                en.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
                en.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
                en.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
                en.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
                en.USUA_IN_BLOQUEADO = item.USUA_IN_BLOQUEADO;
                en.USUA_IN_SISTEMA = item.USUA_IN_SISTEMA;
                en.USUA_DT_BLOQUEADO = item.USUA_DT_BLOQUEADO;
                en.USUA_NR_CPF = item.USUA_NR_CPF;
                en.USUA_NR_RG = item.USUA_NR_RG;
                en.USUA_IN_COMPRADOR = item.USUA_IN_COMPRADOR;
                en.USUA_IN_APROVADOR = item.USUA_IN_APROVADOR;
                en.USUA_IN_PENDENTE_CODIGO = item.USUA_IN_PENDENTE_CODIGO;
                en.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
                en.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
                en.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
                en.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
                en.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
                en.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
                en.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
                en.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
                en.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
                en.USUA_IN_ATIVO = 1;
                en.USUA_DT_ALTERACAO = DateTime.Today;
                en.EMPR_CD_ID = item.EMPR_CD_ID;
                en.USUA_NM_APELIDO = item.USUA_NM_APELIDO;
                en.USUA_NM_PREFIXO = item.USUA_NM_PREFIXO;
                en.USUA_NM_SUFIXO = item.USUA_NM_SUFIXO;
                en.TICL_CD_ID = item.ESPE_CD_ID;
                en.USUA_NR_CLASSE = item.USUA_NR_CLASSE;

                Int32 volta = baseApp.ValidateReativar(en, usuario);
                listaMaster = new List<USUARIO>();
                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                if (Session["FiltroUsuario"] != null)
                {
                    FiltrarUsuario((USUARIO)Session["FiltroUsuario"]);
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O usuário " + item.USUA_NM_NOME.ToUpper() + " foi reativado com sucesso.";
                Session["MensUsuario"] = 61;

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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
            Session["FileQueueUsuario"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueUsuario(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);
                Session["UsuarioAlterada"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileQueueUsuarioBlob(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // 1. DEFINIÇÃO DE CAMINHOS
                String caminhoRelativo = "Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
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
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);
                Session["UsuarioAlterada"] = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFileUsuario(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 50)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensUsuario"] = 7;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileUsuarioBlob(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 50)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensUsuario"] = 7;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // 1. DEFINIÇÃO DO CAMINHO (Mesmo para Local e Azure)
                // Removida a barra inicial para o Azure não criar uma pasta raiz vazia
                String caminhoRelativo = "Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Anexos/";
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

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                USUARIO_ANEXO foto = new USUARIO_ANEXO();
                foto.USAN_AQ_ARQUIVO = "~" + caminhoRelativo + fileName;
                foto.USAN_DT_ANEXO = DateTime.Today;
                foto.USAN_IN_ATIVO = 1;
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
                foto.USAN_IN_TIPO = tipo;
                foto.USAN_NM_TITULO = fileName;
                foto.USUA_CD_ID = item.USUA_CD_ID;

                item.USUARIO_ANEXO.Add(foto);
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoQueueUsuario(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idUsu = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idUsu);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // Salva arquivo
                    System.IO.File.WriteAllBytes(path, file.Contents);

                    // Gravar registro
                    item.USUA_AQ_FOTO = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Foto - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFotoQueueUsuarioBlob(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idUsu = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idUsu);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // 1. DEFINIÇÃO DE CAMINHOS (Removendo a barra inicial para o Azure)
                    String caminhoRelativo = "Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                    String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                    String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                    // Garante que a pasta local existe
                    if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                    // 2. CÓPIA LOCAL
                    System.IO.File.WriteAllBytes(fullPathLocal, file.Contents);

                    // 3. CÓPIA PARA O AZURE BLOB STORAGE
                    try
                    {
                        CONFIGURACAO conf = CarregaConfiguracaoGeral();
                        string connString = conf.CONF_NM_STORAGE_CONN;
                        string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                        var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        // Nome do blob incluindo as "pastas" virtuais
                        string blobName = caminhoRelativo + fileName;
                        var blobClient = containerClient.GetBlobClient(blobName);

                        // Upload idempotente usando MemoryStream
                        using (var ms = new MemoryStream(file.Contents))
                        {
                            await blobClient.UploadAsync(ms, overwrite: true);
                        }
                    }
                    catch (Exception exAzure)
                    {
                        return RedirectToAction("VoltarAnexoUsuario");
                    }

                    // 4. ATUALIZAÇÃO DO REGISTRO
                    item.USUA_AQ_FOTO = "~/" + caminhoRelativo + fileName;
                    objetoAntes = item;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Foto - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoUsuario(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensUsuario"] = 7;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                String caminho = "/Imagens/" + idAss.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
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
                    item.USUA_AQ_FOTO = "~" + caminho + fileName;
                    objeto = item;
                    Int32 volta = baseApp.ValidateEdit(item, objeto);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Foto - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = fileName,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult UploadFotoUsuarioBlob(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdUsuario"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensUsuario"] = 10;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                USUARIO item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensUsuario"] = 11;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensUsuario"] = 7;
                    return RedirectToAction("VoltarAnexoUsuario");
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // 1. DEFINIÇÃO DE CAMINHOS
                    String caminhoRelativo = "Imagens/" + item.ASSI_CD_ID.ToString() + "/Usuario/" + item.USUA_CD_ID.ToString() + "/Fotos/";
                    String caminhoLocal = Server.MapPath("~/" + caminhoRelativo);
                    String fullPathLocal = Path.Combine(caminhoLocal, fileName);

                    if (!Directory.Exists(caminhoLocal)) Directory.CreateDirectory(caminhoLocal);

                    // 2. CÓPIA LOCAL
                    file.SaveAs(fullPathLocal);

                    // 3. CÓPIA PARA O AZURE BLOB STORAGE (Síncrono)
                    try
                    {
                        file.InputStream.Position = 0;

                        CONFIGURACAO conf = CarregaConfiguracaoGeral();
                        string connString = conf.CONF_NM_STORAGE_CONN;
                        string containerName = conf.CONF_NM_STORAGE_CONTAINER;

                        var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(connString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        string blobName = caminhoRelativo + fileName;
                        var blobClient = containerClient.GetBlobClient(blobName);

                        // Chamada Síncrona usando .GetRawResponse() ou apenas omitindo await e usando Upload
                        // No SDK novo, usamos Upload(stream, overwrite) para modo síncrono
                        blobClient.Upload(file.InputStream, overwrite: true);
                    }
                    catch (Exception exAzure)
                    {
                        Session["MsgCRUD"] = "Erro na sincronização Azure: " + exAzure.Message;
                        Session["MensPaciente"] = 61;
                        return RedirectToAction("VoltarAnexoPaciente");
                    }

                    // Gravar registro
                    item.USUA_AQ_FOTO = "~" + caminhoRelativo + fileName;
                    objetoAntes = item;
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usu.ASSI_CD_ID,
                        USUA_CD_ID = usu.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Usuario - Foto - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Usuario: " + item.USUA_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);
                }

                Session["ListaUsuario"] = null;
                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioLista()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "UsuarioLista" + "_" + data + ".pdf";
                List<USUARIO> lista = (List<USUARIO>)Session["ListaUsuario"];
                USUARIO filtro = (USUARIO)Session["FiltroUsuario"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
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

                cell = new PdfPCell(new Paragraph("Usuários", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Grid
                table = new PdfPTable(new float[] { 120f, 80f, 100f, 70f, 100f, 70f, 60f, 70f, 50f, 50f, 50f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Usuários selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 11;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Apelido", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Especialidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Perfil", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Bloqueado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Acessos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Foto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (USUARIO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_APELIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.USUA_NM_ESPECIALIDADE, meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.USUA_NR_CPF, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PERFIL.PERF_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_USUARIO.CAUS_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_IN_BLOQUEADO == 1 ? "Sim" : "Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NR_ACESSOS.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (System.IO.File.Exists(Server.MapPath(item.USUA_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.USUA_AQ_FOTO));
                        image.ScaleAbsolute(20, 20);
                        cell.AddElement(image);
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
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line2);

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.USUA_NM_NOME != null)
                    {
                        parametros += "Nome: " + filtro.USUA_NM_NOME;
                        ja = 1;
                    }
                    if (filtro.USUA_NM_LOGIN != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Login: " + filtro.USUA_NM_LOGIN;
                            ja = 1;
                        }
                        else
                        {
                            parametros +=  " e Login: " + filtro.USUA_NM_LOGIN;
                        }
                    }
                    if (filtro.USUA_NM_EMAIL != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "E-Mail: " + filtro.USUA_NM_EMAIL;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e E-Mail: " + filtro.USUA_NM_EMAIL;
                        }
                    }
                    if (filtro.PERF_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Perfil: " + filtro.PERFIL.PERF_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Perfil: " + filtro.PERFIL.PERF_NM_NOME;
                        }
                    }
                    if (ja == 0)
                    {
                        parametros = "Nenhum filtro definido.";
                    }
                }
                else
                {
                    parametros = "Nenhum filtro definido.";
                }
                Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                // Linha Horizontal
                Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line3);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO aten = baseApp.GetItemById((Int32)Session["IdUsuario"]);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "Usuario_" + aten.USUA_CD_ID.ToString() + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font meuFontGreen = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.GREEN);

                // Cria documento
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

                // Linha horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Cabeçalho
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
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

                cell = new PdfPCell(new Paragraph("Usuário - Detalhes", meuFont2))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.Border = 0;
                cell.Colspan = 4;
                table.AddCell(cell);

                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);
                line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados Gerais
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.USUA_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Assinante: " + aten.EMPRESA.EMPR_NM_NOME, meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + aten.USUA_NM_NOME, meuFontGreen));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Login: " + aten.USUA_NM_LOGIN, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tratamento: " + aten.USUA_NM_PREFIXO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Títulos: " + aten.USUA_NM_SUFIXO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_IN_INDICA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Chave PIX: " + aten.USUA_NR_CHAVE_PIX, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("   ", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph("E-Mail: " + aten.USUA_NM_EMAIL, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Perfil: " + aten.PERFIL.PERF_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_IN_CONSULTA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Marca Consulta: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Marca Consulta: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_IN_INDICA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Pode indicar: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Pode indicar: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Acessos
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Dados de Acesso", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.USUA_IN_BLOQUEADO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Bloqueado: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Bloqueado: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_DT_BLOQUEADO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Bloqueio: " + aten.USUA_DT_BLOQUEADO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Bloqueio: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_IN_PROVISORIO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Senha Provisória: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Senha Provisória: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_IN_LOGIN_PROVISORIO == 1)
                {
                    cell = new PdfPCell(new Paragraph("Login Provisório: Sim", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Login Provisório: Não", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                if (aten.USUA_DT_ALTERACAO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração: " + aten.USUA_DT_ALTERACAO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                if (aten.USUA_DT_TROCA_SENHA != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração de Senha: " + aten.USUA_DT_TROCA_SENHA.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Alteração de Senha: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph("Acessos: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_ACESSOS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_DT_ACESSO != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Último Acesso: " + aten.USUA_DT_ACESSO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Último Acesso: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph("Falhas de Login: " + CrossCutting.Formatters.DecimalFormatter(aten.USUA_NR_FALHAS.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                if (aten.USUA_DT_ULTIMA_FALHA != null)
                {
                    cell = new PdfPCell(new Paragraph("Data Última Falha: " + aten.USUA_DT_ULTIMA_FALHA.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Data Última Falha: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Observações
                Chunk chunk1 = new Chunk("Observações: " + aten.USUA_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenha(Int32 id)
        {
            
            // Prepara view
            return RedirectToAction("TrocarSenha", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult EnviarEMailUsuarioForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EnviarEMailUsuario", new { id = (Int32)Session["IdUsuario"] });
        }

        [HttpGet]
        public ActionResult EnviarSMSUsuarioForm()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("EnviarSMSUsuario", new { id = (Int32)Session["IdUsuario"] });
        }

        [HttpGet]
        public ActionResult EnviarEMailUsuario(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Usuários - E-Mail";

                //Verifica possibilidade
                USUARIO cont = baseApp.GetItemById(id);
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 1).ToList().Count;
                if ((Int32)Session["NumEMail"] <= num)
                {
                    Session["MensUsuario"] = 200;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(cont.USUA_CD_ID, cont.ASSI_CD_ID, "USUARIO_EMAIL", "Usuario", "EnviarEMailUsuario");

                Session["Usuario"] = cont;
                ViewBag.PessoaExterna = cont;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = cont.USUA_NM_NOME;
                mens.ID = id;
                mens.USUA_CD_ID = cont.USUA_CD_ID;
                mens.MODELO = cont.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MENS_NM_NOME = "Mensagem para Usuário";
                mens.MENS_IN_TIPO_EMAIL = 1;
                mens.TIPO_ENVIO = 1;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EnviarEMailUsuario(MensagemViewModel vm)
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
                    vm.MENS_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_TEXTO);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);

                    // Critica
                    if (vm.MENS_TX_TEXTO == null)
                    {
                        Session["MensUsuario"] = 280;
                        return RedirectToAction("VoltarAnexoUsuario");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = await ProcessaEnvioEMailUsuario(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Usuário - E-Mail - Envio",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.USUA_NM_NOME,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    if ((Int32)Session["VoltaUsu"] == 4)
                    {
                        return RedirectToAction("VerUsuario", new { id = cont.USUA_CD_ID });
                    }
                    else
                    {
                        return RedirectToAction("VoltarAnexoUsuario");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailUsuario(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
            String erro = null;
            String status = "Succeeded";
            String iD = Xid.NewXid().ToString();

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.USUA_NM_NOME + "</b>";

            // Prepara assinatura
            String classe = String.Empty;
            if (usuario.TIPO_CARTEIRA_CLASSE != null)
            {
                classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            }
            String rod = usuario.USUA_NM_NOME + ". ";
            if (usuario.ESPECIALIDADE != null)
            {
                rod += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
            }
            else
            {
                rod += usuario.USUA_NM_ESPECIALIDADE + "<br />";
            }
            rod += classe + ". CPF: " + usuario.USUA_NR_CPF;

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            body = body.Replace("\r\n", "<br />");
            String emailBody = cab + "<br /><br />" + body + "<br />" + rod;

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Usuário - " + cont.USUA_NM_NOME;
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = cont.USUA_NM_EMAIL;
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
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }

            // Grava envio
            if (status == "Succeeded")
            {
                vm.MENS_NM_NOME = "Usuário - " + cont.USUA_NM_NOME;
                vm.MENS_NM_CAMPANHA = cont.USUA_NM_EMAIL;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null;
                vm.USUA_CD_ID = cont.USUA_CD_ID;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(baseApp, confApp, meApp);
                Int32 voltaX = envio.GravarMensagemEnviada(vm, usuario, emailBody, status, iD, erro, "Usuário - Envio de Mensagem");
                Session["MensUsuario"] = 992;
                Session["MsgCRUD"] = "E-Mail enviado com sucesso para o usuário " + cont.USUA_NM_NOME.ToUpper();
                Session["IdMail"] = iD;
            }
            else
            {
                Session["MensUsuario"] = 993;
                Session["MsgCRUD"] = "Falha no envio de E-Mail para o usuário " + cont.USUA_NM_NOME.ToUpper();
                Session["IdMail"] = iD;
                Session["StatusMail"] = status;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult EnviarSMSUsuario(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Usuários - SMS";

                //Verifica possibilidade
                Int32 num = CarregaMensagemEnviada().Where(p => p.MEEN_DT_DATA_ENVIO.Value.Month == DateTime.Today.Date.Month & p.MEEN_DT_DATA_ENVIO.Value.Year == DateTime.Today.Date.Year & p.MEEN_IN_TIPO == 2).ToList().Count;
                if ((Int32)Session["NumSMS"] <= num)
                {
                    Session["MensUsuario"] = 201;
                    return RedirectToAction("MontarTelaUsuario");
                }

                // Grava Acesso
                USUARIO item = baseApp.GetItemById(id);
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(item.USUA_CD_ID, item.ASSI_CD_ID, "USUARIO_SMS", "Usuario", "EnviarSMSUsuario");

                Session["Usuario"] = item;
                ViewBag.PessoaExterna = item;
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = item.USUA_NM_NOME;
                mens.ID = id;
                mens.USUA_CD_ID = item.USUA_CD_ID;
                mens.MODELO = item.USUA_NR_CELULAR;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 2;
                mens.MENS_NM_NOME = "Mensagem para Usuário";
                mens.TIPO_ENVIO = 1;
                mens.MENS_IN_TIPO_EMAIL = 1;
                return View(mens);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult EnviarSMSUsuario(MensagemViewModel vm)
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
                    vm.MENS_TX_SMS = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_TX_SMS);
                    vm.MENS_NM_LINK = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.MENS_NM_LINK);

                    // Critica
                    if (vm.MENS_TX_SMS == null)
                    {
                        Session["MensUsuario"] = 280;
                        return RedirectToAction("VoltarAnexoUsuario");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ProcessaEnvioSMSUsuario(vm, usuarioLogado);
                    USUARIO cont = (USUARIO)Session["Usuario"];

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Usuário - SMS - Envio",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = cont.USUA_NM_NOME,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    if ((Int32)Session["VoltaUsu"] == 4)
                    {
                        return RedirectToAction("VerUsuario", "Usuario");
                    }
                    else
                    {
                        return RedirectToAction("VoltarAnexoUsuario");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuários";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSUsuario(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO cont = (USUARIO)Session["Usuario"];
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String erro = null;
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            String iD = Xid.NewXid().ToString();

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.USUA_NM_NOME + ".  ";

            // Prepara assinatura
            String classe = String.Empty;
            if (usuario.TIPO_CARTEIRA_CLASSE != null)
            {
                classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
            }
            String rod = usuario.USUA_NM_NOME + ". ";
            rod += usuario.USUA_NM_ESPECIALIDADE + ". ";
            rod += classe + " CPF: " + usuario.USUA_NR_CPF;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Decriptografa chaves
            String login = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_LOGIN_SMS_CRIP);
            String senha = CrossCutting.Cryptography.Decrypt(conf.CONF_SG_SENHA_SMS_CRIP);

            // Monta token
            String text = login + ":" + senha;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + " " + vm.MENS_TX_SMS + ". " + rod;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(texto);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine(". Link: " + vm.LINK);
            }
            String body = str.ToString();
            String smsBody = body;

            // inicia processo
            String resposta = String.Empty;
            try
            {
                // processa envio
                String listaDest = "55" + Regex.Replace(cont.USUA_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
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

                // Grava envio
                vm.MENS_NM_NOME = "Usuário - " + cont.USUA_NM_NOME;
                vm.MENS_NM_CAMPANHA = cont.USUA_NR_CELULAR;
                vm.FORN_CD_ID = null;
                vm.CLIE_CD_ID = null;
                vm.MENS_IN_USUARIO = cont.USUA_CD_ID;
                EnvioEMailGeralBase envio = new EnvioEMailGeralBase(baseApp, confApp, meApp);
                Int32 volta = envio.GravarMensagemEnviadaSMS(vm, usuario, smsBody, "Mensagem SMS para Usuário");

                Session["MensUsuario"] = 994;
                Session["MsgCRUD"] = "Mensagem SMS enviada com sucesso para o usuário " + cont.USUA_NM_NOME.ToUpper();
                Session["IdSMS"] = iD;
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                Session["MensUsuario"] = 995;
                Session["MsgCRUD"] = "Falha no envio de mensagem SMS para o usuário " + cont.USUA_NM_NOME.ToUpper();
                Session["IdSMS"] = iD;
                return 0;
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
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["UsuarioAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<PERFIL> CarregaPerfil()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PERFIL> conf = new List<PERFIL>();
                if (Session["Perfis"] == null)
                {
                    conf = perfApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<PERFIL>)Session["Perfis"];
                }
                conf = conf.Where(p => p.ASSI_CD_ID == idAss).ToList();
                Session["Perfis"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<TIPO_CARTEIRA_CLASSE> CarregaClasse()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_CARTEIRA_CLASSE> conf = new List<TIPO_CARTEIRA_CLASSE>();
                if (Session["Classes"] == null)
                {
                    conf = baseApp.GetAllClasse();
                }
                else
                {
                    conf = (List<TIPO_CARTEIRA_CLASSE>)Session["Classes"];
                }
                Session["Classes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<ESPECIALIDADE> CarregaEspecialidade()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<ESPECIALIDADE> conf = new List<ESPECIALIDADE>();
                if (Session["Especialidades"] == null)
                {
                    conf = baseApp.GetAllEspecialidade(idAss);
                }
                else
                {
                    if ((Int32)Session["EspecialidadeAlterada"] == 1)
                    {
                        conf = baseApp.GetAllEspecialidade(idAss);
                    }
                    else
                    {
                        conf = (List<ESPECIALIDADE>)Session["Especialidades"];
                    }
                }
                Session["EspecialidadeAlterada"] = 0;
                Session["Especialidades"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CATEGORIA_USUARIO> CarregaCategoriaUsuario()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CATEGORIA_USUARIO> conf = new List<CATEGORIA_USUARIO>();
                if (Session["CategoriaUsuarios"] == null)
                {
                    conf = cusApp.GetAllItens(idAss);
                }
                else
                {
                    conf = (List<CATEGORIA_USUARIO>)Session["CategoriaUsuarios"];
                }
                Session["CategoriaUsuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult VerMensagensEnviadas()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["FlagMensagensEnviadas"] = 4;
            return RedirectToAction("MontarTelaMensagensEnviadas", "BaseAdmin");
        }

        public ActionResult CriptoSenha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO item = (USUARIO)Session["UserCredentials"];

            USUARIO block = new USUARIO();
            block.USUA_CD_ID = item.USUA_CD_ID;
            block.ASSI_CD_ID = item.ASSI_CD_ID;
            block.PERF_CD_ID = item.PERF_CD_ID;
            block.CARG_CD_ID = item.CARG_CD_ID;
            block.USUA_NM_NOME = item.USUA_NM_NOME;
            block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            block.USUA_NM_SENHA = item.USUA_NM_SENHA;
            block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            block.USUA_IN_BLOQUEADO = 0;
            block.USUA_DT_BLOQUEADO = null;
            block.EMPR_CD_ID = item.EMPR_CD_ID;

            String senha = block.USUA_NM_SENHA;
            byte[] salt = CrossCutting.Cryptography.GenerateSalt();
            String hashedPassword = CrossCutting.Cryptography.HashPassword(block.USUA_NM_SENHA, salt);
            block.USUA_NM_SENHA = hashedPassword;
            Int32 volta = baseApp.ValidateEdit(block, block);
            Session["UserCredentials"] = block;

            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult DecriptoSenha()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO item = (USUARIO)Session["UserCredentials"];

            USUARIO block = new USUARIO();
            block.USUA_CD_ID = item.USUA_CD_ID;
            block.ASSI_CD_ID = item.ASSI_CD_ID;
            block.PERF_CD_ID = item.PERF_CD_ID;
            block.CARG_CD_ID = item.CARG_CD_ID;
            block.USUA_NM_NOME = item.USUA_NM_NOME;
            block.USUA_NM_LOGIN = item.USUA_NM_LOGIN;
            block.USUA_NM_EMAIL = item.USUA_NM_EMAIL;
            block.USUA_NR_TELEFONE = item.USUA_NR_TELEFONE;
            block.USUA_NR_CELULAR = item.USUA_NR_CELULAR;
            block.USUA_NM_SENHA = item.USUA_NM_SENHA;
            block.USUA_NM_SENHA_CONFIRMA = item.USUA_NM_SENHA_CONFIRMA;
            block.USUA_NM_NOVA_SENHA = item.USUA_NM_NOVA_SENHA;
            block.USUA_IN_PROVISORIO = item.USUA_IN_PROVISORIO;
            block.USUA_IN_LOGIN_PROVISORIO = item.USUA_IN_LOGIN_PROVISORIO;
            block.USUA_IN_ATIVO = item.USUA_IN_ATIVO;
            block.USUA_DT_ALTERACAO = item.USUA_DT_ALTERACAO;
            block.USUA_DT_TROCA_SENHA = item.USUA_DT_TROCA_SENHA;
            block.USUA_DT_ACESSO = item.USUA_DT_ACESSO;
            block.USUA_DT_ULTIMA_FALHA = item.USUA_DT_ULTIMA_FALHA;
            block.USUA_DT_CADASTRO = item.USUA_DT_CADASTRO;
            block.USUA_NR_ACESSOS = item.USUA_NR_ACESSOS;
            block.USUA_NR_FALHAS = item.USUA_NR_FALHAS;
            block.USUA_TX_OBSERVACOES = item.USUA_TX_OBSERVACOES;
            block.USUA_AQ_FOTO = item.USUA_AQ_FOTO;
            block.USUA_IN_LOGADO = item.USUA_IN_LOGADO;
            block.USUA_IN_BLOQUEADO = 0;
            block.USUA_DT_BLOQUEADO = null;
            block.EMPR_CD_ID = item.EMPR_CD_ID;

            String senha = block.USUA_NM_SENHA;
            senha = CrossCutting.Cryptography.Decrypt(senha);
            block.USUA_NM_SENHA = senha;
            Int32 volta = baseApp.ValidateEdit(block, block);
            Session["UserCredentials"] = block;
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaLogExcecao()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_ADMIN == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "BaseAdmin");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                ViewBag.Usuarios = new SelectList(baseApp.GetAllItens(idAss).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                if ((List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"] == null)
                {
                    listaMasterLogExc = baseApp.GetAllLogExcecao(idAss);
                    Session["ListaLogExcecao"] = listaMasterLogExc;
                }
                ViewBag.Listas = (List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"];
                ViewBag.Logs = ((List<LOG_EXCECAO_NOVO>)Session["ListaLogExcecao"]).Count;

                // Mensagens
                if ((Int32)Session["MensLog"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                }

                // Abre view
                Session["MensLog"] = 0;
                objLogExc = new LOG_EXCECAO_NOVO();
                objLogExc.LOEX_DT_DATA = DateTime.Today.Date;
                return View(objLogExc);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroLogExcecao()
        {
            Session["ListaLogExcecao"] = null;
            return RedirectToAction("MontarTelaLogExcecao");
        }

        [HttpPost]
        public ActionResult FiltrarLogExcecao(LOG_EXCECAO_NOVO item)
        {
            try
            {
                // Sanitização
                item.LOEX_NM_GERADOR = CrossCutting.UtilitariosGeral.CleanStringGeral(item.LOEX_NM_GERADOR);

                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOG_EXCECAO_NOVO> listaObj = new List<LOG_EXCECAO_NOVO>();
                Tuple<Int32, List<LOG_EXCECAO_NOVO>, Boolean> volta = baseApp.ExecuteFilterExcecao(item.USUA_CD_ID, item.LOEX_DT_DATA, item.LOEX_NM_GERADOR, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensLog"] = 1;
                    return RedirectToAction("MontarTelaLogExcecao");
                }

                // Sucesso
                listaMasterLogExc = volta.Item2;
                Session["ListaLogExcecao"] = listaMasterLogExc;
                return RedirectToAction("MontarTelaLogExcecao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerLogExcecao(Int32 id)
        {
            try
            {
                // Prepara view
                LOG_EXCECAO_NOVO item = baseApp.GetLogExcecaoById(id);
                return View(item);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Auditoria";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Auditoria", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
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

                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                USUARIO_ANEXO item = baseApp.GetAnexoById(id);
                item.USAN_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Usuário - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = item.USAN_AQ_ARQUIVO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["UsuarioAlterada"] = 1;
                return RedirectToAction("VoltarAnexoUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
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
                Session["MensagensEnviadas"] = conf;
                Session["MensagensEnviadaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public ActionResult GerarRelatorioUsuario()
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

                String nomeRel = "UsuarioLista" + "_" + data + ".pdf";
                List<USUARIO> lista = (List<USUARIO>)Session["ListaUsuario"];

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

                    cell1 = new PdfPCell(new Paragraph("Usuários Cadastrados", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Usuários Cadastrados", meuFont2))
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
                PdfPTable table = new PdfPTable(new float[] { 120f, 80f, 100f, 70f, 100f, 70f, 60f, 70f, 50f, 50f, 50f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HeaderRows = 1;

                cell = new PdfPCell(new Paragraph("Nome", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Apelido", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Especialidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("CPF", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Celular", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Perfil", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Bloqueado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Acessos", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Foto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (USUARIO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_APELIDO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.ESPECIALIDADE != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.USUA_NM_ESPECIALIDADE, meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.USUA_NR_CPF, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NR_CELULAR, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PERFIL.PERF_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_USUARIO.CAUS_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_IN_BLOQUEADO == 1 ? "Sim" : "Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.USUA_NR_ACESSOS.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (System.IO.File.Exists(Server.MapPath(item.USUA_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        Image image = Image.GetInstance(Server.MapPath(item.USUA_AQ_FOTO));
                        image.ScaleAbsolute(20, 20);
                        cell.AddElement(image);
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
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaIncluirIndicacao()
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
                    if (usuario.USUA_IN_INDICA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Indicação";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Indicação - Inclusão";

                // Prepara view
                List<INDICACAO> listaPac = CarregaIndicacao();
                listaPac = listaPac.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();

                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_1.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                 
                INDICACAO item = new INDICACAO();
                IndicacaoViewModel vm = Mapper.Map<INDICACAO, IndicacaoViewModel>(item);
                vm.INDI_IN_ATIVO = 1;
                vm.INDI_DT_DATA = DateTime.Now;
                vm.INDI_DT_ATUALIZACAO = DateTime.Now;
                vm.INDI_GU_IDENTIFICADOR = Xid.NewXid().ToString();
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.ASSI_CD_ID = idAss;
                vm.INDI_IN_SISTEMA = 6;
                vm.INDI_IN_STATUS = 1;
                vm.INDI_VL_PAGAMENTO = 0;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "INDICACAO", "Usuario", "MontarTelaIncluirIndicacao");

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MontarTelaIncluirIndicacao(IndicacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.INDI_NM_INDICADO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.INDI_NM_INDICADO);
                    vm.INDI_TX_MENSAGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.INDI_TX_MENSAGEM);
                    vm.INDI_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.INDI_NM_EMAIL);

                    // Executa a operação
                    INDICACAO item = Mapper.Map<IndicacaoViewModel, INDICACAO>(vm);
                    Int32 volta = baseApp.ValidateCreateIndicacao(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Indicação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<INDICACAO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Processa mensagens
                    INDICACAO indMsg = baseApp.GetIndicacaoById(item.INDI_CD_ID);
                    Int32 voltaCons = await EnviarEMailIndicacao(indMsg, 1);

                    // Verifica retorno
                    Session["IdIndicacao"] = item.INDI_CD_ID;
                    Session["IndicacaoAlterada"] = 1;
                    Session["ListaIndicacoes"] = null;
                    Session["Indicacoes"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A indicação " + item.INDI_GU_IDENTIFICADOR + " feita por " + usuarioLogado.USUA_NM_NOME.ToUpper() + " foi incluída com sucesso.";
                    Session["MensPaciente"] = 888;

                    // Retorno
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuario";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<INDICACAO> CarregaIndicacao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<INDICACAO> conf = new List<INDICACAO>();
                if (Session["Indicacoes"] == null)
                {
                    conf = baseApp.GetAllIndicacao(idAss);
                }
                else
                {
                    if ((Int32)Session["IndicacaoAlterada"] == 1)
                    {
                        conf = baseApp.GetAllIndicacao(idAss);
                    }
                    else
                    {
                        conf = (List<INDICACAO>)Session["Indicacoes"];
                    }
                }
                conf = conf.Where(p => p.INDI_IN_SISTEMA == 6).ToList();
                Session["IndicacaoAlterada"] = 0;
                Session["Indicacoes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuários";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuários", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public async Task<Int32> EnviarEMailIndicacao(INDICACAO indicacao, Int32 tipo)
        {
            // Recupera informações
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            CONFIGURACAO conf = CarregaConfiguracaoGeral();
            List<AttachmentModel> models = new List<AttachmentModel>();

            // Decriptografa chaves
            String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
            String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

            // Processo
            try
            {
                // Recupera Templates
                TEMPLATE_EMAIL template = temApp.GetByCode("CRIAINDI", idAss);
                TEMPLATE_EMAIL templateRTi = new TEMPLATE_EMAIL();
                if (tipo == 1)
                {
                    templateRTi = temApp.GetByCode("RTIINDI", idAss);
                }
                else
                {
                    templateRTi = temApp.GetByCode("RTIEXINDI", idAss);
                }

                // ===== MENSAGEM PARA RTi
                String cab = String.Empty;
                String texto = String.Empty;
                String assinatura = String.Empty;
                if (tipo < 3)
                {
                    // Prepara mensagem
                    models = null;
                    cab = templateRTi.TEEM_TX_CABECALHO;
                    texto = templateRTi.TEEM_TX_CORPO;
                    assinatura = templateRTi.TEEM_TX_DADOS;

                    if (texto.Contains("{data}"))
                    {
                        texto = texto.Replace("{data}", indicacao.INDI_DT_DATA.Value.ToLongDateString());
                    }
                    if (texto.Contains("{nome}"))
                    {
                        texto = texto.Replace("{nome}", indicacao.INDI_NM_INDICADO);
                    }
                    if (texto.Contains("{mail}"))
                    {
                        texto = texto.Replace("{mail}", indicacao.INDI_NM_EMAIL);
                    }
                    if (texto.Contains("{fone}"))
                    {
                        texto = texto.Replace("{fone}", indicacao.INDI_NR_TELEFONE);
                    }
                    if (texto.Contains("{celular}"))
                    {
                        texto = texto.Replace("{celular}", indicacao.INDI_NR_CELULAR);
                    }
                    if (texto.Contains("{guid}"))
                    {
                        texto = texto.Replace("{guid}", indicacao.INDI_GU_IDENTIFICADOR);
                    }
                    String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                    EmailAzure mensagem = new EmailAzure();
                    if (tipo == 1)
                    {
                        mensagem.ASSUNTO = "Indicação Cadastrada - " + indicacao.INDI_NM_INDICADO;
                    }
                    else
                    {
                        mensagem.ASSUNTO = "Indicação Excluída pelo Assinante - " + indicacao.INDI_NM_INDICADO;
                    }
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_TO_DESTINO = conf.CONF_EM_CRMSYS;
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
                        Session["VoltaExcecao"] = "Usuario";
                        Session["Excecao"] = ex;
                        Session["ExcecaoTipo"] = ex.GetType().ToString();
                        GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                        Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                        return 0;
                    }
                }

                // ===== MENSAGEM PARA INDICADO
                if (tipo == 1 || tipo == 3)
                {
                    // Prepara mensagem
                    cab = template.TEEM_TX_CABECALHO;
                    texto = template.TEEM_TX_CORPO;

                    if (texto.Contains("{nome}"))
                    {
                        texto = texto.Replace("{nome}", indicacao.INDI_NM_INDICADO);
                    }
                    if (texto.Contains("{mensagem}"))
                    {
                        texto = texto.Replace("{mensagem}", indicacao.INDI_TX_MENSAGEM);
                    }

                    // Prepara assinatura
                    assinatura = String.Empty;
                    String classe = String.Empty;
                    if (usuario.TIPO_CARTEIRA_CLASSE != null)
                    {
                        classe = usuario.TIPO_CARTEIRA_CLASSE.TICL_NM_NOME + ": " + usuario.USUA_NR_CLASSE;
                    }
                    assinatura = "<b>" + usuario.USUA_NM_NOME + "</b><br />";
                    if (usuario.ESPECIALIDADE != null)
                    {
                        assinatura += usuario.ESPECIALIDADE.ESPE_NM_NOME + "<br />";
                    }
                    else
                    {
                        assinatura += usuario.USUA_NM_ESPECIALIDADE + "<br />";
                    }
                    assinatura += classe + "  CPF: " + usuario.USUA_NR_CPF + "<br />";

                    // Prepara corpo da mensagem
                    String emailBody = cab + "<br />" + texto + "<br /><br />" + assinatura;

                    // Carrega anexo
                    models = new List<AttachmentModel>();
                    String caminho = "/BaseAdmin/Documentacao/";
                    String fileNamePDF = "WebDoctorPro_Divulgacao.pdf";
                    String path = Path.Combine(Server.MapPath(caminho), fileNamePDF);

                    AttachmentModel model = new AttachmentModel();
                    model.PATH = path;
                    model.ATTACHMENT_NAME = fileNamePDF;
                    model.CONTENT_TYPE = MediaTypeNames.Application.Pdf;
                    models.Add(model);

                    // Monta e-mail
                    NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                    EmailAzure mensagem = new EmailAzure();
                    mensagem.ASSUNTO = "Indicação - " + indicacao.INDI_NM_INDICADO;
                    mensagem.CORPO = emailBody;
                    mensagem.DEFAULT_CREDENTIALS = false;
                    mensagem.EMAIL_TO_DESTINO = indicacao.INDI_NM_EMAIL;
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
                        Session["VoltaExcecao"] = "Usuario";
                        Session["Excecao"] = ex;
                        Session["ExcecaoTipo"] = ex.GetType().ToString();
                        GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                        Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                        return 0;
                    }
                }

                // Sucesso
                return 1;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 1;
            }
        }

        [HttpGet]
        public ActionResult MontarTelaIndicacao()
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
                    if (usuario.USUA_IN_INDICA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Indicação";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Atestados";

                // Carrega listas
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                if (Session["ListaIndicacoes"] == null)
                {
                    listaMasterInd = CarregaIndicacao().Where(p => p.INDI_IN_ATIVO == 1 & p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                    Session["ListaIndicacoes"] = listaMasterInd;
                }

                // Monta demais listas
                ViewBag.Listas = (List<INDICACAO>)Session["ListaIndicacoes"];
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                var status = new List<SelectListItem>();
                status.Add(new SelectListItem() { Text = "Indicado", Value = "1" });
                status.Add(new SelectListItem() { Text = "Em Processamento", Value = "2" });
                status.Add(new SelectListItem() { Text = "Recusado", Value = "3" });
                status.Add(new SelectListItem() { Text = "Cancelado", Value = "4" });
                status.Add(new SelectListItem() { Text = "Pausado", Value = "5" });
                status.Add(new SelectListItem() { Text = "Sucesso", Value = "6" });
                ViewBag.Status = new SelectList(status, "Value", "Text");

                // Mensagem
                if (Session["MensUsuario"] != null)
                {
                    if ((Int32)Session["MensUsuario"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensUsuario"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Acerta estado
                Session["MensUsuario"] = null;
                Session["VoltaMsg"] = 0;
                Session["VoltarPesquisa"] = 0;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7.pdf";
                Session["VoltarIndicacao"] = 1;

                // Carrega view
                objetoInd = new INDICACAO();

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "INDICACAO", "Usuario", "MontarTelaIndicacao");
                return View(objetoInd);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarIndicacao(INDICACAO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                item.INDI_NM_INDICADO = CrossCutting.UtilitariosGeral.CleanStringDocto(item.INDI_NM_INDICADO);
                item.INDI_TX_MENSAGEM = CrossCutting.UtilitariosGeral.CleanStringDocto(item.INDI_TX_MENSAGEM);

                // Executa a operação
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                List<INDICACAO> listaObj = new List<INDICACAO>();
                Tuple<Int32, List<INDICACAO>, Boolean> volta = baseApp.ExecuteFilterTupleIndicacao(item.USUA_CD_ID, item.INDI_NM_INDICADO, item.INDI_DT_DATA, item.INDI_DT_DESFECHO, item.INDI_NM_EMAIL, item.INDI_IN_STATUS, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensUsuario"] = 1;
                    return RedirectToAction("MontarTelaIndicacao");
                }

                // Sucesso
                listaMasterInd = volta.Item2.ToList();
                listaMasterInd = listaMasterInd.Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                Session["ListaIndicacoes"] = listaMasterInd;
                return RedirectToAction("MontarTelaIndicacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroIndicacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Session["ListaIndicacoes"] = null;
                return RedirectToAction("MontarTelaIndicacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult EditarIndicacao(Int32 id)
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
                    if (usuario.USUA_IN_INDICA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Indicação";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Indicação - Edição";

                // Prepara view
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/7/Ajuda7_2.pdf";
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                INDICACAO item = baseApp.GetIndicacaoById(id);
                Session["IdIndicacao"] = item.INDI_CD_ID;
                IndicacaoViewModel vm = Mapper.Map<INDICACAO, IndicacaoViewModel>(item);
                Session["Indicacao"] = item;

                List<INDICACAO_ACAO> acoes = item.INDICACAO_ACAO.Where(p => p.INAC_IN_ATIVO == 1).ToList();
                ViewBag.Acoes = acoes;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "INDICACAO_EDITAR", "Usuario", "EditarIndicacao");
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditarIndicacao(IndicacaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.INDI_NM_INDICADO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.INDI_NM_INDICADO);
                    vm.INDI_TX_MENSAGEM = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.INDI_TX_MENSAGEM);

                    // Executa a operação
                    INDICACAO item = Mapper.Map<IndicacaoViewModel, INDICACAO>(vm);
                    Int32 volta = baseApp.ValidateEditIndicacao(item);

                    // Verifica retorno
                    Session["IdIndicacao"] = item.INDI_CD_ID;
                    Session["IndicacaoAlterada"] = 1;
                    Session["ListaIndicacoes"] = null;

                    // Monta Log
                    INDICACAO ind = baseApp.GetIndicacaoById(item.INDI_CD_ID);
                    String frase = ind.INDI_CD_ID.ToString() + "|" + ind.INDI_NM_INDICADO.ToString() + "|" + ind.USUA_CD_ID.ToString() + "|" + ind.INDI_DT_DATA.ToString() + "|" + ind.INDI_GU_IDENTIFICADOR + "|" + ind.INDI_TX_MENSAGEM;
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Indicação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = frase,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A indicação " + item.INDI_GU_IDENTIFICADOR + " de " + item.INDI_NM_INDICADO.ToUpper() + " foi alterada com sucesso.";
                    Session["MensUsuario"] = 61;

                    // Retorno
                    return RedirectToAction("MontarTelaIndicacao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Usuario";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ExcluirIndicacao(Int32 id)
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
                    if (usuario.USUA_IN_INDICA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Indicação";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                INDICACAO item = baseApp.GetIndicacaoById(id);
                item.INDI_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateEditIndicacao(item);

                Session["IndicacaoAlterada"] = 1;
                Session["ListaIndicacoes"] = null;
                Session["Indicacoes"] = null;

                // Processa mensagens
                INDICACAO indMsg = baseApp.GetIndicacaoById(item.INDI_CD_ID);
                Int32 voltaCons = await EnviarEMailIndicacao(indMsg, 2);

                // Monta Log
                INDICACAO cli = baseApp.GetIndicacaoById(item.INDI_CD_ID);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Indicação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Indicado: " + cli.INDI_NM_INDICADO + " | Data: " + item.INDI_DT_DATA.ToString() + " | Identificador: " + item.INDI_GU_IDENTIFICADOR,
                    //LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A indicação " + item.INDI_GU_IDENTIFICADOR + " de " + item.INDI_NM_INDICADO.ToUpper() + " foi excluída com sucesso.";
                Session["MensUsuario"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaIndicacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerIndicacao(Int32 id)
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
                    if (usuario.USUA_IN_INDICA == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Indicação";
                        return RedirectToAction("MontarTelaPaciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Indicação - Visualização";

                // Prepara view
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "INDICACAO_VER", "Usuario", "VerIndicacao");

                INDICACAO item = baseApp.GetIndicacaoById(id);
                IndicacaoViewModel vm = Mapper.Map<INDICACAO, IndicacaoViewModel>(item);
                Session["Indicacao"] = item;

                List<INDICACAO_ACAO> acoes = item.INDICACAO_ACAO.Where(p => p.INAC_IN_ATIVO == 1).ToList();
                ViewBag.Acoes = acoes;

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Usuario";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Usuario", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public async Task<ActionResult> ReenviarMail(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

            INDICACAO indMsg = baseApp.GetIndicacaoById(id);
            Int32 voltaCons = await EnviarEMailIndicacao(indMsg, 3);

            // Mensagem do CRUD
            Session["MsgCRUD"] = "A indicação " + indMsg.INDI_GU_IDENTIFICADOR + " para " + indMsg.INDI_NM_INDICADO.ToUpper() + " foi reenviada com sucesso.";
            Session["MensUsuario"] = 61;

            return RedirectToAction("MontarTelaIndicacao");
        }

        [HttpPost]
        public JsonResult GetUsuarios(String term)
        {
            List<USUARIO> usu = CarregaUsuario();
            List<String> nomes = usu.Select(p => p.USUA_NM_NOME).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n }) 
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }
    }
}
