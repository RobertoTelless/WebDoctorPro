using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicoAnotacaoViewModel
    {
        [Key]
        public int MEAT_CD_ID { get; set; }
        public int MEEV_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MEAT_DT_ANOTACAO { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string MEAT_TX_ANOTACAO { get; set; }
        public int MEAT_IN_ATIVO { get; set; }

        public virtual MEDICOS_ENVIO MEDICOS_ENVIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}