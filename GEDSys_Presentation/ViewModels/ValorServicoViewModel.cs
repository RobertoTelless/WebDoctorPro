using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ValorServicoViewModel
    {
        [Key]
        public int VASE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE REFERÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REFERÊNCIA deve ser uma data válida")]
        public Nullable<System.DateTime> VASE_DT_REFERENCIA { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VASE_NR_VALOR { get; set; }
        [Required(ErrorMessage = "Campo DESCONTO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VASE_NR_DESCONTO { get; set; }
        public int VASE_IN_ATIVO { get; set; }

        public virtual TIPO_SERVICO_CONSULTA TIPO_SERVICO_CONSULTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONSULTA_RECEBIMENTO> CONSULTA_RECEBIMENTO { get; set; }
    }
}