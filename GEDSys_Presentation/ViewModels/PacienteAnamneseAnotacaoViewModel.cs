using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteAnamneseAnotacaoViewModel
    {
        [Key]
        public int PAAA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PAAM_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAA_DT_ANOTACAO { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string PAAA_TX_ANOTACAO { get; set; }
        public int PAAA_IN_ATIVO { get; set; }

        public virtual PACIENTE_ANAMNESE PACIENTE_ANAMNESE { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}