using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_ValorConsulta
    {
        public int VACO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> TIVL_CD_ID { get; set; }
        public Nullable<System.DateTime> VACO_DT_REFERENCIA { get; set; }
        public string VACO_NM_NOME { get; set; }
        public Nullable<decimal> VACO_NR_VALOR { get; set; }
        public Nullable<decimal> VACO_NR_DESCONTO { get; set; }
        public int VACO_IN_ATIVO { get; set; }
        public Nullable<int> VACO_IN_PADRAO { get; set; }
        public string VACO_NM_EXIBE { get; set; }

    }
}
