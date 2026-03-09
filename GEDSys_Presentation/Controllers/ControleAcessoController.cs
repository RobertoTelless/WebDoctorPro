using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using CRMPresentation.App_Start;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using CrossCutting;
using System.Text;
using System.Net;
using ERP_Condominios_Solution.Classes;
using System.Threading.Tasks;
using GEDSys_Presentation.App_Start;


namespace ERP_Condominios_Solution.Controllers
{
    public class ControleAcessoController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly ITemplateAppService temApp;
        private readonly IAssinanteAppService assApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAcessoMetodoAppService aceApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public ControleAcessoController(IUsuarioAppService baseApps, IConfiguracaoAppService confApps, ITemplateAppService temApps, IAssinanteAppService assApps, IUsuarioAppService usuApps, IAcessoMetodoAppService aceApps)
        {
            baseApp = baseApps;
            confApp = confApps;
            temApp = temApps;
            assApp = assApps;
            usuApp = usuApps;
            aceApp = aceApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            USUARIO item = new USUARIO();
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
            return View(vm);
        }


        public ActionResult AtualizarCaptcha()
        {

            String texto = string.Empty;
            Random randNum = new Random();

            Session["Captcha"] = randNum.Next(10000, 99999).ToString();
            ViewBag.Captcha = "~/Handlers/ghCaptcha.ashx?" + Session["Captcha"];

            return RedirectToAction("Login", "ControleAcesso");
        }

