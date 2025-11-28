using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteAnotacaoViewModel
    {
        [Key]
        public int PAAN_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PAAN_DT_ANOTACAO { get; set; }
        public int USUA_CD_ID { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string PAAN_TX_ANOTACAO { get; set; }
        public int PAAN_IN_ATIVO { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual PACIENTE PACIENTE1 { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}