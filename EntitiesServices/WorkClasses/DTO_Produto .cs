using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Produto
    {
        public int PROD_CD_ID { get; set; }
        public string PROD_NM_NOME { get; set; }
        public int PROD_IN_ATIVO { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> CAPR_CD_ID { get; set; }
        public Nullable<int> SCPR_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> TIEM_CD_ID { get; set; }
        public Nullable<int> PROD_IN_USUARIO_ALTERACAO { get; set; }
        public Nullable<System.DateTime> PROD_DT_CADASTRO { get; set; }
        public Nullable<System.DateTime> PROD_DT_ALTERACAO { get; set; }
        public Nullable<int> PROD_IN_TIPO_PRODUTO { get; set; }
        public Nullable<int> PROD_IN_COMPOSTO { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        public string PROD_CD_CODIGO { get; set; }
        public string PROD_NR_BARCODE { get; set; }
        public Nullable<int> PROD_IN_FRACIONADO { get; set; }
        public string PROD_DS_DESCRICAO { get; set; }
        public string PROD_DS_INFORMACOES { get; set; }
        public string PROD_NR_REFERENCIA { get; set; }
        public string PROD_NM_MARCA { get; set; }
        public string PROD_NM_MODELO { get; set; }
        public string PROD_NM_REFERENCIA_FABRICANTE { get; set; }
        public string PROD_NM_FABRICANTE { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_ATUAL { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_MAXIMO { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_MINIMO { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_RESERVA { get; set; }
        public Nullable<decimal> PROD_VL_MEDIA_VENDA_MENSAL { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        public Nullable<decimal> PROD_VL_CUSTO { get; set; }
        public Nullable<decimal> PROD_VL_CVM_RECEITA { get; set; }
        public Nullable<decimal> PROD_VL_CVM_UNITARIO { get; set; }
        public Nullable<decimal> PROD_VL_CVM_PESO { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_MINIMO { get; set; }
        public Nullable<decimal> PROD_PC_DESCONTO { get; set; }
        public Nullable<decimal> PROD_VL_ULTIMO_CUSTO { get; set; }
        public Nullable<decimal> PROD_VL_PRECO_ANTERIOR { get; set; }
        public Nullable<decimal> PROD_VL_CUSTO_CONCORRENTE_MEDIO { get; set; }
        public Nullable<decimal> PROD_VL_MARGEM_CONTRIBUICAO { get; set; }
        public Nullable<decimal> PROD_VL_FATOR_CORRECAO { get; set; }
        public string PROD_TX_OBSERVACOES { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_CUSTO { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_VENDA { get; set; }
        public Nullable<int> PROD_IN_PECA { get; set; }
        public Nullable<decimal> PROD_VL_ESTOQUE_TOTAL { get; set; }
        public Nullable<int> PROD_IN_SISTEMA { get; set; }
        public string PROD_NM_FORNECEDOR { get; set; }
        public Nullable<int> PROD_IN_LOCACAO { get; set; }
        public Nullable<decimal> PROD_VL_LOCACAO { get; set; }
        public Nullable<decimal> PROD_VL_LOCACAO_PROMOCAO { get; set; }
        public Nullable<decimal> PROD_VL_LOCACAO_MULTA { get; set; }
        public Nullable<decimal> PROD_VL_LOCACAO_TAXAS { get; set; }
    }
}
