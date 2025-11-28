using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicoViewModel
    {
        public int MEDC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 e no máximo 100 caracteres.")]
        public string MEDC_NM_MEDICO { get; set; }
        [Required(ErrorMessage = "Campo CRM obrigatorio")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "O CRM deve ter no minimo 1 e no máximo 15 caracteres.")]
        public string MEDC_NR_CRM { get; set; }
        public Nullable<int> ESPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(150, ErrorMessage = "O E-MAIL deve ter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string MEDC_EM_EMAIL { get; set; }
        [StringLength(50, ErrorMessage = "O CELULAR deve ter no máximo 50 caracteres.")]
        public string MEDC_NR_CELULAR { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve ter no máximo 50 caracteres.")]
        public string MEDC_NR_TELEFONE { get; set; }
        public string MEDC_GU_IDENTIFICADOR { get; set; }
        public int MEDC_IN_ATIVO { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve ter no máximo 50 caracteres.")]
        public string MEDC_NM_ENDERECO { get; set; }
        [StringLength(10, ErrorMessage = "O NUMERO deve ter no máximo 10 caracteres.")]
        public string MEDC_NR_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "O COMPLEMENTO deve ter no máximo 20 caracteres.")]
        public string MEDC_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve ter no máximo 50 caracteres.")]
        public string MEDC_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve ter no máximo 50 caracteres.")]
        public string MEDC_NM_CIDADE { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres.")]
        public string MEDC_NR_CEP { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }

        public virtual ESPECIALIDADE ESPECIALIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEDICOS_ENVIO> MEDICOS_ENVIO { get; set; }
        public virtual UF UF { get; set; }

    }
}