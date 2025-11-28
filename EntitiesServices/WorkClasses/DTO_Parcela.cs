using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Parcela
    {
        public int LOPA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        public Nullable<System.DateTime> LOPA_DT_VENCIMENTO { get; set; }
        public Nullable<System.DateTime> LOPA_DT_PAGAMENTO { get; set; }
        public Nullable<decimal> LOPA_VL_VALOR { get; set; }
        public Nullable<decimal> LOPA_VL_VALOR_PAGO { get; set; }
        public Nullable<int> LOPA_IN_STATUS { get; set; }
        public Nullable<int> LOPA_IN_ATRASO { get; set; }
        public int LOPA_IN_ATIVO { get; set; }
        public Nullable<int> LOPA_IN_PARCELA { get; set; }
        public string LOPA_DS_DESCRICAO { get; set; }
        public Nullable<decimal> LOPA_VL_DESCONTO { get; set; }
        public Nullable<decimal> LOPA_VL_JUROS { get; set; }
        public Nullable<decimal> LOPA_VL_TAXAS { get; set; }
        public Nullable<int> LOPA_IN_QUITADA { get; set; }
        public Nullable<int> LOPA_NR_PACELAS { get; set; }
        public string LOPA_NM_PARCELAS { get; set; }
        public Nullable<System.DateTime> LOPA_DT_DUMMY { get; set; }
        public Nullable<int> LOPA_IN_PACIENTE_DUMMY { get; set; }
    }
}
