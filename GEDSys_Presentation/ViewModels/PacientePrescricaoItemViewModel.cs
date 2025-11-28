using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacientePrescricaoItemViewModel
    {
        [Key]
        public int PAPI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE USO obrigatorio")]
        public Nullable<int> TIFO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo MEDICAMENTO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "MEDICAMENTO deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string PAPI_NM_REMEDIO { get; set; }
        [Required(ErrorMessage = "Campo POSOLOGIA obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "POSOLOGIA deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string PAPI_DS_POSOLOGIA { get; set; }
        public Nullable<int> PAPI_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "NOME GENÉRICO deve conter no máximo 250 caracteres.")]
        public string PAPI_NM_GENERICO { get; set; }
        [StringLength(250, ErrorMessage = "APRESENTAÇÃO deve conter no máximo 250 caracteres.")]
        public string PAPI_NM_APRESENTACAO { get; set; }
        [StringLength(250, ErrorMessage = "LABORATÓRIO deve conter no máximo 250 caracteres.")]
        public string PAPI_NM_LABORATORIO { get; set; }
        public Int32? MEDI_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "QUANTIDADE deve conter no máximo 50 caracteres.")]
        public string PAPI_NR_QUANTIDADE { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual PACIENTE_PRESCRICAO PACIENTE_PRESCRICAO { get; set; }
        public virtual TIPO_FORMA TIPO_FORMA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}