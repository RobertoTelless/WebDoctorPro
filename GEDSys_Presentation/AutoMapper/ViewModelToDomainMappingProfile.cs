using System;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<AssinanteViewModel, ASSINANTE>();
            CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            CreateMap<TemplateEMailViewModel, TEMPLATE_EMAIL>();
            CreateMap<AssinanteAnotacaoViewModel, ASSINANTE_ANOTACAO>();
            CreateMap<AssinantePagamentoViewModel, ASSINANTE_PAGAMENTO>();
            CreateMap<ValorConsultaViewModel, PLANO>();
            CreateMap<AssinantePlanoViewModel, ASSINANTE_PLANO>();
            CreateMap<MensagemEmitidaViewModel, MENSAGENS_ENVIADAS_SISTEMA>();
            CreateMap<EmpresaViewModel, EMPRESA>();
            CreateMap<RecursividadeViewModel, RECURSIVIDADE>();
            CreateMap<MensagemViewModel, MENSAGENS>();
            CreateMap<PerfilViewModel, PERFIL>();

            CreateMap<TipoPacienteViewModel, TIPO_PACIENTE>();
            CreateMap<ConvenioViewModel, CONVENIO>();
            CreateMap<TipoExameViewModel, TIPO_EXAME>();
            CreateMap<PacienteAnotacaoViewModel, PACIENTE_ANOTACAO>();
            CreateMap<PacienteConsultaViewModel, PACIENTE_CONSULTA>();
            CreateMap<PacienteAnamneseViewModel, PACIENTE_ANAMNESE>();
            CreateMap<PacientePrescricaoViewModel, PACIENTE_PRESCRICAO>();
            CreateMap<PacienteExameViewModel, PACIENTE_EXAMES>();
            CreateMap<PacienteExameFisicoViewModel, PACIENTE_EXAME_FISICOS>();
            CreateMap<TipoAtestadoViewModel, TIPO_ATESTADO>();
            CreateMap<PacienteAtestadoViewModel, PACIENTE_ATESTADO>();
            CreateMap<PacienteSolicitacaoViewModel, PACIENTE_SOLICITACAO>();
            CreateMap<GrupoViewModel, GRUPO_PAC>();
            CreateMap<GrupoContatoViewModel, GRUPO_PACIENTE>();
            CreateMap<PacienteContatoViewModel, PACIENTE_CONTATO>();
            CreateMap<PacienteExameAnotacaoViewModel, PACIENTE_EXAME_ANOTACAO>();
            CreateMap<PacienteViewModel, PACIENTE>();
            CreateMap<PacientePrescricaoItemViewModel, PACIENTE_PRESCRICAO_ITEM>();
            CreateMap<LaboratorioViewModel, LABORATORIO>();
            CreateMap<TemplateSMSViewModel, TEMPLATE_SMS>();
            CreateMap<MedicamentoBaseViewModel, MEDICAMENTO>();
            CreateMap<ConfiguracaoCalendarioViewModel, CONFIGURACAO_CALENDARIO>();
            CreateMap<ConfiguracaoCalendarioBloqueioViewModel, CONFIGURACAO_CALENDARIO_BLOQUEIO>();
            CreateMap<ConfiguracaoAnamneseViewModel, CONFIGURACAO_ANAMNESE>();
            CreateMap<AvisoLembreteViewModel, AVISO_LEMBRETE>();
            CreateMap<SolicitacaoViewModel, SOLICITACAO>();
            CreateMap<EspecialidadeViewModel, ESPECIALIDADE>();
            CreateMap<FormaRecebimentoViewModel, FORMA_RECEBIMENTO>();
            CreateMap<TipoPagamentoViewModel, TIPO_PAGAMENTO>();
            CreateMap<TipoValorConsultaViewModel, TIPO_VALOR_CONSULTA>();
            CreateMap<TipoValorServicoViewModel, TIPO_SERVICO_CONSULTA>();
            CreateMap<ValorConsulta1ViewModel, VALOR_CONSULTA>();
            CreateMap<ValorServicoViewModel, VALOR_SERVICO>();
            CreateMap<ValorConvenioViewModel, VALOR_CONVENIO>();
            CreateMap<PagamentoViewModel, CONSULTA_PAGAMENTO>();
            CreateMap<PagamentoAnotacaoViewModel, PAGAMENTO_ANOTACAO>();
            CreateMap<RecebimentoViewModel, CONSULTA_RECEBIMENTO>();
            CreateMap<RecebimentoAnotacaoViewModel, RECEBIMENTO_ANOTACAO>();
            CreateMap<PacienteAnamneseAnotacaoViewModel, PACIENTE_ANAMNESE_ANOTACAO>();

            CreateMap<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>();
            CreateMap<ProdutoAnotacaoViewModel, PRODUTO_ANOTACAO>();
            CreateMap<ProdutoCustoViewModel, PRODUTO_CUSTO>();
            CreateMap<ProdutoPrecoVendaViewModel, PRODUTO_PRECO_VENDA>();
            CreateMap<ProdutoViewModel, PRODUTO>();
            CreateMap<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>();
            CreateMap<UnidadeViewModel, UNIDADE>();
            CreateMap<ProdutoEstoqueFilialViewModel, PRODUTO_ESTOQUE_FILIAL>();
            CreateMap<MovimentoEstoqueProdutoViewModel, MOVIMENTO_ESTOQUE_PRODUTO>();

            CreateMap<IndicacaoViewModel, INDICACAO>();
            CreateMap<IndicacaoAcaoViewModel, INDICACAO_ACAO>();
            CreateMap<IndicacaoAnexoViewModel, INDICACAO_ANEXO>();

            CreateMap<MedicoViewModel, MEDICOS>();
            CreateMap<MedicoEnvioViewModel, MEDICOS_ENVIO>();
            CreateMap<MedicoAnexoViewModel, MEDICOS_ENVIO_ANEXO>();
            CreateMap<MedicoAnotacaoViewModel, MEDICOS_ENVIO_ANOTACAO>();
            CreateMap<VideoViewModel, VIDEO_BASE>();

            CreateMap<QuestionarioBerlimViewModel, QUESTIONARIO_BERLIM>();
            CreateMap<QuestionarioEpworthViewModel, QUESTIONARIO_EPWORTH>();
            CreateMap<QuestionarioBangViewModel, QUESTIONARIO_STOPBANG>();

            CreateMap<TipoHistoricoViewModel, TIPO_HISTORICO>();
            CreateMap<LocacaoViewModel, LOCACAO>();
            CreateMap<LocacaoParcelaViewModel, LOCACAO_PARCELA>();
            CreateMap<LocacaoAnotacaoViewModel, LOCACAO_ANOTACAO>();
            CreateMap<LocacaoOcorrenciaViewModel, LOCACAO_OCORRENCIA>();
            CreateMap<ContratoLocacaoViewModel, CONTRATO_LOCACAO>();
            CreateMap<MedicoMensagemViewModel, MEDICOS_MENSAGEM>();
            CreateMap<PacienteRespostaViewModel, RESPOSTA_CONSULTA>();
            CreateMap<AreaPacienteViewModel, AREA_PACIENTE>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<PacienteVacinaViewModel, PACIENTE_VACINA>();

        }
    }
}