using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Produto_Concorrente
    {
        public int PRPF_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public string PRPF_NM_CONCORRENTE { get; set; }
        public decimal PRPF_VL_PRECO_CONCORRENTE { get; set; }
        public int PRPF_IN_ATIVO { get; set; }
        public System.DateTime PRPF_DT_CADASTRO { get; set; }
        public Nullable<int> PRPF_IN_SISTEMA { get; set; }
    }
}
