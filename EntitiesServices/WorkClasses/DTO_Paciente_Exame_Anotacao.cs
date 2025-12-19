using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Exame_Anotacao
    {
        public int PAET_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PAEX_CD_ID { get; set; }
        public Nullable<System.DateTime> PAET_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string PAET_TX_ANOTACAO { get; set; }
        public Nullable<int> PAET_IN_ATIVO { get; set; }
    }
}
