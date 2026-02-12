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
    public class ProdutoController : Controller
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

        private String msg;
        private Exception exception;
        PRODUTO objetoProd = new PRODUTO();
        PRODUTO objetoProdAntes = new PRODUTO();
        List<PRODUTO> listaMasterProd = new List<PRODUTO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        PRODUTO_FALHA objetoFalha = new PRODUTO_FALHA();
        PRODUTO_FALHA objetoAntesFalha = new PRODUTO_FALHA();
        List<PRODUTO_FALHA> listaMasterFalha = new List<PRODUTO_FALHA>();
        String extensao;


        public ProdutoController(
            IProdutoAppService prodApps
            , ILogAppService logApps
            , IUnidadeAppService unApps
            , ICategoriaProdutoAppService cpApps
            , IEmpresaAppService filApps
            , ISubcategoriaProdutoAppService scpApps
            , IConfiguracaoAppService confApps
            , IUsuarioAppService usuApps, IEmpresaAppService empApps)
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

        public void FlagContinua()
        {
            Session["VoltaCliente"] = 3;
        }

        public ActionResult IncluirCategoriaProduto()
        {
            Session["VoltaCatProduto"] = 2;
            Session["CategoriaToProduto"] = true;
            return RedirectToAction("IncluirCatProduto", "TabelasAuxiliares");
        }

        public ActionResult IncluirSubCategoriaProduto()
        {
            Session["VoltaSubCatProduto"] = 2;
            Session["SubCategoriaToProduto"] = true;
            return RedirectToAction("IncluirSubCatProduto", "TabelasAuxiliares");
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

        [HttpPost]
        public JsonResult BuscaNomeProduto(String nome)
        {
            List<Hashtable> listResult = new List<Hashtable>();
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<PRODUTO> lista = CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 2).ToList();

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

        public ActionResult MontarTelaDashboardProduto()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produto";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega valores
                List<PRODUTO> produto = CarregarProduto();
                Int32 produtos = produto.Count;
                Session["ProdutoConta"] = produtos;
                ViewBag.Produtos = produtos;

                // Recupera Produtos por Categoria
                if (Session["ListaProdutoCats"] == null)
                {
                    List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
                    List<CATEGORIA_PRODUTO> catc = CarregarCatProduto();
                    foreach (CATEGORIA_PRODUTO item in catc)
                    {
                        Int32 num = produto.Where(p => p.CAPR_CD_ID == item.CAPR_CD_ID).ToList().Count;
                        if (num > 0)
                        {
                            ModeloViewModel mod1 = new ModeloViewModel();
                            mod1.Nome = item.CAPR_NM_NOME;
                            mod1.Valor = num;
                            lista7.Add(mod1);
                        }
                    }
                    ViewBag.ListaProdutoCats = lista7;
                    Session["ListaProdutoCats"] = lista7;
                }
                else
                {
                    ViewBag.ListaProdutoCats = (List<ModeloViewModel>)Session["ListaProdutoCats"];
                }

                // Recupera Produtos por Espécie
                if (Session["ListaProdutoEspecie"] == null)
                {
                    List<ModeloViewModel> lista9 = new List<ModeloViewModel>();
                    for (int i = 1; i < 3; i++)
                    {
                        Int32 num = produto.Where(p => p.PROD_IN_TIPO_PRODUTO == i).ToList().Count;
                        if (num > 0)
                        {
                            ModeloViewModel mod3 = new ModeloViewModel();
                            mod3.Nome = i == 1 ? "Material" : "Produto";
                            mod3.Valor = num;
                            lista9.Add(mod3);
                        }
                    }
                    ViewBag.ListaProdutoEspecie = lista9;
                    Session["ListaProdutoEspecie"] = lista9;
                }
                else
                {
                    ViewBag.ListaProdutoEspecie = (List<ModeloViewModel>)Session["ListaProdutoEspecie"];
                }

                // Recupera Produtos acima do máximo
                if (Session["ListaProdutoAcima"] == null)
                {
                    List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
                    List<PRODUTO> acima = produto.Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    Int32 totAcima = acima.Count;
                    foreach (PRODUTO item in acima)
                    {
                        ModeloViewModel mod4 = new ModeloViewModel();
                        mod4.Nome = item.PROD_NM_NOME;
                        mod4.ValorDec1 = item.PROD_VL_ESTOQUE_ATUAL.Value;
                        mod4.ValorDec2 = item.PROD_VL_ESTOQUE_MAXIMO.Value;
                        if (item.PROD_VL_ESTOQUE_MAXIMO > 0)
                        {
                            mod4.ValorDec3 = ((item.PROD_VL_ESTOQUE_ATUAL.Value * 100) / item.PROD_VL_ESTOQUE_MAXIMO.Value) - 100;
                        }
                        else
                        {
                            mod4.ValorDec3 = 0;
                        }
                        lista1.Add(mod4);
                    }
                    ViewBag.ListaProdutoAcima = lista1;
                    Session["ListaProdutoAcima"] = lista1;
                    Session["TotAcima"] = totAcima;
                    ViewBag.ProdutosAcima = totAcima;
                }
                else
                {
                    ViewBag.ListaProdutoAcima = (List<ModeloViewModel>)Session["ListaProdutoAcima"];
                    ViewBag.ProdutosAcima = (Int32)Session["TotAcima"];
                }

                // Recupera Produtos abaixo do minimo
                if (Session["ListaProdutoAbaixo"] == null)
                {
                    List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
                    List<PRODUTO> abaixo = produto.Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    Int32 totAbaixo = abaixo.Count;
                    foreach (PRODUTO item in abaixo)
                    {
                        ModeloViewModel mod6 = new ModeloViewModel();
                        mod6.Nome = item.PROD_NM_NOME;
                        mod6.ValorDec1 = item.PROD_VL_ESTOQUE_ATUAL.Value;
                        mod6.ValorDec2 = item.PROD_VL_ESTOQUE_MINIMO.Value;
                        lista3.Add(mod6);
                    }
                    ViewBag.ListaProdutoAbaixo = lista3;
                    Session["ListaProdutoAbaixo"] = lista3;
                    Session["TotAbaixo"] = totAbaixo;
                    ViewBag.ProdutosAbaixo = totAbaixo;
                }
                else
                {
                    ViewBag.ListaProdutoAbaixo = (List<ModeloViewModel>)Session["ListaProdutoAbaixo"];
                    ViewBag.ProdutosAbaixo = (Int32)Session["TotAbaixo"];
                }

                // Maximo x Minimo
                List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
                ModeloViewModel mod7 = new ModeloViewModel();
                mod7.Nome = "Estoque Normal";
                mod7.Valor = produtos;
                lista4.Add(mod7);
                mod7 = new ModeloViewModel();
                mod7.Nome = "Estoque Acima do Máximo";
                mod7.Valor = (Int32)Session["TotAcima"];
                lista4.Add(mod7);
                mod7 = new ModeloViewModel();
                mod7.Nome = "Estoque Abaixo do Mínimo";
                mod7.Valor = (Int32)Session["TotAbaixo"];
                lista4.Add(mod7);
                ViewBag.ListaProdutoAcimaAbaixoGraf = lista4;
                Session["ListaProdutoAcimaAbaixoGraf"] = lista4;

                // Recupera Produtos zerados
                if (Session["ListaProdutoZerado"] == null)
                {
                    List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
                    List<PRODUTO> zerado = produto.Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    Int32 totZerado = zerado.Count;
                    foreach (PRODUTO item in zerado)
                    {
                        ModeloViewModel mod8 = new ModeloViewModel();
                        mod8.Nome = item.PROD_NM_NOME;
                        mod8.ValorDec1 = item.PROD_VL_ESTOQUE_ATUAL.Value;
                        mod8.ValorDec2 = item.PROD_VL_ESTOQUE_MINIMO.Value;
                        lista5.Add(mod8);
                    }
                    ViewBag.ListaProdutoZerado = lista5;
                    Session["ListaProdutoZerado"] = lista5;
                    Session["TotZerado"] = totZerado;
                    ViewBag.ProdutosZerado = totZerado;
                }
                else
                {
                    ViewBag.ListaProdutoZerado = (List<ModeloViewModel>)Session["ListaProdutoZerado"];
                    ViewBag.ProdutosZerado = (Int32)Session["TotZerado"];
                }

                // Recupera Produtos esgotando em 30 dias
                if (Session["ListaProdutoEsgota"] == null)
                {
                    List<ModeloViewModel> lista6 = new List<ModeloViewModel>();
                    List<PRODUTO> lp = produto.Where(p => p.PROD_VL_MEDIA_VENDA_MENSAL > 0).ToList();
                    List<PRODUTO> esgota = lp.Where(p => ((p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1) & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    Int32 totEsgota = esgota.Count;
                    foreach (PRODUTO item in esgota)
                    {
                        ModeloViewModel mod9 = new ModeloViewModel();
                        mod9.Nome = item.PROD_NM_NOME;
                        mod9.ValorDec1 = item.PROD_VL_ESTOQUE_ATUAL.Value;
                        mod9.ValorDec2 = item.PROD_VL_MEDIA_VENDA_MENSAL.Value;
                        lista6.Add(mod9);
                    }
                    ViewBag.ListaProdutoEsgota = lista6;
                    Session["ListaProdutoEsgota"] = lista6;
                    Session["TotEsgota"] = totEsgota;
                    ViewBag.ProdutosEsgota = totEsgota;
                }
                else
                {
                    ViewBag.ListaProdutoEsgota = (List<ModeloViewModel>)Session["ListaProdutoEsgota"];
                    ViewBag.ProdutosEsgota = (Int32)Session["TotEsgota"];
                }
                Session["FlagProduto"] = 0;
                Session["VoltaBaseProduto"] = 2;
                Session["VoltaProduto"] = 0;
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produtos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produtos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaProduto()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
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

                // Carrega Produtos
                if (Session["ListaProduto"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.OrderBy(p => p.PROD_IN_TIPO_PRODUTO).ThenBy(p => p.PROD_NM_NOME).ToList();
                    Session["ListaProduto"] = listaMasterProd;
                }
                ViewBag.Listas = (List<PRODUTO>)Session["ListaProduto"];

                // Carrega Listas
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProduto"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> sits = new List<SelectListItem>();
                sits.Add(new SelectListItem() { Text = "Estoque acima do máximo", Value = "1" });
                sits.Add(new SelectListItem() { Text = "Estoque abaixo do mínimo", Value = "2" });
                sits.Add(new SelectListItem() { Text = "Estoque zerado", Value = "3" });
                sits.Add(new SelectListItem() { Text = "Estoque esgotando em 30 dias", Value = "4" });
                ViewBag.Sits = new SelectList(sits, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_1.pdf";

                List<SelectListItem> relat = new List<SelectListItem>();
                relat.Add(new SelectListItem() { Text = "Lista de Produtos*", Value = "1" });
                relat.Add(new SelectListItem() { Text = "Abaixo do Estoque", Value = "2" });
                relat.Add(new SelectListItem() { Text = "Acima do Estoque", Value = "3" });
                relat.Add(new SelectListItem() { Text = "Estoque Zerado", Value = "4" });
                relat.Add(new SelectListItem() { Text = "Esgotando 30 dias", Value = "5" });
                ViewBag.Relatorio = new SelectList(relat, "Value", "Text");

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
                    if ((Int32)Session["MensProduto"] == 99)
                    {
                        ModelState.AddModelError("", "Foram processados e incluídos " + ((Int32)Session["Conta"]).ToString() + " produtos");
                        ModelState.AddModelError("", "Foram anotadas " + ((Int32)Session["Ressalva"]).ToString() + " ressalvas em produtos incluídos");
                        ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " produtos");
                    }
                    if ((Int32)Session["MensProduto"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0312", CultureInfo.CurrentCulture));
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
                objetoProd.PROD_IN_PECA = 0;
                Session["PrecoCustoAlterado"] = 0;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = null;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
                Session["VoltaProdutoWidget"] = 1;
                Session["VoltaCatProduto"] = 1;
                Session["VoltaSubCatProduto"] = 1;
                if (Session["FiltroProduto"] != null)
                {
                    objetoProd = (PRODUTO)Session["FiltroProduto"];
                }
                return View(objetoProd);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produtos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produtos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult ProcessaRelatorioProduto(Int32? TIPO_RELATORIO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32? tipoRel = TIPO_RELATORIO;

            if (tipoRel == 1)
            {
                return RedirectToAction("GerarRelatorioLista");
            }
            if (tipoRel == 2)
            {
                return RedirectToAction("GerarRelatorioListaAbaixo");
            }
            if (tipoRel == 3)
            {
                return RedirectToAction("GerarRelatorioListaAcima");
            }
            if (tipoRel == 4)
            {
                return RedirectToAction("GerarRelatorioListaZerado");
            }
            if (tipoRel == 5)
            {
                return RedirectToAction("GerarRelatorioListaEsgota");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        [HttpPost]
        public JsonResult GetProdutoNome(String term)
        {
            List<PRODUTO> usu = CarregarProduto();
            List<String> nomes = usu.Select(p => p.PROD_NM_NOME).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProdutoNomeLocacao(String term)
        {
            List<PRODUTO> usu = CarregarProduto().Where(p => p.PROD_IN_TIPO_PRODUTO == 2 & p.PROD_IN_LOCACAO == 1).ToList();

            List<String> nomes = usu.Select(p => p.PROD_NM_NOME).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMarcaNome(String term)
        {
            List<PRODUTO> usu = CarregarProduto();
            List<String> nomes = usu.Select(p => p.PROD_NM_MARCA).Distinct().ToList();
            var resultados = nomes
                .Where(n => n.ToLower().StartsWith(term.ToLower()))
                .Select(n => new { label = n, value = n })
                .ToList();
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RetirarFiltroProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaProduto"] = null;
                Session["FiltroProduto"] = null;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produtos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produtos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTodosProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterProd = prodApp.GetAllItens(idAss);
                Session["FiltroProduto"] = null;
                Session["ListaProduto"] = listaMasterProd;
                Session["BuscaProduto"] = 2;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produtos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produtos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        public ActionResult MostrarTudoProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                listaMasterProd = prodApp.GetAllItensAdm(idAss);
                Session["FiltroProduto"] = null;
                Session["ListaProduto"] = listaMasterProd;
                Session["BuscaProduto"] = 2;
                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produtos";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produtos", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        public ActionResult FiltrarProdutoBase(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaProduto");
                }

                // Sucesso
                Session["MensProduto"] = 0;
                if (item.PROD_IN_PECA == 0)
                {
                    listaMasterProd = volta.Item2;
                }
                else if (item.PROD_IN_PECA == 1)
                {
                    List<PRODUTO> acima = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = acima;
                }
                else if (item.PROD_IN_PECA == 2)
                {
                    List<PRODUTO> abaixo = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL <= p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = abaixo;
                }
                else if (item.PROD_IN_PECA == 3)
                {
                    List<PRODUTO> zerado = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = zerado;
                }
                else if (item.PROD_IN_PECA == 4)
                {
                    List<PRODUTO> esgota = (volta.Item2).Where(p => p.PROD_VL_MEDIA_VENDA_MENSAL > 0).ToList();
                    esgota = esgota.Where(p => (p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    listaMasterProd = esgota;
                }
                else 
                {
                    listaMasterProd = volta.Item2;
                }
                Session["ListaProduto"] = listaMasterProd;

                // Volta
                return RedirectToAction("MontarTelaProduto");
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

        public ActionResult VoltarBaseProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["Clonar"] == 1)
            {
                Session["Clonar"] = 0;
                listaMasterProd = new List<PRODUTO>();
                Session["ListaProduto"] = null;
            }
            if ((Int32)Session["VoltaProduto"] == 70)
            {
                return RedirectToAction("MontarTelaEstoque", "Estoque");
            }
            if ((Int32)Session["VoltaProduto"] == 70)
            {
                return RedirectToAction("MontarTelaEstoque", "Estoque");
            }
            if ((Int32)Session["VoltaProduto"] == 71)
            {
                return RedirectToAction("MontarTelaLocacao", "Locacao");
            }
            Session["ProdutoAlterada"] = 1;
            return RedirectToAction("MontarTelaProduto");
        }

        public ActionResult VoltarProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        public ActionResult VoltarBaseProdutoDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaProduto");
        }

        [HttpGet]
        public ActionResult IncluirProduto()
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                Session["ListaPrecoProduto"] = null;

                //Verifica possibilidade
                Int32 num = CarregarProduto().Count;
                if ((Int32)Session["NumProdutos"] <= num)
                {
                    Session["MensProduto"] = 51;
                    return RedirectToAction("MontarTelaProduto", "Produto");
                }

                // Prepara listas
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");

                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                List<SelectListItem> loca = new List<SelectListItem>();
                loca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                loca.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Locacao = new SelectList(loca, "Value", "Text");
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_10.pdf";

                // Mensagens
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0413", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0324", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0261", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0414", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 44)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0421", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 66)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0687", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 67)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0688", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                }

                // Prepara view
                Session["VoltaCatProduto"] = 2;
                Session["VoltaSubCatProduto"] = 2;
                PRODUTO item = new PRODUTO();
                ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
                vm.ASSI_CD_ID = idAss;
                vm.PROD_DT_CADASTRO = DateTime.Today.Date;
                vm.PROD_DT_ALTERACAO = DateTime.Today.Date;
                vm.PROD_IN_ATIVO = 1;
                vm.PROD_IN_COMPOSTO = 0;
                vm.PROD_VL_PRECO_VENDA = 0;
                vm.PROD_VL_PRECO_MINIMO = 0;
                vm.PROD_VL_PRECO_PROMOCAO = 0;
                vm.PROD_PC_DESCONTO = 0;
                vm.PROD_VL_ULTIMO_CUSTO = 0;
                vm.PROD_VL_PRECO_ANTERIOR = 0;
                vm.PROD_VL_CVM_PESO = 0;
                vm.PROD_VL_CVM_RECEITA = 0;
                vm.PROD_VL_CVM_UNITARIO = 0;
                vm.PROD_VL_FATOR_CORRECAO = 0;
                vm.PROD_VL_ESTOQUE_ATUAL = 0;
                vm.PROD_VL_ESTOQUE_MAXIMO = 0;
                vm.PROD_VL_ESTOQUE_MINIMO = 0;
                vm.PROD_VL_ESTOQUE_RESERVA = 0;
                vm.PROD_VL_MEDIA_VENDA_MENSAL = 0;
                vm.PROD_VL_ESTOQUE_ATUAL = 0;
                vm.PROD_IN_FRACIONADO = 0;
                vm.PROD_IN_PECA = 0;
                vm.PROD_IN_TIPO_PRODUTO = 0;
                vm.PROD_IN_SISTEMA = 6;
                vm.PROD_IN_LOCACAO = 0;
                vm.PROD_VL_LOCACAO = 0;
                vm.PROD_VL_LOCACAO_MULTA = 0;
                vm.PROD_VL_LOCACAO_PROMOCAO = 0;
                return View(vm);
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IncluirProduto(ProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Hashtable result = new Hashtable();

            ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
            ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
            List<SelectListItem> loca = new List<SelectListItem>();
            loca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            loca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Locacao = new SelectList(loca, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PROD_CD_CODIGO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_CD_CODIGO);
                    vm.PROD_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_NOME);
                    vm.PROD_NM_MARCA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_MARCA);
                    vm.PROD_NM_MODELO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_MODELO);
                    vm.PROD_NM_FABRICANTE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_FABRICANTE);
                    vm.PROD_NR_REFERENCIA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NR_REFERENCIA);
                    vm.PROD_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_DS_DESCRICAO);
                    vm.PROD_DS_INFORMACOES = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_DS_INFORMACOES);

                    // Preparação - Conversão   
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);

                    // Criticas
                    item.PROD_IN_COMPOSTO = 0;
                    if (item.PROD_VL_MEDIA_VENDA_MENSAL <= 0 || item.PROD_VL_MEDIA_VENDA_MENSAL == null)
                    {
                        item.PROD_VL_MEDIA_VENDA_MENSAL = 1;
                    }
                    //if (item.PROD_CD_CODIGO == null || item.PROD_CD_CODIGO == "0")
                    //{
                    //    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0324", CultureInfo.CurrentCulture));
                    //    return View(vm);
                    //}
                    if (item.PROD_CD_CODIGO != null & item.PROD_CD_CODIGO != "0" )
                    {
                        if (prodApp.CheckExist(item.PROD_CD_CODIGO, idAss) != null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0261", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    if (vm.PROD_IN_LOCACAO == 1)
                    {
                        if (vm.PROD_VL_LOCACAO == 0 || vm.PROD_VL_LOCACAO == null)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0687", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                        if (vm.PROD_VL_LOCACAO < vm.PROD_VL_LOCACAO_PROMOCAO)
                        {
                            ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0688", CultureInfo.CurrentCulture));
                            return View(vm);
                        }
                    }
                    else
                    {
                        vm.PROD_VL_LOCACAO = 0;
                        vm.PROD_VL_LOCACAO_PROMOCAO = 0;
                        vm.PROD_VL_LOCACAO_MULTA = 0;
                        vm.PROD_VL_LOCACAO_TAXAS = 0;
                        vm.PROD_IN_LOCACAO = 0;
                    }

                    // Executa a operação
                    item.PROD_IN_USUARIO_ALTERACAO = usuarioLogado.USUA_CD_ID;
                    item.PROD_DT_ALTERACAO = DateTime.Now;
                    Int32 volta = prodApp.ValidateCreate(item, usuarioLogado);
                    Int32 logId = volta;

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensProduto"] = 3;
                        return RedirectToAction("MontarTelaProduto");
                    }

                    // Acerta codigo do produto
                    if (item.PROD_CD_CODIGO == null)
                    {
                        item.PROD_CD_CODIGO = item.PROD_CD_ID.ToString();
                        volta = prodApp.ValidateEdit(item, item, usuarioLogado);
                    }

                    // Carrega foto e processa alteracao
                    if (item.PROD_AQ_FOTO == null)
                    {
                        item.PROD_AQ_FOTO = "~/Imagens/Base/icone_imagem.jpg";
                        volta = prodApp.ValidateEdit(item, item);
                    }

                    // Inclui no historico de custo
                    if (item.PROD_VL_ULTIMO_CUSTO != null)
                    {
                        PRODUTO_CUSTO custo = new PRODUTO_CUSTO();
                        custo.ASSI_CD_ID = idAss;
                        custo.PROD_CD_ID = item.PROD_CD_ID;
                        custo.PRCU_DT_CUSTO = DateTime.Today.Date;
                        custo.PRCU_VL_CUSTO = item.PROD_VL_ULTIMO_CUSTO;
                        custo.PRCU_IN_ATIVO = 1;
                        Int32 volta2 = prodApp.ValidateCreateCusto(custo, idAss);
                    }

                    // Inclui no historico de preco de venda
                    if (item.PROD_IN_TIPO_PRODUTO == 2)
                    {
                        if (item.PROD_VL_PRECO_VENDA != null)
                        {   
                            PRODUTO_PRECO_VENDA venda = new PRODUTO_PRECO_VENDA();
                            venda.ASSI_CD_ID = idAss;
                            venda.PROD_CD_ID = item.PROD_CD_ID;
                            venda.PRPV_DT_PRECO_VENDA = DateTime.Today.Date;
                            venda.PRPV_VL_PRECO_VENDA = item.PROD_VL_PRECO_VENDA;
                            venda.PRPV_PC_DESCONTO = item.PROD_PC_DESCONTO == null ? 0 : item.PROD_PC_DESCONTO;
                            venda.PRPV_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_PROMOCAO == null ? 0 : item.PROD_VL_PRECO_PROMOCAO;
                            venda.PRPV_IN_ATIVO = 1;
                            if (item.TIEM_CD_ID != null)
                            {
                                venda.TIEM_CD_ID = item.TIEM_CD_ID;
                            }
                            Int32 volta2 = prodApp.ValidateCreatePrecoVenda(venda, item.PROD_CD_ID, idAss);
                        }
                    }

                    // Inclui estoques
                    PRODUTO pp = prodApp.GetItemById(item.PROD_CD_ID);
                    EMPRESA emp = empApp.GetItemById(usuarioLogado.EMPR_CD_ID.Value);
                    List<EMPRESA_FILIAL> fils = emp.EMPRESA_FILIAL.Where(p => p.EMFI_IN_ATIVO == 1 & p.EMFI_IN_MATRIZ == 1).ToList();
                    foreach (EMPRESA_FILIAL fil in fils)
                    {
                        PRODUTO_ESTOQUE_FILIAL est = new PRODUTO_ESTOQUE_FILIAL();
                        est.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                        est.EMFI_CD_ID = fil.EMFI_CD_ID;
                        est.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID;
                        est.PROD_CD_ID = item.PROD_CD_ID;
                        est.PREF_IN_ATIVO = 1;
                        est.PREF_QN_ESTOQUE = 0;
                        est.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Today.Date;
                        est.PREF_QN_QUANTIDADE_RESERVADA = 0;
                        est.PREF_QN_QUANTIDADE_ALTERADA = 0;
                        pp.PRODUTO_ESTOQUE_FILIAL.Add(est);
                    }
                    volta = prodApp.ValidateEdit(pp, pp);

                    // Grava movimentação do estoque
                    MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                    mov.ASSI_CD_ID = idAss;
                    mov.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID;
                    mov.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    mov.PROD_CD_ID = item.PROD_CD_ID;
                    mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                    mov.MOEP_IN_TIPO_MOVIMENTO = 1;
                    mov.MOEP_VL_QUANTIDADE_MOVIMENTO = 0;
                    mov.MOEP_VL_QUANTIDADE_ANTERIOR = 0;
                    mov.MOEP_IN_ORIGEM = "Produto";
                    mov.MOEP_IN_CHAVE_ORIGEM = 0;
                    mov.MOEP_IN_ATIVO = 1;
                    mov.MOEP_IN_ULTIMO = 1;
                    mov.MOEP_DS_JUSTIFICATIVA = "Informação Inicial - Zerado";
                    mov.EMFI_CD_ID = usuarioLogado.EMFI_CD_ID;
                    mov.MOEP_EMFI_CD_ID = usuarioLogado.EMFI_CD_ID;
                    mov.MOEP_EMFI_CD_ID = null;
                    mov.MOEP_EMFI_CD_ID_ALVO = null;
                    mov.MOEP_IN_TIPO = 4;
                    mov.CRPV_CD_ID = null;
                    mov.COBA_CD_ID = null;
                    mov.FOPA_CD_ID = null;
                    mov.MOEP_VL_VALOR_MOVIMENTO = 0;
                    mov.FORN_CD_ID = null;
                    mov.MOEP_DS_MANUTENCAO_OBSERVACAO = null;
                    mov.MOEP_IN_PENDENTE = 0;
                    mov.MOEP_IN_AUTORIZADOR = null;
                    mov.MOEP_IN_TIPO_LANCAMENTO = 0;
                    mov.MOEP_DT_LANCAMENTO = null;
                    mov.MOEP_DT_PAGAMENTO = null;
                    mov.MOEP_DT_AUTORIZACAO = null;
                    mov.MOEP_GU_GUID = Xid.NewXid().ToString();
                    Int32 volta4 = prodApp.ValidateCreateMovimento(mov, usuarioLogado);

                    // Inclui historico de estoque
                    PRODUTO_ESTOQUE_HISTORICO hist = new PRODUTO_ESTOQUE_HISTORICO();
                    hist.ASSI_CD_ID = usuarioLogado.ASSI_CD_ID;
                    hist.PROD_CD_ID = item.PROD_CD_ID;
                    hist.PREH_IN_ATIVO = 1;
                    hist.PREH_DT_DATA = DateTime.Today.Date;
                    hist.PREH_DT_COMPLETA = DateTime.Now;
                    hist.PREH_QN_ESTOQUE = 0;
                    hist.PREH_IN_PENDENTE = 0;
                    hist.PREH_NM_TIPO = "Entrada";
                    hist.MOEP_CD_ID = mov.MOEP_CD_ID;
                    volta = prodApp.ValidateCreateEstoqueHistorico(hist, idAss);

                    // Grava Log de produto
                    PRODUTO_LOG logProd = new PRODUTO_LOG();
                    logProd.PROD_CD_ID = item.PROD_CD_ID;
                    logProd.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    logProd.LOG_CD_ID = logId;
                    logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                    logProd.PRLG_DS_OPERACAO = "Inclusão de Material/Produto";
                    Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Trata anexos
                    Session["IdVolta"] = item.PROD_CD_ID;
                    if (Session["FileQueueProduto"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueProduto"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueProduto(file);
                            }
                            else
                            {
                                UploadFotoQueueProduto(file);
                            }
                        }
                        Session["FileQueueProduto"] = null;
                    }

                    vm.PROD_CD_ID = item.PROD_CD_ID;
                    Session["IdProduto"] = item.PROD_CD_ID;
                    Session["IdVolta"] = item.PROD_CD_ID;
                    Session["MensProduto"] = 0;
                    Session["LinhaAlterada"] = item.PROD_CD_ID;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O material/produto " + item.PROD_NM_NOME.ToUpper() + " foi incluído com sucesso.";
                    Session["MensProduto"] = 61;

                    // Sucesso
                    listaMasterProd = new List<PRODUTO>();
                    Session["Produtos"] = null;
                    Session["ListaProduto"] = null;
                    Session["ProdutoAlterada"] = 1;
                    Session["FlagProduto"] = 1;
                    Session["AbaProduto"] = 1;
                    Session["RecuperaEstado"] = 1;
                    Session["FlagAlteraEstado"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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

        public DTO_Produto_Venda MontarProdutoVendaDTOObj(PRODUTO_PRECO_VENDA l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Produto_Venda()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    PRPV_CD_ID = l.PRPV_CD_ID,
                    PRPV_DT_PRECO_VENDA = l.PRPV_DT_PRECO_VENDA,
                    PROD_CD_ID = l.PROD_CD_ID,
                    PRPV_IN_ATIVO = l.PRPV_IN_ATIVO,
                    PRPV_IN_SISTEMA = l.PRPV_IN_SISTEMA,
                    PRPV_PC_DESCONTO = l.PRPV_PC_DESCONTO,
                    PRPV_VL_PRECO_EMBALAGEM = l.PRPV_VL_PRECO_EMBALAGEM,
                    PRPV_VL_PRECO_PROMOCAO = l.PRPV_VL_PRECO_PROMOCAO,
                    PRPV_VL_PRECO_VENDA = l.PRPV_VL_PRECO_VENDA,
                    TIEM_CD_ID = l.TIEM_CD_ID,
                };
                return mediDTO;
            }

        }

        public DTO_Produto_Concorrente MontarProdutoConcorrenteDTOObj(PRODUTO_CONCORRENTE l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Produto_Concorrente()
                {
                    PROD_CD_ID = l.PROD_CD_ID,
                    PRPF_CD_ID = l.PRPF_CD_ID,
                    PRPF_DT_CADASTRO = l.PRPF_DT_CADASTRO,
                    PRPF_IN_ATIVO = l.PRPF_IN_ATIVO,
                    PRPF_IN_SISTEMA = l.PRPF_IN_SISTEMA,
                    PRPF_NM_CONCORRENTE = l.PRPF_NM_CONCORRENTE,
                    PRPF_VL_PRECO_CONCORRENTE = l.PRPF_VL_PRECO_CONCORRENTE,
                };
                return mediDTO;
            }

        }

        public DTO_Produto_Custo MontarProdutoCustoDTOObj(PRODUTO_CUSTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Produto_Custo()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    PRCU_CD_ID = l.PRCU_CD_ID,
                    PRCU_DT_CUSTO = l.PRCU_DT_CUSTO,
                    PROD_CD_ID = l.PROD_CD_ID,
                    PRCU_IN_ATIVO = l.PRCU_IN_ATIVO,
                    PRCU_IN_SISTEMA = l.PRCU_IN_SISTEMA,
                    PRCU_VL_CUSTO = l.PRCU_VL_CUSTO,
                };
                return mediDTO;
            }

        }

        public ActionResult ClonarProduto(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_INCLUSAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Sufixo
                String sufixo = Cryptography.GenerateRandomPassword(4);

                // Prepara objeto
                PRODUTO item = prodApp.GetItemById(id);
                PRODUTO novo = new PRODUTO();
                novo.ASSI_CD_ID = item.ASSI_CD_ID;
                novo.CAPR_CD_ID = item.CAPR_CD_ID;
                novo.PROD_AQ_FOTO = item.PROD_AQ_FOTO;
                novo.PROD_DS_DESCRICAO = item.PROD_DS_DESCRICAO;
                novo.PROD_DS_INFORMACOES = item.PROD_DS_INFORMACOES;
                novo.PROD_IN_ATIVO = 1;
                novo.PROD_IN_TIPO_PRODUTO = item.PROD_IN_TIPO_PRODUTO;
                if (item.PROD_IN_TIPO_PRODUTO == 1)
                {
                    novo.PROD_NM_NOME = item.PROD_NM_NOME + " ====== MATERIAL DUPLICADO ======";
                }
                else
                {
                    novo.PROD_NM_NOME = item.PROD_NM_NOME + " ====== PRODUTO DUPLICADO ======";
                }
                novo.PROD_NR_REFERENCIA = item.PROD_NR_REFERENCIA;
                novo.PROD_TX_OBSERVACOES = item.PROD_TX_OBSERVACOES;
                novo.PROD_VL_ULTIMO_CUSTO = 0;
                novo.PROD_VL_PRECO_MINIMO = 0;
                novo.PROD_VL_PRECO_PROMOCAO = 0;
                novo.PROD_VL_PRECO_VENDA = 0;
                novo.PROD_VL_MARGEM_CONTRIBUICAO = item.PROD_VL_MARGEM_CONTRIBUICAO;
                novo.PROD_VL_PRECO_ANTERIOR = item.PROD_VL_PRECO_ANTERIOR;
                novo.PROD_PC_DESCONTO = 0;
                novo.PROD_NM_MARCA = item.PROD_NM_MARCA;
                novo.PROD_NM_MODELO = item.PROD_NM_MODELO;
                novo.PROD_NM_FABRICANTE = item.PROD_NM_FABRICANTE;
                novo.SCPR_CD_ID = item.SCPR_CD_ID;
                novo.UNID_CD_ID = item.UNID_CD_ID;
                novo.PROD_CD_CODIGO = item.PROD_CD_CODIGO + "_" + sufixo;
                novo.PROD_DT_ALTERACAO = DateTime.Today.Date;
                novo.PROD_DT_CADASTRO = DateTime.Today.Date;
                novo.PROD_IN_COMPOSTO = item.PROD_IN_COMPOSTO;
                novo.PROD_IN_PECA = item.PROD_IN_PECA;
                novo.PROD_IN_USUARIO_ALTERACAO = usuario.USUA_CD_ID;
                novo.PROD_NM_REFERENCIA_FABRICANTE = item.PROD_NM_REFERENCIA_FABRICANTE;
                novo.PROD_NR_BARCODE = null;
                novo.PROD_TX_OBSERVACOES = item.PROD_TX_OBSERVACOES;
                novo.PROD_VL_CUSTO = 0;
                novo.PROD_VL_CUSTO_CONCORRENTE_MEDIO = 0;
                novo.PROD_VL_CVM_PESO = 0;
                novo.PROD_VL_CVM_RECEITA = 0;
                novo.PROD_VL_CVM_UNITARIO = 0;
                novo.PROD_VL_ESTOQUE_ATUAL = 0;
                novo.PROD_VL_ESTOQUE_CUSTO = 0;
                novo.PROD_VL_ESTOQUE_MAXIMO = 0;
                novo.PROD_VL_ESTOQUE_MINIMO = 0;
                novo.PROD_VL_ESTOQUE_RESERVA = 0;
                novo.PROD_VL_ESTOQUE_VENDA = 0;
                novo.PROD_VL_FATOR_CORRECAO = 0;
                novo.PROD_VL_MARGEM_CONTRIBUICAO = 0;
                novo.PROD_VL_MEDIA_VENDA_MENSAL = 0;
                novo.PROD_VL_PRECO_ANTERIOR = 0;
                novo.SCPR_CD_ID = item.SCPR_CD_ID;
                novo.TIEM_CD_ID = item.TIEM_CD_ID;
                novo.PROD_IN_FRACIONADO = item.PROD_IN_FRACIONADO;
                novo.PROD_NM_FORNECEDOR = item.PROD_NM_FORNECEDOR;
                novo.PROD_IN_LOCACAO = 0;
                novo.PROD_VL_LOCACAO = 0;
                novo.PROD_VL_LOCACAO_MULTA = 0;
                novo.PROD_VL_LOCACAO_PROMOCAO = 0;
                novo.PROD_VL_LOCACAO_TAXAS = 0;

                Int32 volta = prodApp.ValidateCreate(novo, usuario);
                Session["IdVolta"] = novo.PROD_CD_ID;
                Session["Clonar"] = 1;

                // Cria pastas
                String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/Fotos/";
                Directory.CreateDirectory(Server.MapPath(caminho));
                caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/Anexos/";
                Directory.CreateDirectory(Server.MapPath(caminho));

                // Copia imagem
                String origem = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
                String destino = "/Imagens/" + idAss.ToString() + "/Produtos/" + novo.PROD_CD_ID.ToString() + "/Fotos/";

                String caminhoFisicoOrigem = Server.MapPath(origem);
                String caminhoFisicoDestino = Server.MapPath(destino);

                String[] listaArquivos = Directory.GetFiles(caminhoFisicoOrigem, "*", SearchOption.TopDirectoryOnly);
                foreach (String caminhoCompletoArquivoOrigem in listaArquivos)
                {
                    // Extrai apenas o nome do arquivo (ex: "imagem1.jpg")
                    String nomeArquivo = Path.GetFileName(caminhoCompletoArquivoOrigem);

                    // Cria o caminho completo do arquivo de destino
                    String caminhoCompletoArquivoDestino = Path.Combine(caminhoFisicoDestino, nomeArquivo);

                    // Copia o arquivo (True permite sobrescrever se já existir)
                    System.IO.File.Copy(caminhoCompletoArquivoOrigem, caminhoCompletoArquivoDestino, true);
                }

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O material/produto " + item.PROD_NM_NOME.ToUpper() + " foi duplicado com sucesso.";
                Session["MensProduto"] = 61;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult EditarProduto(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produto";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                PRODUTO item = prodApp.GetItemById(id);
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == item.PROD_IN_TIPO_PRODUTO).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.CAPR_CD_ID == item.CAPR_CD_ID).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                List<SelectListItem> loca = new List<SelectListItem>();
                loca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
                loca.Add(new SelectListItem() { Text = "Não", Value = "0" });
                ViewBag.Locacao = new SelectList(loca, "Value", "Text");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_11.pdf";
                Session["Limite"] = DateTime.Today.Date.AddDays(-30);
                List<MOVIMENTO_ESTOQUE_PRODUTO> estoques = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(p => p.MOEP_IN_ATIVO == 1).OrderByDescending(p => p.MOEP_DT_MOVIMENTO).Take(10).ToList();
                ViewBag.ListaMov = estoques;

                // Recupera custo
                PRODUTO_CUSTO custo = item.PRODUTO_CUSTO.ToList().OrderByDescending(p => p.PRCU_CD_ID).FirstOrDefault();
                if (custo != null)
                {
                    item.PROD_VL_ULTIMO_CUSTO = custo.PRCU_VL_CUSTO.Value;
                }
                else
                {
                    item.PROD_VL_ULTIMO_CUSTO = 0;
                }

                // Monta evolução custos
                List<PRODUTO_CUSTO> custos = item.PRODUTO_CUSTO.ToList();
                List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
                foreach (PRODUTO_CUSTO itemCusto in custos)
                {
                    ModeloViewModel mod1 = new ModeloViewModel();
                    mod1.DataEmissao = itemCusto.PRCU_DT_CUSTO.Value;
                    mod1.ValorDec1 = itemCusto.PRCU_VL_CUSTO.Value;
                    lista1.Add(mod1);
                }
                lista1 = lista1.OrderBy(p => p.DataEmissao).ToList();
                ViewBag.ListaCusto = lista1;
                ViewBag.ContaCusto = lista1.Count;
                Session["ListaCusto"] = lista1;

                // Monta evolução precos
                if (item.PROD_IN_TIPO_PRODUTO == 2)
                {
                    List<PRODUTO_PRECO_VENDA> precos = item.PRODUTO_PRECO_VENDA.ToList();
                    List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
                    foreach (PRODUTO_PRECO_VENDA itemPreco in precos)
                    {
                        ModeloViewModel mod1 = new ModeloViewModel();
                        mod1.DataEmissao = itemPreco.PRPV_DT_PRECO_VENDA.Value;
                        mod1.ValorDec1 = itemPreco.PRPV_VL_PRECO_VENDA.Value;
                        mod1.ValorDec2 = itemPreco.PRPV_VL_PRECO_PROMOCAO.Value;
                        lista2.Add(mod1);
                    }
                    lista2 = lista2.OrderBy(p => p.DataEmissao).ToList();
                    ViewBag.ListaPreco = lista2;
                    ViewBag.ContaPreco = lista2.Count;
                    Session["ListaPreco"] = lista2;
                }
                else
                {
                    Session["ListaPreco"] = null;
                }

                // Monta evolução movimentacoes
                DateTime limite = (DateTime)Session["Limite"];
                estoques = item.MOVIMENTO_ESTOQUE_PRODUTO.Where(p => p.MOEP_IN_ATIVO == 1).ToList();
                List<DateTime> datas = estoques.Select(p => p.MOEP_DT_MOVIMENTO.Date).Distinct().ToList();
                datas.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
                foreach (DateTime data in datas)
                {
                    if (data.Date > limite)
                    {
                        decimal? conta = estoques.Where(p => p.MOEP_DT_MOVIMENTO.Date == data).Sum(p => p.MOEP_VL_QUANTIDADE_MOVIMENTO);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = data;
                        mod.ValorDec1 = conta.Value;
                        lista5.Add(mod);
                    }
                }
                lista5 = lista5.OrderBy(p => p.DataEmissao).ToList();
                ViewBag.ListaEstoque = lista5;
                ViewBag.ContaEstoque = lista5.Count;
                Session["ListaEstoque"] = lista5;

                // Monta evolução estoque
                List<PRODUTO_ESTOQUE_HISTORICO> hist = item.PRODUTO_ESTOQUE_HISTORICO.ToList();
                List<DateTime> datas1 = hist.Select(p => p.PREH_DT_DATA.Value.Date).Distinct().ToList();
                datas1.Sort((i, j) => i.Date.CompareTo(j.Date));
                List<ModeloViewModel> lista8 = new List<ModeloViewModel>();
                foreach (DateTime data in datas1)
                {
                    if (data.Date > limite)
                    {
                        decimal? conta = hist.Where(p => p.PREH_DT_DATA.Value.Date == data).Sum(p => p.PREH_QN_ESTOQUE);
                        ModeloViewModel mod = new ModeloViewModel();
                        mod.DataEmissao = data;
                        mod.ValorDec1 = conta.Value;
                        lista8.Add(mod);
                    }
                }
                lista8 = lista8.OrderBy(p => p.DataEmissao).ToList();
                ViewBag.ListaEstoqueTotal = lista8;
                ViewBag.ContaEstoqueTotal = lista8.Count;
                Session["ListaEstoqueTotal"] = lista8;

                // Recupera preço medio da concorrencia
                Int32 totalConc = 0;
                Decimal somaConc = 0;
                Decimal media = 0;
                if (item.PROD_IN_TIPO_PRODUTO == 2)
                {
                    List<PRODUTO_CONCORRENTE> concs = item.PRODUTO_CONCORRENTE.ToList();
                    if (concs.Count > 0)
                    {
                        totalConc = concs.Count;
                        somaConc = concs.Sum(p => p.PRPF_VL_PRECO_CONCORRENTE);
                        media = somaConc / totalConc;
                    }
                }

                // Monta preco x Concorrencia
                if (item.PROD_IN_TIPO_PRODUTO == 2)
                {
                    List<PRODUTO_PRECO_VENDA> precos1 = item.PRODUTO_PRECO_VENDA.ToList();
                    List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
                    foreach (PRODUTO_PRECO_VENDA itemPreco in precos1)
                    {
                        ModeloViewModel mod1 = new ModeloViewModel();
                        mod1.DataEmissao = itemPreco.PRPV_DT_PRECO_VENDA.Value;
                        mod1.ValorDec1 = itemPreco.PRPV_VL_PRECO_VENDA.Value;
                        mod1.ValorDec2 = media;
                        lista4.Add(mod1);
                    }
                    lista4 = lista4.OrderBy(p => p.DataEmissao).ToList();
                    ViewBag.ListaConc = lista4;
                    ViewBag.ContaConc = lista4.Count;
                    Session["ListaConc"] = lista4;
                }

                // Calcula flag de estoque
                Int32 flagEstoque = 0;
                if (item.PROD_VL_ESTOQUE_ATUAL < item.PROD_VL_ESTOQUE_MINIMO)
                {
                    flagEstoque = 1;
                }
                if (item.PROD_VL_ESTOQUE_ATUAL > item.PROD_VL_ESTOQUE_MAXIMO)
                {
                    flagEstoque = 2;
                }
                if (item.PROD_VL_ESTOQUE_ATUAL <= 0)
                {
                    flagEstoque = 3;
                }

                // Exibe mensagem
                if (Session["MensProduto"] != null)
                {
                    if ((Int32)Session["MensProduto"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 6)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0431", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 30)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 33)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0287", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 34)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0288", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 35)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0289", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 36)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0290", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 37)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0291", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 38)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0292", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensProduto"] == 66)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0687", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 67)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0688", CultureInfo.CurrentCulture));
                        Session["MensProduto"] = 0;
                    }
                    if ((Int32)Session["MensProduto"] == 61)
                    {
                        TempData["MensagemAcerto"] = (String)Session["MsgCRUD"];
                        TempData["TemMensagem"] = 1;
                    }
                }

                // Exibe
                Session["Acerta"] = 0;
                Session["MensProduto"] = 0;
                Session["VoltaConsulta"] = 1;
                objetoProdAntes = item;
                Session["Produto"] = item;
                Session["IdVolta"] = id;
                Session["IdProduto"] = id;
                Session["VoltaLog"] = 2;
                Session["VoltaCatProduto"] = 3;
                Session["VoltaSubCatProduto"] = 3;
                ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
                return View(vm);
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditarProduto(ProdutoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == vm.PROD_IN_TIPO_PRODUTO).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
            ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.CAPR_CD_ID == vm.CAPR_CD_ID).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
            ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 6).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            List<SelectListItem> loca = new List<SelectListItem>();
            loca.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            loca.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Locacao = new SelectList(loca, "Value", "Text");


            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitização
                    vm.PROD_CD_CODIGO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_CD_CODIGO);
                    vm.PROD_NM_NOME = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_NOME);
                    vm.PROD_NM_MARCA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_MARCA);
                    vm.PROD_NM_MODELO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_MODELO);
                    vm.PROD_NM_FABRICANTE = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NM_FABRICANTE);
                    vm.PROD_NR_REFERENCIA = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_NR_REFERENCIA);
                    vm.PROD_DS_DESCRICAO = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_DS_DESCRICAO);
                    vm.PROD_DS_INFORMACOES = CrossCutting.UtilitariosGeral.CleanStringGeralNoBreak(vm.PROD_DS_INFORMACOES);

                    //Critica
                    if (vm.PROD_IN_LOCACAO == 1)
                    {
                        if (vm.PROD_VL_LOCACAO == 0 || vm.PROD_VL_LOCACAO == null)
                        {
                            Session["MensProduto"] = 66;
                            return View(vm);
                        }
                        if (vm.PROD_VL_LOCACAO < vm.PROD_VL_LOCACAO_PROMOCAO)
                        {
                            Session["MensProduto"] = 67;
                            return View(vm);
                        }
                    }
                    else
                    {
                        vm.PROD_VL_LOCACAO = 0;
                        vm.PROD_VL_LOCACAO_PROMOCAO = 0;
                        vm.PROD_VL_LOCACAO_MULTA = 0;
                        vm.PROD_VL_LOCACAO_TAXAS = 0;
                    }

                    // Acerta media
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    if (vm.PROD_VL_MEDIA_VENDA_MENSAL <= 0)
                    {
                        vm.PROD_VL_MEDIA_VENDA_MENSAL = 1;
                    }

                    // Acerta alteracao
                    vm.PROD_DT_ALTERACAO = DateTime.Today.Date;
                    vm.PROD_IN_USUARIO_ALTERACAO = usuarioLogado.USUA_CD_ID;
                    PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);

                    // Grava alteração
                    Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes, usuarioLogado);
                    Int32 logId = volta;

                    // Soma estoques
                    if (item.PROD_IN_COMPOSTO == 0)
                    {
                        PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                        List<PRODUTO_ESTOQUE_FILIAL> ests = prod.PRODUTO_ESTOQUE_FILIAL.Where(p => p.PREF_IN_ATIVO == 1).ToList();
                        Int32 soma = ests.Sum(p => p.PREF_QN_ESTOQUE.Value);
                        prod.PROD_VL_ESTOQUE_ATUAL = soma;
                        Int32 volta1 = prodApp.ValidateEdit(prod, prod);
                    }

                    // Grava Log de produto
                    Session["IdProduto"] = item.PROD_CD_ID;
                    PRODUTO_LOG logProd = new PRODUTO_LOG();
                    logProd.PROD_CD_ID = item.PROD_CD_ID;
                    logProd.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    logProd.LOG_CD_ID = logId;
                    logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                    logProd.PRLG_DS_OPERACAO = "Alteração de Produto";
                    Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                    // Sucesso
                    listaMasterProd = new List<PRODUTO>();
                    Session["ListaProduto"] = null;
                    Session["ProdutoAlterada"] = 1;
                    Session["FlagProduto"] = 1;
                    Session["FlagAlteraEstado"] = 1;
                    Session["Produtos"] = null;

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O material/produto " + item.PROD_NM_NOME.ToUpper() + " foi alterado com sucesso.";
                    Session["MensProduto"] = 61;

                    // Retornos
                    Session["AbaProduto"] = 1;
                    Session["LinhaAlterada"] = item.PROD_CD_ID;
                    Int32? a = (Int32)Session["ProdutoAlterada"];
                    Session["RecuperaEstado"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ConsultarProduto(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuario = (USUARIO)Session["UserCredentials"];

                // Prepara view
                PRODUTO item = prodApp.GetItemById(id);
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                ViewBag.CdUsuario = usuario.USUA_CD_ID;

                // Recupera precos
                PRODUTO_PRECO_VENDA prec = item.PRODUTO_PRECO_VENDA.ToList().OrderByDescending(p => p.PRPV_CD_ID).FirstOrDefault();
                item.PROD_VL_PRECO_VENDA = prec.PRPV_VL_PRECO_VENDA.Value;
                item.PROD_VL_PRECO_PROMOCAO = prec.PRPV_VL_PRECO_PROMOCAO.Value;
                item.PROD_PC_DESCONTO = prec.PRPV_PC_DESCONTO.Value;
                item.PROD_VL_PRECO_ANTERIOR = item.PROD_VL_PRECO_VENDA;
                item.PROD_VL_PRECO_MINIMO = prec.PRPV_VL_PRECO_EMBALAGEM.Value;

                // Recupera custo
                PRODUTO_CUSTO custo = item.PRODUTO_CUSTO.ToList().OrderByDescending(p => p.PRCU_CD_ID).FirstOrDefault();
                item.PROD_VL_ULTIMO_CUSTO = custo.PRCU_VL_CUSTO.Value;

                objetoProdAntes = item;
                Session["Produto"] = item;
                Session["IdVolta"] = id;
                Session["IdProduto"] = id;
                ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
                return View(vm);
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

        // Filtro em cascata de subcategoria
        [HttpPost]
        public JsonResult FiltrarSubCategoriaProduto(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaSubFiltrada = CarregarSubCatProduto().OrderBy(p => p.SCPR_NM_NOME).ToList();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    listaSubFiltrada = listaSubFiltrada.Where(x => x.CAPR_CD_ID == id & x.SCPR_IN_TIPO == 1).ToList();
                }
                return Json(listaSubFiltrada.Select(x => new { x.SCPR_CD_ID, x.SCPR_NM_NOME }));
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
                return null;
            }
        }

        [HttpPost]
        public JsonResult FiltrarUnidade(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaSubFiltrada = CarregarUnidade();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    if (id == 1)
                    {
                        listaSubFiltrada = listaSubFiltrada.Where(x => x.UNID_IN_FRACIONADA == 1).ToList();
                    }
                }
                return Json(listaSubFiltrada.Select(x => new { x.UNID_CD_ID, x.UNID_NM_NOME }));
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
                return null;
            }
        }

        [HttpPost]
        public JsonResult FiltrarProduto(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaSubFiltrada = CarregarProduto();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    listaSubFiltrada = listaSubFiltrada.Where(x => x.SCPR_CD_ID == id & x.PROD_IN_TIPO_PRODUTO != 3 & x.PROD_IN_COMPOSTO == 0).ToList();
                }
                return Json(listaSubFiltrada.Select(x => new { x.PROD_CD_ID, x.PROD_NM_NOME }));
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
                return null;
            }
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaProduto(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaFiltrada = cpApp.GetAllItens(idAss).OrderBy(p => p.CAPR_NM_NOME).ToList();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    listaFiltrada = listaFiltrada.Where(x => x.SUBCATEGORIA_PRODUTO.Any(s => s.SCPR_CD_ID == id) & x.CAPR_IN_TIPO == 1).ToList();
                }

                return Json(listaFiltrada.Select(x => new { x.CAPR_CD_ID, x.CAPR_NM_NOME }));
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
                return null;
            }
        }

        [HttpPost]
        public JsonResult FiltrarCategoriaTipo(Int32? id)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                var listaFiltrada = cpApp.GetAllItens(idAss).OrderBy(p => p.CAPR_NM_NOME).ToList();

                // Filtro para caso o placeholder seja selecionado
                if (id != null)
                {
                    listaFiltrada = listaFiltrada.Where(x => x.CAPR_IN_TIPO == id).ToList();
                }
                return Json(listaFiltrada.Select(x => new { x.CAPR_CD_ID, x.CAPR_NM_NOME }));
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
                return null;
            }
        }

        [HttpGet]
        public ActionResult ExcluirProduto(Int32 id)
        {
            try
            {
                // Valida acesso
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];
                    //Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_EXCLUSAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Executar

                // Processa exclusão
                PRODUTO item = prodApp.GetItemById(id);
                objetoProdAntes = (PRODUTO)Session["Produto"];
                item.PROD_IN_ATIVO = 0;
                Int32 volta = prodApp.ValidateDelete(item, usuario);
                Int32 logId = volta;

                // Retorno
                if (volta == 1)
                {
                    Session["MensProduto"] = 5;
                    return RedirectToAction("MontarTelaProduto", "Produto");
                }

                // Grava Log de produto
                PRODUTO_LOG logProd = new PRODUTO_LOG();
                logProd.PROD_CD_ID = item.PROD_CD_ID;
                logProd.USUA_CD_ID = usuario.USUA_CD_ID;
                logProd.LOG_CD_ID = logId;
                logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                logProd.PRLG_DS_OPERACAO = "Exclusão de Produto";
                Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O material/produto " + item.PROD_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensProduto"] = 61;

                // Finalização
                listaMasterProd = new List<PRODUTO>();
                Session["ListaProduto"] = null;
                Session["FiltroProduto"] = null;
                Session["ProdutoAlterada"] = 1;
                Session["FlagProduto"] = 1;
                Session["RecuperaEstado"] = 1;
                Session["FlagProduto"] = 1;
                Session["FlagAlteraEstado"] = 1;
                return RedirectToAction("MontarTelaProduto");
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

        [HttpGet]
        public ActionResult ReativarProduto(Int32 id)
        {
            try
            {
                // Valida acesso
                USUARIO usuario = new USUARIO();
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];
                    //Verfifica permissão
                    if (usuario.PERFIL.PERF_IN_REATIVA_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                //Verifica possibilidade
                Int32 num = CarregarProduto().Count;
                if ((Int32)Session["NumProdutos"] <= num)
                {
                    Session["MensProduto"] = 51;
                    return RedirectToAction("MontarTelaProduto", "Produto");
                }

                // Executar
                // Processa reativação
                PRODUTO item = prodApp.GetItemById(id);
                objetoProdAntes = (PRODUTO)Session["Produto"];
                item.PROD_IN_ATIVO = 1;
                Int32 volta = prodApp.ValidateReativar(item, usuario);
                Int32 logId = volta;

                // Grava Log de produto
                PRODUTO_LOG logProd = new PRODUTO_LOG();
                logProd.PROD_CD_ID = item.PROD_CD_ID;
                logProd.USUA_CD_ID = usuario.USUA_CD_ID;
                logProd.LOG_CD_ID = logId;
                logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                logProd.PRLG_DS_OPERACAO = "Reativação de Produto";
                Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                // Finalização
                listaMasterProd = new List<PRODUTO>();
                Session["ListaProduto"] = null;
                Session["FiltroProduto"] = null;
                Session["ProdutoAlterada"] = 1;
                Session["FlagProduto"] = 1;
                Session["RecuperaEstado"] = 1;
                Session["FlagAlteraEstado"] = 1;
                return RedirectToAction("MontarTelaProduto");
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

        [HttpGet]
        public ActionResult VerAnexoProduto(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
                Session["AbaProduto"] = 5;
                return View(item);
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


        public ActionResult VoltarAnexoProduto()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((Int32)Session["VoltaProduto"] == 70)
            {
                return RedirectToAction("MontarTelaEstoque", "Estoque");
            }
            if ((Int32)Session["VoltaProduto"] == 71)
            {
                return RedirectToAction("MontarTelaLocacao", "Locacao");
            }
            Session["ListaProduto"] = null;
            Session["ProdutoAlterada"] = 1;
            Session["Produtos"] = null;
            return RedirectToAction("EditarProduto", new { id = (Int32)Session["IdProduto"] });
        }

        public FileResult DownloadProduto(Int32 id)
        {
            try
            {
                PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
                String arquivo = item.PRAN_AQ_ARQUIVO;
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
                else if (arquivo.Contains(".mp4"))
                {
                    contentType = "video/mp4";
                }
                return File(arquivo, contentType, nomeDownload);
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
                return null;
            }
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            try
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
                Session["FileQueueProduto"] = queue;
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
            }
        }

        [HttpPost]
        public ActionResult UploadFileQueueProduto(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensProduto"] = 5;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                Int32 idProd = (Int32)Session["IdVolta"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                PRODUTO item = prodApp.GetById(idProd);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 100)
                {
                    Session["MensProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                System.IO.File.WriteAllBytes(path, file.Contents);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PRODUTO_ANEXO foto = new PRODUTO_ANEXO();
                foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PRAN_DT_ANEXO = DateTime.Today;
                foto.PRAN_IN_ATIVO = 1;
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
                foto.PRAN_IN_TIPO = tipo;
                foto.PRAN_NM_TITULO = fileName;
                foto.PROD_CD_ID = item.PROD_CD_ID;

                item.PRODUTO_ANEXO.Add(foto);
                objetoProdAntes = item;
                Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes);
                Session["AbaProduto"] = 5;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpPost]
        public async Task<ActionResult> UploadFileProduto(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensProduto"] = 5;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                Int32 idProd = (Int32)Session["IdVolta"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                PRODUTO item = prodApp.GetById(idProd);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 100)
                {
                    Session["MensProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensProduto"] = 7;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Anexos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
                //file.SaveAs(path);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Gravar registro
                PRODUTO_ANEXO foto = new PRODUTO_ANEXO();
                foto.PRAN_AQ_ARQUIVO = "~" + caminho + fileName;
                foto.PRAN_DT_ANEXO = DateTime.Today;
                foto.PRAN_IN_ATIVO = 1;
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
                foto.PRAN_IN_TIPO = tipo;
                foto.PRAN_NM_TITULO = fileName;
                foto.PROD_CD_ID = item.PROD_CD_ID;

                item.PRODUTO_ANEXO.Add(foto);
                objetoProdAntes = item;
                Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes);
                Session["AbaProduto"] = 5;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usu.ASSI_CD_ID,
                    USUA_CD_ID = usu.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Produto - Anexo - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Produto: " + item.PROD_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpPost]
        public ActionResult UploadFotoQueueProduto(FileQueue file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensProduto"] = 5;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                Int32 idProd = (Int32)Session["IdVolta"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                PRODUTO item = prodApp.GetById(idProd);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = file.Name;
                if (fileName.Length > 100)
                {
                    Session["MensProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
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
                    item.PROD_AQ_FOTO = "~" + caminho + fileName;
                    objetoProd = item;
                    Int32 volta = prodApp.ValidateEdit(item, objetoProd);
                }
                else
                {
                    ViewBag.Message = CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
                }
                Session["AbaProduto"] = 3;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpPost]
        public async Task<ActionResult> UploadFotoProduto(HttpPostedFileBase file)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                if (file == null)
                {
                    Session["MensProduto"] = 5;
                    return RedirectToAction("VoltarAnexoProduto");
                }
                Int32 idProd = (Int32)Session["IdVolta"];
                Int32 idAss = (Int32)Session["IdAssinante"];

                PRODUTO item = prodApp.GetById(idProd);
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                var fileName = Path.GetFileName(file.FileName);
                if (fileName.Length > 100)
                {
                    Session["MensProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Critica tamanho arquivo
                var fileSize = file.ContentLength;
                if (fileSize > 50000000)
                {
                    Session["MensProduto"] = 7;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                String caminho = "/Imagens/" + idAss.ToString() + "/Produtos/" + item.PROD_CD_ID.ToString() + "/Fotos/";
                String path = Path.Combine(Server.MapPath(caminho), fileName);
                file.SaveAs(path);

                //Recupera tipo de arquivo
                extensao = Path.GetExtension(fileName);
                String a = extensao;

                // Checa extensão
                if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
                {
                    // Salva arquivo
                    //file.SaveAs(path);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.InputStream.CopyToAsync(stream);
                    }

                    // Gravar registro
                    item.PROD_AQ_FOTO = "~" + caminho + fileName;
                    objetoProd = item;
                    Int32 volta = prodApp.ValidateEdit(item, objetoProd);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usu.ASSI_CD_ID,
                        USUA_CD_ID = usu.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Foto - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Produto: " + item.PROD_NM_NOME.ToUpper() + " | Anexo: " + fileName + " | Data: " + DateTime.Today.Date,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                }
                else
                {
                    ViewBag.Message = CRMSys_Base.ResourceManager.GetString("M0016", CultureInfo.CurrentCulture);
                }
                Session["AbaProduto"] = 3;
                return RedirectToAction("VoltarAnexoProduto");
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

        public ActionResult GerarRelatorioFiltro()
        {
            return RedirectToAction("GerarRelatorioLista", new { id = 1 });
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
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<PRODUTO> lista = new List<PRODUTO>();
                nomeRel = "ProdutoLista" + "_" + data + ".pdf";
                titulo = "Produtos - Listagem";
                lista = (List<PRODUTO>)Session["ListaProduto"];
                PRODUTO filtro = (PRODUTO)Session["FiltroProduto"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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

                    cell1 = new PdfPCell(new Paragraph("Materiais/Produtos", meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Materiais/Produtos", meuFont2))
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

                // Linha horizontal
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                // Grid
                PdfPTable table = new PdfPTable(new float[] { 50f, 180f, 80f, 80f, 50f, 100f, 100f, 50f, 50f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Materiais/Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 11;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Tipo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Nome", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Venda (R$)", meuFont))
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
                cell = new PdfPCell(new Paragraph("Imagem", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

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
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Produto", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }

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
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.PROD_IN_TIPO_PRODUTO == 2)
                    {
                        if (item.PROD_VL_PRECO_VENDA != null)
                        {
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_PRECO_VENDA.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(new Paragraph("0,00", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
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

                    if (item.PROD_VL_ESTOQUE_ATUAL != null)
                    {
                        cell = new PdfPCell(new Paragraph(item.PROD_VL_ESTOQUE_ATUAL.Value.ToString(), meuFont))
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

                // Rodapé
                Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk1);

                String parametros = String.Empty;
                Int32 ja = 0;
                if (filtro != null)
                {
                    if (filtro.CAPR_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtro.CAPR_CD_ID.ToString();
                        ja = 1;
                    }
                    if (filtro.SCPR_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                        }
                    }
                    if (filtro.PROD_CD_CODIGO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Código: " + filtro.PROD_CD_CODIGO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Código: " + filtro.PROD_CD_CODIGO;
                        }
                    }
                    if (filtro.PROD_NM_NOME != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtro.PROD_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: " + filtro.PROD_NM_NOME;
                        }
                    }
                    if (filtro.PROD_NM_MARCA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Marca: " + filtro.PROD_NM_MARCA;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Marca: " + filtro.PROD_NM_MARCA;
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

                // Finaliza
                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();

                return RedirectToAction("MontarTelaProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "CRMsys", 1, (USUARIO)Session["UserCredentials"]);
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
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                PRODUTO aten = prodApp.GetItemById((Int32)Session["IdVolta"]);
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = "Produto_" + aten.PROD_CD_ID.ToString() + "_" + data + ".pdf";
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
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

                    cell1 = new PdfPCell(new Paragraph("Relatório Detalhado - " + aten.PROD_NM_NOME, meuFont2))
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
                    PdfPCell cell2 = new PdfPCell(new Paragraph("Relatório Detalhado - " + aten.PROD_NM_NOME, meuFont2))
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

                Document pdfDoc = new Document(PageSize.A4, 10, 10, 60, 40);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfWriter.PageEvent = new CustomPageEventHelper(headerTable, footerTable);
                pdfDoc.Open();

                // Dados Gerais
                PdfPTable table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (System.IO.File.Exists(Server.MapPath(aten.PROD_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.Colspan = 2;
                    Image image = Image.GetInstance(Server.MapPath(aten.PROD_AQ_FOTO));
                    image.ScaleAbsolute(150, 150);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("", meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(" ", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell();

                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Nome: " + aten.PROD_NM_NOME, meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Tipo: " + (aten.PROD_IN_TIPO_PRODUTO == 1 ? "Material" : "Produto"), meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Subcategoria: " + aten.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Código (SKU): " + aten.PROD_CD_CODIGO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Código Barras: " + aten.PROD_NR_BARCODE, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Última Alteração: " + aten.PROD_DT_ALTERACAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Marca: " + aten.PROD_NM_MARCA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Modelo: " + aten.PROD_NM_MODELO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Fabricante: " + aten.PROD_NM_FABRICANTE, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Referência: " + aten.PROD_NR_REFERENCIA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.UNIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph("Unidade: " + aten.UNIDADE.UNID_NM_NOME, meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Unidade: -", meuFont));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }


                cell = new PdfPCell(new Paragraph("Descrição: " + aten.PROD_DS_DESCRICAO, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Informações do Produto: " + aten.PROD_DS_INFORMACOES, meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Precos
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                if (aten.PROD_IN_TIPO_PRODUTO == 2)
                {

                    cell = new PdfPCell(new Paragraph("Preços", meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 5;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph("Venda (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_PRECO_VENDA.Value), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Preço Anterior (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_PRECO_ANTERIOR.Value), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 1;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Locação (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_LOCACAO.Value), meuFont));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);
                }

                cell = new PdfPCell(new Paragraph(" ", meuFont));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Custos", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Último Custo (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_ULTIMO_CUSTO.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estimativa de Venda/Mês: " + CrossCutting.Formatters.DecimalFormatter(aten.PROD_VL_MEDIA_VENDA_MENSAL.Value), meuFont));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Estoque
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Estoque", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Estoque Atual: " + aten.PROD_VL_ESTOQUE_ATUAL, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque Máximo: " + aten.PROD_VL_ESTOQUE_MAXIMO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque Mínimo: " + aten.PROD_VL_ESTOQUE_MINIMO, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Reserva de Estoque: " + aten.PROD_VL_ESTOQUE_RESERVA, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                pdfDoc.Add(table);

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Movimentações
                table = new PdfPTable(new float[] { 80f, 80f, 80f, 90f, 120f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Movimentações do Estoque", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.MOVIMENTO_ESTOQUE_PRODUTO.Count > 0)
                {
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
                    cell = new PdfPCell(new Paragraph("Quantidade", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
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

                    foreach (MOVIMENTO_ESTOQUE_PRODUTO item in aten.MOVIMENTO_ESTOQUE_PRODUTO)
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
                        else
                        {
                            cell = new PdfPCell(new Paragraph("Saída", meuFont))
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
                            cell = new PdfPCell(new Paragraph("Devolução de venda", meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                        }
                        else if (item.MOEP_IN_TIPO == 3)
                        {
                            cell = new PdfPCell(new Paragraph("Retorno de manutenção", meuFont))
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
                    }
                    pdfDoc.Add(table);
                }

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Movimentações
                table = new PdfPTable(new float[] { 100f, 80f, 80f, 80f, 80f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Histórico do Estoque", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                if (aten.PRODUTO_ESTOQUE_HISTORICO.Count > 0)
                {
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
                    cell = new PdfPCell(new Paragraph("Movimento", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph("Estoque", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);

                    foreach (PRODUTO_ESTOQUE_HISTORICO item in aten.PRODUTO_ESTOQUE_HISTORICO.OrderBy(p => p.PREH_DT_COMPLETA))
                    {
                        cell = new PdfPCell(new Paragraph(item.PREH_DT_COMPLETA.Value.ToString(), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
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

                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PREH_QN_ESTOQUE.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PREH_QN_ESTOQUE_TOTAL.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    pdfDoc.Add(table);
                }

                // Vendas
                if (aten.PROD_IN_TIPO_PRODUTO == 2)
                {
                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Historico de Vendas
                    table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Histórico de Preços de Venda", meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 4;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    if (aten.PRODUTO_PRECO_VENDA.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph("Data", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Preço de Venda(R$)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Desconto (%)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Preço Promoção (R$)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (PRODUTO_PRECO_VENDA item in aten.PRODUTO_PRECO_VENDA)
                        {
                            cell = new PdfPCell(new Paragraph(item.PRPV_DT_PRECO_VENDA.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PRPV_VL_PRECO_VENDA.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(item.PRPV_PC_DESCONTO.Value.ToString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PRPV_VL_PRECO_PROMOCAO.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                        pdfDoc.Add(table);
                    }
                }

                if (aten.PROD_IN_COMPOSTO != 1)
                {
                    // Linha Horizontal
                    line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line1);

                    // Historico de custos
                    table = new PdfPTable(new float[] { 120f, 120f});
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 1f;
                    table.SpacingAfter = 1f;

                    cell = new PdfPCell(new Paragraph("Histórico de Preços de Custo", meuFontBold));
                    cell.Border = 0;
                    cell.Colspan = 2;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    if (aten.PRODUTO_CUSTO.Count > 0)
                    {
                        cell = new PdfPCell(new Paragraph("Data", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Preço de Custo (R$)", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cell);

                        foreach (PRODUTO_CUSTO item in aten.PRODUTO_CUSTO)
                        {
                            cell = new PdfPCell(new Paragraph(item.PRCU_DT_CUSTO.Value.ToShortDateString(), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PRCU_VL_CUSTO.Value), meuFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(cell);
                        }
                        pdfDoc.Add(table);
                    }
                }

                // Linha Horizontal
                line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line1);

                // Observações
                Chunk chunk1 = new Chunk("Observações: " + aten.PROD_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
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
                return RedirectToAction("VoltarAnexoProduto");
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
                conf = conf.Where(p => p.USUA_IN_SISTEMA == 6).ToList();
                Session["UsuarioAlterada"] = 0;
                Session["Usuarios"] = conf;
                return conf;
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
                return null;
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
                conf = conf.Where(p => p.PROD_IN_SISTEMA == 6).ToList();
                Session["ProdutoAlterada"] = 0;
                Session["Produtos"] = conf;
                return conf;
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
                return null;
            }
        }

        public List<PRODUTO> CarregarProdutoUltimas(Int32 linhas)
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PRODUTO> conf = new List<PRODUTO>();
                if (Session["ProdutosUltimas"] == null)
                {
                    conf = prodApp.GetAllItensUltimas(idAss, linhas);
                }
                else
                {
                    if ((Int32)Session["ProdutoAlterada"] == 1)
                    {
                        conf = prodApp.GetAllItensUltimas(idAss, linhas);
                    }
                    else
                    {
                        conf = (List<PRODUTO>)Session["ProdutosUltimas"];
                    }
                }
                conf = conf.Where(p => p.PROD_IN_SISTEMA == 6).ToList();
                Session["ProdutoAlterada"] = 0;
                Session["ProdutosUltimas"] = conf;
                return conf;
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
                return null;
            }
        }

        public List<CATEGORIA_PRODUTO>  CarregarCatProduto()
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
                conf = conf.Where(p => p.CAPR_IN_SISTEMA == 6).ToList();
                Session["CatProdutoAlterada"] = 0;
                Session["CatProdutos"] = conf;
                return conf;
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
                conf = conf.Where(p => p.SCPR_IN_SISTEMA == 6).ToList();
                Session["SubCatProdutoAlterada"] = 0;
                Session["SubCatProdutos"] = conf;
                return conf;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "CRMSys", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
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
                conf = conf.Where(p => p.UNID_IN_TIPO_UNIDADE == 6).ToList();
                Session["UnidadeAlterada"] = 0;
                Session["Unidades"] = conf;
                return conf;
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
                return null;
            }
        }

        public List<String> CarregarTipoProduto()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<String> conf = new List<String>();
            conf.Add("Material");
            conf.Add("Produto");
            return conf;
        }

        public ActionResult IncluirAnotacaoProduto()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                PRODUTO item = prodApp.GetItemById((Int32)Session["IdProduto"]);
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO_ANOTACAO coment = new PRODUTO_ANOTACAO();
                ProdutoAnotacaoViewModel vm = Mapper.Map<PRODUTO_ANOTACAO, ProdutoAnotacaoViewModel>(coment);
                vm.PRAT_DT_ANOTACAO = DateTime.Now;
                vm.PRAT_IN_ATIVO = 1;
                vm.ASSI_CD_ID = item.ASSI_CD_ID;
                vm.USUARIO = usuarioLogado;
                vm.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirAnotacaoProduto(ProdutoAnotacaoViewModel vm)
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
                    PRODUTO_ANOTACAO item = Mapper.Map<ProdutoAnotacaoViewModel, PRODUTO_ANOTACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO not = prodApp.GetItemById((Int32)Session["IdProduto"]);

                    item.USUARIO = null;
                    not.PRODUTO_ANOTACAO.Add(item);
                    objetoProdAntes = not;
                    Int32 volta = prodApp.ValidateEdit(not, objetoProdAntes);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Anotação - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Produto: " + not.PROD_NM_NOME + " | Anotação: " + item.PRAT_DS_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A anotação no produto " + not.PROD_NM_NOME.ToUpper() + " foi incluída com sucesso.";
                    Session["MensProduto"] = 61;

                    // Sucesso
                    Session["AbaProduto"] = 4;
                    return RedirectToAction("EditarProduto", new { id = (Int32)Session["IdProduto"] });
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
        public ActionResult EditarProdutoCusto(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Prepara view
                PRODUTO_CUSTO item = prodApp.GetCustoById(id);
                ProdutoCustoViewModel vm = Mapper.Map<PRODUTO_CUSTO, ProdutoCustoViewModel>(item);
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoCusto(ProdutoCustoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO_CUSTO item = Mapper.Map<ProdutoCustoViewModel, PRODUTO_CUSTO>(vm);
                    Int32 volta = prodApp.ValidateEditCusto(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epcPROD",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_CUSTO>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PrecoCustoAlterado"] = 1;
                    Session["AbaProduto"] = 2;
                    Session["RecuperaEstado"] = 1;
                    Session["FlagProduto"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoCusto(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera custo
                Session["MensProduto"] = 0;
                PRODUTO prod = (PRODUTO)Session["Produto"];
                PRODUTO_CUSTO item = prodApp.GetCustoById(id);
                item.PRCU_IN_ATIVO = 0;

                // Verifica possibilidade
                if (prod.PRODUTO_CUSTO.Count == 1)
                {
                    Session["MensProduto"] = 34;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Executa
                Int32 volta = prodApp.ValidateEditCusto(item);

                // Acerta produto
                List<PRODUTO_CUSTO> lista = prod.PRODUTO_CUSTO.OrderByDescending(p => p.PRCU_DT_CUSTO).ToList();
                prod.PROD_VL_ULTIMO_CUSTO = lista.FirstOrDefault().PRCU_VL_CUSTO.Value;
                volta = prodApp.ValidateEdit(prod, prod);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xpcPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_CUSTO>(item),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["AbaProduto"] = 2;
                Session["RecuperaEstado"] = 1;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult ReativarProdutoCusto(Int32 id)
        {
            try
            {
                // Recupera custo
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                Session["MensProduto"] = 0;
                PRODUTO prod = (PRODUTO)Session["Produto"];
                PRODUTO_CUSTO item = prodApp.GetCustoById(id);
                item.PRCU_IN_ATIVO = 1;

                // Verifica possibilidade
                if (prodApp.CheckExistCusto(item, idAss) != null)
                {
                    Session["MensProduto"] = 35;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Executa
                Int32 volta = prodApp.ValidateEditCusto(item);

                // Acerta produto
                List<PRODUTO_CUSTO> lista = prod.PRODUTO_CUSTO.OrderByDescending(p => p.PRCU_DT_CUSTO).ToList();
                prod.PROD_VL_ULTIMO_CUSTO = lista.FirstOrDefault().PRCU_VL_CUSTO.Value;
                volta = prodApp.ValidateEdit(prod, prod);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rpcPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_CUSTO>(item),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                Session["RecuperaEstado"] = 1;
                Session["FlagProduto"] = 1;

                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult IncluirProdutoCusto()
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                PRODUTO prod = (PRODUTO)Session["Produto"];
                ViewBag.Nome = prod.PROD_NM_NOME;
                ViewBag.Codigo = prod.PROD_CD_CODIGO;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_6.pdf";

                PRODUTO_CUSTO item = new PRODUTO_CUSTO();
                ProdutoCustoViewModel vm = Mapper.Map<PRODUTO_CUSTO, ProdutoCustoViewModel>(item);
                vm.PROD_CD_ID = (Int32)Session["IdProduto"];
                vm.PRCU_IN_ATIVO = 1;
                vm.PRCU_DT_CUSTO = DateTime.Today.Date;
                vm.ASSI_CD_ID = idAss;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoCusto(ProdutoCustoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Executa a operação
                    PRODUTO_CUSTO item = Mapper.Map<ProdutoCustoViewModel, PRODUTO_CUSTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateCreateCusto(item, idAss);

                    // Verifica volta
                    Session["MensProduto"] = 0;
                    if (volta == 1)
                    {
                        Session["MensProduto"] = 33;
                        return RedirectToAction("VoltarAnexoProduto");
                    }

                    // Acerta produto
                    PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                    prod.PROD_VL_ULTIMO_CUSTO = item.PRCU_VL_CUSTO;
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Produto_Custo dto = MontarProdutoCustoDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Preço de Custo - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 voltaM = logApp.ValidateCreate(log);

                    // Grava Log de produto
                    PRODUTO_LOG logProd = new PRODUTO_LOG();
                    logProd.PROD_CD_ID = item.PROD_CD_ID;
                    logProd.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    logProd.LOG_CD_ID = log.LOG_CD_ID;
                    logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                    logProd.PRLG_DS_OPERACAO = "Inclusão de Custo";
                    Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O produto " + prod.PROD_NM_NOME.ToUpper() + " teve um novo preço de custo incluído com sucesso.";
                    Session["MensProduto"] = 61;

                    // Encerra
                    Session["PrecoCustoAlterado"] = 1;
                    Session["AbaProduto"] = 2;
                    Session["RecuperaEstado"] = 1;
                    Session["FlagProduto"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult EditarProdutoPrecoVenda(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Monta listas
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                PRODUTO_PRECO_VENDA item = prodApp.GetPrecoVendaById(id);
                ProdutoPrecoVendaViewModel vm = Mapper.Map<PRODUTO_PRECO_VENDA, ProdutoPrecoVendaViewModel>(item);
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProdutoPrecoVenda(ProdutoPrecoVendaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    PRODUTO_PRECO_VENDA item = Mapper.Map<ProdutoPrecoVendaViewModel, PRODUTO_PRECO_VENDA>(vm);
                    Int32 volta = prodApp.ValidateEditPrecoVenda(item, (Int32)Session["IdProduto"]);

                    // Acerta produto
                    prod.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_VENDA;
                    prod.PROD_VL_PRECO_VENDA = item.PRPV_VL_PRECO_VENDA.Value;
                    prod.PROD_VL_PRECO_PROMOCAO = item.PRPV_VL_PRECO_PROMOCAO.Value;
                    prod.PROD_PC_DESCONTO = item.PRPV_PC_DESCONTO.Value;
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "epvPROD",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_PRECO_VENDA>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta4 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["PrecoCustoAlterado"] = 1;
                    Session["AbaProduto"] = 2;
                    Session["RecuperaEstado"] = 1;
                    Session["FlagProduto"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirProdutoPrecoVenda(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                // Recupera preço
                Session["MensProduto"] = 0;
                PRODUTO prod = (PRODUTO)Session["Produto"];
                PRODUTO_PRECO_VENDA item = prodApp.GetPrecoVendaById(id);
                item.PRPV_IN_ATIVO = 0;

                // Verifica possibilidade
                if (prod.PRODUTO_PRECO_VENDA.Count == 1)
                {
                    Session["MensProduto"] = 36;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Executa
                Int32 volta = prodApp.ValidateEditPrecoVenda(item, (Int32)Session["IdProduto"]);

                // Acerta produto
                prod = prodApp.GetItemById(prod.PROD_CD_ID);
                List<PRODUTO_PRECO_VENDA> lista = prod.PRODUTO_PRECO_VENDA.OrderByDescending(p => p.PRPV_DT_PRECO_VENDA).ToList();
                PRODUTO_PRECO_VENDA prec = lista.FirstOrDefault();

                prod.PROD_VL_PRECO_VENDA = prec.PRPV_VL_PRECO_VENDA.Value;
                prod.PROD_VL_PRECO_PROMOCAO = prec.PRPV_VL_PRECO_PROMOCAO.Value;
                prod.PROD_PC_DESCONTO = prec.PRPV_PC_DESCONTO.Value;
                prod.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_VENDA;
                prod.TIEM_CD_ID = prec.TIEM_CD_ID.Value;
                prod.PROD_VL_PRECO_MINIMO = prec.PRPV_VL_PRECO_EMBALAGEM.Value;
                volta = prodApp.ValidateEdit(prod, prod);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "xpvPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_PRECO_VENDA>(item),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);

                Session["AbaProduto"] = 2;
                Session["RecuperaEstado"] = 1;

                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult ReativarProdutoPrecoVenda(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];

                // Recupera custo
                Session["MensProduto"] = 0;
                PRODUTO prod = (PRODUTO)Session["Produto"];
                PRODUTO_PRECO_VENDA item = prodApp.GetPrecoVendaById(id);
                item.PRPV_IN_ATIVO = 1;

                // Verifica possibilidade
                if (prodApp.CheckExistVenda(item, idAss) != null)
                {
                    Session["MensProduto"] = 37;
                    return RedirectToAction("VoltarAnexoProduto");
                }

                // Executa
                Int32 volta = prodApp.ValidateEditPrecoVenda(item, (Int32)Session["IdProduto"]);

                // Acerta produto
                List<PRODUTO_PRECO_VENDA> lista = prod.PRODUTO_PRECO_VENDA.OrderByDescending(p => p.PRPV_DT_PRECO_VENDA).ToList();
                PRODUTO_PRECO_VENDA prec = lista.FirstOrDefault();

                prod.PROD_VL_PRECO_VENDA = prec.PRPV_VL_PRECO_VENDA.Value;
                prod.PROD_VL_PRECO_PROMOCAO = prec.PRPV_VL_PRECO_PROMOCAO.Value;
                prod.PROD_PC_DESCONTO = prec.PRPV_PC_DESCONTO.Value;
                prod.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_VENDA;
                prod.TIEM_CD_ID = prec.TIEM_CD_ID.Value;
                prod.PROD_VL_PRECO_MINIMO = prec.PRPV_VL_PRECO_EMBALAGEM.Value;
                volta = prodApp.ValidateEdit(prod, prod);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rpvPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_PRECO_VENDA>(item),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);

                Session["RecuperaEstado"] = 1;
                Session["FlagProduto"] = 1;

                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult IncluirProdutoPrecoVenda()
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Monta lista
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_5.pdf";

                // Prepara view
                PRODUTO prod = (PRODUTO)Session["Produto"];
                ViewBag.Nome = prod.PROD_NM_NOME;
                ViewBag.Codigo = prod.PROD_CD_CODIGO;

                PRODUTO_PRECO_VENDA item = new PRODUTO_PRECO_VENDA();
                ProdutoPrecoVendaViewModel vm = Mapper.Map<PRODUTO_PRECO_VENDA, ProdutoPrecoVendaViewModel>(item);
                vm.PROD_CD_ID = (Int32)Session["IdProduto"];
                vm.PRPV_IN_ATIVO = 1;
                vm.PRPV_DT_PRECO_VENDA = DateTime.Today.Date;
                vm.PRPV_VL_PRECO_EMBALAGEM = 0;
                vm.PRPV_PC_DESCONTO = 0;
                vm.PRPV_VL_PRECO_PROMOCAO = 0;
                vm.PRPV_VL_PRECO_VENDA = 0;
                vm.ASSI_CD_ID = idAss;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirProdutoPrecoVenda(ProdutoPrecoVendaViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Executa a operação
                    PRODUTO prod = prodApp.GetItemById(vm.PROD_CD_ID);
                    PRODUTO_PRECO_VENDA item = Mapper.Map<ProdutoPrecoVendaViewModel, PRODUTO_PRECO_VENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    if (prod.TIEM_CD_ID != null)
                    {
                        item.TIEM_CD_ID = prod.TIEM_CD_ID;
                    }
                    Int32 volta = prodApp.ValidateCreatePrecoVenda(item, (Int32)Session["IdProduto"], idAss);

                    // Verifica volta
                    Session["MensProduto"] = 0;
                    if (volta == 1)
                    {
                        Session["MensProduto"] = 38;
                        return RedirectToAction("VoltarAnexoProduto");
                    }

                    // Acerta produto
                    prod.PROD_VL_PRECO_ANTERIOR = prod.PROD_VL_PRECO_VENDA;
                    prod.PROD_VL_PRECO_VENDA = item.PRPV_VL_PRECO_VENDA.Value;
                    prod.PROD_VL_PRECO_PROMOCAO = item.PRPV_VL_PRECO_PROMOCAO.Value;
                    prod.PROD_PC_DESCONTO = item.PRPV_PC_DESCONTO.Value;
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Produto_Venda dto = MontarProdutoVendaDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Preço de Venda - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 voltaM = logApp.ValidateCreate(log);

                    // Grava Log de produto
                    PRODUTO_LOG logProd = new PRODUTO_LOG();
                    logProd.PROD_CD_ID = item.PROD_CD_ID;
                    logProd.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    logProd.LOG_CD_ID = log.LOG_CD_ID;
                    logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                    logProd.PRLG_DS_OPERACAO = "Inclusão de Preço de Venda";
                    Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O produto " + prod.PROD_NM_NOME.ToUpper() + " teve um novo preço de venda incluído com sucesso.";
                    Session["MensProduto"] = 61;

                    // Encerra
                    Session["PrecoCustoAlterado"] = 1;
                    Session["AbaProduto"] = 2;
                    Session["FlagProduto"] = 1;
                    Session["RecuperaEstado"] = 1;

                    return RedirectToAction("VoltarAnexoProduto");
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

        public JsonResult GetDadosCusto()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaCusto"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valoresCusto", valor1);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosEstoque()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaEstoque"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("datas", dias);
                result.Add("quantEstoque", valor1);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosEstoqueTotal()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaEstoqueTotal"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                }

                Hashtable result = new Hashtable();
                result.Add("datas", dias);
                result.Add("quantEstoque", valor1);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosPreco()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPreco"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                    valor2.Add(item.ValorDec2);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valoresVenda", valor1);
                result.Add("valoresPromocao", valor2);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosMedia()
        {
            try
            {
                List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaConc"];
                List<String> dias = new List<String>();
                List<Decimal> valor1 = new List<Decimal>();
                List<Decimal> valor2 = new List<Decimal>();
                dias.Add(" ");
                valor1.Add(0);
                valor2.Add(0);

                foreach (ModeloViewModel item in listaCP1)
                {
                    dias.Add(item.DataEmissao.ToShortDateString());
                    valor1.Add(item.ValorDec1);
                    valor2.Add(item.ValorDec2);
                }

                Hashtable result = new Hashtable();
                result.Add("dias", dias);
                result.Add("valoresVenda", valor1);
                result.Add("media", valor2);
                return Json(result);
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
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditarPrecoConcorrente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Prepara view
                PRODUTO_CONCORRENTE item = prodApp.GetConcorrenteById(id);
                ProdutoConcorrenteViewModel vm = Mapper.Map<PRODUTO_CONCORRENTE, ProdutoConcorrenteViewModel>(item);
                Session["Concorrente"] = item;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPrecoConcorrente(ProdutoConcorrenteViewModel vm)
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
                    PRODUTO_CONCORRENTE item = Mapper.Map<ProdutoConcorrenteViewModel, PRODUTO_CONCORRENTE>(vm);
                    Int32 volta = prodApp.ValidateEditConcorrente(item);

                    // Atualiza Produto
                    PRODUTO prod = prodApp.GetItemById((Int32)Session["IdProduto"]);
                    List<PRODUTO_CONCORRENTE> precs = prod.PRODUTO_CONCORRENTE.Where(p => p.PRPF_IN_ATIVO == 1).ToList();
                    Decimal? media = precs.Sum(p => p.PRPF_VL_PRECO_CONCORRENTE) / precs.Count;
                    prod.PROD_VL_CUSTO_CONCORRENTE_MEDIO = media;
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Produto_Concorrente dto = MontarProdutoConcorrenteDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    DTO_Produto_Concorrente dtoAntes = MontarProdutoConcorrenteDTOObj((PRODUTO_CONCORRENTE)Session["Concorrente"]);
                    String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Preço de Concorrente - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 voltaM = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O produto " + prod.PROD_NM_NOME.ToUpper() + " teve um preço de concorrente alterado com sucesso.";
                    Session["MensProduto"] = 61;

                    // Verifica retorno
                    Session["AbaProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirPrecoConcorrente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO not = prodApp.GetItemById((Int32)Session["IdProduto"]);
                PRODUTO_CONCORRENTE item = prodApp.GetConcorrenteById(id);
                item.PRPF_IN_ATIVO = 0;
                Int32 volta = prodApp.ValidateEditConcorrente(item);

                // Configura serialização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Produto_Concorrente dto = MontarProdutoConcorrenteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Produto - Preço de Concorrente - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };
                Int32 voltaM = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O produto " + not.PROD_NM_NOME.ToUpper() + " teve um preço de concorrente excluído com sucesso.";
                Session["MensProduto"] = 61;

                Session["AbaProduto"] = 6;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult ReativarPrecoConcorrente(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO_CONCORRENTE item = prodApp.GetConcorrenteById(id);
                item.PRPF_IN_ATIVO = 1;
                Int32 volta = prodApp.ValidateEditConcorrente(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "rptPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_CONCORRENTE>(item),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta4 = logApp.ValidateCreate(log);

                Session["AbaProduto"] = 6;
                Session["RecuperaEstado"] = 1;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult IncluirPrecoConcorrente()
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Prepara view
                PRODUTO prod = (PRODUTO)Session["Produto"];
                ViewBag.Nome = prod.PROD_NM_NOME;
                ViewBag.Codigo = prod.PROD_CD_CODIGO;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_9.pdf";

                PRODUTO_CONCORRENTE item = new PRODUTO_CONCORRENTE();
                ProdutoConcorrenteViewModel vm = Mapper.Map<PRODUTO_CONCORRENTE, ProdutoConcorrenteViewModel>(item);
                vm.PROD_CD_ID = (Int32)Session["IdProduto"];
                vm.PRPF_IN_ATIVO = 1;
                vm.PRPF_DT_CADASTRO = DateTime.Today.Date;
                vm.PRPF_VL_PRECO_CONCORRENTE = 0;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirPrecoConcorrente(ProdutoConcorrenteViewModel vm)
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
                    PRODUTO_CONCORRENTE item = Mapper.Map<ProdutoConcorrenteViewModel, PRODUTO_CONCORRENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = prodApp.ValidateCreateConcorrente(item);

                    // Atualiza Produto
                    PRODUTO prod = prodApp.GetItemById((Int32)Session["IdProduto"]);
                    List<PRODUTO_CONCORRENTE> precs = prod.PRODUTO_CONCORRENTE.Where(p => p.PRPF_IN_ATIVO == 1).ToList();
                    Decimal? media = precs.Sum(p => p.PRPF_VL_PRECO_CONCORRENTE) / precs.Count;
                    prod.PROD_VL_CUSTO_CONCORRENTE_MEDIO = media;
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Configura serialização
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    // Monta Log
                    DTO_Produto_Concorrente dto = MontarProdutoConcorrenteDTOObj(item);
                    String json = JsonConvert.SerializeObject(dto, settings);
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Preço de Concorrente - Inclusão",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 voltaM = logApp.ValidateCreate(log);

                    // Grava Log de produto
                    PRODUTO_LOG logProd = new PRODUTO_LOG();
                    logProd.PROD_CD_ID = item.PROD_CD_ID;
                    logProd.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    logProd.LOG_CD_ID = log.LOG_CD_ID;
                    logProd.PRLG_DT_MOVIMENTO = DateTime.Now;
                    logProd.PRLG_DS_OPERACAO = "Inclusão de Preço de Concorrente";
                    Int32 volta5 = prodApp.ValidateCreateLog(logProd);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "O produto " + prod.PROD_NM_NOME.ToUpper() + " teve um novo preço de concorrente incluído com sucesso.";
                    Session["MensProduto"] = 61;

                    // Verifica retorno
                    Session["AbaProduto"] = 6;
                    return RedirectToAction("VoltarAnexoProduto");
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
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditarAnotacaoProduto(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Prepara view
                Session["AbaProduto"] = 4;
                PRODUTO_ANOTACAO item = prodApp.GetAnotacaoById(id);
                PRODUTO cli = prodApp.GetItemById(item.PROD_CD_ID);
                ProdutoAnotacaoViewModel vm = Mapper.Map<PRODUTO_ANOTACAO, ProdutoAnotacaoViewModel>(item);
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarAnotacaoProduto(ProdutoAnotacaoViewModel vm)
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
                    PRODUTO not = prodApp.GetItemById((Int32)Session["IdProduto"]);
                    PRODUTO_ANOTACAO item = Mapper.Map<ProdutoAnotacaoViewModel, PRODUTO_ANOTACAO>(vm);
                    Int32 volta = prodApp.ValidateEditAnotacao(item);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "Produto - Anotação - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = "Produto: " + not.PROD_NM_NOME.ToUpper() + " | Anotação: " + item.PRAT_DS_ANOTACAO,
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta1 = logApp.ValidateCreate(log);

                    // Mensagem do CRUD
                    Session["MsgCRUD"] = "A anotação no produto " + not.PROD_NM_NOME.ToUpper() + " foi alterada com sucesso.";
                    Session["MensProduto"] = 61;

                    // Verifica retorno
                    Session["ProdutoAlterada"] = 1;
                    Session["AbaProduto"] = 4;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult ExcluirAnotacaoProduto(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO not = prodApp.GetItemById((Int32)Session["IdProduto"]);

                Session["AbaProduto"] = 4;
                PRODUTO_ANOTACAO item = prodApp.GetAnotacaoById(id);
                item.PRAT_IN_ATIVO = 0;
                Int32 volta = prodApp.ValidateEditAnotacao(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Produto - Anotação - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Produto: " + not.PROD_NM_NOME.ToUpper() + " | Anotação: " + item.PRAT_DS_ANOTACAO,
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "A anotação no produto " + not.PROD_NM_NOME.ToUpper() + " foi excluída com sucesso.";
                Session["MensProduto"] = 61;

                Session["ProdutoAlterada"] = 1;
                Session["AbaProduto"] = 4;
                return RedirectToAction("VoltarAnexoProduto");
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

        public JsonResult GetDadosProdutoCatLista()
        {
            try
            {
                List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoCats"];
                List<String> cat = new List<String>();
                List<Int32> valor = new List<Int32>();
                cat.Add(" ");
                valor.Add(0);

                foreach (ModeloViewModel item in lista)
                {
                    cat.Add(item.Nome);
                    valor.Add(item.Valor);
                }

                Hashtable result = new Hashtable();
                result.Add("cats", cat);
                result.Add("valores", valor);
                return Json(result);
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
                return null;
            }
        }

        public JsonResult GetDadosProdutoTipoLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoTipo"];
            List<String> tipo = new List<String>();
            List<Int32> valor = new List<Int32>();
            tipo.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                tipo.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("tipos", tipo);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosProdutoEspecieLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoEspecie"];
            List<String> esp = new List<String>();
            List<Int32> valor = new List<Int32>();
            esp.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                esp.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("esps", esp);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosProdutoCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosProdutoTipo()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoTipo"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosProdutoEspecie()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoEspecie"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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

        public JsonResult GetDadosSituacao()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaProdutoAcimaAbaixoGraf"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
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
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MonterTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaEstoque"] == null)
                {
                    listaMasterProd = CarregarProduto().Where(p => p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                    Session["ListaEstoque"] = listaMasterProd;
                }
                List<PRODUTO> prod = (List<PRODUTO>)Session["ListaEstoque"];
                ViewBag.Listas = prod;
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaEstoque"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                Session["PrecoCustoAlterado"] = 0;

                // Recupera Produtos acima do máximo
                List<PRODUTO> acima = prod.Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO).ToList();
                Int32 totAcima = acima.Count;
                ViewBag.ProdutosAcima = totAcima;

                // Recupera Produtos abaixo do minimo
                List<PRODUTO> abaixo = prod.Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO).ToList();
                Int32 totAbaixo = abaixo.Count;
                ViewBag.ProdutosAbaixo = totAbaixo;

                // Recupera Produtos zerados
                List<PRODUTO> zerado = prod.Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0).ToList();
                Int32 totZerado = zerado.Count;
                ViewBag.ProdutosZerado = totZerado;

                // Recupera Produtos esgotando em 30 dias
                List<PRODUTO> esgota = prod.Where(p => (p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1).ToList();
                Int32 totEsgota = esgota.Count;
                ViewBag.ProdutosEsgota = totEsgota;

                // Calcula estoque financeiro
                Decimal? estoqueCusto = 0;
                Decimal? estoqueVenda = 0;
                estoqueCusto = prod.Sum(p => p.PROD_VL_ESTOQUE_CUSTO.Value);
                estoqueVenda = prod.Sum(p => p.PROD_VL_ESTOQUE_VENDA.Value);
                ViewBag.EstoqueCusto = estoqueCusto;
                ViewBag.EstoqueVenda = estoqueVenda;

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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
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
                    if ((Int32)Session["MensProduto"] == 99)
                    {
                        ModelState.AddModelError("", "Foram processados e atualizados " + ((Int32)Session["Conta"]).ToString() + " produtos");
                        ModelState.AddModelError("", "Foram processados e rejeitados " + ((Int32)Session["Falha"]).ToString() + " produtos");
                    }
                    if ((Int32)Session["MensProduto"] == 200)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0316", CultureInfo.CurrentCulture));
                    }
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
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
                Session["ListaEstoque"] = null;
                Session["FiltroEstoaue"] = null;
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
                Session["FiltroEstoaue"] = item;
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
                listaMasterProd = volta.Item2;
                Session["ListaEstoque"] = volta.Item2;

                // Volta
                return RedirectToAction("MontarTelaEstoque");
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

        public ActionResult VoltarBaseEstoque()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaProduto"] = null;
            return RedirectToAction("MontarTelaEstoque");
        }

        public ActionResult GerarRelatorioestoqueMark()
        {
            return RedirectToAction("GerarRelatorioEstoque", new { id = 1 });
        }

        public ActionResult GerarRelatorioEstoque(Int32 id)
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
                        return RedirectToAction("MonterTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Prepara geração
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Int32 idAss = (Int32)Session["IdAssinante"];
                String data = DateTime.Today.Date.ToShortDateString();
                data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
                String nomeRel = String.Empty;
                String titulo = String.Empty;
                List<PRODUTO> lista = new List<PRODUTO>();
                nomeRel = "EstoqueLista" + "_" + data + ".pdf";
                titulo = "Estoque - Listagem";
                lista = (List<PRODUTO>)Session["ListaEstoque"];
                PRODUTO filtro = (PRODUTO)Session["FiltroEstoque"];
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
                table = new PdfPTable(new float[] { 150f, 100f, 100f, 80f, 140f, 140f, 80f, 80f, 80f, 80f, 40f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Produtos selecionados pelos parametros de filtro abaixo", meuFont1))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
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
                cell = new PdfPCell(new Paragraph("Categoria", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Atual", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Máximo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Mínimo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Média Mensal", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Esgotamento (Meses)", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Imagem", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO item in lista)
                {
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
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_ATUAL.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_MAXIMO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_MINIMO.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_MEDIA_VENDA_MENSAL.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.PROD_VL_ESTOQUE_ATUAL.Value / item.PROD_VL_MEDIA_VENDA_MENSAL.Value), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    if (System.IO.File.Exists(Server.MapPath(item.PROD_AQ_FOTO)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.PROD_AQ_FOTO));
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
                    if (filtro.CAPR_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtro.CAPR_CD_ID.ToString();
                        ja = 1;
                    }
                    if (filtro.SCPR_CD_ID > 0)
                    {
                        if (ja == 0)
                        {
                            parametros += "Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                            ja = 1;
                        }
                        else
                        {
                            parametros += "e Subcategoria: " + filtro.SCPR_CD_ID.ToString();
                        }
                    }
                    if (filtro.PROD_CD_CODIGO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Código: " + filtro.PROD_CD_CODIGO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Código: " + filtro.PROD_CD_CODIGO;
                        }
                    }
                    if (filtro.PROD_NM_NOME != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtro.PROD_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: " + filtro.PROD_NM_NOME;
                        }
                    }
                    if (filtro.PROD_NM_MARCA != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Marca: " + filtro.PROD_NM_MARCA;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Marca: " + filtro.PROD_NM_MARCA;
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

                return RedirectToAction("MontarTelaEstoque");
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

        [HttpGet]
        public ActionResult EditarEstoque(Int32 id)
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
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Prepara view
                PRODUTO item = prodApp.GetItemById(id);
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Exibe mensagem
                if ((Int32)Session["MensProduto"] == 30)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0118", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 33)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0287", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 34)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0288", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 35)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0289", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 36)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0290", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 37)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0291", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensProduto"] == 38)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0292", CultureInfo.CurrentCulture));
                }

                // Exibe
                Session["Acerta"] = 0;
                Session["MensProduto"] = 0;
                Session["VoltaConsulta"] = 1;
                objetoProdAntes = item;
                Session["Produto"] = item;
                Session["IdVolta"] = id;
                Session["IdProduto"] = id;
                ProdutoViewModel vm = Mapper.Map<PRODUTO, ProdutoViewModel>(item);
                return View(vm);
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

        [HttpPost]
        public ActionResult EditarEstoque(ProdutoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                    ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO item = Mapper.Map<ProdutoViewModel, PRODUTO>(vm);
                    Int32 volta = prodApp.ValidateEdit(item, objetoProdAntes, usuarioLogado);

                    // Sucesso
                    listaMasterProd = new List<PRODUTO>();
                    Session["ListaEstoque"] = null;
                    Session["ProdutoAlterada"] = 1;
                    Session["FlagProduto"] = 1;

                    // Retornos
                    Session["LinhaAlterada"] = item.PROD_CD_ID;
                    Session["RecuperaEstado"] = 1;
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
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult MontarTelaProdutoAcima()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaProdAcima"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1).ToList();
                    Session["ListaProdAcima"] = listaMasterProd;
                }
                ViewBag.Listas = (List<PRODUTO>)Session["ListaProdAcima"];
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProdAcima"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["PrecoCustoAlterado"] = 0;

                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
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
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
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

        public ActionResult RetirarFiltroProdutoAcima()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaProdAcima"] = null;
            Session["FiltroProduto"] = null;
            return RedirectToAction("MontarTelaProdutoAcima");
        }

        [HttpPost]
        public ActionResult FiltrarProdutoAcima(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaDashboardProduto");
                }

                // Sucesso
                List<PRODUTO> acima = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL > p.PROD_VL_ESTOQUE_MAXIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                Session["MensProduto"] = 0;
                listaMasterProd = acima;
                Session["ListaProdAcima"] = acima;

                // Volta
                return RedirectToAction("MontarTelaProdutoAcima");
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

        [HttpGet]
        public ActionResult MontarTelaProdutoAbaixo()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaProdAbaixo"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.Where(p => p.PROD_VL_ESTOQUE_ATUAL < p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1).ToList();
                    Session["ListaProdAbaixo"] = listaMasterProd;
                }
                ViewBag.Listas = (List<PRODUTO>)Session["ListaProdAbaixo"];
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProdAbaixo"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");
                Session["PrecoCustoAlterado"] = 0;

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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
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
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
                return View(objetoProd);
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

        public ActionResult RetirarFiltroProdutoAbaixo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaProdAbaixo"] = null;
            Session["FiltroProduto"] = null;
            return RedirectToAction("MontarTelaProdutoAbaixo");
        }

        [HttpPost]
        public ActionResult FiltrarProdutoAbaixo(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaDashboardProduto");
                }

                // Sucesso
                List<PRODUTO> acima = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL <= p.PROD_VL_ESTOQUE_MINIMO & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                Session["MensProduto"] = 0;
                listaMasterProd = acima;
                Session["ListaProdAbaixo"] = acima;

                // Volta
                return RedirectToAction("MontarTelaProdutoAbaixo");
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

        [HttpGet]
        public ActionResult MontarTelaProdutoEsgota()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaProdEsgota"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.Where(p => p.PROD_VL_MEDIA_VENDA_MENSAL > 0).ToList();
                    listaMasterProd = listaMasterProd.Where(p => (p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1  & p.PROD_IN_ATIVO == 1).ToList();
                    Session["ListaProdEsgota"] = listaMasterProd;
                }
                ViewBag.Listas = (List<PRODUTO>)Session["ListaProdEsgota"];
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProdEsgota"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["PrecoCustoAlterado"] = 0;
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
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
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
                return View(objetoProd);
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

        public ActionResult RetirarFiltroProdutoEsgota()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaProdEsgota"] = null;
            Session["FiltroProduto"] = null;
            return RedirectToAction("MontarTelaProdutoEsgota");
        }

        [HttpPost]
        public ActionResult FiltrarProdutoEsgota(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaDashboardProduto");
                }

                // Sucesso
                List<PRODUTO> acima = (volta.Item2).Where(p => (p.PROD_VL_ESTOQUE_ATUAL / p.PROD_VL_MEDIA_VENDA_MENSAL) <= 1  & p.PROD_IN_ATIVO == 1  & p.PROD_IN_COMPOSTO == 0).ToList();
                Session["MensProduto"] = 0;
                listaMasterProd = acima;
                Session["ListaProdEsgota"] = acima;

                // Volta
                return RedirectToAction("MontarTelaProdutoEsgota");
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

        [HttpGet]
        public ActionResult VerLog(Int32 id)
        {
            try
            {
                // Verifica se tem usuario logado
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Recupera e exibe
                Session["AbaProduto"] = 12;
                return RedirectToAction("VerLog", "Auditoria", new { id = id });
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

        [HttpGet]
        public ActionResult ExcluirAnexo(Int32 id)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                PRODUTO_ANEXO item = prodApp.GetAnexoById(id);
                PRODUTO pro = prodApp.GetItemById(item.PROD_CD_ID);
                item.PRAN_IN_ATIVO = 0;
                Int32 volta = prodApp.ValidateEditAnexo(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                    USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Produto - Anexo - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = "Produto: " + pro.PROD_NM_NOME.ToUpper() + " | Anexo: " + item.PRAN_NM_TITULO.ToUpper() + " | Data: " + item.PRAN_DT_ANEXO.ToShortDateString(),
                    LOG_IN_SISTEMA = 6
                };
                Int32 volta1 = logApp.ValidateCreate(log);

                // Mensagem do CRUD
                Session["MsgCRUD"] = "O anexo " + item.PRAN_NM_TITULO.ToUpper() + " do produto " + pro.PROD_NM_NOME.ToUpper() + " foi excluído com sucesso.";
                Session["MensProduto"] = 61;

                Session["AbaProduto"] = 5;
                Session["ProdutoAlterada"] = 1;
                return RedirectToAction("VoltarAnexoProduto");
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

        [HttpGet]
        public ActionResult MontarTelaProdutoZerado()
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
                    if (usuario.PERFIL.PERF_IN_ACESSO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                if (Session["ListaProdZerado"] == null)
                {
                    listaMasterProd = CarregarProduto();
                    listaMasterProd = listaMasterProd.Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1).ToList();
                    Session["ListaProdZerado"] = listaMasterProd;
                }
                ViewBag.Listas = (List<PRODUTO>)Session["ListaProdZerado"];
                ViewBag.Cats = new SelectList(CarregarCatProduto().Where(p => p.CAPR_IN_TIPO == 1).OrderBy(x => x.CAPR_NM_NOME).ToList<CATEGORIA_PRODUTO>(), "CAPR_CD_ID", "CAPR_NM_NOME");
                ViewBag.Subs = new SelectList(CarregarSubCatProduto().Where(p => p.SCPR_IN_TIPO == 1).OrderBy(x => x.SCPR_NM_NOME).ToList<SUBCATEGORIA_PRODUTO>(), "SCPR_CD_ID", "SCPR_NM_NOME");
                ViewBag.Unidades = new SelectList(CarregarUnidade().Where(x => x.UNID_IN_TIPO_UNIDADE == 1).OrderBy(p => p.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
                ViewBag.Produtos = ((List<PRODUTO>)Session["ListaProdZerado"]).Count;
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
                Session["PrecoCustoAlterado"] = 0;
                List<SelectListItem> tipo = new List<SelectListItem>();
                tipo.Add(new SelectListItem() { Text = "Material", Value = "1" });
                tipo.Add(new SelectListItem() { Text = "Produto", Value = "2" });
                ViewBag.Tipos = new SelectList(tipo, "Value", "Text");

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
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0182", CultureInfo.CurrentCulture));
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
                }

                // Abre view
                objetoProd = new PRODUTO();
                objetoProd.PROD_IN_ATIVO = 1;
                Session["VoltaProduto"] = 1;
                Session["VoltaConsulta"] = 1;
                Session["FlagVoltaProd"] = 1;
                Session["MensProduto"] = 0;
                Session["Clonar"] = 0;
                Session["Acerta"] = 0;
                Session["AbaProduto"] = 1;
                return View(objetoProd);
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

        public ActionResult RetirarFiltroProdutoZerado()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaProdZerado"] = null;
            Session["FiltroProduto"] = null;
            return RedirectToAction("MontarTelaProdutoZerado");
        }

        [HttpPost]
        public ActionResult FiltrarProdutoZerado(PRODUTO item)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Executa a operação
                List<PRODUTO> listaObj = new List<PRODUTO>();
                Session["FiltroProduto"] = item;
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 idAss = (Int32)Session["IdAssinante"];
                Tuple<Int32, List<PRODUTO>, Boolean> volta = prodApp.ExecuteFilterTuple(item.CAPR_CD_ID, item.SCPR_CD_ID, item.PROD_NM_NOME, item.PROD_NM_MARCA, item.PROD_CD_CODIGO, item.PROD_IN_TIPO_PRODUTO, item.PROD_IN_COMPOSTO, item.PROD_DT_ALTERACAO, idAss);

                // Verifica retorno
                if (volta.Item1 == 1)
                {
                    Session["MensProduto"] = 1;
                    return RedirectToAction("MontarTelaProdutoZerado");
                }

                // Sucesso
                List<PRODUTO> acima = (volta.Item2).Where(p => p.PROD_VL_ESTOQUE_ATUAL <= 0 & p.PROD_IN_ATIVO == 1 & p.PROD_IN_COMPOSTO == 0).ToList();
                Session["MensProduto"] = 0;
                listaMasterProd = acima;
                Session["ListaProdZerado"] = acima;

                // Volta
                return RedirectToAction("MontarTelaProdutoZerado");
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

        public ActionResult GerarRelatorioListaAcima()
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
                nomeRel = "ProdutoListaAcima" + "_" + data + ".pdf";
                titulo = "Materiais/Produtos - Acima do Estoque Máximo";
                lista = (List<PRODUTO>)Session["ListaProdAcima"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                PdfPTable table = new PdfPTable(new float[] { 180f, 60f, 60f, 100f, 100f, 50f, 80f, 80f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Atual", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Máximo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Diferença (%)", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    Decimal? dif = (item.PROD_VL_ESTOQUE_ATUAL * 100) / item.PROD_VL_ESTOQUE_MAXIMO;
                    if (dif > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(dif.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

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
                Session["AbaDash"] = 2;
                return RedirectToAction("MontarTelaEstoque");
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

    
        public ActionResult GerarRelatorioListaAbaixo()
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
                nomeRel = "ProdutoListaAbaixo" + "_" + data + ".pdf";
                titulo = "Materiais/Produtos - Abaixo do Estoque Mínimo";
                //lista = (List<ModeloViewModel>)Session["ListaProdutoAbaixo"];
                lista = (List<PRODUTO>)Session["ListaProdAbaixo"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                PdfPTable table = new PdfPTable(new float[] { 180f, 60f, 60f, 100f, 100f, 50f, 80f, 80f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Atual", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Mínimo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Diferença (%)", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    Decimal? dif = (item.PROD_VL_ESTOQUE_ATUAL * 100) / item.PROD_VL_ESTOQUE_MINIMO;
                    if (dif > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(dif.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

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
                Session["AbaDash"] = 2;
                return RedirectToAction("MontarTelaDashboardProduto");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Produto";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "CRMsys", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }
    
        public ActionResult GerarRelatorioListaZerado()
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
                nomeRel = "ProdutoListaZerado" + "_" + data + ".pdf";
                titulo = "Materiais/Produtos - Estoque Zerado";
                lista = (List<PRODUTO>)Session["ListaProdZerado"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                PdfPTable table = new PdfPTable(new float[] { 180f, 60f, 60f, 100f, 100f, 50f, 80f, 80f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Atual", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Mínimo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Diferença (%)", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    Decimal? dif = (item.PROD_VL_ESTOQUE_ATUAL * 100) / item.PROD_VL_ESTOQUE_MINIMO;
                    if (dif > 0)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(dif.Value), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

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
                Session["AbaDash"] = 2;
                return RedirectToAction("MontarTelaEstoque");
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

        public ActionResult GerarRelatorioListaEsgota()
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
                nomeRel = "ProdutoListaEsgota" + "_" + data + ".pdf";
                titulo = "Materiais/Produtos - Estoque Esgotando em 30 Dias";
                lista = (List<PRODUTO>)Session["ListaProdEsgota"];
                Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

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
                PdfPTable table = new PdfPTable(new float[] { 180f, 80f, 80f, 100f, 100f, 50f, 80f, 80f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estoque Atual", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Estimativa Mensal", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
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
                cell = new PdfPCell(new Paragraph("", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (PRODUTO item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.PROD_NM_NOME, meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
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
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_PRODUTO.CAPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.SUBCATEGORIA_PRODUTO.SCPR_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_CD_CODIGO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MODELO, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.PROD_NM_MARCA, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.PROD_IN_TIPO_PRODUTO == 1)
                    {
                        cell = new PdfPCell(new Paragraph("Material", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("Produto", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_LEFT
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
                Session["AbaDash"] = 2;
                return RedirectToAction("MontarTelaEstoque");
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

        public ActionResult GerarRelatorioListaEstoqueFilial()
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
                List<ModeloViewModel> lista = new List<ModeloViewModel>();
                nomeRel = "ProdutoEstoqueFilial" + "_" + data + ".pdf";
                titulo = "Produtos - Estoque por Filial";
                lista = (List<ModeloViewModel>)Session["ListaProdutoFilial"];
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
                table = new PdfPTable(new float[] { 100f, 100f, 60f, 100f, 100f, 50f, 80f, 80f, 60f, 60f });
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Local", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Produto", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Est.Atual", meuFont))
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
                cell = new PdfPCell(new Paragraph("Sub-Categoria", meuFont))
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
                cell = new PdfPCell(new Paragraph("Modelo", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Marca", meuFont))
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
                cell = new PdfPCell(new Paragraph("", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                foreach (ModeloViewModel item in lista)
                {
                    cell = new PdfPCell(new Paragraph(item.Nome4, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (item.ValorDec != null)
                    {
                        cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.ValorDec), meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Paragraph("0,00", meuFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Paragraph(item.Nome6, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.Nome7, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome1, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome3, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome5, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Paragraph(item.Nome8, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);

                    if (System.IO.File.Exists(Server.MapPath(item.Nome9)))
                    {
                        cell = new PdfPCell();
                        image = Image.GetInstance(Server.MapPath(item.Nome9));
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
                Session["AbaDash"] = 2;
                return RedirectToAction("MontarTelaEstoque");
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

        [HttpGet]
        public ActionResult EditarEstoqueFilial(Int32 id)
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
                    if (usuario.PERFIL.PERF_IN_EDICAO_PROD == 0)
                    {
                        Session["MensPermissao"] = 2;
                        Session["ModuloPermissao"] = "Produtos";
                        return RedirectToAction("MontarTelaPaciente", "Paciente");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Prepara view
                PRODUTO prod = (PRODUTO)Session["Produto"];
                ViewBag.Nome = prod.PROD_NM_NOME;
                ViewBag.Codigo = prod.PROD_CD_CODIGO;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_8.pdf";

                // Monta lista de autorizadores
                List<USUARIO> auts = CarregarUsuario().Where(p => p.PERFIL.PERF_SG_SIGLA == "ADM").ToList();
                ViewBag.Autorizadores = new SelectList(auts, "USUA_CD_ID", "USUA_NM_NOME");

                PRODUTO_ESTOQUE_FILIAL item = prodApp.GetEstoqueFilialById(id);
                ProdutoEstoqueFilialViewModel vm = Mapper.Map<PRODUTO_ESTOQUE_FILIAL, ProdutoEstoqueFilialViewModel>(item);
                vm.PREF_DT_ULTIMO_MOVIMENTO = DateTime.Today.Date;
                Session["UltEstoque"] = vm.PREF_QN_ESTOQUE;
                return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstoqueFilial(ProdutoEstoqueFilialViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((String)Session["Ativa"] == null)
                    {
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                    Int32 idAss = (Int32)Session["IdAssinante"];

                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PRODUTO_ESTOQUE_FILIAL item = Mapper.Map<ProdutoEstoqueFilialViewModel, PRODUTO_ESTOQUE_FILIAL>(vm);
                    vm.PREF_QN_ESTOQUE_TOTAL = vm.PREF_QN_ESTOQUE + vm.PREF_QN_QUANTIDADE_RESERVADA;
                    Int32 volta = prodApp.ValidateEditEstoqueFilial(item);

                    // Soma estoques
                    Int32? estAnterior = (Int32?)Session["UltEstoque"];
                    PRODUTO prod = prodApp.GetItemById(item.PROD_CD_ID);
                    List<PRODUTO_ESTOQUE_FILIAL> ests = prod.PRODUTO_ESTOQUE_FILIAL.Where(p => p.PREF_IN_ATIVO == 1).ToList();
                    Int32 soma = ests.Sum(p => p.PREF_QN_ESTOQUE.Value);
                    Int32 somaRes = ests.Sum(p => p.PREF_QN_QUANTIDADE_RESERVADA.Value);
                    Int32 tot = soma + somaRes;
                    prod.PROD_VL_ESTOQUE_ATUAL = soma;
                    prod.PROD_VL_ESTOQUE_TOTAL = tot;

                    // Atualiza valor do estoque
                    Int32 volta1 = prodApp.ValidateEdit(prod, prod);

                    // Grava movimentação do estoque
                    MOVIMENTO_ESTOQUE_PRODUTO mov = new MOVIMENTO_ESTOQUE_PRODUTO();
                    mov.ASSI_CD_ID = idAss;
                    mov.EMPR_CD_ID = usuarioLogado.EMPR_CD_ID;
                    mov.USUA_CD_ID = usuarioLogado.USUA_CD_ID;
                    mov.PROD_CD_ID = item.PROD_CD_ID;
                    mov.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                    mov.MOEP_IN_TIPO_MOVIMENTO = estAnterior < item.PREF_QN_ESTOQUE ? 1 : 2;
                    mov.MOEP_VL_QUANTIDADE_MOVIMENTO = item.PREF_QN_ESTOQUE > estAnterior ? (item.PREF_QN_ESTOQUE - estAnterior) : (estAnterior - item.PREF_QN_ESTOQUE);
                    mov.MOEP_VL_QUANTIDADE_ANTERIOR = item.PREF_QN_ESTOQUE;
                    mov.MOEP_IN_ORIGEM = "Ajuste Manual";
                    mov.MOEP_IN_CHAVE_ORIGEM = 0;
                    mov.MOEP_IN_ATIVO = 1;
                    mov.MOEP_IN_ULTIMO = 1;
                    mov.MOEP_DS_JUSTIFICATIVA = item.PREF_DS_JUSTIFICATIVA;
                    mov.EMFI_CD_ID = item.EMFI_CD_ID;
                    mov.MOEP_EMFI_CD_ID = item.EMFI_CD_ID;
                    mov.MOEP_EMFI_CD_ID = null;
                    mov.MOEP_EMFI_CD_ID_ALVO = null;
                    mov.MOEP_IN_TIPO = estAnterior < item.PREF_QN_ESTOQUE ? 4 : 8;
                    mov.CRPV_CD_ID = null;
                    mov.COBA_CD_ID = null;
                    mov.FOPA_CD_ID = null;
                    mov.MOEP_VL_VALOR_MOVIMENTO = 0;
                    mov.FORN_CD_ID = null;
                    mov.MOEP_DS_MANUTENCAO_OBSERVACAO = null;
                    mov.MOEP_IN_PENDENTE = 1;
                    mov.MOEP_IN_AUTORIZADOR = vm.USUA_CD_ID;
                    mov.MOEP_IN_TIPO_LANCAMENTO = 0;
                    mov.MOEP_DT_LANCAMENTO = null;
                    mov.MOEP_DT_PAGAMENTO = null;
                    Int32 volta4 = prodApp.ValidateCreateMovimento(mov, usuarioLogado);

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuarioLogado.ASSI_CD_ID,
                        USUA_CD_ID = usuarioLogado.USUA_CD_ID,
                        LOG_NM_OPERACAO = "eefPROD",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO_ESTOQUE_FILIAL>(item),
                        LOG_IN_SISTEMA = 6
                    };
                    Int32 volta2 = logApp.ValidateCreate(log);

                    // Verifica retorno
                    Session["AbaProduto"] = 9;
                    Session["RecuperaEstado"] = 1;
                    Session["FlagProduto"] = 1;
                    return RedirectToAction("VoltarAnexoProduto");
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
        public ActionResult VerMovimentacaoEstoque()
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
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Carrega listas
                PRODUTO prod = (PRODUTO)Session["Produto"];
                ViewBag.Nome = prod.PROD_NM_NOME;
                ViewBag.Codigo = prod.PROD_CD_CODIGO;
                List<MOVIMENTO_ESTOQUE_PRODUTO> movs = prod.MOVIMENTO_ESTOQUE_PRODUTO.ToList();
                movs = movs.Where(p => p.MOEP_IN_ATIVO == 1).ToList();
                Session["ListaMovimEstoque"] = movs;

                // Prepara lista
                ViewBag.Usuarios = new SelectList(CarregarUsuario().OrderBy(p => p.USUA_NM_NOME), "USUA_CD_ID", "USUA_NM_NOME");
                ViewBag.Listas = movs;
                Session["AjudaNivel"] = "../BaseAdmin/Ajuda/10/Ajuda10_7.pdf";

                // Indicadores
                ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

                // Abre view
                Session["AbaProduto"] = 9;
                MOVIMENTO_ESTOQUE_PRODUTO objetoFalha = new MOVIMENTO_ESTOQUE_PRODUTO();
                objetoFalha.MOEP_DT_MOVIMENTO = DateTime.Today.Date;
                objetoFalha.MOEP_DT_DATA_DUMMY = DateTime.Today.Date;
                return View(objetoFalha);
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

        public ActionResult RetirarFiltroMovimento()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ListaMovimEstoque"] = null;
                return RedirectToAction("VerMovimentacaoEstoque");
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
                if (item.USUA_CD_ID > 0)
                {
                    query = query.Where(p => p.USUA_CD_ID == item.USUA_CD_ID);
                }
                if (item.MOEP_DT_MOVIMENTO != DateTime.MinValue & item.MOEP_DT_DATA_DUMMY == DateTime.MinValue)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_MOVIMENTO));
                }
                if (item.MOEP_DT_MOVIMENTO == DateTime.MinValue & item.MOEP_DT_DATA_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p =>DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DT_MOVIMENTO != DateTime.MinValue & item.MOEP_DT_DATA_DUMMY != DateTime.MinValue)
                {
                    query = query.Where(p => DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) >= DbFunctions.TruncateTime(item.MOEP_DT_MOVIMENTO) & DbFunctions.TruncateTime(p.MOEP_DT_MOVIMENTO) <= DbFunctions.TruncateTime(item.MOEP_DT_DATA_DUMMY));
                }
                if (item.MOEP_DS_JUSTIFICATIVA != null)
                {
                    query = query.Where(p => p.MOEP_DS_JUSTIFICATIVA.ToUpper().Contains(item.MOEP_DS_JUSTIFICATIVA.ToUpper()));
                }

                if (query != null)
                {
                    query = query.Where(p => p.ASSI_CD_ID == idAss);
                    query = query.Where(p => p.MOEP_IN_ATIVO == 1);
                    query = query.OrderBy(a => a.MOEP_DT_MOVIMENTO);
                    lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                // Sucesso
                Session["ListaMovimEstoque"] = lista;
                return RedirectToAction("VerMovimentacaoEstoque");
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


        //public ActionResult VerTodosProdutosProposta()
        //{
        //    try
        //    {
        //        // Verifica se tem usuario logado
        //        if ((String)Session["Ativa"] == null)
        //        {
        //            return RedirectToAction("Logout", "ControleAcesso");
        //        }
        //        USUARIO usuario = (USUARIO)Session["UserCredentials"];
        //        Int32 idAss = (Int32)Session["IdAssinante"];

        //        // Processa
        //        ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoPedido"];
        //        Session["AbaDash"] = 2;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = ex.Message;
        //        Session["TipoVolta"] = 2;
        //        Session["VoltaExcecao"] = "Produto";
        //        Session["Excecao"] = ex;
        //        Session["ExcecaoTipo"] = ex.GetType().ToString();
        //        GravaLogExcecao grava = new GravaLogExcecao(usuApp);
        //        Int32 voltaX = grava.GravarLogExcecao(ex, "Produto", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
        //        return RedirectToAction("TrataExcecao", "BaseAdmin");
        //    }
        //}

        public ActionResult VerTodosProdutosAcima()
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
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoAcima"];
                Session["AbaDash"] = 2;
                return View();
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

        public ActionResult VerTodosProdutosEstoqueFilial()
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
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoFilial"];
                Session["AbaDash"] = 2;
                return View();
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


        public ActionResult VerTodosProdutosAbaixo()
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
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoAbaixo"];
                Session["AbaDash"] = 2;
                return View();
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

        public ActionResult VerTodosProdutosZerado()
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
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoZerado"];
                Session["AbaDash"] = 2;
                return View();
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

        public ActionResult VerTodosProdutosEsgota()
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
                ViewBag.Lista = (List<ModeloViewModel>)Session["ListaProdutoEsgota"];
                Session["AbaDash"] = 2;
                return View();
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

        public ActionResult ImprimirContratoLocacao(Int32 id)
        {
            Session["VoltaPrintLocacao"] = 2;
            Session["IdLocacao"] = id;
            Session["AbaProduto"] = 13;
            return RedirectToAction("ImprimirContratoLocacaoDireto", "Locacao");
        }

        public ActionResult EditarLocacao(Int32 id)
        {
            Session["VoltaLocacaoBase"] = 3;
            Session["IdLocacao"] = id;
            Session["AbaProduto"] = 13;
            return RedirectToAction("VoltarAnexoLocacao", "Locacao");
        }

    }
}