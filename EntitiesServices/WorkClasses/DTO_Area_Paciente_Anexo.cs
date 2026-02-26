using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Area_Paciente_Anexo
    {
        public int APAN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int AREA_CD_ID { get; set; }
        public string APAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> APAN_DT_ANEXO { get; set; }
        public Nullable<int> APAN_IN_TIPO { get; set; }
        public string APAN_AQ_ARQUIVO { get; set; }
        public Nullable<int> APAN_IN_ATIVO { get; set; }

    }
}
