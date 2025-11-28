using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Medico
    {
        public int MEDC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public string MEDC_NM_MEDICO { get; set; }
        public string MEDC_NR_CRM { get; set; }
        public Nullable<int> ESPE_CD_ID { get; set; }
        public string MEDC_EM_EMAIL { get; set; }
        public string MEDC_NR_CELULAR { get; set; }
        public string MEDC_NR_TELEFONE { get; set; }
        public string MEDC_GU_IDENTIFICADOR { get; set; }
        public int MEDC_IN_ATIVO { get; set; }
        public string MEDC_NM_ENDERECO { get; set; }
        public string MEDC_NR_NUMERO { get; set; }
        public string MEDC_NM_COMPLEMENTO { get; set; }
        public string MEDC_NM_BAIRRO { get; set; }
        public string MEDC_NM_CIDADE { get; set; }
        public string MEDC_NR_CEP { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }

    }
}
