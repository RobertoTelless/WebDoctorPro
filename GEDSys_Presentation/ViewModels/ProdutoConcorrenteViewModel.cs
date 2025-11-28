using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoConcorrenteViewModel
    {
        [Key]
        public int PRPF_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME DO CONCORRENTE obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "NOME com caracteres inválidos")]
        public string PRPF_NM_CONCORRENTE { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PRPF_VL_PRECO_CONCORRENTE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PRPF_DT_CADASTRO { get; set; }
        public Nullable<int> PRPF_IN_ATIVO { get; set; }
        public Nullable<int> PRPF_IN_SISTEMA { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
    }
}