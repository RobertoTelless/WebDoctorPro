using ApplicationServices.Interfaces;
using AutoMapper;
using Azure.Communication.Email;
using Canducci.Zip;
using CRMPresentation.App_Start;
using CrossCutting;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using EntitiesServices.WorkClasses;
using ERP_Condominios_Solution.Classes;
using ERP_Condominios_Solution.Controllers;
using ERP_Condominios_Solution.ViewModels;
using GEDSys_Presentation.App_Start;
using iText.IO.Codec;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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
using System.Web.UI;
using System.Xml.Linq;
using XidNet;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Image = iTextSharp.text.Image;

namespace GEDSys_Presentation.Controllers
{
    public class NoticiaController : Controller
    {
        private readonly INoticiaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        NOTICIA objeto = new NOTICIA();
        NOTICIA objetoAntes = new NOTICIA();
        List<NOTICIA> listaMaster = new List<NOTICIA>();
        String extensao;

        public NoticiaController(INoticiaAppService baseApps, ILogAppService logApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            usuApp = usuApps;
            confApp = confApps;
        }

        [HttpGet]
        public ActionResult MontarTelaNoticia()
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
                        Session["ModuloPermissao"] = "Noticia";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ModuloAtual"] = "Noticia";

                // Carrega listas
                if ((List<NOTICIA>)Session["ListaNoticia"] == null)
                {
                    listaMaster = CarregaNoticiaGeral();
                    Session["ListaNoticia"] = listaMaster;
                }
                ViewBag.Listas = (List<NOTICIA>)Session["ListaNoticia"];
                ViewBag.Title = "Notícias";

                // Indicadores
                ViewBag.Noticias = ((List<NOTICIA>)Session["ListaNoticia"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagem
                if (Session["MensNoticia"] != null)
                {
                    if ((Int32)Session["MensNoticia"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensNoticia"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Abre view
                objeto = new NOTICIA();
                Session["VoltaNoticia"] = 1;
                Session["MensNoticia"] = 0;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Noticia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroNoticiaGeral()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaNoticia"] = null;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoNoticiaGeral()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMaster = baseApp.GetAllItensAdm(idAss);
                Session["ListaNoticia"] = listaMaster;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarNoticiaGeral(NOTICIA item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> listaObj = new List<NOTICIA>();
                Tuple<Int32, List<NOTICIA>, Boolean> volta = baseApp.ExecuteFilter(item.NOTC_NM_TITULO, item.NOTC_NM_AUTOR, item.NOTC_DT_DATA_AUTOR, item.NOTC_TX_TEXTO, item.NOTC_LK_LINK, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensNoticia"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }

                // Sucesso
                Session["MensNoticia"] = 0;
                listaMaster = volta.Item2;
                Session["ListaNoticia"] = volta.Item2;
                return RedirectToAction("MontarTelaNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Notícia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaNoticia");
        }

        [HttpGet]
        public ActionResult IncluirNoticia()
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
            Session["ModuloAtual"] = "Locacao - Inclusão";
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Prepara view
            NOTICIA item = new NOTICIA();
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.NOTC_DT_EMISSAO = DateTime.Today.Date;
            vm.NOTC_IN_ATIVO = 1;
            vm.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.NOTC_NR_ACESSO = 0;
            vm.NOTC_IN_SISTEMA = 6;           
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirNoticia(NoticiaViewModel vm)
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
                    vm.NOTC_NM_AUTOR = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_AUTOR);
                    vm.NOTC_NM_ORIGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_ORIGEM);
                    vm.NOTC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_TITULO);
                    vm.NOTC_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_TX_TEXTO);
                    vm.NOTC_AQ_ARQUIVO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_AQ_ARQUIVO);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    Session["IdNoticia"] = item.NOTC_CD_ID;

                    // Carrega foto e processa alteracao
                    item.NOTC_AQ_FOTO = "~/Images/p_big2.jpg";
                    volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    if (Session["FileQueueNoticia"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueNoticia"];
                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                            }
                            else
                            {
                                UploadFotoQueueNoticia(file);
                            }
                        }

                        Session["FileQueueNoticia"] = null;
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi incluída com sucesso";
                    Session["MensNoticia"] = 61;

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["IdVolta"] = item.NOTC_CD_ID;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
            Session["FileQueueNoticia"] = queue;
        }

