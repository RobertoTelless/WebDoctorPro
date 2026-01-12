using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Pagamento_Anotacao
    {
        public int PGAN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int COPA_CD_ID { get; set; }
        public Nullable<System.DateTime> PGAN_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string PGAN_TX_ANOTACAO { get; set; }
        public int PGAN_IN_ATIVO { get; set; }
    }
}
