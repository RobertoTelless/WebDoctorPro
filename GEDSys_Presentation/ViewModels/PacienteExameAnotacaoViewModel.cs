using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteExameAnotacaoViewModel
    {
        [Key]
        public int PAET_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public int PAEX_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAET_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string PAET_TX_ANOTACAO { get; set; }
        public Nullable<int> PAET_IN_ATIVO { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual PACIENTE_EXAMES PACIENTE_EXAMES { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}