        [HttpPost]
        public Int32 UploadFotoQueueNoticia(FileQueue file)
        {
            try
            {
                // Inicializa
                Int32 idNot = (Int32)Session["IdNoticia"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensPaciente"] = 5;
                    return 1;
                }

                // Recupera noticia
                NOTICIA item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 250)
                {
                    Session["MensNoticia"] = 6;
                    return 2;
                }

                // Critica tamanho arquivo
                var fileSize = file.Contents.Length;
                if (fileSize > 50000000)
                {
                    Session["MensNoticia"] = 7;
                    return 3;
                }


                // Copia imagem
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item.NOTC_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Noticia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        [HttpGet]
        public ActionResult EditarNoticia(Int32 id)
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
            Session["ModuloAtual"] = "Locacao - Inclusão";
            CONFIGURACAO conf = CarregaConfiguracaoGeral();

            // Mensagens
            if (Session["MensNoticia"] !=  null)
            {
                if ((Int32)Session["MensNoticia"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensNoticia"] == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            NOTICIA item = baseApp.GetItemById(id);
            NoticiaViewModel vm = Mapper.Map<NOTICIA, NoticiaViewModel>(item);
            Session["Noticia"] = item;
            Session["IdNoticia"] = id;
            Session["MensNoticia"] = null;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarNoticia(NoticiaViewModel vm)
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
                    vm.NOTC_NM_AUTOR = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_AUTOR);
                    vm.NOTC_NM_ORIGEM = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_ORIGEM);
                    vm.NOTC_NM_TITULO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_NM_TITULO);
                    vm.NOTC_TX_TEXTO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_TX_TEXTO);
                    vm.NOTC_AQ_ARQUIVO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.NOTC_AQ_ARQUIVO);

                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    NOTICIA item = Mapper.Map<NoticiaViewModel, NOTICIA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateEdit(item, (NOTICIA)Session["Noticia"], usuarioLogado);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi alterada com sucesso";
                    Session["MensNoticia"] = 61;

                    // Sucesso
                    listaMaster = new List<NOTICIA>();
                    Session["ListaNoticia"] = null;
                    Session["VoltaNoticia"] = 1;
                    Session["IdNoticiaVolta"] = item.NOTC_CD_ID;
                    Session["Noticia"] = item;
                    Session["IdVolta"] = item.NOTC_CD_ID;
                    Session["MensNoticia"] = 0;
                    Session["NoticiaAlterada"] = 1;
                    return RedirectToAction("MontarTelaNoticia");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Notícia";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Notícia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirNoticia(Int32 id)
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

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                NOTICIA item = baseApp.GetItemById(id);
                item.NOTC_IN_ATIVO = 0;
                Int32 volta = baseApp.ValidateDelete(item, usuarioLogado);

                Session["NoticiaAlterada"] = 1;
                Session["ListaNoticia"] = null;

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A notícia " + item.NOTC_NM_TITULO.ToUpper() + " foi excluída com sucesso";
                Session["MensNoticia"] = 61;

                // Retorno
                return RedirectToAction("MontarTelaLocacao");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Locacao";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Locacao", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult UploadFotoNoticia(HttpPostedFileBase file)
        {
            try
            {
                // Inicializa
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idNot = (Int32)Session["IdNoticia"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                if (file == null)
                {
                    Session["MensNoticia"] = 5;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // Recupera noticia
                NOTICIA item = baseApp.GetById(idNot);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 250)
                {
                    Session["MensNoticia"] = 6;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensNoticia"] = 7;
                    return RedirectToAction("VoltarAnexoNoticia");
                }

                // Copia imagem
                String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/Noticias/" + item.NOTC_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                item.NOTC_AQ_FOTO = "~" + caminho + fileName;
                objetoAntes = item;
                Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
                listaMaster = new List<NOTICIA>();
                Session["ListaNoticia"] = null;
                Session["NoticiaAlterada"] = 1;

                return RedirectToAction("VoltarAnexoNoticia");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Noticia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarAnexoNoticia()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNoticia", new { id = idNot });
        }














        public List<NOTICIA> CarregaNoticiaGeral()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<NOTICIA> conf = new List<NOTICIA>();
                if (Session["NoticiaGeral"] == null)
                {
                    conf = baseApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["NoticiaAlterada"] == 1)
                    {
                        conf = baseApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<NOTICIA>)Session["NoticiaGeral"];
                    }
                }
                Session["NoticiaGeral"] = conf;
                Session["NoticiaAlterada"] = 0;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Noticia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Noticia";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Noticia", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

    }
}