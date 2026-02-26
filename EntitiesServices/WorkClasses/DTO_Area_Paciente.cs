using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Area_Paciente
    {
        public int AREA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<System.DateTime> AREA_DT_ENTRADA { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO { get; set; }
        public Nullable<int> AREA_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> AREA_DT_CONSULTA { get; set; }
        public Nullable<System.TimeSpan> AREA_HR_INICIO { get; set; }
        public Nullable<System.TimeSpan> AREA_HR_FINAL { get; set; }
        public string AREA_NM_TITULO { get; set; }
        public string AREA_TX_CONTEUDO { get; set; }
        public string AREA_GU_IDENTIFICADOR { get; set; }

    }
}
