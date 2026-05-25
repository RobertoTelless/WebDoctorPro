using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_CRM
    {
        public int CRM1_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> FUNI_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public Nullable<int> TICR_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> MENS_CD_ID { get; set; }
        public Nullable<int> ORIG_CD_ID { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> PEVE_CD_ID { get; set; }
        public string CRM1_GU_GUID { get; set; }
        public Nullable<int> CRM1_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> CRM1_DT_CRIACAO { get; set; }
        public string CRM1_NM_NOME { get; set; }
        public string CRM1_DS_DESCRICAO { get; set; }
        public string CRM1_TX_INFORMACOES_GERAIS { get; set; }
        public int CRM1_IN_STATUS { get; set; }
        public Nullable<System.DateTime> CRM1_DT_CANCELAMENTO { get; set; }
        public string CRM1_DS_MOTIVO_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> CRM1_DT_ENCERRAMENTO { get; set; }
        public string CRM1_DS_INFORMACOES_ENCERRAMENTO { get; set; }
        public Nullable<int> CRM1_IN_ESTRELA { get; set; }
        public Nullable<int> CRM1_IN_VENDA { get; set; }
        public Nullable<int> PEVE_CD_ID1 { get; set; }
        public Nullable<int> PEVE_CD_ID2 { get; set; }
        public Nullable<int> CRM1_IN_DUMMY { get; set; }
        public string CRM1_AQ_IMAGEM { get; set; }
        public Nullable<int> CRM1_NR_TEMPERATURA { get; set; }
        public string CRM1_NM_CAMPANHA { get; set; }
        public Nullable<decimal> CRM1_VL_VALOR_INICIAL { get; set; }
        public Nullable<decimal> CRM1_VL_VALOR_FINAL { get; set; }
        public Nullable<int> CRM1_NR_ATRASO { get; set; }
        public Nullable<int> TRAN_CD_ID { get; set; }
        public Nullable<System.DateTime> CRM1_DT_PREVISAO_ENTREGA { get; set; }
        public Nullable<int> CRM1_IN_AVISO_ENTREGA { get; set; }
        public Nullable<System.DateTime> CRM1_DT_DATA_SAIDA { get; set; }
        public Nullable<int> CRM1_IN_ENTREGA_CONFIRMADA { get; set; }
        public string CRM_DS_INFORMACOES_SAIDA { get; set; }
        public Nullable<int> CRM1_IN_ENVIA_CLIENTE { get; set; }
        public Nullable<int> CRM1_IN_ENCERRADO { get; set; }
        public Nullable<int> EMFI_CD_ID { get; set; }
        public string CRM1_ID_IDENTIFICADOR { get; set; }
        public Nullable<int> CRM1_IN_SISTEMA { get; set; }
        public Nullable<int> LEAD_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }

    }
}
