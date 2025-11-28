using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoEstoqueFilialViewModel
    {
        [Key]
        public int PREF_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public Nullable<int> EMFI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ESTOQUE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O ESTOQUE deve ser um valor numérico positivo")]
        public Nullable<int> PREF_QN_ESTOQUE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PREF_DT_ULTIMO_MOVIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O ESTOQUE deve ser um valor numérico positivo")]
        public Nullable<int> PREF_QN_QUANTIDADE_ALTERADA { get; set; }
        public string PREF_DS_JUSTIFICATIVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O ESTOQUE RESERVADO deve ser um valor numérico positivo")]
        public Nullable<int> PREF_QN_QUANTIDADE_RESERVADA { get; set; }
        public Nullable<int> PREF_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo AUTORIZADOR obrigatorio")]
        public Nullable<int> USUA_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "O ESTOQUE TOTAL deve ser um valor numérico positivo")]
        public Nullable<int> PREF_QN_ESTOQUE_TOTAL { get; set; }

        public virtual EMPRESA_FILIAL EMPRESA_FILIAL { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}