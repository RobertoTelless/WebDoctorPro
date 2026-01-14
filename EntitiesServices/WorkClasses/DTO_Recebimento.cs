using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Recebimento
    {
        public int CORE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> VACO_CD_ID { get; set; }
        public Nullable<decimal> CORE_VL_VALOR { get; set; }
        public Nullable<int> FORE_CD_ID { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        public Nullable<System.DateTime> CORE_DT_RECEBIMENTO { get; set; }
        public int CORE_IN_ATIVO { get; set; }
        public Nullable<int> CORE_IN_CONFERIDO { get; set; }
        public Nullable<int> VASE_CD_ID { get; set; }
        public Nullable<int> VACV_CD_ID { get; set; }
        public Nullable<decimal> CORE_VL_SERVICO { get; set; }
        public Nullable<decimal> CORE_VL_CONVENIO { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public string CORE_NM_RECEBIMENTO { get; set; }
        public Nullable<System.DateTime> CORE_DT_DUMMY { get; set; }
        public string CORE_GU_GUID { get; set; }
    }
}
