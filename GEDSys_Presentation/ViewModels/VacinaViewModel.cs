using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VacinaViewModel
    {
        [Key]
        public int VACI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string VACI_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo FABRICANTE obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O FABRICANTE deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string VACI_NM_FABRICANTE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> VACI_NR_PERIODO { get; set; }
        public Nullable<int> VACI_IN_ATIVO { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string VACI_DS_DESCRICAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_VACINA> PACIENTE_VACINA { get; set; }

    }
}