        public void MontaSessao()
        {
            // Estado
            Session["Close"] = false;
            Session["MensEnvioLogin"] = 0;
            Session["Ativa"] = "1";
            Session["PlanosGeral"] = null;
            Session["PlanosCarga"] = null;
            Session["UF"] = null;
            Session["TipoPessoa"] = null;
            Session["MensEnvioLogin"] = 0;
            Session["MensPermissao"] = 0;
            Session["ModuloPermissao"] = 0;
            Session["Vence30"] = "Não";
            Session["PlanosLista"] = null;
            Session["ListaPerfilBase"] = null;
            Session["FlagDataMens"] = 1;
            Session["LinhaAlterada"] = 0;
            Session["MensagemFabricante"] = 0;
            Session["AjudaNivel"] = 0;
            Session["FlagExcecao"] = 0;
            Session["TipoVolta"] = 0;
            Session["VoltaExcecao"] = null;
            Session["Excecao"] = null;
            Session["ExcecaoTipo"] = null;
            Session["FlagInicial"] = 0;
            Session["FiltroData"] = 1;
            Session["FiltroStatus"] = 1;
            Session["VoltaMsg"] = 0;
            Session["ListaConfirma"] = null;
            Session["Historicos"] = null;
            Session["VoltaAniversario"] = 0;
            Session["ListaAvisoRTi"] = null;
            Session["ListaAcessoTotal"] = null;
            Session["FiltroAcessoTotal"] = null;
            Session["ModuloAtual"] = null;
            Session["AvisosAbertos"] = 0;
            Session["ListaEstoqueBase"] = null;
            Session["VoltarMsgPaciente"] = 0;
            Session["ListaNoticia"] = null;
            Session["NoticiaGeral"] = null;
            Session["NoticiaAlterada"] = 0;
            Session["AreaPacienteAlterada"] = 0;
            Session["AreaPacientes"] = null;
            Session["ListaAreaPaciente"] = null;
            Session["IncluirConsultaArea"] = 1;

            // Permissões
            Session["PermProntuario"] = 0;
            Session["NumPacientes"] = 0;
            Session["NumUsuarios"] = 0;
            Session["NumEMail"] = 0;
            Session["NumSMS"] = 0;
            Session["NumWhatsApp"] = 0;
            Session["NumGrupos"] = 0;
            Session["NumItemGrupos"] = 0;
            Session["PermGeral"] = 1;
            Session["VoltarTabs"] = 1;
            Session["VoltarEmpresa"] = 1;
            Session["VoltaCompara"] = 1;

            // Acesso
            Session["ListaAcessos"] = null;
            Session["MensAcesso"] = null;
            Session["MensFC"] = null;
            Session["MensagemLogin"] = null;
            Session["MensSenha"] = null;
            Session["UltimoLogin"] = null;
            Session["Cargo"] = null;
            Session["NomeMax"] = null;
            Session["Greeting"] = null;
            Session["Nome"] = null;
            Session["Foto"] = null;
            Session["MensEnvioLogin"] = 0;
            Session["CodigoSenha"] = null;
            Session["Senha"] = null;
            Session["VoltaInfoConsulta"] = 0;
            Session["IdLoginSessao"] = null;

            // Periodicidades
            Session["Periodicidades"] = null;
            Session["PeriodicidadeAlterada"] = 0;
            Session["PeriodicidadePlanoAlterada"] = 0;
            Session["Planos"] = null;
            Session["PlanoAlterada"] = 0;

            // Videos
            Session["ListaVideo"] = null;
            Session["VideoAlterada"] = 0;
            Session["Videos"] = null;
            Session["Video"] = null;
            Session["VoltarVideo"] = 1;
            Session["VoltaCliGrupo"] = 0;
            Session["VoltaAnexoExame"] = 1;



            // Configuracao
            Session["Configuracao"] = null;
            Session["ConfAlterada"] = 0;
            Session["ExtensoesPossiveis"] = ".PDF|.TXT|.JPG|.JPEG|.PNG|.GIF|.MP4|.MKV|.XLS|.XLSX|.PPT|.PPTX|.DOC|.DOCX|.ODS|.ODT|.ODP|.ODG";
            Session["VoltaConfCalendario"] = 1;
            Session["ListaBloqueios"] = null;

            // Assinante
            Session["Assinantes"] = null;
            Session["AssinantesGeral"] = null;
            Session["AssinanteAlterada"] = 0;
            Session["IdAssinante"] = 0;

            // Medicos
            Session["ListaMedico"] = null;
            Session["MedicoAlterada"] = 0;
            Session["Medicos"] = null;
            Session["Medico"] = null;
            Session["VoltaMedico"] = 0;
            Session["UF"] = null;
            Session["IdMedico"] = 0;
            Session["IdEnvio"] = 0;
            Session["EnvioAlterada"] = 0;
            Session["ListaEnvio"] = null;
            Session["TipoEnvios"] = null;
            Session["TipoEnvioAlterada"] = 0;
            Session["TipoMedicoEnvio"] = 0;

            // Indicacoes
            Session["IndicacaoAlterada"] = 0;
            Session["Indicacoes"] = null;
            Session["ListaIndicacoes"] = null;
            Session["IdIndicacao"] = null;
            Session["Indicacao"] = null;
            Session["VoltaIndicacao"] = 0;

            // Perfil
            Session["MensPerfil"] = null;
            Session["VoltaPerfil"] = 0;
            Session["TabPerfil"] = 0;
            Session["Perfis"] = null;
            Session["PerfilAlterada"] = 0;
            Session["PerfisBase"] = null;
            Session["ListaPerfilBase"] = null;
            Session["Perfil"] = null;
            Session["PerfilSigla"] = null;
            Session["BlocoAnamnese"] = 1;

            // Log
            Session["ListaLog"] = null;
            Session["FiltroLog"] = null;
            Session["MensLog"] = 0;
            Session["VoltaLog"] = 0;
            Session["MensagemLonga"] = 0;
            Session["IdLoginSessao"] = 0;
            Session["VoltarPesquisa"] = 0;

            // Templates
            Session["ListaTemplateEMail"] = null;
            Session["TemplateEMail"] = null;
            Session["IncluirTemplateEMail"] = 0;
            Session["MensTemplateEMail"] = null;
            Session["VoltaTemplateEMail"] = 0;
            Session["IdTemplateEMail"] = null;
            Session["ModeloEMails"] = null;
            Session["ModeloEMailAlterada"] = 0;
            Session["ListaTemplateEMailHTML"] = null;
            Session["Templates"] = null;
            Session["TemplateAlterada"] = 0;

            // Usuario
            Session["ListaUsuario"] = null;
            Session["FiltroUsuario"] = null;
            Session["Usuarios"] = null;
            Session["UsuariosAdm"] = null;
            Session["UsuarioAlterada"] = 0;
            Session["NumeroUsuarios"] = 0;
            Session["VoltaAnexos"] = 0;
            Session["MensUsuario"] = 0;
            Session["VoltaUsuario"] = 0;
            Session["IdUsuario"] = 0;
            Session["FileQueueUsuario"] = null;
            Session["VoltaUsu"] = 0;
            Session["ListaLogExcecao"] = null;
            Session["Classes"] = null;
            Session["ListaAvisoAtivo"] = null;
            Session["ListaUsuarioConsulta"] = null;

            // Mensagens enviada
            Session["FlagMensagensEnviadas"] = 0;
            Session["MensagensEnviadas"] = null;
            Session["MensagensEnviadaAlterada"] = 0;
            Session["TipoCargaMsgInt"] = 0;
            Session["MostraTodasMensagens"] = 0;
            Session["Enviadas"] = null;

            // Tabelas auxiliares
            Session["ListaTipoPaciente"] = null;
            Session["ListaTipoExame"] = null;
            Session["ListaTipoAtestado"] = null;
            Session["ListaConvenio"] = null;
            Session["TipoPacientes"] = null;
            Session["TipoPacienteAlterada"] = 0;
            Session["TipoExames"] = null;
            Session["TipoExameAlterada"] = 0;
            Session["TipoAtestados"] = null;
            Session["TipoAtestadoAlterada"] = 0;
            Session["Convenios"] = null;
            Session["ConvenioAlterada"] = 0;
            Session["MensAuxiliar"] = 0;
            Session["Cores"] = null;
            Session["Sexos"] = null;
            Session["EstadosCivil"] = null;
            Session["Graus"] = null;
            Session["GrausParente"] = null;
            Session["TipoControles"] = null;
            Session["TipoAtestados"] = null;
            Session["TipoFormas"] = null;
            Session["ListaLaboratorio"] = null;
            Session["Laboratorios"] = null;
            Session["LaboratorioAlterada"] = 0;
            Session["Especialidades"] = null;
            Session["EspecialidadeAlterada"] = 0;
            Session["ListaEspecialidade"] = null;
            Session["ListaFormaRecebimento"] = null;
            Session["FormaRecebimentos"] = null;
            Session["FormaRecebimentoAlterada"] = 0;
            Session["ListaTipoPagamento"] = null;
            Session["TipoPagamentos"] = null;
            Session["TipoPagamentoAlterada"] = 0;
            Session["ListaTipoValorConsulta"] = null;
            Session["TipoValorConsultas"] = null;
            Session["TipoValorConsultaAlterada"] = 0;
            Session["ListaTipoValorServico"] = null;
            Session["TipoValorServicos"] = null;
            Session["TipoValorServicoAlterada"] = 0;

            // Empresa
            Session["Empresas"] = null;
            Session["EmpresaAlterada"] = 0;
            Session["VoltarEmpresa"] = 1;
            Session["IdEmpresa"] = 0;
            Session["Empresa"] = null;
            Session["NomeEmpresa"] = null;
            Session["NivelEmpresa"] = 0;
            Session["ListaAviso"] = null;
            Session["AvisoAlterada"] = 0;
            Session["Avisos"] = null;

            // Mensageria
            Session["Mensagens"] = null;
            Session["MensagemAlterada"] = 0;
            Session["ListaEnvios"] = null;
            Session["ListaMensagemEMail"] = null;
            Session["ListaRecursividadeEMail"] = null;
            Session["ListaEMailDataSaida"] = null;
            Session["ListaEMailMesSaida"] = null;
            Session["ListaEMailEnvioData"] = null;
            Session["FlagMensagensEnviadas"] = 0;
            Session["VoltaMensagem"] = 0;
            Session["MsgCRUD"] = null;

            // Grupo
            Session["ListaGrupo"] = null;
            Session["Grupo"] = null;
            Session["IncluirGrupo"] = 0;
            Session["ListaClienteGrupo"] = null;
            Session["MensGrupo"] = null;
            Session["LinhaAlterada"] = 0;
            Session["VoltaGrupo"] = 0;
            Session["GrupoNovo"] = null;
            Session["IdGrupo"] = null;
            Session["GrupoAlterada"] = 0;
            Session["TotalGrupo"] = null;
            Session["LinhaAlterada"] = 0;
            Session["Grupos"] = null;

            // Pacientes
            Session["VoltaAusencia"] = 0;
            Session["PacientesAusente"] = null;
            Session["Pacientes"] = null;
            Session["PacientesBase"] = null;
            Session["ListaPaciente"] = null;
            Session["FiltroPaciente"] = null;
            Session["VoltaPaciente"] = 0;
            Session["MensPaciente"] = null;
            Session["PacienteAlterada"] = 0;
            Session["IdPaciente"] = null;
            Session["Paciente"] = null;
            Session["ListaPacienteBase"] = null;
            Session["ListaPacienteAnivDia"] = null;
            Session["FlagPaciente"] = 1;
            Session["ListaAnivDia"] = null;
            Session["NivelPaciente"] = 1;
            Session["ListaPacienteCats"] = null;
            Session["ListaDatasPaciente"] = null;
            Session["ListaConsultaData"] = null;
            Session["ListaPacienteAlteradoMes"] = null;
            Session["PacientesGeral"] = null;
            Session["VoltaCatPaciente"] = 0;
            Session["NomePacienteIncluir"] = null;
            Session["ListaPacienteEMail"] = null;
            Session["IdPrescricao"] = 0;
            Session["IdAtestado"] = 0;
            Session["NivelExame"] = 1;
            Session["VoltaPrescricao"] = 0;
            Session["IdUltimaAnamnese"] = null;
            Session["TemAnamnese"] = 0;
            Session["Anamnese"] = null;
            Session["TemFisico"] = 0;
            Session["ExameFisico"] = null;
            Session["PacientesAtraso"] = null;
            Session["PacientesAusente"] = null;
            Session["FiltroPacienteAusencia"] = null;
            Session["FiltroPacienteAtraso"] = null;
            Session["EditaAtestado"] = 0;
            Session["EditaSolicitacao"] = 0;
            Session["EditaPrescricao"] = 0;
            Session["EditaExame"] = 0;
            Session["EditaAnamnese"] = 0;
            Session["EditaFisico"] = 0;
            Session["TemExameFisico"] = 0;
            Session["VoltaAtestado"] = 1;
            Session["ListaConsultas"] = null;
            Session["ListaConsultasGeral"] = null;
            Session["Consultas"] = null;
            Session["ConsultasAlterada"] = 0;
            Session["ListaSolicitacoes"] = null;
            Session["Solicitacoes"] = null;
            Session["SolicitacoesAlterada"] = 0;
            Session["VoltarCentral"] = 1;
            Session["TipoSolicitacao"] = 0;
            Session["ListaAtestados"] = null;
            Session["Atestados"] = null;
            Session["AtestadosAlterada"] = 0;
            Session["ListaExames"] = null;
            Session["Exames"] = null;
            Session["ExamesAlterada"] = 0;
            Session["ListaPrescricoes"] = null;
            Session["Prescricoes"] = null;
            Session["PrescricoesAlterada"] = 0;
            Session["ItensPrescricoes"] = null;
            Session["ItensPrescricoesAlterada"] = 0;
            Session["ListaMedicamentos"] = null;
            Session["ListaMedicamentosPaciente"] = null;
            Session["VoltarConsulta"] = 0;
            Session["VoltaAnamnese"] = 1;
            Session["VoltaFisico"] = 1;
            Session["EscopoConsulta"] = 1;
            Session["ListaHistorico"] = null;
            Session["EditarAtestado"] = 0;
            Session["EditarSolicitacao"] = 0;
            Session["EditarPrescricao"] = 0;
            Session["EditarExame"] = 0;
            Session["VerTodosPaciente"] = 0;
            Session["VoltaListaConsulta"] = 0;
            Session["EditarVer"] = 0;
            Session["VoltarPesquisa"] = 0;
            Session["ProximaConsulta"] = 0;
            Session["ConsultaMarcada"] = 0;
            Session["ConsultaFrase"] = null;
            Session["EscopoMensagem"] = 0;
            Session["PacienteConsulta"] = null;
            Session["ModoConsulta"] = 0;
            Session["VoltaMail"] = 1;
            Session["ListaTemplateSMS"] = null;
            Session["ModeloSMSAlterada"] = 0;
            Session["ModeloSMSs"] = null;
            Session["ListaMensagemSMS"] = null;
            Session["VoltaMensagem"] = 0;
            Session["VoltaCalendario"] = 0;
            Session["ListaMedicamentoBase"] = null;
            Session["Medicamentos"] = null;
            Session["MedicamentoAlterada"] = 0;
            Session["IncluirItem"] = 0;
            Session["VoltaMedicamento"] = 1;
            Session["VoltarExameConsulta"] = 0;
            Session["ListaSolicitacaoBase"] = null;
            Session["Solicitacao"] = null;
            Session["SolicitacaoAlterada"] = 0;
            Session["Solicitacoes"] = null;
            Session["SolicitacaoBaseAlterada"] = 0;
            Session["SolicitacoesBase"] = null;
            Session["ListaConsultaGeral"] = null;
            Session["VoltaConfirmarCancelar"] = 0;
            Session["VoltaEvolucao"] = 0;
            Session["ListaTextoMedico"] = null;
            Session["MedicoTextoAlterada"] = 0;
            Session["MedicoTextos"] = null;

            // Financeiro
            Session["ListaValorConsulta"] = null;
            Session["ValorConsultas"] = null;
            Session["ValorConsultaAlterada"] = 0;
            Session["ListaValorServico"] = null;
            Session["ValorServicos"] = null;
            Session["ValorServicoAlterada"] = 0;
            Session["ListaValorConvenio"] = null;
            Session["ValorConvenios"] = null;
            Session["ValorConvenioAlterada"] = 0;
            Session["VoltaFinanceiro"] = 0;
            Session["ListaPagamento"] = null;
            Session["Pagamentos"] = null;
            Session["PagamentoAlterada"] = 0;
            Session["ListaRecebimento"] = null;
            Session["Recebimentos"] = null;
            Session["RecebimentoAlterada"] = 0;
            Session["ListaMediaPagtoMes"] = null;
            Session["ListaPagtoRecto"] = null;
            Session["ListaPagtosMes"] = null;
            Session["ListaRectosMes"] = null;
            Session["ListaPagtoRectoMes"] = null;

            // Estoque
            Session["ListaCatProduto"] = null;
            Session["ListaSubCatProduto"] = null;
            Session["VoltaCatProduto"] = 0;
            Session["CategoriaToProduto"] = true;
            Session["VoltaSubCatProduto"] = 0;
            Session["SubCategoriaToProduto"] = true;
            Session["Produtos"] = null;
            Session["CatProdutoAlterada"] = 0;
            Session["ListaProdutoCats"] = null;
            Session["ListaProdutoEspecie"] = null;
            Session["ListaProdutoAcima"] = null;
            Session["TotAcima"] = 0;
            Session["ListaProdutoAbaixo"] = null;
            Session["TotAbaixo"] = 0;
            Session["ListaProdutoAcimaAbaixoGraf"] = null;
            Session["ListaProdutoZerado"] = null;
            Session["TotZerado"] = 0;
            Session["ListaProdutoEsgota"] = null;
            Session["TotEsgota"] = 0;
            Session["FlagProduto"] = 0;
            Session["VoltaBaseProduto"] = 2;
            Session["VoltaProduto"] = 0;
            Session["ListaProduto"] = null;
            Session["BuscaProduto"] = 2;
            Session["PrecoCustoAlterado"] = 0;
            Session["VoltaProduto"] = 1;
            Session["VoltaConsulta"] = 1;
            Session["FlagVoltaProd"] = 1;
            Session["MensProduto"] = null;
            Session["Clonar"] = 0;
            Session["Acerta"] = 0;
            Session["AbaProduto"] = 1;
            Session["FiltroProduto"] = null;
            Session["Clonar"] = 0;
            Session["ProdutoAlterada"] = 0;
            Session["CatProdutoAlterada"] = 0;
            Session["CatProdutos"] = null;
            Session["SubCatProdutoAlterada"] = 0;
            Session["SubCatProdutos"] = null;
            Session["UnidadeAlterada"] = 0;
            Session["Unidades"] = null;
            Session["ProdutoAlterada"] = 0;
            Session["Produtos"] = null;
            Session["ProdutosUltimas"] = null;
            Session["NumProduto"] = 0;
            Session["ListaPrecoProduto"] = null;
            Session["VoltaCatProduto"] = 0;
            Session["VoltaSubCatProduto"] = 0;
            Session["IdProduto"] = 0;
            Session["IdVolta"] = 0;
            Session["MensProduto"] = 0;
            Session["LinhaAlterada"] = 0;
            Session["RecuperaEstado"] = 1;
            Session["FlagAlteraEstado"] = 1;
            Session["ListaCusto"] = null;
            Session["ListaPreco"] = null;
            Session["ListaEstoque"] = null;
            Session["ListaEstoqueTotal"] = null;
            Session["Limite"] = 0;
            Session["ListaConc"] = null;
            Session["Acerta"] = 0;
            Session["VoltaConsulta"] = 0;
            Session["Produto"] = null;
            Session["IdVolta"] = 0;
            Session["VoltaLog"] = 0;
            Session["ListaEstoque"] = null;
            Session["PrecoCustoAlterado"] = 0;
            Session["ListaProdAcima"] = null;
            Session["ListaProdEsgota"] = null;
            Session["ListaProdAbaixo"] = null;
            Session["ListaMovimEstoque"] = null;
            Session["ListaProdutoAcima"] = null;
            Session["ListaProdutoFilial"] = null;
            Session["ListaProdutoAbaixo"] = null;
            Session["ListaProdutoZerado"] = null;
            Session["ListaProdutoEsgota"] = null;
            Session["ListaEstoqueBase"] = null;
            Session["BuscaEstoque"] = 0;
            Session["TipoListagem"] = 0;
            Session["ListaMovimEstoque"] = null;
            Session["ListaMovimentoEstoque"] = null;
            Session["ListaFilialEstoque"] = null;
            Session["ListaHistoricoEstoque"] = null;
            Session["FiltroEstoque"] = null;
            Session["TipoInclusao"] = 0;
            Session["FlagAutorizacao"] = 0;
            Session["ListaMovimentoEstoqueExcluidoProduto"] = null;
            Session["ListaMovimentoEstoque"] = null;
            Session["ListaMovimentoEstoqueExcluido"] = null;
            Session["ListaHistoricoEstoque"] = null;
            Session["ListaProdutoCats"] = null;
            Session["CatProdutoAlterada"] = 1;
            Session["ListaProdutoEspecie"] = null;
            Session["ListaProdutoAcima"] = null;
            Session["TotAcima"] = 0;
            Session["ListaProdutoAbaixo"] = null;
            Session["TotAbaixo"] = 0;
            Session["ListaProdutoAcimaAbaixoGraf"] = null;
            Session["ListaProdutoZerado"] = null;
            Session["TotZerado"] = 0;
            Session["ListaProdutoEsgota"] = null;
            Session["TotEsgota"] = 0;
            Session["CatProdutoAlterada"] = 1;

            // Locacao
            Session["ListaLocacao"] = null;
            Session["LocacaoAlterada"] = 0;
            Session["Locacoes"] = null;
            Session["TipoLocacao"] = 1;
            Session["ListaLocacaoAtiva"] = null;
            Session["ListaLocacaoEncerrada"] = null;
            Session["ListaLocacaoAtrasada"] = null;
            Session["ListaLocacaoCancelada"] = null;
            Session["ListaLocacaoPendente"] = null;
            Session["ListaHistoricoLocacao"] = null;
            Session["ListaHistoricoLocacaoGeral"] = null;
            Session["LocacoesHistoricos"] = null;
            Session["LocacoesParcelas"] = null;
            Session["ListaParcelaLocacaoGeral"] = null;
            Session["ListaContrato"] = null;
            Session["ContratoAlterada"] = 0;
            Session["ContratoLocacoes"] = null;
            Session["TipoContratos"] = null;
            Session["VoltaContratoLocacao"] = 0;

















        }

