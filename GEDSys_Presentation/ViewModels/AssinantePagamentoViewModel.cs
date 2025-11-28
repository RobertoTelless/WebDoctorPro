using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AssinantePagamentoViewModel
    {
        [Key]
        public int ASPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE PAGAMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE PAGAMEMTO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_PAGAMENTO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ASPA_VL_VALOR { get; set; }
        [Required(ErrorMessage = "Campo PLANO obrigatorio")]
        public Nullable<int> PLAN_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DO PRÓXIMO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_PROXIMO { get; set; }
        public Nullable<int> ASPA_IN_ATIVO { get; set; }
        public String NOME_PLANO { get; set; }
        public String PERIODICIDADE { get; set; }
        public Nullable<System.DateTime> INICIO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VENCIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VENCIMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_VENCIMENTO { get; set; }
        public Nullable<int> ASPA_NR_ATRASO { get; set; }
        [Required(ErrorMessage = "Campo VALOR PAGO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ASPA_VL_VALOR_PAGO { get; set; }
        public Nullable<int> ASPA_IN_PAGO { get; set; }
        public String MENSAGEM { get; set; }
        public String OBSERVACOES { get; set; }
        public Nullable<int> ASPA_IN_INFORMADO { get; set; }
        public Nullable<int> ASPA_IN_SISTEMA { get; set; }
        public Nullable<int> ASPA_IN_TIPO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DA INFORMAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_INFORMA { get; set; }
        [StringLength(500, ErrorMessage = "AS INFORMAÇÔES DE PAGAMENTO devem conter no máximo 500 caracteres.")]
        public string ASPA_DS_INFORMACOES { get; set; }
        public Nullable<int> PLAS_CD_ID { get; set; }

        public string ValorPagto
        {
            get
            {
                return ASPA_VL_VALOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(ASPA_VL_VALOR.Value) : "0,0";
            }
            set
            {
                ASPA_VL_VALOR = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ValorPago
        {
            get
            {
                return ASPA_VL_VALOR_PAGO.HasValue ? CrossCutting.Formatters.DecimalFormatter(ASPA_VL_VALOR_PAGO.Value) : "0,0";
            }
            set
            {
                ASPA_VL_VALOR_PAGO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public bool Pago
        {
            get
            {
                if (ASPA_IN_PAGO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                ASPA_IN_PAGO = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PAGAMENTO_ANEXO> ASSINANTE_PAGAMENTO_ANEXO { get; set; }
        public virtual PLANO PLANO { get; set; }
        public virtual PLANO_ASSINATURA PLANO_ASSINATURA { get; set; }

    }
}