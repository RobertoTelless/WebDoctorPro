using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MovimentoEstoqueProdutoViewModel
    {
        [Key]
        public int MOEP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public Nullable<int> PROD_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DO MOVIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime MOEP_DT_MOVIMENTO { get; set; }
        public Nullable<int> MOEP_IN_ULTIMO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE MOVIMENTO obrigatorio")]
        public int MOEP_IN_TIPO_MOVIMENTO { get; set; }
        public int MOEP_QN_QUANTIDADE { get; set; }
        public Nullable<decimal> MOEP_VL_QUANTIDADE_ANTERIOR { get; set; }
        public string MOEP_IN_ORIGEM { get; set; }
        public int MOEP_IN_CHAVE_ORIGEM { get; set; }
        public int MOEP_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo JUSTIFICATIVA obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "A JUSTIFICATIVA deve conter no minimo 1 e no máximo 1000 caracteres.")]
        public string MOEP_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> MOEP_QN_ANTES { get; set; }
        public Nullable<int> MOEP_QN_DEPOIS { get; set; }
        public Nullable<int> MOEP_QN_ALTERADA { get; set; }
        public Nullable<int> MOEP_IN_OPERACAO { get; set; }
        public Nullable<int> EMFI_CD_ID { get; set; }
        public Nullable<int> MOEP_EMFI_CD_ID { get; set; }
        public Nullable<System.DateTime> MOEP_DT_DATA_DUMMY { get; set; }
        public Nullable<int> MOEP_IN_TIPO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_DATA_DUMMY_1 { get; set; }
        public Nullable<int> MOEP_EMFI_CD_ID_ALVO { get; set; }
        public Nullable<int> CRPV_CD_ID { get; set; }
        public Nullable<int> COBA_CD_ID { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "A OBSERVAÇÃO deve conter no minimo 1 e no máximo 1000 caracteres.")]
        public string MOEP_DS_MANUTENCAO_OBSERVACAO { get; set; }
        public Nullable<int> MOEP_IN_PENDENTE { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR { get; set; }
        public Nullable<int> MOEP_IN_TIPO_LANCAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MOEP_DT_LANCAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MOEP_DT_PAGAMENTO { get; set; }
        public Nullable<System.DateTime> MOEP_DT_AUTORIZACAO { get; set; }
        public string MOEP_DS_PRODUTO { get; set; }
        public string MOEP_DS_FILIAL { get; set; }
        public Nullable<System.DateTime> MOEP_DT_EXCLUSAO { get; set; }
        public Nullable<int> MOEP_IN_EXCLUIDOR { get; set; }
        public string MOEP_GU_GUID { get; set; }
        public Nullable<int> MOEP_IN_SISTEMA { get; set; }
        public Nullable<int> COPA_CD_ID_1 { get; set; }

        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO_1 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO_2 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO_3 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO_4 { get; set; }

        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_1 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_2 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_3 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_4 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_8 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_5 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_6 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_7 { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO_T { get; set; }

        public Nullable<int> FOPA_CD_ID_1 { get; set; }
        public Nullable<int> FORN_CD_ID_1 { get; set; }

        public Nullable<int> MOEP_IN_AUTORIZADOR_4 { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR_8 { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR_5 { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR_6 { get; set; }
        public Nullable<int> MOEP_IN_AUTORIZADOR_T { get; set; }

        public Nullable<decimal> MOEP_VL_VALOR_MOVIMENTO { get; set; }
        public Nullable<decimal> MOEP_VL_QUANTIDADE_MOVIMENTO { get; set; }

        public string MOEP_PW_SENHA { get; set; }

        public Nullable<int> MOEP_IN_TIPO_LANCAMENTO_1 { get; set; }

        public Nullable<int> MOEP_IN_TIPO_1 { get; set; }
        [StringLength(100, ErrorMessage = "O FORNECEDOR deve conter no máximo 100 caracteres.")]
        public string MOEP_NM_FORNECEDOR { get; set; }

        public String TipoMovimento
        {
            get
            {
                if (MOEP_IN_TIPO_MOVIMENTO == 1)
                {
                    return "Entrada";
                }
                if (MOEP_IN_TIPO_MOVIMENTO == 2)
                {
                    return "Saída";
                }
                if (MOEP_IN_TIPO_MOVIMENTO == 3)
                {
                    return "Transferência";
                }
                return "-";
            }
        }

        public String TipoEntradaSaida
        {
            get
            {
                if (MOEP_IN_TIPO == 1)
                {
                    return "Compra Manual";
                }
                if (MOEP_IN_TIPO == 2)
                {
                    return "Devolução de Venda";
                }
                if (MOEP_IN_TIPO == 3)
                {
                    return "Retorno de Manutenção";
                }
                if (MOEP_IN_TIPO == 4)
                {
                    return "Ajuste Manual - Entrada";
                }
                if (MOEP_IN_TIPO == 5)
                {
                    return "Descarte";
                }
                if (MOEP_IN_TIPO == 6)
                {
                    return "Perda";
                }
                if (MOEP_IN_TIPO == 7)
                {
                    return "Envio para Manutenção";
                }
                if (MOEP_IN_TIPO == 8)
                {
                    return "Ajuste Manual - Saída";
                }
                return "-";
            }
        }

        public String Pendente
        {
            get
            {
                if (MOEP_IN_PENDENTE == 1)
                {
                    return "Pendente";
                }
                if (MOEP_IN_PENDENTE == 2)
                {
                    return "Excluída";
                }
                return "Aprovado";
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
        public virtual EMPRESA_FILIAL EMPRESA_FILIAL { get; set; }
        public virtual EMPRESA_FILIAL EMPRESA_FILIAL1 { get; set; }
        public virtual EMPRESA_FILIAL EMPRESA_FILIAL2 { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ANOTACAO> MOVIMENTO_ANOTACAO { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
        public virtual USUARIO USUARIO2 { get; set; }
        public virtual CONSULTA_PAGAMENTO CONSULTA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_HISTORICO> PRODUTO_ESTOQUE_HISTORICO { get; set; }
    }
}