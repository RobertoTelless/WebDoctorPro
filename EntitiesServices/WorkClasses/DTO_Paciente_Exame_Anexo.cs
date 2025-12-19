using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Exame_Anexo
    {
        public int PAEO_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PAEX_CD_ID { get; set; }
        public Nullable<System.DateTime> PAEO_DT_ANEXO { get; set; }
        public Nullable<int> PAEO_IN_TIPO { get; set; }
        public string PAEO_NM_TITULO { get; set; }
        public string PAEO_AQ_ARQUIVO { get; set; }
        public Nullable<int> PAEO_IN_ATIVO { get; set; }
    }
}
