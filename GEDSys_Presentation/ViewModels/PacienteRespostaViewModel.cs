using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteRespostaViewModel
    {
        [Key]
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
        public Nullable<int> PACO_CD_ID { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
    }
}