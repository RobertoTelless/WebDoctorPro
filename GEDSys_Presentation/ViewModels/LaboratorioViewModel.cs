using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LaboratorioViewModel
    {
        [Key]
        public int LABS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string LABS_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo LINK obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O LINK deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string LABS_LK_LINK { get; set; }
        [StringLength(500, ErrorMessage = "O LINK PESSOAL deve conter no máximo 500 caracteres.")]
        public string LABS_LK_LINK_PESSOAL { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string LABS_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public int LABS_IN_ATIVO { get; set; }

        public virtual UF UF { get; set; }
    }
}