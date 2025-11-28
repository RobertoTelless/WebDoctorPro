using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_ModeloSMS
    {
        public int TSMS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public string TSMS_NM_NOME { get; set; }
        public string TSMS_SG_SIGLA { get; set; }
        public string TSMS_TX_CORPO { get; set; }
        public string TSMS_LK_LINK { get; set; }
        public int TSMS_IN_ATIVO { get; set; }
        public Nullable<int> TSMS_IN_FIXO { get; set; }
        public Nullable<int> TSMS_IN_ROBOT { get; set; }
        public Nullable<int> TSMS_NR_SISTEMA { get; set; }
        public Nullable<int> TSMS_IN_EDITAVEL { get; set; }

    }
}
