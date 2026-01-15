using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Atestado
    {
        public int PAAT_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> TIAT_CD_ID { get; set; }
        public Nullable<System.DateTime> PAAT_DT_DATA { get; set; }
        public string PAAT_NM_TITULO { get; set; }
        public string PAAT_NM_DESTINO { get; set; }
        public string PAAT_TX_TEXTO { get; set; }
        public string PAAT_GU_GUID { get; set; }
        public Nullable<int> PAAT_IN_ATIVO { get; set; }
        public Nullable<int> PAAT__IN_ENVIADO { get; set; }
        public string PAAT_GU_GUID_ENVIO { get; set; }
        public Nullable<System.DateTime> PAAT_DT_ENVIO { get; set; }
        public Nullable<int> PAAT_NR_ENVIOS { get; set; }
        public Nullable<int> PAAT_IN_PDF { get; set; }
        public string PAAT_HT_TEXTO_HTML { get; set; }
        public string PAAT_AQ_ARQUIVO_HTML { get; set; }
        public string PAAT_AQ_ARQUIVO_PDF { get; set; }
        public string PAAT_AQ_ARQUIVO_QRCODE { get; set; }
        public Nullable<System.DateTime> PAAT_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> PAAT_IN_DATA { get; set; }
        public Nullable<System.DateTime> PAAT_DT_DUMMY { get; set; }
        public Nullable<System.DateTime> PAAT_DT_EMISSAO_COMPLETA { get; set; }
        public string PAAT_TK_TOKEN { get; set; }
        public Nullable<System.DateTime> PAAT_DT_VALIDACAO { get; set; }
        public string PAAT_IP_VALIDACAO { get; set; }
        public Nullable<int> PAAT_NR_VALIDACAO { get; set; }
        public Nullable<int> PAAT_IN_VALIDACAO { get; set; }
        public Nullable<int> PAAT_IN_DENUNCIA { get; set; }
        public Nullable<System.DateTime> PAAT_DT_DENUNCIA { get; set; }
        public string PAAT_IP_DENUNCIA { get; set; }
        public Nullable<int> PAAT_NR_DENUNCIA { get; set; }
        public string PAAT_TX_DENUNCIA { get; set; }
        public Nullable<int> PAAT_IN_ASSINADO_DIGITAL { get; set; }
    }
}
