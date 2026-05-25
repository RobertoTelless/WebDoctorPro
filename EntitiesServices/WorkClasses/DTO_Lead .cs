using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Lead
    {
        public int LEAD_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAD_DT_ENTRADA { get; set; }
        public string LEAD_NM_NOME { get; set; }
        public string LEAD_EM_EMAIL { get; set; }
        public string LEAD_NR_CELULAR { get; set; }
        public Nullable<int> LEAD_IN_STATUS { get; set; }
        public string LEAD_NR_CPF { get; set; }
        public string LEAD_NR_CNPJ { get; set; }
        public Nullable<System.DateTime> LEAD_DT_NASCIMENTO { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> LEAD_IN_SISTEMA { get; set; }
        public string LEAD_NM_ENDERECO { get; set; }
        public string LEAD_NR_NUMERO { get; set; }
        public string LEAD_NM_COMPLEMENTO { get; set; }
        public string LEAD_NM_BAIRRO { get; set; }
        public string LEAD_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string LEAD_NR_CEP { get; set; }
        public Nullable<int> LEAD_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAD_DT_DUMMY { get; set; }

    }
}
