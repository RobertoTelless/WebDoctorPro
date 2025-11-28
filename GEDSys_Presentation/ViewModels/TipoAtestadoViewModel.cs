using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TipoAtestadoViewModel
    {
        [Key]
        public int TIAT_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string TIAT_NM_NOME { get; set; }
        public Nullable<int> TIAT_IN_ATIVO { get; set; }
        [StringLength(5000, ErrorMessage = "O MODELO deve conter no máximo 5000 caracteres.")]
        public string TIAT_TX_TEXTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ATESTADO> PACIENTE_ATESTADO { get; set; }
    }
}