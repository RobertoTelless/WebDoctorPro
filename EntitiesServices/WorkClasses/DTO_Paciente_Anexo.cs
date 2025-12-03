using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Anexo
    {
        public int PAAX_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public System.DateTime PAAX_DT_ANEXO { get; set; }
        public string PAAX_NM_TITULO { get; set; }
        public int PAAX_IN_TIPO { get; set; }
        public string PAAX_AQ_ARQUIVO { get; set; }
        public int PAAX_IN_ATIVO { get; set; }
    }
}
