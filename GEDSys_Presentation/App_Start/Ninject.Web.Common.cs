using System;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;
using ERP_Condominios_Solution.ViewModels;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<ITemplateEMailAppService>().To<TemplateEMailAppService>();
            kernel.Bind<ITipoPessoaAppService>().To<TipoPessoaAppService>();
            kernel.Bind<IEMailAgendaAppService>().To<EMailAgendaAppService>();
            kernel.Bind<IAssinanteCnpjAppService>().To<AssinanteCnpjAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<IMensagemEnviadaSistemaAppService>().To<MensagemEnviadaSistemaAppService>();
            kernel.Bind<IEmpresaAppService>().To<EmpresaAppService>();
            kernel.Bind<IRecursividadeAppService>().To<RecursividadeAppService>();
            kernel.Bind<IControleMensagemAppService>().To<ControleMensagemAppService>();
            kernel.Bind<IPerfilAppService>().To<PerfilAppService>();
            kernel.Bind<ITemplateEMailHTMLAppService>().To<TemplateEMailHTMLAppService>();
            kernel.Bind<IMensagemAppService>().To<MensagemAppService>();
            kernel.Bind<IMensagemAutomacaoAppService>().To<MensagemAutomacaoAppService>();
            kernel.Bind<ITipoPacienteAppService>().To<TipoPacienteAppService>();
            kernel.Bind<IConvenioAppService>().To<ConvenioAppService>();
            kernel.Bind<ITipoExameAppService>().To<TipoExameAppService>();
            kernel.Bind<IPacienteAppService>().To<PacienteAppService>();
            kernel.Bind<ITipoAtestadoAppService>().To<TipoAtestadoAppService>();
            kernel.Bind<IGrupoAppService>().To<GrupoAppService>();
            kernel.Bind<ICategoriaUsuarioAppService>().To<CategoriaUsuarioAppService>();
            kernel.Bind<ILaboratorioAppService>().To<LaboratorioAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<ITemplateSMSAppService>().To<TemplateSMSAppService>();
            kernel.Bind<IMedicamentoAppService>().To<MedicamentoAppService>();
            kernel.Bind<IConfiguracaoCalendarioAppService>().To<ConfiguracaoCalendarioAppService>();
            kernel.Bind<IConfiguracaoAnamneseAppService>().To<ConfiguracaoAnamneseAppService>();
            kernel.Bind<IAvisoLembreteAppService>().To<AvisoLembreteAppService>();
            kernel.Bind<ISolicitacaoAppService>().To<SolicitacaoAppService>();
            kernel.Bind<IEspecialidadeAppService>().To<EspecialidadeAppService>();
            kernel.Bind<IAcessoMetodoAppService>().To<AcessoMetodoAppService>();
            kernel.Bind<IFormaRecebimentoAppService>().To<FormaRecebimentoAppService>();
            kernel.Bind<ITipoPagamentoAppService>().To<TipoPagamentoAppService>();
            kernel.Bind<ITipoValorConsultaAppService>().To<TipoValorConsultaAppService>();
            kernel.Bind<ITipoValorServicoAppService>().To<TipoValorServicoAppService>();
            kernel.Bind<IValorConsultaAppService>().To<ValorConsultaAppService>();
            kernel.Bind<IValorServicoAppService>().To<ValorServicoAppService>();
            kernel.Bind<IValorConvenioAppService>().To<ValorConvenioAppService>();
            kernel.Bind<IPagamentoAppService>().To<PagamentoAppService>();
            kernel.Bind<IRecebimentoAppService>().To<RecebimentoAppService>();
            kernel.Bind<ICategoriaProdutoAppService>().To<CategoriaProdutoAppService>();
            kernel.Bind<ISubcategoriaProdutoAppService>().To<SubcategoriaProdutoAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IMedicoAppService>().To<MedicoAppService>();
            kernel.Bind<IVideoAppService>().To<VideoAppService>();
            kernel.Bind<ITipoHistoricoAppService>().To<TipoHistoricoAppService>();
            kernel.Bind<ILocacaoAppService>().To<LocacaoAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<ITemplateEMailService>().To<TemplateEMailService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<IEMailAgendaService>().To<EmailAgendaService>();
            kernel.Bind<IAssinanteCnpjService>().To<AssinanteCnpjService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<IMensagemEnviadaSistemaService>().To<MensagemEnviadaSistemaService>();
            kernel.Bind<IEmpresaService>().To<EmpresaService>();
            kernel.Bind<IRecursividadeService>().To<RecursividadeService>();
            kernel.Bind<IControleMensagemService>().To<ControleMensagemService>();
            kernel.Bind<IPerfilService>().To<PerfilService>();
            kernel.Bind<ITemplateEMailHTMLService>().To<TemplateEMailHTMLService>();
            kernel.Bind<IMensagemService>().To<MensagemService>();
            kernel.Bind<IMensagemAutomacaoService>().To<MensagemAutomacaoService>();
            kernel.Bind<ITipoPacienteService>().To<TipoPacienteService>();
            kernel.Bind<IConvenioService>().To<ConvenioService>();
            kernel.Bind<ITipoExameService>().To<TipoExameService>();
            kernel.Bind<IPacienteService>().To<PacienteService>();
            kernel.Bind<ITipoAtestadoService>().To<TipoAtestadoService>();
            kernel.Bind<IGrupoService>().To<GrupoService>();
            kernel.Bind<ICategoriaUsuarioService>().To<CategoriaUsuarioService>();
            kernel.Bind<ILaboratorioService>().To<LaboratorioService>();
            kernel.Bind<ITemplateSMSService>().To<TemplateSMSService>();
            kernel.Bind<IMedicamentoService>().To<MedicamentoService>();
            kernel.Bind<IConfiguracaoCalendarioService>().To<ConfiguracaoCalendarioService>();
            kernel.Bind<IConfiguracaoAnamneseService>().To<ConfiguracaoAnamneseService>();
            kernel.Bind<IAvisoLembreteService>().To<AvisoLembreteService>();
            kernel.Bind<ISolicitacaoService>().To<SolicitacaoService>();
            kernel.Bind<IEspecialidadeService>().To<EspecialidadeService>();
            kernel.Bind<IAcessoMetodoService>().To<AcessoMetodoService>();
            kernel.Bind<IFormaRecebimentoService>().To<FormaRecebimentoService>();
            kernel.Bind<ITipoPagamentoService>().To<TipoPagamentoService>();
            kernel.Bind<ITipoValorConsultaService>().To<TipoValorConsultaService>();
            kernel.Bind<ITipoValorServicoService>().To<TipoValorServicoService>();
            kernel.Bind<IValorConsultaService>().To<ValorConsultaService>();
            kernel.Bind<IValorServicoService>().To<ValorServicoService>();
            kernel.Bind<IValorConvenioService>().To<ValorConvenioService>();
            kernel.Bind<IPagamentoService>().To<PagamentoService>();
            kernel.Bind<IRecebimentoService>().To<RecebimentoService>();
            kernel.Bind<ICategoriaProdutoService>().To<CategoriaProdutoService>();
            kernel.Bind<ISubcategoriaProdutoService>().To<SubcategoriaProdutoService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IMedicoService>().To<MedicoService>();
            kernel.Bind<IVideoService>().To<VideoService>();
            kernel.Bind<ITipoHistoricoService>().To<TipoHistoricoService>();
            kernel.Bind<ILocacaoService>().To<LocacaoService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IAssinanteAnexoRepository>().To<AssinanteAnexoRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<ITemplateEMailRepository>().To<TemplateEMailRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<IEmailAgendaRepository>().To<EMailAgendaRepository>();
            kernel.Bind<IUsuarioAnotacaoRepository>().To<UsuarioAnotacaoRepository>();
            kernel.Bind<IAssinanteAnotacaoRepository>().To<AssinanteAnotacaoRepository>();
            kernel.Bind<IAssinanteCnpjRepository>().To<AssinanteCnpjRepository>();
            kernel.Bind<IAssinantePagamentoRepository>().To<AssinantePagamentoRepository>();
            kernel.Bind<IPeriodicidadePlanoRepository>().To<PeriodicidadePlanoRepository>();
            kernel.Bind<IPlanoRepository>().To<PlanoRepository>();
            kernel.Bind<IAssinantePlanoRepository>().To<AssinantePlanoRepository>();
            kernel.Bind<IMensagemEnviadaSistemaRepository>().To<MensagemEnviadaSistemaRepository>();
            kernel.Bind<IConfiguracaoChavesRepository>().To<ConfiguracaoChavesRepository>();
            kernel.Bind<IEmpresaRepository>().To<EmpresaRepository>();
            kernel.Bind<IEmpresaAnexoRepository>().To<EmpresaAnexoRepository>();
            kernel.Bind<IRecursividadeRepository>().To<RecursividadeRepository>();
            kernel.Bind<IRecursividadeDestinoRepository>().To<RecursividadeDestinoRepository>();
            kernel.Bind<IRecursividadeDataRepository>().To<RecursividadeDataRepository>();
            kernel.Bind<INacionalidadeRepository>().To<NacionalidadeRepository>();
            kernel.Bind<IMunicipioRepository>().To<MunicipioRepository>();
            kernel.Bind<ILogExcecaoRepository>().To<LogExcecaoRepository>();
            kernel.Bind<IControleMensagemRepository>().To<ControleMensagemRepository>();
            kernel.Bind<IEmpresaFilialRepository>().To<EmpresaFilialRepository>();
            kernel.Bind<IMensagemFabricanteRepository>().To<MensagemFabricanteRepository>();
            kernel.Bind<ITemplateEMailHTMLRepository>().To<TemplateEMailHTMLRepository>();
            kernel.Bind<IMensagemDestinoRepository>().To<MensagemDestinoRepository>();
            kernel.Bind<IMensagemAutomacaoRepository>().To<MensagemAutomacaoRepository>();
            kernel.Bind<IMensagemAnexoRepository>().To<MensagemAnexoRepository>();
            kernel.Bind<IMensagemAutomacaoDatasRepository>().To<MensagemAutomacaoDatasRepository>();
            kernel.Bind<IMensagemRepository>().To<MensagemRepository>();
            kernel.Bind<IResultadoRobotRepository>().To<ResultadoRobotRepository>();
            kernel.Bind<IUsuarioLoginRepository>().To<UsuarioLoginRepository>();
            kernel.Bind<ICategoriaUsuarioRepository>().To<CategoriaUsuarioRepository>();
            kernel.Bind<ITemplateSMSRepository>().To<TemplateSMSRepository>();

            kernel.Bind<ITipoPacienteRepository>().To<TipoPacienteRepository>();
            kernel.Bind<ICorRepository>().To<CorRepository>();
            kernel.Bind<IEstadoCivilRepository>().To<EstadoCivilRepository>();
            kernel.Bind<IConvenioRepository>().To<ConvenioRepository>();
            kernel.Bind<IGrauRepository>().To<GrauRepository>();
            kernel.Bind<ITipoExameRepository>().To<TipoExameRepository>();
            kernel.Bind<ILinguaRepository>().To<LinguaRepository>();
            kernel.Bind<IPacienteConsultaRepository>().To<PacienteConsultaRepository>();
            kernel.Bind<IPacientePrescricaoRepository>().To<PacientePrescricaoRepository>();
            kernel.Bind<ITipoControleRepository>().To<TipoControleRepository>();
            kernel.Bind<IPacienteAnexoRepository>().To<PacienteAnexoRepository>();
            kernel.Bind<IPacienteAnotacaoRepository>().To<PacienteAnotacaoRepository>();
            kernel.Bind<IPacienteAnamneseRepository>().To<PacienteAnamneseRepository>();
            kernel.Bind<IPacienteExamesRepository>().To<PacienteExamesRepository>();
            kernel.Bind<IPacienteExameFisicoRepository>().To<PacienteExameFisicoRepository>();
            kernel.Bind<ITipoAtestadoRepository>().To<TipoAtestadoRepository>();
            kernel.Bind<IPacienteAtestadoRepository>().To<PacienteAtestadoRepository>();
            kernel.Bind<IPacienteSolicitacaoRepository>().To<PacienteSolicitacaoRepository>();
            kernel.Bind<IGrauParentescoRepository>().To<GrauParentescoRepository>();
            kernel.Bind<IPacienteContatoRepository>().To<PacienteContatoRepository>();
            kernel.Bind<IGrupoRepository>().To<GrupoRepository>();
            kernel.Bind<IGrupoContatoRepository>().To<GrupoContatoRepository>();
            kernel.Bind<IPacienteExameAnotacaoRepository>().To<PacienteExameAnotacaoRepository>();
            kernel.Bind<IPacienteExameAnexoRepository>().To<PacienteExameAnexoRepository>();
            kernel.Bind<ITipoClasseRepository>().To<TipoClasseRepository>();
            kernel.Bind<IPacienteRepository>().To<PacienteRepository>();
            kernel.Bind<ITipoFormaRepository>().To<TipoFormaRepository>();
            kernel.Bind<IPacientePrescricaoItemRepository>().To<PacientePrescricaoItemRepository>();
            kernel.Bind<IPacienteHistoricoRepository>().To<PacienteHistoricoRepository>();
            kernel.Bind<ILaboratorioRepository>().To<LaboratorioRepository>();
            kernel.Bind<IMedicamentoRepository>().To<MedicamentoRepository>();
            kernel.Bind<IEspecialidadeRepository>().To<EspecialidadeRepository>();
            kernel.Bind<IConfiguracaoCalendarioRepository>().To<ConfiguracaoCalendarioRepository>();
            kernel.Bind<IConfiguracaoCalendarioBloqueioRepository>().To<ConfiguracaoCalendarioBloqueioRepository>();
            kernel.Bind<IConfiguracaoAnamneseRepository>().To<ConfiguracaoAnamneseRepository>();
            kernel.Bind<IAvisoLembreteRepository>().To<AvisoLembreteRepository>();
            kernel.Bind<ISolicitacaoRepository>().To<SolicitacaoRepository>();
            kernel.Bind<IAcessoMetodoRepository>().To<AcessoMetodoRepository>();
            kernel.Bind<IFormaRecebimentoRepository>().To<FormaRecebimentoRepository>();
            kernel.Bind<ITipoPagamentoRepository>().To<TipoPagamentoRepository>();
            kernel.Bind<ITipoValorConsultaRepository>().To<TipoValorConsultaRepository>();
            kernel.Bind<ITipoValorServicoRepository>().To<TipoValorServicoRepository>();
            kernel.Bind<IValorConsultaRepository>().To<ValorConsultaRepository>();
            kernel.Bind<IValorServicoRepository>().To<ValorServicoRepository>();
            kernel.Bind<IValorConvenioRepository>().To<ValorConvenioRepository>();
            kernel.Bind<IPagamentoRepository>().To<PagamentoRepository>();
            kernel.Bind<IPagamentoAnexoRepository>().To<PagamentoAnexoRepository>();
            kernel.Bind<IPagamentoAnotacaoRepository>().To<PagamentoAnotacaoRepository>();
            kernel.Bind<IRecebimentoRepository>().To<RecebimentoRepository>();
            kernel.Bind<IRecebimentoAnexoRepository>().To<RecebimentoAnexoRepository>();
            kernel.Bind<IRecebimentoAnotacaoRepository>().To<RecebimentoAnotacaoRepository>();
            kernel.Bind<IControleVersaoRepository>().To<ControleVersaoRepository>();
            kernel.Bind<IPacienteFichaRepository>().To<PacienteFichaRepository>();
            kernel.Bind<IPagamentoNotaRepository>().To<PagamentoNotaRepository>();
            kernel.Bind<IRecebimentoReciboRepository>().To<RecebimentoReciboRepository>();
            kernel.Bind<IPacienteAnamneseAnotacaoRepository>().To<PacienteAnamneseAnotacaoRepository>();
            kernel.Bind<IAssinantePlanoAssinaturaRepository>().To<AssinantePlanoAssinaturaRepository>();
            kernel.Bind<IPlanoAssinaturaRepository>().To<PlanoAssinaturaRepository>();
            kernel.Bind<IPacienteLoginRepository>().To<PacienteLoginRepository>();

            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<IMovimentacaoEstoqueRepository>().To<MovimentacaoEstoqueRepository>();
            kernel.Bind<IMovimentoAnotacaoRepository>().To<MovimentoAnotacaoRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            kernel.Bind<IProdutoAnotacaoRepository>().To<ProdutoAnotacaoRepository>();
            kernel.Bind<IProdutoConcorrenteRepository>().To<ProdutoConcorrenteRepository>();
            kernel.Bind<IProdutoCustoRepository>().To<ProdutoCustoRepository>();
            kernel.Bind<IProdutoEstoqueFilialRepository>().To<ProdutoEstoqueFilialRepository>();
            kernel.Bind<IProdutoEstoqueHistoricoRepository>().To<ProdutoEstoqueHistoricoRepository>();
            kernel.Bind<IProdutoLogRepository>().To<ProdutoLogRepository>();
            kernel.Bind<IProdutoPrecoVendaRepository>().To<ProdutoPrecoVendaRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IProdutoFalhaRepository>().To<ProdutoFalhaRepository>();
            kernel.Bind<IPacienteDadosExameFisicoRepository>().To<PacienteDadosExameFisicoRepository>();
            kernel.Bind<IIndicacaoRepository>().To<IndicacaoRepository>();
            kernel.Bind<IIndicacaoAcaoRepository>().To<IndicacaoAcaoRepository>();
            kernel.Bind<IIndicacaoAnexoRepository>().To<IndicacaoAnexoRepository>();
            kernel.Bind<IMedicoRepository>().To<MedicoRepository>();
            kernel.Bind<IMedicoAnexoRepository>().To<MedicoAnexoRepository>();
            kernel.Bind<IMedicoEnvioRepository>().To<MedicoEnvioRepository>();
            kernel.Bind<IMedicoAnotacaoRepository>().To<MedicoAnotacaoRepository>();
            kernel.Bind<ITipoEnvioRepository>().To<TipoEnvioRepository>();
            kernel.Bind<IVideoRepository>().To<VideoRepository>();
            kernel.Bind<ITipoVideoRepository>().To<TipoVideoRepository>();
            kernel.Bind<IQuestionarioBerlimRepository>().To<QuestionarioBerlimRepository>();
            kernel.Bind<IQuestionarioEpworthRepository>().To<QuestionarioEpworthRepository>();
            kernel.Bind<IQuestionarioBangRepository>().To<QuestionarioBangRepository>();
            kernel.Bind<ITipoHistoricoRepository>().To<TipoHistoricoRepository>();
            kernel.Bind<ILocacaoRepository>().To<LocacaoRepository>();
            kernel.Bind<ILocacaoAnexoRepository>().To<LocacaoAnexoRepository>();
            kernel.Bind<ILocacaoAnotacaoRepository>().To<LocacaoAnotacaoRepository>();
            kernel.Bind<ILocacaoParcelaRepository>().To<LocacaoParcelaRepository>();
            kernel.Bind<ILocacaoHistoricoRepository>().To<LocacaoHistoricoRepository>();
            kernel.Bind<ITipoOcorrenciaRepository>().To<TipoOcorrenciaRepository>();
            kernel.Bind<ILocacaoOcorrenciaRepository>().To<LocacaoOcorrenciaRepository>();
            kernel.Bind<ITipoContratoRepository>().To<TipoContratoRepository>();
            kernel.Bind<IContratoLocacaoRepository>().To<ContratoLocacaoRepository>();
            kernel.Bind<IMedicoMensagemRepository>().To<MedicoMensagemRepository>();


        }
    }
}