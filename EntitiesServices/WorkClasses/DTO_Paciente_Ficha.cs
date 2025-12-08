using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Ficha
    {
        public int PAFC_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public System.DateTime PAFC_DT_FICHA { get; set; }
        public string PAFC_NM_TITULO { get; set; }
        public int PAFC_IN_TIPO { get; set; }
        public string PAFC_AQ_ARQUIVO { get; set; }
        public int PAFC_IN_ATIVO { get; set; }
        public int ASSI_CD_ID { get; set; }
    }
}
