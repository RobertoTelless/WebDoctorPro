using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Vacina
    {
        public int PAVI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> VACI_CD_ID { get; set; }
        public Nullable<System.DateTime> PAVI_DT_DATA { get; set; }
        public Nullable<int> PAVI_IN_ATIVO { get; set; }
        public string PAVI_DS_DESCRICAO { get; set; }
        public Nullable<System.DateTime> PAVI_DT_PROXIMA { get; set; }

    }
}
