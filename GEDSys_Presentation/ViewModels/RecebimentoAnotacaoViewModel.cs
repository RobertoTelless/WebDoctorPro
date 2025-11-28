using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class RecebimentoAnotacaoViewModel
    {
        [Key]
        public int REAT_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CORE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> REAT_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string REAT_TX_ANOTACAO { get; set; }
        public int REAT_IN_ATIVO { get; set; }

        public virtual CONSULTA_RECEBIMENTO CONSULTA_RECEBIMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}