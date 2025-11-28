using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PagamentoViewModel
    {
        public int COPA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PAGAMENTO obrigatorio")]
        public Nullable<int> TIPA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COPA_DT_PAGAMENTO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> COPA_VL_VALOR { get; set; }
        public Nullable<int> COPA_IN_CONFERIDO { get; set; }
        public int COPA_IN_ATIVO { get; set; }
        [StringLength(150, ErrorMessage = "NOME DO PAGAMENTO deve conter no máximo 150 caracteres.")]
        public string COPA_NM_NOME { get; set; }
        public Nullable<System.DateTime> COPA_DT_DUMMY { get; set; }
        public string COPA_GU_GUID { get; set; }
        public string COPA_XM_NOTA_FISCAL { get; set; }
        [StringLength(150, ErrorMessage = "NOME DO FAVORECIDO deve conter no máximo 150 caracteres.")]
        public string COPA_NM_FAVORECIDO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VENCIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COPA_DT_VENCIMENTO { get; set; }
        public Nullable<int> COPA_IN_PAGO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> COPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> COPA_VL_MULTA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> COPA_VL_PAGO { get; set; }
        public Nullable<int> COPA_NR_ATRASO { get; set; }
        public Nullable<int> QUITA_PAGAMENTO { get; set; }
        public Nullable<int> RECURSIVO { get; set; }
        public Nullable<int> NUMERO_VEZES { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> DATA_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> DATA_FINAL { get; set; }
        public Nullable<int> PETA_CD_ID { get; set; }
        public Nullable<System.DateTime> COPA_DT_CADASTRO { get; set; }
        public Nullable<int> DIA_FIXO { get; set; }

        public String Quitado
        {
            get
            {
                if (COPA_IN_PAGO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public virtual TIPO_PAGAMENTO TIPO_PAGAMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO_ANEXO> PAGAMENTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO_ANOTACAO> PAGAMENTO_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO_NOTA_FISCAL> PAGAMENTO_NOTA_FISCAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
    }
}