using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Contato
    {
        public int PACO_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> GRPA_CD_ID { get; set; }
        public string PACO_NM_NOME { get; set; }
        public string PACO_EM_EMAIL { get; set; }
        public string PACO_NR_TELEFONE { get; set; }
        public string PACO_NR_CELULAR { get; set; }
        public Nullable<int> PACO_IN_ATIVO { get; set; }
    }
}
