using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ContratoLocacaoViewModel
    {
        [Key]
        public int COLO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE CONTRATO obrigatorio")]
        public Nullable<int> TICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COLO_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string COLO_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo TEXTO DO CONTRATO obrigatorio")]
        public string COLO_TX_TEXTO { get; set; }
        public Nullable<int> COLO_IN_ATIVO { get; set; }

        public virtual TIPO_CONTRATO TIPO_CONTRATO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO> LOCACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO> LOCACAO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO> LOCACAO2 { get; set; }
    }
}