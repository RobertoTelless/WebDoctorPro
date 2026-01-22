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
    public class EstoqueController : Controller
    {
        private readonly IProdutoAppService prodApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaProdutoAppService cpApp;
        private readonly IEmpresaAppService filApp;
        private readonly ISubcategoriaProdutoAppService scpApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IEmpresaAppService empApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly IPagamentoAppService pagApp;
        private readonly ILocacaoAppService locApp;
        private readonly ITipoPagamentoAppService tpApp;

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        ProdutoViewModel objetoProdView = new ProdutoViewModel();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        PRODUTO_FALHA objetoFalha = new PRODUTO_FALHA();
        PRODUTO_FALHA objetoAntesFalha = new PRODUTO_FALHA();
        List<PRODUTO_FALHA> listaMasterFalha = new List<PRODUTO_FALHA>();
        String extensao;

        public EstoqueController(
            IProdutoAppService prodApps
            , ILogAppService logApps
            , IUnidadeAppService unApps
            , ICategoriaProdutoAppService cpApps
            , IEmpresaAppService filApps
            , ISubcategoriaProdutoAppService scpApps
            , IConfiguracaoAppService confApps
            , IUsuarioAppService usuApps, IEmpresaAppService empApps, IMensagemEnviadaSistemaAppService meApps, IRecursividadeAppService recuApps, IPagamentoAppService pagApps, ILocacaoAppService locApps, ITipoPagamentoAppService tpApps)
        {
            prodApp = prodApps;
            logApp = logApps;
            unApp = unApps;
            cpApp = cpApps;
            filApp = filApps;
            scpApp = scpApps;
            confApp = confApps;
            usuApp = usuApps;
            empApp = empApps;
            Trace.Listeners.Add(new TextWriterTraceListener("~/logfile.txt"));
            Trace.AutoFlush = true;
            meApp = meApps;
            recuApp = recuApps;
            pagApp = pagApps;
            locApp = locApps;
            tpApp = tpApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            PRODUTO item = new PRODUTO();
            ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPaciente", "Paciente");
        }

        [HttpPost]
        public JsonResult BuscaNome(String nome)
        {
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRODUTO> lista = CarregarProduto();

            if (nome != null)
            {
                List<PRODUTO> lstProduto = lista.Where(x => x.PROD_NM_NOME != null && x.PROD_NM_NOME.ToLower().Contains(nome.ToLower())).ToList<PRODUTO>();

                if (lstProduto != null)
                {
                    foreach (var item in lstProduto)
                    {
                        Hashtable result = new Hashtable();
                        result.Add("id", item.PROD_CD_ID);
                        result.Add("text", item.PROD_NM_NOME);
                        listResult.Add(result);
                    }
                }
            }
            Session["Produtos"] = lista;
            return Json(listResult);
        }

        [HttpGet]
        public ActionResult MontarTelaEstoque()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_ESTOQUE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Estoque";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }   
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega configuração
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                ViewBag.Linhas = conf.CONF_NR_GRID_PRODUTO.Value;
                Int32 carga = 0;

                // Carrega Produtos
                if (Session["ListaEstoqueBase"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.OrderBy(p => p.PROD_IN_TIPO_PRODUTO).ThenBy(p => p.PROD_NM_NOME).ToList();
                    Session["ListaEstoqueBase"] = listaMasterProd;
                    Session["BuscaEstoque"] = 1;
                    carga = 1;
                }
                ViewBag.Busca = (Int32)Session["BuscaEstoque"];
                String perfil = usuario.PERFIL.PERF_SG_SIGLA;
                listaMasterProd = (List<PRODUTO>)Session["ListaEstoqueBase"];
                listaMasterProd = listaMasterProd.OrderBy(p => p.PROD_IN_TIPO_PRODUTO).ThenBy(p => p.PROD_NM_NOME).ToList();

                // Carrega Listas
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_SISTEMA == 6).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_SISTEMA == 6).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaEstoqueBase"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                ViewBag.Listas = ((List<PRODUTO>)Session["ListaEstoqueBase"]);

                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> sits = new List<SelectListItem>();
                sits.Add(new SelectListItem() { Text = "Exibir Todos", Value = "0" });
                sits.Add(new SelectListItem() { Text = "Estoque acima do máximo", Value = "1" });
                sits.Add(new SelectListItem() { Text = "Estoque abaixo do mínimo", Value = "2" });
                sits.Add(new SelectListItem() { Text = "Estoque zerado", Value = "3" });
                sits.Add(new SelectListItem() { Text = "Estoque esgotando em 30 dias", Value = "4" });
                ViewBag.Sits = new SelectList(sits, "Value", "Text");

                // Carrega widgets
                List<PRODUTO> prods = (List<PRODUTO>)Session["ListaEstoqueBase"];
                Int32 acimaMax = prods.Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO).ToList().Count;
                Int32 abaixoMin = prods.Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO).ToList().Count;
                Int32 zerado = prods.Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0).ToList().Count;
                prods = prods.Where(p => p.PROD_VL_MEDIA_VENDA_MENSAL > 0).ToList();
                Int32 esgota30 = prods.Where(p => (p.PROD_VL_ESTOQUE_ATUAL.Value / p.PROD_VL_MEDIA_VENDA_MENSAL.Value) * 30 < 30).ToList().Count;

                ViewBag.Acima = acimaMax;
                ViewBag.Abaixo = abaixoMin;
                ViewBag.Zerado = zerado;
                ViewBag.Esgota = esgota30;

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0261", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0412", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0260", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 50)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0085", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 51)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0349", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 20)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0086", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 21)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0087", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 22)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0448", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 99)
                    {
                        ModelState.AddModelError("", "Foram processados e atualizados " + ((Int32)Session["Conta"]).ToString() + " movimentos");
                        ModelState.AddModelError("", "Foram processados e bloqueados para autorização " + ((Int32)Session["Autoriza"]).ToString() + " movimentos");
                        if ((Int32)Session["Falha"] > 0)
                        {
                            ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " movimentos");
                            ModelState.AddModelError("", "Foi gerada planilha com as falhas encontradas " + ((String)Session["NomePlanilha"]));
                        }
                    }
                    if ((Int32)Session["MensProduto"] == 111)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0464", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 112)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0465", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 113)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0468", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0312", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 201)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0479", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 114)
                    {
                        String frase = CRMSys_Base.ResourceManager.GetString("M0473", CultureInfo.CurrentCulture) + " " + (String)Session["NomePlanilha"] + ". ";
                        frase += CRMSys_Base.ResourceManager.GetString("M0474", CultureInfo.CurrentCulture);
                        ModelState.AddModelError("", frase);
                    }
                    if ((Int32)Session["MensProduto"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                objetoProd.PROD_IN_COMPOSTO = 0;
                Session["VoltaProduto"] = 70;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["TipoListagem"] = 0;
                Session["Clonar"] = 0;
                Session["ListaMovimEstoque"] = null;
                Session["ListaMovimentoEstoque"] = null;
                Session["ListaFilialEstoque"] = null;
                Session["ListaHistoricoEstoque"] = null;
                Session["AbaProduto"] = 9;
                Session["VoltaProdutoWidget"] = 2;
                if (Session["FiltroEstoque"] != null)
                {
                    objetoProd = (PRODUTO)Session["FiltroEstoque"];
                }
                return View(objetoProd);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroEstoque()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaEstoqueBase"] = null;
                Session["FiltroEstoque"] = null;
                return RedirectToAction("MontarTelaEstoque");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTodosEstoque()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterProd = prodApp.GetAllItens(idAss);
                Session["FiltroEstoque"] = null;
                Session["ListaEstoqueBase"] = listaMasterProd;
                Session["BuscaEstoque"] = 2;
                return RedirectToAction("MontarTelaEstoque");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarEstoque(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroEstoque"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaEstoque");
                }

                // Sucesso
                Session["MensProduto"] = 0;
                if (item.PROD_IN_COMPOSTO == 0)
                {
                    listaMasterProd = volta.Item2;
                }
                else if (item.PROD_IN_COMPOSTO == 1)
                {
                    List<PRODUTO> acima = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = acima;
                }
                else if (item.PROD_IN_COMPOSTO == 2)
                {
                    List<PRODUTO> abaixo = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = abaixo;
                }
                else if (item.PROD_IN_COMPOSTO == 3)
                {
                    List<PRODUTO> zerado = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = zerado;
                }
                else if (item.PROD_IN_COMPOSTO == 4)
                {
                    List<PRODUTO> esgota = (volta.Item2).Where(p => p.PROD_VL_MEDIA_VENDA_MENSAL > 0).ToList();
                    esgota = esgota.Where(p => (p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = esgota;
                }
                else
                {
                    listaMasterProd = volta.Item2;
                }
                Session["ListaEstoqueBase"] = listaMasterProd;
                Session["BuscaEstoque"] = 3;
                Session["TipoListagem"] = item.PROD_IN_COMPOSTO;

                // Volta
                return RedirectToAction("MontarTelaEstoque");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VoltarBaseEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaProduto"] == 999)
            {
                return RedirectToAction("VerMovimentacoes", "Estoque");
            }
            if ((Int32)Session["VoltaProduto"] == 888)
            {
                return RedirectToAction("VerMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            if ((Int32)Session["VoltaProduto"] == 777)
            {
                return RedirectToAction("VerMovimentacaoEstoque", new { id = (Int32)Session["IdMovimentacao"] });
            }
            if ((Int32)Session["VoltaProduto"] == 717)
            {
                return RedirectToAction("MontarTelaPaciente", "Paciente");
            }
            if ((Int32)Session["VoltaProduto"] == 997)
            {
                return RedirectToAction("VerMovimentacoesPendentes");
            }
            return RedirectToAction("MontarTelaEstoque");
        }

        public ActionResult VoltarBaseEstoqueDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEstoque");
        }

        public ActionResult VoltarExcluidoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
        }

        public ActionResult VoltarMovimentacaoEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("VerMovimentacaoEstoque", new { id = (Int32)Session["IdMovimentacao"] });
        }

        public ActionResult VoltarEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaEstoque");
        }

        [HttpGet]
        public ActionResult VerMovimentacaoProduto(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega produto
                PRODUTO prod = prodApp.GetItemById(id);
                ViewBag.Nome = prod.PROD_NM_NOME;
                Session["NomeProduto"] = prod.PROD_NM_NOME;
                Session["IdProdutoAuto"] = prod.PROD_CD_ID;
                if (Session["ListaMovimEstoque"] == null)
                {
                    List<MOVIMENTO_ESTOQUE_PRODUTO> movs = prod.MOVIMENTO_ESTOQUE_PRODUTO.ToList();
                    movs = movs.Where(p => p.MOEP_IN_ATIVO == 1).ToList();
                    Session["ListaMovimEstoque"] = movs;
                }
                ViewBag.Listas = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimEstoque"];

                // Prepara lista
                List<SelectListItem> es = new List<SelectListItem>();
                es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
                es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
                ViewBag.ES = new SelectList(es, "Value", "Text");
                List<SelectListItem> entradas = new List<SelectListItem>();
                entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
                entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
                entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
                entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
                ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
                List<SelectListItem> saidas = new List<SelectListItem>();
                saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
                saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
                saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
                saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
                ViewBag.Saidas = new SelectList(saidas, "Value", "Text");

                List<SelectListItem> pend = new List<SelectListItem>();
                pend.Add(new SelectListItem() { Text = "Não", Value = "0" });
                pend.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Pendente = new SelectList(pend, "Value", "Text");

                ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["TipoInclusao"] = 1;

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 13)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0460", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 14)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0461", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                }

                // Abre view
                Session["VoltaProduto"] = 888;
                Session["FlagAutorizacao"] = 2;
                MOVIMENTO_ESTOQUE_PRODUTO objeto = new MOVIMENTO_ESTOQUE_PRODUTO();
                objeto.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                objeto.MOEP_DT_DATA_DUMMY = DateTime.Today.Date;
                objeto.MOEP_IN_TIPO_MOVIMENTO = 0;
                objeto.MOEP_IN_TIPO = 0;
                objeto.USUA_CD_ID = 0;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMovimentoProduto(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<SelectListItem> es = new List<SelectListItem>();
            es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.ES = new SelectList(es, "Value", "Text");
            List<SelectListItem> entradas = new List<SelectListItem>();
            entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
            entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
            entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
            entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
            ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
            List<SelectListItem> saidas = new List<SelectListItem>();
            saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
            saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
            saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
            saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
            ViewBag.Saidas = new SelectList(saidas, "Value", "Text");

            List<SelectListItem> pend = new List<SelectListItem>();
            pend.Add(new SelectListItem() { Text = "Não", Value = "0" });
            pend.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            ViewBag.Pendente = new SelectList(pend, "Value", "Text");

            ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            try
            {
                // Executa a operação
                CRMSysDBEntities Db = new CRMSysDBEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();

                // Monta condição
                if (item.MOEP_IN_TIPO_MOVIMENTO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == item.MOEP_IN_TIPO_MOVIMENTO);
                }
                if (item.MOEP_IN_TIPO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_TIPO);
                }
                if (item.MOEP_IN_OPERACAO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_OPERACAO);
                }
                if (item.MOEP_IN_PENDENTE > 0)
                {
                    query = query.Where(p => p.MOEP_IN_PENDENTE == item.MOEP_IN_PENDENTE);
                }
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY == null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 == null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1) & DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DS_JUSTIFICATIVA != null)
                {
                    query = query.Where(p => p.PRODUTO.PROD_NM_NOME.ToUpper().Contains(item.MOEP_DS_JUSTIFICATIVA.ToUpper()));
                }
                if (item.MOEP_GU_GUID != null)
                {
                    query = query.Where(p => p.MOEP_GU_GUID == item.MOEP_GU_GUID);
                }
                Int32 id = (Int32)Session["IdProdutoAuto"];
                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.PROD_CD_ID == id);
                    query = query.Where(p => p.MOEP_IN_ATIVO == 1);
                    query = query.OrderByDescending(a => a.MOEP_DT_MOVIMENTO);
                    lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                // Sucesso
                Session["ListaMovimEstoque"] = lista;
                Session["FiltroMovimEstoque"] = item;
                return RedirectToAction("VerMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMovimentoProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMovimEstoque"] = null;
                return RedirectToAction("VerMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMovimentoExcluidoProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMovimentoEstoqueExcluidoProduto"] = null;
                return RedirectToAction("VerMovimentoExcluidoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerMovimentacoes()
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
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega lista
                if (Session["ListaMovimentoEstoque"] == null)
                {
                    List<MOVIMENTO_ESTOQUE_PRODUTO> movs = prodApp.GetAllMovimentos(idAss).ToList();
                    movs = movs.Where(p => p.MOEP_IN_ATIVO == 1).ToList();
                    movs = movs.OrderByDescending(p => p.MOEP_DT_MOVIMENTO).ThenBy(p => p.PROD_CD_ID).ToList();
                    Session["ListaMovimentoEstoque"] = movs;
                }
                ViewBag.Listas = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoEstoque"];

                // Prepara listas
                List<SelectListItem> es = new List<SelectListItem>();
                es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
                es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
                ViewBag.ES = new SelectList(es, "Value", "Text");
                List<SelectListItem> entradas = new List<SelectListItem>();
                entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
                entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
                entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
                entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
                ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
                List<SelectListItem> saidas = new List<SelectListItem>();
                saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
                saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
                saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
                saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
                ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
                List<SelectListItem> pend = new List<SelectListItem>();
                pend.Add(new SelectListItem() { Text = "Não", Value = "0" });
                pend.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                ViewBag.Pendente = new SelectList(pend, "Value", "Text");
                ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["TipoInclusao"] = 2;

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 13)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0460", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 14)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0461", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                }

                // Abre view
                Session["VoltaProduto"] = 999;
                Session["FlagAutorizacao"] = 1;
                Session["IdProdutoAuto"] = 0;
                Session["VoltaProduto"] = 999;
                MOVIMENTO_ESTOQUE_PRODUTO objeto = new MOVIMENTO_ESTOQUE_PRODUTO();
                String dataInicio = "01/" + DateTime.Today.Date.Month.ToString() + "/" + DateTime.Today.Date.Year.ToString();
                objeto.MOEP_DT_DATA_DUMMY_1 = Convert.ToDateTime(dataInicio);
                objeto.MOEP_DT_DATA_DUMMY = DateTime.Today.Date;
                objeto.MOEP_IN_TIPO_MOVIMENTO = 0;
                objeto.MOEP_IN_TIPO = 0;
                objeto.USUA_CD_ID = 0;
                objeto.MOEP_IN_PENDENTE = 0;
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMovimento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMovimentoEstoque"] = null;
                return RedirectToAction("VerMovimentacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerMovimentoExcluido()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                if (Session["ListaMovimentoEstoqueExcluido"] == null)
                {
                    List<MOVIMENTO_ESTOQUE_PRODUTO> movs = prodApp.GetAllMovimentosAdm(idAss).ToList();
                    movs = movs.Where(p => p.MOEP_IN_ATIVO == 0).ToList();
                    movs = movs.OrderByDescending(p => p.MOEP_DT_MOVIMENTO).ThenBy(p => p.PROD_CD_ID).ToList();
                    Session["ListaMovimentoEstoqueExcluido"] = movs;
                }
                ViewBag.Listas = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoEstoqueExcluido"];

                // Prepara listas
                List<SelectListItem> es = new List<SelectListItem>();
                es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
                es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
                ViewBag.ES = new SelectList(es, "Value", "Text");
                List<SelectListItem> entradas = new List<SelectListItem>();
                entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
                entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
                entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
                entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
                ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
                List<SelectListItem> saidas = new List<SelectListItem>();
                saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
                saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
                saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
                saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
                ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
                ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult RetirarFiltroMovimentoExcluido()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMovimentoEstoqueExcluido"] = null;
                Session["FiltroMovimentoEstoqueExcluido"] = null;
                return RedirectToAction("VerMovimentoExcluido");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult VerMovimentoExcluidoProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Carrega produto
                PRODUTO prod = prodApp.GetItemById((Int32)Session["IdProdutoAuto"]);
                ViewBag.Nome = prod.PROD_NM_NOME;

                if (Session["ListaMovimentoEstoqueExcluidoProduto"] == null)
                {
                    List<MOVIMENTO_ESTOQUE_PRODUTO> movs = prodApp.GetAllMovimentosAdm(idAss).ToList();
                    movs = movs.Where(p => p.MOEP_IN_ATIVO == 0).ToList();
                    movs = movs.Where(p => p.PROD_CD_ID == prod.PROD_CD_ID).ToList();
                    movs = movs.OrderByDescending(p => p.MOEP_DT_MOVIMENTO).ThenBy(p => p.PROD_CD_ID).ToList();
                    Session["ListaMovimentoEstoqueExcluidoProduto"] = movs;
                }
                ViewBag.Listas = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoEstoqueExcluidoProduto"];

                // Prepara listas
                List<SelectListItem> es = new List<SelectListItem>();
                es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
                es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
                ViewBag.ES = new SelectList(es, "Value", "Text");
                List<SelectListItem> entradas = new List<SelectListItem>();
                entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
                entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
                entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
                entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
                ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
                List<SelectListItem> saidas = new List<SelectListItem>();
                saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
                saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
                saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
                saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
                ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
                ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMovimento(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                CRMSysDBEntities Db = new CRMSysDBEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();

                // Monta condição
                if (item.MOEP_IN_TIPO_MOVIMENTO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == item.MOEP_IN_TIPO_MOVIMENTO);
                }
                if (item.MOEP_IN_TIPO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_TIPO);
                }
                if (item.MOEP_IN_OPERACAO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_OPERACAO);
                }
                if (item.MOEP_IN_PENDENTE > 0)
                {
                    query = query.Where(p => p.MOEP_IN_PENDENTE == item.MOEP_IN_PENDENTE);
                }
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY == null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 == null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1) & DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DS_JUSTIFICATIVA != null)
                {
                    query = query.Where(p => p.PRODUTO.PROD_NM_NOME.ToUpper().Contains(item.MOEP_DS_JUSTIFICATIVA.ToUpper()));
                }
                if (item.MOEP_GU_GUID != null)
                {
                    query = query.Where(p => p.MOEP_GU_GUID == item.MOEP_GU_GUID);
                }

                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.MOEP_IN_ATIVO == 1);
                    query = query.OrderByDescending(a => a.MOEP_DT_MOVIMENTO);
                    lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                // Sucesso
                Session["ListaMovimentoEstoque"] = lista;
                return RedirectToAction("VerMovimentacoes");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult VerHistoricoEstoque(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega produto
                PRODUTO prod = prodApp.GetItemById(id);
                ViewBag.Nome = prod.PROD_NM_NOME;

                // Verifica
                if (prod.PRODUTO_ESTOQUE_HISTORICO.Count == 0)
                {
                    Session["MensProduto"] = 22;
                    return RedirectToAction("MontarTelaEstoque");
                }

                // Monta lista
                List<PRODUTO_ESTOQUE_HISTORICO> hist = prod.PRODUTO_ESTOQUE_HISTORICO.ToList();
                hist = hist.Where(p => p.PREH_IN_ATIVO == 1 & p.PREH_IN_PENDENTE == 0).OrderByDescending(p => p.PREH_DT_COMPLETA).ToList();
                Session["ListaHistoricoEstoque"] = hist;
                ViewBag.Listas = hist;

                // Recupera evolucao
                List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
                foreach (PRODUTO_ESTOQUE_HISTORICO item in hist)
                {
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.DataEmissao = item.PREH_DT_DATA.Value;
                    mod1.ValorDec1 = item.PREH_QN_ESTOQUE_TOTAL.Value;
                    if (prod.PROD_IN_TIPO_PRODUTO == 2)
                    {
                        mod1.ValorDec2 = item.PREH_QN_ESTOQUE_TOTAL.Value * prod.PROD_VL_PRECO_VENDA.Value;
                    }
                    else
                    {
                        mod1.ValorDec2 = 0;
                    }
                    lista1.Add(mod1);
                }
                lista1 = lista1.OrderBy(p => p.DataEmissao).ToList();
                Session["ListaCusto"] = lista1;

                // Prepara lista
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                }

                // Abre view
                Session["VoltaProduto"] = 999;
                PRODUTO_ESTOQUE_HISTORICO objeto = new PRODUTO_ESTOQUE_HISTORICO();
                return View(objeto);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public JsonResult GetDadosHistorico()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCusto"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                listaCP1 = listaCP1.OrderBy(p => p.DataEmissao).ToList();
                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                    valor2.Add(item.ValorDec2);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valoresHist", valor1);
                result.Add("valoresVenda", valor2);
                return Json(result);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult IncluirMovimentacao(Int32 id)
        {
            Session["TipoInclusao"] = 1;
            return RedirectToAction("IncluirMovimentacaoProduto", new { id = id });
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoProd()
        {
            Session["TipoInclusao"] = 1;
            return RedirectToAction("IncluirMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"]});
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoGeral()
        {
            Session["TipoInclusao"] = 2;
            Int32 id = 0;
            return RedirectToAction("IncluirMovimentacaoProduto", new { id = id });
        }

        [HttpGet]
        public ActionResult IncluirMovimentacaoProduto(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_ATUALIZA_ESTOQUE == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Estoque";
                        return RedirectToAction("MontarTelaestoque", "Estoque");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera tipo
                ViewBag.TipoInclusao = (Int32)Session["TipoInclusao"];

                // Recupera produto
                PRODUTO prod = new PRODUTO();
                if ((Int32)Session["TipoInclusao"] == 1)
                {
                    prod = prodApp.GetItemById(id);
                    ViewBag.Nome = prod.PROD_NM_NOME;
                    ViewBag.Codigo = prod.PROD_CD_CODIGO;
                }

                // Prepara listas
                List<SelectListItem> es = new List<SelectListItem>();
                es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
                es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
                ViewBag.ES = new SelectList(es, "Value", "Text");
                List<SelectListItem> entradas = new List<SelectListItem>();
                entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
                entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
                entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
                entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
                ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
                List<SelectListItem> saidas = new List<SelectListItem>();
                saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
                saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
                saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
                saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
                ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
                List<SelectListItem> cp = new List<SelectListItem>();
                cp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                cp.Add(new SelectListItem() { Text = "Não", Value = "2" });
                ViewBag.CP = new SelectList(cp, "Value", "Text");
                List<SelectListItem> quita = new List<SelectListItem>();
                quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                quita.Add(new SelectListItem() { Text = "Não", Value = "2" });
                ViewBag.Quitado = new SelectList(quita, "Value", "Text");
                ViewBag.Usuarios = new SelectList(CarregarUsuario().Where(p => p.PERFIL.PERF_IN_ACERTO_ESTOQUE == 1).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.TipoPag = new SelectList(CarregaTipoPagamento().OrderBy(p => p.TIPA_NM_PAGAMENTO), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Mensagens
                if (Session["MensEstoque"] != null)
                {
                    if ((Int32)Session["MensEstoque"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0449", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0450", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0451", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0452", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0453", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0454", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0455", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0456", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0457", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0458", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 11)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0459", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 12)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0462", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 13)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0463", CultureInfo.CurrentCulture));
                        Session["MensEstoque"] = 0;
                    }
                    if ((Int32)Session["MensEstoque"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Prepara view
                ViewBag.TipoInclusao = (Int32)Session["TipoInclusao"];
                MOVIMENTO_ESTOQUE_PRODUTO item = new MOVIMENTO_ESTOQUE_PRODUTO();
                MovimentoEstoqueProdutoViewModel vm = Mapper.Map<MOVIMENTO_ESTOQUE_PRODUTO, MovimentoEstoqueProdutoViewModel>(item);
                vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
                vm.EMPR_CD_ID = usuario.EMPR_CD_ID;
                vm.EMFI_CD_ID = usuario.EMFI_CD_ID;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                vm.MOEP_IN_TIPO_MOVIMENTO = 0;
                vm.MOEP_IN_TIPO = 0;
                vm.MOEP_QN_QUANTIDADE = 0;
                vm.MOEP_DS_PRODUTO = prod.PROD_NM_NOME;
                vm.MOEP_DS_FILIAL = usuario.EMPRESA_FILIAL.EMFI_NM_APELIDO;
                if ((Int32)Session["TipoInclusao"] == 1)
                {
                    vm.MOEP_VL_QUANTIDADE_ANTERIOR = prod.PROD_VL_ESTOQUE_ATUAL;
                    vm.PROD_CD_ID = prod.PROD_CD_ID;
                }
                else
                {
                    vm.MOEP_VL_QUANTIDADE_ANTERIOR = 0;
                    vm.PROD_CD_ID = null;
                }
                vm.MOEP_DT_LANCAMENTO = DateTime.Today.Date;
                vm.MOEP_DT_PAGAMENTO = DateTime.Today.Date;
                vm.MOEP_IN_ORIGEM = String.Empty;
                vm.MOEP_IN_ATIVO = 1;
                vm.MOEP_IN_OPERACAO = 0;
                vm.MOEP_IN_AUTORIZADOR_T = 0;
                vm.MOEP_IN_AUTORIZADOR = 0;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult IncluirMovimentacaoProduto(MovimentoEstoqueProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> es = new List<SelectListItem>();
            es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.ES = new SelectList(es, "Value", "Text");
            List<SelectListItem> entradas = new List<SelectListItem>();
            entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
            entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
            entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
            entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
            ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
            List<SelectListItem> saidas = new List<SelectListItem>();
            saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
            saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
            saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
            saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
            ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
            List<SelectListItem> cp = new List<SelectListItem>();
            cp.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            cp.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.CP = new SelectList(cp, "Value", "Text");
            ViewBag.Usuarios = new SelectList(CarregarUsuario().Where(p => p.PERFIL.PERF_IN_ACERTO_ESTOQUE == 1).OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            List<SelectListItem> quita = new List<SelectListItem>();
            quita.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            quita.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Quitado = new SelectList(quita, "Value", "Text");
            ViewBag.TipoPag = new SelectList(CarregaTipoPagamento().OrderBy(p => p.TIPA_NM_PAGAMENTO), "TIPA_CD_ID", "TIPA_NM_PAGAMENTO");

            // LISTA DE VENDAS ================
            String origem = String.Empty;
            String tipo = String.Empty;
            Int32 somaFinal = 0;
            Int32 logRet = 0;
            Int32 mensagem = 0;
            ModeloViewModel modelo = new ModeloViewModel();

            if (ModelState.IsValid)
            {
                try
                {
                    // Critica produto
                    if ((Int32)Session["TipoInclusao"] == 2)
                    {
                        if (vm.PROD_CD_ID == 0 || vm.PROD_CD_ID == null)
                        {
                            Session["MensEstoque"] = 11;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0459", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }

                    // Recupera produto e usuario
                    PRODUTO prod = new PRODUTO();
                    USUARIO usu = usuApp.GetItemById(vm.USUA_CD_ID);
                    if ((Int32)Session["TipoInclusao"] == 2)
                    {
                        prod = prodApp.GetItemById(vm.PROD_CD_ID.Value);
                    }
                    else
                    {
                        prod = prodApp.GetItemById(vm.PROD_CD_ID.Value);
                    }
                    Session["ProdAntes"] = prod;

                    // Critica de entradas
                    Int32 tipoMov = vm.MOEP_IN_TIPO_MOVIMENTO;
                    if (vm.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        // Critica tipo de movimento
                        if (vm.MOEP_IN_TIPO == 0 || vm.MOEP_IN_TIPO == null)
                        {
                            Session["MensEstoque"] = 13;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0463", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Critica quantidade
                        if (vm.MOEP_QN_QUANTIDADE == 0)
                        {
                            Session["MensEstoque"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0450", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.MOEP_QN_QUANTIDADE <= 0)
                        {
                            Session["MensEstoque"] = 6;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0454", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Compra
                        if (vm.MOEP_IN_TIPO == 1)
                        {
                            origem = "Compra Manual";
                            if (vm.MOEP_IN_OPERACAO == 0)
                            {
                                vm.MOEP_IN_OPERACAO = 2;
                            }
                            if (vm.MOEP_IN_OPERACAO == 1)
                            {
                                if (vm.MOEP_IN_AUTORIZADOR == 0)
                                {
                                    vm.MOEP_IN_AUTORIZADOR = 2;
                                }
                                if (vm.MOEP_IN_AUTORIZADOR_T == 0 || vm.MOEP_IN_AUTORIZADOR_T == null)
                                {
                                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0721", CultureInfo.CurrentCulture));
                                    return View(vm);
                                }
                            }

                        }

                        // Devolucao
                        if (vm.MOEP_IN_TIPO == 2)
                        {
                            origem = "Devolução de Venda";
                        }

                        // Retorno de manutenção
                        if (vm.MOEP_IN_TIPO == 3)
                        {
                            origem = "Retorno de Manuteção - Entrada";
                        }

                        // Ajuste manual
                        if (vm.MOEP_IN_TIPO == 4)
                        {
                            origem = "Ajuste Manual - Entrada";
                        }
                    }

                    // Critica de saidas
                    if (vm.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        // Critica tipo de movimento
                        if (vm.MOEP_IN_TIPO_1 == 0 || vm.MOEP_IN_TIPO_1 == null)
                        {
                            Session["MensEstoque"] = 13;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0463", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Critica quantidade
                        if (vm.MOEP_QN_QUANTIDADE == 0)
                        {
                            Session["MensEstoque"] = 2;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0450", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.MOEP_QN_QUANTIDADE <= 0)
                        {
                            Session["MensEstoque"] = 6;
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0454", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Critica possibilidade de saida
                        if (vm.MOEP_QN_QUANTIDADE > prod.PROD_VL_ESTOQUE_ATUAL)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0690", CultureInfo.CurrentCulture));
                            return View(vm);
                        }

                        // Descarte
                        if (vm.MOEP_IN_TIPO_1 == 5)
                        {
                            origem = "Descarte";
                        }

                        // Perdas
                        if (vm.MOEP_IN_TIPO_1 == 6)
                        {
                            origem = "Perdas";
                        }

                        // Envio para manutenção
                        if (vm.MOEP_IN_TIPO_1 == 7)
                        {
                            origem = "Envio para Manuteção - Saída";
                        }

                        // Ajuste manual
                        if (vm.MOEP_IN_TIPO_1 == 8)
                        {
                            origem = "Ajuste Manual - Saída";
                        }
                    }

                    // Acerta seleção de produto
                    if ((Int32)Session["TipoInclusao"] == 2)
                    {
                        // Recupera ultima quantidade
                        vm.MOEP_VL_QUANTIDADE_ANTERIOR = prod.PROD_VL_ESTOQUE_ATUAL;
                    }

                    // Processa entradas
                    Int32 pendente = 0;
                    Int32 gravaHist = 0;
                    Decimal? quant = 0;
                    Int32? tipoOp = vm.MOEP_IN_TIPO;
                    prod = prodApp.GetItemById(vm.PROD_CD_ID.Value);

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    if (vm.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        // Grava Pagamento
                        CONSULTA_PAGAMENTO pagto = new CONSULTA_PAGAMENTO();
                        if (vm.MOEP_IN_TIPO == 1 & vm.MOEP_IN_OPERACAO == 1)
                        {
                            pagto.COPA_IN_ATIVO = 1;
                            pagto.COPA_GU_GUID = Xid.NewXid().ToString();
                            pagto.USUA_CD_ID = usuario.USUA_CD_ID;
                            pagto.ASSI_CD_ID = idAss;
                            pagto.COPA_DT_VENCIMENTO = DateTime.Today.Date;
                            pagto.COPA_IN_CONFERIDO = 0;
                            pagto.COPA_NM_NOME = "Pagamento de compra feita em " + vm.MOEP_NM_FORNECEDOR;
                            pagto.COPA_VL_DESCONTO = 0;
                            pagto.COPA_NM_FAVORECIDO = vm.MOEP_NM_FORNECEDOR;
                            pagto.COPA_VL_MULTA = 0;
                            pagto.COPA_VL_VALOR = vm.MOEP_VL_VALOR_MOVIMENTO;
                            pagto.COPA_XM_NOTA_FISCAL = null;
                            pagto.COPA_DT_CADASTRO = DateTime.Today.Date;
                            pagto.TIPA_CD_ID = vm.MOEP_IN_AUTORIZADOR_T;
                            if (vm.MOEP_IN_AUTORIZADOR == 1)
                            {
                                pagto.COPA_IN_PAGO = 1;
                                pagto.COPA_DT_PAGAMENTO = DateTime.Today.Date;
                                pagto.COPA_VL_PAGO = vm.MOEP_VL_VALOR_MOVIMENTO;
                            }
                            else
                            {
                                pagto.COPA_IN_PAGO = 0;
                                pagto.COPA_DT_PAGAMENTO = null;
                                pagto.COPA_VL_PAGO = 0;
                            }
                            Int32 voltaCP = pagApp.ValidateCreate(pagto, usuario);

                            // Monta Log
                            DTO_Pagamento dto = MontarPagamentoDTOObj(pagto);
                            String json = JsonConvert.SerializeObject(dto, settings);
                            LOG log = new LOG
                            {
                                LOG_DT_DATA = DateTime.Now,
                                ASSI_CD_ID = usuario.ASSI_CD_ID,
                                USUA_CD_ID = usuario.USUA_CD_ID,
                                LOG_NM_OPERACAO = "Pagamento - Inclusão",
                                LOG_IN_ATIVO = 1,
                                LOG_TX_REGISTRO = json,
                                LOG_IN_SISTEMA = 6
                            };
                            Int32 volta1 = logApp.ValidateCreate(log);
                        }

                        // Monta e grava movimentação de entrada 
                        quant = vm.MOEP_QN_QUANTIDADE;
                        MOVIMENTO_ESTOQUE_PRODUTO item = Mapper.Map<MovimentoEstoqueProdutoViewModel, MOVIMENTO_ESTOQUE_PRODUTO>(vm);
                        item.MOEP_IN_ORIGEM = origem;
                        item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        item.EMPR_CD_ID = usuario.EMPR_CD_ID;
                        item.EMFI_CD_ID = vm.EMFI_CD_ID;
                        item.MOEP_IN_ULTIMO = 1;
                        item.MOEP_IN_ATIVO = 1;
                        item.MOEP_IN_PENDENTE = 0;
                        item.MOEP_IN_AUTORIZADOR = null;
                        item.MOEP_VL_QUANTIDADE_MOVIMENTO = vm.MOEP_QN_QUANTIDADE;
                        item.MOEP_VL_VALOR_MOVIMENTO = vm.MOEP_VL_VALOR_MOVIMENTO;
                        item.FOPA_CD_ID = null;
                        item.MOEP_GU_GUID = Xid.NewXid().ToString();
                        item.MOEP_IN_SISTEMA = 6;
                        item.MOEP_IN_TIPO = tipoOp;
                        if (vm.MOEP_IN_TIPO == 1 & vm.MOEP_IN_OPERACAO == 1)
                        {
                            item.COPA_CD_ID_1 = pagto.COPA_CD_ID;
                        }
                        else
                        {
                            item.COPA_CD_ID_1 = null;
                        }
                        Int32 voltaC = prodApp.ValidateCreateMovimento(item, usuario);
                        logRet = voltaC;
                        pendente = 0;

                        // Monta Log
                        DTO_Movimento dto1 = MontarMovimentoDTOObj(item);
                        String json1 = JsonConvert.SerializeObject(dto1, settings);
                        LOG log1 = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuario.ASSI_CD_ID,
                            USUA_CD_ID = usuario.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Estoque - Movimento - Inclusão",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json1,
                            LOG_IN_SISTEMA = 6
                        };
                        Int32 volta2 = logApp.ValidateCreate(log1);

                        // Acerta estoque total
                        Decimal? quantEstoque = 0;
                        PRODUTO novo = new PRODUTO();
                        novo.PROD_AQ_FOTO = prod.PROD_AQ_FOTO;
                        novo.PROD_CD_CODIGO = prod.PROD_CD_CODIGO;
                        novo.PROD_CD_ID = prod.PROD_CD_ID;
                        novo.PROD_DS_DESCRICAO = prod.PROD_DS_DESCRICAO;
                        novo.PROD_DS_INFORMACOES = prod.PROD_DS_INFORMACOES;
                        novo.PROD_DT_ALTERACAO = prod.PROD_DT_ALTERACAO;
                        novo.PROD_DT_CADASTRO = prod.PROD_DT_CADASTRO;
                        novo.PROD_IN_ATIVO = prod.PROD_IN_ATIVO;
                        novo.PROD_IN_COMPOSTO = prod.PROD_IN_COMPOSTO;
                        novo.PROD_IN_FRACIONADO = prod.PROD_IN_FRACIONADO;
                        novo.PROD_IN_PECA = prod.PROD_IN_PECA;
                        novo.PROD_IN_TIPO_PRODUTO = prod.PROD_IN_TIPO_PRODUTO;
                        novo.PROD_IN_USUARIO_ALTERACAO = prod.PROD_IN_USUARIO_ALTERACAO;
                        novo.PROD_NM_FABRICANTE = prod.PROD_NM_FABRICANTE;
                        novo.PROD_NM_MARCA = prod.PROD_NM_MARCA;
                        novo.PROD_NM_MODELO = prod.PROD_NM_MODELO;
                        novo.PROD_NM_NOME = prod.PROD_NM_NOME;
                        novo.PROD_NM_REFERENCIA_FABRICANTE = prod.PROD_NM_REFERENCIA_FABRICANTE;
                        novo.PROD_NR_BARCODE = prod.PROD_NR_BARCODE;
                        novo.PROD_NR_REFERENCIA = prod.PROD_NR_REFERENCIA;
                        novo.PROD_PC_DESCONTO = prod.PROD_PC_DESCONTO;
                        novo.PROD_TX_OBSERVACOES = prod.PROD_TX_OBSERVACOES;
                        novo.PROD_VL_CUSTO = prod.PROD_VL_CUSTO;
                        novo.PROD_VL_CUSTO_CONCORRENTE_MEDIO = prod.PROD_VL_CUSTO_CONCORRENTE_MEDIO;
                        novo.PROD_VL_CVM_PESO = prod.PROD_VL_CVM_PESO;
                        novo.PROD_VL_CVM_RECEITA = prod.PROD_VL_CVM_RECEITA;
                        novo.PROD_VL_CVM_UNITARIO = prod.PROD_VL_CVM_UNITARIO;
                        novo.PROD_VL_ESTOQUE_ATUAL = prod.PROD_VL_ESTOQUE_ATUAL + quant;
                        quantEstoque = prod.PROD_VL_ESTOQUE_ATUAL + quant;
                        novo.PROD_VL_ESTOQUE_CUSTO = prod.PROD_VL_ESTOQUE_CUSTO;
                        novo.PROD_VL_ESTOQUE_MAXIMO = prod.PROD_VL_ESTOQUE_MAXIMO;
                        novo.PROD_VL_ESTOQUE_MINIMO = prod.PROD_VL_ESTOQUE_MINIMO;
                        novo.PROD_VL_ESTOQUE_RESERVA = prod.PROD_VL_ESTOQUE_RESERVA;
                        novo.PROD_VL_ESTOQUE_VENDA = prod.PROD_VL_ESTOQUE_VENDA;
                        novo.PROD_VL_ESTOQUE_TOTAL = somaFinal + prod.PROD_VL_ESTOQUE_RESERVA;
                        novo.PROD_VL_FATOR_CORRECAO = prod.PROD_VL_FATOR_CORRECAO;
                        novo.PROD_VL_MARGEM_CONTRIBUICAO = prod.PROD_VL_MARGEM_CONTRIBUICAO;
                        novo.PROD_VL_MEDIA_VENDA_MENSAL = prod.PROD_VL_MEDIA_VENDA_MENSAL;
                        novo.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_ANTERIOR;
                        novo.PROD_VL_PRECO_MINIMO = prod.PROD_VL_PRECO_MINIMO;
                        novo.PROD_VL_PRECO_PROMOCAO = prod.PROD_VL_PRECO_PROMOCAO;
                        novo.PROD_VL_PRECO_VENDA = prod.PROD_VL_PRECO_VENDA;
                        novo.PROD_VL_ULTIMO_CUSTO = prod.PROD_VL_ULTIMO_CUSTO;
                        novo.PROD_VL_ESTOQUE_CUSTO = prod.PROD_VL_ESTOQUE_CUSTO;
                        novo.PROD_IN_SISTEMA = 6;
                        novo.CAPR_CD_ID = prod.CAPR_CD_ID;
                        novo.SCPR_CD_ID = prod.SCPR_CD_ID;
                        novo.ASSI_CD_ID = prod.ASSI_CD_ID;
                        novo.TIEM_CD_ID = prod.TIEM_CD_ID;
                        novo.UNID_CD_ID = prod.UNID_CD_ID;
                        novo.PROD_NM_FORNECEDOR = prod.PROD_NM_FORNECEDOR;
                        novo.PROD_IN_LOCACAO = prod.PROD_IN_LOCACAO;
                        novo.PROD_VL_LOCACAO = prod.PROD_VL_LOCACAO;
                        novo.PROD_VL_LOCACAO_MULTA = prod.PROD_VL_LOCACAO_MULTA;
                        novo.PROD_VL_LOCACAO_PROMOCAO = prod.PROD_VL_LOCACAO_PROMOCAO;
                        novo.PROD_VL_LOCACAO_TAXAS = prod.PROD_VL_LOCACAO_TAXAS;
                        Int32 voltaP = prodApp.ValidateEdit(novo, novo);
                        gravaHist = 1;

                        // Inclui historico de estoque
                        PRODUTO_ESTOQUE_HISTORICO hist = new PRODUTO_ESTOQUE_HISTORICO();
                        hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        hist.PROD_CD_ID = prod.PROD_CD_ID;
                        hist.PREH_IN_ATIVO = 1;
                        hist.PREH_DT_DATA = DateTime.Today.Date;
                        hist.PREH_DT_COMPLETA = DateTime.Now;
                        hist.PREH_QN_ESTOQUE = quant;
                        hist.PREH_IN_PENDENTE = 0;
                        hist.PREH_NM_TIPO = tipoMov == 1 ? "Entrada" : "Saída";
                        hist.PREH_DS_ORIGEM = origem;
                        hist.MOEP_CD_ID = item.MOEP_CD_ID;
                        hist.PREH_IN_TIPO_MOV = 1;
                        hist.PREH_QN_ESTOQUE_TOTAL = quantEstoque;
                        Int32 voltaH = prodApp.ValidateCreateEstoqueHistorico(hist, idAss);

                        // Monta Log
                        DTO_Produto dto2 = MontarProdutoDTOObj(novo);
                        String json2 = JsonConvert.SerializeObject(dto2, settings);
                        DTO_Produto dto2Antes = MontarProdutoDTOObj((PRODUTO)Session["ProdAntes"]);
                        String json2Antes = JsonConvert.SerializeObject(dto2Antes, settings);
                        LOG log2 = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuario.ASSI_CD_ID,
                            USUA_CD_ID = usuario.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Material/Produto - Alteração",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json2,
                            LOG_TX_REGISTRO_ANTES = json2Antes,
                            LOG_IN_SISTEMA = 6
                        };
                    }

                    // Processa saidas
                    if (vm.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        // Monta e grava movimentação de compra 
                        quant = vm.MOEP_QN_QUANTIDADE;
                        MOVIMENTO_ESTOQUE_PRODUTO item = Mapper.Map<MovimentoEstoqueProdutoViewModel, MOVIMENTO_ESTOQUE_PRODUTO>(vm);
                        item.MOEP_IN_ORIGEM = origem;
                        item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        item.EMPR_CD_ID = usuario.EMPR_CD_ID;
                        item.EMFI_CD_ID = vm.EMFI_CD_ID;
                        item.MOEP_IN_ULTIMO = 1;
                        item.MOEP_IN_ATIVO = 1;
                        item.MOEP_IN_PENDENTE = 0;
                        item.MOEP_IN_AUTORIZADOR = null;
                        item.MOEP_VL_QUANTIDADE_MOVIMENTO = vm.MOEP_QN_QUANTIDADE;
                        item.MOEP_VL_VALOR_MOVIMENTO = vm.MOEP_VL_VALOR_MOVIMENTO_1;
                        item.FOPA_CD_ID = null;
                        item.MOEP_GU_GUID = Xid.NewXid().ToString();
                        item.MOEP_IN_SISTEMA = 6;
                        item.MOEP_IN_TIPO = vm.MOEP_IN_TIPO_1;
                        Int32 voltaC = prodApp.ValidateCreateMovimento(item, usuario);
                        logRet = voltaC;
                        pendente = 0;

                        // Monta Log
                        DTO_Movimento dto1 = MontarMovimentoDTOObj(item);
                        String json1 = JsonConvert.SerializeObject(dto1, settings);
                        LOG log1 = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuario.ASSI_CD_ID,
                            USUA_CD_ID = usuario.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Estoque - Movimento - Inclusão",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json1,
                            LOG_IN_SISTEMA = 6
                        };
                        Int32 volta2 = logApp.ValidateCreate(log1);

                        // Acerta estoque total
                        Decimal? quantEstoque = 0;
                        PRODUTO novo = new PRODUTO();
                        novo.PROD_AQ_FOTO = prod.PROD_AQ_FOTO;
                        novo.PROD_CD_CODIGO = prod.PROD_CD_CODIGO;
                        novo.PROD_CD_ID = prod.PROD_CD_ID;
                        novo.PROD_DS_DESCRICAO = prod.PROD_DS_DESCRICAO;
                        novo.PROD_DS_INFORMACOES = prod.PROD_DS_INFORMACOES;
                        novo.PROD_DT_ALTERACAO = prod.PROD_DT_ALTERACAO;
                        novo.PROD_DT_CADASTRO = prod.PROD_DT_CADASTRO;
                        novo.PROD_IN_ATIVO = prod.PROD_IN_ATIVO;
                        novo.PROD_IN_COMPOSTO = prod.PROD_IN_COMPOSTO;
                        novo.PROD_IN_FRACIONADO = prod.PROD_IN_FRACIONADO;
                        novo.PROD_IN_PECA = prod.PROD_IN_PECA;
                        novo.PROD_IN_TIPO_PRODUTO = prod.PROD_IN_TIPO_PRODUTO;
                        novo.PROD_IN_USUARIO_ALTERACAO = prod.PROD_IN_USUARIO_ALTERACAO;
                        novo.PROD_NM_FABRICANTE = prod.PROD_NM_FABRICANTE;
                        novo.PROD_NM_MARCA = prod.PROD_NM_MARCA;
                        novo.PROD_NM_MODELO = prod.PROD_NM_MODELO;
                        novo.PROD_NM_NOME = prod.PROD_NM_NOME;
                        novo.PROD_NM_REFERENCIA_FABRICANTE = prod.PROD_NM_REFERENCIA_FABRICANTE;
                        novo.PROD_NR_BARCODE = prod.PROD_NR_BARCODE;
                        novo.PROD_NR_REFERENCIA = prod.PROD_NR_REFERENCIA;
                        novo.PROD_PC_DESCONTO = prod.PROD_PC_DESCONTO;
                        novo.PROD_TX_OBSERVACOES = prod.PROD_TX_OBSERVACOES;
                        novo.PROD_VL_CUSTO = prod.PROD_VL_CUSTO;
                        novo.PROD_VL_CUSTO_CONCORRENTE_MEDIO = prod.PROD_VL_CUSTO_CONCORRENTE_MEDIO;
                        novo.PROD_VL_CVM_PESO = prod.PROD_VL_CVM_PESO;
                        novo.PROD_VL_CVM_RECEITA = prod.PROD_VL_CVM_RECEITA;
                        novo.PROD_VL_CVM_UNITARIO = prod.PROD_VL_CVM_UNITARIO;
                        novo.PROD_VL_ESTOQUE_ATUAL = prod.PROD_VL_ESTOQUE_ATUAL - quant;
                        quantEstoque = prod.PROD_VL_ESTOQUE_ATUAL - quant;
                        novo.PROD_VL_ESTOQUE_CUSTO = prod.PROD_VL_ESTOQUE_CUSTO;
                        novo.PROD_VL_ESTOQUE_MAXIMO = prod.PROD_VL_ESTOQUE_MAXIMO;
                        novo.PROD_VL_ESTOQUE_MINIMO = prod.PROD_VL_ESTOQUE_MINIMO;
                        novo.PROD_VL_ESTOQUE_RESERVA = prod.PROD_VL_ESTOQUE_RESERVA;
                        novo.PROD_VL_ESTOQUE_VENDA = prod.PROD_VL_ESTOQUE_VENDA;
                        novo.PROD_VL_ESTOQUE_TOTAL = somaFinal + prod.PROD_VL_ESTOQUE_RESERVA;
                        novo.PROD_VL_FATOR_CORRECAO = prod.PROD_VL_FATOR_CORRECAO;
                        novo.PROD_VL_MARGEM_CONTRIBUICAO = prod.PROD_VL_MARGEM_CONTRIBUICAO;
                        novo.PROD_VL_MEDIA_VENDA_MENSAL = prod.PROD_VL_MEDIA_VENDA_MENSAL;
                        novo.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_ANTERIOR;
                        novo.PROD_VL_PRECO_MINIMO = prod.PROD_VL_PRECO_MINIMO;
                        novo.PROD_VL_PRECO_PROMOCAO = prod.PROD_VL_PRECO_PROMOCAO;
                        novo.PROD_VL_PRECO_VENDA = prod.PROD_VL_PRECO_VENDA;
                        novo.PROD_VL_ULTIMO_CUSTO = prod.PROD_VL_ULTIMO_CUSTO;
                        novo.PROD_VL_ESTOQUE_CUSTO = prod.PROD_VL_ESTOQUE_CUSTO;
                        novo.PROD_IN_SISTEMA = 6;
                        novo.CAPR_CD_ID = prod.CAPR_CD_ID;
                        novo.SCPR_CD_ID = prod.SCPR_CD_ID;
                        novo.ASSI_CD_ID = prod.ASSI_CD_ID;
                        novo.TIEM_CD_ID = prod.TIEM_CD_ID;
                        novo.UNID_CD_ID = prod.UNID_CD_ID;
                        novo.PROD_NM_FORNECEDOR = prod.PROD_NM_FORNECEDOR;
                        novo.PROD_IN_LOCACAO = prod.PROD_IN_LOCACAO;
                        novo.PROD_VL_LOCACAO = prod.PROD_VL_LOCACAO;
                        novo.PROD_VL_LOCACAO_MULTA = prod.PROD_VL_LOCACAO_MULTA;
                        novo.PROD_VL_LOCACAO_PROMOCAO = prod.PROD_VL_LOCACAO_PROMOCAO;
                        novo.PROD_VL_LOCACAO_TAXAS = prod.PROD_VL_LOCACAO_TAXAS;
                        Int32 voltaP = prodApp.ValidateEdit(novo, novo);
                        gravaHist = 1;

                        // Inclui historico de estoque
                        PRODUTO_ESTOQUE_HISTORICO hist = new PRODUTO_ESTOQUE_HISTORICO();
                        hist.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        hist.PROD_CD_ID = prod.PROD_CD_ID;
                        hist.PREH_IN_ATIVO = 1;
                        hist.PREH_DT_DATA = DateTime.Today.Date;
                        hist.PREH_DT_COMPLETA = DateTime.Now;
                        hist.PREH_QN_ESTOQUE = quant;
                        hist.PREH_IN_PENDENTE = 0;
                        hist.PREH_NM_TIPO = tipoMov == 1 ? "Entrada" : "Saída";
                        hist.PREH_DS_ORIGEM = origem;
                        hist.MOEP_CD_ID = item.MOEP_CD_ID;
                        hist.PREH_IN_TIPO_MOV = 2;
                        hist.PREH_QN_ESTOQUE_TOTAL = quantEstoque;
                        Int32 voltaH = prodApp.ValidateCreateEstoqueHistorico(hist, idAss);

                        // Monta Log
                        DTO_Produto dto2 = MontarProdutoDTOObj(novo);
                        String json2 = JsonConvert.SerializeObject(dto2, settings);
                        DTO_Produto dto2Antes = MontarProdutoDTOObj((PRODUTO)Session["ProdAntes"]);
                        String json2Antes = JsonConvert.SerializeObject(dto2Antes, settings);
                        LOG log2 = new LOG
                        {
                            LOG_DT_DATA = DateTime.Now,
                            ASSI_CD_ID = usuario.ASSI_CD_ID,
                            USUA_CD_ID = usuario.USUA_CD_ID,
                            LOG_NM_OPERACAO = "Material/Produto - Alteração",
                            LOG_IN_ATIVO = 1,
                            LOG_TX_REGISTRO = json2,
                            LOG_TX_REGISTRO_ANTES = json2Antes,
                            LOG_IN_SISTEMA = 6
                        };
                    }

                    // Grava histórico
                    if (gravaHist == 1)
                    {
                        // Grava Log de produto
                        PRODUTO_LOG logProd = new PRODUTO_LOG();
                        logProd.PROD_CD_ID = prod.PROD_CD_ID;
                        logProd.USUA_CD_ID = usuario.USUA_CD_ID;
                        logProd.LOG_CD_ID = logRet;
                        logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                        logProd.PRLG_DS_OPERACAO = origem;
                        Int32 volta5 = prodApp.ValidateCreateLog(logProd);
                    }

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A movimentação de estoque para o produto/material " + prod.PROD_NM_NOME.ToUpper() + " foi incluída com sucesso.";
                    Session["MensProduto"] = 61;

                    // Verifica retorno
                    mensagem = 0;
                    modelo = new ModeloViewModel();
                    Session["ListaEstoqueBase"] = null;
                    Session["ProdutoAlterada"] = 1;
                    Session["ListaMovimentoEstoquePendente"] = null;
                    return RedirectToAction("MontarTelaEstoque", new { id = "central" });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Estoque";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        public List<TIPO_PAGAMENTO> CarregaTipoPagamento()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TIPO_PAGAMENTO> conf = new List<TIPO_PAGAMENTO>();
                if (Session["TipoPagamentos"] == null)
                {
                    conf = tpApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["TipoPagamentoAlterada"] == 1)
                    {
                        conf = tpApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<TIPO_PAGAMENTO>)Session["TipoPagamentos"];
                    }
                }
                Session["TipoPagamentos"] = conf;
                Session["TipoPagamentoAlterada"] = 0;
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

        public DTO_Produto MontarProdutoDTOObj(PRODUTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Produto()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    CAPR_CD_ID = l.CAPR_CD_ID,
                    PROD_CD_CODIGO = l.PROD_CD_CODIGO,
                    PROD_CD_ID = l.PROD_CD_ID,
                    SCPR_CD_ID = l.SCPR_CD_ID,
                    TIEM_CD_ID = l.TIEM_CD_ID,
                    UNID_CD_ID = l.UNID_CD_ID,
                    PROD_AQ_FOTO = l.PROD_AQ_FOTO,
                    PROD_DS_DESCRICAO = l.PROD_DS_DESCRICAO,
                    PROD_DS_INFORMACOES = l.PROD_DS_INFORMACOES,
                    PROD_DT_ALTERACAO = l.PROD_DT_ALTERACAO,
                    PROD_DT_CADASTRO = l.PROD_DT_CADASTRO,
                    PROD_IN_ATIVO = l.PROD_IN_ATIVO,
                    PROD_IN_COMPOSTO = l.PROD_IN_COMPOSTO,
                    PROD_IN_FRACIONADO = l.PROD_IN_FRACIONADO,
                    PROD_IN_LOCACAO = l.PROD_IN_LOCACAO,
                    PROD_IN_PECA = l.PROD_IN_PECA,
                    PROD_IN_SISTEMA = l.PROD_IN_SISTEMA,
                    PROD_IN_TIPO_PRODUTO = l.PROD_IN_TIPO_PRODUTO,
                    PROD_IN_USUARIO_ALTERACAO = l.PROD_IN_USUARIO_ALTERACAO,
                    PROD_NM_FABRICANTE = l.PROD_NM_FABRICANTE,
                    PROD_NM_FORNECEDOR = l.PROD_NM_FORNECEDOR,
                    PROD_NM_MARCA = l.PROD_NM_MARCA,
                    PROD_NM_MODELO = l.PROD_NM_MODELO,
                    PROD_NM_NOME = l.PROD_NM_NOME,
                    PROD_NM_REFERENCIA_FABRICANTE = l.PROD_NM_REFERENCIA_FABRICANTE,
                    PROD_NR_BARCODE = l.PROD_NR_BARCODE,
                    PROD_NR_REFERENCIA = l.PROD_NM_REFERENCIA_FABRICANTE,
                    PROD_PC_DESCONTO = l.PROD_PC_DESCONTO,
                    PROD_TX_OBSERVACOES = l.PROD_TX_OBSERVACOES,
                    PROD_VL_CUSTO = l.PROD_VL_CUSTO,
                    PROD_VL_CUSTO_CONCORRENTE_MEDIO = l.PROD_VL_CUSTO_CONCORRENTE_MEDIO,
                    PROD_VL_CVM_PESO = l.PROD_VL_CVM_PESO,
                    PROD_VL_CVM_RECEITA = l.PROD_VL_CVM_RECEITA,
                    PROD_VL_CVM_UNITARIO = l.PROD_VL_CVM_UNITARIO,
                    PROD_VL_ESTOQUE_ATUAL = l.PROD_VL_ESTOQUE_ATUAL,
                    PROD_VL_ESTOQUE_CUSTO = l.PROD_VL_ESTOQUE_CUSTO,
                    PROD_VL_ESTOQUE_MAXIMO = l.PROD_VL_ESTOQUE_MAXIMO,
                    PROD_VL_ESTOQUE_MINIMO = l.PROD_VL_ESTOQUE_MINIMO,
                    PROD_VL_ESTOQUE_RESERVA = l.PROD_VL_ESTOQUE_RESERVA,
                    PROD_VL_ESTOQUE_TOTAL = l.PROD_VL_ESTOQUE_TOTAL,
                    PROD_VL_ESTOQUE_VENDA = l.PROD_VL_ESTOQUE_VENDA,
                    PROD_VL_FATOR_CORRECAO = l.PROD_VL_FATOR_CORRECAO,
                    PROD_VL_LOCACAO = l.PROD_VL_LOCACAO,
                    PROD_VL_LOCACAO_MULTA = l.PROD_VL_LOCACAO_MULTA,
                    PROD_VL_LOCACAO_PROMOCAO = l.PROD_VL_LOCACAO_PROMOCAO,
                    PROD_VL_LOCACAO_TAXAS = l.PROD_VL_LOCACAO_TAXAS,
                    PROD_VL_MARGEM_CONTRIBUICAO = l.PROD_VL_MARGEM_CONTRIBUICAO,
                    PROD_VL_MEDIA_VENDA_MENSAL = l.PROD_VL_MEDIA_VENDA_MENSAL,
                    PROD_VL_PRECO_ANTERIOR = l.PROD_VL_PRECO_ANTERIOR,
                    PROD_VL_PRECO_MINIMO = l.PROD_VL_PRECO_MINIMO,
                    PROD_VL_PRECO_PROMOCAO = l.PROD_VL_PRECO_PROMOCAO,
                    PROD_VL_PRECO_VENDA = l.PROD_VL_PRECO_VENDA,
                    PROD_VL_ULTIMO_CUSTO = l.PROD_VL_ULTIMO_CUSTO,
                };
                return mediDTO;
            }

        }


        public DTO_Pagamento MontarPagamentoDTOObj(CONSULTA_PAGAMENTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Pagamento()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    COPA_CD_ID = l.COPA_CD_ID,
                    TIPA_CD_ID = l.TIPA_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    COPA_DT_CADASTRO = l.COPA_DT_CADASTRO,
                    COPA_DT_PAGAMENTO = l.COPA_DT_PAGAMENTO,
                    COPA_DT_VENCIMENTO = l.COPA_DT_VENCIMENTO,
                    COPA_GU_GUID = l.COPA_GU_GUID,
                    COPA_IN_ATIVO = l.COPA_IN_ATIVO,
                    COPA_IN_CONFERIDO = l.COPA_IN_CONFERIDO,
                    COPA_IN_PAGO = l.COPA_IN_PAGO,
                    COPA_NM_FAVORECIDO = l.COPA_NM_FAVORECIDO,
                    COPA_NM_NOME = l.COPA_NM_NOME,
                    COPA_NR_ATRASO = l.COPA_NR_ATRASO,
                    COPA_VL_DESCONTO = l.COPA_VL_DESCONTO,
                    COPA_VL_MULTA = l.COPA_VL_MULTA,
                    COPA_VL_PAGO = l.COPA_VL_PAGO,
                    COPA_VL_VALOR = l.COPA_VL_VALOR,
                    COPA_XM_NOTA_FISCAL = l.COPA_XM_NOTA_FISCAL,
                };
                return mediDTO;
            }

        }

        public DTO_Movimento MontarMovimentoDTOObj(MOVIMENTO_ESTOQUE_PRODUTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Movimento()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    COBA_CD_ID = l.COBA_CD_ID,
                    COPA_CD_ID_1 = l.COPA_CD_ID_1,
                    CRPV_CD_ID = l.CRPV_CD_ID,
                    EMFI_CD_ID = l.EMFI_CD_ID,
                    EMPR_CD_ID = l.EMPR_CD_ID,
                    FILI_CD_ID = l.FILI_CD_ID,
                    FOPA_CD_ID = l.FOPA_CD_ID,
                    FORN_CD_ID = l.FORN_CD_ID,
                    MOEP_CD_ID = l.MOEP_CD_ID,
                    MOEP_EMFI_CD_ID = l.MOEP_EMFI_CD_ID,
                    MOEP_EMFI_CD_ID_ALVO = l.MOEP_EMFI_CD_ID_ALVO,
                    PROD_CD_ID = l.PROD_CD_ID,
                    MOEP_DS_JUSTIFICATIVA = l.MOEP_DS_JUSTIFICATIVA,
                    MOEP_DS_MANUTENCAO_OBSERVACAO = l.MOEP_DS_MANUTENCAO_OBSERVACAO,
                    MOEP_DT_AUTORIZACAO = l.MOEP_DT_AUTORIZACAO,
                    MOEP_DT_EXCLUSAO = l.MOEP_DT_EXCLUSAO,
                    MOEP_DT_LANCAMENTO = l.MOEP_DT_LANCAMENTO,
                    MOEP_DT_MOVIMENTO = l.MOEP_DT_MOVIMENTO,
                    MOEP_DT_PAGAMENTO = l.MOEP_DT_PAGAMENTO,
                    MOEP_GU_GUID = l.MOEP_GU_GUID,
                    MOEP_IN_ATIVO = l.MOEP_IN_ATIVO,
                    MOEP_IN_AUTORIZADOR = l.MOEP_IN_AUTORIZADOR,
                    MOEP_IN_CHAVE_ORIGEM = l.MOEP_IN_CHAVE_ORIGEM,
                    MOEP_IN_EXCLUIDOR = l.MOEP_IN_EXCLUIDOR,
                    MOEP_IN_OPERACAO = l.MOEP_IN_OPERACAO,
                    MOEP_IN_ORIGEM = l.MOEP_IN_ORIGEM,
                    MOEP_IN_PENDENTE = l.MOEP_IN_PENDENTE,
                    MOEP_IN_SISTEMA = l.MOEP_IN_SISTEMA,
                    MOEP_IN_TIPO = l.MOEP_IN_TIPO,
                    MOEP_IN_TIPO_LANCAMENTO = l.MOEP_IN_TIPO_LANCAMENTO,
                    MOEP_IN_TIPO_MOVIMENTO = l.MOEP_IN_TIPO_MOVIMENTO,
                    MOEP_IN_ULTIMO = l.MOEP_IN_ULTIMO,
                    MOEP_NM_FORNECEDOR = l.MOEP_NM_FORNECEDOR,
                    MOEP_QN_ALTERADA = l.MOEP_QN_ALTERADA,
                    MOEP_QN_ANTES = l.MOEP_QN_ANTES,
                    MOEP_QN_DEPOIS = l.MOEP_QN_DEPOIS,
                    MOEP_QN_QUANTIDADE = l.MOEP_QN_QUANTIDADE,
                    MOEP_VL_QUANTIDADE_ANTERIOR = l.MOEP_VL_QUANTIDADE_ANTERIOR,
                    MOEP_VL_QUANTIDADE_MOVIMENTO = l.MOEP_VL_QUANTIDADE_MOVIMENTO,
                    MOEP_VL_VALOR_MOVIMENTO = l.MOEP_VL_VALOR_MOVIMENTO,
                };
                return mediDTO;
            }

        }

        [HttpGet]
        public ActionResult VerMovimentacaoEstoque(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega movimentação
                MOVIMENTO_ESTOQUE_PRODUTO mov = prodApp.GetMovimentoById(id);
                Session["IdMovimentacao"] = id;

                // Carrega produto
                PRODUTO prod = prodApp.GetItemById(mov.PROD_CD_ID);
                ViewBag.Nome = prod.PROD_NM_NOME;

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                }

                // Abre view
                MovimentoEstoqueProdutoViewModel vm = Mapper.Map<MOVIMENTO_ESTOQUE_PRODUTO, MovimentoEstoqueProdutoViewModel>(mov);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult IncluirAnotacao()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                MOVIMENTO_ESTOQUE_PRODUTO item = prodApp.GetMovimentoById((Int32)Session["IdMovimentacao"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                Session["VoltaProduto"] = 777;
                MOVIMENTO_ANOTACAO coment = new MOVIMENTO_ANOTACAO();
                MovimentoAnotacaoViewModel vm = Mapper.Map<MOVIMENTO_ANOTACAO, MovimentoAnotacaoViewModel>(coment);
                vm.MOAN_DT_ANOTACAO = DateTime.Now;
                vm.MOAN_IN_ATIVO = 1;
                vm.ASSI_CD_ID = item.ASSI_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAnotacao(MovimentoAnotacaoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Executa a operação
                    MOVIMENTO_ANOTACAO item = Mapper.Map<MovimentoAnotacaoViewModel, MOVIMENTO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOVIMENTO_ESTOQUE_PRODUTO not = prodApp.GetMovimentoById((Int32)Session["IdMovimentacao"]);

                    item.USUARIO = null;
                    not.MOVIMENTO_ANOTACAO.Add(item);
                    Int32 volta = prodApp.ValidateEditMovimento(not);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Estoque - Movimentação - Anotação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Movimentação: " + not.MOEP_DT_MOVIMENTO.ToString() + " | Anotação: " + item.MOAN_DS_ANOTACA_,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Sucesso
                    return RedirectToAction("VerMovimentacaoEstoque", new { id = (Int32)Session["IdMovimentacao"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Estoque";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                Session["AbaProduto"] = 4;
                Session["VoltaProduto"] = 777;
                MOVIMENTO_ANOTACAO item = prodApp.GetAnotacaoMovimentoById(id);
                Session["Anotacao"] = item;
                MOVIMENTO_ESTOQUE_PRODUTO cli = prodApp.GetMovimentoById(item.MOEP_CD_ID.Value);
                MovimentoAnotacaoViewModel vm = Mapper.Map<MOVIMENTO_ANOTACAO, MovimentoAnotacaoViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacao(MovimentoAnotacaoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOVIMENTO_ESTOQUE_PRODUTO not = prodApp.GetMovimentoById((Int32)Session["IdMovimentacao"]);
                    MOVIMENTO_ANOTACAO item = Mapper.Map<MovimentoAnotacaoViewModel, MOVIMENTO_ANOTACAO>(vm);
                    Int32 volta = prodApp.ValidateEditAnotacaoMovimento(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Estoque - Movimentação - Anotação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Movimento: " + not.MOEP_DT_MOVIMENTO.ToString() + " | Anotação: " + item.MOAN_DS_ANOTACA_,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta4 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["ProdutoAlterada"] = 1;
                    Session["AbaProduto"] = 4;
                    return RedirectToAction("VerMovimentacaoEstoque", new { id = (Int32)Session["IdMovimentacao"] });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Produto";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return RedirectToAction("TrataExcecao", "BaseAdmin");
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirAnotacao(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                MOVIMENTO_ESTOQUE_PRODUTO not = prodApp.GetMovimentoById((Int32)Session["IdMovimentacao"]);

                Session["AbaProduto"] = 4;
                MOVIMENTO_ANOTACAO item = prodApp.GetAnotacaoMovimentoById(id);
                item.MOAN_IN_ATIVO = 0;
                Int32 volta = prodApp.ValidateEditAnotacaoMovimento(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Estoque - Movimentação - Anotação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Movimento: " + not.MOEP_DT_MOVIMENTO.ToString() + " | Anotação: " + item.MOAN_DS_ANOTACA_,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);
                return RedirectToAction("VerMovimentacaoEstoque", new { id = (Int32)Session["IdMovimentacao"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult ExcluirMovimentacaoEstoque(Int32 id)
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
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega movimentação
                MOVIMENTO_ESTOQUE_PRODUTO mov = prodApp.GetMovimentoById(id);
                Session["IdMovimentacao"] = id;

                // Carrega produto
                PRODUTO prod = prodApp.GetItemById(mov.PROD_CD_ID);
                ViewBag.Nome = prod.PROD_NM_NOME;

                // Processa movimento
                if (mov.MOEP_IN_PENDENTE == 1)
                {
                    // Processa movimento pendente
                    mov.MOEP_IN_PENDENTE = 2;
                    mov.MOEP_IN_ATIVO = 0;
                    mov.MOEP_DS_JUSTIFICATIVA = "Movimentação pendente excluída em " + DateTime.Now.ToString();
                    mov.MOEP_DT_EXCLUSAO = DateTime.Now;
                    mov.MOEP_IN_EXCLUIDOR = usuario.USUA_CD_ID;
                    Int32 voltaP = prodApp.ValidateEditMovimento(mov);
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "excMOEP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Movimento: " + mov.MOEP_GU_GUID + " | Movimento Excluído em: " + mov.MOEP_DT_MOVIMENTO.ToString() + " | Excluído por: " + mov.USUARIO2.USUA_NM_NOME,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);

                // Retorno
                Session["ListaMovimEstoque"] = null;
                Session["ListaMovimentoEstoque"] = null;
                Session["ListaEstoqueBase"] = null;
                Session["ListaMovimentoEstoquePendente"] = null;
                Session["ListaMovimentosPendentes"] = null;
                if ((Int32)Session["VoltaProduto"] == 997)
                {
                    return RedirectToAction("VerMovimentacoesPendentes");
                }
                if ((Int32)Session["IdProdutoAuto"] == 0)
                {
                    return RedirectToAction("VerMovimentacoes");
                }
                return RedirectToAction("VerMovimentacaoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        public ActionResult FiltrarMovimentoExcluido(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<SelectListItem> es = new List<SelectListItem>();
            es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.ES = new SelectList(es, "Value", "Text");
            List<SelectListItem> entradas = new List<SelectListItem>();
            entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
            entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
            entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
            entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
            ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
            List<SelectListItem> saidas = new List<SelectListItem>();
            saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
            saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
            saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
            saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
            ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
            ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            try
            {
                // Executa a operação
                CRMSysDBEntities Db = new CRMSysDBEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();

                // Monta condição
                if (item.MOEP_IN_TIPO_MOVIMENTO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == item.MOEP_IN_TIPO_MOVIMENTO);
                }
                if (item.MOEP_IN_TIPO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_TIPO);
                }
                if (item.MOEP_IN_OPERACAO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_OPERACAO);
                }
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY == null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 == null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1) & DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DS_JUSTIFICATIVA != null)
                {
                    query = query.Where(p => p.PRODUTO.PROD_NM_NOME.ToUpper().Contains(item.MOEP_DS_JUSTIFICATIVA.ToUpper()));
                }
                if (item.MOEP_GU_GUID != null)
                {
                    query = query.Where(p => p.MOEP_GU_GUID == item.MOEP_GU_GUID);
                }
                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.MOEP_IN_ATIVO == 0);
                    query = query.OrderByDescending(a => a.MOEP_DT_MOVIMENTO);
                    lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                // Sucesso
                Session["ListaMovimentoEstoqueExcluido"] = lista;
                Session["FiltroMovimentoEstoqueExcluido"] = item;
                return RedirectToAction("VerMovimentoExcluido");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public ActionResult FiltrarMovimentoExcluidoProduto(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }

            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            List<SelectListItem> es = new List<SelectListItem>();
            es.Add(new SelectListItem() { Text = "Entrada", Value = "1" });
            es.Add(new SelectListItem() { Text = "Saída", Value = "2" });
            ViewBag.ES = new SelectList(es, "Value", "Text");
            List<SelectListItem> entradas = new List<SelectListItem>();
            entradas.Add(new SelectListItem() { Text = "Compra", Value = "1" });
            entradas.Add(new SelectListItem() { Text = "Devolução de Venda", Value = "2" });
            entradas.Add(new SelectListItem() { Text = "Retorno de Manutenção", Value = "3" });
            entradas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "4" });
            ViewBag.Entradas = new SelectList(entradas, "Value", "Text");
            List<SelectListItem> saidas = new List<SelectListItem>();
            saidas.Add(new SelectListItem() { Text = "Descarte", Value = "5" });
            saidas.Add(new SelectListItem() { Text = "Perda", Value = "6" });
            saidas.Add(new SelectListItem() { Text = "Envio para Manutenção", Value = "7" });
            saidas.Add(new SelectListItem() { Text = "Acerto Manual", Value = "8" });
            ViewBag.Saidas = new SelectList(saidas, "Value", "Text");
            ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            try
            {
                // Executa a operação
                CRMSysDBEntities Db = new CRMSysDBEntities();
                Int32 idAss = (Int32)Session["IdAssinante"];
                IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();

                // Monta condição
                if (item.MOEP_IN_TIPO_MOVIMENTO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == item.MOEP_IN_TIPO_MOVIMENTO);
                }
                if (item.MOEP_IN_TIPO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_TIPO);
                }
                if (item.MOEP_IN_OPERACAO > 0)
                {
                    query = query.Where(p => p.MOEP_IN_TIPO == item.MOEP_IN_OPERACAO);
                }
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY == null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 == null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DT_DATA_DUMMY_1 != null & item.MOEP_DT_DATA_DUMMY != null)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY_1) & DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_GU_GUID != null)
                {
                    query = query.Where(p => p.MOEP_GU_GUID == item.MOEP_GU_GUID);
                }
                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.MOEP_IN_ATIVO == 0);
                    query = query.OrderByDescending(a => a.MOEP_DT_MOVIMENTO);
                    lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                // Sucesso
                Session["ListaMovimentoEstoqueExcluidoProduto"] = lista;
                Session["FiltroMovimentoEstoqueExcluidoProduto"] = item;
                return RedirectToAction("VerMovimentoExcluidoProduto", new { id = (Int32)Session["IdProdutoAuto"] });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        // *************************
        // Relatórios
        // *************************
                
        public ActionResult GerarRelatorioLista()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<PRODUTO> lista = new List<PRODUTO>();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Recupera perfil de acesso
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Define título
                nomeRel = "EstoqueLista" + "_" + data + ".pdf";

                // Monta lista
                lista = (List<PRODUTO>)Session["ListaEstoqueBase"];

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

                    cell1 = new PdfPCell(new Paragraph("Estoque", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Estoque", meuFont2))
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

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 50f, 50f, 180f, 80f, 70f, 70f, 70f, 70f, 70f, 70f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                // Cabeçalho
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Código", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome do Material/Produto", meuFont))
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

                cell = new PdfPCell(new Paragraph("Estoque Total", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque Máximo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque Mínimo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Estoque Reservado", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média Mensal", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Esgotamento (Dias)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(" ", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Linhas
                foreach (PRODUTO item in lista)
                {
                    if (item.PROD_IN_TIPO_PRODUTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Material", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.PROD_IN_TIPO_PRODUTO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Produto", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.PROD_VL_ESTOQUE_ATUAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_ATUAL.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (item.PROD_VL_ESTOQUE_MAXIMO != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_MAXIMO.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (item.PROD_VL_ESTOQUE_MINIMO != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_MINIMO.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (item.PROD_VL_ESTOQUE_RESERVA != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_RESERVA.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (item.PROD_VL_MEDIA_VENDA_MENSAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_MEDIA_VENDA_MENSAL.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (item.PROD_VL_MEDIA_VENDA_MENSAL != null & item.PROD_VL_MEDIA_VENDA_MENSAL > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(((item.PROD_VL_ESTOQUE_ATUAL.Value / item.PROD_VL_MEDIA_VENDA_MENSAL.Value)) * 30), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    if (System.IO.File.Exists(Server.MapPath(item.PROD_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        Image image = Image.GetInstance(Server.MapPath(item.PROD_AQ_FOTO));
                        image.ScaleAbsolute(20, 20);
                        cell.AddElement(image);
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("-", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(cell);
                    }
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

                return RedirectToAction("MontarTelaEstoque");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemMovimentacaoProduto()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Recupera perfil de acesso
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Define título
                nomeRel = "EstoqueMovimentacao" + "_" + data + ".pdf";
                titulo = "Estoque - Movimentações de " + (String)Session["NomeProduto"];

                // Monta lista
                lista = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoEstoque"];
                lista = lista.OrderBy(p => p.MOEP_DT_MOVIMENTO).ThenBy(p => p.MOEP_IN_TIPO_MOVIMENTO).ThenBy(p => p.MOEP_IN_TIPO).ToList();

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

                    cell1 = new PdfPCell(new Paragraph("Estoque - Movimentações de " + (String)Session["NomeProduto"], meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Estoque - Movimentações de " + (String)Session["NomeProduto"], meuFont2))
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

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 80f, 80f, 80f, 180f, 80f, 60f, 100f, 180f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                // Cabeçalho
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Classe", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Material/Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Código", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Linhas
                foreach (MOVIMENTO_ESTOQUE_PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.MOEP_DT_MOVIMENTO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Entrada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Saída", meuFont))
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

                    if (item.MOEP_IN_TIPO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Compra", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Devolução de Venda", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Retorno de Manutenção", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Acerto Manual", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Descarte", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 6)
                    {
                        cell = new PdfPCell(new Paragraph("Perda", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 7)
                    {
                        cell = new PdfPCell(new Paragraph("Envio para Manutenção", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 8)    
                    {
                        cell = new PdfPCell(new Paragraph("Acerto Manual", meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.MOEP_QN_QUANTIDADE), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.MOEP_GU_GUID, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.USUARIO.USUA_NM_NOME, meuFont))
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

                return RedirectToAction("VerMovimentacaoProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemEvolucaoEstoque()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<PRODUTO_ESTOQUE_HISTORICO> lista = new List<PRODUTO_ESTOQUE_HISTORICO>();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Recupera perfil de acesso
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Define título
                nomeRel = "EstoqueHistorico" + "_" + data + ".pdf";
                titulo = "Estoque - Histórico de Movimentação";

                // Monta lista
                lista = (List<PRODUTO_ESTOQUE_HISTORICO>)Session["ListaHistoricoEstoque"];
                lista = lista.OrderBy(p => p.PREH_DT_COMPLETA).ToList();

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

                    cell1 = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph(titulo, meuFont2))
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

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 140f, 80f, 80f, 80f, 80f, 100f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                cell.Colspan = 6;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Cabeçalho
                cell = new PdfPCell(new Paragraph("Material/Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Data da Movimentação", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Classe", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Linhas
                foreach (PRODUTO_ESTOQUE_HISTORICO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PREH_DT_COMPLETA.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.PREH_QN_ESTOQUE.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.IntegerFormatter(item.PREH_QN_ESTOQUE_TOTAL.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PREH_NM_TIPO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PREH_DS_ORIGEM, meuFont))
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

                return RedirectToAction("VerHistoricoEstoque");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarListagemMovimentacaoExcluido()
        {
            try
            {
                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Recupera perfil de acesso
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Define título
                nomeRel = "EstoqueMovimentacaoExcluida" + "_" + data + ".pdf";
                titulo = "Estoque - Movimentações Excluídas";

                // Monta lista
                lista = (List<MOVIMENTO_ESTOQUE_PRODUTO>)Session["ListaMovimentoEstoqueExcluidoProduto"];
                MOVIMENTO_ESTOQUE_PRODUTO filtro = (MOVIMENTO_ESTOQUE_PRODUTO)Session["FiltroMovimentoEstoqueExcluidoProduto"];
                lista = lista.OrderBy(p => p.EMFI_CD_ID).ThenByDescending(p => p.MOEP_DT_MOVIMENTO).ThenBy(p => p.MOEP_IN_TIPO_MOVIMENTO).ThenBy(p => p.MOEP_IN_TIPO).ToList();
                 
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
                table.HorizontalAlignment = 1; 
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

                cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
                table = new PdfPTable(new float[] { 80f, 50f, 50f, 80f, 120f, 50f, 50f, 80f, 80f, 50f, 80f, 100f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Movimentações selecionadas pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Cabeçalho
                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Classe", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Material/Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Código", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Exclusão", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Identificador", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                // Linhas
                foreach (MOVIMENTO_ESTOQUE_PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.MOEP_DT_MOVIMENTO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.MOEP_IN_TIPO_MOVIMENTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Entrada", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO_MOVIMENTO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Saída", meuFont))
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

                    if (item.MOEP_IN_TIPO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Compra", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 2)
                    {
                        cell = new PdfPCell(new Paragraph("Devolução de Venda", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 3)
                    {
                        cell = new PdfPCell(new Paragraph("Retorno de Manutenção", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 4)
                    {
                        cell = new PdfPCell(new Paragraph("Acerto Manual", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 5)
                    {
                        cell = new PdfPCell(new Paragraph("Descarte", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 6)
                    {
                        cell = new PdfPCell(new Paragraph("Perda", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 7)
                    {
                        cell = new PdfPCell(new Paragraph("Envio para Manutenção", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else if (item.MOEP_IN_TIPO == 8)
                    {
                        cell = new PdfPCell(new Paragraph("Acerto Manual", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Transferência", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PRODUTO.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.MOEP_VL_QUANTIDADE_MOVIMENTO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    table.AddCell(cell);

                    if (item.MOEP_DT_EXCLUSAO != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.MOEP_DT_EXCLUSAO.Value.ToShortDateString(), meuFont))
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

                    if (item.USUARIO2 != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.USUARIO2.USUA_NM_NOME, meuFont))
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

                    cell = new PdfPCell(new Paragraph(item.MOEP_GU_GUID, meuFont))
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

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.MOEP_IN_TIPO_MOVIMENTO > 0)
                    {
                        parametros += "Tipo: " + RetornaTipo(filtro.MOEP_IN_TIPO_MOVIMENTO);
                        ja = 1;
                    }
                    if (filtro.MOEP_IN_TIPO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Classe: " + RetornaClasse(filtro.MOEP_IN_TIPO.Value);
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Classe: " + RetornaClasse(filtro.MOEP_IN_TIPO.Value);
                        }
                    }
                    if (filtro.MOEP_IN_OPERACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Classe: " + RetornaClasse(filtro.MOEP_IN_OPERACAO.Value);
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Classe: " + RetornaClasse(filtro.MOEP_IN_OPERACAO.Value);
                        }
                    }
                    if (filtro.MOEP_IN_OPERACAO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Classe: " + RetornaClasse(filtro.MOEP_IN_OPERACAO.Value);
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Classe: " + RetornaClasse(filtro.MOEP_IN_OPERACAO.Value);
                        }
                    }
                    if (filtro.USUA_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Responsável: " + filtro.USUARIO.USUA_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Responsável: " + filtro.USUARIO.USUA_NM_NOME;
                        }
                    }
                    if (filtro.MOEP_GU_GUID != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Identificador: " + filtro.MOEP_GU_GUID;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Identificador: " + filtro.MOEP_GU_GUID;
                        }
                    }
                    if (filtro.MOEP_DT_DATA_DUMMY_1 != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Data Início: " + filtro.MOEP_DT_DATA_DUMMY_1.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Data Início: " + filtro.MOEP_DT_DATA_DUMMY_1.ToString();
                        }
                    }
                    if (filtro.MOEP_DT_DATA_DUMMY != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Data Final: " + filtro.MOEP_DT_DATA_DUMMY.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Data Final: " + filtro.MOEP_DT_DATA_DUMMY.ToString();
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

                return RedirectToAction("VerMovimentoExcluidoProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }


                // Prepara geração
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                Int32 idAss = (Int32)Session["IdAssinante"];
                MOVIMENTO_ESTOQUE_PRODUTO mov = prodApp.GetMovimentoById((Int32)Session["IdMovimentacao"]);
                MovimentoEstoqueProdutoViewModel aten = Mapper.Map<MOVIMENTO_ESTOQUE_PRODUTO, MovimentoEstoqueProdutoViewModel>(mov);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "Movimento_" + aten.MOEP_GU_GUID + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

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

                    cell1 = new PdfPCell(new Paragraph("Movimentação de Estoque - Detalhes", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Movimentação de Estoque - Detalhes", meuFont2))
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
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 60, 40);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                Paragraph line1 = new Paragraph("  ");
                pdfDoc.Add(line1);

                // Dados Gerais
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                // Informações da movimentação
                cell = new PdfPCell(new Paragraph("Informações da Movimentação", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Identificador: " + aten.MOEP_GU_GUID, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Tipo: " + aten.TipoMovimento, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Classe: " + aten.TipoEntradaSaida, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável: " + aten.USUARIO.USUA_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Data da Movimentação: " + aten.MOEP_DT_MOVIMENTO.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Dados de exclusão
                if (aten.MOEP_IN_ATIVO == 0)
                {
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Informações da Exclusão da Movimentação", meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Data da Exclusão: " + aten.MOEP_DT_EXCLUSAO.Value.ToShortDateString(), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Excluidor: " + aten.USUARIO2.USUA_NM_NOME, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);
                }

                // Compra Manual
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    if (aten.MOEP_IN_TIPO == 1)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Compra", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 3;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 3;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 1;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Valor da Compra (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.MOEP_VL_VALOR_MOVIMENTO.Value), meuFont));
                        cell.Border = 0;
                        cell.Colspan = 1;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Data do Pagamento: " + aten.MOEP_DT_PAGAMENTO.Value.ToShortDateString(), meuFont));
                        cell.Border = 0;
                        cell.Colspan = 1;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 3;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Devolução de venda
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    if (aten.MOEP_IN_TIPO == 2)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f});
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Devolução de Venda", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("    ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Retorno de manutenção
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    if (aten.MOEP_IN_TIPO == 3)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Retorno de Manutenção", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("      ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fornecedor: " + aten.FORNECEDOR.FORN_NM_NOME, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 2;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Cidade: " + aten.FORNECEDOR.FORN_NM_CIDADE, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 2;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("UF: " + aten.FORNECEDOR.UF.UF_NM_NOME, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 1;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Acerto Manual - Entrada
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    if (aten.MOEP_IN_TIPO == 4)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f});
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Acerto Manual - Entrada", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Status: " + aten.Pendente, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Descarte
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    if (aten.MOEP_IN_TIPO == 5)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Descarte", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Status: " + aten.Pendente, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Perda
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    if (aten.MOEP_IN_TIPO == 6)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Perda", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Status: " + aten.Pendente, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Envio para manutenção
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    if (aten.MOEP_IN_TIPO == 7)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Envio para Manutenção", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("      ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Fornecedor: " + aten.FORNECEDOR.FORN_NM_NOME, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 2;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Cidade: " + aten.FORNECEDOR.FORN_NM_CIDADE, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 2;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("UF: " + aten.FORNECEDOR.UF.UF_NM_NOME, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 1;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 5;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Acerto Manual - Saida
                if (aten.MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    if (aten.MOEP_IN_TIPO == 8)
                    {
                        table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 1f;
                        table.SpacingAfter = 1f;

                        cell = new PdfPCell(new Paragraph("Acerto Manual - Saída", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("       ", meuFontBold));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Quantidade: " + aten.MOEP_VL_QUANTIDADE_MOVIMENTO.Value, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Status: " + aten.Pendente, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Justificativa: " + aten.MOEP_DS_JUSTIFICATIVA, meuFont));
                        cell.Border = 0;
                        cell.Colspan = 4;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(cell);
                        pdfDoc.Add(table);

                        // Linha Horizontal
                        line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line1);
                    }
                }

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();


                return RedirectToAction("VoltarMovimentacaoEstoque");
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

        public List<PRODUTO> CarregarProduto()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PRODUTO> conf = new List<PRODUTO>();
                if (Session["Produtos"] == null)
                {
                    conf = prodApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["ProdutoAlterada"] == 1)
                    {
                        conf = prodApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<PRODUTO>)Session["Produtos"];
                    }
                }
                Session["ProdutoAlterada"] = 0;
                Session["Produtos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<CATEGORIA_PRODUTO> CarregarCatProduto()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<CATEGORIA_PRODUTO> conf = new List<CATEGORIA_PRODUTO>();
                if (Session["CatProdutos"] == null)
                {
                    conf = prodApp.GetAllTipos(idAss);
                }
                else
                {
                    if ((Int32)Session["CatProdutoAlterada"] == 1)
                    {
                        conf = prodApp.GetAllTipos(idAss);
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<SUBCATEGORIA_PRODUTO> CarregarSubCatProduto()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<SUBCATEGORIA_PRODUTO> conf = new List<SUBCATEGORIA_PRODUTO>();
                if (Session["SubCatProdutos"] == null)
                {
                    conf = prodApp.GetAllSubs(idAss);
                }
                else
                {
                    if ((Int32)Session["SubCatProdutoAlterada"] == 1)
                    {
                        conf = prodApp.GetAllSubs(idAss);
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<USUARIO> CarregarUsuario()
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
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public String RetornaTipo(Int32 tipo)
        {
            if (tipo == 1)
            {
                return "Entrada";
            }
            if (tipo == 2)
            {
                return "Saída";
            }
            return " ";
        }

        public String RetornaClasse(Int32 tipo)
        {
            if (tipo == 1)
            {
                return "Compra";
            }
            if (tipo == 2)
            {
                return "Devolução de Venda";
            }
            if (tipo == 3)
            {
                return "Retorno de Manutenção";
            }
            if (tipo == 4)
            {
                return "Ajuste Manual";
            }
            if (tipo == 5)
            {
                return "Descarte";
            }
            if (tipo == 6)
            {
                return "Perda";
            }
            if (tipo == 7)
            {
                return "Envio para Manutenção";
            }
            if (tipo == 8)
            {
                return "Ajuste Manual";
            }
            return " ";
        }

        public List<UNIDADE> CarregarUnidade()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<UNIDADE> conf = new List<UNIDADE>();
                if (Session["Unidades"] == null)
                {
                    conf = prodApp.GetAllUnidades(idAss);
                }
                else
                {
                    if ((Int32)Session["UnidadeAlterada"] == 1)
                    {
                        conf = prodApp.GetAllUnidades(idAss);
                    }
                    else
                    {
                        conf = (List<UNIDADE>)Session["Unidades"];
                    }
                }
                conf = conf.Where(p => p.UNID_IN_TIPO_UNIDADE == 1).ToList();
                Session["UnidadeAlterada"] = 0;
                Session["Unidades"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<LOCACAO> CarregarLocacao()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<LOCACAO> conf = new List<LOCACAO>();
                if (Session["Locacoes"] == null)
                {
                    conf = locApp.GetAllItens(idAss);
                }
                else
                {
                    if ((Int32)Session["LocacaoAlterada"] == 1)
                    {
                        conf = locApp.GetAllItens(idAss);
                    }
                    else
                    {
                        conf = (List<LOCACAO>)Session["Locacoes"];
                    }
                }
                Session["LocacaoAlterada"] = 0;
                Session["Locacoes"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Estoque";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Estoque", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public List<String> CarregarTipoProduto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> conf = new List<String>();
            conf.Add("Produto");
            conf.Add("Insumo");
            return conf;
        }

        public List<String> CarregarComposto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> conf = new List<String>();
            conf.Add("Não");
            conf.Add("Sim");
            return conf;
        }

        public String TipoDecoder(Int32 id)
        {
            if (id == 1)
            {
                return "Entrada";
            }
            if (id == 2)
            {
                return "Saída";
            }
            return "Entrada";
        }

        public String ClasseDecoder(Int32 id)
        {
            if (id == 4)
            {
                return "Ajuste Manual - Entrada";
            }
            if (id == 8)
            {
                return "Ajuste Manual - Saída";
            }
            return "Ajuste Manual - Entrada";
        }


    }
}