using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Aviso
    {
        public int AVIS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<System.DateTime> AVIS_DT_CRIACAO { get; set; }
        public Nullable<System.DateTime> AVIS_DT_AVISO { get; set; }
        public string AVIS_NM_TITULO { get; set; }
        public string AVIS_DS_AVISO { get; set; }
        public Nullable<int> AVIS_IN_CIENTE { get; set; }
        public Nullable<int> AVIS_IN_SISTEMA { get; set; }
        public int AVIS_IN_ATIVO { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }

    }
}
