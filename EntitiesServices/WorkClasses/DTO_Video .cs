using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Video
    {
        public int VIDE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> TIVE_CD_ID { get; set; }
        public Nullable<System.DateTime> VIDE_DT_INCLUSAO { get; set; }
        public string VIDE_NM_TITULO { get; set; }
        public string VIDE_AQ_ARQUIVO { get; set; }
        public Nullable<int> VIDE_IN_ATIVO { get; set; }
        public string VIDE_DS_DESCRICAO { get; set; }

    }
}
