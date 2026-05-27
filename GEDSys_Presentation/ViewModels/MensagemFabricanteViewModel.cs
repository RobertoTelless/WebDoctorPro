using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MensagemFabricanteViewModel
    {
        [Key]
        public int MEFA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CADASTRO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime MEFA_DT_CADASTRO { get; set; }
        [Required(ErrorMessage = "Campo VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime MEFA_DT_VALIDADE { get; set; }
        public int MEFA_IN_ATIVO { get; set; }
        public string MEFA_AQ_ARQUIVO { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 caracteres e no máximo 250 caracteres.")]
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        public string MEFA_NM_TITULO { get; set; }
        public Nullable<int> MEFA_IN_SISTEMA { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> MEFA_IN_TIPO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 caracteres e no máximo 1000 caracteres.")]
        public string MEFA_TX_TEXTO { get; set; }
        public string MEFA_LK_LINK { get; set; }
        public string MEFA_AQ_TAG { get; set; }
        public string MEFA_NM_LINK { get; set; }
        public string MEFA_AQ_VERSAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_FABRICANTE_LIDO> MENSAGEM_FABRICANTE_LIDO { get; set; }



    }
}