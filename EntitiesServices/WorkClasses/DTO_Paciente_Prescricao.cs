using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Prescricao
    {
        public int PAPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }
        public System.DateTime PAPR_DT_DATA { get; set; }
        public string PAPR_NM_REMEDIO { get; set; }
        public string PAPR_NM_DOSAGEM { get; set; }
        public string PAPR_NM_FORMA { get; set; }
        public string PAPR_NM_POSOLOGIA { get; set; }
        public string PAPR_DS_TEXTO { get; set; }
        public int PAPR_IN_ATIVO { get; set; }
        public string PAPR_GU_GUID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PAPR_IN_ENVIADO { get; set; }
        public Nullable<System.DateTime> PAPR_DT_ENVIO { get; set; }
        public string PAPR_GU_GUID_ENVIO { get; set; }
        public string PAPR_HT_TEXTO_HTML { get; set; }
        public string PAPR_AQ_ARQUIVO_HTML { get; set; }
        public string PAPR_AQ_ARQUIVO_PDF { get; set; }
        public string PAPR_AQ_ARQUIVO_QRCODE { get; set; }
        public Nullable<int> PAPR_IN_PDF { get; set; }
        public Nullable<int> PAPR_NR_ENVIOS { get; set; }
        public Nullable<System.DateTime> PAPR_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PAPR_IN_DATA { get; set; }
        public Nullable<System.DateTime> PAPR_DT_EMISSAO { get; set; }
        public Nullable<System.DateTime> PAPR_DT_EMISSAO_COMPLETA { get; set; }
        public string PAPR_TK_TOKEN { get; set; }
        public Nullable<int> PAPR_IN_ASSINADO_DIGITAL { get; set; }
        public Nullable<System.DateTime> PAPR_DT_VALIDACAO { get; set; }
        public string PAPR_IP_VALIDACAO { get; set; }
        public Nullable<int> PAPR_NR_VALIDACAO { get; set; }
        public Nullable<int> PAPR_IN_VALIDACAO { get; set; }
        public Nullable<int> PAPR_IN_DENUNCIA { get; set; }
        public Nullable<System.DateTime> PAPR_DT_DENUNCIA { get; set; }
        public string PAPR_IP_DENUNCIA { get; set; }
        public Nullable<int> PAPR_NR_DENUNCIA { get; set; }
        public string PAPR_TX_DENUNCIA { get; set; }
        public Nullable<int> PAPR_IN_ASSINANDO { get; set; }
    }
}
