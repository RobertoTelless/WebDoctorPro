using System;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<PLANO, ValorConsultaViewModel>();
            CreateMap<ASSINANTE, AssinanteViewModel>();
            CreateMap<ASSINANTE_PAGAMENTO, AssinantePagamentoViewModel>();
            CreateMap<ASSINANTE_ANOTACAO, AssinanteAnotacaoViewModel>();
            CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            CreateMap<TEMPLATE_EMAIL, TemplateEMailViewModel>();
            CreateMap<ASSINANTE_PLANO, AssinantePlanoViewModel>();
            CreateMap<MENSAGENS_ENVIADAS_SISTEMA, MensagemEmitidaViewModel>();
            CreateMap<EMPRESA, EmpresaViewModel>();
            CreateMap<RECURSIVIDADE, RecursividadeViewModel>();
            CreateMap<PERFIL, PerfilViewModel>();
            CreateMap<MENSAGENS, MensagemViewModel>();

            CreateMap<TIPO_PACIENTE, TipoPacienteViewModel>();
            CreateMap<CONVENIO, ConvenioViewModel>();
            CreateMap<TIPO_EXAME, TipoExameViewModel>();
            CreateMap<PACIENTE_ANOTACAO, PacienteAnotacaoViewModel>();
            CreateMap<PACIENTE_CONSULTA, PacienteConsultaViewModel>();
            CreateMap<PACIENTE_ANAMNESE, PacienteAnamneseViewModel>();
            CreateMap<PACIENTE_PRESCRICAO, PacientePrescricaoViewModel>();
            CreateMap<PACIENTE_EXAMES, PacienteExameViewModel>();
            CreateMap<PACIENTE_EXAME_FISICOS, PacienteExameFisicoViewModel>();
            CreateMap<TIPO_ATESTADO, TipoAtestadoViewModel>();
            CreateMap<PACIENTE_SOLICITACAO, PacienteSolicitacaoViewModel>();
            CreateMap<PACIENTE_ATESTADO, PacienteAtestadoViewModel>();
            CreateMap<GRUPO_PAC, GrupoViewModel>();
            CreateMap<GRUPO_PACIENTE, GrupoContatoViewModel>();
            CreateMap<PACIENTE_CONTATO, PacienteContatoViewModel>();
            CreateMap<PACIENTE_EXAME_ANOTACAO, PacienteExameAnotacaoViewModel>();
            CreateMap<PACIENTE, PacienteViewModel>();
            CreateMap<PACIENTE_PRESCRICAO_ITEM, PacientePrescricaoItemViewModel>();
            CreateMap<LABORATORIO, LaboratorioViewModel>();
            CreateMap<TEMPLATE_SMS, TemplateSMSViewModel>();
            CreateMap<MEDICAMENTO, MedicamentoBaseViewModel>();
            CreateMap<CONFIGURACAO_CALENDARIO, ConfiguracaoCalendarioViewModel>();
            CreateMap<CONFIGURACAO_CALENDARIO_BLOQUEIO, ConfiguracaoCalendarioBloqueioViewModel>();
            CreateMap<CONFIGURACAO_ANAMNESE, ConfiguracaoAnamneseViewModel>();
            CreateMap<AVISO_LEMBRETE, AvisoLembreteViewModel>();
            CreateMap<SOLICITACAO, SolicitacaoViewModel>();
            CreateMap<ESPECIALIDADE, EspecialidadeViewModel>();
            CreateMap<FORMA_RECEBIMENTO, FormaRecebimentoViewModel>();
            CreateMap<TIPO_PAGAMENTO, TipoPagamentoViewModel>();
            CreateMap<TIPO_VALOR_CONSULTA, TipoValorConsultaViewModel>();
            CreateMap<TIPO_SERVICO_CONSULTA, TipoValorServicoViewModel>();
            CreateMap<VALOR_CONSULTA, ValorConsulta1ViewModel>();
            CreateMap<VALOR_SERVICO, ValorServicoViewModel>();
            CreateMap<VALOR_CONVENIO, ValorConvenioViewModel>();
            CreateMap<CONSULTA_PAGAMENTO, PagamentoViewModel>();
            CreateMap<PAGAMENTO_ANOTACAO, PagamentoAnotacaoViewModel>();
            CreateMap<CONSULTA_RECEBIMENTO, RecebimentoViewModel>();
            CreateMap<RECEBIMENTO_ANOTACAO, RecebimentoAnotacaoViewModel>();
            CreateMap<PACIENTE_ANAMNESE_ANOTACAO, PacienteAnamneseAnotacaoViewModel>();

            CreateMap<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>();
            CreateMap<PRODUTO_ANOTACAO, ProdutoAnotacaoViewModel>();
            CreateMap<PRODUTO_CONCORRENTE, ProdutoConcorrenteViewModel>();
            CreateMap<PRODUTO_CUSTO, ProdutoCustoViewModel>();
            CreateMap<PRODUTO_PRECO_VENDA, ProdutoPrecoVendaViewModel>();
            CreateMap<PRODUTO, ProdutoViewModel>();
            CreateMap<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>();
            CreateMap<UNIDADE, UnidadeViewModel>();
            CreateMap<PRODUTO_ESTOQUE_FILIAL, ProdutoEstoqueFilialViewModel>();
            CreateMap<MOVIMENTO_ESTOQUE_PRODUTO, MovimentoEstoqueProdutoViewModel>();

            CreateMap<INDICACAO, IndicacaoViewModel>();
            CreateMap<INDICACAO_ACAO, IndicacaoAcaoViewModel>();
            CreateMap<INDICACAO_ANEXO, IndicacaoAnexoViewModel>();

            CreateMap<MEDICOS, MedicoViewModel>();
            CreateMap<MEDICOS_ENVIO, MedicoEnvioViewModel>();
            CreateMap<MEDICOS_ENVIO_ANEXO, MedicoAnexoViewModel>();
            CreateMap<MEDICOS_ENVIO_ANOTACAO, MedicoAnotacaoViewModel>();
            CreateMap<VIDEO_BASE, VideoViewModel>();

            CreateMap<QUESTIONARIO_BERLIM, QuestionarioBerlimViewModel>();
            CreateMap<QUESTIONARIO_EPWORTH, QuestionarioEpworthViewModel>();
            CreateMap<QUESTIONARIO_STOPBANG, QuestionarioBangViewModel>();

            CreateMap<TIPO_HISTORICO, TipoHistoricoViewModel>();
            CreateMap<LOCACAO, LocacaoViewModel>();
            CreateMap<LOCACAO_PARCELA, LocacaoParcelaViewModel>();
            CreateMap<LOCACAO_ANOTACAO, LocacaoAnotacaoViewModel>();
            CreateMap<LOCACAO_OCORRENCIA, LocacaoOcorrenciaViewModel>();
            CreateMap<CONTRATO_LOCACAO, ContratoLocacaoViewModel>();
            CreateMap<MEDICOS_MENSAGEM, MedicoMensagemViewModel>();
            CreateMap<RESPOSTA_CONSULTA, PacienteRespostaViewModel>();
            CreateMap<AREA_PACIENTE, AreaPacienteViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();

        }
    }
}
