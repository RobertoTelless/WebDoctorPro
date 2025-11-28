using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CategoriaProdutoViewModel
    {
        [Key]
        public int CAPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "CATEGORIA com caracteres inválidos")]
        public string CAPR_NM_NOME { get; set; }
        public Nullable<int> CAPR_IN_ATIVO { get; set; }

        public int CAPR_IN_FOOD { get; set; }
        public int CAPR_IN_EXPEDICAO { get; set; }
        public int CAPR_IN_SEO { get; set; }
        public int CAPR_IN_GRADE { get; set; }
        public int CAPR_IN_TAMANHO { get; set; }
        public Nullable<int> CAPR_IN_TIPO { get; set; }
        public Nullable<int> CAPR_IN_SISTEMA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBCATEGORIA_PRODUTO> SUBCATEGORIA_PRODUTO { get; set; }

    }
}