using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Pagamento
    {
        public int COPA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> TIPA_CD_ID { get; set; }
        public Nullable<System.DateTime> COPA_DT_PAGAMENTO { get; set; }
        public Nullable<decimal> COPA_VL_VALOR { get; set; }
        public Nullable<int> COPA_IN_CONFERIDO { get; set; }
        public int COPA_IN_ATIVO { get; set; }
        public string COPA_NM_NOME { get; set; }
        public Nullable<System.DateTime> COPA_DT_DUMMY { get; set; }
        public string COPA_GU_GUID { get; set; }
        public string COPA_XM_NOTA_FISCAL { get; set; }
        public string COPA_NM_FAVORECIDO { get; set; }
        public Nullable<System.DateTime> COPA_DT_VENCIMENTO { get; set; }
        public Nullable<int> COPA_IN_PAGO { get; set; }
        public Nullable<decimal> COPA_VL_DESCONTO { get; set; }
        public Nullable<decimal> COPA_VL_MULTA { get; set; }
        public Nullable<decimal> COPA_VL_PAGO { get; set; }
        public Nullable<int> COPA_NR_ATRASO { get; set; }
        public Nullable<System.DateTime> COPA_DT_CADASTRO { get; set; }
    }
}
