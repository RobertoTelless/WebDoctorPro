using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBCATEGORIA obrigatorio")]
        public int SCPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public Nullable<int> PROD_IN_TIPO_PRODUTO { get; set; }
        [StringLength(30, ErrorMessage = "O CÓDIGO deve conter no máximo 30 caracteres.")]
        public string PROD_CD_CODIGO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PROD_NM_NOME { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 1000 caracteres.")]
        public string PROD_DS_DESCRICAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES devem conter no máximo 1000 caracteres.")]
        public string PROD_DS_INFORMACOES { get; set; }
        [StringLength(50, ErrorMessage = "O FABRICANTE deve conter no máximo 50 caracteres.")]
        public string PROD_NM_FABRICANTE { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MARCA { get; set; }
        [StringLength(50, ErrorMessage = "O MODELO deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MODELO { get; set; }
        [StringLength(30, ErrorMessage = "O CÓDIGO DE BARRAS deve conter no máximo 30 caracteres.")]
        public string PROD_NR_BARCODE { get; set; }
        [StringLength(50, ErrorMessage = "A REFERENCIA deve conter no máximo 50 caracteres.")]
        public string PROD_NR_REFERENCIA { get; set; }
        [StringLength(50, ErrorMessage = "A REFERENCIA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_REFERENCIA_FABRICANTE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ULTIMO_CUSTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_MINIMO { get; set; }
        public int PROD_IN_AVISA_MINIMO { get; set; }
        public Nullable<System.DateTime> PROD_DT_CADASTRO { get; set; }
        public Nullable<int> PROD_IN_ATIVO { get; set; }
        [StringLength(5000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 5000 caracteres.")]
        public string PROD_TX_OBSERVACOES { get; set; }
        public Nullable<int> TIEM_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_PC_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_ANTERIOR { get; set; }
        public Nullable<decimal> PROD_VL_CUSTO_CONCORRENTE_MEDIO { get; set; }
        public Nullable<decimal> PROD_VL_MARGEM_CONTRIBUICAO { get; set; }
        [Required(ErrorMessage = "Campo COMPOSTO obrigatorio")]
        public Nullable<int> PROD_IN_COMPOSTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_CVM_RECEITA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_CVM_UNITARIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_CVM_PESO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_FATOR_CORRECAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_ATUAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_MAXIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_MINIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_RESERVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_MEDIA_VENDA_MENSAL { get; set; }
        public Nullable<decimal> CustoKit { get; set; }
        public Nullable<System.DateTime> PROD_DT_ALTERACAO { get; set; }
        public Nullable<int> PROD_IN_USUARIO_ALTERACAO { get; set; }
        public Nullable<int> PROD_IN_PECA { get; set; }
        [Required(ErrorMessage = "Campo FRACIONADO obrigatorio")]
        public Nullable<int> PROD_IN_FRACIONADO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_FILIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_RESERVA_FILIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_CUSTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_ESTOQUE_TOTAL { get; set; }
        [StringLength(150, ErrorMessage = "O FORnECEDOR deve conter no máximo 150 caracteres.")]
        public string PROD_NM_FORNECEDOR { get; set; }
        public Nullable<int> PROD_IN_SISTEMA { get; set; }
        public Nullable<int> PROD_IN_LOCACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_LOCACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_LOCACAO_PROMOCAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_LOCACAO_MULTA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_LOCACAO_TAXAS { get; set; }

        public string UltimoCusto
        {
            get
            {
                return PROD_VL_ULTIMO_CUSTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_ULTIMO_CUSTO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_ULTIMO_CUSTO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoVenda
        {
            get
            {
                return PROD_VL_PRECO_VENDA.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_VENDA.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_VENDA = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoPromocao
        {
            get
            {
                return PROD_VL_PRECO_PROMOCAO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_PROMOCAO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_PROMOCAO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoMinimo
        {
            get
            {
                return PROD_VL_PRECO_MINIMO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_MINIMO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_MINIMO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }

        public bool AvisaMinima 
        {
            get
            {
                if (PROD_IN_AVISA_MINIMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_AVISA_MINIMO = (value == true) ? 1 : 0;
            }
        }

        public String Tipo
        {
            get
            {
                if (PROD_IN_TIPO_PRODUTO == 1)
                {
                    return "Material";
                }
                return "Produto";
            }
        }

        public String Fracionado
        {
            get
            {
                if (PROD_IN_FRACIONADO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public String Composto
        {
            get
            {
                if (PROD_IN_COMPOSTO == 1)
                {
                    return "Composto";
                }
                return "Simples";
            }
        }

        public string PrecoAnterior
        {
            get
            {
                return PROD_VL_PRECO_ANTERIOR.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_PRECO_ANTERIOR.Value) : string.Empty;
            }
            set
            {
                PROD_VL_PRECO_ANTERIOR = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string Desconto
        {
            get
            {
                return PROD_PC_DESCONTO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_PC_DESCONTO.Value) : string.Empty;
            }
            set
            {
                PROD_PC_DESCONTO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string PrecoConcorrente
        {
            get
            {
                return PROD_VL_CUSTO_CONCORRENTE_MEDIO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_CUSTO_CONCORRENTE_MEDIO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_CUSTO_CONCORRENTE_MEDIO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string CustoReceita
        {
            get
            {
                return PROD_VL_CVM_RECEITA.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_CVM_RECEITA.Value) : string.Empty;
            }
            set
            {
                PROD_VL_CVM_RECEITA = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public Decimal Diferenca
        {
            get
            {
                decimal dif = 0;
                if (PROD_VL_CUSTO_CONCORRENTE_MEDIO != null)
                {
                    if (PROD_VL_PRECO_VENDA > 0 & PROD_VL_CUSTO_CONCORRENTE_MEDIO.Value > 0)
                    {
                        dif = 100 - ((PROD_VL_PRECO_VENDA.Value * 100) / PROD_VL_CUSTO_CONCORRENTE_MEDIO.Value);
                    }
                }
                return dif;
            }
        }
        public string CustoMedioConcorrencia
        {
            get
            {
                return PROD_VL_CUSTO_CONCORRENTE_MEDIO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_CUSTO_CONCORRENTE_MEDIO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_CUSTO_CONCORRENTE_MEDIO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string CustoUnitario
        {
            get
            {
                return PROD_VL_CVM_UNITARIO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_CVM_UNITARIO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_CVM_UNITARIO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }
        public string CustoPeso
        {
            get
            {
                return PROD_VL_CVM_PESO.HasValue ? CrossCutting.Formatters.DecimalFormatter(PROD_VL_CVM_PESO.Value) : string.Empty;
            }
            set
            {
                PROD_VL_CVM_PESO = value != null ? Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true)) : 0;
            }
        }

        public string EstoqueCusto
        {
            get
            {
                Decimal? estoque = 0;
                if (PROD_VL_ULTIMO_CUSTO != null & PROD_VL_ESTOQUE_ATUAL != null)
                {
                    estoque = PROD_VL_ESTOQUE_ATUAL * PROD_VL_ULTIMO_CUSTO;
                }
                return CrossCutting.Formatters.DecimalFormatter(estoque.Value);
            }
        }

        public string EstoqueVenda
        {
            get
            {
                Decimal? estoque = 0;
                if (PROD_VL_PRECO_VENDA != null & PROD_VL_ESTOQUE_ATUAL != null)
                {
                    estoque = PROD_VL_ESTOQUE_ATUAL * PROD_VL_PRECO_VENDA;
                }
                return CrossCutting.Formatters.DecimalFormatter(estoque.Value);
            }
        }

        public Decimal Esgotamento
        {
            get
            {
                Decimal? dias = 0;
                if (PROD_VL_MEDIA_VENDA_MENSAL > 0 & PROD_VL_ESTOQUE_ATUAL > 0)
                {
                    dias = Convert.ToDecimal((PROD_VL_ESTOQUE_ATUAL / PROD_VL_MEDIA_VENDA_MENSAL) * 30);
                }
                return dias.Value;
            }
        }

        public string StatusEstoque
        {
            get
            {
                if (PROD_VL_ESTOQUE_ATUAL < PROD_VL_ESTOQUE_MINIMO)
                {
                    return "Abaixo do Mínimo";
                }
                if (PROD_VL_ESTOQUE_ATUAL > PROD_VL_ESTOQUE_MAXIMO)
                {
                    return "Acima do Máximo";
                }
                if (PROD_VL_ESTOQUE_ATUAL <= 0)
                {
                    return "Zerado ou Negativo";
                }
                return "Situação Normal";
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO> ATENDIMENTO { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMPRA_FORNECEDOR_ITEM> COMPRA_FORNECEDOR_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMPRA_ITEM> COMPRA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM_PECA> CRM_PEDIDO_VENDA_ITEM_PECA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA> FICHA_TECNICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_PRODUTO> FORNECEDOR_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO_PRODUTO> ORDEM_SERVICO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECIFICACAO> PRECIFICACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECIFICACAO_PRODUTO> PRECIFICACAO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANOTACAO> PRODUTO_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_CONCORRENTE> PRODUTO_CONCORRENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_CUSTO> PRODUTO_CUSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_FILIAL> PRODUTO_ESTOQUE_FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_HISTORICO> PRODUTO_ESTOQUE_HISTORICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_KIT> PRODUTO_KIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_KIT> PRODUTO_KIT1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_LOG> PRODUTO_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_PRECO_VENDA> PRODUTO_PRECO_VENDA { get; set; }
        public virtual SUBCATEGORIA_PRODUTO SUBCATEGORIA_PRODUTO { get; set; }
        public virtual TIPO_EMBALAGEM TIPO_EMBALAGEM { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICOS_PECA> SERVICOS_PECA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO> LOCACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AVISO_LEMBRETE> AVISO_LEMBRETE { get; set; }
    }
}