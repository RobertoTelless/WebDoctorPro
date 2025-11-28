using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_ModeloEMail
    {
        public int TEEM_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public string TEEM_NM_NOME { get; set; }
        public string TEEM_SG_SIGLA { get; set; }
        public string TEEM_LK_LINK { get; set; }
        public Nullable<int> TEEM_IN_ATIVO { get; set; }
        public string TEEM_TX_CABECALHO { get; set; }
        public string TEEM_TX_CORPO { get; set; }
        public string TEEM_TX_DADOS { get; set; }
        public string TEEM_AQ_ARQUIVO { get; set; }
        public string TEEM_TX_COMPLETO { get; set; }
        public Nullable<int> TEEM_IN_HTML { get; set; }
        public Nullable<int> TEEM_IN_FIXO { get; set; }
        public Nullable<int> TEEM_IN_PESQUISA { get; set; }
        public Nullable<int> TEEM_IN_IMAGEM { get; set; }
        public Nullable<int> TEEM_IN_SISTEMA { get; set; }
        public Nullable<int> TEEM_IN_ROBOT { get; set; }
        public Nullable<int> TEEM_IN_EDITAVEL { get; set; }
        public Nullable<int> TEEM_IN_ANIVERSARIO { get; set; }

    }
}
