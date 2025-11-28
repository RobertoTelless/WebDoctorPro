using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoPrecoVendaViewModel
    {
        [Key]
        public int PRPV_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PRPV_DT_PRECO_VENDA { get; set; }
        [Required(ErrorMessage = "Campo PREÇO DE VENDA obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O PREÇO DE VENDA deve ser um valor numérico positivo")]
        public Nullable<decimal> PRPV_VL_PRECO_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O DESCONTO deve ser um valor numérico positivo")]
        public Nullable<decimal> PRPV_PC_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O PREÇO DE PROMOÇÃO deve ser um valor numérico positivo")]
        public Nullable<decimal> PRPV_VL_PRECO_PROMOCAO { get; set; }
        public int PRPV_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> TIEM_CD_ID { get; set; }
        public Nullable<decimal> PRPV_VL_PRECO_EMBALAGEM { get; set; }

        public Nullable<decimal> Embalagem { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
        public virtual TIPO_EMBALAGEM TIPO_EMBALAGEM { get; set; }
    }
}