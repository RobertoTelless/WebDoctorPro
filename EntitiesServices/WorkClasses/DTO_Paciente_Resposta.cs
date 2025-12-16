using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Resposta
    {
        public int RECO_CD_ID { get; set; }
        public Nullable<System.DateTime> RECO_DT_DATA { get; set; }
        public string RECO_NM_NOME { get; set; }
        public string RECO_EM_EMAIL { get; set; }
        public string RECO_NR_CELULAR { get; set; }
        public string RECO_TX_MOTIVO { get; set; }
        public Nullable<int> RECO_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public string RECO_NR_CPF { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> RECO_IN_VISTO { get; set; }
        public Nullable<int> RECO_IN_ANAMNESE { get; set; }

    }
}
