using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Usuario
    {
        public int USUA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public int PERF_CD_ID { get; set; }
        public Nullable<int> CAUS_CD_ID { get; set; }
        public Nullable<int> CARG_CD_ID { get; set; }
        public Nullable<int> DEPT_CD_ID { get; set; }
        public string USUA_NM_NOME { get; set; }
        public string USUA_NM_LOGIN { get; set; }
        public string USUA_NM_EMAIL { get; set; }
        public string USUA_NR_MATRICULA { get; set; }
        public string USUA_NR_TELEFONE { get; set; }
        public string USUA_NR_CELULAR { get; set; }
        public string USUA_NR_WHATSAPP { get; set; }
        public string USUA_NM_SENHA { get; set; }
        public string USUA_NM_SENHA_CONFIRMA { get; set; }
        public string USUA_NM_NOVA_SENHA { get; set; }
        public Nullable<int> USUA_IN_BLOQUEADO { get; set; }
        public Nullable<int> USUA_IN_PROVISORIO { get; set; }
        public Nullable<int> USUA_IN_LOGIN_PROVISORIO { get; set; }
        public Nullable<int> USUA_IN_SISTEMA { get; set; }
        public Nullable<int> USUA_IN_ATIVO { get; set; }
        public Nullable<int> USUA_IN_LOGADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_BLOQUEADO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ALTERACAO { get; set; }
        public Nullable<System.DateTime> USUA_DT_TROCA_SENHA { get; set; }
        public Nullable<System.DateTime> USUA_DT_ACESSO { get; set; }
        public Nullable<System.DateTime> USUA_DT_ULTIMA_FALHA { get; set; }
        public Nullable<System.DateTime> USUA_DT_CADASTRO { get; set; }
        public Nullable<int> USUA_NR_ACESSOS { get; set; }
        public Nullable<int> USUA_NR_FALHAS { get; set; }
        public string USUA_AQ_FOTO { get; set; }
        public string USUA_NR_CPF { get; set; }
        public string USUA_NR_RG { get; set; }
        public string USUA_TX_OBSERVACOES { get; set; }
        public Nullable<System.TimeSpan> USUA_TM_INICIO { get; set; }
        public Nullable<System.TimeSpan> USUA_TM_FINAL { get; set; }
        public Nullable<int> USUA_IN_COMPRADOR { get; set; }
        public Nullable<int> USUA_IN_APROVADOR { get; set; }
        public Nullable<int> USUA_IN_ESPECIAL { get; set; }
        public Nullable<int> USUA_IN_CRM { get; set; }
        public Nullable<int> USUA_IN_ERP { get; set; }
        public Nullable<int> USUA_IN_TECNICO { get; set; }
        public Nullable<int> USUA_IN_GERAL { get; set; }
        public Nullable<System.DateTime> USUA_DT_CODIGO { get; set; }
        public Nullable<int> USUA_IN_PENDENTE_CODIGO { get; set; }
        public string USUA_SG_CODIGO { get; set; }
        public string USUA_NM_SENHA_HASH { get; set; }
        public string USUA_NM_SALT_HASH { get; set; }
        public byte[] USUA_NM_SALT { get; set; }
        public Nullable<int> USUA_IN_AVISO_PAGAMENTO { get; set; }
        public Nullable<int> EMFI_CD_ID { get; set; }
        public Nullable<int> USUA_IN_FILIAIS { get; set; }
        public Nullable<int> USUA_IN_HUMANO { get; set; }
        public Nullable<int> USUA_IN_VENDEDOR { get; set; }
        public Nullable<int> USUA_IN_COMISSAO { get; set; }
        public Nullable<int> TICL_CD_ID { get; set; }
        public string USUA_NR_CLASSE { get; set; }
        public string USUA_NM_APELIDO { get; set; }
        public string USUA_NM_ESPECIALIDADE { get; set; }
        public string USUA_NM_PREFIXO { get; set; }
        public string USUA_NM_SUFIXO { get; set; }
        public Nullable<int> ESPE_CD_ID { get; set; }
        public Nullable<int> USUA_IN_CONSULTA { get; set; }
        public Nullable<int> USUA_IN_INDICA { get; set; }
        public string USUA_NR_CHAVE_PIX { get; set; }
    }
}
