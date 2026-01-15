using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Produto_Custo
    {
        public int PRCU_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public Nullable<System.DateTime> PRCU_DT_CUSTO { get; set; }
        public Nullable<decimal> PRCU_VL_CUSTO { get; set; }
        public int PRCU_IN_ATIVO { get; set; }
        public Nullable<int> PRCU_IN_SISTEMA { get; set; }
    }
}
