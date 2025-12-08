using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Acompanhamento
    {
        public int PAAA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PAAM_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> PAAA_DT_ANOTACAO { get; set; }
        public string PAAA_TX_ANOTACAO { get; set; }
        public int PAAA_IN_ATIVO { get; set; }
        public string PAAA_TX_TEXTO { get; set; }
    }
}
