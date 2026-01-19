using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Movimento
    {
        public int MOEP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public System.DateTime MOEP_DT_MOVIMENTO { get; set; }
        public Nullable<int> MOEP_IN_ULTIMO { get; set; }
        public int MOEP_IN_TIPO_MOVIMENTO { get; set; }
        public int MOEP_QN_QUANTIDADE { get; set; }
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO { get; set; }
        public Nullable<decimal> MOEP_VL_QUANTIDADE_ANTERIOR { get; set; }
        public string MOEP_IN_ORIGEM { get; set; }
        public int MOEP_IN_CHAVE_ORIGEM { get; set; }
        public int MOEP_IN_ATIVO { get; set; }
        public string MOEP_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> MOEP_QN_ANTES { get; set; }
        public Nullable<int> MOEP_QN_DEPOIS { get; set; }
        public Nullable<int> MOEP_QN_ALTERADA { get; set; }
        public Nullable<int> MOEP_IN_OPERACAO { get; set; }
        public Nullable<int> EMFI_CD_ID { get; set; }
        public Nullable<int> MOEP_EMFI_CD_ID { get; set; }
        public Nullable<System.DateTime> MOEP_DT_DATA_DUMMY { get; set; }
        public Nullable<int> MOEP_IN_TIPO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_DATA_DUMMY_1 { get; set; }
        public Nullable<int> MOEP_EMFI_CD_ID_ALVO { get; set; }
        public Nullable<int> CRPV_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        public string MOEP_DS_MANUTENCAO_OBSERVACAO { get; set; }
        public Nullable<int> MOEP_IN_PENDENTE { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR { get; set; }
        public Nullable<int> MOEP_IN_TIPO_LANCAMENTO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_LANCAMENTO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_PAGAMENTO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_AUTORIZACAO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_EXCLUSAO { get; set; }
        public Nullable<int> MOEP_IN_EXCLUIDOR { get; set; }
        public string MOEP_GU_GUID { get; set; }
        public Nullable<int> MOEP_IN_SISTEMA { get; set; }
        public string MOEP_NM_FORNECEDOR { get; set; }
        public Nullable<int> COPA_CD_ID_1 { get; set; }
    }
}
