using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConfiguracaoViewModel
    {
        [Key]
        public int CONF_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_FALHAS_DIA { get; set; }
        public string CONF_NM_HOST_SMTP { get; set; }
        public string CONF_NM_PORTA_SMTP { get; set; }
        public string CONF_NM_EMAIL_EMISSOO { get; set; }
        public string CONF_NM_SENHA_EMISSOR { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_REFRESH_DASH { get; set; }
        public string CONF_NM_ARQUIVO_ALARME { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_REFRESH_NOTIFICACAO { get; set; }
        public string CONF_SG_LOGIN_SMS { get; set; }
        public string CONF_SG_LOGIN_SMS_CRIP { get; set; }
        public string CONF_SG_SENHA_SMS { get; set; }
        public string CONF_SG_SENHA_SMS_CRIP { get; set; }
        public string CONF_SG_LOGIN_SMS_PRIORITARIO { get; set; }
        public string CONF_SG_LOGIN_SMS_PRIORITARIO_CRIP { get; set; }
        public string CONF_SG_SENHA_SMS_PRIORITARIO { get; set; }
        public string CONF_SG_SENHA_SMS_PRIORITARIO_CRIP { get; set; }
        public string CONF_NM_SENDGRID_LOGIN { get; set; }
        public string CONF_NM_SENDGRID_LOGIN_CRIP { get; set; }
        public string CONF_NM_SENDGRID_PWD { get; set; }
        public string CONF_NM_SENDGRID_PWD_CRIP { get; set; }
        public string CONF_NM_SENDGRID_APIKEY { get; set; }
        public string CONF_NM_SENDGRID_APIKEY_CRIP { get; set; }
        public Nullable<int> CONF_IN_RESIDUAL { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_DIAS_ATENDIMENTO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_DIAS_ACAO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_DIAS_PROPOSTA { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_DIAS_VENDA { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_MARGEM_ATRASO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_DIAS_RESERVA_ESTOQUE { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_NUMERO_INICIAL_PROPOSTA { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_NUMERO_INICIAL_PEDIDO { get; set; }
        public Nullable<int> CONF_IN_CNPJ_DUPLICADO { get; set; }
        public Nullable<int> CONF_IN_INCLUIR_SEM_ESTOQUE { get; set; }
        public Nullable<int> CONF_IN_ASSINANTE_FILIAL { get; set; }
        public Nullable<int> CONF_IN_FALHA_IMPORTACAO { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_ETAPAS_CRM { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_ADM { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_GER { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_VEN { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_OPR { get; set; }
        public Nullable<int> CONF_IN_NOTIF_ACAO_USU { get; set; }
        public string CONF_LK_LINK_SISTEMA { get; set; }
        public string CONF_EM_CRMSYS { get; set; }
        public string CONF_EM_CRMSYS1 { get; set; }
        public string CONF_NR_SUPORTE_ZAP { get; set; }
        public string CONF_NR_SUPORTE_ZAP1 { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_VALIDADE_SENHA { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_NR_TAMANHO_SENHA { get; set; }
        public Nullable<int> CONF_NR_VALIDADE_PESQUISA { get; set; }
        public Nullable<int> CONF_IN_LOGO_EMPRESA { get; set; }
        public Nullable<int> CONF_NR_GRID_CLIENTE { get; set; }
        public Nullable<int> CONF_NR_GRID_MENSAGEM { get; set; }
        public Nullable<int> CONF_NR_GRID_PRODUTO { get; set; }
        public string CONF_CS_CONNECTION_STRING_AZURE { get; set; }
        public string CONF_CS_CONNECTION_STRING_AZURE_CRIP { get; set; }
        public string CONF_NM_KEY_AZURE { get; set; }
        public string CONF_NM_KEY_AZURE_CRIP { get; set; }
        public string CONF_NM_ENDPOINT_AZURE { get; set; }
        public string CONF_NM_ENDPOINT_AZURE_CRIP { get; set; }
        public string CONF_NM_EMISSOR_AZURE { get; set; }
        public string CONF_NM_EMISSOR_AZURE_CRIP { get; set; }
        public Nullable<int> CONF_IN_VALIDADE_CODIGO { get; set; }
        public Nullable<int> CONF_NR_GRID_DOCUMENTO { get; set; }
        public Nullable<int> CONF_NR_DIAS_LOG { get; set; }
        public Nullable<int> CONF_NR_DIAS_FIM_LOG { get; set; }
        public string CONF_EM_CONTATO { get; set; }
        public Nullable<int> CONF_NR_AVISO_CONTAS { get; set; }
        public Nullable<int> CONF_IN_MENSAGENS_CP { get; set; }
        public Nullable<int> CONF_IN_MENSAGENS_CR { get; set; }
        public Nullable<int> CONF_IN_ROBOT { get; set; }
        public Nullable<int> CONF_IN_CLIENTE_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_FORNECEDOR_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_BANCO_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_CONTA_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_MODELO_MAIL_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_CENTRO_CUSTO_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_PAGAR_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_RECEBER_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_USUARIO_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_PAGAR_SEM_SALDO { get; set; }
        public string CONF_NM_SUFIXO_RECORRENCIA { get; set; }
        public string CONF_NM_SUFIXO_NUMERO { get; set; }
        public Nullable<int> CONF_IN_SUFIXO_NUMERO { get; set; }
        public Nullable<int> CONF_IN_DASH_INICIAL { get; set; }
        public Nullable<int> CONF_IN_CRM_SISTEMA { get; set; }
        public Nullable<int> CONF_IN_MENSAGEM_CRM { get; set; }
        public Nullable<int> CONF_IN_MENSAGEM_FABRICANTE { get; set; }
        public Nullable<int> CONF_IN_EMAIL_ROBOT { get; set; }
        public Nullable<int> CONF_IN_SMS_ROBOT { get; set; }
        public Nullable<int> CONF_IN_KANBAN_GRID { get; set; }
        public Nullable<int> CONF_IN_DIAS_ESTADO { get; set; }
        public Nullable<int> CONF_IN_MENSAGEM_COMPRA { get; set; }
        public Nullable<int> CONF_IN_COMISSAO_GERENTE { get; set; }
        public string CONF_NM_JUSTIFICATIVA_PADRAO { get; set; }
        public Nullable<int> CONF_PC_ALTERACAO_ESTOQUE { get; set; }
        public string CONF_NM_CAMINHO_PLANILHA { get; set; }
        public Nullable<int> CONF_IN_PACIENTE_ATRASO { get; set; }
        public Nullable<int> CONF_IN_PACIENTE_AUSENCIA { get; set; }
        public Nullable<int> CONF_IN_MENSAGEM_CONSULTA { get; set; }
        public string CONF_LK_VALIDACAO { get; set; }
        public Nullable<int> CONF_IN_EXIBE_LOGO { get; set; }
        public Nullable<int> CONF_IN_EMAIL_AUTOMATICO { get; set; }
        public string CONF_LK_LINK_VALIDACAO { get; set; }
        public Nullable<int> CONF_NR_DIAS_CONFIRMACAO { get; set; }
        public Nullable<int> CONF_NR_MESES_RETORNO { get; set; }
        public Nullable<int> CONF_IN_INCLUIR_REMEDIO { get; set; }
        public Nullable<int> CONF_IN_INCLUIR_SOLICITACAO { get; set; }
        public Nullable<int> CONF_IN_GERA_RECEBIMENTO { get; set; }
        public Nullable<int> CONF_IN_INCLUIR_PACIENTE_SEGUE { get; set; }
        public Nullable<int> CONF_IN_PACIENTE_FOTO_CAMERA { get; set; }
        public Nullable<int> CONF_IN_PADRAO_ANAMNESE { get; set; }
        public string CONF_FD_FICHAS { get; set; }
        public string CON_TK_TOKEN_API_PAGTO { get; set; }
        public Nullable<int> CONF_IN_CALCULA_PROXIMA_CONSULTA { get; set; }
        public Nullable<int> CONF_IN_DIAS_PROXIMA_CONSULTA { get; set; }
        public Nullable<int> CONF_IN_PISCA { get; set; }
        public Nullable<int> CONF_IN_ENVIA_EXAME_ZAP { get; set; }
        public Nullable<int> CONF_IN_ENVIA_ANIVERSARIO { get; set; }
        public Nullable<int> CONF_IN_ENVIA_CONFIRMACAO { get; set; }
        public Nullable<int> CONF_IN_ENVIA_PAGAMENTO { get; set; }
        [StringLength(20, ErrorMessage = "WHATSAPP deve conter no máximo 20 caracteres.")]
        public string CONF_NR_WHAPSAPP { get; set; }
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Deve ser um valor inteiro positivo")]
        public Nullable<int> CONF_IN_VALIDADE_FAIXA { get; set; }
        public Nullable<int> CONF_IN_ENVIA_PACIENTE_CADASTRO { get; set; }
        public Nullable<int> CONF_IN_ENVIA_ATRASO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CONF_IN_MAXIMO_ENVIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CONF_IN_INTERVALO_ENVIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CONF_IN_HORA_LIMITE { get; set; }
        public Nullable<int> CONF_IN_MENSAGEM_MARCACAO { get; set; }
        public Nullable<int> CONF_IN_MARCA_CONSULTA_HORA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CONF_NR_MARCA_CONSULTA_HORA { get; set; }
        public Nullable<int> CONF_IN_ASSINA_DIGITAL_ATESTADO { get; set; }
        public Nullable<int> CONF_IN_ASSINA_DIGITAL_SOLICITACAO { get; set; }
        public Nullable<int> CONF_IN_ASSINA_DIGITAL_PRESCRICAO { get; set; }
        [StringLength(150, ErrorMessage = "LOCAL DO ARQUIVO .PFX deve conter no máximo 150 caracteres.")]
        public string CONF_IN_ASSINA_DIGITAL_LOCAL_PFX { get; set; }
        public Nullable<int> CONF_IN_MODELO_ANAMNESE { get; set; }
        public Nullable<int> CONF_IN_AVISO_ESTOQUE { get; set; }
        public Nullable<int> CONF_IN_ASSINA_DIGITAL_LOCACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CONF_VL_ACRESCIMO_ATRASO_PARCELA { get; set; }
        [StringLength(12, ErrorMessage = "SENHA DO PACIENTE deve conter no máximo 12 caracteres.")]
        public string CONF_NM_SENHA_PACIENTE { get; set; }
        public Nullable<int> CONF_IN_RECIBO_SRF { get; set; }
        public Nullable<int> CONF_IN_DOC_PRONTUARIO { get; set; }
        public Nullable<int> CONF_IN_ABA_VACINA { get; set; }
        public Nullable<int> CONF_IN_ABA_EXAME { get; set; }
        public Nullable<int> CONF_IN_ABA_ATESTADO { get; set; }
        public Nullable<int> CONF_IN_ABA_SOLICITACAO { get; set; }
        public Nullable<int> CONF_IN_ABA_PRESCRICAO { get; set; }
        public Nullable<int> CONF_IN_ABA_LOCACAO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}