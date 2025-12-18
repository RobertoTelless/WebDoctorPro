using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Solicitacao
    {
        public int PASO_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public string PASO_GU_GUID { get; set; }
        public Nullable<System.DateTime> PASO_DT_EMISSAO { get; set; }
        public string PASO_NM_TITULO { get; set; }
        public string PASO_TX_TEXTO { get; set; }
        public Nullable<int> PASO_IN_ENVIADO { get; set; }
        public Nullable<System.DateTime> PASO_DT_ENVIO { get; set; }
        public string PASO_GU_GUID_ENVIO { get; set; }
        public Nullable<int> PASO_IN_ATIVO { get; set; }
        public Nullable<int> TIEX_CD_ID { get; set; }
        public Nullable<int> PASO_NR_ENVIOS { get; set; }
        public Nullable<int> PASO_IN_PDF { get; set; }
        public string PASO_HT_TEXT_HTML { get; set; }
        public string PASO_AQ_ARQUIVO_HTML { get; set; }
        public string PASO_AQ_ARQUIVO_PDF { get; set; }
        public string PASO_AQ_ARQUIVO_QRCODE { get; set; }
        public Nullable<System.DateTime> PASO_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> PASO_IN_DATA { get; set; }
        public string PASO_DS_INDICACAO_CLINICA { get; set; }
        public Nullable<int> PASO_IN_QUANTIDADE { get; set; }
        public Nullable<System.DateTime> PASO_DT_DUMMY { get; set; }
        public Nullable<System.DateTime> PASO_DT_EMISSAO_COMPLETA { get; set; }
        public string PASO_TK_TOKEN { get; set; }
        public Nullable<int> SOLI_CD_ID { get; set; }
        public Nullable<int> PASO_IN_ASSINADO_DIGITAL { get; set; }
        public Nullable<System.DateTime> PASO_DT_VALIDACAO { get; set; }
        public string PASO_IP_VALIDACAO { get; set; }
        public Nullable<int> PASO_NR_VALIDACAO { get; set; }
        public Nullable<int> PASO_IN_VALIDACAO { get; set; }
        public string PASO_IP_DENUNCIA { get; set; }
        public Nullable<int> PASO_NR_DENUNCIA { get; set; }
        public string PASO_TX_DENUNCIA { get; set; }
        public Nullable<int> PASO_IN_DENUNCIA { get; set; }
        public Nullable<System.DateTime> PASO_DT_DENUNCIA { get; set; }
    }
}
