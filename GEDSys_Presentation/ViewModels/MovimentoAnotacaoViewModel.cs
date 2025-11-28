using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MovimentoAnotacaoViewModel
    {
        [Key]
        public int MOAN_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> MOEP_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DA ANOTAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DA ANOTAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> MOAN_DT_ANOTACAO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string MOAN_DS_ANOTACA_ { get; set; }
        public Nullable<int> MOAN_IN_ATIVO { get; set; }

        public virtual MOVIMENTO_ESTOQUE_PRODUTO MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}