using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_ContratoLocacao
    {
        public int COLO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }
        public Nullable<System.DateTime> COLO_DT_CRIACAO { get; set; }
        public string COLO_NM_NOME { get; set; }
        public string COLO_TX_TEXTO { get; set; }
        public Nullable<int> COLO_IN_ATIVO { get; set; }

    }
}
