using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Anotacao
    {
        public int PAAN_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public System.DateTime PAAN_DT_ANOTACAO { get; set; }
        public int USUA_CD_ID { get; set; }
        public string PAAN_TX_ANOTACAO { get; set; }
        public int PAAN_IN_ATIVO { get; set; }
    }
}
