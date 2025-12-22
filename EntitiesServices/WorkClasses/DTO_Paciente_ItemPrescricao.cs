using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_ItemPrescricao
    {
        public int PAPI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PAPR_CD_ID { get; set; }
        public Nullable<int> TIFO_CD_ID { get; set; }
        public string PAPI_NM_REMEDIO { get; set; }
        public string PAPI_DS_POSOLOGIA { get; set; }
        public Nullable<int> PAPI_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public string PAPI_NM_GENERICO { get; set; }
        public Nullable<System.DateTime> PAPI_DT_DATA_1 { get; set; }
        public Nullable<System.DateTime> PAPI_DT_DATA_2 { get; set; }
        public string PAPI_NM_APRESENTACAO { get; set; }
        public string PAPI_NM_LABORATORIO { get; set; }
        public string PAPI_NR_QUANTIDADE { get; set; }
    }
}
