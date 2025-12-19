using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Exame
    {
        public int PAEX_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> TIEX_CD_ID { get; set; }
        public Nullable<System.DateTime> PAEX_DT_DATA { get; set; }
        public string PAEX_NM_NOME { get; set; }
        public string PAEX_DS_COMENTARIOS { get; set; }
        public string PAEX_AQ_ARQUIVO { get; set; }
        public Nullable<int> PAEX_IN_TIPO { get; set; }
        public Nullable<int> PAEX_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string PAEX_DS_DIAGNOSTICO { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<System.DateTime> PAEX_DT_DUMMY { get; set; }
        public Nullable<int> LABS_CD_ID { get; set; }
        public Nullable<int> PASO_CD_ID { get; set; }
    }
}
