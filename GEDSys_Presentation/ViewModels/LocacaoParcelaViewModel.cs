using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LocacaoParcelaViewModel
    {
        [Key]
        public int LOPA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOPA_DT_VENCIMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOPA_DT_PAGAMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOPA_VL_VALOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOPA_VL_VALOR_PAGO { get; set; }
        public Nullable<int> LOPA_IN_STATUS { get; set; }
        public Nullable<int> LOPA_IN_ATRASO { get; set; }
        public int LOPA_IN_ATIVO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> LOPA_IN_PARCELA { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250 caracteres.")]
        public string LOPA_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOPA_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOPA_VL_JUROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOPA_VL_TAXAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> LOPA_IN_QUITADA { get; set; }
        public Nullable<int> LOPA_NR_PACELAS { get; set; }
        [StringLength(10, ErrorMessage = "O NÚMERO DA PARCELA deve conter no máximo 10 caracteres.")]
        public string LOPA_NM_PARCELAS { get; set; }
        public Nullable<int> LOPA_IN_LANCAMENTO { get; set; }
        public Nullable<int> FORMA_RECEBE { get; set; }

        public string ValorPago
        {
            get
            {
                return LOPA_VL_VALOR_PAGO.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOPA_VL_VALOR_PAGO.Value) : string.Empty;
            }
            set
            {
                LOPA_VL_VALOR_PAGO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string ValorDesconto
        {
            get
            {
                return LOPA_VL_DESCONTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOPA_VL_DESCONTO.Value) : string.Empty;
            }
            set
            {
                LOPA_VL_DESCONTO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Juros
        {
            get
            {
                return LOPA_VL_JUROS.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOPA_VL_JUROS.Value) : string.Empty;
            }
            set
            {
                LOPA_VL_JUROS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Taxas
        {
            get
            {
                return LOPA_VL_TAXAS.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOPA_VL_TAXAS.Value) : string.Empty;
            }
            set
            {
                LOPA_VL_TAXAS = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        public string Valor
        {
            get
            {
                return LOPA_VL_VALOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOPA_VL_VALOR.Value) : string.Empty;
            }
            set
            {
                LOPA_VL_VALOR = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public string Status
        {
            get
            {
                if (LOPA_IN_STATUS == 1)
                {
                    return "Liquidado";
                }
                if (LOPA_IN_STATUS == 2)
                {
                    return "Atrasado";
                }
                return "Em Aberto";
            }
        }

        public string Quitada
        {
            get
            {
                if (LOPA_IN_QUITADA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string Lancamento
        {
            get
            {
                if (LOPA_IN_LANCAMENTO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [JsonIgnore]
        public virtual LOCACAO LOCACAO { get; set; }
    }
}