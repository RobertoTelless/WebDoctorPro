using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Produto_Venda
    {
        public int PRPV_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public Nullable<int> TIEM_CD_ID { get; set; }
        public Nullable<System.DateTime> PRPV_DT_PRECO_VENDA { get; set; }
        public Nullable<decimal> PRPV_VL_PRECO_VENDA { get; set; }
        public Nullable<decimal> PRPV_PC_DESCONTO { get; set; }
        public Nullable<decimal> PRPV_VL_PRECO_PROMOCAO { get; set; }
        public Nullable<decimal> PRPV_VL_PRECO_EMBALAGEM { get; set; }
        public int PRPV_IN_ATIVO { get; set; }
        public Nullable<int> PRPV_IN_SISTEMA { get; set; }
    }
}
