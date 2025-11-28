using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoCustoViewModel
    {
        [Key]
        public int PRCU_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PRCU_DT_CUSTO { get; set; }
        [Required(ErrorMessage = "Campo CUSTO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O CUSTO deve ser um valor numérico positivo")]
        public Nullable<decimal> PRCU_VL_CUSTO { get; set; }
        public int PRCU_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
    }
}