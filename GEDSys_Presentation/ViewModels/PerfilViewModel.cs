using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PerfilViewModel
    {
        [Key]
        public int PERF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 e no máximo 5 caracteres.")]
        public string PERF_SG_SIGLA { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string PERF_NM_NOME { get; set; }
        public Nullable<int> PERF_IN_ATIVO { get; set; }
        public Nullable<int> PERF_IN_FIXO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CUSTO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_EDICAO_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_RELAT_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CUSTO_FIXO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_ADMIN { get; set; }
        public Nullable<int> PERF_IN_ACESSO_EMPRESA { get; set; }
        public Nullable<int> PERF_IN_EDICAO_EMPRESA { get; set; }
        public Nullable<int> PERF_IN_VINCULA_EMPRESA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_FT { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_FT { get; set; }
        public Nullable<int> PERF_IN_EDICAO_FT { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_FT { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_FT { get; set; }
        public Nullable<int> PERF_IN_ACESSO_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN_EDICAO_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN_REATIVA_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN_VINCULA_FORNECEDOR { get; set; }
        public Nullable<int> PERF_IN__ACESSO_MAQUINA { get; set; }
        public Nullable<int> PERF_IN__INCLUSAO_MAQUINA { get; set; }
        public Nullable<int> PERF_IN__EDICAO_MAQUINA { get; set; }
        public Nullable<int> PERF_IN__EXCLUSAO_MAQUINA { get; set; }
        public Nullable<int> PERF_IN__REATIVA_MAQUINA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_META { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_META { get; set; }
        public Nullable<int> PERF_IN_EDICAO_META { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_META { get; set; }
        public Nullable<int> PERF_IN_RELAT_META { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PLAT { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_PLAT { get; set; }
        public Nullable<int> PERF_IN_EDICAO_PLAT { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_PLAT { get; set; }
        public Nullable<int> PERF_IN_REATIVA_PLAT { get; set; }
        public Nullable<int> PERF_IN_ACESSO_TICKET { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_TICKET { get; set; }
        public Nullable<int> PERF_IN_EDICAO_TICKET { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_TICKET { get; set; }
        public Nullable<int> PERF_IN_REATIVA_TICKET { get; set; }
        public Nullable<int> PERF_IN_ACESSO_EMBA { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_EMBA { get; set; }
        public Nullable<int> PERF_IN_EDICAO_EMBA { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_EMBA { get; set; }
        public Nullable<int> PERF_IN_REATIVA_EMBA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_VENDA { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_VENDA { get; set; }
        public Nullable<int> PERF_IN_EDICAO_VENDA { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_VENDA { get; set; }
        public Nullable<int> PERF_IN_RELAT_VENDA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PROD { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_PROD { get; set; }
        public Nullable<int> PERF_IN_EDICAO_PROD { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_PROD { get; set; }
        public Nullable<int> PERF_IN_REATIVA_PROD { get; set; }
        public Nullable<int> PERF_IN_ACESSO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_ATUALIZA_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_EDITAR_PRODUTO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PRECI { get; set; }
        public Nullable<int> PERF_IN_NOVA_PRECI { get; set; }
        public Nullable<int> PERF_IN_ACESSO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_EDICAO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_BLOQUEIO_USUARIO { get; set; }
        public Nullable<int> PERF_IN_ALTERA_REUNIAO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_EDITAR_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CRM { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CRM { get; set; }
        public Nullable<int> PERF_IN_EDITAR_CRM { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CRM { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CRM { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_ACAO { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_ACAO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_ACAO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_TEMPLATE { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_TEMPLATE { get; set; }
        public Nullable<int> PERF_IN_EDITAR_TEMPLATE { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_TEMPLATE { get; set; }
        public Nullable<int> PERF_IN_REATIVA_TEMPLATE { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CLIENTE_CONTATO { get; set; }
        public Nullable<int> PERF_IN_EDITAR_CLIENTE_CONTATO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CLIENTE_CONTATO { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CLIENTE_CONTATO { get; set; }
        public Nullable<int> PERF_IN_FALHA_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_CANCELA_CRM { get; set; }
        public Nullable<int> PERF_IN_APROVA_CRM { get; set; }
        public Nullable<int> PERF_IN_ETAPA_CRM { get; set; }
        public Nullable<int> PERF_IN_ENCERRA_CRM { get; set; }
        public Nullable<int> PERF_IN_CONTATO_CRM { get; set; }
        public Nullable<int> PERF_IN_ACOMPANHA_CRM { get; set; }
        public Nullable<int> PERF_IN_MENSAGEM_CONTATO_CRM { get; set; }
        public Nullable<int> PERF_IN_MENSAGEM_CLIENTE { get; set; }
        public Nullable<int> PERF_IN_REATIVA_ACAO { get; set; }
        public Nullable<int> PERF_IN_ENCERRA_ACAO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_CANCELAR_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_APROVAR_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_ENVIO_PROPOSTA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_FUNIL { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_FUNIL { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_FUNIL { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_FUNIL { get; set; }
        public Nullable<int> PERF_IN_REATIVA_FUNIL { get; set; }
        public Nullable<int> PERF_IN_ACESSO_ACAO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CADASTRO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_FINANCEIRO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_BANCO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CONTA_BANCO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CENTRO_CUSTO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PAGREC { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_PAGREC { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_PAGREC { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_PAGREC { get; set; }
        public Nullable<int> PERF_IN_REATIVA_PAGREC { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CC { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CC { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_CC { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CC { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CC { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_BANCO { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_BANCO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_BANCO { get; set; }
        public Nullable<int> PERF_IN_REATIVA_BANCO { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CONTA { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_CONTA { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CONTA { get; set; }
        public Nullable<int> PERF_IN_REATIVA_CONTA { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_LANCAMENTO { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_LANCAMENTO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_LANCAMENTO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CR { get; set; }
        public Nullable<int> PERF_IN_ACESSO_CP { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CP { get; set; }
        public Nullable<int> PERF_IN_EDICAO_CP { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CP { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_CP { get; set; }
        public Nullable<int> PERF_IN_PAGAR_CP { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_CR { get; set; }
        public Nullable<int> PERF_IN_EDICAO_CR { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_CR { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_CR { get; set; }
        public Nullable<int> PERF_IN_RECEBER_CR { get; set; }
        public Nullable<int> PERF_IN_ACESSO_AUX { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_AUX { get; set; }
        public Nullable<int> PERF_IN_EDICAO_AUX { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_AUX { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_AUX { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_FILIAL { get; set; }
        public Nullable<int> PERF_IN_EDICAO_FILIAL { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_FILIAL { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_FILIAL { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> PERF_IN_ACESSO_SERVICO { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_SERVICO { get; set; }
        public Nullable<int> PERF_IN_EDICAO_SERVICO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_SERVICO { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_SERVICO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_GRUPO { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_GRUPO { get; set; }
        public Nullable<int> PERF_IN_EDICAO_GRUPO { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_GRUPO { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_GRUPO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_MENSAGEM { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_MENSAGEM { get; set; }
        public Nullable<int> PERF_IN_ACESSO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_INCLUSAO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_ALTERACAO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_EXCLUSAO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_REATIVACAO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_CANCELAR_COMPRA { get; set; }
        public Nullable<int> PERF_IN_APROVAR_COMPRA { get; set; }
        public Nullable<int> PERF_IN_ENVIO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_RECEBER_COMPRA { get; set; }
        public Nullable<int> PERF_IN_MENSAGEM_CONTATO_COMPRA { get; set; }
        public Nullable<int> PERF_IN_INCLUIR_MOVIMENTACAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_EXCLUIR_MOVIMENTACAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_COMPRA_MANUAL_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_DEVOLUCAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_ACERTO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_DESCARTE_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_MANUTENCAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_PERDA_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_TRANSFERENCIA_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_VER_MOVIMENTACAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_EDITAR_MOVIMENTACAO_ESTOQUE { get; set; }
        public Nullable<int> PERF_IN_ACESSO_REGIAO { get; set; }
        public Nullable<int> PERF_IN_INCLUIR_REGIAO { get; set; }
        public Nullable<int> PERF_IN_ALTERAR_REGIAO { get; set; }
        public Nullable<int> PERF_IN_EXCLUIR_REGIAO { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_REGIAO { get; set; }
        public Nullable<int> PERF_IN_ACESSO_TRANSPORTADORA { get; set; }
        public Nullable<int> PERF_IN_INCLUIR_TRANSPORTADORA { get; set; }
        public Nullable<int> PERF_IN_ALTERAR_TRANSPORTADORA { get; set; }
        public Nullable<int> PERF_IN_EXCLUIR_TRANSPORTADORA { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_TRANSPORTADORA { get; set; }
        public Nullable<int> PERF_IN_ACESSO_PACIENTE { get; set; }
        public Nullable<int> PERF_IN_INCLUIR_PACIENTE { get; set; }
        public Nullable<int> PERF_IN_ALTERAR_PACIENTE { get; set; }
        public Nullable<int> PERF_IN_EXCLUIR_PACIENTE { get; set; }
        public Nullable<int> PERF_IN_REATIVAR_PACIENTE { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_ATESTADO { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_SOLICITACAO { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_CONSULTA_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_CONSULTA_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_CONSULTA_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_PRESCRICAO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_PRESCRICAO_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_PRESCRICAO_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_PRESCRICAO_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_ATESTADO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_ATESTADO_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_ATESTADO_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_ATESTADO_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_SOLICITACAO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_SOLICITACAO_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_SOLICITACAO_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_SOLICITACAO_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_PACIENTE_CONSULTA_ACESSO { get; set; }
        public Nullable<int> PAERF_IN_PACIENTE_MENSAGEM { get; set; }
        public Nullable<int> PERF_IN_EXAME_ACESSO { get; set; }
        public Nullable<int> PERF_IN_EXAME_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_EXAME_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_EXAME_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_ANAMNESE_ACESSO { get; set; }
        public Nullable<int> PERF_IN_ANAMNESE_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_ANAMNESE_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_ANAMNESE_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_FISICO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_FISICO_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_FISICO_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_FISICO_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_PRESCRICAO_ENVIAR { get; set; }
        public Nullable<int> PERF_IN_ATESTADO_ENVIAR { get; set; }
        public Nullable<int> PERF_IN_SOLICITACAO_ENVIAR { get; set; }
        public Nullable<int> PERF_IN_VISAO_GERAL { get; set; }
        public Nullable<int> PERF_IN_BLOQUEIO { get; set; }
        public Nullable<int> PERF_IN_CONF_CANC_CONSULTA { get; set; }

        public Nullable<int> PERF_IN_FINANCEIRO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_PAG_ACESSO { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_REC_ACESSO { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_PAG_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_PAG_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_PAG_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_REC_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_REC_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_REC_EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_RELATORIO { get; set; }
        public Nullable<int> PERF_IN_FINANCEIRO_CONTADOR { get; set; }
        public Nullable<int> PERF_IN_ENVIAR_MEDICO { get; set; }
        public Nullable<int> PERF_IN_LOCACAO_ACESSO { get; set; }
        public Nullable<int> PERF_IN_LOCACAO_INCLUIR { get; set; }
        public Nullable<int> PERF_IN_LOCACAO_ALTERAR { get; set; }
        public Nullable<int> PERF_IN_LOCACAO__EXCLUIR { get; set; }
        public Nullable<int> PERF_IN_LOCACAO_RENOVAR { get; set; }
        public Nullable<int> PERF_IN_LOCACAO_ENCERRAR { get; set; }

        public Nullable<int> SELECIONAR_TUDO { get; set; }
        public bool SelecionarTudo
        {
            get
            {
                if (SELECIONAR_TUDO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                SELECIONAR_TUDO = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoAdmin
        {
            get
            {
                if (PERF_IN_ACESSO_ADMIN == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_ADMIN = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoEmpresa
        {
            get
            {
                if (PERF_IN_ACESSO_EMPRESA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_EMPRESA = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirFilial
        {
            get
            {
                if (PERF_IN_INCLUSAO_FILIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_FILIAL = (value == true) ? 1 : 0;
            }
        }
        public bool EditarFilial
        {
            get
            {
                if (PERF_IN_EDICAO_FILIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDICAO_FILIAL = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirFilial
        {
            get
            {
                if (PERF_IN_EXCLUSAO_FILIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_FILIAL = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarFilial
        {
            get
            {
                if (PERF_IN_REATIVACAO_FILIAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVACAO_FILIAL = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoFunil
        {
            get
            {
                if (PERF_IN_ACESSO_FUNIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_FUNIL = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirFunil
        {
            get
            {
                if (PERF_IN_INCLUSAO_FUNIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_FUNIL = (value == true) ? 1 : 0;
            }
        }
        public bool EditarFunil
        {
            get
            {
                if (PERF_IN_ALTERACAO_FUNIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERACAO_FUNIL = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirFunil
        {
            get
            {
                if (PERF_IN_EXCLUSAO_FUNIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_FUNIL = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarFunil
        {
            get
            {
                if (PERF_IN_REATIVA_FUNIL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_FUNIL = (value == true) ? 1 : 0;
            }
        }






        public bool AcessoCliente
        {
            get
            {
                if (PERF_IN_ACESSO_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirCliente
        {
            get
            {
                if (PERF_IN_INCLUSAO_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool EditarCliente
        {
            get
            {
                if (PERF_IN_EDITAR_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDITAR_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirCliente
        {
            get
            {
                if (PERF_IN_EXCLUSAO_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarCliente
        {
            get
            {
                if (PERF_IN_REATIVA_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool EditarContatoCliente
        {
            get
            {
                if (PERF_IN_EDITAR_CLIENTE_CONTATO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDITAR_CLIENTE_CONTATO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirContatoCliente
        {
            get
            {
                if (PERF_IN_INCLUSAO_CLIENTE_CONTATO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_CLIENTE_CONTATO = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirContatoCliente
        {
            get
            {
                if (PERF_IN_EXCLUSAO_CLIENTE_CONTATO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_CLIENTE_CONTATO = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarContatoCliente
        {
            get
            {
                if (PERF_IN_REATIVA_CLIENTE_CONTATO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_CLIENTE_CONTATO = (value == true) ? 1 : 0;
            }
        }
        public bool FalhaCliente
        {
            get
            {
                if (PERF_IN_FALHA_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FALHA_CLIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool MensagemCliente
        {
            get
            {
                if (PERF_IN_MENSAGEM_CLIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_MENSAGEM_CLIENTE = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoUsuario
        {
            get
            {
                if (PERF_IN_ACESSO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_USUARIO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirUsuario
        {
            get
            {
                if (PERF_IN_INCLUSAO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_USUARIO = (value == true) ? 1 : 0;
            }
        }
        public bool EditarUsuario
        {
            get
            {
                if (PERF_IN_EDICAO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDICAO_USUARIO = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirUsuario
        {
            get
            {
                if (PERF_IN_EXCLUSAO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_USUARIO = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarUsuario
        {
            get
            {
                if (PERF_IN_REATIVACAO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVACAO_USUARIO = (value == true) ? 1 : 0;
            }
        }
        public bool BLoquearUsuario
        {
            get
            {
                if (PERF_IN_BLOQUEIO_USUARIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_BLOQUEIO_USUARIO = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoTemplate
        {
            get
            {
                if (PERF_IN_ACESSO_TEMPLATE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_TEMPLATE = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirTemplate
        {
            get
            {
                if (PERF_IN_INCLUSAO_TEMPLATE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_TEMPLATE = (value == true) ? 1 : 0;
            }
        }
        public bool EditarTemplate
        {
            get
            {
                if (PERF_IN_EDITAR_TEMPLATE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDITAR_TEMPLATE = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirTemplate
        {
            get
            {
                if (PERF_IN_EXCLUSAO_TEMPLATE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_TEMPLATE = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarTemplate
        {
            get
            {
                if (PERF_IN_REATIVA_TEMPLATE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_TEMPLATE = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoServico
        {
            get
            {
                if (PERF_IN_ACESSO_SERVICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_SERVICO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirServico
        {
            get
            {
                if (PERF_IN_INCLUSAO_SERVICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_SERVICO = (value == true) ? 1 : 0;
            }
        }
        public bool EditarServico
        {
            get
            {
                if (PERF_IN_EDICAO_SERVICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDICAO_SERVICO = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirServico
        {
            get
            {
                if (PERF_IN_EXCLUSAO_SERVICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_SERVICO = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarServico
        {
            get
            {
                if (PERF_IN_REATIVACAO_SERVICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVACAO_SERVICO = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoProduto
        {
            get
            {
                if (PERF_IN_ACESSO_PROD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_PROD = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirProduto
        {
            get
            {
                if (PERF_IN_INCLUSAO_PROD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_PROD = (value == true) ? 1 : 0;
            }
        }
        public bool EditarProduto
        {
            get
            {
                if (PERF_IN_EDICAO_PROD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDICAO_PROD = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirProduto
        {
            get
            {
                if (PERF_IN_EXCLUSAO_PROD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_PROD = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarProduto
        {
            get
            {
                if (PERF_IN_REATIVA_PROD == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_PROD = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoCRM
        {
            get
            {
                if (PERF_IN_ACESSO_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirCRM
        {
            get
            {
                if (PERF_IN_EXCLUSAO_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarCRM
        {
            get
            {
                if (PERF_IN_REATIVA_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool EditarCRM
        {
            get
            {
                if (PERF_IN_EDITAR_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDITAR_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirCRM
        {
            get
            {
                if (PERF_IN_INCLUSAO_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_CRM = (value == true) ? 1 : 0;
            }
        }

        public bool CancelarCRM
        {
            get
            {
                if (PERF_IN_CANCELA_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_CANCELA_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool EtapaCRM
        {
            get
            {
                if (PERF_IN_ETAPA_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ETAPA_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool EncerrarCRM
        {
            get
            {
                if (PERF_IN_ENCERRA_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ENCERRA_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool AcompanharCRM
        {
            get
            {
                if (PERF_IN_ACOMPANHA_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACOMPANHA_CRM = (value == true) ? 1 : 0;
            }
        }

        public bool ContatoCRM
        {
            get
            {
                if (PERF_IN_CONTATO_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_CONTATO_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool MensagemContatoCRM
        {
            get
            {
                if (PERF_IN_MENSAGEM_CONTATO_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_MENSAGEM_CONTATO_CRM = (value == true) ? 1 : 0;
            }
        }

        public bool EditarAcaoCRM
        {
            get
            {
                if (PERF_IN_ALTERACAO_ACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERACAO_ACAO = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirAcaoCRM
        {
            get
            {
                if (PERF_IN_EXCLUSAO_ACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_ACAO = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarAcaoCRM
        {
            get
            {
                if (PERF_IN_REATIVA_ACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVA_ACAO = (value == true) ? 1 : 0;
            }
        }
        public bool EncerrarAcaoCRM
        {
            get
            {
                if (PERF_IN_ENCERRA_ACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ENCERRA_ACAO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirAcaoCRM
        {
            get
            {
                if (PERF_IN_INCLUSAO_ACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_ACAO = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoProposta
        {
            get
            {
                if (PERF_IN_ACESSO_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirProposta
        {
            get
            {
                if (PERF_IN_INCLUSAO_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool CancelarProposta
        {
            get
            {
                if (PERF_IN_CANCELAR_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_CANCELAR_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool AprovarProposta
        {
            get
            {
                if (PERF_IN_APROVAR_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_APROVAR_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool EnviarProposta
        {
            get
            {
                if (PERF_IN_ENVIO_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ENVIO_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool EditarProposta
        {
            get
            {
                if (PERF_IN_ALTERACAO_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERACAO_PROPOSTA = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirProposta
        {
            get
            {
                if (PERF_IN_EXCLUSAO_PROPOSTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_PROPOSTA = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoFT
        {
            get
            {
                if (PERF_IN_ACESSO_FT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_FT = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirFT
        {
            get
            {
                if (PERF_IN_INCLUSAO_FT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUSAO_FT = (value == true) ? 1 : 0;
            }
        }
        public bool EditarFT
        {
            get
            {
                if (PERF_IN_EDICAO_FT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDICAO_FT = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirFT
        {
            get
            {
                if (PERF_IN_EXCLUSAO_FT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUSAO_FT = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarFT
        {
            get
            {
                if (PERF_IN_REATIVACAO_FT == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVACAO_FT = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoEstoque
        {
            get
            {
                if (PERF_IN_ACESSO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirMovimentacaoEstoque
        {
            get
            {
                if (PERF_IN_INCLUIR_MOVIMENTACAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUIR_MOVIMENTACAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirMovimentacaoEstoque
        {
            get
            {
                if (PERF_IN_EXCLUIR_MOVIMENTACAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUIR_MOVIMENTACAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool CompraManualEstoque
        {
            get
            {
                if (PERF_IN_COMPRA_MANUAL_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_COMPRA_MANUAL_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool DevolucaoEstoque
        {
            get
            {
                if (PERF_IN_DEVOLUCAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_DEVOLUCAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool AcertoEstoque
        {
            get
            {
                if (PERF_IN_ACERTO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACERTO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool DescarteEstoque
        {
            get
            {
                if (PERF_IN_DESCARTE_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_DESCARTE_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool ManutencaoEstoque
        {
            get
            {
                if (PERF_IN_MANUTENCAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_MANUTENCAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool PerdaEstoque
        {
            get
            {
                if (PERF_IN_PERDA_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PERDA_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool TransferenciaEstoque
        {
            get
            {
                if (PERF_IN_TRANSFERENCIA_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_TRANSFERENCIA_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool VerMovimentacaoEstoque
        {
            get
            {
                if (PERF_IN_VER_MOVIMENTACAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_VER_MOVIMENTACAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool EditarMovimentacaoEstoque
        {
            get
            {
                if (PERF_IN_EDITAR_MOVIMENTACAO_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EDITAR_MOVIMENTACAO_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool AtualizacaoEstoque
        {
            get
            {
                if (PERF_IN_ATUALIZA_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATUALIZA_ESTOQUE = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoRegiao
        {
            get
            {
                if (PERF_IN_ACESSO_REGIAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_REGIAO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirRegiao
        {
            get
            {
                if (PERF_IN_INCLUIR_REGIAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUIR_REGIAO = (value == true) ? 1 : 0;
            }
        }
        public bool EditarRegiao
        {
            get
            {
                if (PERF_IN_ALTERAR_REGIAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERAR_REGIAO = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirRegiao
        {
            get
            {
                if (PERF_IN_EXCLUIR_REGIAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUIR_REGIAO = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarRegiao
        {
            get
            {
                if (PERF_IN_REATIVAR_REGIAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVAR_REGIAO = (value == true) ? 1 : 0;
            }
        }
        public bool AcessoTransportadora
        {
            get
            {
                if (PERF_IN_ACESSO_TRANSPORTADORA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_TRANSPORTADORA = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirTransportadora
        {
            get
            {
                if (PERF_IN_INCLUIR_TRANSPORTADORA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUIR_TRANSPORTADORA = (value == true) ? 1 : 0;
            }
        }
        public bool EditarTransportadora
        {
            get
            {
                if (PERF_IN_ALTERAR_TRANSPORTADORA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERAR_TRANSPORTADORA = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirTransportadora
        {
            get
            {
                if (PERF_IN_EXCLUIR_TRANSPORTADORA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUIR_TRANSPORTADORA = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarTransportadora
        {
            get
            {
                if (PERF_IN_REATIVAR_TRANSPORTADORA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVAR_TRANSPORTADORA = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoPaciente
        {
            get
            {
                if (PERF_IN_ACESSO_PACIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ACESSO_PACIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirPaciente
        {
            get
            {
                if (PERF_IN_INCLUIR_PACIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_INCLUIR_PACIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarPaciente
        {
            get
            {
                if (PERF_IN_ALTERAR_PACIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ALTERAR_PACIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirPaciente
        {
            get
            {
                if (PERF_IN_EXCLUIR_PACIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXCLUIR_PACIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool ReativarPaciente
        {
            get
            {
                if (PERF_IN_REATIVAR_PACIENTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_REATIVAR_PACIENTE = (value == true) ? 1 : 0;
            }
        }
        public bool AtestadoPaciente
        {
            get
            {
                if (PERF_IN_PACIENTE_ATESTADO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_ATESTADO = (value == true) ? 1 : 0;
            }
        }
        public bool SolicitacaoPaciente
        {
            get
            {
                if (PERF_IN_PACIENTE_SOLICITACAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_SOLICITACAO = (value == true) ? 1 : 0;
            }
        }
        public bool ConsultaIncluirPaciente
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool ConsultaAlterarPaciente
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ConsultaExcluirPaciente
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoPrescricao
        {
            get
            {
                if (PERF_IN_PRESCRICAO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PRESCRICAO_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirPrescricao
        {
            get
            {
                if (PERF_IN_PRESCRICAO_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PRESCRICAO_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarPrescricao
        {
            get
            {
                if (PERF_IN_PRESCRICAO_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PRESCRICAO_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirPrescricao
        {
            get
            {
                if (PERF_IN_PRESCRICAO_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PRESCRICAO_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool EnviarPrescricao
        {
            get
            {
                if (PERF_IN_PRESCRICAO_ENVIAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PRESCRICAO_ENVIAR = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoAtestado
        {
            get
            {
                if (PERF_IN_ATESTADO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATESTADO_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool AcessoConsulta
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirAtestado
        {
            get
            {
                if (PERF_IN_ATESTADO_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATESTADO_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarAtestado
        {
            get
            {
                if (PERF_IN_ATESTADO_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATESTADO_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirAtestado
        {
            get
            {
                if (PERF_IN_ATESTADO_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATESTADO_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool EnviarAtestado
        {
            get
            {
                if (PERF_IN_ATESTADO_ENVIAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ATESTADO_ENVIAR = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoSolicitacao
        {
            get
            {
                if (PERF_IN_SOLICITACAO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_SOLICITACAO_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirSolicitacao
        {
            get
            {
                if (PERF_IN_SOLICITACAO_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_SOLICITACAO_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarSolicitacao
        {
            get
            {
                if (PERF_IN_SOLICITACAO_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_SOLICITACAO_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirSolicitacao
        {
            get
            {
                if (PERF_IN_SOLICITACAO_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_SOLICITACAO_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool EnviarSolicitacao
        {
            get
            {
                if (PERF_IN_SOLICITACAO_ENVIAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_SOLICITACAO_ENVIAR = (value == true) ? 1 : 0;
            }
        }

        public bool AcessoPacienteConsulta
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirPacienteConsulta
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarPacienteConsulta
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirPacienteConsulta
        {
            get
            {
                if (PERF_IN_PACIENTE_CONSULTA_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_PACIENTE_CONSULTA_EXCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool PacienteMensagem
        {
            get
            {
                if (PAERF_IN_PACIENTE_MENSAGEM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAERF_IN_PACIENTE_MENSAGEM = (value == true) ? 1 : 0;
            }
        }
        public bool AcessoExame
        {
            get
            {
                if (PERF_IN_EXAME_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXAME_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirExame
        {
            get
            {
                if (PERF_IN_EXAME_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXAME_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarExame
        {
            get
            {
                if (PERF_IN_EXAME_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXAME_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirExame
        {
            get
            {
                if (PERF_IN_EXAME_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_EXAME_EXCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AcessoAnamnese
        {
            get
            {
                if (PERF_IN_ANAMNESE_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ANAMNESE_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirAnamnese
        {
            get
            {
                if (PERF_IN_ANAMNESE_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ANAMNESE_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarAnamnese
        {
            get
            {
                if (PERF_IN_ANAMNESE_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ANAMNESE_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirAnamnese
        {
            get
            {
                if (PERF_IN_ANAMNESE_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ANAMNESE_EXCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AcessoFisico
        {
            get
            {
                if (PERF_IN_FISICO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FISICO_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool IncluirFisico
        {
            get
            {
                if (PERF_IN_FISICO_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FISICO_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool AlterarFisico
        {
            get
            {
                if (PERF_IN_FISICO_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FISICO_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool ExcluirFisico
        {
            get
            {
                if (PERF_IN_FISICO_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FISICO_EXCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool VisaoGeral
        {
            get
            {
                if (PERF_IN_VISAO_GERAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_VISAO_GERAL = (value == true) ? 1 : 0;
            }
        }
        public bool Bloqueio
        {
            get
            {
                if (PERF_IN_BLOQUEIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_BLOQUEIO = (value == true) ? 1 : 0;
            }
        }
        public bool ConfCanc
        {
            get
            {
                if (PERF_IN_CONF_CANC_CONSULTA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_CONF_CANC_CONSULTA = (value == true) ? 1 : 0;
            }
        }
        public bool FinanceiroAcesso
        {
            get
            {
                if (PERF_IN_FINANCEIRO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool FinanceiroContador
        {
            get
            {
                if (PERF_IN_FINANCEIRO_CONTADOR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_CONTADOR = (value == true) ? 1 : 0;
            }
        }
        public bool FinanceiroRelatorio
        {
            get
            {
                if (PERF_IN_FINANCEIRO_RELATORIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_RELATORIO = (value == true) ? 1 : 0;
            }
        }
        public bool PagamentoAcesso
        {
            get
            {
                if (PERF_IN_FINANCEIRO_PAG_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_PAG_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool PagamentoInclusao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_PAG_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_PAG_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool PagamentoEdicao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_PAG_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_PAG_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool PagamentoExclusao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_PAG_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_PAG_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool RecebimentoAcesso
        {
            get
            {
                if (PERF_IN_FINANCEIRO_REC_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_REC_ACESSO = (value == true) ? 1 : 0;
            }
        }
        public bool RecebimentoInclusao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_REC_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_REC_INCLUIR = (value == true) ? 1 : 0;
            }
        }
        public bool RecebimentoEdicao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_REC_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_REC_ALTERAR = (value == true) ? 1 : 0;
            }
        }
        public bool RecebimentoExclusao
        {
            get
            {
                if (PERF_IN_FINANCEIRO_REC_EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_FINANCEIRO_REC_EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool EnviarMedico
        {
            get
            {
                if (PERF_IN_ENVIAR_MEDICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_ENVIAR_MEDICO = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoAcesso
        {
            get
            {
                if (PERF_IN_LOCACAO_ACESSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO_ACESSO = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoIncluir
        {
            get
            {
                if (PERF_IN_LOCACAO_INCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO_INCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoAlterar
        {
            get
            {
                if (PERF_IN_LOCACAO_ALTERAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO_ALTERAR = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoExcluir
        {
            get
            {
                if (PERF_IN_LOCACAO__EXCLUIR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO__EXCLUIR = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoRenovar
        {
            get
            {
                if (PERF_IN_LOCACAO_RENOVAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO_RENOVAR = (value == true) ? 1 : 0;
            }
        }

        public bool LocacaoEncerrar
        {
            get
            {
                if (PERF_IN_LOCACAO_ENCERRAR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PERF_IN_LOCACAO_ENCERRAR = (value == true) ? 1 : 0;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }

    }
}