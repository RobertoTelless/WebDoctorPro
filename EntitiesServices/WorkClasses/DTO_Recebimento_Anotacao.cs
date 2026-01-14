using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Recebimento_Anotacao
    {
        public int REAT_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CORE_CD_ID { get; set; }
        public Nullable<System.DateTime> REAT_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string REAT_TX_ANOTACAO { get; set; }
        public Nullable<int> REAT_IN_ATIVO { get; set; }
    }
}
