using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Ocorrencia
    {
        public int LOOC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        public Nullable<int> TIOC_CD_ID { get; set; }
        public Nullable<System.DateTime> LOOC_DT_OCORRENCIA { get; set; }
        public string LOOC_NM_TITULO { get; set; }
        public string LOOC_DS_DESCRICAO { get; set; }
        public string LOOC_SERIE_ENTRADA { get; set; }
        public string LOOC_SERIE_SAIDA { get; set; }
        public int LOOC_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
    }
}
