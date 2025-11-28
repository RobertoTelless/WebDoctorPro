using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Medico_Envio
    {
        public int MEEV_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int MEDC_CD_ID { get; set; }
        public Nullable<System.DateTime> MEEV_DT_ENVIO { get; set; }
        public string MEEV_TX_MENSAGEM { get; set; }
        public int MEEV_IN_ATIVO { get; set; }
        public string MEEV_GU_IDENTIFICADOR { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> TIEN_CD_ID { get; set; }
        public string MEEV_NM_TITULO { get; set; }
        public Nullable<int> MEEV_IN_ENVIOS { get; set; }
        public Nullable<System.DateTime> MEEV_DT_REMESSA { get; set; }
        public Nullable<int> MEEV_IN_ANAMNESE { get; set; }
        public Nullable<decimal> MEEV_NR_PRESSAO_POSITIVA { get; set; }
        public Nullable<int> MEEV_IN_IAH { get; set; }
        public string MEEV_NM_EQUIPAMENTO { get; set; }
        public string MEEV_NM_MASCARA { get; set; }
        public Nullable<System.DateTime> MEEV_DT_NOITE_INICIO { get; set; }
        public Nullable<System.DateTime> MEEV_DT_NOITE_FINAL { get; set; }
        public Nullable<int> MEEV_IN_NUM_NOITES { get; set; }
        public Nullable<decimal> MEEV_NR_PARAM_PRESSAO { get; set; }
        public Nullable<decimal> MEEV_NR_MEDIA_USO { get; set; }
        public Nullable<decimal> MEEV_IN_PERCENTUAL { get; set; }
        public Nullable<decimal> MEEV_NR_IAH_RESIDUAL { get; set; }
        public string MEEV_DS_SINTOMAS { get; set; }
        public Nullable<int> MEEV_IN_IAH_RESIDUAL { get; set; }
        public Nullable<int> MEEV_IN_ENVIADO { get; set; }

    }
}
