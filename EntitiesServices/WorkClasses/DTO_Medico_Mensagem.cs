using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Medico_Mensagem
    {
        public int METX_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> METX_DT_CRIACAO { get; set; }
        public string METX_NM_NOME { get; set; }
        public string METX_TX_TEXTO { get; set; }
        public Nullable<int> METX_IN_ATIVO { get; set; }
    }
}