        [HttpGet]
        public ActionResult Login()
        {
            // Mensagens
            if (Session["MensagemLogin"] != null)
            {
                if ((Int32)Session["MensagemLogin"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0211", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0322", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 16)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 17)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 18)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 19)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 20)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 24)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 55)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0330", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 65)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0671", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0332", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 90)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0341", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 91)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0342", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 92)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0344", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 93)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0345", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 94)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0343", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 25)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0408", CultureInfo.CurrentCulture) + " CRMSys");
                }
                if ((Int32)Session["MensagemLogin"] == 66)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0442", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 94)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0624", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 95)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0625", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 97)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0626", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0636", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensagemLogin"] == 100)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0671", CultureInfo.CurrentCulture));
                }
            }

            // Exibe tela
            MontaSessao();
            Session["Close"] = false;
            Session["MensSenha"] = null;
            Session["MensagemLogin"] = null;
            Session["UserCredentials"] = null;
            USUARIO item = new USUARIO();
            UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
            vm.USUA_IN_HUMANO = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel vm)
        {
            try
            {
                // Inicialização
                USUARIO usuario;
                Session["UserCredentials"] = null;
                ViewBag.Usuario = null;
                Session["MensSenha"] = 0;
                Session["MensagemLogin"] = 0;
                String senha = vm.USUA_NM_SENHA;
                vm.USUA_IN_COMPRADOR = 0;
                Session["AssinantePendente"] = 0;
                Int32? cookie = vm.USUA_IN_COOKIE;

                // Verifica humano
                if (vm.USUA_IN_HUMANO == null || vm.USUA_IN_HUMANO == 0)
                {
                    Session["MensagemLogin"] = 66;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0442", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringGeral(vm.USUA_NM_LOGIN);
                vm.USUA_NM_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(vm.USUA_NM_SENHA);

                // Valida credenciais
                Int32 volta = baseApp.ValidateLogin(vm.USUA_NM_LOGIN, vm.USUA_NM_SENHA, out usuario);
                Session["UserCredentials"] = usuario;
                if (volta == 1)
                {
                    Session["MensagemLogin"] = 11;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 2)
                {
                    Session["MensagemLogin"] = 12;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0002", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 3)
                {
                    Session["MensagemLogin"] = 13;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 5)
                {
                    Session["MensagemLogin"] = 14;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0005", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 4)
                {
                    Session["MensagemLogin"] = 15;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 6)
                {
                    Session["MensagemLogin"] = 16;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0006", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 7)
                {
                    Session["MensagemLogin"] = 17;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0007", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 9)
                {
                    Session["MensagemLogin"] = 18;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0073", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 10)
                {
                    Session["MensagemLogin"] = 19;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0109", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 11)
                {
                    Session["MensagemLogin"] = 20;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0012", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 20)
                {
                    Session["MensagemLogin"] = 21;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 22)
                {
                    Session["MensagemLogin"] = 22;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0228", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 40)
                {
                    Session["MensagemLogin"] = 24;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0264", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (volta == 55)
                {
                    Session["MensagemLogin"] = 25;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0408", CultureInfo.CurrentCulture) + " SysFin");
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Verifica assinante
                ASSINANTE assinante = assApp.GetItemById(usuario.ASSI_CD_ID);
                Session["Assinante"] = assinante;
                if (assinante == null)
                {
                    Session["MensagemLogin"] = 90;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0341", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (assinante.ASSI_IN_BLOQUEADO == 1)
                {
                    Session["MensagemLogin"] = 91;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0342", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (assinante.ASSI_IN_ATIVO == 0)
                {
                    Session["MensagemLogin"] = 92;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0344", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }
                if (assinante.ASSI_IN_VENCIDO == 1)
                {
                    Session["MensagemLogin"] = 93;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0345", CultureInfo.CurrentCulture));
                    vm.USUA_NM_LOGIN = String.Empty;
                    vm.USUA_NM_SENHA = String.Empty;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Verifica se é demo e validade
                Session["eDemo"] = 0;
                Session["diasDemo"] = 0;
                Int32 diasDemo = 0;
                Session["DemoVencido"] = 0;
                if (assinante.ASSI_IN_TIPO == 2)
                {
                    ASSINANTE_PLANO_ASSINATURA plano = assinante.ASSINANTE_PLANO_ASSINATURA.FirstOrDefault();
                    if (plano.ASPA_DT_VALIDADE.Date < DateTime.Today.Date)
                    {
                        Session["DemoVencido"] = 1;
                        Session["MensFC"] = 334;
                        Session["FraseDemoVencido"] = "Sua assinatura de demonstração expirou em " + plano.ASPA_DT_VALIDADE.ToLongDateString() + ". Para continuar a usar o WebDoctorPro você deverá contratar um plano de assinatura";

                        return RedirectToAction("MontarTelaCompraBasicoNova", "BaseAdmin");
                    }
                    else
                    {
                        Session["DemoVencido"] = 2;
                        diasDemo = (plano.ASPA_DT_VALIDADE.Date - DateTime.Today.Date).Days;
                        Session["eDemo"] = 1;
                        TempData["DiasDemo"] = diasDemo;
                        TempData["DemoAtivo"] = true;
                    }
                }

                // Verifica assinatura pendente
                if (assinante.ASSI_IN_TIPO == 3)
                {
                    Session["MensFC"] = 95;
                    Session["AssinantePendente"] = 1;                    
                    return RedirectToAction("MontarTelaFaleConoscoInicio", "BaseAdmin");
                }

                // Armazena credenciais para autorização
                Session["UserCredentials"] = usuario;
                Session["Usuario"] = usuario;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["PlanosVencidos"] = null;
                Session["IdEmpresa"] = usuario.EMPR_CD_ID;
                Session["Empresa"] = usuario.EMPRESA;
                Session["NomeEmpresa"] = usuario.EMPRESA.EMPR_NM_NOME;
                Session["PerfilUsuario"] = usuario.PERFIL;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["NomeEmpresaAssina"] = usuario.EMPRESA.EMPR_NM_GUERRA;
                Session["Cargo"] = usuario.PERFIL.PERF_NM_NOME;
                Session["ModoEntrada"] = 1;

                // Reseta flags de permissao e totais
                Session["PermProntuario"] = 0;
                Session["NumPacientes"] = 0;
                Session["NumUsuarios"] = 0;
                Session["NumEMail"] = 0;
                Session["NumSMS"] = 0;
                Session["NumWhatsApp"] = 0;
                Session["NumGrupos"] = 0;
                Session["NumItemGrupos"] = 0;
                Session["PermGeral"] = 1;
                Session["PermMensageria"] = 0;
                Session["NumProduto"] = 0;
                Session["PermFinanceiro"] = 0;
                Session["PermEstoque"] = 0;
                Session["PermSono"] = 0;
                Session["NumLocacao"] = 0;
                Session["PermLocacao"] = 0;

                // Configuraçoes de escopo de dados
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                Session["MensagemFabricante"] = conf.CONF_IN_MENSAGEM_FABRICANTE;
                
                CacheService cache = CacheService.GetInstance();
                CONFIGURACAO cacheConf = CarregaConfiguracaoGeralCache();

                // Recupera Plano do assinante
                List<ASSINANTE_PLANO_ASSINATURA> plAss = usuario.ASSINANTE.ASSINANTE_PLANO_ASSINATURA.Where(p => p.ASPA_IN_SISTEMA == 6).ToList();
                plAss = plAss.Where(p => p.ASPA_IN_ATIVO == 1).ToList();

                // Verifica se tem plano
                if (plAss.Count == 0)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0215", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (plAss.Count > 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0340", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Verifica validade
                foreach (ASSINANTE_PLANO_ASSINATURA item in plAss)
                {
                    // Verifica validade
                    if (item.ASPA_DT_VALIDADE < DateTime.Today.Date)
                    {
                        // Atualiza assinante
                        assinante.ASSI_IN_BLOQUEADO = 1;
                        Int32 voltaAss = assApp.ValidateEdit(assinante);

                        // Retorna
                        Session["MensagemLogin"] = 94;
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0343", CultureInfo.CurrentCulture));
                        vm.USUA_NM_LOGIN = String.Empty;
                        vm.USUA_NM_SENHA = String.Empty;
                        return RedirectToAction("Login", "ControleAcesso");
                    }
                    if (item.ASPA_DT_VALIDADE < DateTime.Today.Date.AddDays(30))
                    {
                        Session["Vence30"] = "Sim";
                    }
                    else
                    {
                        Session["Vence30"] = "Não";
                    }

                    // Dados do plano
                    Session["NomePlano"] = item.PLANO_ASSINATURA.PLAS_NM_EXIBE;

                    // Recupera Permissões
                    Session["PermMensageria"] = item.PLANO_ASSINATURA.PLAS_IN_MENSAGERIA;
                    Session["PermFinanceiro"] = item.PLANO_ASSINATURA.PLAS_IN_FINANCEIRO;
                    Session["PermEstoque"] = item.PLANO_ASSINATURA.PLAS_IN_ESTOQUE;
                    Session["PermSono"] = item.PLANO_ASSINATURA.PLAS_IN_SONO;
                    Session["PermLocacao"] = item.PLANO_ASSINATURA.PLAS_IN_LOCACAO;

                    // Recupera limites
                    Session["NumPacientes"] = item.PLANO_ASSINATURA.PLAS_NR_CLIENTES;
                    Session["NumEMail"] = item.PLANO_ASSINATURA.PLAS_NR_EMAIL;
                    Session["NumUsuarios"] = item.PLANO_ASSINATURA.PLAS_NR_USUARIOS;
                    Session["NumSMS"] = item.PLANO_ASSINATURA.PLAS_NR_SMS;
                    Session["NumWhatsApp"] = item.PLANO_ASSINATURA.PLAS_NR_WHATSAPP;
                    Session["NumGrupos"] = item.PLANO_ASSINATURA.PLAS_NR_GRUPO;
                    Session["NumItemGrupos"] = item.PLANO_ASSINATURA.PLAS_NR_ITEM_GRUPO;
                    Session["NumPagamentos"] = item.PLANO_ASSINATURA.PLAS_NR_CP;
                    Session["NumRecebimentos"] = item.PLANO_ASSINATURA.PLAS_NR_CR;
                    Session["NumProdutos"] = item.PLANO_ASSINATURA.PLAS_NR_PRODUTO;
                    Session["NumLocacao"] = item.PLANO_ASSINATURA.PLAS_NR_LOCACAO;
                }

                // Recupera ultimo login
                USUARIO_LOGIN ultimoLogin = baseApp.GetAllLogin(usuario.ASSI_CD_ID).Where(p => p.USLO_IN_SISTEMA == 6 & p.USUA_CD_ID == usuario.USUA_CD_ID).OrderByDescending(p => p.USLO_DT_LOGIN).FirstOrDefault();
                Session["UltimoLogin"] = "Primeiro Acesso";
                if (ultimoLogin != null)
                {
                    Session["UltimoLogin"] = "Último Acesso: " + ultimoLogin.USLO_DT_LOGIN.ToString();
                }

                // Atualiza view
                String frase = String.Empty;
                String nome = usuario.USUA_NM_NOME;
                if (DateTime.Now.Hour <= 12)
                {
                    frase = "Bom dia, " + nome;
                }
                else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
                {
                    frase = "Boa tarde, " + nome;
                }
                else
                {
                    frase = "Boa noite, " + nome;
                }

                ViewBag.Greeting = frase;
                ViewBag.Nome = usuario.USUA_NM_NOME;
                ViewBag.Foto = usuario.USUA_AQ_FOTO;

                // Trata Nome
                String nomeMax = String.Empty;
                if (usuario.USUA_NM_NOME.Contains(" "))
                {
                    nomeMax = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
                }
                else
                {
                    nomeMax = usuario.USUA_NM_NOME;
                }

                // Prepara ambiente
                String[] partesDoNome = usuario.USUA_NM_NOME.Split(' ');
                Session["NomeMax"] = nomeMax;
                Session["Greeting"] = frase;
                Session["Nome"] = partesDoNome[0];
                Session["Foto"] = usuario.USUA_AQ_FOTO;
                Session["Perfil"] = usuario.PERFIL;
                Session["PerfilSigla"] = usuario.PERFIL.PERF_SG_SIGLA;
                Session["FlagInicial"] = 0;
                Session["FiltroData"] = 1;
                Session["FiltroStatus"] = 1;
                Session["IdAssinante"] = usuario.ASSI_CD_ID;
                Session["IdUsuario"] = usuario.USUA_CD_ID;
                Session["ExtensoesPossiveis"] = ".PDF|.TXT|.JPG|.JPEG|.PNG|.GIF|.MP4|.MKV|.XLS|.XLSX|.PPT|.PPTX|.DOC|.DOCX|.ODS|.ODT|.ODP|.ODG";
                Session["ExtensoesPossiveisFicha"] = ".PDF|.JPG|.JPEG|.PNG|.GIF";
                Session["IdMarcacao"] = usuario.USUA_CD_ID;
                Session["VoltaSolicitacao"] = 0;

                // Grava login no historico
                USUARIO_LOGIN login = new USUARIO_LOGIN();
                login.ASSI_CD_ID = usuario.ASSI_CD_ID;
                login.USUA_CD_ID = usuario.USUA_CD_ID;
                login.USLO_DT_LOGIN = DateTime.Now;
                login.USLO_IN_SISTEMA = 6;
                login.USLO_IN_ATIVO = 1;
                Int32 voltaLog = baseApp.ValidateCreateLogin(login);
                Session["IdLoginSessao"] = login.USLO_CD_ID;

                // Grava Acesso
                String ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (String.IsNullOrEmpty(ip))
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                if (ip == "::1")
                {
                    ip = "127.0.0.1";
                }
                Session["IPBase"] = ip;
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "LOGIN", "ControleAcesso", "Login", ip);

                // Trata cookie
                if (cookie == 1)
                {
                    Boolean cook = CookieManager.VerificarValidadeCookie();
                    if (!cook)
                    {
                        CookieManager.GravarCookieInicioBase();
                    }

                }

                // Verifica pagamentos
                Session["PagamentoAtraso"] = 0;
                List<ASSINANTE_PAGAMENTO> pags = assinante.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_DT_VENCIMENTO > DateTime.Today.Date.AddDays(-10) & p.ASPA_IN_PAGO == 0 & p.ASPA_IN_SISTEMA == 6).ToList();
                if (pags.Count > 0)
                {
                    Session["PagamentoAtraso"] = 1;
                    Session["MensEmpresa"] = 700;
                    Session["NivelEmpresa"] = 4;
                    Session["FrasePagtoAtraso"] = "Sua assinatura tem " + pags.Count.ToString() + " pagamento(s) vencido(s) em aberto. Favor efetuar o pagamento e informa-lo diretamente nesta mesma página para evitar interrupções no uso.";
                    return RedirectToAction("MontarTelaEmpresa", "Empresa");
                }
                // Verifica pagamentos
                Session["PagamentoAtraso"] = 0;
                pags = assinante.ASSINANTE_PAGAMENTO.Where(p => p.ASPA_DT_VENCIMENTO <= DateTime.Today.Date.AddDays(-10) & p.ASPA_IN_PAGO == 0 & p.ASPA_IN_SISTEMA == 6).ToList();
                if (pags.Count > 0)
                {
                    TempData["MostrarModalAtraso"] = true;
                    return RedirectToAction("Login", "ControleAcesso");
                }

                // Route
                Session["MensSenha"] = 0;
                if (volta == 30)
                {
                    Session["MensagemLogin"] = 23;
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0262", CultureInfo.CurrentCulture));
                    return RedirectToAction("TrocarSenhaCodigo", "ControleAcesso");
                }
                if (usuario.USUA_IN_PROVISORIO == 1)
                {
                    Session["MensSenha"] = 22;
                    return RedirectToAction("TrocarSenhaInicio", "ControleAcesso");
                }
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    return RedirectToAction("MontarTelaPaciente", "Paciente");
                }
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("Login", "ControleAcesso");
            }
        }

        public ActionResult Logout()
        {
            //if (Session["IdLoginSessao"] != null)
            //{
            //    // Grava data/hora do logout
            //    USUARIO_LOGIN loginAtual = usuApp.GetLoginById((Int32)Session["IdLoginSessao"]);
            //    DateTime inicio = loginAtual.USLO_DT_LOGIN;
            //    DateTime final = DateTime.Now;
            //    TimeSpan difference = final - inicio;
            //    double hoursDifference = Math.Floor(difference.TotalHours);
            //    double minutesDifference = difference.Minutes;
            //    String duracao = hoursDifference.ToString() + " hora(s) e " + minutesDifference.ToString() + " minuto(s)";

            //    USUARIO_LOGIN novo = new USUARIO_LOGIN();
            //    novo.USLO_CD_ID = loginAtual.USLO_CD_ID;
            //    novo.USLO_DT_LOGIN = loginAtual.USLO_DT_LOGIN;
            //    novo.USLO_IN_ATIVO = loginAtual.USLO_IN_ATIVO;
            //    novo.USLO_IN_SISTEMA = loginAtual.USLO_IN_SISTEMA;
            //    novo.USUA_CD_ID = loginAtual.USUA_CD_ID;
            //    novo.ASSI_CD_ID = loginAtual.ASSI_CD_ID;
            //    novo.USLO_DT_LOGOUT = final;
            //    novo.USLO_TM_DURACAO = duracao;
            //    Int32 voltaLog = baseApp.ValidateEditLogin(novo);

            //    // Grava Acesso
            //    ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
            //    Int32 voltaX = grava.GravaAcesso(loginAtual.USUA_CD_ID, loginAtual.ASSI_CD_ID, "LOGOUT", "ControleAcesso", "Login");
            //}

            // Grava flags de saida
            Session.Clear();
            Session["TemCookie"] = 0;
            Boolean cook = CookieManager.VerificarValidadeCookie();
            if (cook)
            {
                Session["TemCookie"] = 1;
            }
            return RedirectToAction("Login", "ControleAcesso");
        }

        public ActionResult SairWebDoctor()
        {
            // Grava flags de saida
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Session["IdLoginSessao"] = null;
            Session["MensagemLogin"] = 99;
            Session["UserCredentials"] = null;
            Session["MensEnvioLogin"] = 0;
            Session["Close"] = false;
            Session["MensagemLogin"] = null;
            Session["Versao1"] = "../BaseAdmin/Versao/Versao_1_2_0_0.pdf";
            Session["Versao2"] = "../BaseAdmin/Versao/Versao_1_2_1_0.pdf";
            Session["Versao3"] = "../BaseAdmin/Versao/Versao_1_2_2_0.pdf";
            Session["Versao4"] = "../BaseAdmin/Versao/Versao_2_0_0_0.pdf";
            Session["eDemo"] = 0;
            Session["DemoVencido"] = 0;
            Session["PagVencido"] = 0;
            Session["AssinantePendente"] = 0;
            //return RedirectToAction("Login", "ControleAcesso");
            return RedirectToAction("CarregarLandingPage", "BaseAdmin");
        }

        //[System.Web.Services.WebMethod]
        //public void LogSessionTime()
        //{
        //    // Grava data/hora do logout
        //    if (Session["IdLoginSessao"] != null)
        //    {
        //        USUARIO_LOGIN loginAtual = usuApp.GetLoginById((Int32)Session["IdLoginSessao"]);
        //        if (loginAtual != null)
        //        {
        //            DateTime inicio = loginAtual.USLO_DT_LOGIN;
        //            DateTime final = DateTime.Now;
        //            TimeSpan difference = final - inicio;
        //            double hoursDifference = Math.Floor(difference.TotalHours);
        //            double minutesDifference = difference.Minutes;
        //            String duracao = hoursDifference.ToString() + " hora(s) e " + minutesDifference.ToString() + " minuto(s)";

        //            USUARIO_LOGIN novo = new USUARIO_LOGIN();
        //            novo.USLO_CD_ID = loginAtual.USLO_CD_ID;
        //            novo.USLO_DT_LOGIN = loginAtual.USLO_DT_LOGIN;
        //            novo.USLO_IN_ATIVO = loginAtual.USLO_IN_ATIVO;
        //            novo.USLO_IN_SISTEMA = loginAtual.USLO_IN_SISTEMA;
        //            novo.USUA_CD_ID = loginAtual.USUA_CD_ID;
        //            novo.ASSI_CD_ID = loginAtual.ASSI_CD_ID;
        //            novo.USLO_DT_LOGOUT = final;
        //            novo.USLO_TM_DURACAO = duracao;
        //            novo.USLO_TM_DURACAO_SPAN = difference;
        //            novo.USLO_NM_MODULO = (String)Session["ModuloAtual"];
        //            Int32 voltaLog = baseApp.ValidateEditLogin(novo);
        //        }
        //    }
        //}

        public ActionResult Cancelar()
        {
            return RedirectToAction("Logout", "ControleAcesso");
        }

        [HttpGet]
        public ActionResult TrocarSenha()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();
                if ((USUARIO)Session["UserCredentials"] != null)
                {
                    usuario = (USUARIO)Session["UserCredentials"];
                }
                else
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                Session["ModuloAtual"] = "Senha - Alteração";

                // Grava Acesso
                ControleAcessoMetodo grava = new ControleAcessoMetodo(aceApp);
                Int32 voltaX = grava.GravaAcesso(usuario.USUA_CD_ID, usuario.ASSI_CD_ID, "TROCASENHA_IN", "ControleAcesso", "TrocarSenha");

                // Reseta senhas
                USUARIO usu = new USUARIO();
                usu.ASSI_CD_ID = usuario.ASSI_CD_ID;
                usu.USUA_NM_NOVA_SENHA = null;
                usu.USUA_NM_SENHA_CONFIRMA = null;
                usu.USUA_NM_LOGIN = null;
                UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usu);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }


                // Checa credenciais e atualiza acessos
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = baseApp.ValidateChangePasswordInterno(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0657", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0664", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0665", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0666", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0667", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0660", CultureInfo.CurrentCulture) + ". Tamanho mínimo: " + conf.CONF_NR_TAMANHO_SENHA.ToString() + " caracteres.";
                    ModelState.AddModelError("", frase);
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0668", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Retorno
                Session["MensSenha"] = 10;
                Session["MensagemLogin"] = 100; 
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [ValidateInput(false)]
        public async Task<Int32> ProcessaEnvioEMailSenha(MensagemViewModel vm, USUARIO usuario, Int32 tipo)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return 0;
                }

                // Recupera usuario
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Processa e-mail
                CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
                String header = null;
                String body = null;
                String data = null;

                if (tipo == 1)
                {
                    // Recupera template e-mail
                    header = temApp.GetByCode("TROCASENHA").TEMP_TX_CABECALHO;
                    body = temApp.GetByCode("TROCASENHA").TEMP_TX_CORPO;
                    data = temApp.GetByCode("TROCASENHA").TEMP_TX_DADOS;

                    // Prepara dados do e-mail  
                    header = header.Replace("{nome}", usuario.USUA_NM_NOME);
                    body = body.Replace("{codigo}", vm.MODELO);
                }
                else
                {
                    // Recupera template e-mail
                    header = temApp.GetByCode("CONFSENHA").TEMP_TX_CABECALHO;
                    body = temApp.GetByCode("CONFSENHA").TEMP_TX_CORPO;
                    data = temApp.GetByCode("CONFSENHA").TEMP_TX_DADOS;

                    // Prepara dados do e-mail  
                    header = header.Replace("{nome}", usuario.USUA_NM_NOME);
                }

                String status = "Succeeded";
                String iD = "xyz";
                String erro = null;
                
                // Concatena
                String emailBody = header + body + data;

                // Decriptografa chaves
                String emissor = CrossCutting.Cryptography.Decrypt(conf.CONF_NM_EMISSOR_AZURE_CRIP);
                String conn = CrossCutting.Cryptography.Decrypt(conf.CONF_CS_CONNECTION_STRING_AZURE_CRIP);

                // Monta e-mail
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Troca de Senha";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = usuario.USUA_NM_EMAIL;
                mensagem.NOME_EMISSOR_AZURE = emissor;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "WebDoctor";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conn;

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    Session["TipoVolta"] = 2;
                    Session["VoltaExcecao"] = "Acesso";
                    Session["Excecao"] = ex;
                    Session["ExcecaoTipo"] = ex.GetType().ToString();
                    GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                    Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                    return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return 0;
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigo()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Verifica se tem usuario logado
                USUARIO usuario = new USUARIO();

                // Mensagens
                if (Session["MensSenha"] != null)
                {
                    if ((Int32)Session["MensSenha"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0235", CultureInfo.CurrentCulture) + usuario.USUA_NM_EMAIL);
                    }
                    if ((Int32)Session["MensSenha"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0261", CultureInfo.CurrentCulture));
                        return RedirectToAction("Logout", "ControleAcesso");
                    }
                }

                // Monta view
                Session["MensSenha"] = 0;
                USUARIO usu = (USUARIO)Session["UserCredentials"];
                usu.USUA_NR_MATRICULA = String.Empty;
                return View(usu);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaCodigo(USUARIO vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Sanitização
                vm.USUA_SG_CODIGO = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_SG_CODIGO);

                // Valida codigo
                CONFIGURACAO conf = confApp.GetItemById(vm.ASSI_CD_ID);
                String codigo = vm.USUA_SG_CODIGO;
                if (vm.USUA_NR_MATRICULA == null)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (vm.USUA_NR_MATRICULA != codigo.Trim())
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                DateTime limite = vm.USUA_DT_CODIGO.Value.AddMinutes(conf.CONF_IN_VALIDADE_CODIGO.Value);
                if (limite < DateTime.Now)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0663", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Processa
                USUARIO usuario = (USUARIO)Session["UserCredentials"];
                Int32 volta = await baseApp.ValidateChangePasswordFinal(vm);

                // Gera mensagem
                MensagemViewModel mens = new MensagemViewModel();
                mens.NOME = vm.USUA_NM_NOME;
                mens.ID = vm.USUA_CD_ID;
                mens.MODELO = vm.USUA_NM_EMAIL;
                mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                mens.MENS_IN_TIPO = 1;
                mens.MODELO = codigo;
                Int32 xx = await ProcessaEnvioEMailSenha(mens, vm, 2);

                // Retorno
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult GerarSenha()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Mensagens
                if (Session["MensSenha"] != null)
                {
                    // Mensagens
                    if ((Int32)Session["MensSenha"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0096", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    }
                }

                // Abre tela
                USUARIO item = new USUARIO();
                UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }


        [HttpPost]
        public async Task<ActionResult> GerarSenha(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);

                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = await baseApp.GenerateNewPassword(item.USUA_NM_EMAIL);
                if (volta != 0)
                {
                    Session["MensSenha"] = volta;
                    return RedirectToAction("GerarSenha", "ControleAcesso");
                }
                Session["MensagemLogin"] = 55;

                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaInicio()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Reseta senhas
                USUARIO usu = new USUARIO();
                usu.USUA_NM_NOVA_SENHA = null;
                usu.USUA_NM_SENHA_CONFIRMA = null;
                usu.USUA_NM_LOGIN = null;
                UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usu);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Checa credenciais e atualiza acessos
                CONFIGURACAO conf = CarregaConfiguracaoGeral();
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = await baseApp.ValidateChangePassword(item);
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0657", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0664", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0665", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0666", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0667", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 9)
                {
                    String frase = CRMSys_Base.ResourceManager.GetString("M0660", CultureInfo.CurrentCulture) + ". Tamanho mínimo: " + conf.CONF_NR_TAMANHO_SENHA.ToString() + " caracteres.";
                    ModelState.AddModelError("", frase);
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0668", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 33)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0738", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Retorno
                Session["MensSenha"] = 10;
                return RedirectToAction("TrocarSenhaCodigoInicio", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpGet]
        public ActionResult TrocarSenhaCodigoInicio()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Mensagens
                if ((Int32)Session["MensSenha"] == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0234", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0233", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSenha"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0663", CultureInfo.CurrentCulture));
                }

                // Monta view
                Session["MensSenha"] = 0;
                USUARIO usu = new USUARIO();
                usu.USUA_SG_CODIGO = String.Empty;
                UsuarioLoginViewModel vm = Mapper.Map<USUARIO, UsuarioLoginViewModel>(usu);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TrocarSenhaCodigoInicio(UsuarioLoginViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }
                // Sanitização
                vm.USUA_NR_MATRICULA = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_MATRICULA);

                // Processa
                USUARIO item = Mapper.Map<UsuarioLoginViewModel, USUARIO>(vm);
                Int32 volta = await baseApp.ValidateChangePasswordFinal(item);

                // mensagens
                if (volta == 1)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0008", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0657", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0664", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0665", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 7)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0666", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 8)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0667", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 10)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 11)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0661", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 12)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0668", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 13)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 14)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 15)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 16)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0669", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 17)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0670", CultureInfo.CurrentCulture));
                    return View(vm);
                }
                if (volta == 18)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0663", CultureInfo.CurrentCulture));
                    return View(vm);
                }

                // Retorno
                Session["MensagemLogin"] = 65;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
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
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        public CONFIGURACAO CarregaConfiguracaoGeralCache()
        {
            try
            {
                Int32 idAss = (Int32)Session["IdAssinante"];
                String id = "CONF";
                CacheService cache = CacheService.GetInstance();
                CONFIGURACAO objeto = new CONFIGURACAO();

                var cacheItem = cache.Get(id);
                if (cacheItem == null)
                {
                    objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                    cache.Add(id, objeto);
                }
                else
                {
                    if ((Int32)Session["ConfAlterada"] == 1)
                    {
                        objeto = confApp.GetAllItems(idAss).FirstOrDefault();
                        cache.AddOrUpdate(id, objeto);
                    }
                    else
                    {
                        objeto = (CONFIGURACAO)cacheItem;
                    }
                }
                Session["ConfAlterada"] = 0;
                return objeto;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(usuApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return null;
            }
        }

        [HttpGet]
        public ActionResult GerarNovaSenha()
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Mensagens
                if (Session["MensSenha"] != null)
                {
                    // Mensagens
                    if ((Int32)Session["MensSenha"] == 1)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0001", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 2)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0657", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 3)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0003", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 4)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0004", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 5)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0653", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 7)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0658", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 8)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0654", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 9)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0655", CultureInfo.CurrentCulture));
                    }
                    if ((Int32)Session["MensSenha"] == 10)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0656", CultureInfo.CurrentCulture));
                    }
                }

                // Abre tela
                USUARIO item = new USUARIO();
                UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(item);
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GerarNovaSenha(UsuarioViewModel vm)
        {
            try
            {
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Logout", "ControleAcesso");
                }

                // Sanitização
                vm.USUA_NM_EMAIL = CrossCutting.UtilitariosGeral.CleanStringMail(vm.USUA_NM_EMAIL);
                vm.USUA_NM_LOGIN = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NM_LOGIN);
                vm.USUA_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_CPF);
                vm.USUA_NR_TELEFONE = CrossCutting.UtilitariosGeral.CleanStringDocto(vm.USUA_NR_TELEFONE);

                // Processa
                Session["UserCredentials"] = null;
                USUARIO item = Mapper.Map<UsuarioViewModel, USUARIO>(vm);
                Int32 volta = await baseApp.GerarNovaSenha(item);
                if (volta != 0)
                {
                    Session["MensSenha"] = volta;
                    return RedirectToAction("GerarSenha", "ControleAcesso");
                }
                Session["MensagemLogin"] = 55;
                return RedirectToAction("Login", "ControleAcesso");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["TipoVolta"] = 2;
                Session["VoltaExcecao"] = "Acesso";
                Session["Excecao"] = ex;
                Session["ExcecaoTipo"] = ex.GetType().ToString();
                GravaLogExcecao grava = new GravaLogExcecao(baseApp);
                Int32 voltaX = grava.GravarLogExcecao(ex, "Acesso", "WebDoctor", 1, (USUARIO)Session["UserCredentials"]);
                return RedirectToAction("TrataExcecao", "BaseAdmin");
            }
        }

    }
}