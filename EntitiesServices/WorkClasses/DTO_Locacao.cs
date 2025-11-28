using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Locacao
    {
        public int LOCA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public int PETA_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> LOCA_DT_INICIO { get; set; }
        public Nullable<int> LOCA_NR_PRAZO { get; set; }
        public Nullable<int> LOCA_IN_QUANTIDADE { get; set; }
        public Nullable<decimal> LOCA_VL_PARCELA { get; set; }
        public Nullable<int> LOCA_IN_RENOVACAO { get; set; }
        public Nullable<int> LOCA_IN_ATIVO { get; set; }
        public Nullable<int> LOCA_IN_ENCERRADO { get; set; }
        public Nullable<decimal> LOCA_VL_TOTAL { get; set; }
        public string LOCA_GU_GUID { get; set; }
        public Nullable<int> LOCA_IN_STATUS { get; set; }
        public string LOCA_NR_NUMERO { get; set; }
        public string LOCA_NM_TITULO { get; set; }
        public string LOCA_DS_DESCRICAO { get; set; }
        public Nullable<System.DateTime> LOCA_DT_DUMMY { get; set; }
        public Nullable<int> LOCA_NR_DIA { get; set; }
        public string LOCA_NM_PACIENTE_DUMMY { get; set; }
        public string LOCA_NM_PRODUTO_DUMMY { get; set; }
        public Nullable<System.DateTime> LOCA_DT_CANCELAMENTO { get; set; }
        public string LOCA_DS_JUSTIFICATIVA { get; set; }
        public Nullable<System.DateTime> LOCA_DT_ENCERRAMENTO { get; set; }
        public Nullable<System.DateTime> LOCA_DT_FINAL { get; set; }
        public Nullable<int> LOCA_NR_ATRASO { get; set; }
        public Nullable<System.DateTime> LOCA_DT_RENOVACAO { get; set; }
        public Nullable<int> LOCA_IN_RENOVACOES { get; set; }
        public string LOCA_NR_SERIE { get; set; }
        public Nullable<int> LOCA_IN_GARANTIA { get; set; }
        public Nullable<System.DateTime> LOCA_DT_GARANTIA { get; set; }
        public Nullable<System.DateTime> LOCA_DT_APROVACAO { get; set; }
        public Nullable<int> LOCA_IN_CONTRATO { get; set; }
        public string LOCA_XM_NOTA_FISCAL { get; set; }
        public Nullable<int> LOCA_NR_GARANTIA { get; set; }
        public string LOCA_TK_TOKEN { get; set; }
        public string LOCA_AQ_ARQUIVO_QRCODE { get; set; }
        public Nullable<System.DateTime> LOCA_DT_EMISSAO { get; set; }
        public Nullable<int> LOCA_IN_ASSINADO_DIGITAL { get; set; }
    }
}
