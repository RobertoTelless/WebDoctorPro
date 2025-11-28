using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class SubCategoriaProdutoViewModel
    {
        [Key]
        public int SCPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 no máximo 50 caracteres.")]
        [RegularExpression(@"^([a-zA-Zà-úÀ-Ú0-9@#$%&*]|-|_|\s)+$$", ErrorMessage = "SUBCATEGORIA com caracteres inválidos")]
        public string SCPR_NM_NOME { get; set; }
        public Nullable<int> SCPR_IN_ATIVO { get; set; }
        public Nullable<int> SCPR_IN_TIPO { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> SCPR_IN_SISTEMA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PECA> PECA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }

    }
}