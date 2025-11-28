using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_ModeloEMailHTML
    {
        public int TEHT_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public string TEHT_NM_NOME { get; set; }
        public string TEHT_AQ_ARQUIVO { get; set; }
        public Nullable<int> TEHT_IN_ATIVO { get; set; }
        public Nullable<int> TEHT_IN_SISTEMA { get; set; }
        public Nullable<System.DateTime> TEHT_DT_CADASTRO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }

    }
}
