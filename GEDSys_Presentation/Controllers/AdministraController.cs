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
using iText.IO.Codec;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
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
using Humanizer;
using Newtonsoft.Json;
using EntitiesServices.Work_Classes;
using System.Windows.Input;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GEDSys_Presentation.Controllers
{
    public class AdministraController : Controller
    {
        private readonly IPacienteAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly ITipoPessoaAppService tpApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IGrupoAppService gruApp;
        private readonly IMensagemEnviadaSistemaAppService meApp;
        private readonly IEmpresaAppService empApp;
        private readonly IAssinanteAppService assApp;
        private readonly IControleMensagemAppService cmApp;
        private readonly IRecursividadeAppService recuApp;
        private readonly IMensagemAppService mensApp;
        private readonly ITipoPacienteAppService tpaApp;
        private readonly ILaboratorioAppService labApp;
        private readonly ITemplateEMailAppService temApp;
        private readonly IMedicamentoAppService medApp;
        private readonly ITemplateSMSAppService smsApp;
        private readonly IConfiguracaoAnamneseAppService anaApp;
        private readonly IAvisoLembreteAppService aviApp;
        private readonly IConfiguracaoCalendarioAppService calApp;
        private readonly ISolicitacaoAppService solApp;
        private readonly IValorConsultaAppService vcApp;
        private readonly IAcessoMetodoAppService aceApp;
        private readonly IProdutoAppService prodApp;
        private readonly ILocacaoAppService locApp;
        private readonly IAreaPacienteAppService areaApp;

#pragma warning disable CS0169 // O campo "PacienteController.msg" nunca é usado
        private String msg;
#pragma warning restore CS0169 // O campo "PacienteController.msg" nunca é usado
#pragma warning disable CS0169 // O campo "PacienteController.exception" nunca é usado
        private Exception exception;
#pragma warning restore CS0169 // O campo "PacienteController.exception" nunca é usado
        private PACIENTE objeto = new PACIENTE();
        private PACIENTE objetoAntes = new PACIENTE();
        private List<PACIENTE> listaMaster = new List<PACIENTE>();
        private List<PACIENTE> listaMasterAtraso = new List<PACIENTE>();
        private List<PACIENTE> listaMasterAusencia = new List<PACIENTE>();
        private List<PACIENTE_CONSULTA> listaMasterConsulta = new List<PACIENTE_CONSULTA>();
        private List<PACIENTE_SOLICITACAO> listaMasterSolicitacao = new List<PACIENTE_SOLICITACAO>();
        private PACIENTE_SOLICITACAO objetoSolicitacao = new PACIENTE_SOLICITACAO();
        private List<PACIENTE_ATESTADO> listaMasterAtestado = new List<PACIENTE_ATESTADO>();
        private PACIENTE_ATESTADO objetoAtestado = new PACIENTE_ATESTADO();
        private List<PACIENTE_EXAMES> listaMasterExame = new List<PACIENTE_EXAMES>();
        private PACIENTE_EXAMES objetoExame = new PACIENTE_EXAMES();
        private List<PACIENTE_PRESCRICAO> listaMasterPrescricao = new List<PACIENTE_PRESCRICAO>();
        private PACIENTE_PRESCRICAO objetoPrescricao = new PACIENTE_PRESCRICAO();
        private List<PACIENTE_PRESCRICAO_ITEM> listaMasterItem = new List<PACIENTE_PRESCRICAO_ITEM>();
        private PACIENTE_PRESCRICAO_ITEM objetoItem = new PACIENTE_PRESCRICAO_ITEM();
        private PACIENTE_CONSULTA objetoConsulta = new PACIENTE_CONSULTA();
        private List<PACIENTE_CONSULTA> listaMasterCalendario = new List<PACIENTE_CONSULTA>();
        private MedicamentoViewModel objetoRemedio = new MedicamentoViewModel();
        private List<MedicamentoViewModel> listaMasterRemedio = new List<MedicamentoViewModel>();
        private PACIENTE_HISTORICO objetoHistorico = new PACIENTE_HISTORICO();
        private List<PACIENTE_HISTORICO> listaMasterHistorico = new List<PACIENTE_HISTORICO>();
        private List<USUARIO> listaMasterUsuario = new List<USUARIO>();
        private USUARIO objetoUsuario = new USUARIO();
        private List<PACIENTE_CONSULTA> listaMasterCalendarioMarcacao = new List<PACIENTE_CONSULTA>();
        private String extensao;

        public AdministraController(IPacienteAppService baseApps, ILogAppService logApps, ITipoPessoaAppService tpApps, IUsuarioAppService usuApps, IConfiguracaoAppService confApps, IGrupoAppService gruApps, IMensagemEnviadaSistemaAppService meApps, IEmpresaAppService empApps, IAssinanteAppService assApps, IControleMensagemAppService cmApps, IRecursividadeAppService recuApps, IMensagemAppService mensApps, ITipoPacienteAppService tpaApps, ILaboratorioAppService labApps, ITemplateEMailAppService temApps, IMedicamentoAppService medApps, ITemplateSMSAppService smsApps, IConfiguracaoAnamneseAppService anaApps, IAvisoLembreteAppService aviApps, IConfiguracaoCalendarioAppService calApps, ISolicitacaoAppService solApps, IValorConsultaAppService vcApps, IAcessoMetodoAppService aceApps, IProdutoAppService prodApps, ILocacaoAppService locApps, IAreaPacienteAppService areaApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            tpApp = tpApps;
            usuApp = usuApps;
            confApp = confApps;
            gruApp = gruApps;
            meApp = meApps;
            empApp = empApps;
            assApp = assApps;
            cmApp = cmApps;
            recuApp = recuApps;
            mensApp = mensApps;
            tpaApp = tpaApps;
            labApp = labApps;
            temApp = temApps;
            medApp = medApps;
            smsApp = smsApps;
            anaApp = anaApps;
            aviApp = aviApps;
            calApp = calApps;
            solApp = solApps;
            vcApp = vcApps;
            aceApp = aceApps;
            prodApp = prodApps;
            locApp = locApps;
            areaApp = areaApps;
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
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }
    }
}